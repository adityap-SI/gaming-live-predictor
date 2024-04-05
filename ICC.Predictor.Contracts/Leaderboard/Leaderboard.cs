using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.Contracts.Leaderboard
{
    #region " Leaderboard "

    public class Users
    {
        public string UserId { get; set; }
        public string UserTeamId { get; set; }
        public string GUID { get; set; }
        public string TeamName { get; set; }
        public string FullName { get; set; }
        public string RankNo { set; get; }
        public string Rank { set; get; }
        public string Trend { set; get; }
        public long TotalMember { get; set; }
        public string Notation { set; get; }
        public string CurrentGamedayPoints { set; get; }
        public string Points { set; get; }


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
        public int TotalMembers { get; set; }
    }


    #endregion
}
