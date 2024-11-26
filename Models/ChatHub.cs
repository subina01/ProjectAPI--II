using Microsoft.AspNetCore.SignalR;

namespace Carrental.WebAPI.Models
{
    public class ChatHub : Hub
    {
        // This method is triggered when a client connects to the SignalR Hub
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value; // Extract userId from JWT claims
            if (!string.IsNullOrEmpty(userId))
            {
                // Add the connection to a SignalR group identified by the user's username
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync(); // Call the base implementation
        }

        // Optionally, handle disconnects as well
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove the connection from the group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // Send a message to the recipient
        public async Task SendMessageToUser(string recipientUserName, string messageContent)
        {
            // Broadcast the message to the recipient user group
            await Clients.Group(recipientUserName).SendAsync("ReceiveMessage", messageContent);
        }
    }

}
