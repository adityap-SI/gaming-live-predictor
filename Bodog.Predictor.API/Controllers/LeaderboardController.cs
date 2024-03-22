using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Bodog.Predictor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : BaseController
    {
        private readonly Blanket.Leaderboard.Leaderbaord _LeaderbaordContext;
        private readonly IHostingEnvironment _Env;

        public LeaderboardController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _LeaderbaordContext = new Blanket.Leaderboard.Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        ///<summary>
        ///Returns users rank.
        ///</summary>
        /// <param name="optType">1 for Overall rank | 2 for Daily rank | 3 for Weekly rank</param>
        /// <param name="gamedayId">Used in case of fetching a user's rank Daily. Pass 0 in case of other.</param>
        /// <param name="phaseId">Used in case of fetching a user's rank Weekly. Pass 0 in case of other.</param>
        /// <param name="userguid">The GUID of the user</param>
        ///<param name="backdoor">backdoor</param>
        ///<returns></returns>
        [HttpGet("{userguid}/getuserrank")]
        public async Task<IActionResult> GetUserRank(Int32 optType, Int32 gamedayId, Int32 phaseId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetUserRank(optType, gamedayId, phaseId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// API to fetch the Leaderboard for Bodog Predictor game
        /// </summary>
        /// <param name="optType">1 for Overall rank | 2 for Daily rank | 3 for Weekly rank</param>
        /// <param name="gamedayId">Used in case of fetching a user's rank Daily. Pass 0 in case of other.</param>
        /// <param name="phaseId">Used in case of fetching a user's rank Weekly. Pass 0 in case of other.</param>
        /// <param name="pageOneChunk">First page size value</param>
        /// <param name="pageChunk">The chunk size</param>
        /// <param name="pageNo">Pagination parameter. Current page no.</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("leaders")]
        [ActionName("leaders")]
        [HttpGet]
        public async Task<IActionResult> Leaders(Int32 optType, Int32 gamedayId, Int32 phaseId, Int32 pageOneChunk, Int32 pageChunk, Int32 pageNo, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetTopRank(optType, phaseId, gamedayId, pageOneChunk, pageChunk, pageNo);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns played gamedays.
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("{userguid}/playedgamedays")]
        public async Task<IActionResult> GetPlayedGameDays(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.PlayedGamedays();
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

    }
}