using ComeNet.Models;
using ComeNet.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebRTC.Hubs
{
	public class DefaultHubs : Hub
    {
		

		private readonly IUserConnectionManager _videoConnectionManager;

        public DefaultHubs(IUserConnectionManager VideoConnectionManager)
        {
            _videoConnectionManager = VideoConnectionManager;
        }

        private static Dictionary<string, List<string>> VideoConnectionMap = new Dictionary<string, List<string>>();
        private static string VideoConnectionMapLocker = string.Empty;

        private static Dictionary<string, List<string>> RoomMap = new Dictionary<string, List<string>>();
        private static string RoomMapLocker = string.Empty;

        public async Task JoinRoom(string roomId, string userId,string realId)
        {

            if (realId == null) return ;

            if (RoomMap.ContainsKey(roomId))
            {
                List<string> usersInRoom = RoomMap[roomId];

                    if (usersInRoom.Contains(realId))
                    {

                     return;

                    }
                    else
                    {

                    if (usersInRoom.Count > 2)
                    {
                        Console.WriteLine($"房間 '{roomId}' 中的用戶數大於2。");
                        return;
                    }
                    else
                    {

                        lock (RoomMapLocker)
                        {
                            if (!RoomMap.ContainsKey(roomId))
                            {
                                RoomMap[roomId] = new List<string>();
                            }
                            RoomMap[roomId].Add(realId);
                        }
                        lock (VideoConnectionMapLocker)
                        {
                            if (!VideoConnectionMap.ContainsKey(userId))
                            {
                                VideoConnectionMap[userId] = new List<string>();
                            }
                            VideoConnectionMap[userId].Add(Context.ConnectionId);
                        }

                        Users.list.Add(Context.ConnectionId, userId);

                        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

                        
                        await Clients.Group(roomId).SendAsync("user-connected", userId);

                    
                    }

                }


                   
            }
            else
            {

                lock (RoomMapLocker)
                {
                    if (!RoomMap.ContainsKey(roomId))
                    {
                        RoomMap[roomId] = new List<string>();
                    }
                    RoomMap[roomId].Add(realId);
                }
                Console.WriteLine($"房間 '{roomId}' 不存在。");
                Users.list.Add(Context.ConnectionId, userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                await Clients.Group(roomId).SendAsync("user-connected", userId);
            }

        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {

            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];


            if(userId != "")
            {
                lock (RoomMapLocker)
                {
                    foreach (var roomId in RoomMap.Keys)
                    {
                        if (RoomMap.ContainsKey(roomId))
                        {
                            if (RoomMap[roomId].Contains(userId))
                            {
                                
                                RoomMap[roomId].Remove(userId);
                                break;
                            }
                        }
                    }
                }

                lock (VideoConnectionMapLocker)
                {
                    foreach (var userIdinmap in VideoConnectionMap.Keys)
                    {
                        if (VideoConnectionMap.ContainsKey(userIdinmap))
                        {
                            if (VideoConnectionMap[userIdinmap].Contains(Context.ConnectionId))
                            {
                                VideoConnectionMap[userIdinmap].Remove(Context.ConnectionId);
                                
                                Clients.All.SendAsync("user-disconnected", userIdinmap);
                                break;
                            }
                        }
                    }
                }


            }


           

          

            




            return base.OnDisconnectedAsync(exception);
        }


    }
}