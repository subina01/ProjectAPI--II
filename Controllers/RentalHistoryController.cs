using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RentalHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetRentalHistory")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetRentalHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var rentalHistory = await _context.Bookings
                .Include(b => b.Return)
                .Include(b => b.BookingConfirmation)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Where(b => b.UserId == userId)
                .Select(b => new RentalHistoryViewModel
                {
                    BookingId = b.Id,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Place = b.Place,
                    ActualReturnDate = b.Return != null ? b.Return.ActualReturnDate : (DateTime?)null,
                    DamageReported = b.Return != null ? b.Return.DamageReported : "Not Returned",
                    TotalAmount = b.BookingConfirmation.TotalAmount + (b.ReturnConfirmation != null ? b.ReturnConfirmation.TotalAmount : 0),
                    PaymentMethod = b.BookingConfirmation.PaymentMethod,
                    Rating = b.Return != null ? b.Return.Rating : 0,

                    VehicleModel = b.Vehicle.Model.VehicleModelName,
                    VehicleCategory = b.Vehicle.Category.VehicleCategoryName,
                    VehicleBrand = b.Vehicle.Brand.VehicleBrandName,
                    VehicleImages = b.Vehicle.VehicleImages
                        .Select(i => $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{i.ImagePath}")
                        .ToList()
                })
                .ToListAsync();

            if (!rentalHistory.Any())
            {
                return NotFound("No rental history found for this user.");
            }

            return Ok(rentalHistory);
        }



        [HttpGet("GetAllRentalHistory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRentalHistory()
        {
            var rentalHistory = await _context.Bookings
                .Include(b => b.Return)
                .Include(b => b.ReturnConfirmation) 
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.BookingConfirmation)
                .Select(b => new RentalHistoryViewModel
                {
                    BookingId = b.Id,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Place = b.Place,
                    ActualReturnDate = b.Return != null ? b.Return.ActualReturnDate : (DateTime?)null,
                    DamageReported = b.Return != null ? b.Return.DamageReported : "Not Returned",
                    TotalAmount = b.BookingConfirmation.TotalAmount + (b.ReturnConfirmation != null ? b.ReturnConfirmation.TotalAmount : 0),
                    PaymentMethod = b.BookingConfirmation.PaymentMethod,
                    Rating = b.Return != null ? b.Return.Rating : 0,
                    VehicleModel = b.Vehicle.Model != null ? b.Vehicle.Model.VehicleModelName : "Unknown Model",
                    VehicleBrand = b.Vehicle.Brand != null ? b.Vehicle.Brand.VehicleBrandName : "Unknown Brand",
                    VehicleCategory = b.Vehicle.Category != null ? b.Vehicle.Category.VehicleCategoryName : "Unknown Category",
                    VehicleImageUrl = b.Vehicle.VehicleImages != null && b.Vehicle.VehicleImages.Count > 0
                ? $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{b.Vehicle.VehicleImages.First().ImagePath}"
                : "No Image Available"
                })
                .ToListAsync();

            if (!rentalHistory.Any())
            {
                return NotFound("No rental history found.");
            }

            return Ok(rentalHistory);
        }


    }
}

