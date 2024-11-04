using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadsDirectory;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
            _uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        }

        private string GetLicenseImageUrl(string imagePath)
        {
            return $"{Request.Scheme}://{Request.Host}/api/Booking/images/{imagePath}";
        }

        [HttpGet("GetBookings")]
        public IActionResult GetBookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Model)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .ToList();

            foreach (var booking in bookings)
            {
                if (!string.IsNullOrEmpty(booking.LicenseImgPath))
                {
                    booking.LicenseImgPath = GetLicenseImageUrl(booking.LicenseImgPath);
                }

                if (booking.Vehicle != null && booking.Vehicle.VehicleImages != null)
                {
                    foreach (var image in booking.Vehicle.VehicleImages)
                    {
                        if (!string.IsNullOrEmpty(image.ImagePath))
                        {
                            image.ImagePath = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{image.ImagePath}";
                        }
                    }
                }
            }

            return Ok(bookings);
        }

        // Get a specific booking by ID
        [HttpGet("GetBooking/{id}")]
        public IActionResult GetBooking(int id)
        {
            var booking = _context.Bookings
                 .Include(b => b.Vehicle)
                 .ThenInclude(v => v.Model)
                 .Include(b => b.Vehicle)
                 .ThenInclude(v => v.Brand)
                 .Include(b => b.Vehicle)
                 .ThenInclude(v => v.Category)
                 .Include(b => b.Vehicle)
                 .ThenInclude(v => v.VehicleImages)
                 .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NoContent();
            }

            if (!string.IsNullOrEmpty(booking.LicenseImgPath))
            {
                booking.LicenseImgPath = GetLicenseImageUrl(booking.LicenseImgPath);
            }

            if (booking.Vehicle != null && booking.Vehicle.VehicleImages != null)
            {
                foreach (var image in booking.Vehicle.VehicleImages)
                {
                    if (!string.IsNullOrEmpty(image.ImagePath))
                    {
                        image.ImagePath = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/{image.ImagePath}";
                    }
                }
            }
            return Ok(booking);
        }
        // Assuming you have a User property that represents the current user
        [HttpPost("AddBooking")]
        public async Task<IActionResult> AddBooking([FromForm] Carrental.WebAPI.Models.Booking booking, IFormFile licenseImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          

            try
            {
                if (licenseImage != null && licenseImage.Length > 0)
                {
                    if (!Directory.Exists(_uploadsDirectory))
                    {
                        Directory.CreateDirectory(_uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(licenseImage.FileName);
                    var filePath = Path.Combine(_uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await licenseImage.CopyToAsync(stream);
                    }

                    booking.LicenseImgPath = fileName;
                }
                else
                {
                    booking.LicenseImgPath = "default-image.jpg";
                }

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Booking added successfully.",
                    LicenseImgUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{booking.LicenseImgPath}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateBooking/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromForm] Carrental.WebAPI.Models.Booking booking, IFormFile? licenseImage = null)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }

  
            try
            {
                existingBooking.StartDate = booking.StartDate;
                existingBooking.EndDate = booking.EndDate;
                existingBooking.Place = booking.Place;
                existingBooking.PhoneNumber = booking.PhoneNumber;
                existingBooking.Address = booking.Address;
                existingBooking.BillingAddress = booking.BillingAddress;
                existingBooking.InsuranceRequired = booking.InsuranceRequired;
                existingBooking.SpecialRequests = booking.SpecialRequests;

                if (licenseImage != null && licenseImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingBooking.LicenseImgPath))
                    {
                        var oldImagePath = Path.Combine(_uploadsDirectory, existingBooking.LicenseImgPath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Path.GetFileName(licenseImage.FileName);
                    var filePath = Path.Combine(_uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await licenseImage.CopyToAsync(stream);
                    }

                    existingBooking.LicenseImgPath = fileName;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Booking updated successfully.",
                    LicenseImgUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{existingBooking.LicenseImgPath}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Delete a booking
        [HttpDelete("DeleteBooking/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            try
            {
                if (!string.IsNullOrEmpty(booking.LicenseImgPath))
                {
                    var imagePath = Path.Combine(_uploadsDirectory, booking.LicenseImgPath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                return Ok("Booking deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get License Image by Booking ID
        [HttpGet("GetLicenseImage/{bookingId}")]
        public async Task<IActionResult> GetLicenseImage(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null || string.IsNullOrEmpty(booking.LicenseImgPath))
            {
                return NotFound("Image not found.");
            }

            var filePath = Path.Combine(_uploadsDirectory, booking.LicenseImgPath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found.");
            }

            var image = System.IO.File.ReadAllBytes(filePath);
            return File(image, "image/jpeg");
        }

        [HttpGet("images/{filename}")]
        public IActionResult GetLicenseImage(string filename)
        {
            var filePath = Path.Combine(_uploadsDirectory, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "image/jpeg");
        }
    }
}
