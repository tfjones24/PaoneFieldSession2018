using Newtonsoft.Json.Linq;
using SupaSpeedGrader.API;
using SupaSpeedGrader.Helpers;
using SupaSpeedGrader.Models;
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
        public async Task<ActionResult> Index()
        {
            oauthHelper oauth = new oauthHelper(Request);

            /***********************************************************/
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (oauth.verifySignature())
            {
                //TODO: Get course ID
                string course_id = Request.Form.Get("custom_canvas_course_id");
                Uri testUri = Request.Url;

                //TODO: Get user key from storage
                //TODO: Get all questions or something
                JArray sectionsJSON = await userCalls.getSectionList("9802~jT11gMJZiaByfs7vBVI2PFQje0YhKwtunlzpw8h6HAMuELHGXodejJzT2mONVMdS", "https://" + Request.UrlReferrer.Host, course_id);
                JArray quizListJSON = await userCalls.getListQuizzesInCourse("9802~jT11gMJZiaByfs7vBVI2PFQje0YhKwtunlzpw8h6HAMuELHGXodejJzT2mONVMdS", "https://" + Request.UrlReferrer.Host, course_id);

                //TODO: Get quesiton data ready to go
                NavigationModel nav = new NavigationModel();

                //Objects for section names and IDs to pass in
                List<string> sections = new List<string>();
                Dictionary<string, string> sectionIDs = new Dictionary<string, string>();

                //Objects for quiz names and IDs to pass in
                List<string> quizzes = new List<string>();
                Dictionary<string, string> quizIDs = new Dictionary<string, string>();

                //Get section data extracted
                JToken token = sectionsJSON.First;
                List<JToken> tokens = new List<JToken>();

                //Get all tokens from section data
                do
                {
                    tokens.Add(token);
                    token = token.Next;
                } while (token != null);
                //Parse out name and ID for use later
                for (int i = 0; i < tokens.Count; i++)
                {
                    sections.Add(tokens[i].Value<string>("name"));
                    sectionIDs.Add(tokens[i].Value<string>("name"), tokens[i].Value<string>("id"));
                }

                //Now do quiz data
                token = quizListJSON.First;
                tokens = new List<JToken>();

                //Get all tokens from quiz data
                while (token != null)
                {
                    tokens.Add(token);
                    token = token.Next;
                }
                //Parse out name and ID for use later
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i].Value<string>("published").ToLower() == "true")
                    {
                        quizzes.Add(tokens[i].Value<string>("title"));
                        quizIDs.Add(tokens[i].Value<string>("title"), tokens[i].Value<string>("id"));
                    }
                }


                //TODO: Put question data in model
                //Put section and quiz info into nav model
                nav.sections = sections;
                nav.sectionID = sectionIDs;

                nav.quizzes = quizzes;
                nav.quizID = quizIDs;

                return View(nav);
            }

            //Oops, not canvas or someone trying to breach our shit. Give em good test data.
            NavigationModel nav2 = new NavigationModel();
            nav2.addHardValue();
            return View(nav2);
        }

        //Returns the grading page with the selected question
        public ActionResult Grade()
        {
            return View(new GradeModel());
        }

        //Let's see some help
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

        // OH GOD WILLS authentication token: 9802~jT11gMJZiaByfs7vBVI2PFQje0YhKwtunlzpw8h6HAMuELHGXodejJzT2mONVMdS
        // Just a good reference for post-token creation work, inclduign sample API call
        public async Task<ActionResult> willName()
        {
            oauthHelper oauth = new oauthHelper(Request);

            /***********************************************************/
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (oauth.verifySignature())
            {
                string user_id = Request.Form.Get("custom_canvas_user_id");

                Uri testUri = Request.Url;

                JObject rval = await userCalls.getUserData("9802~jT11gMJZiaByfs7vBVI2PFQje0YhKwtunlzpw8h6HAMuELHGXodejJzT2mONVMdS", "https://" + Request.UrlReferrer.Host, user_id);

                

                willNameModel model = new willNameModel();
                model.name = "SUCCESSish";
                model.id = jsonHelpers.GetJObjectValue(rval, "name");
                return View(model);
            }
            return View(new willNameModel());
        }

        
    }
}