namespace HealthCareCenter.Models
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public int PatientId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string? BotReply { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; } = null!;
    }
}
