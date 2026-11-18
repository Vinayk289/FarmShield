using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class CorrectiveAction
    {
        [Key]
        public int ActionId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        public int? InspectionId { get; set; }
        [ForeignKey("InspectionId")]
        public Inspection? Inspection { get; set; }

        public int? DiseaseReportId { get; set; }
        [ForeignKey("DiseaseReportId")]
        public DiseaseReport? DiseaseReport { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AssignedTo { get; set; } = string.Empty;

        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

        public DateTime? CompletedDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Overdue

        [MaxLength(500)]
        public string? VerificationNotes { get; set; }
    }
}

