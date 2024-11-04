using Carrental.WebAPI.Models;
using System.Collections.Generic;

namespace Carrental.WebAPI.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public decimal Price { get; set; }
        public string? Detail { get; set; }
        public List<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
        public bool Popular { get; set; }
        public string? Damage { get; set; }
        public bool Available { get; set; }
        public int ModelId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }


        public VehicleModel? Model { get; set; }
        public VehicleBrand? Brand { get; set; }
        public VehicleCategory? Category { get; set; }
    }

}