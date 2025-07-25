using Microsoft.AspNetCore.SignalR;

namespace HealthCareConsultation.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Notify others that someone joined
            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task SendMessage(string roomId, string sender, string message)
        {
            // Send message to all clients in the room
            await Clients.Group(roomId).SendAsync("ReceiveMessage", sender, message);
        }

        public async Task SendTyping(string roomId, string sender)
        {
            // Send typing indicator to others in the room
            await Clients.OthersInGroup(roomId).SendAsync("ShowTyping", sender);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle disconnection if needed
            await base.OnDisconnectedAsync(exception);
        }
    }
}