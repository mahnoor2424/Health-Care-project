using HealthCareConsultation.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareConsultation.Data
{
    public class HealthcareDbContext : DbContext
    {
        public HealthcareDbContext(DbContextOptions<HealthcareDbContext> options)
            : base(options)
        {
        }

        // DbSet for DoctorProfile model
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }  // Ye DbSet<DoctorProfile> hona chahiye, object nahi

     
     
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public object Patients { get; internal set; }
        public DbSet<PrescriptionModel> Prescriptions { get; set; }
        public object Doctors { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
