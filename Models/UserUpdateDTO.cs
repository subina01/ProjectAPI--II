namespace Carrental.WebAPI.Models
{
    public class UserUpdateDTO
    {

        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Address { get; set; }
        public string? DriverLicInfo { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; } 
    }
}
