using System;

namespace ICC.Predictor.Contracts.Session
{
    public class UserCookie
    {
        public string UserId { get; set; }
        public string SocialId { get; set; }
        //public String SessionId { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
    }

    public class GameCookie
    {
        public GameCookie()
        {
            FullName = ""; //this.ScreenName = "";
            //this.ClientId = "";
            GUID = "";
            TeamId = ""; TeamName = ""; CountryId = ""; CountryName = "";
            FavTeamId = ""; FavTeamName = ""; CurrGamedayId = ""; IsTourActive = "";
            IsRegistered = "";
        }

        public string FullName { get; set; }
        //public String ScreenName { get; set; }
        //public String ClientId { get; set; }
        public string GUID { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string FavTeamId { get; set; }
        public string FavTeamName { get; set; }
        public string CurrGamedayId { get; set; }
        public string IsTourActive { get; set; }
        public string IsRegistered { get; set; }
    }

    public class UserDetails
    {
        public UserCookie User { get; set; }
        public GameCookie Game { get; set; }
    }
}