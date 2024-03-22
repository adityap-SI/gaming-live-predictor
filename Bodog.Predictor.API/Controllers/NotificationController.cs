using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Contracts.Notification;
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
    public class NotificationController : BaseController
    {
        private readonly Blanket.Notification.Subscription _SubscriptionContext;
        private readonly IHostingEnvironment _Env;

        public NotificationController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _SubscriptionContext = new Blanket.Notification.Subscription(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// Performs subscription related operations
        /// </summary>
        /// <param name="subscription">Post Data</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/subscriptions")]
        [ActionName("subscriptions")]
        [HttpPost]
        public async Task<IActionResult> Subscription([FromBody]Subscription subscription, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    String language = "en";
                    HTTPResponse response = await _SubscriptionContext.Subscriptions(subscription, language);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Updates the user's subscription details on device update
        /// </summary>
        /// <param name="subscription">Post Data</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/deviceupdate")]
        [ActionName("deviceupdate")]
        [HttpPost]
        public async Task<IActionResult> DeviceUpdate([FromBody]Subscription subscription, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    String language = "en";
                    HTTPResponse response = await _SubscriptionContext.DeviceUpdate(subscription, language);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Get Events subscribed by a user
        /// </summary>
        /// <param name="deviceId"> Id of the user's Device </param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/usersubscriptions")]
        [ActionName("usersubscriptions")]
        [HttpGet]
        public async Task<IActionResult> GetUserSubscription(String deviceId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _SubscriptionContext.EventsGet(deviceId);

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