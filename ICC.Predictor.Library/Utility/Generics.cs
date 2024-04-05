using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using ICC.Predictor.Contracts.Common;

namespace ICC.Predictor.Library.Utility
{
    public class GenericFunctions
    {
        public static string Serialize(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = int.MaxValue });
        }

        public static T Deserialize<T>(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = int.MaxValue });
        }

        public static string GetWebData(string url)
        {
            string strRetVal = string.Empty;

            try
            {
                System.Net.HttpWebRequest mHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                mHttpWebRequest.Method = "GET";
                mHttpWebRequest.Timeout = 30000; // 30 Seconds
                mHttpWebRequest.KeepAlive = false;
                mHttpWebRequest.ProtocolVersion = System.Net.HttpVersion.Version10;
                mHttpWebRequest.Accept = "application/json";
                mHttpWebRequest.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

                using (System.Net.WebResponse response = mHttpWebRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        strRetVal = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex) { }

            return strRetVal;
        }

        public static string PostWebData(string url, string param)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            byte[] bytes = Encoding.UTF8.GetBytes(param);
            req.ContentLength = bytes.Length;
            Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);

            os.Close();

            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return "";
            StreamReader sr = new StreamReader(resp.GetResponseStream());

            string responsecontent = sr.ReadToEnd().Trim();
            return responsecontent;
        }

        public static DateTime ToUSCulture(string dateTime)
        {
            return Convert.ToDateTime(dateTime, new System.Globalization.CultureInfo("en-US"));
        }

        public static FeedTime GetFeedTime()
        {
            return new FeedTime()
            {
                UTCtime = TimeZone.CurrentUTCtime(),
                //CESTtime = TimeZone.CurrentCESTtime(),
                ISTtime = TimeZone.CurrentISTtime()
            };
        }

        public static void AssetMeta(long retVal, ref HTTPMeta httpMeta, string message = "")
        {
            httpMeta.Success = retVal == 1;
            httpMeta.RetVal = retVal;
            httpMeta.Message = !string.IsNullOrEmpty(message) ? message : retVal == 1 ? "Success" : "Failed";
            httpMeta.Timestamp = GetFeedTime();
        }

        public static string DecryptedValue(string encryptedValue)
        {
            string val = "";

            try
            {
                if (!string.IsNullOrEmpty(encryptedValue))
                    val = Encryption.BaseDecrypt(encryptedValue);
            }
            catch
            {
                try
                {
                    val = BareEncryption.BaseDecrypt(encryptedValue);
                }
                catch { }
            }

            return val;
        }

        public static string DebugTable(System.Data.DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--- cursor {" + table.TableName + "} ---");
            sb.Append(Environment.NewLine);
            int zeilen = table.Rows.Count;
            int spalten = table.Columns.Count;

            // Header
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string s = table.Columns[i].ToString();
                sb.Append(string.Format("{0,-20} | ", s));
            }
            sb.Append(Environment.NewLine);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append("---------------------|-");
            }
            sb.Append(Environment.NewLine);

            // Data
            for (int i = 0; i < zeilen; i++)
            {
                System.Data.DataRow row = table.Rows[i];

                for (int j = 0; j < spalten; j++)
                {
                    string s = row[j].ToString();
                    if (s.Length > 20) s = s.Substring(0, 17) + "...";
                    sb.Append(string.Format("{0,-20} | ", s));
                }
                sb.Append(Environment.NewLine);
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append("---------------------|-");
            }
            sb.Append(Environment.NewLine);

            Debug.WriteLine(sb);//writes to Output window
            return sb.ToString();
        }

        public static string EmailBody(string service, string contents)
        {
            string body = string.Empty;

            body = "This is a system generated mail from " + service + " daemon service running on " + Environment.MachineName.ToUpper() + ".<br/><br/>";
            body += "The service invoked the " + service + " process.<br/><br/>";
            body += "" + contents + "<br/><br/>";
            body += "Thanks.";

            return body;
        }

        public static int[] GetPagePoints(int pageOneChunk, int pageChunk, int pageNo)
        {
            int[] address = new int[2];

            int mPageOneSize = pageOneChunk;
            int mCurrPageSize = pageChunk;
            int mPageNo = pageNo;

            int mFrom = 0;
            int mTo = 0;

            mTo = mPageOneSize + (mPageNo - 1) * mCurrPageSize;
            if (mPageNo == 1)
                mFrom = mTo - mPageOneSize;
            else
                mFrom = mTo - mCurrPageSize;

            mFrom = mFrom + 1;

            address[0] = mFrom;
            address[1] = mTo;

            return address;
        }

    }
}