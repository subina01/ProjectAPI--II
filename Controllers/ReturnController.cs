using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
        public class ReturnController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public ReturnController(ApplicationDbContext context)
            {
                _context = context;
            }

            // Add a new return
            [HttpPost("AddReturn")]
            [Authorize(Roles = "User")]
        public async Task<IActionResult> AddReturn([FromBody] Return returnData)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            returnData.UserId = userId;

            try
                {
                    
                    var bookingExists = await _context.Bookings.AnyAsync(b => b.Id == returnData.BookingId);
                    if (!bookingExists)
                    {
                        return BadRequest("Booking does not exist.");
                    }

                    _context.Returns.Add(returnData);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Return added successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            // Get all returns
            [HttpGet("GetReturns")]
            public IActionResult GetReturns()
            {
                var returns = _context.Returns
                    .Include(r => r.Booking)
                    .ToList();

                return Ok(returns);
            }

            // Get a specific return by ID
            [HttpGet("GetReturn/{id}")]
            public IActionResult GetReturn(int id)
            {
                var returnRecord = _context.Returns
                    .Include(r => r.Booking)
                    .FirstOrDefault(r => r.Id == id);

                if (returnRecord == null)
                {
                    return NoContent();
                }

                return Ok(returnRecord);
            }

            // Update a return
            [HttpPut("UpdateReturn/{id}")]
            [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateReturn(int id, [FromBody] Return returnData)
            {
                var existingReturn = await _context.Returns.FindAsync(id);
                if (existingReturn == null)
                {
                    return NotFound("Return not found.");
                }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (existingReturn.UserId != userId)
            {
                return Forbid("You are not allowed to modify this booking.");
            }

            try
                {
                    existingReturn.ActualReturnDate = returnData.ActualReturnDate;
                    existingReturn.DamageReported = returnData.DamageReported;
                    existingReturn.Rating = returnData.Rating;
                   existingReturn.ReturnLocation = returnData.ReturnLocation;

                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Return updated successfully." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            // Delete a return
            [HttpDelete("DeleteReturn/{id}")]
            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReturn(int id)
            {
                var returnRecord = await _context.Returns.FindAsync(id);
                if (returnRecord == null)
                {
                    return NotFound("Return not found.");
                }

                try
                {
                    _context.Returns.Remove(returnRecord);
                    await _context.SaveChangesAsync();

                    return Ok("Return deleted successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
