using System;

namespace ICC.Predictor.Contracts.Common
{
    public class ResponseObject
    {
        public ResponseObject()
        {
            Value = null;
            FeedTime = null;
        }

        public object Value { get; set; }
        public FeedTime FeedTime { get; set; }
    }

    public class HTTPResponse
    {
        public HTTPResponse()
        {
            Meta = new HTTPMeta();
        }

        public object Data { get; set; }
        public HTTPMeta Meta { get; set; }
    }

    public class HTTPMeta
    {
        public string Message { get; set; }
        public long RetVal { get; set; }
        public bool Success { get; set; }
        public FeedTime Timestamp { get; set; }
    }

    public class HTTPLog
    {
        public FeedTime Timestamp { get; set; }
        public string Function { get; set; }
        public string Message { get; set; }
        public string RequestType { get; set; }
        public string RequestUri { get; set; }
        public string UserAgent { get; set; }
        public string Payload { get; set; }
        public object Cookies { get; set; }
    }

    public class FeedTime
    {
        public string UTCtime { get; set; }
        public string ISTtime { get; set; }
    }

    public class Notification
    {
        public string Option { get; set; }
        public string Caption { get; set; }
        public string Service { get; set; }
    }
}