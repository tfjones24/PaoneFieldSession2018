using SupaSpeedGrader.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SupaSpeedGrader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grade()
        {
            return View();
        }
        public ActionResult Help()
        {
            return View();
        }
        // Following code copied in as a base

        /// <summary>
		/// Step 1: redirect the user to authenticate through Canvas
		/// https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-1
		/// 
		/// This is the method that should be defined in the LTI XML config
		/// When the user clicks on the name of the app in Canvas, it should 
		/// post the LTI launch request to this method.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        public async Task<ActionResult> oauth2Request()
        {
            oauthHelper oauth = new oauthHelper(Request);

            /***********************************************************/
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (oauth.verifySignature())
            {
                string oauth2ClientId = (ConfigurationManager.AppSettings["oauth2ClientId"] != null) ? ConfigurationManager.AppSettings["oauth2ClientId"] : Guid.NewGuid().ToString();
                string redirectUrl = string.Format("{0}://{1}/login/oauth2/auth?client_id={2}&&response_type=code&redirect_uri={3}&state={4}", Request.UrlReferrer.Scheme, Request.UrlReferrer.DnsSafeHost, oauth2ClientId, Request.Url.ToString().Replace("oauth2Request", "oauth2Response"), Guid.NewGuid().ToString());
                Response.Redirect(redirectUrl, true);
            }

            /***********************************************************/
            //	If we're here LTI validation failed, return a dead view
            /***********************************************************/
            /*resultModel model = new resultModel();
            model.title = "What happened...";
            model.message = "Uh-oh looks like LTI validation failed, the user should never see this view.";*/
            return View();
        }

        /// <summary>
        /// Step 2: If step one succeeded and the Developer Key is properly defined,
        /// 		the next call received by this method, after the user clicks "Authorize"
        /// https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-1		
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> oauth2Response()
        {
            /*
            resultModel model = new resultModel();
            model.title = "Success!!";
            model.message = "Step 2 of the OAuth2 workflow was executed, as described here:";
            model.linkTitle = "Step 2: Redirect back to the request_uri, or out-of-band redirect";
            model.link = "https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-2";
            return View("result", model);
            */
            return View();
        }

    }
}