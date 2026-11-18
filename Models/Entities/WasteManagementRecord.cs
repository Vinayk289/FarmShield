using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class WasteManagementRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        public DateTime DisposalDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(80)]
        public string WasteType { get; set; } = "Manure Compost"; // Manure Compost, Mortality Carcass, Medical Waste, General Refuse

        [Required]
        [MaxLength(80)]
        public string DisposalMethod { get; set; } = "Deep Composting Facility"; // Composting, Deep Burial, Incineration, Authorized Collector

        [Range(0.1, 100000.0)]
        public double QuantityKg { get; set; }

        public bool SanitationOfDisposalAreaChecked { get; set; } = true;

        [MaxLength(80)]
        public string? DisposalDepthOrTemp { get; set; } = "65°C Thermophilic Phase";

        [Required]
        [MaxLength(100)]
        public string SupervisorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

