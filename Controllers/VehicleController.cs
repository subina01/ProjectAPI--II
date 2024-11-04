using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadsDirectory;

        public VehicleController(ApplicationDbContext context)
        {
            _context = context;
            _uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        }

        // Get all Vehicles
        [HttpGet("GetVehicles")]
        public IActionResult GetVehicles()
        {
            var vehicles = _context.Vehicles
                 .AsNoTracking()
                .Include(v => v.VehicleImages)
                .Include(v => v.Category)
                .Include(v => v.Model)
                .Include(v => v.Brand)
                .ToList();


            string baseUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/";



            foreach (var vehicle in vehicles)
            {
                if (vehicle.VehicleImages != null && vehicle.VehicleImages.Count > 0)
                {
                    foreach (var image in vehicle.VehicleImages)
                    {

                        image.ImagePath = $"{baseUrl}{image.ImagePath}";
                    }
                }
            }

            return Ok(vehicles);
        }

        // Get a specific Vehicle by ID
        [HttpGet("GetVehicle/{id}")]
        public IActionResult GetVehicle(int id)
        {
            var vehicle = _context.Vehicles
                 .AsNoTracking()
                .Include(v => v.VehicleImages)
                .Include(v => v.Category)
                .Include(v => v.Model)
                .Include(v => v.Brand)
                .FirstOrDefault(v => v.VehicleId == id);

            if (vehicle == null)
            {
                return NoContent();
            }


            string baseUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/";


            if (vehicle.VehicleImages != null && vehicle.VehicleImages.Count > 0)
            {
                foreach (var image in vehicle.VehicleImages)
                {

                    image.ImagePath = $"{baseUrl}{image.ImagePath}";
                }
            }

            return Ok(vehicle);
        }

        // Add a new Vehicle
        [HttpPost]
        [Route("AddVehicle")]
        public async Task<IActionResult> AddVehicle([FromForm] Vehicle vehicle, List<IFormFile> imageFiles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.VehicleBrands.Any(b => b.BrandId == vehicle.BrandId) ||
                !_context.VehicleCategories.Any(c => c.CategoryId == vehicle.CategoryId) ||
                !_context.VehicleModels.Any(m => m.ModelId == vehicle.ModelId))
            {
                return BadRequest("Invalid BrandId, CategoryId, or ModelId.");
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            if (!Directory.Exists(_uploadsDirectory))
            {
                Directory.CreateDirectory(_uploadsDirectory);
            }

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var imagePath = Path.Combine(_uploadsDirectory, file.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var vehicleImage = new VehicleImage
                        {
                            ImagePath = file.FileName,
                            VehicleId = vehicle.VehicleId
                        };

                        _context.VehicleImages.Add(vehicleImage);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return Ok("Vehicle added successfully with images.");
        }

        // Update a Vehicle
        [HttpPut("UpdateVehicle/{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromForm] Vehicle vehicle, [FromForm] List<IFormFile> vehicleImages)
        {
            var existingVehicle = await _context.Vehicles.Include(v => v.VehicleImages).FirstOrDefaultAsync(v => v.VehicleId == id);
            if (existingVehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            existingVehicle.VehicleImages.Clear();

            if (vehicleImages != null && vehicleImages.Count > 0)
            {
                foreach (var image in vehicleImages)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var filePath = Path.Combine(_uploadsDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var vehicleImage = new VehicleImage
                        {
                            ImagePath = fileName,
                            VehicleId = existingVehicle.VehicleId
                        };
                        existingVehicle.VehicleImages.Add(vehicleImage);
                    }
                }
            }

            existingVehicle.Price = vehicle.Price;
            existingVehicle.Detail = vehicle.Detail;
            existingVehicle.Popular = vehicle.Popular;
            existingVehicle.Damage = vehicle.Damage;
            existingVehicle.Available = vehicle.Available;
            existingVehicle.ModelId = vehicle.ModelId;
            existingVehicle.BrandId = vehicle.BrandId;
            existingVehicle.CategoryId = vehicle.CategoryId;

            await _context.SaveChangesAsync();

            return Ok("Vehicle updated successfully.");
        }

        // Delete a Vehicle
        [HttpDelete("DeleteVehicle/{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
            _context.ReseedAllTables();

            return Ok("Vehicle deleted successfully");
        }

        // Get Vehicles By Category
        [HttpGet("GetVehiclesByCategory/{categoryId}")]
        public IActionResult GetVehiclesByCategory(int categoryId)
        {
            var vehicles = _context.Vehicles
                .Where(v => v.CategoryId == categoryId)
                 .AsNoTracking()
                .Include(v => v.Category)
                .Include(v => v.Model)
                .Include(v => v.Brand)
                .Include(v => v.VehicleImages)
                .ToList();

            if (vehicles == null || vehicles.Count == 0)
            {
                return NotFound($"No vehicles found for category id {categoryId}");
            }


            string baseUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/";


            foreach (var vehicle in vehicles)
            {
                if (vehicle.VehicleImages != null && vehicle.VehicleImages.Count > 0)
                {
                    foreach (var image in vehicle.VehicleImages)
                    {
                        image.ImagePath = $"{baseUrl}{image.ImagePath}";
                    }
                }
            }

            return Ok(vehicles);
        }

        // Get popular vehicles
        [HttpGet("GetPopularVehicles")]
        public IActionResult GetPopularVehicles()
        {
            var vehicles = _context.Vehicles
                .Where(v => v.Popular == true)
                 .AsNoTracking()
                .Include(v => v.Category)
                .Include(v => v.Model)
                .Include(v => v.Brand)
                .Include(v => v.VehicleImages)
                .ToList();

            if (vehicles == null || vehicles.Count == 0)
            {
                return NotFound("No popular vehicles found.");
            }


            string baseUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/";


            foreach (var vehicle in vehicles)
            {
                if (vehicle.VehicleImages != null && vehicle.VehicleImages.Count > 0)
                {
                    foreach (var image in vehicle.VehicleImages)
                    {
                        image.ImagePath = $"{baseUrl}{image.ImagePath}";
                    }
                }
            }

            return Ok(vehicles);
        }

        // Get all available Vehicles
        [HttpGet("GetAvailableVehicles")]
        public IActionResult GetAvailableVehicles()
        {
            var vehicles = _context.Vehicles
                .AsNoTracking()
                .Where(v => v.Available == true)
                .Include(v => v.VehicleImages)
                .Include(v => v.Category)
                .Include(v => v.Model)
                .Include(v => v.Brand)
                .ToList();

            string baseUrl = $"{Request.Scheme}://{Request.Host}/api/Vehicle/images/";

            foreach (var vehicle in vehicles)
            {
                if (vehicle.VehicleImages != null && vehicle.VehicleImages.Count > 0)
                {
                    foreach (var image in vehicle.VehicleImages)
                    {
                        image.ImagePath = $"{baseUrl}{image.ImagePath}";
                    }
                }
            }

            return Ok(vehicles);
        }

        // Endpoint to get vehicle images
        [HttpGet("images/{filename}")]
        public IActionResult GetImage(string filename)
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
