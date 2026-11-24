//using HealthCareCenter.Data;
//using HealthCareCenter.DTOs;
//using HealthCareCenter.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HealthCareCenter.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class AppointmentsController : ControllerBase
//    {
//        private readonly HealthCareCenterDBContext _db;
//        private readonly IAppointmentService _appointmentService;

//        public AppointmentsController(HealthCareCenterDBContext db, IAppointmentService appointmentService)
//        {
//            _db = db;
//            _appointmentService = appointmentService;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<object>>> GetAppointments(
//            [FromQuery] DateOnly? date,
//            [FromQuery] int? doctorId,
//            [FromQuery] int? patientId,
//            CancellationToken ct)
//        {
//            var query = _db.Appointments
//                .AsNoTracking()
//                .Include(a => a.Patient)
//                .Include(a => a.Doctor)
//                .AsQueryable();

//            if (date.HasValue)
//                query = query.Where(a => a.AppointmentDate == date.Value);

//            if (doctorId.HasValue)
//                query = query.Where(a => a.DoctorId == doctorId.Value);

//            if (patientId.HasValue)
//                query = query.Where(a => a.PatientId == patientId.Value);

//            var list = await query
//                .OrderBy(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .Select(a => new
//                {
//                    a.AppointmentId,
//                    a.PatientId,
//                    PatientName = a.Patient.FullName,
//                    a.DoctorId,
//                    DoctorName = a.Doctor.FullName,
//                    a.AppointmentDate,
//                    a.StartTime,
//                    a.Status
//                })
//                .ToListAsync(ct);

//            return Ok(list);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin,Receptionist,Patient")]
//        public async Task<IActionResult> Create(
//            [FromBody] CreateAppointmentRequest request, CancellationToken ct)
//        {
//            var (ok, error, appointment) = await _appointmentService.CreateAsync(request, ct);
//            if (!ok || appointment is null)
//                return Conflict(new { message = error ?? "Unable to book appointment." });

//            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.AppointmentId }, appointment);
//        }

//        [HttpPatch("{id:int}/status")]
//        [Authorize(Roles = "Admin,Receptionist,Doctor")]
//        public async Task<IActionResult> UpdateStatus(
//            int id, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken ct)
//        {
//            var updated = await _appointmentService.UpdateStatusAsync(id, request.Status, ct);
//            if (!updated)
//                return NotFound();

//            return NoContent();
//        }
//    }
//}

using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(HealthCareCenterDBContext db, IAppointmentService appointmentService)
        {
            _db = db;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAppointments(
            [FromQuery] DateOnly? date,
            [FromQuery] int? doctorId,
            [FromQuery] int? patientId,
            CancellationToken ct)
        {
            var query = _db.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate == date.Value);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            var list = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.PatientId,
                    PatientName = a.Patient.FullName,
                    a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    a.AppointmentDate,
                    a.StartTime,
                    a.Status
                })
                .ToListAsync(ct);

            return Ok(list);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Create(
            [FromBody] CreateAppointmentRequest request, CancellationToken ct)
        {
            var (ok, error, appointment) = await _appointmentService.CreateAsync(request, ct);
            if (!ok || appointment is null)
                return Conflict(new { message = error ?? "Unable to book appointment." });

            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.AppointmentId }, appointment);
        }

        [HttpPut("{id:int}/reschedule")]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Reschedule(
            int id, [FromBody] RescheduleAppointmentRequest request, CancellationToken ct)
        {
            var (ok, error) = await _appointmentService.RescheduleAsync(id, request, ct);
            if (!ok)
                return Conflict(new { message = error ?? "Unable to reschedule appointment." });

            return NoContent();
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken ct)
        {
            var (ok, error) = await _appointmentService.UpdateStatusAsync(id, request.Status, ct);
            if (!ok)
                return BadRequest(new { message = error });

            return NoContent();
        }
    }
}
