using HealthCareCenter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;

        public ReportsController(HealthCareCenterDBContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken ct)
        {
            var totalPatients = await _db.Patients.CountAsync(ct);
            var totalPaidRevenue = await _db.Invoices
                .Where(i => i.PaymentStatus == "Paid")
                .SumAsync(i => (decimal?)i.Amount, ct) ?? 0m;

            var clinicLoad = await _db.Clinics
                .AsNoTracking()
                .Select(c => new
                {
                    c.ClinicId,
                    ClinicName = c.Name,
                    AppointmentsCount = c.Doctors.SelectMany(d => d.Appointments).Count()
                })
                .ToListAsync(ct);

            return Ok(new
            {
                totalPatients,
                totalPaidRevenue,
                clinicLoad
            });
        }
    }
}
