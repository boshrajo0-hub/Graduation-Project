//namespace HealthCareCenter.DTOs
//{
//    //public class BackendDtos
//    //{

//    //}
//    // Auth
//    public record LoginRequest(string Email, string Password);
//    public record LoginResponse(string Token, int UserId, string Email, string Role);

//    // Appointments
//    public record CreateAppointmentRequest(
//        int PatientId,
//        int DoctorId,
//        DateOnly AppointmentDate,
//        TimeOnly StartTime);

//    public record UpdateAppointmentStatusRequest(string Status); // Scheduled, Completed, Cancelled


//    public record CreateClinicRequest(string Name, string? Description);

//    public record CreateDoctorRequest(
//    int UserId,
//    string FullName,
//    string? Specialty,
//    int ClinicId,
//    string? Phone);

//    // Visits & Prescriptions
//    public record CreateVisitRequest(int AppointmentId, string? DoctorNotes);
//    public record PrescriptionItemDto(string MedicineName, string? Dose, string? Duration);
//    public record CreatePrescriptionRequest(int VisitId, List<PrescriptionItemDto> Items);

//    // Invoices
//    public record CreateInvoiceRequest(int VisitId, decimal Amount);
//    public record PayInvoiceRequest(string PaymentStatus); // Paid, Unpaid

//    // AI
//    public record SummarizeNoteRequest(string DoctorNotes);
//    public record ApproveSummaryRequest(string ApprovedSummary);
//    public record ChatRequest(int PatientId, string Message);
//    public record ChatResponse(string Reply, DateTime CreatedAt);
//}


namespace HealthCareCenter.DTOs
{
    // Auth & Users
    public record LoginRequest(string Email, string Password);
    public record LoginResponse(string Token, int UserId, string Email, string Role);
    public record CreateUserRequest(string Email, string Password, string Role, bool IsActive = true);
    public record UpdateUserRequest(string Role, bool IsActive);
    public record UserDto(int UserId, string Email, string Role, bool IsActive);

    // Patients
    public record CreatePatientRequest(
        int UserId,
        string FullName,
        DateOnly? BirthDate,
        string? Gender,
        string? Phone,
        string? Address);

    public record UpdatePatientRequest(
        string FullName,
        DateOnly? BirthDate,
        string? Gender,
        string? Phone,
        string? Address);

    // Clinics, Doctors & Schedules
    public record CreateClinicRequest(string Name, string? Description);

    public record CreateDoctorRequest(
        int UserId,
        string FullName,
        string Specialty,
        int ClinicId,
        string? Phone);

    public record UpsertDoctorScheduleRequest(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime);

    // Appointments
    public record CreateAppointmentRequest(
        int PatientId,
        int DoctorId,
        DateOnly AppointmentDate,
        TimeOnly StartTime);

    public record RescheduleAppointmentRequest(
        DateOnly AppointmentDate,
        TimeOnly StartTime);

    public record UpdateAppointmentStatusRequest(string Status); // Scheduled, Completed, Cancelled

    // Visits & Prescriptions
    public record CreateVisitRequest(int AppointmentId, string? DoctorNotes);
    public record PrescriptionItemDto(string MedicineName, string? Dose, string? Duration);
    public record CreatePrescriptionRequest(int VisitId, List<PrescriptionItemDto> Items);

    // Invoices
    public record CreateInvoiceRequest(int VisitId, decimal Amount);
    public record PayInvoiceRequest(string PaymentStatus); // Paid, Unpaid

    // AI
    public record SummarizeNoteRequest(string DoctorNotes);
    public record ApproveSummaryRequest(string ApprovedSummary);
    public record ChatRequest(int PatientId, string Message);
    public record ChatResponse(string Reply, DateTime CreatedAt, int? BookedAppointmentId = null);
}
