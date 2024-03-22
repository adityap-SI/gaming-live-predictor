using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.S3.Model;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bodog.Predictor.Library.AWS
{
    public class S3 : Logs
    {

        public S3(IOptions<Application> appSettings) : base(appSettings)
        {
        }

        public async Task<String> ReadS3Asset(String fileName)
        {
            String content = "";
            String key = _AWSS3FolderPath + fileName;

            try
            {
                using (client = S3Client())
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key
                    };

                    //var response = await client.GetObjectAsync(request).ConfigureAwait(false);
                    var response = await client.GetObjectAsync(request);

                    using (Stream amazonStream = response.ResponseStream)
                    {
                        using (StreamReader sr = new StreamReader(amazonStream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return content;
        }

        public async Task<bool> ReplaceImageOnS3(Stream imageStream, String GUID, String Matchid, String gameday, String extension)
        {
            bool success = false;
            String fileName = GUID + "_" + Matchid + "_" + gameday;
            String date = DateTime.UtcNow.Date.ToString("MM-dd-yyyy");

            String key = _AWSS3FolderPath + "/static/image-share/" + date + "/" + fileName + "." + extension.ToString();

            try
            {
                //using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(AccessKey, Token, Amazon.RegionEndpoint.USEast1))
                using (client = S3Client())
                {
                    try
                    {
                        GetObjectRequest request = new GetObjectRequest()
                        {
                            BucketName = _AWSS3Bucket,
                            Key = key
                        };

                        var response = await client.GetObjectAsync(request);

                        if (response != null)
                        {
                            var deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = _AWSS3Bucket,
                                Key = key
                            };

                            DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(deleteObjectRequest);

                            if (deleteResponse != null)
                                success = await WriteImageOnS3(imageStream, key, extension);
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                            success = await WriteImageOnS3(imageStream, key, extension);
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode.Equals("NoSuchKey"))
                    success = await WriteImageOnS3(imageStream, key, extension);
            }

            imageStream.Dispose();
            return success;
        }

        private async Task<bool> WriteImageOnS3(Stream imageStream, String bucketKey, String extension)
        {
            bool success = false;
            String key = bucketKey;
            try
            {
                //using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(AccessKey, Token, Amazon.RegionEndpoint.USEast1))
                using (client = S3Client())
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key,
                        InputStream = imageStream,
                        ContentType = "image/" + extension.ToString(),
                        CannedACL = S3CannedACL.PublicRead
                    };

                    PutObjectResponse response = await client.PutObjectAsync(request);

                    success = response != null;
                }
            }
            catch { }

            return success;
        }

        public async Task<byte[]> ReadS3Image(String fileName)
        {
            byte[] content = null;
            String key = _AWSS3FolderPath + fileName;

            try
            {
                using (client = S3Client())
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key
                    };

                    var response = await client.GetObjectAsync(request);

                    using (Stream amazonStream = response.ResponseStream)
                    {
                        var buffer = new byte[16 * 1024];
                        int bytesRead = -1;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            while ((bytesRead = amazonStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                                content = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return content;
        }

        public async Task<bool> WriteS3Asset(String fileName, Object content, bool serialize)
        {
            bool success = false;
            String key = _AWSS3FolderPath + fileName;
            String extension = Path.GetExtension(fileName).Replace(".", "").ToLower().Trim();

            try
            {
                //using (client = _AWSOptions.CreateServiceClient<IAmazonS3>())
                using (client = S3Client())
                {
                    PutObjectRequest request = new PutObjectRequest();
                    request.BucketName = _AWSS3Bucket;
                    request.Key = key;
                    request.CannedACL = S3CannedACL.PublicRead;
                    request.ContentType = "application/" + extension;
                    request.ContentBody = serialize ? GenericFunctions.Serialize(content) : content.ToString();

                    if (extension == "txt")
                        request.ContentType = "text/plain";
                    if (extension == "html")
                        request.ContentType = "text/html";

                    var response = await client.PutObjectAsync(request);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        success = true;
                }
            }
            catch (Exception ex) { }

            return success;
        }

        public async Task<bool> WriteS3Asset(String fileName, byte[] imageBytes, bool makeDownloadable = false)
        {
            bool success = false;
            String key = _AWSS3FolderPath + fileName;
            String extension = Path.GetExtension(fileName).Replace(".", "").ToLower().Trim();

            try
            {
                using (client = S3Client())
                {
                    PutObjectRequest request = new PutObjectRequest();
                    request.BucketName = _AWSS3Bucket;
                    request.Key = key;
                    request.CannedACL = S3CannedACL.PublicRead;
                    request.ContentType = "image/" + extension;
                    request.InputStream = new MemoryStream(imageBytes);

                    if (makeDownloadable)
                        //to make the image auto-download on open
                        request.ContentType = "application/octet-stream";

                    var response = await client.PutObjectAsync(request);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        success = true;
                }
            }
            catch (Exception ex) { }

            return success;
        }
    }
}