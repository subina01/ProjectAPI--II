using System;
using System.ComponentModel.DataAnnotations;


namespace Carrental.WebAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; } 

        [Required]
        public DateTime? EndDate { get; set; } 
        [MaxLength(200)]
        public string? LicenseImgPath { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Place { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Required]
        [MaxLength(10)]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? BillingAddress { get; set; }

        public bool InsuranceRequired { get; set; }

        public string? SpecialRequests { get; set; }
        public string UserId { get; set; }

        public BookingConfirmation? BookingConfirmation { get; set; }
        public ReturnConfirmation? ReturnConfirmation { get; set; }



        public Return? Return { get; set; }

    


    }
}
