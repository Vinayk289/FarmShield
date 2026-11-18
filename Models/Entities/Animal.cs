using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FarmShield.Models.Enums;

namespace FarmShield.Models.Entities
{
    public class Animal
    {
        [Key]
        public int AnimalId { get; set; }

        [Required]
        public int FarmId { get; set; }
        [ForeignKey("FarmId")]
        public Farm? Farm { get; set; }

        [Required]
        [MaxLength(30)]
        public string TagNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Breed { get; set; } = string.Empty;

        [Required]
        public PigCategory Category { get; set; } = PigCategory.Grower;

        [Required]
        public AnimalGender Gender { get; set; } = AnimalGender.Female;

        public DateTime DateOfBirth { get; set; }

        [Range(0.5, 600.0)]
        public double CurrentWeightKg { get; set; }

        public AnimalHealthStatus HealthCondition { get; set; } = AnimalHealthStatus.Healthy;

        [Required]
        [MaxLength(50)]
        public string PenBarnNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? MotherTag { get; set; }

        [MaxLength(30)]
        public string? FatherTag { get; set; }

        public AnimalStatus Status { get; set; } = AnimalStatus.Active;

        public DateTime? MortalityDate { get; set; }

        [MaxLength(250)]
        public string? MortalityReason { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
        public ICollection<DiseaseReport> DiseaseReports { get; set; } = new List<DiseaseReport>();
    }
}

