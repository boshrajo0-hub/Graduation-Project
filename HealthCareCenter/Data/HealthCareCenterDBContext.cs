//using HealthCareCenter.Models;
//using Microsoft.EntityFrameworkCore;

//namespace HealthCareCenter.Data
//{
//    public class HealthCareCenterDBContext : DbContext
//    {
//        public HealthCareCenterDBContext(DbContextOptions<HealthCareCenterDBContext> options) : base(options)
//        {
//        }
//        public DbSet<User> Users => Set<User>();
//        public DbSet<Patient> Patients => Set<Patient>();
//        public DbSet<Clinic> Clinics => Set<Clinic>();
//        public DbSet<Doctor> Doctors => Set<Doctor>();
//        public DbSet<Appointment> Appointments => Set<Appointment>();
//        public DbSet<Visit> Visits => Set<Visit>();
//        public DbSet<Invoice> Invoices => Set<Invoice>();
//        public DbSet<Prescription> Prescriptions => Set<Prescription>();
//        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
//        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Users
//            modelBuilder.Entity<User>(entity =>
//            {
//                entity.ToTable("Users");
//                entity.HasKey(e => e.UserId);
//                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
//                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
//                entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
//                entity.Property(e => e.IsActive).IsRequired();

//                entity.HasIndex(e => e.Email).IsUnique();
//            });

//            // Patients
//            modelBuilder.Entity<Patient>(entity =>
//            {
//                entity.ToTable("Patients");
//                entity.HasKey(e => e.PatientId);
//                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
//                entity.Property(e => e.BirthDate).HasColumnType("date");
//                entity.Property(e => e.Gender).HasMaxLength(10);
//                entity.Property(e => e.Phone).HasMaxLength(20);
//                entity.Property(e => e.Address).HasMaxLength(200);

//                entity.HasOne(e => e.User)
//                    .WithOne(u => u.Patient)
//                    .HasForeignKey<Patient>(e => e.UserId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Clinics
//            modelBuilder.Entity<Clinic>(entity =>
//            {
//                entity.ToTable("Clinics");
//                entity.HasKey(e => e.ClinicId);
//                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
//                entity.Property(e => e.Description).HasMaxLength(200);
//            });

//            // Doctors
//            modelBuilder.Entity<Doctor>(entity =>
//            {
//                entity.ToTable("Doctors");
//                entity.HasKey(e => e.DoctorId);
//                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
//                entity.Property(e => e.Specialty).HasMaxLength(100).IsRequired();
//                entity.Property(e => e.Phone).HasMaxLength(20);

//                entity.HasOne(e => e.User)
//                    .WithOne(u => u.Doctor)
//                    .HasForeignKey<Doctor>(e => e.UserId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                entity.HasOne(e => e.Clinic)
//                    .WithMany(c => c.Doctors)
//                    .HasForeignKey(e => e.ClinicId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Appointments
//            modelBuilder.Entity<Appointment>(entity =>
//            {
//                entity.ToTable("Appointments");
//                entity.HasKey(e => e.AppointmentId);
//                entity.Property(e => e.AppointmentDate).HasColumnType("date").IsRequired();
//                entity.Property(e => e.StartTime).HasColumnType("time(7)").IsRequired();
//                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

//                entity.HasOne(e => e.Patient)
//                    .WithMany(p => p.Appointments)
//                    .HasForeignKey(e => e.PatientId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                entity.HasOne(e => e.Doctor)
//                    .WithMany(d => d.Appointments)
//                    .HasForeignKey(e => e.DoctorId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Visits
//            modelBuilder.Entity<Visit>(entity =>
//            {
//                entity.ToTable("Visits");
//                entity.HasKey(e => e.VisitId);
//                entity.Property(e => e.DoctorNotes).HasColumnType("nvarchar(max)");
//                entity.Property(e => e.AiSummary).HasColumnType("nvarchar(max)");
//                entity.Property(e => e.VisitDate).HasColumnType("datetime2(7)").IsRequired();

//                entity.HasOne(e => e.Appointment)
//                    .WithOne(a => a.Visit)
//                    .HasForeignKey<Visit>(e => e.AppointmentId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Invoices
//            modelBuilder.Entity<Invoice>(entity =>
//            {
//                entity.ToTable("Invoices");
//                entity.HasKey(e => e.InvoiceId);
//                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)").IsRequired();
//                entity.Property(e => e.PaymentStatus).HasMaxLength(20).IsRequired();
//                entity.Property(e => e.PaymentDate).HasColumnType("datetime2(7)");

//                entity.HasOne(e => e.Visit)
//                    .WithMany(v => v.Invoices)
//                    .HasForeignKey(e => e.VisitId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Prescriptions
//            modelBuilder.Entity<Prescription>(entity =>
//            {
//                entity.ToTable("Prescriptions");
//                entity.HasKey(e => e.PrescriptionId);
//                entity.Property(e => e.IsDispensed).IsRequired();
//                entity.Property(e => e.DispensedDate).HasColumnType("datetime2(7)");

//                entity.HasOne(e => e.Visit)
//                    .WithMany(v => v.Prescriptions)
//                    .HasForeignKey(e => e.VisitId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });

//            // PrescriptionItems
//            modelBuilder.Entity<PrescriptionItem>(entity =>
//            {
//                entity.ToTable("PrescriptionItems");
//                entity.HasKey(e => e.ItemId);
//                entity.Property(e => e.MedicineName).HasMaxLength(100).IsRequired();
//                entity.Property(e => e.Dose).HasMaxLength(50);
//                entity.Property(e => e.Duration).HasMaxLength(50);

//                entity.HasOne(e => e.Prescription)
//                    .WithMany(p => p.PrescriptionItems)
//                    .HasForeignKey(e => e.PrescriptionId)
//                    .OnDelete(DeleteBehavior.Cascade);
//            });

//            // ChatMessages
//            modelBuilder.Entity<ChatMessage>(entity =>
//            {
//                entity.ToTable("ChatMessages");
//                entity.HasKey(e => e.MessageId);
//                entity.Property(e => e.MessageText).HasColumnType("nvarchar(max)").IsRequired();
//                entity.Property(e => e.BotReply).HasColumnType("nvarchar(max)");
//                entity.Property(e => e.CreatedAt).HasColumnType("datetime2(7)").IsRequired();

//                entity.HasOne(e => e.Patient)
//                    .WithMany(p => p.ChatMessages)
//                    .HasForeignKey(e => e.PatientId)
//                    .OnDelete(DeleteBehavior.Restrict);
//            });
//        }
//    }
//}




using HealthCareCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Data
{
    public class HealthCareCenterDBContext : DbContext
    {
        public HealthCareCenterDBContext(DbContextOptions<HealthCareCenterDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Visit> Visits => Set<Visit>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients");
                entity.HasKey(e => e.PatientId);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.BirthDate).HasColumnType("date");
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Patient)
                    .HasForeignKey<Patient>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Clinic>(entity =>
            {
                entity.ToTable("Clinics");
                entity.HasKey(e => e.ClinicId);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctors");
                entity.HasKey(e => e.DoctorId);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Specialty).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Clinic)
                    .WithMany(c => c.Doctors)
                    .HasForeignKey(e => e.ClinicId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.ToTable("DoctorSchedules");
                entity.HasKey(e => e.ScheduleId);
                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.StartTime).HasColumnType("time(7)").IsRequired();
                entity.Property(e => e.EndTime).HasColumnType("time(7)").IsRequired();

                entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek }).IsUnique();

                entity.HasOne(e => e.Doctor)
                    .WithMany(d => d.Schedules)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointments");
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.AppointmentDate).HasColumnType("date").IsRequired();
                entity.Property(e => e.StartTime).HasColumnType("time(7)").IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.ToTable("Visits");
                entity.HasKey(e => e.VisitId);
                entity.Property(e => e.DoctorNotes).HasColumnType("nvarchar(max)");
                entity.Property(e => e.AiSummary).HasColumnType("nvarchar(max)");
                entity.Property(e => e.VisitDate).HasColumnType("datetime2(7)").IsRequired();

                entity.HasOne(e => e.Appointment)
                    .WithOne(a => a.Visit)
                    .HasForeignKey<Visit>(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");
                entity.HasKey(e => e.InvoiceId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.PaymentStatus).HasMaxLength(20).IsRequired();
                entity.Property(e => e.PaymentDate).HasColumnType("datetime2(7)");

                entity.HasOne(e => e.Visit)
                    .WithMany(v => v.Invoices)
                    .HasForeignKey(e => e.VisitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescriptions");
                entity.HasKey(e => e.PrescriptionId);
                entity.Property(e => e.IsDispensed).IsRequired();
                entity.Property(e => e.DispensedDate).HasColumnType("datetime2(7)");

                entity.HasOne(e => e.Visit)
                    .WithMany(v => v.Prescriptions)
                    .HasForeignKey(e => e.VisitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PrescriptionItem>(entity =>
            {
                entity.ToTable("PrescriptionItems");
                entity.HasKey(e => e.ItemId);
                entity.Property(e => e.MedicineName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Dose).HasMaxLength(50);
                entity.Property(e => e.Duration).HasMaxLength(50);

                entity.HasOne(e => e.Prescription)
                    .WithMany(p => p.PrescriptionItems)
                    .HasForeignKey(e => e.PrescriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");
                entity.HasKey(e => e.MessageId);
                entity.Property(e => e.MessageText).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(e => e.BotReply).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2(7)").IsRequired();

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
