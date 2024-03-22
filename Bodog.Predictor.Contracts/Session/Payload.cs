using System;

namespace Bodog.Predictor.Contracts.Session
{
    public class Credentials
    {
        public Int32 OptType { get; set; }
        public Int32 PlatformId { get; set; }
        public Int32 UserId { get; set; }
        public String SocialId { get; set; }
        public Int32 ClientId { get; set; }
        public String FullName { get; set; }
        public String EmailId { get; set; }
        public String CountryCode { get; set; }
        public Int64 PhoneNo { get; set; }
        public String ProfilePicture { get; set; }
        //public String CountryOfResidence { get; set; }
    }
}