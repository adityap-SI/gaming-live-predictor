using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Blanket.Leaderboard;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace ICC.Predictor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : BaseController
    {
        private readonly Leaderbaord _LeaderbaordContext;
        private readonly IWebHostEnvironment _Env;

        public LeaderboardController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IWebHostEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _LeaderbaordContext = new Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
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
        public async Task<IActionResult> GetUserRank(int optType, int gamedayId, int phaseId, string backdoor = null)
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
        /// API to fetch the Leaderboard for ICC Predictor game
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
        public async Task<IActionResult> Leaders(int optType, int gamedayId, int phaseId, int pageOneChunk, int pageChunk, int pageNo, string backdoor = null)
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
        public async Task<IActionResult> GetPlayedGameDays(string backdoor = null)
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