using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LiveController : BaseController
    {
        private readonly Blanket.Feeds.Gameplay _FeedContext;
        private readonly IWebHostEnvironment _Env;

        public LiveController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IWebHostEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _FeedContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// Returns Current Gameday Matches  
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("currentgamedaymatches")]
        public async Task<IActionResult> CurrentGamedayMatches(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetCurrentGamedayMatches();

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