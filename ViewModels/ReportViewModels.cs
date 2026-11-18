using FarmShield.Models.Entities;

namespace FarmShield.ViewModels
{
    public class ReportFilterViewModel
    {
        public string ReportType { get; set; } = "compliance";
        public int? FarmId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();

        // Datasets for report preview
        public List<Farm> Farms { get; set; } = new();
        public List<Animal> Animals { get; set; } = new();
        public List<PoultryBatch> PoultryBatches { get; set; } = new();
        public List<Inspection> Inspections { get; set; } = new();
        public List<DiseaseReport> DiseaseReports { get; set; } = new();
        public List<Vaccination> Vaccinations { get; set; } = new();
        public List<Visitor> Visitors { get; set; } = new();
        public List<CleaningRecord> CleaningRecords { get; set; } = new();
        public List<FeedRecord> FeedRecords { get; set; } = new();
        public List<WaterQualityRecord> WaterQualityRecords { get; set; } = new();
        public List<PestControlRecord> PestControlRecords { get; set; } = new();
        public List<WasteManagementRecord> WasteRecords { get; set; } = new();
    }
}

