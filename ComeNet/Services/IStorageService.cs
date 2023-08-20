using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using static AWSWEBAPP.Models.Aws;
using AWSCredentials = AWSWEBAPP.Models.Aws.AWSCredentials;

namespace AWSWEBAPP.Services
{
    public interface IStorageService
    {

        Task<S3ResponseDTO> UploadFileAsync(AWSS3Object s3obj, AWSCredentials aWSCredentials);

    }

    public class StorageService : IStorageService
    {
        public async Task<S3ResponseDTO> UploadFileAsync(AWSS3Object s3obj, AWSCredentials aWSCredentials)
        {            
            var credentials = new BasicAWSCredentials(aWSCredentials.AwsKey, aWSCredentials.AwsSecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1
            };
            var response = new S3ResponseDTO();

            try
            {

                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = s3obj.InputStream,
                    Key = s3obj.Name,
                    BucketName = s3obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using var client = new AmazonS3Client(credentials, config);

                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
                response.StatusCode = 200;
                response.message = s3obj.Name;


            }
            catch (AmazonS3Exception ex)
            {
                response.StatusCode = (int)ex.StatusCode;
                response.message = ex.Message;

            }
            catch (Exception ex)
            {

                response.StatusCode = 500;
                response.message =ex.Message;

            }

            return response;
        }
        
    }
}
