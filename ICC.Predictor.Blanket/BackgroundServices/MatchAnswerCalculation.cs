using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Automate;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.BackgroundServices
{
    public class MatchAnswerCalculation : Common.BaseBlanket
    {
        private readonly Feeds.Gameplay _Feeds;
        private readonly int _TourId;

        public MatchAnswerCalculation(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public List<Fixtures> GetFinishedMatches()
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                string lang = "en";
                HTTPResponse httpResponse = _Feeds.GetFixtures(lang).Result;

                if (httpResponse.Meta.RetVal == 1)
                {
                    if (httpResponse.Data != null)
                        fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                }
                else
                    throw new Exception("GetFixtures RetVal is not 1.");

                if (fixtures != null && fixtures.Any())
                {
                    //Fetch all open matches. Not considering Live matches.
                    fixtures = fixtures.Where(i => i.Match_Inning_Status == 6 && i.IsQuestionAnswerProcess == 0 && i.MatchStatus == 2).ToList();
                }
                else
                    throw new Exception("Fixtures is either - NULL OR Has matches with no data.");
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServices.MatchAnswerCalculation.GetFinishedMatches: " + ex.Message);
            }

            return fixtures;
        }

        public List<Fixtures> GetFirstInningFinishedMatches(int matchId)
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                string lang = "en";
                HTTPResponse httpResponse = _Feeds.GetFixtures(lang).Result;

                if (httpResponse.Meta.RetVal == 1)
                {
                    if (httpResponse.Data != null)
                        fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                }
                else
                    throw new Exception("GetFixtures RetVal is not 1.");

                if (fixtures != null && fixtures.Any())
                {
                    //Fetch all open matches. Not considering Live matches.
                    fixtures = fixtures.Where(i => i.MatchId == matchId).ToList();
                }
                else
                    throw new Exception("Fixtures is either - NULL OR Has matches with no data.");
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServices.MatchAnswerCalculation.GetFinishedMatches: " + ex.Message);
            }

            return fixtures;
        }
    }
}
