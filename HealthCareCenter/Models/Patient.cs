namespace HealthCareCenter.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public User User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
