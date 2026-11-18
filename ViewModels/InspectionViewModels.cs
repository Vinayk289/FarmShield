using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class InspectionListViewModel
    {
        public List<Inspection> Inspections { get; set; } = new();
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public InspectionStatus? StatusFilter { get; set; }
        public RiskLevel? RiskFilter { get; set; }
        public double AverageComplianceScore { get; set; }
        public int CertifiedFarmsCount { get; set; }
    }

    public class InspectionChecklistResponseInput
    {
        public int ChecklistItemId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ItemTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public ComplianceStatus Status { get; set; } = ComplianceStatus.Compliant;
        public string? Remarks { get; set; }
        public string? CorrectiveAction { get; set; }
    }

    public class InspectionCreateViewModel
    {
        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm to Inspect")]
        public int FarmId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Inspection Date")]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow.Date;

        [Required(ErrorMessage = "Auditor Name is required")]
        [Display(Name = "Veterinary Auditor / Inspector Name")]
        [MaxLength(100)]
        public string InspectorName { get; set; } = string.Empty;

        [Display(Name = "Auditor Designation")]
        [MaxLength(100)]
        public string InspectorDesignation { get; set; } = "Veterinary Biosecurity Auditor";

        [MaxLength(1000)]
        [Display(Name = "General Observations & Violations Summary")]
        public string? ViolationsIdentified { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Inspector Recommendations")]
        public string? Recommendations { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Required Corrective Actions & Deadlines")]
        public string? CorrectiveActionRequired { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
        public List<InspectionChecklistResponseInput> Items { get; set; } = new();
    }

    public class InspectionDetailsViewModel
    {
        public Inspection Inspection { get; set; } = default!;
        public List<InspectionResponse> Responses { get; set; } = new();
        public List<CorrectiveAction> CorrectiveActions { get; set; } = new();
    }
}

