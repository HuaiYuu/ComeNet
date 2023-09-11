namespace ComeNet.Services
{
	public interface IUserConnectionManager
	{
		void KeepUserConnection(string userId, string connectionId);
		void RemoveUserConnection(string connectionId);

		void RemoveUserConnectionStatus(string logoutuserid);

        List<string> GetUserConnections(string userId);
        List<string> GetAllUsers();
    }


	public class UserConnectionManager : IUserConnectionManager
	{
		private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
		private static string userConnectionMapLocker = string.Empty;

		public void KeepUserConnection(string userId, string connectionId)
		{
			lock (userConnectionMapLocker)
			{
				if (!userConnectionMap.ContainsKey(userId))
				{
					userConnectionMap[userId] = new List<string>();
				}
				userConnectionMap[userId].Add(connectionId);
			}
		}

        public void RemoveUserConnectionStatus(string logoutuserid)
        {

            lock (userConnectionMapLocker)
            {
             userConnectionMap.Remove(logoutuserid);
            }
        }


        public void RemoveUserConnection(string connectionId)
		{
			
			lock (userConnectionMapLocker)
			{
				foreach (var userId in userConnectionMap.Keys)
				{
					if (userConnectionMap.ContainsKey(userId))
					{
						if (userConnectionMap[userId].Contains(connectionId))
						{
							userConnectionMap[userId].Remove(connectionId);							
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
			lock (userConnectionMapLocker)
			{
				conn = userConnectionMap[userId];
			}
			return conn;
		}

        public List<string> GetAllUsers()
        {
            List<string> allUsers;
            lock (userConnectionMapLocker)
            {
                allUsers = userConnectionMap.Keys.ToList();
            }
            return allUsers;
        }
    }
}
