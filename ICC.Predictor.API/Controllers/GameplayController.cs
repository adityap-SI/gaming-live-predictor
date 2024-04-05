using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Blanket.Feeds;
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
    public class GameplayController : BaseController
    {
        private readonly Gameplay _GamePlayContext;
        private readonly IWebHostEnvironment _Env;

        public GameplayController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IWebHostEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _GamePlayContext = new Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        #region " GET "

        /// <summary>
        /// Returns Question for the match.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("matchquestions")]
        public async Task<IActionResult> MatchQuestions(int MatchId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetQuestions(MatchId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        ///<summary>
        ///Returns prediction submitted by users.
        ///</summary>
        ///<param name="MatchId">MatchId</param>
        ///<param name="GameDayId">GameDayId</param>
        /// <param name="userguid">The GUID of the user</param>
        ///<param name="backdoor">backdoor</param>
        ///<returns></returns>
        [HttpGet("{userguid}/getpredictions")]
        public async Task<IActionResult> GetPredictions(int MatchId, int GameDayId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetUserPredictions(MatchId, GameDayId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Return recent results of teams.
        /// </summary>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpGet("recentresults")]
        public async Task<IActionResult> RecentResults(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetRecentResults();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns match status and match inning status of match.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpGet("matchinningstatus")]
        public async Task<IActionResult> MatchInningStatus(int MatchId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.MatchInningStatus(MatchId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        [HttpGet("{userguid}/getprofile")]
        public async Task<IActionResult> GetProfile(int PlatformId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.GetUserProfile(PlatformId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }


        ///<summary>
        ///Returns prediction submitted by users.
        ///</summary>
        ///<param name="MatchId">MatchId</param>
        ///<param name="GameDayId">GameDayId</param>
        ///<param name="UserId">UserId</param>
        ///<param name="UserTeamId">GameDayId</param>
        /// <param name="userguid">The GUID of the user</param>
        ///<param name="backdoor">backdoor</param>
        ///<returns></returns>
        [HttpGet("{userguid}/getotheruserpredictions")]
        public async Task<IActionResult> GetOtherUserPredictions(int MatchId, int GameDayId, string UserId, string UserTeamId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetOtherUserPredictions(MatchId, GameDayId, UserId, UserTeamId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        [HttpGet("{userguid}/getgameplays")]
        public async Task<IActionResult> GetGamePlays(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetGamePlays();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        #endregion " GET "

        #region " POST "
        /// <summary>
        /// Submit the User's predictions.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="TourGamedayId">TourGamedayId</param>
        /// <param name="QuestionId">QuestionId</param>
        /// <param name="OptionId">OptionId</param>
        /// <param name="PlatformId">PlatformId - 1 Android | 2 IOS</param>
        /// <param name="userguid">The GUID of the user</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpPost("{userguid}/userprediction")]
        public IActionResult UserPrediction(int MatchId, int TourGamedayId, int QuestionId, int OptionId, int PlatformId, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _GamePlayContext.UserPrediction(MatchId, TourGamedayId, QuestionId, OptionId, PlatformId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        #endregion " POST "
    }
}