using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.S3.Model;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICC.Predictor.Library.AWS
{
    public class Logs : BaseAws
    {
        protected IAmazonS3 client;

        public Logs(IOptions<Application> appSettings) : base(appSettings)
        {
        }

        public async void AppendS3Logs(HTTPLog logMessage)
        {
            try
            {
                DateTime mDate = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                string date = mDate.ToString("MM-dd-yyyy");

                string key = _AWSS3FolderPath + "/logs/" + date + "/" + "log-" + mDate.Hour + ".json";

                using (client = S3Client())
                //using (client = new AmazonS3Client(_AWSS3Region))
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key
                    };

                    var response = await client.GetObjectAsync(request);

                    using (Stream amazonStream = response.ResponseStream)
                    {
                        StreamReader amazonStreamReader = new StreamReader(amazonStream);
                        string logs = amazonStreamReader.ReadToEnd();
                        List<HTTPLog> existing = GenericFunctions.Deserialize<List<HTTPLog>>(logs);

                        WriteS3Logs(ParseLogs(logMessage, existing));
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode.Equals("NoSuchKey"))
                    WriteS3Logs(ParseLogs(logMessage, null));
            }
        }

        private async void WriteS3Logs(List<HTTPLog> logMessage)
        {
            try
            {
                DateTime mDate = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                string date = mDate.ToString("MM-dd-yyyy");

                string key = _AWSS3FolderPath + "/logs/" + date + "/" + "log-" + mDate.Hour + ".json";

                using (client = S3Client())
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key,
                        ContentType = "application/json",
                        ContentBody = GenericFunctions.Serialize(logMessage)
                    };

                    await client.PutObjectAsync(request);
                }
            }
            catch { }
        }

        private List<HTTPLog> ParseLogs(HTTPLog newLog, List<HTTPLog> existingLog)
        {
            List<HTTPLog> newRange = new List<HTTPLog>();

            if (existingLog != null)
                newRange.AddRange(existingLog);

            newRange.Add(newLog);

            return newRange;
        }
    }
}