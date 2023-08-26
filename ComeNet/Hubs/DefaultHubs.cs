using Microsoft.AspNetCore.SignalR;

namespace WebRTC.Hubs
{
    public class DefaultHubs: Hub
    {

        public async Task JoinRoom(string roomId, string userId)
        {
            Users.list.Add(Context.ConnectionId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("user-connected", userId);            
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage( string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task Showname(string message)
        {
            await Clients.All.SendAsync("shownamelist", message);
        }
    }
}
