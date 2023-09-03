using AWSWEBAPP.Models;
using ComeNet.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AWSWEBAPP.Services
{
    public interface IQueueService
    {
        // 將好友請求加入佇列
        void Enqueue(FriendRequest request);

        // 從佇列中取出下一個好友請求
        FriendRequest Dequeue();

        // 獲取佇列中的請求數量
        int GetRequestCount();

        // 刪除佇列中的所有請求
        void ClearQueue();
    }

    public class FriendRequest
    {
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
    }

    public class InMemoryQueueService : IQueueService
    {
        private Queue<FriendRequest> requestQueue = new Queue<FriendRequest>();

        public void Enqueue(FriendRequest request)
        {
            requestQueue.Enqueue(request);
        }

        public FriendRequest Dequeue()
        {
            return requestQueue.Count > 0 ? requestQueue.Dequeue() : null;
        }

        public int GetRequestCount()
        {
            return requestQueue.Count;
        }

        public void ClearQueue()
        {
            requestQueue.Clear();
        }
    }
}
