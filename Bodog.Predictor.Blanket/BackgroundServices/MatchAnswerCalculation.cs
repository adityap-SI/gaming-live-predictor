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

namespace Bodog.Predictor.Blanket.BackgroundServices
{
    public class MatchAnswerCalculation : Common.BaseBlanket
    {
        private readonly Feeds.Gameplay _Feeds;
        private readonly Int32 _TourId;

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
                String lang = "en";
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

        public List<Fixtures> GetFirstInningFinishedMatches(Int32 matchId)
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                String lang = "en";
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
