using Microsoft.EntityFrameworkCore;
using TinaKuafor.Models;

namespace TinaKuafor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<Testimonial> Testimonials { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<AppointmentService> AppointmentServices { get; set; } = null!;
        public DbSet<BusinessHours> BusinessHours { get; set; } = null!;
        public DbSet<SiteSettings> SiteSettings { get; set; } = null!;
        public DbSet<ClosedDay> ClosedDays { get; set; } = null!;
        public DbSet<FinancialRecord> FinancialRecords { get; set; } = null!;
        public DbSet<BusinessDocument> Documents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppointmentService>()
                .HasOne(a => a.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppointmentService>()
                .HasOne(a => a.Service)
                .WithMany(s => s.AppointmentServices)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure FinancialRecord relationships
            modelBuilder.Entity<FinancialRecord>()
                .HasOne(f => f.Appointment)
                .WithMany()
                .HasForeignKey(f => f.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            modelBuilder.Entity<Service>()
                .HasIndex(s => s.UrlSlug)
                .IsUnique();

            modelBuilder.Entity<ServiceCategory>()
                .HasIndex(c => c.UrlSlug)
                .IsUnique();

            // Configure SiteSettings to have only one record
            modelBuilder.Entity<SiteSettings>()
                .HasData(new SiteSettings { Id = 1 });
        }
    }
}
