using Newtonsoft.Json.Linq;
using NLog;
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
    public class apiController : Controller
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<JsonResult> getQuestionsAsync(string quiz, string state)
        {
            oauthHelper oauth = Newtonsoft.Json.JsonConvert.DeserializeObject<oauthHelper>(sqlHelper.getStateJson(Guid.Parse(state)));

            oauth.accessToken = sqlHelper.getUserAccessToken(long.Parse(oauth.custom_canvas_user_id));

            if (oauth.accessToken != null)
            {

                //now validate the token to make sure it's still good
                //if the token is no good try to refresh it
                if (oauth.accessToken.tokenRefreshRequired)
                {
                    if (await userCalls.requestUserToken(oauth, oauth.accessToken.responseUrl, "refresh_token", null, oauth.accessToken.refreshToken) == false)
                    {
                        /***********************************************************/
                        //	If we're here it the user may have deleted the access token in their profile.
                        //  In this case we will request a brand new token, forcing the user to "Authorize" again.
                        //  To test this, delete the token in your Canvas user profile.
                        /***********************************************************/

                        return Json(new { Result = "AUTHFAIL" });

                    }

                }


            }
            _logger.Error("Calling for questions");
            JArray questions = await userCalls.getListQuestionsInQuiz(oauth.accessToken.accessToken, "https://" + oauth.host, oauth.custom_canvas_course_id, quiz);

            _logger.Error("Call for questions returned:" + questions.ToString());

            return Json(new { Result = "SUCCESS", Extra = questions.ToString() });
        }
    }
}
