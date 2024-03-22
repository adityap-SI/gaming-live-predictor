using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Automate;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.Simulation
{
    public class Simulation : Common.BaseBlanket
    {
        private readonly DataAccess.Simulation.Simulation _DBContext;
        private readonly DataAccess.Feeds.Gameplay _DBFeedContext;
        private readonly Blanket.Scoring.Process _ScoringContext;
        private readonly Blanket.Automate.PointsCal _PointsCalContext;
        private Blanket.BackgroundServices.GameLocking _Locking;
        private readonly Int32 _TourId;

        public Simulation(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Simulation.Simulation(postgre);
            _DBFeedContext = new DataAccess.Feeds.Gameplay(postgre);
            _Locking = new Blanket.BackgroundServices.GameLocking(appSettings, null, aws, postgre, redis, cookies, asset);
            _ScoringContext = new Blanket.Scoring.Process(appSettings, aws, postgre, redis, cookies, asset);
            _PointsCalContext = new Blanket.Automate.PointsCal(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        #region " GET "

        public List<Fixtures> getFixtures()
        {
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            List<Fixtures> mFixtures = new List<Fixtures>();

            responseObject = _DBFeedContext.GetFixtures(1, _TourId, "en", ref httpMeta);
            mFixtures = (List<Fixtures>)responseObject.Value;

            return mFixtures;
        }

        #endregion

        #region " POST "

        public Int32 SubmitMatchForProcess(Int32 MatchId)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                retVal = _DBContext.SubmitMatchForProcess(optype, _TourId, MatchId);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 GenerarateUser(Int32 UserCount)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            //bool success = false;

            try
            {
                retVal = _DBContext.GenerateUser(optype, _TourId, UserCount);

                //retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 GenerarateUserPredictioons(Int32 MatchId, Int32 OptionId)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            //bool success = false;

            try
            {
                retVal = _DBContext.GenerateUserPredictions(optype, _TourId, MatchId, OptionId);

                //retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 UserPointProcess(Int32 GamedayId, Int32 MatchdayId)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                retVal = _DBContext.UserPointProcess(optype, _TourId, GamedayId, MatchdayId);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 MasterdataRollback()
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                retVal = _DBContext.MasterDataRollback(optype, _TourId, 0, 0);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Simulation.Simulation.MasterdataRollback", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return retVal;
        }

        public Int32 UserdataRollback()
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                retVal = _DBContext.UserDataRollback(optype, _TourId, 0, 0);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 SubmitMatchLineups(String MatchFile)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                MatchFeed mMatchFeed = _Locking.GetMatchScoresFeed(MatchFile);
                List<Lineups> mLineups = _Locking.GetLineupsFromMatchFeed(mMatchFeed);

                if (mLineups != null && mLineups.Count() > 0)
                {
                    Int32 lockVal = 0, optType = 1;
                    try
                    {
                        lockVal = _Locking.InsertMatchLineups(optType, Convert.ToInt32(mMatchFeed.Matchdetail.Match.Id), mLineups);
                    }
                    catch (Exception ex) { }

                    return lockVal;
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 SubmitMatchToss(String MatchFile)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;

            try
            {
                MatchFeed mMatchFeed = _Locking.GetMatchScoresFeed(MatchFile);
                List<Lineups> mLineups = _Locking.GetLineupsFromMatchFeed(mMatchFeed);
                if (mMatchFeed.Matchdetail.Tosswonby != null && mMatchFeed.Innings.Count() > 0)
                {
                    Innings mInning = mMatchFeed.Innings.Where(s => s.Number.ToLower() == "first").FirstOrDefault();

                    if (mInning != null && mInning.Battingteam != null)
                    {
                        Int32 inningOneBatTeamId = mInning.Battingteam.SmartIntParse();
                        Int32 inningOneBowlTeamId = inningOneBatTeamId == mMatchFeed.Matchdetail.Team_Home.SmartIntParse() ?
                            mMatchFeed.Matchdetail.Team_Away.SmartIntParse() : mMatchFeed.Matchdetail.Team_Home.SmartIntParse();
                        Int32 optType = 1;
                        try
                        {
                            retVal = _Locking.ProcessMatchToss(optType, mMatchFeed.Matchdetail.Match.Id.SmartIntParse(), inningOneBatTeamId, inningOneBowlTeamId, inningOneBowlTeamId, inningOneBatTeamId);
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public Int32 SubmitMatchAnswers(Int32 MatchId)
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;
            try
            {
                List<Fixtures> mFixtures = getFixtures();

                Fixtures mMatch = mFixtures.Where(c => c.MatchId == MatchId).FirstOrDefault();
                success = _ScoringContext.CalculateAnswers(mMatch);
                if (success)
                    _ScoringContext.SubmitMatchWinTeam(mMatch);
                if (success)
                    retVal = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retVal;
        }

        public Int32 RunPointCalculation()
        {
            Int32 retVal = -50;
            Int32 optype = 1;
            bool success = false;
            DataSet ds = new DataSet();

            try
            {
                Matchdays matchdays = new Matchdays();
                matchdays = _PointsCalContext.Matchdays();

                if (matchdays != null && matchdays.GamedayId != 0)
                {
                    retVal = _PointsCalContext.UserPointsProcess(matchdays.GamedayId, matchdays.Matchday);
                    ds = _PointsCalContext.UserPointsProcessReports(retVal, matchdays.GamedayId, matchdays.Matchday);
                }


            }
            catch (Exception ex)
            {

            }

            return retVal;
        }

        public Int32 UpdateMatchDateTime(Int32 matchId, String matchdatetime)
        {
            Int32 retVal = -50;
            Int32 optype = 1;

            try
            {
                retVal = _DBContext.UpdateMatchDateTime(optype, _TourId, matchId, matchdatetime);
            }
            catch (Exception ex)
            {

            }

            return retVal;
        }

        #endregion

    }
}
