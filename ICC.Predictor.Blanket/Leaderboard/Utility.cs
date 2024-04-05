using System;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Contracts.Leaderboard;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Blanket.Leaderboard
{
    class Utility : BaseBlanket
    {
        public Utility(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
               : base(appSettings, aws, postgre, redis, cookies, asset)
        {
        }

        public ResponseObject FetchRecords(string data, int from, int to)
        {
            ResponseObject res = new ResponseObject();

            try
            {
                res = GenericFunctions.Deserialize<ResponseObject>(data);

                if (res != null)
                {
                    Top ranks = GenericFunctions.Deserialize<Top>(GenericFunctions.Serialize(res.Value));

                    if (ranks != null && ranks.Users != null && ranks.Users.Any())
                    {
                        List<Users> users = new List<Users>();

                        try
                        {
                            int startIndex = from - 1;

                            if (ranks.Users.Count >= to)
                                users = ranks.Users.GetRange(startIndex, to - startIndex);
                            else
                                users = ranks.Users.GetRange(startIndex, ranks.Users.Count - startIndex);
                        }
                        catch
                        {
                            //Reaches here when list count is less than start index (starting point of record).
                            users = new List<Users>();
                        }

                        ranks.Users = users;
                    }

                    res.Value = ranks;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

    }
}
