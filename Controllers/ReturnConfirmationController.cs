using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnConfirmationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private const decimal LateFeeRate = 200; 

        public ReturnConfirmationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get total amount for a return
        [HttpGet("GetTotalAmount/{returnId}")]
        public IActionResult GetTotalAmount(int returnId)
        {
            var returnDetails = _context.Returns.Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
                .ThenInclude(v => v.Brand)
                .FirstOrDefault(r => r.Id == returnId);

            if (returnDetails == null)
            {
                return NotFound("Return not found.");
            }

            var pricingInfo = CalculateReturnPricing(returnDetails);

            return Ok(new
            {
                TotalAmountBeforeFees = pricingInfo.TotalBeforeFees,
                TotalLateFees = pricingInfo.TotalLateFees,
                DamageFee = pricingInfo.DamageFee,
                PaymentMethods = new[] { "Cash on Delivery", "PayPal" }
            });
        }

        [HttpPost("ConfirmReturn/{returnId}")]
        public IActionResult ConfirmReturn(int returnId, [FromBody] ReturnConfirmation confirmation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var returnDetails = _context.Returns
                .Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
                .FirstOrDefault(r => r.Id == returnId);

            if (returnDetails == null)
            {
                return NotFound("Return not found.");
            }

           
            if (confirmation.TotalAmount == 0)
            {
                var pricingInfo = CalculateReturnPricing(returnDetails);
                confirmation.TotalAmount = pricingInfo.TotalAmount;
                confirmation.DamageFee = pricingInfo.DamageFee;
                confirmation.TotalLateFees = pricingInfo.TotalLateFees;
                confirmation.TotalBeforeFees = pricingInfo.TotalBeforeFees;
            }

            confirmation.ReturnId = returnId;

            
            if (confirmation.TotalAmount > 0)
            {
                
                if (confirmation.PaymentMethod == "PayPal")
                {
                    if (string.IsNullOrEmpty(confirmation.Email))
                    {
                        return BadRequest("Email is required for PayPal payment.");
                    }

                    bool paymentSuccess = ProcessPayment(confirmation.Email, returnDetails);
                    if (!paymentSuccess)
                    {
                        return StatusCode(500, "Payment processing failed.");
                    }
                }
            }
            else
            {
                
                confirmation.PaymentMethod = null;
            }

            _context.ReturnConfirmations.Add(confirmation);
            returnDetails.Booking.Vehicle.Available = true;
            _context.SaveChanges();

            return Ok("Return confirmed successfully.");
        }


        [HttpGet("GetReturnConfirmationDetails/{returnId}")]
        public IActionResult GetReturnConfirmationDetails(int returnId)
        {
            var returnDetails = _context.Returns
                .Include(r => r.Booking)
                .ThenInclude(b => b.Vehicle)
                .Include(r => r.Booking.Vehicle.Brand)
                .Include(r => r.Booking.Vehicle.Category)
                .Include(r => r.Booking.Vehicle.Model)
                .Include(r => r.ReturnConfirmation)
                .FirstOrDefault(r => r.Id == returnId);

            if (returnDetails == null)
            {
                return NotFound("Return not found.");
            }

            if (returnDetails.ReturnConfirmation == null)
            {
                return NotFound("Return confirmation details not found.");
            }

            var response = new
            {
                ReturnConfirmationInfo = new
                {
                    returnDetails.ReturnConfirmation.Id,
                    returnDetails.ReturnConfirmation.TotalAmount,
                    returnDetails.ReturnConfirmation.DamageFee,
                    returnDetails.ReturnConfirmation.TotalLateFees,
                    returnDetails.ReturnConfirmation.PaymentMethod,
                    returnDetails.ReturnConfirmation.Email
                },
                ReturnInfo = new
                {
                    returnDetails.Id,
                    returnDetails.ActualReturnDate,
                    returnDetails.Booking.StartDate,
                    returnDetails.Booking.EndDate,
                    returnDetails.Booking.Place,
                    Vehicle = new
                    {
                        returnDetails.Booking.Vehicle.VehicleId,
                        returnDetails.Booking.Vehicle.Price,
                        Model = returnDetails.Booking.Vehicle.Model.VehicleModelName,
                        Brand = returnDetails.Booking.Vehicle.Brand.VehicleBrandName,
                        Category = returnDetails.Booking.Vehicle.Category.VehicleCategoryName,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{returnDetails.Booking.Vehicle.VehicleImages.FirstOrDefault()?.ImagePath}"
                    }
                }
            };

            return Ok(response);
        }

        private bool ProcessPayment(string email, Return returnDetails)
        {
           
            return true;
        }

        private (decimal TotalAmount, decimal DamageFee, decimal TotalBeforeFees, decimal TotalLateFees) CalculateReturnPricing(Return returnDetails)
        {
            DateTime scheduledEndDate = returnDetails.Booking.EndDate.GetValueOrDefault(DateTime.Now);
            DateTime actualReturnDate = returnDetails.ActualReturnDate;

            int totalLateDays = (actualReturnDate > scheduledEndDate) ? (actualReturnDate - scheduledEndDate).Days : 0;
            decimal totalLateFees = totalLateDays * LateFeeRate; 

            decimal damageFee = CalculateDamageFee(returnDetails.DamageReported);
            decimal totalBeforeFees = 0m;
            decimal totalAmount =   totalLateFees + damageFee;

            return (totalAmount, damageFee, totalBeforeFees, totalLateFees);
        }

        private decimal CalculateDamageFee(string? damageReported)
        {
            switch (damageReported)
            {
                case "No Damage":
                    return 0;
                case "Minor Scratches":
                    return 100;
                case "Dents":
                    return 300;
                case "Cracked Windshield":
                case "Broken Lights":
                    return 500;
                case "Engine Issues":
                case "Tire Damage":
                    return 700;
                default:
                    return 0;
            }
        }
    }
}
