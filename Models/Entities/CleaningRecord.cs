using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class CleaningRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        [MaxLength(100)]
        public string FacilityArea { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EquipmentSanitized { get; set; } = string.Empty;

        public bool VehicleDisinfectionStation { get; set; } = true;

        public bool FootbathRefreshed { get; set; } = true;

        public bool HandwashingSuppliesChecked { get; set; } = true;

        [Required]
        [MaxLength(100)]
        public string DisinfectantUsed { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DilutionRatio { get; set; } = "1:200";

        [Required]
        [MaxLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        [Required]
        public DateTime CleaningDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Status { get; set; } = "Verified";

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

