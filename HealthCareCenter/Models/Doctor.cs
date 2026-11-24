//namespace HealthCareCenter.Models
//{
//    public class Doctor
//    {
//        public int DoctorId { get; set; }
//        public int UserId { get; set; }
//        public string FullName { get; set; } = string.Empty;
//        public string Specialty { get; set; } = string.Empty;
//        public int ClinicId { get; set; }
//        public string? Phone { get; set; }

//        public User User { get; set; } = null!;
//        public Clinic Clinic { get; set; } = null!;
//        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
//    }
//}


namespace HealthCareCenter.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public string? Phone { get; set; }

        public User User { get; set; } = null!;
        public Clinic Clinic { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    }
}