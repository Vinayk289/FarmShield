using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int? FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public NotificationCategory Category { get; set; } = NotificationCategory.System;

        public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

        [MaxLength(200)]
        public string? TargetUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}

