namespace Carrental.WebAPI.Models
{
    public class UserDetailsDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string DriverLicInfo { get; set; }
        public string UserType { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}
