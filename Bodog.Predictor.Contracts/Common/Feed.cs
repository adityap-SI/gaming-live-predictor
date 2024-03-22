using System;

namespace Bodog.Predictor.Contracts.Common
{
    public class ResponseObject
    {
        public ResponseObject()
        {
            this.Value = null;
            this.FeedTime = null;
        }

        public Object Value { get; set; }
        public FeedTime FeedTime { get; set; }
    }

    public class HTTPResponse
    {
        public HTTPResponse()
        {
            this.Meta = new HTTPMeta();
        }

        public Object Data { get; set; }
        public HTTPMeta Meta { get; set; }
    }

    public class HTTPMeta
    {
        public String Message { get; set; }
        public Int64 RetVal { get; set; }
        public bool Success { get; set; }
        public FeedTime Timestamp { get; set; }
    }

    public class HTTPLog
    {
        public FeedTime Timestamp { get; set; }
        public String Function { get; set; }
        public String Message { get; set; }
        public String RequestType { get; set; }
        public String RequestUri { get; set; }
        public String UserAgent { get; set; }
        public String Payload { get; set; }
        public Object Cookies { get; set; }
    }

    public class FeedTime
    {
        public String UTCtime { get; set; }
        public String ISTtime { get; set; }
    }

    public class Notification
    {
        public String Option { get; set; }
        public String Caption { get; set; }
        public String Service { get; set; }
    }
}