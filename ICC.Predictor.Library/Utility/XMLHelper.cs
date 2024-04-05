using ICC.Predictor.Contracts.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace ICC.Predictor.Library.Utility
{
    public class XMLHelper
    {
        public static XmlReader ReadXMLApi(string api_url)
        {
            try
            {
                JsonResult v = WebRequest(Method.GET, api_url, "", "");
                StringReader strReader = new StringReader(v.responseString);
                XmlTextReader xmlReader = new XmlTextReader(strReader);
                return xmlReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static XmlReader ReadXMLApi(string api_url, ref JsonResult objJsonResult)
        {
            try
            {
                //string str404ErrMsg = "The remote server returned an error: (404) Not Found";
                XmlTextReader xmlReader = null;

                objJsonResult = WebRequest(Method.GET, api_url, "", "");

                if (objJsonResult.error)
                {
                    objJsonResult.error = true;
                    objJsonResult.error_message = "Data Not Found for URI api " + api_url;
                    return xmlReader;
                }

                StringReader strReader = new StringReader(objJsonResult.responseString);
                xmlReader = new XmlTextReader(strReader);
                return xmlReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime ParseExactDate(string date, string original_format, string required_format)
        {
            return Convert.ToDateTime(DateTime.ParseExact(date, original_format, System.Globalization.CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy"));
        }

        public static JsonResult WebRequest(Method method, string url, string postData, string OAuthHeader)
        {
            try
            {
                string responseData = string.Empty;
                HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                webRequest.Method = method.ToString();
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;
                if (method == Method.POST)
                {
                    webRequest.UseDefaultCredentials = true;
                    webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.Headers["Authorization"] = OAuthHeader;
                    Stream requestWriter = webRequest.GetRequestStream();
                    try
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(postData);

                        requestWriter.Write(bytes, 0, bytes.Length);
                        requestWriter.Close();
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult() { error = true, error_message = ex.Message };
                    }
                    finally
                    {
                        requestWriter.Close();
                        requestWriter = null;
                    }
                }
                responseData = WebResponse(webRequest);

                webRequest = null;
                return new JsonResult() { error = false, responseString = responseData };
            }
            catch (Exception ex)
            {
                return new JsonResult() { error = true, error_message = ex.Message };
            }
        }

        public static string WebResponse(HttpWebRequest webRequest)
        {
            Stream responseStream = null;
            StreamReader responseReader = null;
            string responseData = "";
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                responseStream = webResponse.GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception exc)
            {
                throw new Exception("could not post : " + exc.Message);
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseReader.Close();
                }
            }
            return responseData;
        }
    }

    public enum Method
    {
        GET, POST
    }
}
