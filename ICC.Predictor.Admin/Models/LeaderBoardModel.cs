using ICC.Predictor.Blanket.Leaderboard;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.Models
{
    public class LeaderBoardModel : Reports
    {
        public int? TopUser { get; set; }
        public int? LeaderBoardTypeId { get; set; }
        public List<LeaderBoardType> LeaderBoardTypes { get; set; }
        public int? GamedayId { get; set; }
        public List<int> GamedayList { get; set; }
        public int? PhaseId { get; set; }
        public List<int> PhaseList { get; set; }
    }

    public class LeaderBoardType
    {
        public int LeaderBoardId { get; set; }
        public string LeaderBoardName { get; set; }
    }

    #region " LeaderBoard Worker "
    public class LeaderBoardWorker
    {
        public LeaderBoardModel GetModel(Leaderbaord leaderbaordContext, bool fetchData = false)
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
