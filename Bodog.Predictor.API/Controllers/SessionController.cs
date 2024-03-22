using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Session;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bodog.Predictor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : BaseController
    {

        private readonly Blanket.Session.User _SessionContext;

        public SessionController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _SessionContext = new Blanket.Session.User(appSettings, aws, postgre, redis, cookies, asset);
        }


        /// <summary>
        /// Creates session for a user
        /// </summary>
        /// <param name="credentials">Payload</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("/user/login")]
        [HttpPost]
        public IActionResult Login(Credentials credentials, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _SessionContext.Login(credentials);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        [HttpPost("{userguid}/userphoneupdate")]
        public IActionResult UserPhoneUpdate(Int32 platformId, Int32 clientId, Int64 phoneNumber, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _SessionContext.UserPhoneUpdate(platformId, clientId, phoneNumber);
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