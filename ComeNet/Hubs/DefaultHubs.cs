using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebRTC.Hubs
{
    
    public class DefaultHubs: Hub
    {

        private readonly IUserConnectionManager _videoConnectionManager;

        public DefaultHubs(IUserConnectionManager VideoConnectionManager)
        {
            _videoConnectionManager = VideoConnectionManager;
        }

        public async Task JoinRoom(string roomId, string userId)
        {
            Users.list.Add(Context.ConnectionId, userId);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).SendAsync("user-connected", userId);            
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);

            RealUsers.Userlist.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

       
    }
}
