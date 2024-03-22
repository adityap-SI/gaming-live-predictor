using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;

namespace Bodog.Predictor.Library.Asset
{
    public class Constants : Read, Interfaces.Asset.IAsset
    {
        private readonly String _RedisBaseKey;

        public Constants(IAWS aws, IRedis redis, IOptions<Application> appSettings) : base(aws, redis, appSettings)
        {
            _RedisBaseKey = appSettings.Value.Properties.ClientName + $"-fantasy-{_Connection}-{_TourId}";
        }

        public String Languages()
        {
            String key = $"/assets/languages/languages_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-languages";

            return key;
        }

        public String Fixtures(String lang)
        {
            String key = $"/assets/fixtures/fixtures_{_TourId}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-fixtures-{lang}";

            return key;
        }

        public String Skills(String lang)
        {
            String key = $"/assets/skill/skill_{_TourId}_{lang}.html";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-skill-{lang}";

            return key;
        }

        public String MatchQuestions(Int32? QuestionsMatchID)
        {
            String key = $"/assets/matchquestions/questions_{_TourId}_{QuestionsMatchID}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-questions_-{QuestionsMatchID}";

            return key;
        }

        public String RecentResult()
        {
            String key = $"/assets/recentmatchresults/recentresults_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-recentresults_-{_TourId}";

            return key;
        }

        public String MatchInningStatus(Int32 MatchId)
        {
            String key = $"/assets/matchinningstatus/matchstatus_{_TourId}_{MatchId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-matchinningstatus_-{MatchId}";

            return key;
        }

        public String LeaderBoard(Int32 vOptType, Int32 gamedayId, Int32 phaseId)
        {
            String key = $"/assets/leaderboard/leaderboard_{_TourId}_{vOptType}_{gamedayId}_{phaseId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-leaderboard_-{vOptType}_{gamedayId}_{phaseId}";

            return key;
        }

        public String Debug(String FileName)
        {
            String key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public String ShareImage(String FileName)
        {
            String key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public String CurrentGamedayMatches()
        {
            String key = $"/assets/currentgameday/currentgamedaymatches_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-currentgamedaymatches-{_TourId}";

            return key;
        }

        public String UserDetailsReport()
        {
            String key = $"/assets/userdetailsreport/userdetailsreport{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-userdetailsreport-{_TourId}";

            return key;
        }

        #region " Notifications "

        public String NotificationTopics()
        {
            String key = $"/assets/notification/topics_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-topics_{_TourId}";

            return key;
        }        

        public String UniqueEvents()
        {
            String key = $"/assets/notification/events_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-events_{_TourId}";

            return key;
        }

        public String NotificationText()
        {
            String key = $"/assets/notification/text_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-text_{_TourId}";

            return key;
        }

        public String NotificationStatus()
        {
            String key = $"/assets/notification/notification_status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-statuss_{_TourId}";

            return key;
        }

        #endregion
    }
}