using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Visitor
    {
        [Key]
        public int VisitorId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        [MaxLength(100)]
        public string VisitorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Organization { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PurposeOfVisit { get; set; } = string.Empty;

        [Required]
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;

        public DateTime? ExitTime { get; set; }

        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        public bool VisitedAnotherFarmRecently { get; set; }

        [MaxLength(250)]
        public string? LastFarmVisitedDetails { get; set; }

        public bool DisinfectionConfirmed { get; set; }

        public bool ShowerTaken { get; set; }

        public bool ProtectiveClothingIssued { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Approved;

        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}

