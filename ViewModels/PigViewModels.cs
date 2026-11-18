using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class PigListViewModel
    {
        public List<Animal> Pigs { get; set; } = new();
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public string? SearchTag { get; set; }
        public PigCategory? CategoryFilter { get; set; }
        public AnimalHealthStatus? HealthFilter { get; set; }
        public int TotalCount { get; set; }
        public int TotalHealthy { get; set; }
        public int TotalSickOrQuarantined { get; set; }
        public int TotalSows { get; set; }
        public int TotalBoars { get; set; }
        public int TotalGrowers { get; set; }
        public int TotalPiglets { get; set; }
    }

    public class PigCreateEditViewModel
    {
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Animal Identification Tag is required")]
        [Display(Name = "Tag Number / ID")]
        [MaxLength(30)]
        public string TagNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Breed is required")]
        [MaxLength(80)]
        public string Breed { get; set; } = "Large White Yorkshire";

        [Required]
        [Display(Name = "Animal Category")]
        public PigCategory Category { get; set; } = PigCategory.Grower;

        [Required]
        public AnimalGender Gender { get; set; } = AnimalGender.Female;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddMonths(-4);

        [Required]
        [Range(0.5, 600.0, ErrorMessage = "Weight must be between 0.5kg and 600kg")]
        [Display(Name = "Current Weight (kg)")]
        public double CurrentWeightKg { get; set; } = 45.0;

        [Required]
        [Display(Name = "Health Status")]
        public AnimalHealthStatus HealthCondition { get; set; } = AnimalHealthStatus.Healthy;

        [Required(ErrorMessage = "Pen / Barn Number is required")]
        [Display(Name = "Pen / Housing Unit")]
        [MaxLength(50)]
        public string PenBarnNumber { get; set; } = "Barn A - Pen 1";

        [Display(Name = "Dam (Mother) Tag")]
        [MaxLength(30)]
        public string? MotherTag { get; set; }

        [Display(Name = "Sire (Father) Tag")]
        [MaxLength(30)]
        public string? FatherTag { get; set; }

        [Display(Name = "Livestock Status")]
        public AnimalStatus Status { get; set; } = AnimalStatus.Active;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }
}

