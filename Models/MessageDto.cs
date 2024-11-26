namespace Carrental.WebAPI.Models
{
    public class MessageDto
    {
        public string SenderId { get; set; }
        public string RecipientUserName { get; set; }
        public string Content { get; set; }
    }
}
