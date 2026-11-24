using HealthCareCenter.DTOs;
using HealthCareCenter.Models;

namespace HealthCareCenter.Services.Interfaces
{
    public interface IClinicalService
    {
        Task<Visit?> CreateVisitAsync(CreateVisitRequest request, CancellationToken ct = default);
        Task<bool> ApproveSummaryAsync(int visitId, string approvedSummary, CancellationToken ct = default);
        Task<Prescription?> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken ct = default);
        Task<bool> DispensePrescriptionAsync(int prescriptionId, CancellationToken ct = default);
        Task<Invoice?> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken ct = default);
        Task<bool> UpdatePaymentStatusAsync(int invoiceId, string status, CancellationToken ct = default);
    }
}
