using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Blanket.Leaderboard;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Feeds
{
    public class Ingestion : BaseBlanket
    {
        private readonly Gameplay _FeedContext;
        private readonly Leaderbaord _LeaderbaordContext;
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly DataAccess.Notification.Subscription _NotificationContext;

        private readonly int _TourId;
        private readonly List<string> _Lang;

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

        public async Task<int> Languages()
        {
            int retVal = -50;

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

        public async Task<int> Fixtures()
        {
            int retVal = -50;
            bool success = false;

            try
            {
                foreach (string lang in await GetLanguages())
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

        public async Task<int> Skills()
        {
            int retVal = -50;
            bool success = false;

            try
            {
                foreach (string lang in await GetLanguages())
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

        public async Task<int> Questions(int QuestionsMatchID)
        {
            int retVal = -50;
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

        public async Task<int> AllMatchQuestions()
        {
            int retVal = -50;
            bool success = false;
            string lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            HTTPResponse hTTPResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            try
            {
                hTTPResponse = await _FeedContext.GetFixtures(lang);
                responseObject = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(hTTPResponse.Data));
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));

                foreach (Fixtures mFixture in fixtures)
                {
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

        public async Task<int> GetRecentResults()
        {
            int retVal = -50;
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

        public async Task<int> MatchInningStatus(int MatchID)
        {
            int retVal = -50;
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

        public async Task<int> LeaderBoard(int GamedayId, int PhaseId)
        {
            int retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();

            try
            {
                int[] mOptTypes = new int[3] { 1, 2, 3 };//1: Overall; 2: Gameday; 3: Weekly
                int pageNo = 1, top = 1000, fromRowNo = 1, toRowNo = 1000;


                foreach (int mOptType in mOptTypes)
                {
                    ResponseObject response = _DBContext.Top(mOptType, PhaseId, GamedayId, pageNo, top, _TourId, fromRowNo, toRowNo, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                    {
                        if (mOptType == 1)
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

        public async Task<int> CurrentGamedayMatches()
        {
            int retVal = -50;
            bool success = false;

            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;
            string lang = "en";
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<CurrentGamedayMatches> mCurrentGamedayMatches = new List<CurrentGamedayMatches>();
            try
            {

                string data = await _Asset.GET(_Asset.Fixtures(lang));

                mFixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));

                mFixtures = mFixtures.Where(i => i.MatchStatus != 3).OrderBy(x => GenericFunctions.ToUSCulture(x.Deadlinedate)).Select(o => o).ToList();
                if (mFixtures.Any())
                {
                    int CurrentGamedayId = mFixtures[0].TourGamedayId;
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
                            Live = fixture.MatchStatus == 2 && fixture.Match_Inning_Status != 6 ? 1 : 0,
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
        public async Task<int> InsertTopics()
        {
            int retVal = -50;
            int optType = 1;
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

        public async Task<int> IngestNotificationMessages(string notificationMessages)
        {
            int retVal = -50;
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

        public async Task<int> IngestNotificationStatus()
        {
            int retVal = -50;
            bool success = false;
            string lang = "en";
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