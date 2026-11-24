//using HealthCareCenter.Services.Interfaces;
//using HealthCareCenter.Data;
//using HealthCareCenter.DTOs;
//using HealthCareCenter.Models;
//using Microsoft.EntityFrameworkCore;

//namespace HealthCareCenter.Services.Implementations
//{
//    public class AppointmentService : IAppointmentService
//    {
//        private const int DefaultSlotMinutes = 30;
//        private readonly HealthCareCenterDBContext _db;

//        public AppointmentService(HealthCareCenterDBContext db)
//        {
//            _db = db;
//        }

//        public async Task<(bool Ok, string? Error, Appointment? Appointment)> CreateAsync(
//            CreateAppointmentRequest request, CancellationToken ct = default)
//        {
//            var patientExists = await _db.Patients.AnyAsync(p => p.PatientId == request.PatientId, ct);
//            if (!patientExists)
//                return (false, "Patient not found.", null);

//            var doctorExists = await _db.Doctors.AnyAsync(d => d.DoctorId == request.DoctorId, ct);
//            if (!doctorExists)
//                return (false, "Doctor not found.", null);

//            var newStart = request.StartTime;
//            var newEnd = newStart.AddMinutes(DefaultSlotMinutes);

//            var existingSlots = await _db.Appointments
//                .Where(a => a.DoctorId == request.DoctorId
//                         && a.AppointmentDate == request.AppointmentDate
//                         && a.Status != "Cancelled")
//                .Select(a => a.StartTime)
//                .ToListAsync(ct);

//            bool hasConflict = existingSlots.Any(existingStart =>
//            {
//                var existingEnd = existingStart.AddMinutes(DefaultSlotMinutes);
//                return newStart < existingEnd && existingStart < newEnd;
//            });

//            if (hasConflict)
//                return (false, "The selected time slot conflicts with an existing appointment for this doctor.", null);

//            var appointment = new Appointment
//            {
//                PatientId = request.PatientId,
//                DoctorId = request.DoctorId,
//                AppointmentDate = request.AppointmentDate,
//                StartTime = request.StartTime,
//                Status = "Scheduled"
//            };

//            _db.Appointments.Add(appointment);
//            await _db.SaveChangesAsync(ct);

//            return (true, null, appointment);
//        }

//        public async Task<bool> UpdateStatusAsync(int appointmentId, string status, CancellationToken ct = default)
//        {
//            var appt = await _db.Appointments.FindAsync([appointmentId], ct);
//            if (appt is null)
//                return false;

//            appt.Status = status;
//            await _db.SaveChangesAsync(ct);
//            return true;
//        }
//    }
//}

using HealthCareCenter.Services.Interfaces;
using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private const int DefaultSlotMinutes = 30;
        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Scheduled", "Completed", "Cancelled"
        };

        private readonly HealthCareCenterDBContext _db;

        public AppointmentService(HealthCareCenterDBContext db)
        {
            _db = db;
        }

        public async Task<(bool Ok, string? Error, Appointment? Appointment)> CreateAsync(
            CreateAppointmentRequest request, CancellationToken ct = default)
        {
            var patientExists = await _db.Patients.AnyAsync(p => p.PatientId == request.PatientId, ct);
            if (!patientExists)
                return (false, "Patient not found.", null);

            var doctorExists = await _db.Doctors.AnyAsync(d => d.DoctorId == request.DoctorId, ct);
            if (!doctorExists)
                return (false, "Doctor not found.", null);

            var validationError = await ValidateSlotAsync(
                request.DoctorId, request.AppointmentDate, request.StartTime, excludeAppointmentId: null, ct);

            if (validationError is not null)
                return (false, validationError, null);

            var appointment = new Appointment
            {
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                Status = "Scheduled"
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync(ct);

            return (true, null, appointment);
        }

        public async Task<(bool Ok, string? Error)> RescheduleAsync(
            int appointmentId, RescheduleAppointmentRequest request, CancellationToken ct = default)
        {
            var appt = await _db.Appointments.FindAsync([appointmentId], ct);
            if (appt is null)
                return (false, "Appointment not found.");

            if (appt.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                appt.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return (false, $"Cannot reschedule an appointment with status '{appt.Status}'.");
            }

            var validationError = await ValidateSlotAsync(
                appt.DoctorId, request.AppointmentDate, request.StartTime, excludeAppointmentId: appointmentId, ct);

            if (validationError is not null)
                return (false, validationError);

            appt.AppointmentDate = request.AppointmentDate;
            appt.StartTime = request.StartTime;
            await _db.SaveChangesAsync(ct);

            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> UpdateStatusAsync(
            int appointmentId, string status, CancellationToken ct = default)
        {
            if (!AllowedStatuses.Contains(status))
                return (false, "Invalid status. Allowed values: Scheduled, Completed, Cancelled.");

            var appt = await _db.Appointments.FindAsync([appointmentId], ct);
            if (appt is null)
                return (false, "Appointment not found.");

            appt.Status = AllowedStatuses.First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
            await _db.SaveChangesAsync(ct);
            return (true, null);
        }

        private async Task<string?> ValidateSlotAsync(
            int doctorId,
            DateOnly date,
            TimeOnly startTime,
            int? excludeAppointmentId,
            CancellationToken ct)
        {
            var newEnd = startTime.AddMinutes(DefaultSlotMinutes);

            // Check doctor working schedule if schedules have been configured for this doctor
            var doctorSchedules = await _db.DoctorSchedules
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync(ct);

            if (doctorSchedules.Count > 0)
            {
                var daySchedule = doctorSchedules.FirstOrDefault(s => s.DayOfWeek == date.DayOfWeek);
                if (daySchedule is null)
                    return $"The doctor does not work on {date.DayOfWeek}.";

                if (startTime < daySchedule.StartTime || newEnd > daySchedule.EndTime)
                    return $"Appointment must fall within the doctor's working hours ({daySchedule.StartTime:HH:mm} - {daySchedule.EndTime:HH:mm}).";
            }

            var existingSlots = await _db.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate == date
                         && a.Status != "Cancelled"
                         && (!excludeAppointmentId.HasValue || a.AppointmentId != excludeAppointmentId.Value))
                .Select(a => a.StartTime)
                .ToListAsync(ct);

            bool hasConflict = existingSlots.Any(existingStart =>
            {
                var existingEnd = existingStart.AddMinutes(DefaultSlotMinutes);
                return startTime < existingEnd && existingStart < newEnd;
            });

            return hasConflict
                ? "The selected time slot conflicts with an existing appointment for this doctor."
                : null;
        }
    }
}

