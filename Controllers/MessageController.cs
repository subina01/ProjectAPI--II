using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Carrental.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            // Get the sender's UserId from JWT claims
            var senderId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId))
            {
                return Unauthorized("Invalid user.");
            }

            // Validate the recipient's username
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.UserName == messageDto.RecipientUserName);
            if (recipient == null)
            {
                return NotFound("Recipient not found.");
            }

            // Save the message to the database
            var message = new Message
            {
                SenderId = senderId,
                RecipientUserName = messageDto.RecipientUserName,
                Content = messageDto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Notify the recipient via SignalR
            await _hubContext.Clients.Group(messageDto.RecipientUserName).SendAsync("ReceiveMessage", messageDto.Content);

            return Ok("Message sent successfully.");
        }

    }
}
