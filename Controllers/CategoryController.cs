using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Vehicle Categories
        [HttpGet]
        [Route("GetVehicleCategories")]
        public IActionResult GetVehicleCategories()
        {
            return Ok(_context.VehicleCategories.ToList());
        }

        // Get a specific Vehicle Category by ID
        [HttpGet]
        [Route("GetVehicleCategory/{id}")]
        public IActionResult GetVehicleCategory(int id)
        {
            var category = _context.VehicleCategories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                return NoContent();
            }
            return Ok(category);
        }

        // Add a new Vehicle Category
        [HttpPost]
        [Route("AddVehicleCategory")]
        public IActionResult AddVehicleCategory([FromBody] VehicleCategory category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.VehicleCategories.Add(category);
            _context.SaveChanges();

            return Ok("Vehicle Category added successfully");
        }

        // Update an existing Vehicle Category
        [HttpPut]
        [Route("UpdateVehicleCategory/{id}")]
        public IActionResult UpdateVehicleCategory(int id, [FromBody] VehicleCategory category)
        {
            var existingCategory = _context.VehicleCategories.FirstOrDefault(x => x.CategoryId == id);
            if (existingCategory == null)
            {
                return BadRequest("Vehicle Category not found");
            }

            existingCategory.VehicleCategoryName = category.VehicleCategoryName;

            _context.SaveChanges();

            return Ok("Vehicle Category updated successfully");
        }

        // Delete a Vehicle Category
        [HttpDelete]
        [Route("DeleteVehicleCategory/{id}")]
        public IActionResult DeleteVehicleCategory(int id)
        {
            var category = _context.VehicleCategories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                return BadRequest("Vehicle Category not found");
            }

            _context.VehicleCategories.Remove(category);
            _context.SaveChanges();
            _context.ReseedAllTables();

            return Ok("Vehicle Category deleted successfully");
        }

    }
}
