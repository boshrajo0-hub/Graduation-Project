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
//    public class ClinicalController : ControllerBase
//    {
//        private readonly HealthCareCenterDBContext _db;
//        private readonly IClinicalService _clinicalService;

//        public ClinicalController(HealthCareCenterDBContext db, IClinicalService clinicalService)
//        {
//            _db = db;
//            _clinicalService = clinicalService;
//        }

//        [HttpPost("visits")]
//        [Authorize(Roles = "Doctor,Admin")]
//        public async Task<IActionResult> CreateVisit([FromBody] CreateVisitRequest request, CancellationToken ct)
//        {
//            var visit = await _clinicalService.CreateVisitAsync(request, ct);
//            if (visit is null)
//                return NotFound(new { message = "Appointment not found." });

//            return Ok(visit);
//        }

//        [HttpPost("prescriptions")]
//        [Authorize(Roles = "Doctor")]
//        public async Task<IActionResult> CreatePrescription(
//            [FromBody] CreatePrescriptionRequest request, CancellationToken ct)
//        {
//            var prescription = await _clinicalService.CreatePrescriptionAsync(request, ct);
//            if (prescription is null)
//                return NotFound(new { message = "Visit not found." });

//            return Ok(prescription);
//        }

//        [HttpGet("prescriptions/pending")]
//        [Authorize(Roles = "Pharmacist,Admin")]
//        public async Task<ActionResult<IEnumerable<object>>> GetPendingPrescriptions(CancellationToken ct)
//        {
//            var pending = await _db.Prescriptions
//                .AsNoTracking()
//                .Where(p => !p.IsDispensed)
//                .Include(p => p.PrescriptionItems)
//                .Include(p => p.Visit)
//                    .ThenInclude(v => v.Appointment)
//                        .ThenInclude(a => a.Patient)
//                .Select(p => new
//                {
//                    p.PrescriptionId,
//                    p.VisitId,
//                    PatientName = p.Visit.Appointment.Patient.FullName,
//                    p.IsDispensed,
//                    Items = p.PrescriptionItems.Select(i => new
//                    {
//                        i.ItemId,
//                        i.MedicineName,
//                        i.Dose,
//                        i.Duration
//                    })
//                })
//                .ToListAsync(ct);

//            return Ok(pending);
//        }

//        [HttpPatch("prescriptions/{id:int}/dispense")]
//        [Authorize(Roles = "Pharmacist,Admin")]
//        public async Task<IActionResult> Dispense(int id, CancellationToken ct)
//        {
//            var ok = await _clinicalService.DispensePrescriptionAsync(id, ct);
//            if (!ok)
//                return NotFound();

//            return NoContent();
//        }

//        [HttpPost("invoices")]
//        [Authorize(Roles = "Receptionist,Admin")]
//        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request, CancellationToken ct)
//        {
//            var invoice = await _clinicalService.CreateInvoiceAsync(request, ct);
//            if (invoice is null)
//                return NotFound(new { message = "Visit not found." });

//            return Ok(invoice);
//        }

//        [HttpPatch("invoices/{id:int}/payment")]
//        [Authorize(Roles = "Receptionist,Admin")]
//        public async Task<IActionResult> UpdatePayment(
//            int id, [FromBody] PayInvoiceRequest request, CancellationToken ct)
//        {
//            var ok = await _clinicalService.UpdatePaymentStatusAsync(id, request.PaymentStatus, ct);
//            if (!ok)
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
    public class ClinicalController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;
        private readonly IClinicalService _clinicalService;

        public ClinicalController(HealthCareCenterDBContext db, IClinicalService clinicalService)
        {
            _db = db;
            _clinicalService = clinicalService;
        }

        [HttpPost("visits")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateVisit([FromBody] CreateVisitRequest request, CancellationToken ct)
        {
            var visit = await _clinicalService.CreateVisitAsync(request, ct);
            if (visit is null)
                return NotFound(new { message = "Appointment not found." });

            return Ok(new
            {
                visit.VisitId,
                visit.AppointmentId,
                visit.DoctorNotes,
                visit.AiSummary,
                visit.VisitDate
            });
        }

        [HttpPost("prescriptions")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreatePrescription(
            [FromBody] CreatePrescriptionRequest request, CancellationToken ct)
        {
            var prescription = await _clinicalService.CreatePrescriptionAsync(request, ct);
            if (prescription is null)
                return NotFound(new { message = "Visit not found." });

            return Ok(new
            {
                prescription.PrescriptionId,
                prescription.VisitId,
                prescription.IsDispensed,
                prescription.DispensedDate,
                Items = prescription.PrescriptionItems.Select(i => new
                {
                    i.ItemId,
                    i.MedicineName,
                    i.Dose,
                    i.Duration
                })
            });
        }

        [HttpGet("prescriptions/pending")]
        [Authorize(Roles = "Pharmacist,Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetPendingPrescriptions(CancellationToken ct)
        {
            var pending = await _db.Prescriptions
                .AsNoTracking()
                .Where(p => !p.IsDispensed)
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                        .ThenInclude(a => a.Patient)
                .Select(p => new
                {
                    p.PrescriptionId,
                    p.VisitId,
                    PatientName = p.Visit.Appointment.Patient.FullName,
                    p.IsDispensed,
                    Items = p.PrescriptionItems.Select(i => new
                    {
                        i.ItemId,
                        i.MedicineName,
                        i.Dose,
                        i.Duration
                    })
                })
                .ToListAsync(ct);

            return Ok(pending);
        }

        [HttpPatch("prescriptions/{id:int}/dispense")]
        [Authorize(Roles = "Pharmacist,Admin")]
        public async Task<IActionResult> Dispense(int id, CancellationToken ct)
        {
            var ok = await _clinicalService.DispensePrescriptionAsync(id, ct);
            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpPost("invoices")]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request, CancellationToken ct)
        {
            var invoice = await _clinicalService.CreateInvoiceAsync(request, ct);
            if (invoice is null)
                return NotFound(new { message = "Visit not found." });

            return Ok(new
            {
                invoice.InvoiceId,
                invoice.VisitId,
                invoice.Amount,
                invoice.PaymentStatus,
                invoice.PaymentDate
            });
        }

        [HttpPatch("invoices/{id:int}/payment")]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> UpdatePayment(
            int id, [FromBody] PayInvoiceRequest request, CancellationToken ct)
        {
            var ok = await _clinicalService.UpdatePaymentStatusAsync(id, request.PaymentStatus, ct);
            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}
