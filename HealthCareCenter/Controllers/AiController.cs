using HealthCareCenter.DTOs;
using HealthCareCenter.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly IClinicalService _clinicalService;

        public AiController(IAiService aiService, IClinicalService clinicalService)
        {
            _aiService = aiService;
            _clinicalService = clinicalService;
        }

        // Step 1 for doctor: generate draft summary for review
        [HttpPost("summarize-note")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SummarizeNote(
            [FromBody] SummarizeNoteRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.DoctorNotes))
                return BadRequest(new { message = "Doctor notes cannot be empty." });

            var draftSummary = await _aiService.SummarizeDoctorNotesAsync(request.DoctorNotes, ct);
            return Ok(new { draftSummary });
        }

        // Step 2 for doctor: save approved/edited summary to Visits.AiSummary
        [HttpPut("visits/{visitId:int}/approve-summary")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ApproveSummary(
            int visitId, [FromBody] ApproveSummaryRequest request, CancellationToken ct)
        {
            var saved = await _clinicalService.ApproveSummaryAsync(visitId, request.ApprovedSummary, ct);
            if (!saved)
                return NotFound(new { message = "Visit not found." });

            return NoContent();
        }

        [HttpPost("chat")]
        [Authorize(Roles = "Patient,Receptionist,Admin")]
        public async Task<ActionResult<ChatResponse>> Chat(
            [FromBody] ChatRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { message = "Message cannot be empty." });

            var response = await _aiService.AskChatbotAsync(request, ct);
            return Ok(response);
        }
    }
}
