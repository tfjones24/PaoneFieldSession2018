﻿using Newtonsoft.Json.Linq;
using NLog;
using SupaSpeedGrader.API;
using SupaSpeedGrader.Helpers;
using SupaSpeedGrader.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SupaSpeedGrader.Controllers
{
    public class HomeController : Controller
    {
        public static bool devMode = true;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
		/// These are the values generated by Canvas when you create a Developer Key
		/// https://community.canvaslms.com/docs/DOC-5141
		/// </summary>
		private static string oauth2ClientId = (System.Configuration.ConfigurationManager.AppSettings["oauth2ClientId"] != null) ? System.Configuration.ConfigurationManager.AppSettings["oauth2ClientId"] : string.Empty;
        private static string oauth2ClientKey = (System.Configuration.ConfigurationManager.AppSettings["oauth2ClientKey"] != null) ? System.Configuration.ConfigurationManager.AppSettings["oauth2ClientKey"] : string.Empty;


        public async Task<ActionResult> Index(string state = null)
        {
            oauthHelper oauth = new oauthHelper(Request);
            bool letsGo = false;

            if (devMode || state == null)
            {
                if (!oauth.verifySignature() && !devMode)
                {
                    resultModel model = new resultModel();
                    model.title = "Error Forbidden";
                    model.message = "Bad access. Relaunch LTI app to request new token.";
                    return View("result", model);
                }

                if (oauth.verifySignature())
                {
                    oauth.accessToken = new userAccessToken("9802~Zvtl4cszHBTBQ9z6aAAQ0Mxn9DnyjdVwEukgemkZViqqwVX8jadCGKSFygMzvz0E", Convert.ToInt64(oauth.custom_canvas_user_id), 15000);
                    letsGo = true;
                }
                else
                {
                    oauth.accessToken = new userAccessToken("9802~Zvtl4cszHBTBQ9z6aAAQ0Mxn9DnyjdVwEukgemkZViqqwVX8jadCGKSFygMzvz0E", 2033, 15000);
                    //letsGo = true;
                }

                if (devMode && state == null)
                {
                    Guid stateId = Guid.NewGuid();
                    string jsonState = Newtonsoft.Json.JsonConvert.SerializeObject(oauth);
                    sqlHelper.storeState(stateId, jsonState);
                    state = stateId.ToString();
                }

            }
            else
            {
                //let's deserialize that [state] json into an object so we can reference the variables
                oauth = Newtonsoft.Json.JsonConvert.DeserializeObject<oauthHelper>(sqlHelper.getStateJson(Guid.Parse(state)));

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

                            resultModel model = new resultModel();
                            model.title = "Error Forbidden";
                            model.message = "Bad access token. Relaunch LTI app to request new token.";
                            return View("result", model);

                        }

                    }

                    letsGo = true;

                }
            }


            /***********************************************************/
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (letsGo)
            {
                string course_id = oauth.custom_canvas_course_id;
                Uri testUri = oauth.Url;

                //TODO: Get user key from storage
                //TODO: Get all questions or something
                JArray sectionsJSON = await userCalls.getSectionList(oauth.accessToken.accessToken, "https://" + oauth.host, course_id);
                JArray quizListJSON = await userCalls.getListQuizzesInCourse(oauth.accessToken.accessToken, "https://" + oauth.host, course_id);

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
                        nav.quizColor.Add(tokens[i].Value<string>("title"), "unstarted");
                    }
                }


                //TODO: Put question data in model
                //Put section and quiz info into nav model
                nav.sections = sections;
                nav.sectionID = sectionIDs;

                nav.quizzes = quizzes;
                nav.quizID = quizIDs;

                nav.state = state;

                return View(nav);
            }

            _logger.Error("Bad Index Login attempt! " + state);
            //Oops, not canvas or someone trying to breach our shit. Give em good test data.
            NavigationModel nav2 = new NavigationModel();
            nav2.addHardValue();
            nav2.state = state;
            return View(nav2);
        }

        //Returns the grading page with the selected question
        // quiz - quizID passed from previous page
        // section[] - array of all sections selected, can include "all" as section
        // question - questionID to grade
        // state - unique stateID of user
        public async Task<ActionResult> Grade(string quiz, string[] section, string question, string state)
        {
            // Create a grade model
            GradeModel model = new GradeModel();

            // Let's pull in a save state!
            _logger.Error("Pulling in a saved state: " + state);
            oauthHelper oauth = Newtonsoft.Json.JsonConvert.DeserializeObject<oauthHelper>(sqlHelper.getStateJson(Guid.Parse(state)));

            oauth.accessToken = sqlHelper.getUserAccessToken(long.Parse(oauth.custom_canvas_user_id));
            _logger.Error("State loaded: " + state);

            // Refresh token time! just keeps shit updated
            if (oauth.accessToken != null)
            {
                _logger.Error("Checking token validity for state: " + state);
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
                        _logger.Error("Token bad, renewal failed! state: " + state);
                        resultModel modelFail = new resultModel();
                        modelFail.title = "Error Forbidden";
                        modelFail.message = "Bad access token. Relaunch LTI app to request new token.";
                        return View("result", modelFail);

                    }
                    _logger.Error("token renewed! state: " + state);

                }

            }

            // Okay we good, let's load data into the model and dispatch it
            /* Need:
             * quiz name
             * question name
             * question score
             * question instructions
             * 
             * per student:
             * *student name
             * *student response
             * *student score
             * *student comment
             */

            // Grab the quiz object from Canvas
            JObject quizJSON = await userCalls.getQuizInCourse(oauth.accessToken.accessToken, "https://" + oauth.host, oauth.custom_canvas_course_id, quiz);

            // Load quiz name and ID
            model.quizName = jsonHelpers.GetJObjectValue(quizJSON, "title");
            model.quizID = quiz;

            // Load question name, ID, score
            model.questionName = sqlHelper.getQuestionName(quiz, oauth.custom_canvas_course_id, question); 
            model.questionID = question;
            model.gradeOutOf = Int32.Parse(sqlHelper.getQuestionMaxScore(quiz, oauth.custom_canvas_course_id, question)); 

            // Fuck the rubric, that shit broke
            // TODO: implement the rubric...somehow?
            model.rubricParsed = 1; // Dammit Tanner, why does 1 mean no rubric?

            // Load some student data
            model.numStudent = Int32.Parse(sqlHelper.getNumberQuestions(quiz, oauth.custom_canvas_course_id)); //TODO:grab from SQL database

            //TODO: loop to add all student IDs as names along with creating entry with answer, grade, comment in namesGrades
            string[] studentshit = sqlHelper.getStudentSubmissionSQL(quiz, oauth.custom_canvas_course_id, question, "10874");
            //TODO: make sure this is it...


            return View(model);
        }

        //Let's see some help
        public ActionResult Help()
        {
            return View();
        }
        public ActionResult Rubric()
        {
            return View(new RubricModel());

        }


        public ActionResult FileUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase myFile)
        {
            var fileName = Path.GetFileName(myFile.FileName);
            myFile.SaveAs(Path.Combine(@"c:\projects", fileName));

            HttpFileCollectionBase files = Request.Files;
            return View("FileUpload");
        }

        // Following code copied in as a base, then modified for our own use

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

            bool requireOathRedirect = false;
            string jsonState = string.Empty;

            resultModel model = new resultModel();

            /***********************************************************/
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (oauth.verifySignature())
            {
                //Check to see if this user already has a token
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
                            requireOathRedirect = true;
                            //_logger.Warn("Found existing token, but unable to refresh.  Possible that user deleted the token in their Canvas profile, execute a new OAuth2 workflow to request a new access token.");
                        }
                        else
                        {
                            model.title = "Token Refresh Success!!";
                            model.message = "We have successfully refreshed our existing user access token.";
                            model.linkTitle = "Using a Refresh Token to get a new Access Token";
                            model.link = "https://canvas.instructure.com/doc/api/file.oauth.html#using-refresh-tokens";
                        }

                    }
                    else
                    {
                        model.title = "Success!!";
                        model.message = "Our token is still valid, no need to refresh.";

                        
                    }

                    if (!requireOathRedirect)
                    {
                        //generate a new unique stateId to store the LTI varibles for this uer
                        //  -- the stateId will allow us to retrieve the values on subsequent NON-LTI calls
                        Guid stateId = Guid.NewGuid();
                        jsonState = Newtonsoft.Json.JsonConvert.SerializeObject(oauth);
                        sqlHelper.storeState(stateId, jsonState);

                        //for demo purposes only, we will display the LTI variables on the client, don't do this in production environment, this is only for testing
                        model.stateJson = jsonState;

                        //store the unique stateId in a hidden field so it can be passed back from the client to the server
                        model.oauth2State = stateId.ToString();

                        //display our token information
                        model.accessToken = oauth.accessToken.accessToken;
                        model.refreshToken = oauth.accessToken.refreshToken;
                        model.tokenLife = oauth.accessToken.tokenLife.ToString();

                        //TODO: redirect since token is GOOD!
                        string redirectUrl = string.Format("https://canvas.packamoon.com/Home/Index?state={0}", stateId.ToString());
                        Response.Redirect(redirectUrl, true);
                    }
                }

                if (oauth.accessToken == null || requireOathRedirect)
                {
                    //ok, we either didn't find an existing token, or the existing token was not valid and we need a new one
                    //  this is the OAuth2 redirect back to Canvas to ask the user to authorize our application
                    Guid stateId = Guid.NewGuid();
                    string state = stateId.ToString();
                    jsonState = Newtonsoft.Json.JsonConvert.SerializeObject(oauth);
                    sqlHelper.storeState(state, jsonState);
                    string oauth2ClientId = (ConfigurationManager.AppSettings["oauth2ClientId"] != null) ? ConfigurationManager.AppSettings["oauth2ClientId"] : Guid.NewGuid().ToString();
                    string redirectUrl = string.Format("{0}://{1}/login/oauth2/auth?client_id={2}&response_type=code&state={4}&redirect_uri={3}", Request.UrlReferrer.Scheme, Request.UrlReferrer.DnsSafeHost, oauth2ClientId, Request.Url.ToString().Replace("oauth2Request", "oauthreturn"), state);
                    Response.Redirect(redirectUrl, true);
                }

            }

            /***********************************************************/
            //	If we're here LTI validation failed, return a dead view
            /***********************************************************/
            model = new resultModel();
            model.title = "What happened...";
            model.message = "Uh-oh looks like LTI validation failed, the user should never see this view.";
            return View("result", model);
        }

        /// <summary>
        /// Step 2: If step one succeeded and the Developer Key is properly defined,
        /// 		the next call received by this method, after the user clicks "Authorize"
        /// https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-1		
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> oauthreturn(string code = null, string state = null, string error = null)
        {
            //FOR ILLUSTRATION: notice that when oauth is created, all of the LTI variables are empty
            oauthHelper oauth = new oauthHelper(Request);

            resultModel model = new resultModel();

            if (error == null)
            {
                model.title = "Success!!";
                model.message = "Step 2 of the OAuth2 workflow was executed, as described here:";
                model.linkTitle = "Step 2: Redirect back to the request_uri, or out-of-band redirect";
                model.link = "https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-2";

                // the [code] will be used in the API call to request the user token
                // https://canvas.instructure.com/doc/api/file.oauth.html#oauth2-flow-3
                // this is the [code] parameter called for in Step 3 of the document 
                model.oauth2Code = code;
//
                // this is the [state] identifier that we created in the launch request and sent to Canvas in the redirect to get use authorization
                // we will use this value below to retrieve those launch parameters
                model.oauth2State = state;

                // hopefully this value is always null or empty
                model.oauth2Error = (string.IsNullOrEmpty(error)) ? string.Empty : error;

                //ok, let's get the [state] that we stored in the launch request
                //    -- here i'm storing the state string in the model so it can be displayed in the browser
                //		 NOTE: this is for demo purposes only, you never want to return this data to the client in production
                model.stateJson = sqlHelper.getStateJsonQuery(state);

                //let's deserialize that [state] json into an object so we can reference the variables
                oauth = Newtonsoft.Json.JsonConvert.DeserializeObject<oauthHelper>(sqlHelper.getStateJson(state));

                //now lets get the user access token
                if (await userCalls.requestUserToken(oauth, "https://canvas.packamoon.com/Home/oauthreturn", "authorization_code", code) == false)
                {
                    /***********************************************************/
                    //	If we're here Canvas must have responded with an error, 
                    //  tell the user something went wrong
                    /***********************************************************/
                    model.title = "What happened...";
                    model.message = "Uh-oh looks like Canvas failed to return a user access token.";
                }
                else
                {
                    /***********************************************************/
                    //	If we're here Canvas should have returned an access token, 
                    //  let's display in the browser for testing purposes
                    //  again you never want to return this data to the client in production
                    /***********************************************************/
                    model.accessToken = oauth.accessToken.accessToken;
                    model.refreshToken = oauth.accessToken.refreshToken;
                    model.tokenLife = oauth.accessToken.tokenLife.ToString();

                    string jsonState = Newtonsoft.Json.JsonConvert.SerializeObject(oauth);
                    sqlHelper.storeState(state, jsonState);

                    string redirectUrl = string.Format("https://canvas.packamoon.com/Home/Index?state={0}", state);
                    Response.Redirect(redirectUrl, true);
                }
            }
            else
            {
                /***********************************************************/
                //	If we're here Canvas must have responded with an error
                /***********************************************************/
                model.title = "What happened...";
                model.message = "Uh-oh looks like Canvas responded with an error.";
            }

            //TODO: store new token, pass off to rest of app

            return View("result", model);
        }



        // OH GOD WILLS authentication token: 9802~jT11gMJZiaByfs7vBVI2PFQje0YhKwtunlzpw8h6HAMuELHGXodejJzT2mONVMdS
        // Just a good reference for post-token creation work, inclduign sample API call
        public async Task<ActionResult> willName()
        {
            oauthHelper oauth = new oauthHelper(Request);
            
            //	Make sure the LTI signature is valid
            /***********************************************************/
            if (oauth.verifySignature())
            {
                string user_id = Request.Form.Get("custom_canvas_user_id");
                string course_id = Request.Form.Get("custom_canvas_course_id");

                Uri testUri = Request.Url;
                //Changed these API keys to Mark's
                JObject rval = await userCalls.getUserData("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, user_id);

                //RestSharp.RestResponse ifThisWorks = await userCalls.tempTestForPut();

                

                JArray rval2 = await userCalls.getListQuizzesInCourse("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id);
                string quizId = rval2.First.Next.Next.Value<string>("id");
                JObject rvalQuizReportMake = await userCalls.createQuizReport("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId);
                
                string reportLink = rvalQuizReportMake.Last.Previous.First.Value<string>("url");
                //rvalQuizReportLink = await userCalls.getQuizReportLink(("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId, reportLink);

                //This is the file directory/name for the quiz report .csv file
                string localFileName = "D:\\putShitHere\\"+ quizId + "_data.csv";
                //string remoteFileUrl = rvalQuizReportMake

                //This downloads the generated report file for parsing
                WebClient webClient = new WebClient();
                webClient.DownloadFile(reportLink, localFileName);

                //JObject rvalSub = await userCalls.getQuizSubmissions("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId);

                //JArray rval3 = await userCalls.getListQuestionsInQuiz("9802~XGk8BJGRorTVCHIH4fY3bSgqLQabhXoVi7DkY1aO36kBCA0pupuuhS1hVWiWboko", "https://" + Request.UrlReferrer.Host, course_id, quizId);
                //The quiz submission id needed for getQuizSubmissionQuestions (which gives the 
                JObject rval4 = await userCalls.getQuizSubmissions("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId);

                string submissionId = rval4.First.First.First.Value<string>("id");
                //For the following function execution "36113" is the question id, "100" is the question score and the last param is the comment for the question
                RestSharp.RestResponse finalSubmitTest = await userCalls.putQuizQuestionScoreComment("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId, submissionId, "36113", "100", "Just for funziez");

                JArray getListQuizzesAssignmentsInCourse = await userCalls.getListQuizzesAssignmentsInCourse("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id);
                string quizAssignmentId = getListQuizzesAssignmentsInCourse.Last.Value<string>("id");
                JArray rvalQuizSubmissionThroughAssignments = await userCalls.getQuizzesAssignmentsSubmissions("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizAssignmentId);

                //This will parse the given .csv file into an array of size (Rows x Cols)
                //Reminder: each row (other than the first) corresponds to a student's quiz submission, following the format for collumns of 
                //var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(localFileName);
                //parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                //parser.SetDelimiters(new string[] { "," });
                //List<string[]> dataToParse = new List<string[]>();
                //while (!parser.EndOfData)
                //{
                //    string[] row = parser.ReadFields();
                //    /* do something */
                //    _logger.Error("Here's a row: " + row.ToString());
                //    dataToParse.Add(row);
                //}


                willNameModel model = new willNameModel();
                //string submissionId = rval4.First.First.First.Value<string>("id");

                //string questionId = rval3.First.Value<string>("id");
                //Console.WriteLine("submission ID is " + submissionId);
                //JObject rval5 = await userCalls.getQuizSubmissionQuestions("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, submissionId);

                JArray rval6 = await userCalls.getListQuestionsInQuiz("9802~K2vUBhafkYpSjiA53Cn0dzdTbQxc9mw5QMauku6eUFxhWXSqcoUfIeaBeqjfxSOy", "https://" + Request.UrlReferrer.Host, course_id, quizId);

                


                model.name = "SUCCESS!  Data.csv has been downloaded to the D: drive at location " + localFileName;
                model.id = jsonHelpers.GetJObjectValue(rval, "name");
                return View(model);
            }
            return View(new willNameModel());
        }

        
    }
}