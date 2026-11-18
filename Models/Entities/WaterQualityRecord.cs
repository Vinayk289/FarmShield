using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class WaterQualityRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        public DateTime TestingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        public string WaterSource { get; set; } = "Borewell / Deep Aquifer";

        [Range(0.0, 14.0)]
        public double PhLevel { get; set; } = 6.8;

        [Range(0.0, 20.0)]
        public double ChlorinePpm { get; set; } = 3.5;

        [Range(0.0, 5000.0)]
        public double TdsLevel { get; set; } = 420.0;

        [Required]
        [MaxLength(80)]
        public string ColiformBacterialTestResult { get; set; } = "Negative / 0 CFU per 100ml";

        [Required]
        [MaxLength(100)]
        public string SanitizationMethod { get; set; } = "Chlorination & Inline UV Treatment";

        public bool IsPotableSafe { get; set; } = true;

        [Required]
        [MaxLength(100)]
        public string TestedBy { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

