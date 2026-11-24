using System.Numerics;

namespace HealthCareCenter.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Doctor, Receptionist, Pharmacist, Patient
        public bool IsActive { get; set; } = true;

        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
