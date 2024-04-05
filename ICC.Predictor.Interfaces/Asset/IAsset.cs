using System;
using System.Threading.Tasks;

namespace ICC.Predictor.Interfaces.Asset
{
    public interface IAsset
    {
        Task<string> GET(string key);

        Task<bool> SET(string key, object content, bool serialize = true);

        string Languages();

        string Fixtures(string lang);

        string Skills(string lang);

        string MatchQuestions(int? MatchQuestions);

        string RecentResult();

        string MatchInningStatus(int MatchId);

        string LeaderBoard(int vOptType, int gamedayId, int phaseId);

        string Debug(string FileName);

        string ShareImage(string FileName);

        string CurrentGamedayMatches();

        string UniqueEvents();

        string NotificationTopics();

        string NotificationStatus();

        string NotificationText();

        string UserDetailsReport();
    }
}