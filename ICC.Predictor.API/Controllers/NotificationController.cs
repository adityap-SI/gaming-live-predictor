using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Notification;
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
    public class NotificationController : BaseController
    {
        private readonly Blanket.Notification.Subscription _SubscriptionContext;
        private readonly IWebHostEnvironment _Env;

        public NotificationController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IWebHostEnvironment env)
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
        public async Task<IActionResult> Subscription([FromBody] Subscription subscription, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    string language = "en";
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
        public async Task<IActionResult> DeviceUpdate([FromBody] Subscription subscription, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    string language = "en";
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
        public async Task<IActionResult> GetUserSubscription(string deviceId, string backdoor = null)
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