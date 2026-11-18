using FarmShield.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farm> Farms => Set<Farm>();
        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<PoultryBatch> PoultryBatches => Set<PoultryBatch>();
        public DbSet<PoultryDailyLog> PoultryDailyLogs => Set<PoultryDailyLog>();
        public DbSet<Visitor> Visitors => Set<Visitor>();
        public DbSet<BiosecurityActivity> BiosecurityActivities => Set<BiosecurityActivity>();
        public DbSet<CleaningRecord> CleaningRecords => Set<CleaningRecord>();
        public DbSet<FeedRecord> FeedRecords => Set<FeedRecord>();
        public DbSet<WaterQualityRecord> WaterQualityRecords => Set<WaterQualityRecord>();
        public DbSet<PestControlRecord> PestControlRecords => Set<PestControlRecord>();
        public DbSet<WasteManagementRecord> WasteManagementRecords => Set<WasteManagementRecord>();
        public DbSet<Inspection> Inspections => Set<Inspection>();
        public DbSet<InspectionChecklistItem> InspectionChecklistItems => Set<InspectionChecklistItem>();
        public DbSet<InspectionResponse> InspectionResponses => Set<InspectionResponse>();
        public DbSet<DiseaseReport> DiseaseReports => Set<DiseaseReport>();
        public DbSet<Vaccination> Vaccinations => Set<Vaccination>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Indexes & Unique constraints
            builder.Entity<Farm>()
                .HasIndex(f => f.FarmCode)
                .IsUnique();

            builder.Entity<Animal>()
                .HasIndex(a => new { a.FarmId, a.TagNumber })
                .IsUnique();

            builder.Entity<PoultryBatch>()
                .HasIndex(p => new { p.FarmId, p.BatchCode })
                .IsUnique();

            // Configure Delete Behaviors to avoid cycles in SQL Server
            builder.Entity<Farm>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.OwnedFarms)
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inspection>()
                .HasOne(i => i.Inspector)
                .WithMany(u => u.ConductedInspections)
                .HasForeignKey(i => i.InspectorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inspection>()
                .HasOne(i => i.Farm)
                .WithMany(f => f.Inspections)
                .HasForeignKey(i => i.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InspectionResponse>()
                .HasOne(r => r.Inspection)
                .WithMany(i => i.Responses)
                .HasForeignKey(r => r.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InspectionResponse>()
                .HasOne(r => r.ChecklistItem)
                .WithMany()
                .HasForeignKey(r => r.ChecklistItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DiseaseReport>()
                .HasOne(d => d.Farm)
                .WithMany(f => f.DiseaseReports)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DiseaseReport>()
                .HasOne(d => d.Animal)
                .WithMany(a => a.DiseaseReports)
                .HasForeignKey(d => d.AnimalId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<DiseaseReport>()
                .HasOne(d => d.PoultryBatch)
                .WithMany(p => p.DiseaseReports)
                .HasForeignKey(d => d.PoultryBatchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Vaccination>()
                .HasOne(v => v.Farm)
                .WithMany(f => f.Vaccinations)
                .HasForeignKey(v => v.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vaccination>()
                .HasOne(v => v.Animal)
                .WithMany(a => a.Vaccinations)
                .HasForeignKey(v => v.AnimalId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Vaccination>()
                .HasOne(v => v.PoultryBatch)
                .WithMany(p => p.Vaccinations)
                .HasForeignKey(v => v.PoultryBatchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CorrectiveAction>()
                .HasOne(c => c.Farm)
                .WithMany(f => f.CorrectiveActions)
                .HasForeignKey(c => c.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CorrectiveAction>()
                .HasOne(c => c.Inspection)
                .WithMany(i => i.CorrectiveActions)
                .HasForeignKey(c => c.InspectionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CorrectiveAction>()
                .HasOne(c => c.DiseaseReport)
                .WithMany(d => d.CorrectiveActions)
                .HasForeignKey(c => c.DiseaseReportId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<PoultryDailyLog>()
                .HasOne(l => l.Batch)
                .WithMany(b => b.DailyLogs)
                .HasForeignKey(l => l.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.Farm)
                .WithMany(f => f.Notifications)
                .HasForeignKey(n => n.FarmId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

