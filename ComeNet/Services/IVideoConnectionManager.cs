namespace ComeNet.Services
{
	public interface IVideoConnectionManager
	{
		void KeepUserConnection(string userId, string connectionId);
		void RemoveUserConnection(string connectionId);

		void RemoveUserConnectionStatus(string logoutuserid);

        List<string> GetUserConnections(string userId);
        List<string> GetAllUsers();
    }


	public class VideoConnectionManager : IUserConnectionManager
	{
        private static Dictionary<string, List<string>> VideoConnectionMap = new Dictionary<string, List<string>>();
        private static string VideoConnectionMapLocker = string.Empty;

        public void KeepUserConnection(string userId, string connectionId)
		{
			lock (VideoConnectionMapLocker)
			{
				if (!VideoConnectionMap.ContainsKey(userId))
				{
                    VideoConnectionMap[userId] = new List<string>();
				}
                VideoConnectionMap[userId].Add(connectionId);
			}
		}

        public void RemoveUserConnectionStatus(string logoutuserid)
        {

            lock (VideoConnectionMapLocker)
            {
                VideoConnectionMap.Remove(logoutuserid);
            }
        }


        public void RemoveUserConnection(string connectionId)
		{
			
			lock (VideoConnectionMapLocker)
			{
				foreach (var userId in VideoConnectionMap.Keys)
				{
					if (VideoConnectionMap.ContainsKey(userId))
					{
						if (VideoConnectionMap[userId].Contains(connectionId))
						{
                            VideoConnectionMap[userId].Remove(connectionId);							
                            break;
						}
					}
				}
			}
		}
		public List<string> GetUserConnections(string userId)
		{
			if(userId == null)
			{
                return null;
            }

			var conn = new List<string>();
			lock (VideoConnectionMapLocker)
			{
				conn = VideoConnectionMap[userId];
			}
			return conn;
		}

        public List<string> GetAllUsers()
        {
            List<string> allUsers;
            lock (VideoConnectionMapLocker)
            {
                allUsers = VideoConnectionMap.Keys.ToList();
            }
            return allUsers;
        }
    }


   
}
