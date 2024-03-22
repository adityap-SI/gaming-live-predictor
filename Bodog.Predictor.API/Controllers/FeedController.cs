using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bodog.Predictor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : BaseController
    {
        private readonly Blanket.Feeds.Gameplay _FeedContext;
        private readonly IHostingEnvironment _Env;

        public FeedController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _FeedContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// API Status check
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        [ProducesResponseType(200)]
        public ActionResult Ping()
        {
            if (_Env.IsDevelopment())
            {
                string jsonFile = "Bodog.Predictor.API.json";
                string jsonPath = System.IO.Path.Combine(@"D:\GitHub\gaming-bodog-predictor\Bodog.Predictor.API\bin\Debug\netcoreapp2.1\", jsonFile);
                string swaggerUrl = "http://localhost:56801/swagger/v1/swagger.json";
                System.IO.File.WriteAllText(jsonPath, Library.Utility.GenericFunctions.GetWebData(swaggerUrl));
            }

            return Ok("Bodog.Predictor.API");
        }

        /// <summary>
        /// Returns all the available languages
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("languages")]
        public async Task<IActionResult> Languages(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetLanguages();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns language-wise fixtures for the tournament
        /// </summary>
        /// <param name="lang">Language | Default = en</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("fixtures")]
        public async Task<IActionResult> Fixtures(string lang, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    lang = await _FeedContext.DefaultLang(lang);

                    HTTPResponse response = await _FeedContext.GetFixtures(lang);

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
