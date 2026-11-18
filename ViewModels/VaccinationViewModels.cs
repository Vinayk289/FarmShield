using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class VaccinationListViewModel
    {
        public List<Vaccination> Vaccinations { get; set; } = new();
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public VaccinationStatus? StatusFilter { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalScheduled { get; set; }
        public int TotalOverdue { get; set; }
    }

    public class VaccinationCreateViewModel
    {
        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Display(Name = "Select Target Pig (Optional)")]
        public int? AnimalId { get; set; }

        [Display(Name = "Select Target Poultry Batch (Optional)")]
        public int? PoultryBatchId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required")]
        [Display(Name = "Vaccine Brand / Trade Name")]
        [MaxLength(120)]
        public string VaccineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target disease is required")]
        [Display(Name = "Target Disease Pathogen")]
        [MaxLength(120)]
        public string DiseaseTargeted { get; set; } = string.Empty;

        [Required(ErrorMessage = "Batch / Lot number is required")]
        [Display(Name = "Vaccine Batch / Lot Number")]
        [MaxLength(60)]
        public string BatchLotNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Administration Date")]
        public DateTime VaccinationDate { get; set; } = DateTime.UtcNow.Date;

        [DataType(DataType.Date)]
        [Display(Name = "Next Booster Due Date")]
        public DateTime? NextDueDate { get; set; }

        [Required]
        [Display(Name = "Administered Dosage")]
        [MaxLength(50)]
        public string DoseAdministered { get; set; } = "2.0 ml";

        [Required]
        [Display(Name = "Administration Route")]
        [MaxLength(50)]
        public string Route { get; set; } = "Intramuscular";

        [Required(ErrorMessage = "Administered By is required")]
        [Display(Name = "Administered By (Veterinarian / Technician)")]
        [MaxLength(100)]
        public string AdministeredBy { get; set; } = string.Empty;

        [Display(Name = "Vaccination Status")]
        public VaccinationStatus Status { get; set; } = VaccinationStatus.Completed;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
        public List<Animal> AvailableAnimals { get; set; } = new();
        public List<PoultryBatch> AvailableBatches { get; set; } = new();
    }
}

