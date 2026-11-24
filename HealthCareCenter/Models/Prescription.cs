namespace HealthCareCenter.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int VisitId { get; set; }
        public bool IsDispensed { get; set; }
        public DateTime? DispensedDate { get; set; }

        public Visit Visit { get; set; } = null!;
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
