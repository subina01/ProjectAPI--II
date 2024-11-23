using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Carrental.WebAPI.Models
{
    public class User : IdentityUser 
    {
        
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(150)]
        public string? Address { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string EmailId { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactNo { get; set; }

        [StringLength(50)]
        [RegularExpression("^[A-Z]{1,2}\\d{6,7}$", ErrorMessage = "Invalid driver license number.")]
        public string? DriverLicInfo { get; set; }

        public string UserType { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;
        public bool IsBlocked { get; set; } = false;
        public DateTime? BlockedUntil { get; set; }

      
    }
}
