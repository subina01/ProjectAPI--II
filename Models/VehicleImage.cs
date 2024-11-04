namespace Carrental.WebAPI.Models
{
    public class VehicleImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } 
        public int VehicleId { get; set; } 
        public Vehicle Vehicle { get; set; } 
    }
}
