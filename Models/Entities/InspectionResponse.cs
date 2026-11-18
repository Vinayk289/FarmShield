using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class InspectionResponse
    {
        [Key]
        public int ResponseId { get; set; }

        [Required]
        public int InspectionId { get; set; }
        [ForeignKey("InspectionId")]
        public Inspection? Inspection { get; set; }

        [Required]
        public int ChecklistItemId { get; set; }
        [ForeignKey("ChecklistItemId")]
        public InspectionChecklistItem? ChecklistItem { get; set; }

        [Required]
        public ComplianceStatus Status { get; set; } = ComplianceStatus.Compliant;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [MaxLength(500)]
        public string? CorrectiveAction { get; set; }
    }
}

