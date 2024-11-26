namespace Carrental.WebAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientUserName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
