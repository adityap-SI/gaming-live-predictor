using Bodog.Predictor.Blanket.Leaderboard;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Contracts.Notification;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.Feeds
{
    public class Ingestion : Common.BaseBlanket
    {
        private readonly Gameplay _FeedContext;
        private readonly Leaderbaord _LeaderbaordContext;
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly DataAccess.Notification.Subscription _NotificationContext;

        private readonly Int32 _TourId;
        private readonly List<String> _Lang;

        public Ingestion(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _FeedContext = new Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _LeaderbaordContext = new Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _DBContext = new DataAccess.Leaderboard.Leaderbaord(postgre);
            _NotificationContext = new DataAccess.Notification.Subscription(postgre);
            _TourId = appSettings.Value.Properties.TourId;
            _Lang = appSettings.Value.Properties.Languages;
        }

        public async Task<Int32> Languages()
        {
            Int32 retVal = -50;

            try
            {
                ResponseObject response = new ResponseObject();
                response.Value = _Lang;
                response.FeedTime = GenericFunctions.GetFeedTime();

                bool success = await _Asset.SET(_Asset.Languages(), response);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Fixtures()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                foreach (String lang in await GetLanguages())
                {
                    HTTPResponse response = await _FeedContext.GetFixtures(lang, offloadDb: false);

                    if (response.Meta.RetVal == 1)
                        success = await _Asset.SET(_Asset.Fixtures(lang), response.Data);
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Skills()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                foreach (String lang in await GetLanguages())
                {
                    HTTPResponse response = await _FeedContext.GetSkills(lang, offloadDb: false);

                    if (response.Meta.RetVal == 1)
                        success = await _Asset.SET(_Asset.Skills(lang), response.Data);
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Questions(Int32 QuestionsMatchID)
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.GetQuestions(QuestionsMatchID, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchQuestions(QuestionsMatchID), response.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> AllMatchQuestions()
        {
            Int32 retVal = -50;
            bool success = false;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            HTTPResponse hTTPResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            try
            {
                hTTPResponse = await _FeedContext.GetFixtures(lang);
                responseObject = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(hTTPResponse.Data));
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));

                foreach (Fixtures mFixture in fixtures) { 
                HTTPResponse response = await _FeedContext.GetQuestions(mFixture.MatchId, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchQuestions(mFixture.MatchId), response.Data);

                retVal = Convert.ToInt32(success);
                }
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> GetRecentResults()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.GetRecentResults(offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.RecentResult(), response.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> MatchInningStatus(Int32 MatchID)
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.MatchInningStatus(MatchID, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchInningStatus(MatchID), response.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> LeaderBoard(Int32 GamedayId, Int32 PhaseId)
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();

            try
            {
                Int32[] mOptTypes = new Int32[3] { 1, 2, 3 };//1: Overall; 2: Gameday; 3: Weekly
                Int32 pageNo = 1, top = 1000, fromRowNo = 1, toRowNo = 1000;


                foreach(Int32 mOptType in mOptTypes)
                {
                     ResponseObject response = _DBContext.Top(mOptType, PhaseId, GamedayId, pageNo, top, _TourId, fromRowNo, toRowNo, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                    {
                        if(mOptType == 1)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, 0, 0), response);
                        else if (mOptType == 2)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, GamedayId, 0), response);
                        else if (mOptType == 3)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, 0, PhaseId), response);
                    }
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal;

        }

        public async Task<Int32> CurrentGamedayMatches()
        {
            Int32 retVal = -50;
            bool success = false;

            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 optType = 1;
            String lang = "en";
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<CurrentGamedayMatches> mCurrentGamedayMatches = new List<CurrentGamedayMatches>();
            try
            {

                String data = await _Asset.GET(_Asset.Fixtures(lang));

                mFixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));

                mFixtures = mFixtures.Where(i => i.MatchStatus != 3).OrderBy(x => GenericFunctions.ToUSCulture(x.Deadlinedate)).Select(o => o).ToList();
                if (mFixtures.Any())
                {
                    Int32 CurrentGamedayId = mFixtures[0].TourGamedayId;
                    mFixtures = mFixtures.Where(c => c.TourGamedayId == CurrentGamedayId).ToList();
                    foreach (Fixtures fixture in mFixtures)
                    {
                        mCurrentGamedayMatches.Add(new CurrentGamedayMatches
                        {
                            MatchId = fixture.MatchId,
                            TeamGamedayId = fixture.TeamGamedayId,
                            TourGamedayId = fixture.TourGamedayId,
                            Date = fixture.Date,
                            MatchStatus = fixture.MatchStatus,
                            Live = (fixture.MatchStatus == 2 && fixture.Match_Inning_Status != 6) ? 1 : 0,
                            Match_Inning_Status = fixture.Match_Inning_Status
                        });
                    }
                }

                responseObject.Value = mCurrentGamedayMatches;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.CurrentGamedayMatches(), responseObject);
                retVal = Convert.ToInt32(success);

               
                
            }
            catch (Exception ex)
            { }

            return retVal;
        }

        #region " Notification "
        public async Task<Int32> InsertTopics()
        {
            Int32 retVal = -50;
            Int32 optType = 1;
            HTTPMeta hTTPMeta = new HTTPMeta();
            bool success = false;

            try
            {
               
                    ResponseObject response = _NotificationContext.TopicsGet(optType, _TourId, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                        success = await _Asset.SET(_Asset.NotificationTopics(), response);
              

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> IngestNotificationMessages(String notificationMessages)
        {
            Int32 retVal = -50;
            bool success = false;
            try
            {
                NotificationText mNotificationText = new NotificationText();
                mNotificationText = GenericFunctions.Deserialize<NotificationText>(notificationMessages);
                success = await _Asset.SET(_Asset.NotificationText(), mNotificationText);
                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }
            return retVal;
        }

        public async Task<Int32> IngestNotificationStatus()
        {
            Int32 retVal = -50;
            bool success = false;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            HTTPResponse hTTPResponse = new HTTPResponse();
            List<NotificationStatus> notificationStatuses = new List<NotificationStatus>();
            ResponseObject responseObject = new ResponseObject();
            try
            {
                hTTPResponse = await _FeedContext.GetFixtures(lang);
                responseObject = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(hTTPResponse.Data));
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));

                foreach (Fixtures mFixture in fixtures)
                {
                    notificationStatuses.Add(new NotificationStatus
                    {
                        MatchId = mFixture.MatchId,
                        PreMatchNotification = false
                    });
                    success = await _Asset.SET(_Asset.NotificationStatus(), notificationStatuses);

                    retVal = Convert.ToInt32(success);
                }
            }
            catch (Exception ex)
            {
               await _Asset.SET(_Asset.Debug("IngestNotificationStatus"), ex.Message);
            }
            return retVal;
        }
        #endregion

    }
}