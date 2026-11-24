////using HealthCareCenter.Data;
////using HealthCareCenter.Models;
////using Microsoft.AspNetCore.Authorization;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;

////namespace HealthCareCenter.Controllers
////{
////    [ApiController]
////    [Route("api")]
////    [Authorize]
////    public class ClinicsAndDoctorsController : ControllerBase
////    {
////        private readonly HealthCareCenterDBContext _db;

////        public ClinicsAndDoctorsController(HealthCareCenterDBContext db)
////        {
////            _db = db;
////        }

////        [HttpGet("clinics")]
////        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics(CancellationToken ct)
////        {
////            var clinics = await _db.Clinics
////                .AsNoTracking()
////                .OrderBy(c => c.Name)
////                .ToListAsync(ct);

////            return Ok(clinics);
////        }

////        [HttpPost("clinics")]
////        [Authorize(Roles = "Admin")]
////        public async Task<ActionResult<Clinic>> CreateClinic([FromBody] Clinic clinic, CancellationToken ct)
////        {
////            _db.Clinics.Add(clinic);
////            await _db.SaveChangesAsync(ct);
////            return CreatedAtAction(nameof(GetClinics), new { id = clinic.ClinicId }, clinic);
////        }

////        [HttpGet("doctors")]
////        public async Task<ActionResult<IEnumerable<object>>> GetDoctors(
////            [FromQuery] int? clinicId, CancellationToken ct)
////        {
////            var query = _db.Doctors
////                .AsNoTracking()
////                .Include(d => d.Clinic)
////                .AsQueryable();

////            if (clinicId.HasValue)
////                query = query.Where(d => d.ClinicId == clinicId.Value);

////            var doctors = await query
////                .Select(d => new
////                {
////                    d.DoctorId,
////                    d.FullName,
////                    d.Specialty,
////                    d.ClinicId,
////                    ClinicName = d.Clinic.Name,
////                    d.Phone
////                })
////                .ToListAsync(ct);

////            return Ok(doctors);
////        }

////        [HttpPost("doctors")]
////        [Authorize(Roles = "Admin")]
////        public async Task<ActionResult<Doctor>> CreateDoctor([FromBody] Doctor doctor, CancellationToken ct)
////        {
////            _db.Doctors.Add(doctor);
////            await _db.SaveChangesAsync(ct);
////            return CreatedAtAction(nameof(GetDoctors), new { id = doctor.DoctorId }, doctor);
////        }
////    }
////}



//using HealthCareCenter.Data;
//using HealthCareCenter.DTOs;
//using HealthCareCenter.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HealthCareCenter.Controllers
//{
//    [ApiController]
//    [Route("api")]
//    [Authorize]
//    public class ClinicsAndDoctorsController : ControllerBase
//    {
//        private readonly HealthCareCenterDBContext _db;

//        public ClinicsAndDoctorsController(HealthCareCenterDBContext db)
//        {
//            _db = db;
//        }

//        [HttpGet("clinics")]
//        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics(CancellationToken ct)
//        {
//            var clinics = await _db.Clinics
//                .AsNoTracking()
//                .OrderBy(c => c.Name)
//                .ToListAsync(ct);

//            return Ok(clinics);
//        }

//        [HttpPost("clinics")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<object>> CreateClinic(
//            [FromBody] CreateClinicRequest request, CancellationToken ct)
//        {
//            var clinic = new Clinic
//            {
//                Name = request.Name,
//                Description = request.Description
//            };

//            _db.Clinics.Add(clinic);
//            await _db.SaveChangesAsync(ct);

//            return CreatedAtAction(nameof(GetClinics), new { id = clinic.ClinicId }, new
//            {
//                clinic.ClinicId,
//                clinic.Name,
//                clinic.Description
//            });
//        }

//        [HttpGet("doctors")]
//        public async Task<ActionResult<IEnumerable<object>>> GetDoctors(
//            [FromQuery] int? clinicId, CancellationToken ct)
//        {
//            var query = _db.Doctors
//                .AsNoTracking()
//                .Include(d => d.Clinic)
//                .AsQueryable();

//            if (clinicId.HasValue)
//                query = query.Where(d => d.ClinicId == clinicId.Value);

//            var doctors = await query
//                .Select(d => new
//                {
//                    d.DoctorId,
//                    d.FullName,
//                    d.Specialty,
//                    d.ClinicId,
//                    ClinicName = d.Clinic.Name,
//                    d.Phone
//                })
//                .ToListAsync(ct);

//            return Ok(doctors);
//        }

//        //[HttpPost("doctors")]
//        //[Authorize(Roles = "Admin")]
//        //public async Task<ActionResult<Doctor>> CreateDoctor([FromBody] Doctor doctor, CancellationToken ct)
//        //{
//        //    _db.Doctors.Add(doctor);
//        //    await _db.SaveChangesAsync(ct);
//        //    return CreatedAtAction(nameof(GetDoctors), new { id = doctor.DoctorId }, doctor);
//        //}
//        [HttpPost("doctors")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<object>> CreateDoctor(
//    [FromBody] CreateDoctorRequest request, CancellationToken ct)
//        {
//            var clinicExists = await _db.Clinics
//                .AnyAsync(c => c.ClinicId == request.ClinicId, ct);

//            if (!clinicExists)
//                return BadRequest(new { message = $"Clinic with ID {request.ClinicId} does not exist." });

//            if (request.UserId > 0)
//            {
//                var userExists = await _db.Users
//                    .AnyAsync(u => u.UserId == request.UserId, ct);

//                if (!userExists)
//                    return BadRequest(new { message = $"User with ID {request.UserId} does not exist." });
//            }

//            var doctor = new Doctor
//            {
//                UserId = request.UserId,
//                FullName = request.FullName,
//                Specialty = request.Specialty,
//                ClinicId = request.ClinicId,
//                Phone = request.Phone
//            };

//            _db.Doctors.Add(doctor);
//            await _db.SaveChangesAsync(ct);

//            return CreatedAtAction(nameof(GetDoctors), new { id = doctor.DoctorId }, new
//            {
//                doctor.DoctorId,
//                doctor.UserId,
//                doctor.FullName,
//                doctor.Specialty,
//                doctor.ClinicId,
//                doctor.Phone
//            });
//        }
//    }
//}


using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ClinicsAndDoctorsController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;

        public ClinicsAndDoctorsController(HealthCareCenterDBContext db)
        {
            _db = db;
        }

        [HttpGet("clinics")]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics(CancellationToken ct)
        {
            var clinics = await _db.Clinics
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

            return Ok(clinics);
        }

        [HttpPost("clinics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateClinic(
            [FromBody] CreateClinicRequest request, CancellationToken ct)
        {
            var clinic = new Clinic
            {
                Name = request.Name,
                Description = request.Description
            };

            _db.Clinics.Add(clinic);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetClinics), new { id = clinic.ClinicId }, new
            {
                clinic.ClinicId,
                clinic.Name,
                clinic.Description
            });
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctors(
            [FromQuery] int? clinicId, CancellationToken ct)
        {
            var query = _db.Doctors
                .AsNoTracking()
                .Include(d => d.Clinic)
                .Include(d => d.Schedules)
                .AsQueryable();

            if (clinicId.HasValue)
                query = query.Where(d => d.ClinicId == clinicId.Value);

            var doctors = await query
                .Select(d => new
                {
                    d.DoctorId,
                    d.FullName,
                    d.Specialty,
                    d.ClinicId,
                    ClinicName = d.Clinic.Name,
                    d.Phone,
                    Schedules = d.Schedules.Select(s => new
                    {
                        s.ScheduleId,
                        s.DayOfWeek,
                        s.StartTime,
                        s.EndTime
                    })
                })
                .ToListAsync(ct);

            return Ok(doctors);
        }

        [HttpPost("doctors")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateDoctor(
            [FromBody] CreateDoctorRequest request, CancellationToken ct)
        {
            var clinicExists = await _db.Clinics
                .AnyAsync(c => c.ClinicId == request.ClinicId, ct);

            if (!clinicExists)
                return BadRequest(new { message = $"Clinic with ID {request.ClinicId} does not exist." });

            var user = await _db.Users.FindAsync([request.UserId], ct);
            if (user is null)
                return BadRequest(new { message = $"User with ID {request.UserId} does not exist." });

            if (!user.Role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Linked user must have the 'Doctor' role." });

            var doctor = new Doctor
            {
                UserId = request.UserId,
                FullName = request.FullName,
                Specialty = request.Specialty,
                ClinicId = request.ClinicId,
                Phone = request.Phone
            };

            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetDoctors), new { id = doctor.DoctorId }, new
            {
                doctor.DoctorId,
                doctor.UserId,
                doctor.FullName,
                doctor.Specialty,
                doctor.ClinicId,
                doctor.Phone
            });
        }

        [HttpPut("doctors/{doctorId:int}/schedules")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpsertDoctorSchedule(
            int doctorId, [FromBody] UpsertDoctorScheduleRequest request, CancellationToken ct)
        {
            if (request.EndTime <= request.StartTime)
                return BadRequest(new { message = "EndTime must be after StartTime." });

            var doctorExists = await _db.Doctors.AnyAsync(d => d.DoctorId == doctorId, ct);
            if (!doctorExists)
                return NotFound(new { message = "Doctor not found." });

            var existing = await _db.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == request.DayOfWeek, ct);

            if (existing is null)
            {
                _db.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctorId,
                    DayOfWeek = request.DayOfWeek,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime
                });
            }
            else
            {
                existing.StartTime = request.StartTime;
                existing.EndTime = request.EndTime;
            }

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
