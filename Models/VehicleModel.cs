using System.Text.Json.Serialization;

namespace Carrental.WebAPI.Models
{
    public class VehicleModel
    {
        public int ModelId { get; set; }
        public string? VehicleModelName { get; set; }

        [JsonIgnore]
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
