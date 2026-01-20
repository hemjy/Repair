using Microsoft.EntityFrameworkCore;
using Repair.Domain;

namespace Repair.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.Database.Migrate();
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<PhonePart> PhoneParts { get; set; }
        public DbSet<PhoneModel> PhoneModels { get; set; }
        public DbSet<RepairPrice> RepairPrices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
