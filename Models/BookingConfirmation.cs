using Carrental.WebAPI.Models;
using System.ComponentModel.DataAnnotations;

public class BookingConfirmation
{
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalBeforeDiscount { get; set; }
    [Required]
    public string? PaymentMethod { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
}
