using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class DiseaseListViewModel
    {
        public List<DiseaseReport> Reports { get; set; } = new();
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public DiseaseSeverity? SeverityFilter { get; set; }
        public DiseaseVerificationStatus? StatusFilter { get; set; }
        public int TotalActiveOutbreaks { get; set; }
        public int TotalUnderInvestigation { get; set; }
        public int TotalResolvedCases { get; set; }
    }

    public class DiseaseReportCreateViewModel
    {
        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Display(Name = "Select Pig Tag (Optional - if swine case)")]
        public int? AnimalId { get; set; }

        [Display(Name = "Select Poultry Batch (Optional - if avian flock case)")]
        public int? PoultryBatchId { get; set; }

        [Required(ErrorMessage = "Disease Name or Suspected Syndrome is required")]
        [Display(Name = "Disease / Syndrome Name")]
        [MaxLength(120)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Disease Category")]
        [MaxLength(80)]
        public string DiseaseCategory { get; set; } = "Viral";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Detection")]
        public DateTime DetectionDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [Display(Name = "Severity Classification")]
        public DiseaseSeverity Severity { get; set; } = DiseaseSeverity.Medium;

        [Required(ErrorMessage = "Clinical symptoms are required")]
        [Display(Name = "Observable Clinical Signs & Symptoms")]
        [MaxLength(500)]
        public string Symptoms { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000, ErrorMessage = "At least 1 animal must be affected")]
        [Display(Name = "Number of Affected Animals / Birds")]
        public int AffectedCount { get; set; } = 1;

        [Range(0, 100000)]
        [Display(Name = "Mortality Count (if any deaths)")]
        public int MortalityCount { get; set; } = 0;

        [Display(Name = "Quarantine & Physical Isolation immediately enforced")]
        public bool IsolationImplemented { get; set; } = true;

        [Display(Name = "Preliminary Treatment / Medications Administered")]
        [MaxLength(500)]
        public string? TreatmentGiven { get; set; }

        [Display(Name = "Veterinary Verification Status")]
        public DiseaseVerificationStatus VerificationStatus { get; set; } = DiseaseVerificationStatus.Suspected;

        [Display(Name = "Veterinary Officer Clinical Notes / Differential Diagnosis")]
        [MaxLength(1000)]
        public string? VeterinaryNotes { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
        public List<Animal> AvailableAnimals { get; set; } = new();
        public List<PoultryBatch> AvailableBatches { get; set; } = new();
    }
}

