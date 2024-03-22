using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
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
    public class ShareController : BaseController
    {
        private readonly Blanket.Sharing.ImageGeneration _ImageGenerationContext;
        private readonly IHostingEnvironment _Env;
        private readonly String _Domain;
        protected string _AWSS3FolderPath;
        protected readonly IAWS _AWS;

        public ShareController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _ImageGenerationContext = new Blanket.Sharing.ImageGeneration(appSettings, aws, postgre, redis, cookies, asset, env);
            _Env = env;
            _Domain = appSettings.Value.API.Domain;
            _AWSS3FolderPath = appSettings.Value.Connection.AWS.S3FolderPath;
            _AWS = aws;
        }

        /// <summary>
        /// Generates Share Image.
        /// </summary>
        /// <param name="userguid">The GUID of the user</param>
        /// <param name="matchid">MatchId</param>
        /// <param name="gamedayid">GamedayId</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(String userguid, Int32 matchid, Int32 gamedayid, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _ImageGenerationContext.GenerateImage(userguid, matchid, gamedayid);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// WEB - Generates a user's points images and return meta tags to share on Facebook.
        /// </summary>        
        /// <param name="userguid">User GUID from Cookie</param>
        /// <param name="matchid">Id of the match for which the image is to be shared</param>
        /// <param name="gamedayid">Id of the gameday for which the image is to be shared</param>
        /// <param name="title">Title of the share</param>
        /// <param name="description">Description of the share</param>  
        /// <param name="redirectLink">Link For Redirect</param>
        /// <param name="backdoor"></param>
        /// <returns>An Image</returns>
        [HttpGet("{userguid}/facebookshare")]
        public async Task<ActionResult<HttpResponseMessage>> GetFbShareMetaTags(String userguid, Int32 matchid, Int32 gamedayid, String title, String description = "", String redirectLink = null, string backdoor = null)
        {
            if (ModelState.IsValid)
            {

                //System.Web.HttpContext.Current.Response.AppendHeader("Edge-control", "cache-maxage=0s");
                Response.Headers.Add("Edge-control", "cache-maxage=0s");

                HttpResponseMessage response = new HttpResponseMessage();
                HTTPResponse mHTTPResponse = new HTTPResponse();
                StringBuilder mSb = new StringBuilder();
                try
                {

                    String vURL = String.Empty;                   
                    String mTitle = !String.IsNullOrEmpty(title) ? HttpUtility.UrlDecode(title) : "";
                    String mDescription = !String.IsNullOrEmpty(description) ? HttpUtility.UrlDecode(description) : "";
                    String mRedirectLink = !String.IsNullOrEmpty(redirectLink) ? HttpUtility.UrlDecode(redirectLink) : "";

                    
                    String fileName = userguid + "_" + matchid + "_" + gamedayid + ".jpg";
                    String date = DateTime.UtcNow.Date.ToString("MM-dd-yyyy");
                    String key = date + "/" + fileName;
                    vURL = "https://" + _Domain + "/static-assets/image-share/" + key + "?ts=" + DateTime.Now.ToString("ddmmyyHHss");
                    //vURL = _Domain + "/api/share/" + userguid + "/getshareimage" + "?userguid=" + userguid + "&matchid=" + matchid + "&gamedayid=" + gamedayid;


                    mSb.Append("<!DOCTYPE html>");
                    mSb.Append("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
                    mSb.Append("<head><meta charset=\"utf-8\"/><title></title>");

                    mSb.Append("<meta name=\"thumb\" content=" + vURL + ">");

                    mSb.Append("<meta property=\"og:image\" content=\"" + vURL + "\" />");

                    mSb.Append("<meta  property=\"og:image:width\" content=\"1200\" />");
                    mSb.Append("<meta  property=\"og:image:height\" content=\"628\" />");

                    mSb.Append("<meta property=\"og:site_name\" content=\"FIFA Worldcup Fantasy\" />");
                    mSb.Append("<meta  property=\"og:type\" content=\"website\" />");
                    mSb.Append("<meta  property=\"og:title\" content=\"" + mTitle + "\" />");
                    mSb.Append("<meta  property=\"og:description\" content=\"" + mDescription + "\" />");

                    mSb.Append("<meta name = \"twitter:card\" content = \"summary_large_image\" />");
                    mSb.Append("<meta name = \"twitter:title\" content = \"" + mTitle + "\" >");
                    mSb.Append("<meta name = \"twitter:description\" content = \"" + mDescription + " \" >");
                    mSb.Append("<meta name = \"twitter:image\" content = \"" + vURL + "\" >");

                    mSb.Append("<script>window.location.href='" + mRedirectLink + "'</script></head><body></body></html>");

                    response.Content = new StringContent(mSb.ToString());
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");


                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = mSb.ToString()
                };

                }
                catch { }

                return response;
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// WEB - Generates a user's points images and return meta tags to share on Twitter.
        /// </summary>        
        /// <param name="userguid">User GUID from Cookie</param>
        /// <param name="matchid">Id of the match for which the image is to be shared</param>
        /// <param name="gamedayid">Id of the gameday for which the image is to be shared</param>
        /// <param name="title">Title of the share</param>
        /// <param name="description">Description of the share</param>  
        /// <param name="redirectLink">Link For Redirect</param>
        /// <param name="backdoor"></param>
        /// <returns>An Image</returns>
        [HttpGet("{userguid}/twittershare")]
        public async Task<ActionResult<RedirectResult>> TwitterPostImage(String userguid, Int32 matchid, Int32 gamedayid, String title, String description = "", String redirectLink = null, string backdoor = null)
        {
            if (ModelState.IsValid)
            {

                //System.Web.HttpContext.Current.Response.AppendHeader("Edge-control", "cache-maxage=0s");
                Response.Headers.Add("Edge-control", "cache-maxage=0s");

                HttpResponseMessage response = new HttpResponseMessage();
                HTTPResponse mHTTPResponse = new HTTPResponse();
                String vURL = String.Empty;
                try
                {
                    String mTitle = !String.IsNullOrEmpty(title) ? HttpUtility.UrlDecode(title) : "";
                    String mDescription = !String.IsNullOrEmpty(description) ? HttpUtility.UrlDecode(description) : "";
                    String mRedirectLink = !String.IsNullOrEmpty(redirectLink) ? HttpUtility.UrlDecode(redirectLink) : "";


                    vURL = Uri.EscapeDataString(_Domain + "/api/share/" + userguid + "/facebookshare?userguid=" + userguid + "&matchid=" + matchid + "&gamedayid=" + gamedayid + "&title=" + HttpUtility.UrlEncode(mTitle) + "&description=" + HttpUtility.UrlEncode(mDescription) + "&redirectLink=" + redirectLink);

                    vURL = "https://twitter.com/intent/tweet?url=" + vURL.Replace("%20", "%2B").Replace("%23", "%2523");

                }
                catch { }

                return Redirect(vURL);
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// WEB - Get user's points image.     
        /// </summary>
        /// <param name="userguid">The GUID of the user</param>
        /// <param name="matchid">MatchId</param>
        /// <param name="gamedayid">GamedayId</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns>An Image</returns>
        [HttpGet("{userguid}/getshareimage")]
        public async Task<ActionResult<HttpResponseMessage>> GetShareImage(String userguid, Int32 matchid, Int32 gamedayid, string backdoor = null)
        {
            if (ModelState.IsValid)
            {

                //System.Web.HttpContext.Current.Response.AppendHeader("Edge-control", "cache-maxage=0s");
                Response.Headers.Add("Edge-control", "cache-maxage=0s");

                HttpResponseMessage response = new HttpResponseMessage();
                try
                {

                    String fileName = userguid + "_" + matchid + "_" + gamedayid;
                    String date = DateTime.UtcNow.Date.ToString("MM-dd-yyyy");

                    String key = "/assets/image-share/" + date + "/" + fileName + ".jpg";


                    byte[] byteArray = await _AWS.ReadS3Image(key);

                    //response.Content = new ByteArrayContent(byteArray);
                    //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                    return File(byteArray, "image/jpeg");

                }
                catch { }

                return response;
            }
            else
                return BadRequest();
        }
    }
}