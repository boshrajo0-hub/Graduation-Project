namespace HealthCareCenter.Models
{
    public class Visit
    {
        public int VisitId { get; set; }
        public int AppointmentId { get; set; }
        public string? DoctorNotes { get; set; }
        public string? AiSummary { get; set; }
        public DateTime VisitDate { get; set; }

        public Appointment Appointment { get; set; } = null!;
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
