using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class PoultryBatch
    {
        [Key]
        public int BatchId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        [MaxLength(30)]
        public string BatchCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string BatchName { get; set; } = string.Empty;

        [Required]
        public PoultryType PoultryType { get; set; } = PoultryType.Broiler;

        [Required]
        [MaxLength(80)]
        public string Breed { get; set; } = string.Empty;

        public DateTime HatchDate { get; set; }

        [Range(1, 1000000)]
        public int InitialBirdCount { get; set; }

        [Range(0, 1000000)]
        public int CurrentBirdCount { get; set; }

        [Required]
        [MaxLength(50)]
        public string HouseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string HousingType { get; set; } = "Deep Litter System";

        public PoultryBatchStatus Status { get; set; } = PoultryBatchStatus.Active;

        public int TotalMortality { get; set; } = 0;

        [Range(0.0, 100.0)]
        public double TargetFcr { get; set; } = 1.6;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        // Navigation
        public ICollection<PoultryDailyLog> DailyLogs { get; set; } = new List<PoultryDailyLog>();
        public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
        public ICollection<DiseaseReport> DiseaseReports { get; set; } = new List<DiseaseReport>();
    }
}

