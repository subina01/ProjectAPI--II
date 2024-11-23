using System;
using System.ComponentModel.DataAnnotations;

namespace Carrental.WebAPI.Models
{
    public class Return
    {
        
        public int Id { get; set; }

      
        public int BookingId { get; set; }

        public Booking? Booking { get; set; }

        [Required]
        public DateTime ActualReturnDate { get; set; }

        public string? DamageReported { get; set; }



        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string UserId { get; set; }
        public string? ReturnLocation { get; set; }

        public ReturnConfirmation? ReturnConfirmation { get; set; }
    }
}
