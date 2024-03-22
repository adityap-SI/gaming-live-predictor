using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.S3.Model;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bodog.Predictor.Library.AWS
{
    public class Logs : BaseAws
    {
        protected IAmazonS3 client;

        public Logs(IOptions<Contracts.Configuration.Application> appSettings) : base(appSettings)
        {
        }

        public async void AppendS3Logs(Contracts.Common.HTTPLog logMessage)
        {
            try
            {
                DateTime mDate = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                String date = mDate.ToString("MM-dd-yyyy");

                String key = _AWSS3FolderPath + "/logs/" + date + "/" + "log-" + mDate.Hour + ".json";

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
                        List<Contracts.Common.HTTPLog> existing = GenericFunctions.Deserialize<List<Contracts.Common.HTTPLog>>(logs);

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

        private async void WriteS3Logs(List<Contracts.Common.HTTPLog> logMessage)
        {
            try
            {
                DateTime mDate = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                String date = mDate.ToString("MM-dd-yyyy");

                String key = _AWSS3FolderPath + "/logs/" + date + "/" + "log-" + mDate.Hour + ".json";

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

        private List<Contracts.Common.HTTPLog> ParseLogs(Contracts.Common.HTTPLog newLog, List<Contracts.Common.HTTPLog> existingLog)
        {
            List<Contracts.Common.HTTPLog> newRange = new List<Contracts.Common.HTTPLog>();

            if (existingLog != null)
                newRange.AddRange(existingLog);

            newRange.Add(newLog);

            return newRange;
        }
    }
}