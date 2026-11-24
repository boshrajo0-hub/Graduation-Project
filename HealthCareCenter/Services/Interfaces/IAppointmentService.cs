//using HealthCareCenter.DTOs;
//using HealthCareCenter.Models;

//namespace HealthCareCenter.Services.Interfaces
//{
//    public interface IAppointmentService
//    {
//        Task<(bool Ok, string? Error, Appointment? Appointment)> CreateAsync(
//        CreateAppointmentRequest request, CancellationToken ct = default);

//        Task<bool> UpdateStatusAsync(int appointmentId, string status, CancellationToken ct = default);
//    }
//}


using HealthCareCenter.DTOs;
using HealthCareCenter.Models;

namespace HealthCareCenter.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<(bool Ok, string? Error, Appointment? Appointment)> CreateAsync(
            CreateAppointmentRequest request, CancellationToken ct = default);

        Task<(bool Ok, string? Error)> RescheduleAsync(
            int appointmentId, RescheduleAppointmentRequest request, CancellationToken ct = default);

        Task<(bool Ok, string? Error)> UpdateStatusAsync(
            int appointmentId, string status, CancellationToken ct = default);
    }
}