//using System.Net.Http.Json;
//using System.Text;
//using System.Text.Json;
//using Microsoft.EntityFrameworkCore;
//using HealthCareCenter.Data;
//using HealthCareCenter.DTOs;
//using HealthCareCenter.Models;
//using HealthCareCenter.Services.Interfaces;


//namespace HealthCareCenter.Services.Implementations
//{
//    public class LlmAiService : IAiService
//    {
//        private readonly HealthCareCenterDBContext _db;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly IConfiguration _config;
//        private readonly ILogger<LlmAiService> _logger;

//        public LlmAiService(
//            HealthCareCenterDBContext db,
//            IHttpClientFactory httpClientFactory,
//            IConfiguration config,
//            ILogger<LlmAiService> logger)
//        {
//            _db = db;
//            _httpClientFactory = httpClientFactory;
//            _config = config;
//            _logger = logger;
//        }

//        public async Task<string> SummarizeDoctorNotesAsync(string doctorNotes, CancellationToken ct = default)
//        {
//            const string systemPrompt = """
//            أنت مساعد طبي إداري لنظام إدارة مجمع صحي.
//            لخّص ملاحظات الطبيب باللغة العربية بشكل منظم وواضح وفق العناوين التالية فقط:
//            - الشكوى:
//            - الملاحظات الأساسية:
//            - الخطة المقترحة:
//            لا تضف أي تشخيص جديد غير مذكور في ملاحظات الطبيب.
//            """;

//            return await CallLlmAsync(systemPrompt, doctorNotes,
//                "تعذر توليد الملخص حالياً بسبب انقطاع خدمة الذكاء الاصطناعي. يمكنك إدخال الملخص يدوياً.", ct);
//        }

//        public async Task<ChatResponse> AskChatbotAsync(ChatRequest request, CancellationToken ct = default)
//        {
//            // Fetch only non-sensitive catalog info (clinics & doctors) for guidance
//            var clinicDirectory = await _db.Doctors
//                .AsNoTracking()
//                .Include(d => d.Clinic)
//                .Select(d => $"- د. {d.FullName} | التخصص: {d.Specialty} | العيادة: {d.Clinic.Name}")
//                .ToListAsync(ct);

//            var directoryText = clinicDirectory.Count > 0
//                ? string.Join("\n", clinicDirectory)
//                : "لا توجد بيانات عيادات متاحة حالياً.";

//            var systemPrompt = $"""
//            أنت مساعد ذكي لمجمع صحي. مهمتك الإجابة عن أسئلة المرضى وتوجيههم إرشادياً إلى العيادة المناسبة فقط.
//            القواعد الصارمة:
//            1. لا تقدم تشخيصاً طبياً ولا تصف أدوية أو علاجاً.
//            2. في حال وجود أعراض خطيرة أو طارئة، نبّه المريض فوراً إلى التوجه إلى قسم الطوارئ.
//            3. التوجيه إلى العيادة إرشادي فقط.
//            4. لا تطلب أو تذكر بيانات شخصية حساسة.

//            الأطباء والعيادات المتوفرة:
//            {directoryText}
//            """;

//            var reply = await CallLlmAsync(systemPrompt, request.Message,
//                "المساعد الذكي غير متاح مؤقتاً. يرجى التواصل مع الاستقبال لحجز المواعيد أو الاستفسار.", ct);

//            var now = DateTime.UtcNow;
//            var chatRecord = new ChatMessage
//            {
//                PatientId = request.PatientId,
//                MessageText = request.Message,
//                BotReply = reply,
//                CreatedAt = now
//            };

//            _db.ChatMessages.Add(chatRecord);
//            await _db.SaveChangesAsync(ct);

//            return new ChatResponse(reply, now);
//        }

//        private async Task<string> CallLlmAsync(
//            string systemPrompt,
//            string userPrompt,
//            string fallbackMessage,
//            CancellationToken ct)
//        {
//            try
//            {
//                var endpoint = _config["AiSettings:ProviderUrl"];
//                var apiKey = _config["AiSettings:ApiKey"];
//                var timeoutSec = _config.GetValue<int?>("AiSettings:TimeoutSeconds") ?? 20;

//                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
//                    return fallbackMessage;

//                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
//                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSec));

//                var client = _httpClientFactory.CreateClient();
//                client.DefaultRequestHeaders.Authorization =
//                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

//                // OpenAI-compatible chat completions payload; adjust body/response parsing if using another provider
//                var payload = new
//                {
//                    model = _config["AiSettings:Model"] ?? "gpt-4o-mini",
//                    messages = new[]
//                    {
//                    new { role = "system", content = systemPrompt },
//                    new { role = "user", content = userPrompt }
//                },
//                    temperature = 0.2
//                };

//                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
//                using var response = await client.PostAsync(endpoint, content, cts.Token);

//                if (!response.IsSuccessStatusCode)
//                {
//                    _logger.LogWarning("LLM request failed with status {StatusCode}", response.StatusCode);
//                    return fallbackMessage;
//                }

//                using var doc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cts.Token);
//                var text = doc?.RootElement
//                    .GetProperty("choices")[0]
//                    .GetProperty("message")
//                    .GetProperty("content")
//                    .GetString();

//                return string.IsNullOrWhiteSpace(text) ? fallbackMessage : text.Trim();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error while calling external LLM service.");
//                return fallbackMessage;
//            }
//        }
//    }
//}



using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Models;
using HealthCareCenter.Services.Interfaces;

namespace HealthCareCenter.Services.Implementations
{
    public class LlmAiService : IAiService
    {
        private readonly HealthCareCenterDBContext _db;
        private readonly IAppointmentService _appointmentService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<LlmAiService> _logger;

        public LlmAiService(
            HealthCareCenterDBContext db,
            IAppointmentService appointmentService,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<LlmAiService> logger)
        {
            _db = db;
            _appointmentService = appointmentService;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<string> SummarizeDoctorNotesAsync(string doctorNotes, CancellationToken ct = default)
        {
            const string systemPrompt = """
            أنت مساعد طبي إداري لنظام إدارة مجمع صحي.
            لخّص ملاحظات الطبيب باللغة العربية بشكل منظم وواضح وفق العناوين التالية فقط:
            - الشكوى:
            - الملاحظات الأساسية:
            - الخطة المقترحة:
            لا تضف أي تشخيص جديد غير مذكور في ملاحظات الطبيب.
            """;

            return await CallLlmAsync(systemPrompt, doctorNotes,
                "تعذر توليد الملخص حالياً بسبب انقطاع خدمة الذكاء الاصطناعي. يمكنك إدخال الملخص يدوياً.", ct);
        }

        public async Task<ChatResponse> AskChatbotAsync(ChatRequest request, CancellationToken ct = default)
        {
            var doctors = await _db.Doctors
                .AsNoTracking()
                .Include(d => d.Clinic)
                .Include(d => d.Schedules)
                .ToListAsync(ct);

            var directoryText = doctors.Count > 0
                ? string.Join("\n", doctors.Select(d =>
                {
                    var schedules = d.Schedules.Count > 0
                        ? string.Join("، ", d.Schedules.Select(s => $"{s.DayOfWeek}: {s.StartTime:HH:mm}-{s.EndTime:HH:mm}"))
                        : "غير محدد";
                    return $"- [DoctorId:{d.DoctorId}] د. {d.FullName} | التخصص: {d.Specialty} | العيادة: {d.Clinic.Name} | الدوام: {schedules}";
                }))
                : "لا توجد بيانات عيادات متاحة حالياً.";

            var systemPrompt = $"""
            أنت مساعد ذكي لمجمع صحي. مهمتك الإجابة عن أسئلة المرضى (ساعات الدوام، الأطباء المتوفرون، خطوات الحجز)، ومساعدتهم في حجز موعد، وتوجيههم إرشادياً إلى العيادة المناسبة.
            القواعد الصارمة:
            1. لا تقدم تشخيصاً طبياً ولا تصف أدوية أو علاجاً.
            2. في حال وجود أعراض خطيرة أو طارئة، نبّه المريض فوراً إلى التوجه إلى قسم الطوارئ.
            3. التوجيه إلى العيادة إرشادي فقط.
            4. لا تطلب أو تذكر بيانات شخصية حساسة.
            5. خطوات الحجز: اختيار العيادة والطبيب المناسب، تحديد التاريخ والوقت ضمن ساعات دوام الطبيب (مدة الموعد 30 دقيقة)، ثم تأكيد الحجز.
            6. إذا طلب المريض حجز موعد وحدد الطبيب والتاريخ والوقت بوضوح، أضف في نهاية ردك السطر التالي بالضبط:
               BOOK_APPOINTMENT|doctorId=<رقم_الطبيب>|date=<YYYY-MM-DD>|time=<HH:mm>

            الأطباء والعيادات المتوفرة:
            {directoryText}
            """;

            var rawReply = await CallLlmAsync(systemPrompt, request.Message,
                "المساعد الذكي غير متاح مؤقتاً. يرجى التواصل مع الاستقبال لحجز المواعيد أو الاستفسار.", ct);

            var finalReply = rawReply;
            int? bookedAppointmentId = null;

            // Detect and execute booking command if emitted by the assistant
            var match = Regex.Match(
                rawReply,
                @"BOOK_APPOINTMENT\|doctorId=(\d+)\|date=(\d{4}-\d{2}-\d{2})\|time=(\d{2}:\d{2})",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                finalReply = Regex.Replace(rawReply, @"BOOK_APPOINTMENT\|[^\r\n]+", string.Empty).Trim();

                if (int.TryParse(match.Groups[1].Value, out var doctorId) &&
                    DateOnly.TryParse(match.Groups[2].Value, out var date) &&
                    TimeOnly.TryParse(match.Groups[3].Value, out var time))
                {
                    var booking = await _appointmentService.CreateAsync(
                        new CreateAppointmentRequest(request.PatientId, doctorId, date, time), ct);

                    if (booking.Ok && booking.Appointment is not null)
                    {
                        bookedAppointmentId = booking.Appointment.AppointmentId;
                        finalReply += $"\n\nتم تأكيد حجز موعدك بنجاح (رقم الموعد: {bookedAppointmentId}) بتاريخ {date:yyyy-MM-dd} الساعة {time:HH:mm}.";
                    }
                    else
                    {
                        finalReply += $"\n\nتعذر إتمام الحجز تلقائياً: {booking.Error}";
                    }
                }
            }

            var now = DateTime.UtcNow;
            var chatRecord = new ChatMessage
            {
                PatientId = request.PatientId,
                MessageText = request.Message,
                BotReply = finalReply,
                CreatedAt = now
            };

            _db.ChatMessages.Add(chatRecord);
            await _db.SaveChangesAsync(ct);

            return new ChatResponse(finalReply, now, bookedAppointmentId);
        }

        private async Task<string> CallLlmAsync(
            string systemPrompt,
            string userPrompt,
            string fallbackMessage,
            CancellationToken ct)
        {
            try
            {
                var endpoint = _config["AiSettings:ProviderUrl"];
                var apiKey = _config["AiSettings:ApiKey"];
                var timeoutSec = _config.GetValue<int?>("AiSettings:TimeoutSeconds") ?? 20;

                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
                    return fallbackMessage;

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSec));

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    model = _config["AiSettings:Model"] ?? "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.2
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(endpoint, content, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("LLM request failed with status {StatusCode}", response.StatusCode);
                    return fallbackMessage;
                }

                using var doc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cts.Token);
                var text = doc?.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return string.IsNullOrWhiteSpace(text) ? fallbackMessage : text.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling external LLM service.");
                return fallbackMessage;
            }
        }
    }
}
