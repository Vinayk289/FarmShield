using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class BiosecurityActivity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        public BiosecurityCategory Category { get; set; }

        [Required]
        [MaxLength(100)]
        public string ActivityType { get; set; } = string.Empty;

        [Required]
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        public string ResponsiblePerson { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public ComplianceStatus ComplianceStatus { get; set; } = ComplianceStatus.Compliant;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [MaxLength(255)]
        public string? DocumentAttachmentPath { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}

