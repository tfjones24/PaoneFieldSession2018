using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SupaSpeedGrader.Helpers;
using NLog;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace SupaSpeedGrader.API
{
    public class userCalls
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
		/// Implementation of Get User profile data
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="canvasUserId"></param>
		/// <returns>returns a dynamic json object, will throw exception</returns>
		public static async Task<dynamic> getUserData(string accessToken, string baseUrl, string canvasUserId)
        {
            dynamic rval = null;
            string urlCommand = "api/v1/users/:id";

            urlCommand = urlCommand.Replace(":id", canvasUserId);


            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;
        }
        /**
         * Gets a list of all the quizes in the given course, to be used for quiz selection.  
         * TODO we need to taransfer the quiz ID that is selected into the next function to get that specific quiz
        */
        public static async Task<dynamic> getListQuizzesInCourse(string accessToken, string baseUrl, string canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes?per_page=150";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        public static async Task<dynamic> getListQuizzesAssignmentsInCourse(string accessToken, string baseUrl, string canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/assignments?per_page=150";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        public static async Task<dynamic> getQuizzesAssignmentsSubmissions(string accessToken, string baseUrl, string canvasCourseId, string assignmentId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/assignments/:assignment_id/submissions?include[]=submission_comments&per_page=150";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":assignment_id", assignmentId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        /**
         * Gets a list of all the questions in the given quiz, to be used for question selection.  
         * 
        */
        public static async Task<dynamic> getListQuestionsInQuiz(string accessToken, string baseUrl, string canvasCourseId, string quizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/questions?per_page=150";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", quizId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        public static async Task<dynamic> createQuizReport(string accessToken, string baseUrl, string canvasCourseId, string quizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/reports?quiz_report[report_type]=student_analysis";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", quizId);

            using (HttpResponseMessage response = await clsHttpMethods.httpPOST(baseUrl, urlCommand, accessToken, null))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        /*
        //I don't think this needs to exist actually TODO
        public static async Task<dynamic> deleteOldQuizReport(string accessToken, string baseUrl, string canvasCourseId, string quizId)
        {
            dynamic rval = null;
            JObject rval2 = await getQuizReports(accessToken, "https://" + baseUrl, canvasCourseId, quizId);
            string reportId = rval2.First.Value<string>("id");

            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/reports/:id";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", quizId);
            urlCommand = urlCommand.Replace(":id", reportId);

            using (HttpResponseMessage response = await clsHttpMethods.httpPOST(baseUrl, urlCommand, accessToken, null))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        
        public static async Task<dynamic> getQuizReports(string accessToken, string baseUrl, string canvasCourseId, string quizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/reports";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", quizId);
            

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        */
        /**
         * Gets a single quiz object, given the courseID and QuizID
         * 
        */
        public static async Task<dynamic> getQuizInCourse(string accessToken, string baseUrl, string canvasCourseId, string canvasQuizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:id";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":id", canvasQuizId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        /**
         * Gets a single quiz question given the CourseID, QuizID, and QuestionID
         * 
        */ 
        /*
        public static async Task<dynamic> getQuizQuestion(string accessToken, string baseUrl, string canvasCourseId, string canvasQuizId, string canvasQuizQuestionId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/questions/:id";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", canvasQuizId);
            urlCommand = urlCommand.Replace(":id", canvasQuizQuestionId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        */
        public static async Task<dynamic> getQuizSubmissions(string accessToken, string baseUrl, string canvasCourseId, string canvasQuizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/submissions?per_page=150";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);
            urlCommand = urlCommand.Replace(":quiz_id", canvasQuizId);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("include[]", "submission");
            //vars.Add("include[]", "quiz");
            //vars.Add("include[]", "user");
            //urlCommand = clsHttpMethods.concatenateHttpVars(urlCommand, vars);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }

        /**
         * Quiz submission IDs and student ids need to be tied somehow.  
         * This function should get the answers for a specific quiz submission, which should be tied to the student.  It will also contain the correct answers depending on the question type
        */
        /*
        public static async Task<dynamic> getQuizSubmissionQuestions(string accessToken, string baseUrl, string canvasQuizSubmissionId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/quiz_submissions/:quiz_submission_id/questions?per_page=150";

            urlCommand = urlCommand.Replace(":quiz_submission_id", canvasQuizSubmissionId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        */

        /**
         * Gets list of all sections in the current class
         */ 
        public static async Task<dynamic> getSectionList(string accessToken, string baseUrl, string canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/sections";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId);

            using (HttpResponseMessage response = await clsHttpMethods.httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }

            return rval;

        }
        




        //FUNCTION FOR POSTING QUESTION COMMENTS AND GRADES
        //NOTE: THIS WILL ONLY WORK FOR SINGLE SUBMISSION/ATTEMPT QUIZZES
        //This is because the attempt number must be passed back as part of the JSON object
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task<dynamic> putQuizQuestionScoreComment(string accessToken, string baseUrl, string courseID, string quizID, string submissionID, string questionID, string questionScore, string questionComment)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore IDE1006 // Naming Styles
        {
            //IF this fails, check https://community.canvaslms.com/thread/6062 for some massively important JSON context information, especially regarding required parameters
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/submissions/:id";
            string jsonData = "{\"quiz_submissions\":[{\"attempt\": 1,\"questions\": {\"" + questionID + "\": {\"score\": " + questionScore + ",\"comment\": " + JsonConvert.ToString(questionComment) + "}}}]}";

            urlCommand = urlCommand.Replace(":course_id", courseID);
            urlCommand = urlCommand.Replace(":quiz_id", quizID);
            urlCommand = urlCommand.Replace(":id", submissionID);

            var client = new RestClient(baseUrl+urlCommand);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return response;
        }

        

        /* OAuth Functions */
        /// <summary>
        /// Call this method to either genreate a new access token, or refresh an existing access token,
        /// to all you to make API calls using the identity of the user who is logged into canvas
        /// </summary>
        /// <param name="oauth">our [state] object containing our launch parameters</param>
        /// <param name="oauth2ResponseUrl">this url must match the url defined in your Developer Key</param>
        /// <param name="grantType">used to ask canvsa to generate a new access token, or refresh an existing access token</param>
        /// <param name="code">the code returned by canvas during the authorize redirect</param>
        /// <param name="refreshToken">if performing a refresh, this is the refreshToken value</param>
        /// <returns></returns>
        public static async Task<bool> requestUserToken(oauthHelper oauth, string oauth2ResponseUrl, string grantType, string code = null, string refreshToken = null)
        {
            bool rval = true;
            string json = string.Empty;

            string oauth2ClientId = (System.Configuration.ConfigurationManager.AppSettings["oauth2ClientId"] != null) ? System.Configuration.ConfigurationManager.AppSettings["oauth2ClientId"] : string.Empty;
            string oauth2ClientKey = (System.Configuration.ConfigurationManager.AppSettings["oauth2ClientKey"] != null) ? System.Configuration.ConfigurationManager.AppSettings["oauth2ClientKey"] : string.Empty;


            // API Call to get OAuth2 token for user:
            // https://canvas.instructure.com/doc/api/file.oauth_endpoints.html#post-login-oauth2-token
            string urlCommand = "/login/oauth2/token";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // custom_canvas_api_domain was given to us in the lti launch parameters, and we stored those parameters in our [state], 
                    // which was deserialized into our oauthHelper object
                    // now we can use it to make the api call to the correct instance of canvas
                    client.BaseAddress = new Uri("https://" + oauth.custom_canvas_api_domain);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // now we need to authenticate to Canvas, this is where your Developer Key comes into play
                    Dictionary<string, string> vars = new Dictionary<string, string>();
                    vars.Add("grant_type", grantType);              //value will be either 'authorization_code' or 'refresh_token'
                    vars.Add("client_id", oauth2ClientId);          //this is the developer key "ID" value
                    vars.Add("client_secret", oauth2ClientKey);     //this is the developer key "Key" value
                    vars.Add("redirect_uri", oauth2ResponseUrl);    //this is the developer key "URI" value

                    if (!string.IsNullOrEmpty(code)) vars.Add("code", code);  //if canvas gave us a code, send it
                    if (!string.IsNullOrEmpty(refreshToken)) vars.Add("refresh_token", refreshToken);  //if we defined a refresh token, send it

                    string varString = Newtonsoft.Json.JsonConvert.SerializeObject(vars);
                    HttpContent httpContent = new FormUrlEncodedContent(vars.ToArray());
                    _logger.Error("Call API[" + urlCommand + "]: " + varString);

                    using (HttpResponseMessage response = await client.PostAsync(urlCommand, httpContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            // excellent, canvsa returned a success code, we should have also received token data, let's store the token data
                            json = await response.Content.ReadAsStringAsync();
                            _logger.Error("API Resonse: [" + json + "]");
                            oauth.accessToken = new userAccessToken(json, oauth2ResponseUrl);
                        }
                        else
                        {
                            // uh-oh, canvas returned an error code, let's log it for forensics
                            _logger.Error("[REQUESTUSERTOKEN] Canvas API call failed: code[" + response.StatusCode + "] reason[" + response.ReasonPhrase + "]");
                            _logger.Error("     response object: " + Newtonsoft.Json.JsonConvert.SerializeObject(response));
                            rval = false;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                //hopefully we never end up here, log this exception for forensics
                _logger.Error(err, "Request for user token failed");
                rval = false;
            }

            return rval;
        }


    }
}