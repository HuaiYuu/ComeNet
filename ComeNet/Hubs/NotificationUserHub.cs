using ComeNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http;
using WebRTC.Hubs;

namespace ComeNet.Hubs
{
	public class NotificationUserHub : Hub
	{

		private readonly IUserConnectionManager _userConnectionManager;
		public NotificationUserHub(IUserConnectionManager userConnectionManager)
		{
			_userConnectionManager = userConnectionManager;
		}
		public string GetConnectionId() 
		{
			var httpContext = this.Context.GetHttpContext();
			var userId = httpContext.Request.Query["userId"];
			
            _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);

			

			return Context.ConnectionId;
		}

        public string CreateGroup(string userId,string groupname)
        {
            _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
            Groups.AddToGroupAsync(Context.ConnectionId, groupname);

            return "ok";
        }

        public async Task JoinRoom(string roomId)
		{
            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
			await Clients.Group(roomId).SendAsync("user-connected", userId, roomId);           
        }
        

        public async override Task OnDisconnectedAsync(Exception exception)
		{
			//get the connectionId
			var connectionId = Context.ConnectionId;
			_userConnectionManager.RemoveUserConnection(connectionId);
			var value = await Task.FromResult(0);
		}
	}
}
