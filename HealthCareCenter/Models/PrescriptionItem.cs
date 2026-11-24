namespace HealthCareCenter.Models
{
    public class PrescriptionItem
    {
        public int ItemId { get; set; }
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Dose { get; set; }
        public string? Duration { get; set; }

        public Prescription Prescription { get; set; } = null!;
    }
}
