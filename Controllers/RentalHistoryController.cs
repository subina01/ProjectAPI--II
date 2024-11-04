using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

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
        public async Task<IActionResult> GetRentalHistory()
        {
            
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var rentalHistory = await _context.Bookings
                .Include(b => b.Return)
                .Include(b => b.BookingConfirmation)
                .Where(b => b.Email == email)
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
                    Rating = b.Return != null ? b.Return.Rating : 0
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

