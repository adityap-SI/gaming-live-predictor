using System;
using System.IO;
using System.Threading.Tasks;

namespace Bodog.Predictor.Interfaces.AWS
{
    public interface IAWS
    {
        Task<String> ReadS3Asset(String fileName);

        Task<bool> ReplaceImageOnS3(Stream imageStream, String GUID, String Matchid, String gameday, String extension);

        Task<byte[]> ReadS3Image(String fileName);

        Task<bool> WriteS3Asset(String fileName, Object content, bool serialize);

        Task<bool> WriteS3Asset(String fileName, byte[] imageBytes, bool makeDownloadable = false);

        void AppendS3Logs(Contracts.Common.HTTPLog logMessage);

        Task<bool> SendSESMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null);
    }
}