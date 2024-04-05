using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Amazon.SimpleEmail;
using ICC.Predictor.Contracts.Configuration;

namespace ICC.Predictor.Library.AWS
{
    public class BaseAws
    {
        protected string _AWSS3Bucket;
        protected string _AWSS3FolderPath;
        protected RegionEndpoint _AWSS3Region;
        protected RegionEndpoint _AWSSESRegion;

        private bool _UseCredentials;

        public BaseAws(IOptions<Application> appSettings)
        {
            _AWSS3Bucket = appSettings.Value.Connection.AWS.S3Bucket;
            _AWSS3FolderPath = appSettings.Value.Connection.AWS.S3FolderPath;
            _AWSS3Region = RegionEndpoint.USEast1;
            _AWSSESRegion = RegionEndpoint.USEast1;
            _UseCredentials = appSettings.Value.Connection.AWS.UseCredentials;
        }

        public IAmazonS3 S3Client()
        {
            if (_UseCredentials)
                return new AmazonS3Client(Credentials._AWSCredentials, _AWSS3Region);
            else
                return new AmazonS3Client(_AWSS3Region);
        }

        public AmazonSimpleEmailServiceClient SESClient()
        {
            if (_UseCredentials)
                return new AmazonSimpleEmailServiceClient(Credentials._AWSCredentials, _AWSSESRegion);
            else
                return new AmazonSimpleEmailServiceClient(_AWSSESRegion);
        }
    }
}
