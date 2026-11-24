//using HealthCareCenter.Services.Interfaces;
//using HealthCareCenter.Models;
//using HealthCareCenter.DTOs;
//using Microsoft.EntityFrameworkCore;
//using HealthCareCenter.Data;



//namespace HealthCareCenter.Services.Implementations
//{
//    public class ClinicalService : IClinicalService
//    {
//        private readonly HealthCareCenterDBContext _db;

//        public ClinicalService(HealthCareCenterDBContext db)
//        {
//            _db = db;
//        }

//        public async Task<Visit?> CreateVisitAsync(CreateVisitRequest request, CancellationToken ct = default)
//        {
//            var appointment = await _db.Appointments.FindAsync([request.AppointmentId], ct);
//            if (appointment is null)
//                return null;

//            var visit = new Visit
//            {
//                AppointmentId = request.AppointmentId,
//                DoctorNotes = request.DoctorNotes,
//                VisitDate = DateTime.UtcNow
//            };

//            appointment.Status = "Completed";
//            _db.Visits.Add(visit);
//            await _db.SaveChangesAsync(ct);
//            return visit;
//        }

//        // Saves ONLY after doctor reviews and approves the summary
//        public async Task<bool> ApproveSummaryAsync(int visitId, string approvedSummary, CancellationToken ct = default)
//        {
//            var visit = await _db.Visits.FindAsync([visitId], ct);
//            if (visit is null)
//                return false;

//            visit.AiSummary = approvedSummary;
//            await _db.SaveChangesAsync(ct);
//            return true;
//        }

//        public async Task<Prescription?> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken ct = default)
//        {
//            var visitExists = await _db.Visits.AnyAsync(v => v.VisitId == request.VisitId, ct);
//            if (!visitExists)
//                return null;

//            var prescription = new Prescription
//            {
//                VisitId = request.VisitId,
//                IsDispensed = false,
//                PrescriptionItems = request.Items.Select(i => new PrescriptionItem
//                {
//                    MedicineName = i.MedicineName,
//                    Dose = i.Dose,
//                    Duration = i.Duration
//                }).ToList()
//            };

//            _db.Prescriptions.Add(prescription);
//            await _db.SaveChangesAsync(ct);
//            return prescription;
//        }

//        public async Task<bool> DispensePrescriptionAsync(int prescriptionId, CancellationToken ct = default)
//        {
//            var prescription = await _db.Prescriptions.FindAsync([prescriptionId], ct);
//            if (prescription is null)
//                return false;

//            prescription.IsDispensed = true;
//            prescription.DispensedDate = DateTime.UtcNow;
//            await _db.SaveChangesAsync(ct);
//            return true;
//        }

//        public async Task<Invoice?> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken ct = default)
//        {
//            var visitExists = await _db.Visits.AnyAsync(v => v.VisitId == request.VisitId, ct);
//            if (!visitExists)
//                return null;

//            var invoice = new Invoice
//            {
//                VisitId = request.VisitId,
//                Amount = request.Amount,
//                PaymentStatus = "Unpaid"
//            };

//            _db.Invoices.Add(invoice);
//            await _db.SaveChangesAsync(ct);
//            return invoice;
//        }

//        public async Task<bool> UpdatePaymentStatusAsync(int invoiceId, string status, CancellationToken ct = default)
//        {
//            var invoice = await _db.Invoices.FindAsync([invoiceId], ct);
//            if (invoice is null)
//                return false;

//            invoice.PaymentStatus = status;
//            invoice.PaymentDate = status.Equals("Paid", StringComparison.OrdinalIgnoreCase)
//                ? DateTime.UtcNow
//                : null;

//            await _db.SaveChangesAsync(ct);
//            return true;
//        }
//    }
//}

using HealthCareCenter.Services.Interfaces;
using HealthCareCenter.Models;
using HealthCareCenter.DTOs;
using Microsoft.EntityFrameworkCore;
using HealthCareCenter.Data;

namespace HealthCareCenter.Services.Implementations
{
    public class ClinicalService : IClinicalService
    {
        private static readonly HashSet<string> AllowedPaymentStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Paid", "Unpaid"
        };

        private readonly HealthCareCenterDBContext _db;

        public ClinicalService(HealthCareCenterDBContext db)
        {
            _db = db;
        }

        public async Task<Visit?> CreateVisitAsync(CreateVisitRequest request, CancellationToken ct = default)
        {
            var appointment = await _db.Appointments.FindAsync([request.AppointmentId], ct);
            if (appointment is null)
                return null;

            var visit = new Visit
            {
                AppointmentId = request.AppointmentId,
                DoctorNotes = request.DoctorNotes,
                VisitDate = DateTime.UtcNow
            };

            appointment.Status = "Completed";
            _db.Visits.Add(visit);
            await _db.SaveChangesAsync(ct);
            return visit;
        }

        public async Task<bool> ApproveSummaryAsync(int visitId, string approvedSummary, CancellationToken ct = default)
        {
            var visit = await _db.Visits.FindAsync([visitId], ct);
            if (visit is null)
                return false;

            visit.AiSummary = approvedSummary;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<Prescription?> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken ct = default)
        {
            var visitExists = await _db.Visits.AnyAsync(v => v.VisitId == request.VisitId, ct);
            if (!visitExists)
                return null;

            var prescription = new Prescription
            {
                VisitId = request.VisitId,
                IsDispensed = false,
                PrescriptionItems = request.Items.Select(i => new PrescriptionItem
                {
                    MedicineName = i.MedicineName,
                    Dose = i.Dose,
                    Duration = i.Duration
                }).ToList()
            };

            _db.Prescriptions.Add(prescription);
            await _db.SaveChangesAsync(ct);
            return prescription;
        }

        public async Task<bool> DispensePrescriptionAsync(int prescriptionId, CancellationToken ct = default)
        {
            var prescription = await _db.Prescriptions.FindAsync([prescriptionId], ct);
            if (prescription is null)
                return false;

            prescription.IsDispensed = true;
            prescription.DispensedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<Invoice?> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken ct = default)
        {
            var visitExists = await _db.Visits.AnyAsync(v => v.VisitId == request.VisitId, ct);
            if (!visitExists)
                return null;

            var invoice = new Invoice
            {
                VisitId = request.VisitId,
                Amount = request.Amount,
                PaymentStatus = "Unpaid"
            };

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync(ct);
            return invoice;
        }

        public async Task<bool> UpdatePaymentStatusAsync(int invoiceId, string status, CancellationToken ct = default)
        {
            if (!AllowedPaymentStatuses.Contains(status))
                return false;

            var invoice = await _db.Invoices.FindAsync([invoiceId], ct);
            if (invoice is null)
                return false;

            var canonical = AllowedPaymentStatuses.First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
            invoice.PaymentStatus = canonical;
            invoice.PaymentDate = canonical == "Paid" ? DateTime.UtcNow : null;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
