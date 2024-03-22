using System;
using System.Collections.Generic;
using System.Text;

namespace Bodog.Predictor.Contracts.Leaderboard
{
    #region " Leaderboard "

    public class Users
    {
        public String UserId { get; set; }
        public String UserTeamId { get; set; }
        public String GUID { get; set; }
        public String TeamName { get; set; }
        public String FullName { get; set; }
        public String RankNo { set; get; }
        public String Rank { set; get; }
        public String Trend { set; get; }
        public Int64 TotalMember { get; set; }
        public string Notation { set; get; }
        public String CurrentGamedayPoints { set; get; }
        public String Points { set; get; }


        //public Int64 GamedayNo { get; set; }
        //public Int64 GamedayId { get; set; }


        //public String SocialId { get; set; }
        //public Int64 ClientId { get; set; }

        //public String PhasePoints { set; get; }
        //public String OverallPoints { set; get; }

        //public bool IsCeleb { get; set; }

        //public FeedTime FeedTime { get; set; }
    }
    public class Top
    {
        public List<Users> Users { get; set; }
        public Int32 TotalMembers { get; set; }
    }


    #endregion
}
