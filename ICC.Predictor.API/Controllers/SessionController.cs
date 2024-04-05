using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICC.Predictor.Blanket.Session;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Session;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ICC.Predictor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : BaseController
    {

        private readonly User _SessionContext;

        public SessionController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            IHttpContextAccessor httpContext)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _SessionContext = new User(appSettings, aws, postgre, redis, cookies, asset);
        }


        /// <summary>
        /// Creates session for a user
        /// </summary>
        /// <param name="credentials">Payload</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("/user/login")]
        [HttpPost]
        public IActionResult Login(Credentials credentials, string backdoor = null)
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
        public IActionResult UserPhoneUpdate(int platformId, int clientId, long phoneNumber, string backdoor = null)
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