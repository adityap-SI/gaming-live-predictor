using System;

namespace Bodog.Predictor.Contracts.Session
{
    public class UserCookie
    {
        public String UserId { get; set; }
        public String SocialId { get; set; }
        //public String SessionId { get; set; }
        public String EmailId { get; set; }
        public String PhoneNo { get; set; }
    }

    public class GameCookie
    {
        public GameCookie()
        {
            this.FullName = ""; //this.ScreenName = "";
            //this.ClientId = "";
            this.GUID = "";
            this.TeamId = ""; this.TeamName = ""; this.CountryId = ""; this.CountryName = "";
            this.FavTeamId = ""; this.FavTeamName = ""; this.CurrGamedayId = ""; this.IsTourActive = "";
            this.IsRegistered = "";
        }

        public String FullName { get; set; }
        //public String ScreenName { get; set; }
        //public String ClientId { get; set; }
        public String GUID { get; set; }
        public String TeamId { get; set; }
        public String TeamName { get; set; }
        public String CountryId { get; set; }
        public String CountryName { get; set; }
        public String FavTeamId { get; set; }
        public String FavTeamName { get; set; }
        public String CurrGamedayId { get; set; }
        public String IsTourActive { get; set; }
        public String IsRegistered { get; set; }
    }

    public class UserDetails
    {
        public UserCookie User { get; set; }
        public GameCookie Game { get; set; }
    }
}