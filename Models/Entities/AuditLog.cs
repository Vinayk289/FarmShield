using System.ComponentModel.DataAnnotations;

namespace FarmShield.Models.Entities
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string EntityName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? EntityId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Details { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? IpAddress { get; set; }
    }
}

