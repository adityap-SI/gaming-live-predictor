using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;

namespace ICC.Predictor.Library.Asset
{
    public class Constants : Read, Interfaces.Asset.IAsset
    {
        private readonly string _RedisBaseKey;

        public Constants(IAWS aws, IRedis redis, IOptions<Application> appSettings) : base(aws, redis, appSettings)
        {
            _RedisBaseKey = appSettings.Value.Properties.ClientName + $"-fantasy-{_Connection}-{_TourId}";
        }

        public string Languages()
        {
            string key = $"/assets/languages/languages_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-languages";

            return key;
        }

        public string Fixtures(string lang)
        {
            string key = $"/assets/fixtures/fixtures_{_TourId}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-fixtures-{lang}";

            return key;
        }

        public string Skills(string lang)
        {
            string key = $"/assets/skill/skill_{_TourId}_{lang}.html";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-skill-{lang}";

            return key;
        }

        public string MatchQuestions(int? QuestionsMatchID)
        {
            string key = $"/assets/matchquestions/questions_{_TourId}_{QuestionsMatchID}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-questions_-{QuestionsMatchID}";

            return key;
        }

        public string RecentResult()
        {
            string key = $"/assets/recentmatchresults/recentresults_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-recentresults_-{_TourId}";

            return key;
        }

        public string MatchInningStatus(int MatchId)
        {
            string key = $"/assets/matchinningstatus/matchstatus_{_TourId}_{MatchId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-matchinningstatus_-{MatchId}";

            return key;
        }

        public string LeaderBoard(int vOptType, int gamedayId, int phaseId)
        {
            string key = $"/assets/leaderboard/leaderboard_{_TourId}_{vOptType}_{gamedayId}_{phaseId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-leaderboard_-{vOptType}_{gamedayId}_{phaseId}";

            return key;
        }

        public string Debug(string FileName)
        {
            string key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public string ShareImage(string FileName)
        {
            string key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public string CurrentGamedayMatches()
        {
            string key = $"/assets/currentgameday/currentgamedaymatches_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-currentgamedaymatches-{_TourId}";

            return key;
        }

        public string UserDetailsReport()
        {
            string key = $"/assets/userdetailsreport/userdetailsreport{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-userdetailsreport-{_TourId}";

            return key;
        }

        #region " Notifications "

        public string NotificationTopics()
        {
            string key = $"/assets/notification/topics_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-topics_{_TourId}";

            return key;
        }

        public string UniqueEvents()
        {
            string key = $"/assets/notification/events_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-events_{_TourId}";

            return key;
        }

        public string NotificationText()
        {
            string key = $"/assets/notification/text_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-text_{_TourId}";

            return key;
        }

        public string NotificationStatus()
        {
            string key = $"/assets/notification/notification_status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-statuss_{_TourId}";

            return key;
        }

        #endregion
    }
}