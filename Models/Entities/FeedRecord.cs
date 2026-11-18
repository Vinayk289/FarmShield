using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class FeedRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        public string FeedType { get; set; } = string.Empty;

        [Required]
        [MaxLength(60)]
        public string BatchLotNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string SupplierName { get; set; } = string.Empty;

        [Range(0.1, 100000.0)]
        public double QuantityKg { get; set; }

        public bool QualityCheckPassed { get; set; } = true;

        public bool MoistureLevelOk { get; set; } = true;

        public bool NoMoldDetected { get; set; } = true;

        [MaxLength(100)]
        public string StorageSiloCondition { get; set; } = "Dry, sealed and vermin-proof";

        [Required]
        [MaxLength(100)]
        public string InspectedBy { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

