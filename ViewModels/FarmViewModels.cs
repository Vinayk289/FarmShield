using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class FarmListViewModel
    {
        public List<Farm> Farms { get; set; } = new();
        public string? SearchTerm { get; set; }
        public FarmType? FilterType { get; set; }
        public FarmStatus? FilterStatus { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }
    }

    public class FarmCreateEditViewModel
    {
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Farm Name is required")]
        [Display(Name = "Farm Name")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farm Owner Name is required")]
        [Display(Name = "Owner Full Name")]
        [MaxLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Farm Type")]
        public FarmType FarmType { get; set; } = FarmType.Pig;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "District / County is required")]
        [MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "State / Province is required")]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [Phone]
        [Display(Name = "Contact Phone")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(0.5, 50000.0, ErrorMessage = "Area must be between 0.5 and 50,000 acres")]
        [Display(Name = "Farm Area (Acres)")]
        public double FarmAreaAcres { get; set; } = 50.0;

        [Required]
        [Range(1, 1000000, ErrorMessage = "Capacity must be at least 1")]
        [Display(Name = "Animal / Bird Capacity")]
        public int TotalCapacity { get; set; } = 2500;

        [Display(Name = "Farm Status")]
        public FarmStatus Status { get; set; } = FarmStatus.Active;

        [Display(Name = "BioSafety Level")]
        public BioSafetyLevel BioSafetyLevel { get; set; } = BioSafetyLevel.Level2_Enhanced;

        [MaxLength(500)]
        [Display(Name = "Farm Description & Perimeter Features")]
        public string? Description { get; set; }

        [Display(Name = "Farm Image URL")]
        public string? ImageUrl { get; set; }
    }

    public class FarmDetailsViewModel
    {
        public Farm Farm { get; set; } = default!;
        public int TotalPigs { get; set; }
        public int TotalBirds { get; set; }
        public int ActiveDiseaseAlerts { get; set; }
        public double LatestComplianceRate { get; set; }
        public List<Inspection> RecentInspections { get; set; } = new();
        public List<Visitor> RecentVisitors { get; set; } = new();
        public List<BiosecurityActivity> RecentActivities { get; set; } = new();
        public List<DiseaseReport> ActiveDiseases { get; set; } = new();
        public List<Vaccination> UpcomingVaccinations { get; set; } = new();
    }
}

