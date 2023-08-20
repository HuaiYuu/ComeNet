using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using Amazon.S3.Transfer;
using Amazon.S3;
using static AWSWEBAPP.Models.Aws;
using System.Configuration;
using Amazon.CloudWatchLogs;
using Amazon;

namespace AWSWEBAPP.Services
{
    public interface IAwslogService
    {
        Task WriteLogAsync(string logMessage, string logCategory, LogLevel logLevel);
    }

    public class AwslogService : IAwslogService
    {

        AmazonCloudWatchLogsClient cloudWatchLogsClient = new AmazonCloudWatchLogsClient("AKIAWVTSAQZ3KKXAPWMQ", "/0F0SXFXHhgeCl9FHYh3I9ToD2ycRQBPZS69W3dD", RegionEndpoint.APSoutheast1);
        public async Task WriteLogAsync(string logMessage, string logCategory, LogLevel logLevel)
        {
            var logStreamName = $"{logCategory}-{logLevel}-{DateTime.Now:yyyyMMdd}";
            var request = new PutLogEventsRequest
            {
                LogGroupName = "stylishlog",               
                LogStreamName = logStreamName,
                LogEvents = new List<InputLogEvent>
            {
                new InputLogEvent
                {
                    Timestamp = DateTime.Now,
                    Message = logMessage
                }
            }
            };
            await cloudWatchLogsClient.CreateLogGroupAsync(new CreateLogGroupRequest("stylishlog"));
            await cloudWatchLogsClient.CreateLogStreamAsync(new CreateLogStreamRequest("stylishlog", "logStreamName"));
            await cloudWatchLogsClient.PutLogEventsAsync(request);
        }

    }
}
