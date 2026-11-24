namespace HealthCareCenter.Models
{
    public class Clinic
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
