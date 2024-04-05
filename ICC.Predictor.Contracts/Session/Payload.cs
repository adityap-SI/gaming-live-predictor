using System;

namespace ICC.Predictor.Contracts.Session
{
    public class Credentials
    {
        public int OptType { get; set; }
        public int PlatformId { get; set; }
        public int UserId { get; set; }
        public string SocialId { get; set; }
        public int ClientId { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public string CountryCode { get; set; }
        public long PhoneNo { get; set; }
        public string ProfilePicture { get; set; }
        //public String CountryOfResidence { get; set; }
    }
}