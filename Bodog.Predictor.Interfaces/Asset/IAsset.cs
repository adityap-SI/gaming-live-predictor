using System;
using System.Threading.Tasks;

namespace Bodog.Predictor.Interfaces.Asset
{
    public interface IAsset
    {
        Task<String> GET(String key);

        Task<bool> SET(String key, Object content, bool serialize = true);

        String Languages();
        
        String Fixtures(String lang);

        String Skills(String lang);

        String MatchQuestions(Int32? MatchQuestions);

        String RecentResult();

        String MatchInningStatus(Int32 MatchId);

        String LeaderBoard(Int32 vOptType, Int32 gamedayId, Int32 phaseId);

        String Debug(String FileName);

        String ShareImage(String FileName);

        String CurrentGamedayMatches();

        String UniqueEvents();

        String NotificationTopics(); 

        String NotificationStatus();

        String NotificationText();

        String UserDetailsReport();
    }
}