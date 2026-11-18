using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class PoultryListViewModel
    {
        public List<PoultryBatch> Batches { get; set; } = new();
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public PoultryType? TypeFilter { get; set; }
        public PoultryBatchStatus? StatusFilter { get; set; }
        public int TotalBirdsInFlocks { get; set; }
        public int TotalBroilerFlocks { get; set; }
        public int TotalLayerFlocks { get; set; }
        public int TotalMortalityOverall { get; set; }
    }

    public class PoultryBatchCreateViewModel
    {
        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Batch Code is required")]
        [Display(Name = "Batch Identifier Code")]
        [MaxLength(30)]
        public string BatchCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Batch Name is required")]
        [Display(Name = "Batch Description / Name")]
        [MaxLength(100)]
        public string BatchName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Poultry Type")]
        public PoultryType PoultryType { get; set; } = PoultryType.Broiler;

        [Required(ErrorMessage = "Breed is required")]
        [MaxLength(80)]
        public string Breed { get; set; } = "Cobb 500";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Hatch Date")]
        public DateTime HatchDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [Range(10, 500000, ErrorMessage = "Initial Count must be between 10 and 500,000 birds")]
        [Display(Name = "Initial Bird Count (Chicks Placed)")]
        public int InitialBirdCount { get; set; } = 10000;

        [Required(ErrorMessage = "House / Shed Number is required")]
        [Display(Name = "House / Shed Number")]
        [MaxLength(50)]
        public string HouseNumber { get; set; } = "Shed #1";

        [Required]
        [Display(Name = "Housing System Type")]
        [MaxLength(80)]
        public string HousingType { get; set; } = "Deep Litter System";

        [Range(1.0, 5.0)]
        [Display(Name = "Target Feed Conversion Ratio (FCR)")]
        public double TargetFcr { get; set; } = 1.6;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class DailyLogCreateViewModel
    {
        public int BatchId { get; set; }
        public PoultryBatch Batch { get; set; } = default!;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Record Date")]
        public DateTime LogDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [Range(0, 10000, ErrorMessage = "Mortality cannot be negative")]
        [Display(Name = "Daily Mortality (Birds Died)")]
        public int MortalityCount { get; set; } = 0;

        [Required]
        [Range(0.0, 50000.0)]
        [Display(Name = "Feed Consumed (kg)")]
        public double FeedConsumedKg { get; set; } = 500.0;

        [Required]
        [Range(0.0, 100000.0)]
        [Display(Name = "Water Consumed (Liters)")]
        public double WaterConsumedLiters { get; set; } = 1000.0;

        [Range(0, 100000)]
        [Display(Name = "Total Eggs Collected (Layers only)")]
        public int EggsCollected { get; set; } = 0;

        [Range(0, 10000)]
        [Display(Name = "Damaged / Broken Eggs")]
        public int DamagedEggs { get; set; } = 0;

        [Range(0.0, 10000.0)]
        [Display(Name = "Average Bird Weight (grams)")]
        public double AverageWeightGrams { get; set; } = 1200.0;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

