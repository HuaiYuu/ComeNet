﻿using ComeNet.Services;
using Microsoft.AspNetCore.SignalR;
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
            Groups.AddToGroupAsync(Context.ConnectionId,"19to19");

            Clients.Group( "19to19").SendAsync("user-connected", userId);
            return Context.ConnectionId;
		}

        public string CreateGroup(string userId,string groupname)
        {

            _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
            Groups.AddToGroupAsync(Context.ConnectionId, groupname);

            return "ok";
        }

        public async Task JoinRoom(string roomId, string userId)
		{
			Users.list.Add(Context.ConnectionId, userId);
			await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
			await Clients.Group(roomId).SendAsync("user-connected", userId);
		}

		//Called when a connection with the hub is terminated.
		public async override Task OnDisconnectedAsync(Exception exception)
		{
			//get the connectionId
			var connectionId = Context.ConnectionId;
			_userConnectionManager.RemoveUserConnection(connectionId);
			var value = await Task.FromResult(0);//adding dump code to follow the template of Hub > OnDisconnectedAsync
		}
	}
}
