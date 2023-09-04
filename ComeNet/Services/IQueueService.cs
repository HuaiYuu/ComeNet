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
        
        void Enqueue(ToolRequest request);
        ToolRequest Dequeue();
        int GetRequestCount();       
        void ClearQueue();
    }

    public class ToolRequest
    {
        public string ToolName { get; set; }
        public int ReceiverUserId { get; set; }
    }

    public class QueueService : IQueueService
    {
        private Queue<ToolRequest> requestQueue = new Queue<ToolRequest>();

        public void Enqueue(ToolRequest request)
        {


            requestQueue.Enqueue(request);
        }

        public ToolRequest Dequeue()
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
