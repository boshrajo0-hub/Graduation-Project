//using Microsoft.AspNetCore.Mvc;

//namespace HealthCareCenter.Controllers
//{
//    public class PatientsController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
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
    public class PatientsController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;
        private readonly IPatientService _patientService;

        public PatientsController(HealthCareCenterDBContext db, IPatientService patientService)
        {
            _db = db;
            _patientService = patientService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatients(
            [FromQuery] string? search, CancellationToken ct)
        {
            var query = _db.Patients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.FullName.Contains(search) || (p.Phone != null && p.Phone.Contains(search)));

            var patients = await query
                .OrderBy(p => p.FullName)
                .Select(p => new
                {
                    p.PatientId,
                    p.UserId,
                    p.FullName,
                    p.BirthDate,
                    p.Gender,
                    p.Phone,
                    p.Address
                })
                .ToListAsync(ct);

            return Ok(patients);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CreatePatient(
            [FromBody] CreatePatientRequest request, CancellationToken ct)
        {
            var (ok, error, patient) = await _patientService.CreateAsync(request, ct);
            if (!ok || patient is null)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = patient.PatientId }, new
            {
                patient.PatientId,
                patient.UserId,
                patient.FullName,
                patient.BirthDate,
                patient.Gender,
                patient.Phone,
                patient.Address
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> UpdatePatient(
            int id, [FromBody] UpdatePatientRequest request, CancellationToken ct)
        {
            var (ok, error) = await _patientService.UpdateAsync(id, request, ct);
            if (!ok)
                return BadRequest(new { message = error });

            return NoContent();
        }

        [HttpGet("{id:int}/medical-record")]
        [Authorize(Roles = "Admin,Doctor,Receptionist,Patient")]
        public async Task<IActionResult> GetMedicalRecord(int id, CancellationToken ct)
        {
            var record = await _patientService.GetMedicalRecordAsync(id, ct);
            if (record is null)
                return NotFound(new { message = "Patient not found." });

            return Ok(record);
        }
    }
}
