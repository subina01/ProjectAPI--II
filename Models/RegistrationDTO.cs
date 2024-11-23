using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Carrental.WebAPI.Models
{
    public class RegistrationDTO
    {

        public string? UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? EmailId { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContactNo { get; set; }

        [StringLength(150)]
        public string? Address { get; set; }


        [StringLength(50)]
        [RegularExpression("^[A-Z]{1,2}\\d{6,7}$", ErrorMessage = "Invalid driver license number.")]
        public string? DriverLicInfo { get; set; }

        public string? UserType { get; set; }
    }
}
