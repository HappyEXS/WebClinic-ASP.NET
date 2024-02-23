using Microsoft.EntityFrameworkCore;

namespace przychodnia.Models
{
    public class StorageContext : DbContext
    {
        public StorageContext(DbContextOptions options) : base(options) { }

        public DbSet<DoctorModel> Doctors { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<ScheduleModel> Schedules { get; set; }
        public DbSet<SpecialityModel> Specialities { get; set; }
        public DbSet<VisitModel> Visits { get; set; }
    }
}
