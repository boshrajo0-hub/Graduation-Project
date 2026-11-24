using HealthCareCenter.DTOs;
using HealthCareCenter.Models;

namespace HealthCareCenter.Services.Interfaces
{
    public interface IPatientService
    {
        Task<(bool Ok, string? Error, Patient? Patient)> CreateAsync(CreatePatientRequest request, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> UpdateAsync(int patientId, UpdatePatientRequest request, CancellationToken ct = default);
        Task<object?> GetMedicalRecordAsync(int patientId, CancellationToken ct = default);
    }
}