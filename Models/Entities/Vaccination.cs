using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Vaccination
    {
        [Key]
        public int VaccinationId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        public int? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; }

        public int? PoultryBatchId { get; set; }
        [ForeignKey("PoultryBatchId")]
        public PoultryBatch? PoultryBatch { get; set; }

        [Required]
        [MaxLength(120)]
        public string VaccineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string DiseaseTargeted { get; set; } = string.Empty;

        [Required]
        [MaxLength(60)]
        public string BatchLotNumber { get; set; } = string.Empty;

        [Required]
        public DateTime VaccinationDate { get; set; } = DateTime.UtcNow;

        public DateTime? NextDueDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string DoseAdministered { get; set; } = "2 ml";

        [Required]
        [MaxLength(50)]
        public string Route { get; set; } = "Intramuscular"; // Subcutaneous, Intramuscular, Drinking Water, Spray, Eye Drop

        [Required]
        [MaxLength(100)]
        public string AdministeredBy { get; set; } = string.Empty;

        public VaccinationStatus Status { get; set; } = VaccinationStatus.Completed;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

