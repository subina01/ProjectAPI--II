using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingConfirmationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
       

        public BookingConfirmationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get total amount for a booking
        [HttpGet("GetTotalAmount/{bookingId}")]
        [Authorize(Roles = "User")]
        public IActionResult GetTotalAmount(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Vehicle)
                .ThenInclude(v => v.Brand)
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

          
            var pricingInfo = CalculateDynamicPrice(booking);

            return Ok(new
            {
                RentalCharge = booking.Vehicle.Brand.RentalCharge,
                TotalAmountBeforeDiscount = pricingInfo.TotalBeforeDiscount,
                DiscountAmount = pricingInfo.DiscountAmount,
                TotalAmount = pricingInfo.FinalAmount,
                PaymentMethods = new[] { "Cash on Delivery", "PayPal" }
            });
        }

        [HttpPost("ConfirmBooking/{bookingId}")]
        [Authorize(Roles = "User")]
        public IActionResult ConfirmBooking(int bookingId, [FromBody] BookingConfirmation confirmation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          

            var booking = _context.Bookings
                .Include(b => b.Vehicle)
                .ThenInclude(v => v.Brand)
                .FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            if (confirmation.TotalAmount == 0)
            {
                var pricingInfo = CalculateDynamicPrice(booking);
                confirmation.TotalAmount = pricingInfo.FinalAmount;
                confirmation.DiscountAmount = pricingInfo.DiscountAmount;
                confirmation.TotalBeforeDiscount = pricingInfo.TotalBeforeDiscount;
            }

            confirmation.BookingId = bookingId;

            if (confirmation.PaymentMethod == "PayPal")
            {
                if (string.IsNullOrEmpty(confirmation.Email))
                {
                    return BadRequest("Email is required for PayPal payment.");
                }

                bool paymentSuccess = ProcessPayment(confirmation.Email, booking);
                if (!paymentSuccess)
                {
                    return StatusCode(500, "Payment processing failed.");
                }
            }

            _context.BookingConfirmations.Add(confirmation);

           
            booking.Vehicle.Available = false;
            _context.SaveChanges();

            return Ok("Booking confirmed successfully.");
        }

        private bool ProcessPayment(string email, Booking booking)
        {
            return true;
        }

        [HttpGet("GetBookingConfirmationDetails/{bookingId}")]
        [Authorize(Roles = "User")]
        public IActionResult GetBookingConfirmationDetails(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.BookingConfirmation)
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (booking.UserId != userId)
            {
                return Forbid("You are not allowed to cancel this booking.");
            }

            if (booking.BookingConfirmation == null)
            {
                return NotFound("Booking confirmation details not found.");
            }


            var response = new
            {
                BookingConfirmationInfo = new
                {
                    booking.BookingConfirmation.Id,
                    booking.BookingConfirmation.TotalAmount,
                    booking.BookingConfirmation.DiscountAmount,
                    booking.BookingConfirmation.TotalBeforeDiscount,
                    booking.BookingConfirmation.PaymentMethod,
                    booking.BookingConfirmation.Email
                },
                BookingInfo = new
                {
                    booking.Id,
                    booking.StartDate,
                    booking.EndDate,
                    booking.Place,
                    booking.PhoneNumber,
                    booking.Email,
                    booking.Address,
                    booking.BillingAddress,
                    booking.InsuranceRequired,
                    booking.SpecialRequests,
                    Vehicle = new
                    {
                        booking.Vehicle.VehicleId,
                        booking.Vehicle.Price,
                        Model = booking.Vehicle.Model.VehicleModelName,
                        Brand = booking.Vehicle.Brand.VehicleBrandName,
                        Category = booking.Vehicle.Category.VehicleCategoryName,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{booking.Vehicle.VehicleImages.FirstOrDefault()?.ImagePath}"
                    }
                }
            };

            return Ok(response);
        }
        [HttpGet("GetAllBookingConfirmationDetails")]
        [Authorize(Roles = "User")]
        // Retrieve all bookings for the current user
        public IActionResult GetAllBookingConfirmationDetails()
        {
            
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

          
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId) 
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.BookingConfirmation)
                .ToList();

          
            if (!bookings.Any())
            {
                return NotFound("No bookings found for the user.");
            }

           
            var response = bookings.Select(booking => new
            {
                BookingConfirmationInfo = new
                {
                    booking.BookingConfirmation.Id,
                    booking.BookingConfirmation.TotalAmount,
                    booking.BookingConfirmation.DiscountAmount,
                    booking.BookingConfirmation.TotalBeforeDiscount,
                    booking.BookingConfirmation.PaymentMethod,
                    booking.BookingConfirmation.Email
                },
                BookingInfo = new
                {
                    booking.Id,
                    booking.StartDate,
                    booking.EndDate,
                    booking.Place,
                    booking.PhoneNumber,
                    booking.Email,
                    booking.Address,
                    booking.BillingAddress,
                    booking.InsuranceRequired,
                    booking.SpecialRequests,
                    Vehicle = new
                    {
                        booking.Vehicle.VehicleId,
                        booking.Vehicle.Price,
                        Model = booking.Vehicle.Model.VehicleModelName,
                        Brand = booking.Vehicle.Brand.VehicleBrandName,
                        Category = booking.Vehicle.Category.VehicleCategoryName,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{booking.Vehicle.VehicleImages.FirstOrDefault()?.ImagePath}"
                    }
                }
            });

            return Ok(response);
        }


        [HttpGet("GetAllBookingDetails")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllBookingDetails()
        {
            var bookings = _context.Bookings
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages) 
                .Include(b => b.BookingConfirmation)
                .ToList();

            if (!bookings.Any())
            {
                return NotFound("No bookings found.");
            }

            var response = bookings.Select(booking => new
            {
                BookingConfirmationInfo = new
                {
                    booking.BookingConfirmation.Id,
                    booking.BookingConfirmation.TotalAmount,
                    booking.BookingConfirmation.DiscountAmount,
                    booking.BookingConfirmation.TotalBeforeDiscount,
                    booking.BookingConfirmation.PaymentMethod,
                    booking.BookingConfirmation.Email
                },
                BookingInfo = new
                {
                    booking.Id,
                    booking.StartDate,
                    booking.EndDate,
                    booking.Place,
                    booking.PhoneNumber,
                    booking.Email,
                    booking.Address,
                    booking.BillingAddress,
                    booking.InsuranceRequired,
                    booking.SpecialRequests,
                    Vehicle = new
                    {
                        booking.Vehicle.VehicleId,
                        booking.Vehicle.Price,
                        Model = booking.Vehicle.Model.VehicleModelName,
                        Brand = booking.Vehicle.Brand.VehicleBrandName,
                        Category = booking.Vehicle.Category.VehicleCategoryName,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{booking.Vehicle.VehicleImages.FirstOrDefault()?.ImagePath}"
                    }
                }
            }).ToList();

            var responseWithCount = new
            {
                TotalCount = bookings.Count,
                Bookings = response
            };

            return Ok(response);
        }


        private (decimal FinalAmount, decimal DiscountAmount, decimal TotalBeforeDiscount) CalculateDynamicPrice(Booking booking)
        {
            var vehiclePrice = booking.Vehicle.Price;
            var brandCharges = booking.Vehicle.Brand.RentalCharge;

           
            var rentalDuration = (booking.EndDate - booking.StartDate)?.Days ?? 1;

            
            decimal basePrice = (vehiclePrice + brandCharges) * rentalDuration;

            
            var dayOfWeek = booking.StartDate.DayOfWeek;

            
            decimal discountPercentage = 0;

            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                    discountPercentage = 0.10m; 
                    break;
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    discountPercentage = 0.30m; 
                    break;
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    discountPercentage = 0.60m; 
                    break;
                default:
                    discountPercentage = 0; 
                    break;
            }

          
            if (rentalDuration > 7 && rentalDuration <= 14)
            {
                discountPercentage += 0.10m; 
            }
            else if (rentalDuration > 14)
            {
                discountPercentage += 0.20m; 
            }

           
            switch (booking.Vehicle.Damage) 
            {
                case "No Damage":
                    discountPercentage += 0.00m;
                    break;
                case "Minor Scratches":
                    discountPercentage += 0.05m; 
                    break;
                case "Dents":
                    discountPercentage += 0.02m; 
                    break;
                case "Cracked Windshield":
                case "Broken Lights":
                    discountPercentage += 0.01m; 
                    break;
                case "Engine Issues":
                case "Tire Damage":
                    discountPercentage += 0.05m; 
                    break;
                case "Interior Wear and Tear":
                case "Body Rust":
                case "Chassis Damage":
                case "Other":
                    discountPercentage += 0.06m; 
                    break;
                default:
                    break;
            }

           
            if (discountPercentage > 1.0m)
            {
                discountPercentage = 1.0m;
            }

            decimal discountAmount = basePrice * discountPercentage;
            decimal finalPrice = basePrice - discountAmount;

            return (finalPrice, discountAmount, basePrice);
        }

        [HttpDelete("CancelBooking/{bookingId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingConfirmation)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (booking.UserId != userId)
            {
                return Forbid("You are not allowed to cancel this booking.");
            }

            decimal refundAmount = 0;  

            try
            {
                decimal totalAmountPaid = 0;

                if (booking.BookingConfirmation != null)
                {
                    totalAmountPaid = booking.BookingConfirmation.TotalAmount;
                    decimal cancellationFee = totalAmountPaid * 0.02m;
                    refundAmount = totalAmountPaid - cancellationFee;

                   
                    _context.BookingConfirmations.Remove(booking.BookingConfirmation);
                }

                _context.Bookings.Remove(booking);
                _context.ReseedAllTables();
     
                await _context.SaveChangesAsync();

                
                return Ok(new
                {
                    Message = "Booking and confirmation canceled successfully.",
                    RefundAmount = refundAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAdminEarnings")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminEarnings()
        {
            
            var bookings = _context.Bookings
                .Include(b => b.BookingConfirmation)
                .Include(b => b.Vehicle)
                .ToList();

            if (!bookings.Any())
            {
                return NotFound("No bookings found.");
            }

            
            decimal totalAdminEarnings = 0;
            decimal totalDealerEarnings = 0;

            foreach (var booking in bookings)
            {
                if (booking.BookingConfirmation != null)
                {
                    decimal totalAmount = booking.BookingConfirmation.TotalAmount;
                    decimal adminEarnings = totalAmount * 0.40m; 
                    decimal dealerEarnings = totalAmount - adminEarnings; 

                    totalAdminEarnings += adminEarnings;
                    totalDealerEarnings += dealerEarnings;
                }
            }

            
            var response = new
            {
                TotalAdminEarnings = totalAdminEarnings,
                TotalDealerEarnings = totalDealerEarnings
            };

            return Ok(response);
        }


    }
}
