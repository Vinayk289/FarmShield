using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.ViewModels
{
    public class BiosecurityDashboardViewModel
    {
        public int? SelectedFarmId { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();
        public List<BiosecurityActivity> Activities { get; set; } = new();
        public int TotalVisitorsLogged { get; set; }
        public int TotalCleaningsLogged { get; set; }
        public int TotalFeedTests { get; set; }
        public int TotalWaterTests { get; set; }
        public int TotalPestChecks { get; set; }
        public int TotalWasteDisposals { get; set; }
    }

    public class VisitorCreateViewModel
    {
        [Required(ErrorMessage = "Farm is required")]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Visitor name is required")]
        [Display(Name = "Visitor Full Name")]
        [MaxLength(100)]
        public string VisitorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [Display(Name = "Phone / Mobile Number")]
        [MaxLength(25)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Organization is required")]
        [Display(Name = "Company / Organization / Affiliation")]
        [MaxLength(150)]
        public string Organization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose of visit is required")]
        [Display(Name = "Purpose of Farm Entry")]
        [MaxLength(200)]
        public string PurposeOfVisit { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Entry Date & Time")]
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;

        [Display(Name = "Vehicle Registration Plate (if vehicle entering gate)")]
        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        [Display(Name = "Has visitor visited another pig/poultry farm within past 72 hours? (High Risk Alert)")]
        public bool VisitedAnotherFarmRecently { get; set; } = false;

        [Display(Name = "Details of previous farm visited")]
        [MaxLength(250)]
        public string? LastFarmVisitedDetails { get; set; }

        [Display(Name = "Vehicle tire spray & undercarriage decontamination completed")]
        public bool DisinfectionConfirmed { get; set; } = true;

        [Display(Name = "Sanitary shower-in taken at bio-gate")]
        public bool ShowerTaken { get; set; } = true;

        [Display(Name = "Clean protective coveralls and farm-specific sanitized boots issued")]
        public bool ProtectiveClothingIssued { get; set; } = true;

        [Display(Name = "Approval Status")]
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Approved;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class CleaningCreateViewModel
    {
        [Required]
        [Display(Name = "Select Farm")]
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Facility Area is required")]
        [Display(Name = "Facility / Pen / Barn Area")]
        [MaxLength(100)]
        public string FacilityArea { get; set; } = "Farrowing Barn A";

        [Required(ErrorMessage = "Equipment Sanitized is required")]
        [Display(Name = "Equipment Sanitized & Washed")]
        [MaxLength(100)]
        public string EquipmentSanitized { get; set; } = "Feeders, Water Nipples, Slats, Walls";

        [Display(Name = "Vehicle Disinfection Arch Operating & Recharged")]
        public bool VehicleDisinfectionStation { get; set; } = true;

        [Display(Name = "Entrance Footbaths Replenished with Fresh Solution")]
        public bool FootbathRefreshed { get; set; } = true;

        [Display(Name = "Handwashing Stations Stocked with Antibacterial Soap & Water")]
        public bool HandwashingSuppliesChecked { get; set; } = true;

        [Required]
        [Display(Name = "Disinfectant Chemical Brand")]
        [MaxLength(100)]
        public string DisinfectantUsed { get; set; } = "Virkon S (Peroxygen Formulation)";

        [Required]
        [Display(Name = "Dilution Ratio")]
        [MaxLength(50)]
        public string DilutionRatio { get; set; } = "1:100";

        [Required]
        [Display(Name = "Performed By (Staff Name)")]
        [MaxLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime CleaningDate { get; set; } = DateTime.UtcNow.Date;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class FeedCreateViewModel
    {
        [Required]
        public int FarmId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [MaxLength(100)]
        public string FeedType { get; set; } = "Grower Pellets";

        [Required]
        [MaxLength(60)]
        public string BatchLotNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string SupplierName { get; set; } = string.Empty;

        [Required]
        [Range(1.0, 100000.0)]
        public double QuantityKg { get; set; } = 5000;

        public bool QualityCheckPassed { get; set; } = true;
        public bool MoistureLevelOk { get; set; } = true;
        public bool NoMoldDetected { get; set; } = true;

        [MaxLength(100)]
        public string StorageSiloCondition { get; set; } = "Moisture-sealed & bird-proof";

        [Required]
        [MaxLength(100)]
        public string InspectedBy { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class WaterCreateViewModel
    {
        [Required]
        public int FarmId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TestingDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [MaxLength(100)]
        public string WaterSource { get; set; } = "Borewell Source #1";

        [Required]
        [Range(0.0, 14.0)]
        public double PhLevel { get; set; } = 6.8;

        [Required]
        [Range(0.0, 20.0)]
        public double ChlorinePpm { get; set; } = 3.0;

        [Required]
        [Range(0.0, 5000.0)]
        public double TdsLevel { get; set; } = 350.0;

        [Required]
        [MaxLength(80)]
        public string ColiformBacterialTestResult { get; set; } = "0 CFU / 100ml (Passed)";

        [Required]
        [MaxLength(100)]
        public string SanitizationMethod { get; set; } = "Automated Chlorination & UV Sterilization";

        public bool IsPotableSafe { get; set; } = true;

        [Required]
        [MaxLength(100)]
        public string TestedBy { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class PestControlCreateViewModel
    {
        [Required]
        public int FarmId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [MaxLength(80)]
        public string TargetPest { get; set; } = "Rodents & Flies";

        [Range(0, 500)]
        public int BaitStationsChecked { get; set; } = 25;

        [Range(0, 500)]
        public int TrapsServiced { get; set; } = 10;

        [Required]
        [MaxLength(30)]
        public string InfestationLevel { get; set; } = "None"; // None, Low, Moderate, High

        [Required]
        [MaxLength(150)]
        public string ChemicalOrMethodUsed { get; set; } = "Brodifacoum tamper bait & pyrethroid residual spray";

        [MaxLength(300)]
        public string? CorrectiveActionTaken { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NextScheduledDate { get; set; } = DateTime.UtcNow.Date.AddDays(14);

        [Required]
        [MaxLength(100)]
        public string InspectorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }

    public class WasteCreateViewModel
    {
        [Required]
        public int FarmId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DisposalDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [MaxLength(80)]
        public string WasteType { get; set; } = "Manure Compost";

        [Required]
        [MaxLength(80)]
        public string DisposalMethod { get; set; } = "High-Heat Composting Facility";

        [Required]
        [Range(0.1, 100000.0)]
        public double QuantityKg { get; set; } = 1500;

        public bool SanitationOfDisposalAreaChecked { get; set; } = true;

        [MaxLength(80)]
        public string? DisposalDepthOrTemp { get; set; } = "65°C Thermophilic Range";

        [Required]
        [MaxLength(100)]
        public string SupervisorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public List<Farm> AvailableFarms { get; set; } = new();
    }
}

