using System.ComponentModel.DataAnnotations;

namespace FarmShield.Models.Entities
{
    public class InspectionChecklistItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ItemTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? StandardReference { get; set; }

        public int Weight { get; set; } = 1;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

