using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Admin.Models
{
    public class LeaderBoardModel: Reports
    {
        public Int32? TopUser { get; set; }
        public Int32? LeaderBoardTypeId { get; set; }
        public List<LeaderBoardType> LeaderBoardTypes { get; set; }
        public Int32? GamedayId { get; set; }
        public List<Int32> GamedayList { get; set; }
        public Int32? PhaseId { get; set; }
        public List<Int32> PhaseList { get; set; }
    }

    public class LeaderBoardType
    {
        public Int32 LeaderBoardId { get; set; }
        public String LeaderBoardName { get; set; }
    }

    #region " LeaderBoard Worker "
    public class LeaderBoardWorker
    {
        public LeaderBoardModel GetModel(Blanket.Leaderboard.Leaderbaord leaderbaordContext, bool fetchData = false)
        {

            LeaderBoardModel model = new LeaderBoardModel();
            List<Fixtures> mFixtures = new List<Fixtures>();

            #region " LeaderBoardType Dropdown "
            model.LeaderBoardTypes = new List<LeaderBoardType>();
            model.LeaderBoardTypes.Add(new LeaderBoardType { LeaderBoardId = 1, LeaderBoardName = "Overall" });
            model.LeaderBoardTypes.Add(new LeaderBoardType { LeaderBoardId = 2, LeaderBoardName = "Gameday" });
            model.LeaderBoardTypes.Add(new LeaderBoardType { LeaderBoardId = 3, LeaderBoardName = "Weekly" });
            #endregion " LeaderBoardType Dropdown "


            #region " Match Dropdown "

            mFixtures = leaderbaordContext.getFixtures();

            model.GamedayList = mFixtures.Where(y => y.MatchStatus == 3).Select(x => x.GamedayId).Distinct().ToList();
            model.PhaseList = mFixtures.Where(y => y.MatchStatus == 3).Select(x => x.phaseId).Distinct().ToList();

            model.LeaderBoardList = new List<AdminLeaderBoard>();
            #endregion

            return model;

        }
    }
    #endregion " LeaderBoard Worker "

}
