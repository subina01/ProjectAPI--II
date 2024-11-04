using System.Text.Json.Serialization;

namespace Carrental.WebAPI.Models
{
    public class VehicleBrand
    {
        public int BrandId { get; set; }
        public string VehicleBrandName { get; set; }

        public decimal RentalCharge { get; set; }

        [JsonIgnore]
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
