using System;

namespace ICC.Predictor.Library.AWS
{
    public class Credentials
    {
        //public static Amazon.Runtime.AWSCredentials _AWSCredentials
        //{
        //    get { return new Amazon.Runtime.BasicAWSCredentials(); }
        //}
        private static string _S3UserKey { get { return "AKIAJZCSKMHYD6AYTBMA"; } }
        private static string _S3UserSecret { get { return "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"; } }

        public static Amazon.Runtime.AWSCredentials _AWSCredentials
        {
            get { return new Amazon.Runtime.BasicAWSCredentials(_S3UserKey, _S3UserSecret); }
        }
    }
}