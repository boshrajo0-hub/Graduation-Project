using HealthCareCenter.DTOs;

namespace HealthCareCenter.Services.Interfaces
{
    public interface IAiService
    {
        Task<string> SummarizeDoctorNotesAsync(string doctorNotes, CancellationToken ct = default);
        Task<ChatResponse> AskChatbotAsync(ChatRequest request, CancellationToken ct = default);
    }
}
