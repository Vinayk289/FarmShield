using System.ComponentModel.DataAnnotations;
using FarmShield.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace FarmShield.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = AppRoles.FarmManager;

        public int? AssignedFarmId { get; set; }
        public Farm? AssignedFarm { get; set; }

        [MaxLength(100)]
        public string? Designation { get; set; }

        [MaxLength(150)]
        public string? Specialization { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Farm> OwnedFarms { get; set; } = new List<Farm>();
        public ICollection<Inspection> ConductedInspections { get; set; } = new List<Inspection>();
    }
}

