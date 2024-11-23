using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleBrandController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehicleBrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Vehicle Brands
        [HttpGet]
        [Route("GetVehicleBrands")]
        [Authorize(Roles = "Admin,User,Dealer")]
        public IActionResult GetVehicleBrands()
        {
            return Ok(_context.VehicleBrands.ToList());
        }

       
        [HttpGet]
        [Route("GetVehicleBrand/{id}")]
        [Authorize(Roles = "Admin,User,Dealer")]
        public IActionResult GetVehicleBrand(int id)
        {
            var vehicleBrand = _context.VehicleBrands.FirstOrDefault(x => x.BrandId == id);
            if (vehicleBrand == null)
            {
                return NoContent();
            }
            return Ok(vehicleBrand);
        }

        
        [HttpPost]
        [Route("AddVehicleBrand")]
        [Authorize(Roles = "Admin,Dealer")]
        public IActionResult AddVehicleBrand(VehicleBrand vehicleBrand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.VehicleBrands.Add(vehicleBrand);
            _context.SaveChanges();

            return Ok("Vehicle Brand added successfully");
        }

        // Update a Vehicle Brand
        [HttpPut]
        [Route("UpdateVehicleBrand/{id}")]
        [Authorize(Roles = "Admin,Dealer")]
        public IActionResult UpdateVehicleBrand(int id, VehicleBrand vehicleBrand)
        {
            var existingVehicleBrand = _context.VehicleBrands.FirstOrDefault(x => x.BrandId == id);
            if (existingVehicleBrand == null)
            {
                return BadRequest("Vehicle Brand not found");
            }

            existingVehicleBrand.VehicleBrandName = vehicleBrand.VehicleBrandName; 

            _context.SaveChanges();

            return Ok("Vehicle Brand updated successfully");
        }

        // Delete a Vehicle Brand
        [HttpDelete]
        [Route("DeleteVehicleBrand/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteVehicleBrand(int id)
        {
            var vehicleBrand = _context.VehicleBrands.FirstOrDefault(x => x.BrandId == id);
            if (vehicleBrand == null)
            {
                return BadRequest("Vehicle Brand not found");
            }

            _context.VehicleBrands.Remove(vehicleBrand);
            _context.SaveChanges();
            _context.ReseedAllTables();

            return Ok("Vehicle Brand deleted successfully");
        }

        }
}
