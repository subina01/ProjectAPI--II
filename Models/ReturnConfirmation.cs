using System.ComponentModel.DataAnnotations;

namespace Carrental.WebAPI.Models
{
    public class ReturnConfirmation
    {
        public int Id { get; set; }
        public int ReturnId { get; set; }
        public Return? Return { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DamageFee { get; set; }
        public decimal TotalBeforeFees { get; set; }
        public decimal TotalLateFees { get; set; }
        public string? PaymentMethod { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? ReturnLocation { get; set; }
    }
}
