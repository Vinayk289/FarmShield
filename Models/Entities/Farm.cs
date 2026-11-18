using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Farm
    {
        [Key]
        public int FarmId { get; set; }

        [Required]
        [MaxLength(20)]
        public string FarmCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        [Required]
        [MaxLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        public FarmType FarmType { get; set; } = FarmType.Mixed;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Range(0.1, 10000.0)]
        public double FarmAreaAcres { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Range(1, 1000000)]
        public int TotalCapacity { get; set; }

        public FarmStatus Status { get; set; } = FarmStatus.Active;

        public BioSafetyLevel BioSafetyLevel { get; set; } = BioSafetyLevel.Level2_Enhanced;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        // Navigation properties
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
        public ICollection<PoultryBatch> PoultryBatches { get; set; } = new List<PoultryBatch>();
        public ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
        public ICollection<BiosecurityActivity> BiosecurityActivities { get; set; } = new List<BiosecurityActivity>();
        public ICollection<CleaningRecord> CleaningRecords { get; set; } = new List<CleaningRecord>();
        public ICollection<FeedRecord> FeedRecords { get; set; } = new List<FeedRecord>();
        public ICollection<WaterQualityRecord> WaterQualityRecords { get; set; } = new List<WaterQualityRecord>();
        public ICollection<PestControlRecord> PestControlRecords { get; set; } = new List<PestControlRecord>();
        public ICollection<WasteManagementRecord> WasteManagementRecords { get; set; } = new List<WasteManagementRecord>();
        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
        public ICollection<DiseaseReport> DiseaseReports { get; set; } = new List<DiseaseReport>();
        public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
        public ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

