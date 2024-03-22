using Bodog.Predictor.Contracts.Common;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Bodog.Predictor.Library.Utility
{
    public class GenericFunctions
    {
        public static String Serialize(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = Int32.MaxValue });
        }

        public static T Deserialize<T>(String data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = Int32.MaxValue });
        }

        public static String GetWebData(String url)
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

        public static String PostWebData(String url, String param)
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

        public static DateTime ToUSCulture(String dateTime)
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

        public static void AssetMeta(Int64 retVal, ref HTTPMeta httpMeta, String message = "")
        {
            httpMeta.Success = (retVal == 1);
            httpMeta.RetVal = retVal;
            httpMeta.Message = !String.IsNullOrEmpty(message) ? message : (retVal == 1 ? "Success" : "Failed");
            httpMeta.Timestamp = GetFeedTime();
        }

        public static String DecryptedValue(String encryptedValue)
        {
            string val = "";

            try
            {
                if (!String.IsNullOrEmpty(encryptedValue))
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

        public static String DebugTable(System.Data.DataTable table)
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
                sb.Append(String.Format("{0,-20} | ", s));
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
                    sb.Append(String.Format("{0,-20} | ", s));
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

        public static String EmailBody(String service, String contents)
        {
            String body = String.Empty;

            body = "This is a system generated mail from " + service + " daemon service running on " + Environment.MachineName.ToUpper() + ".<br/><br/>";
            body += "The service invoked the " + service + " process.<br/><br/>";
            body += "" + contents + "<br/><br/>";
            body += "Thanks.";

            return body;
        }

        public static Int32[] GetPagePoints(Int32 pageOneChunk, Int32 pageChunk, Int32 pageNo)
        {
            Int32[] address = new Int32[2];

            Int32 mPageOneSize = pageOneChunk;
            Int32 mCurrPageSize = pageChunk;
            Int32 mPageNo = pageNo;

            Int32 mFrom = 0;
            Int32 mTo = 0;

            mTo = mPageOneSize + ((mPageNo - 1) * mCurrPageSize);
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