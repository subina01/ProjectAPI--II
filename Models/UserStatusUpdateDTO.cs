namespace Carrental.WebAPI.Models
{
    public class UserStatusUpdateDTO
    {

        public bool IsBlocked { get; set; } 

        public DateTime? BlockedUntil { get; set; }
    }
}
