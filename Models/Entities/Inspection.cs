using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Inspection
    {
        [Key]
        public int InspectionId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        [MaxLength(30)]
        public string InspectionCode { get; set; } = string.Empty;

        [Required]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string InspectorId { get; set; } = string.Empty;
        [ForeignKey("InspectorId")]
        public ApplicationUser? Inspector { get; set; }

        [Required]
        [MaxLength(100)]
        public string InspectorName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string InspectorDesignation { get; set; } = "Veterinary Biosecurity Auditor";

        public int TotalChecklistItems { get; set; }
        public int CompliantItems { get; set; }
        public int NonCompliantItems { get; set; }
        public int NotApplicableItems { get; set; }

        [Range(0.0, 100.0)]
        public double CompliancePercentage { get; set; }

        public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;

        public InspectionStatus Status { get; set; } = InspectionStatus.Completed;

        [MaxLength(1000)]
        public string? ViolationsIdentified { get; set; }

        [MaxLength(1000)]
        public string? Recommendations { get; set; }

        [MaxLength(1000)]
        public string? CorrectiveActionRequired { get; set; }

        public bool CertifiedBiosecure { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<InspectionResponse> Responses { get; set; } = new List<InspectionResponse>();
        public ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
    }
}

