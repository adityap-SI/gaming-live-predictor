using ICC.Predictor.Contracts.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ICC.Predictor.Interfaces.AWS
{
    public interface IAWS
    {
        Task<string> ReadS3Asset(string fileName);

        Task<bool> ReplaceImageOnS3(Stream imageStream, string GUID, string Matchid, string gameday, string extension);

        Task<byte[]> ReadS3Image(string fileName);

        Task<bool> WriteS3Asset(string fileName, object content, bool serialize);

        Task<bool> WriteS3Asset(string fileName, byte[] imageBytes, bool makeDownloadable = false);

        void AppendS3Logs(HTTPLog logMessage);

        Task<bool> SendSESMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null);
    }
}