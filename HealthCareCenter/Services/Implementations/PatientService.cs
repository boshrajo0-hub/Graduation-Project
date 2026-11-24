using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Models;
using HealthCareCenter.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly HealthCareCenterDBContext _db;

        public PatientService(HealthCareCenterDBContext db)
        {
            _db = db;
        }

        public async Task<(bool Ok, string? Error, Patient? Patient)> CreateAsync(
            CreatePatientRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return (false, "Patient full name is required.", null);

            var user = await _db.Users.FindAsync([request.UserId], ct);
            if (user is null)
                return (false, $"User with ID {request.UserId} does not exist.", null);

            if (!user.Role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                return (false, "Linked user must have the 'Patient' role.", null);

            var alreadyLinked = await _db.Patients.AnyAsync(p => p.UserId == request.UserId, ct);
            if (alreadyLinked)
                return (false, "This user account is already linked to a patient profile.", null);

            var patient = new Patient
            {
                UserId = request.UserId,
                FullName = request.FullName.Trim(),
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                Phone = request.Phone,
                Address = request.Address
            };

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync(ct);

            return (true, null, patient);
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(
            int patientId, UpdatePatientRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return (false, "Patient full name is required.");

            var patient = await _db.Patients.FindAsync([patientId], ct);
            if (patient is null)
                return (false, "Patient not found.");

            patient.FullName = request.FullName.Trim();
            patient.BirthDate = request.BirthDate;
            patient.Gender = request.Gender;
            patient.Phone = request.Phone;
            patient.Address = request.Address;

            await _db.SaveChangesAsync(ct);
            return (true, null);
        }

        public async Task<object?> GetMedicalRecordAsync(int patientId, CancellationToken ct = default)
        {
            var record = await _db.Patients
                .AsNoTracking()
                .Where(p => p.PatientId == patientId)
                .Select(p => new
                {
                    p.PatientId,
                    p.UserId,
                    p.FullName,
                    p.BirthDate,
                    p.Gender,
                    p.Phone,
                    p.Address,
                    Appointments = p.Appointments
                        .OrderByDescending(a => a.AppointmentDate)
                        .ThenByDescending(a => a.StartTime)
                        .Select(a => new
                        {
                            a.AppointmentId,
                            a.AppointmentDate,
                            a.StartTime,
                            a.Status,
                            DoctorName = a.Doctor.FullName,
                            ClinicName = a.Doctor.Clinic.Name,
                            Visit = a.Visit == null ? null : new
                            {
                                a.Visit.VisitId,
                                a.Visit.VisitDate,
                                a.Visit.DoctorNotes,
                                a.Visit.AiSummary,
                                Prescriptions = a.Visit.Prescriptions.Select(pr => new
                                {
                                    pr.PrescriptionId,
                                    pr.IsDispensed,
                                    pr.DispensedDate,
                                    Items = pr.PrescriptionItems.Select(i => new
                                    {
                                        i.ItemId,
                                        i.MedicineName,
                                        i.Dose,
                                        i.Duration
                                    })
                                }),
                                Invoices = a.Visit.Invoices.Select(inv => new
                                {
                                    inv.InvoiceId,
                                    inv.Amount,
                                    inv.PaymentStatus,
                                    inv.PaymentDate
                                })
                            }
                        })
                })
                .FirstOrDefaultAsync(ct);

            return record;
        }
    }
}