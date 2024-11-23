using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleModelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehicleModelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Vehicle Models
        [HttpGet]
        [Route("GetVehicleModels")]
        [Authorize(Roles = "Admin,User,Dealer")]
        public IActionResult GetVehicleModels()
        {
            return Ok(_context.VehicleModels.ToList());
        }

        // Get a specific Vehicle Model by ID
        [HttpGet]
        [Route("GetVehicleModel/{id}")]
        [Authorize(Roles = "Admin,User,Dealer")]
        public IActionResult GetVehicleModel(int id)
        {
            var vehicleModel = _context.VehicleModels.FirstOrDefault(x => x.ModelId == id);
            if (vehicleModel == null)
            {
                return NoContent();
            }
            return Ok(vehicleModel);
        }

        // Add a new Vehicle Model
        [HttpPost]
        [Route("AddVehicleModel")]
        [Authorize(Roles = "Admin,Dealer")]
        public IActionResult AddVehicleModel(VehicleModel vehicleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.VehicleModels.Add(vehicleModel);
            _context.SaveChanges();

            return Ok("Vehicle Model added successfully");
        }

        // Update a Vehicle Model
        [HttpPut]
        [Route("UpdateVehicleModel/{id}")]
        [Authorize(Roles = "Admin,Dealer")]
        public IActionResult UpdateVehicleModel(int id, VehicleModel vehicleModel)
        {
            var existingVehicleModel = _context.VehicleModels.FirstOrDefault(x => x.ModelId == id);
            if (existingVehicleModel == null)
            {
                return BadRequest("Vehicle Model not found");
            }

            existingVehicleModel.VehicleModelName = vehicleModel.VehicleModelName;
            _context.SaveChanges();

            return Ok("Vehicle Model updated successfully");
        }

        // Delete a Vehicle Model
        [HttpDelete]
        [Route("DeleteVehicleModel/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteVehicleModel(int id)
        {
            var vehicleModel = _context.VehicleModels.FirstOrDefault(x => x.ModelId == id);
            if (vehicleModel == null)
            {
                return BadRequest("Vehicle Model not found");
            }

            _context.VehicleModels.Remove(vehicleModel);
            _context.SaveChanges();
            _context.ReseedAllTables();

            return Ok("Vehicle Model deleted successfully");
        }
    }
}