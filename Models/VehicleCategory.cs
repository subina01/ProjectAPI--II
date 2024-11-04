using System.Text.Json.Serialization;

namespace Carrental.WebAPI.Models
{
    public class VehicleCategory
    {

        public int CategoryId { get; set; }
        public string? VehicleCategoryName { get; set; }

        [JsonIgnore]
        public ICollection<Vehicle> Vehicles { get; set; }

    }
}
