using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class PestControlRecord
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
        [MaxLength(80)]
        public string TargetPest { get; set; } = "Rodents & Flies";

        [Range(0, 500)]
        public int BaitStationsChecked { get; set; }

        [Range(0, 500)]
        public int TrapsServiced { get; set; }

        [Required]
        [MaxLength(30)]
        public string InfestationLevel { get; set; } = "None"; // None, Low, Moderate, High

        [Required]
        [MaxLength(150)]
        public string ChemicalOrMethodUsed { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? CorrectiveActionTaken { get; set; }

        public DateTime NextScheduledDate { get; set; } = DateTime.UtcNow.AddDays(14);

        [Required]
        [MaxLength(100)]
        public string InspectorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

