using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class DiseaseReport
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        public int? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; }

        public int? PoultryBatchId { get; set; }
        [ForeignKey("PoultryBatchId")]
        public PoultryBatch? PoultryBatch { get; set; }

        [Required]
        [MaxLength(120)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string DiseaseCategory { get; set; } = "Viral"; // Viral, Bacterial, Parasitic, Nutritional, Unknown

        [Required]
        public DateTime DetectionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DiseaseSeverity Severity { get; set; } = DiseaseSeverity.Medium;

        [Required]
        [MaxLength(500)]
        public string Symptoms { get; set; } = string.Empty;

        [Range(0, 100000)]
        public int AffectedCount { get; set; }

        [Range(0, 100000)]
        public int MortalityCount { get; set; }

        public bool IsolationImplemented { get; set; } = true;

        [MaxLength(500)]
        public string? TreatmentGiven { get; set; }

        public DiseaseVerificationStatus VerificationStatus { get; set; } = DiseaseVerificationStatus.Suspected;

        [MaxLength(1000)]
        public string? VeterinaryNotes { get; set; }

        [MaxLength(100)]
        public string? VerifiedBy { get; set; }

        public DateTime? VerificationDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReportedBy { get; set; } = string.Empty;

        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
    }
}

