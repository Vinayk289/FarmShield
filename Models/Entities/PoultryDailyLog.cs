using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmShield.Models.Entities
{
    public class PoultryDailyLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int BatchId { get; set; }
        [ForeignKey("BatchId")]
        public PoultryBatch? Batch { get; set; }

        [Required]
        public DateTime LogDate { get; set; } = DateTime.UtcNow.Date;

        [Range(0, 50000)]
        public int MortalityCount { get; set; }

        [Range(0.0, 50000.0)]
        public double FeedConsumedKg { get; set; }

        [Range(0.0, 100000.0)]
        public double WaterConsumedLiters { get; set; }

        [Range(0, 100000)]
        public int EggsCollected { get; set; }

        [Range(0, 10000)]
        public int DamagedEggs { get; set; }

        [Range(0.0, 10000.0)]
        public double AverageWeightGrams { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(100)]
        public string RecordedBy { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}

