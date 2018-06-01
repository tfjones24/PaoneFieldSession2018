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

namespace SupaSpeedGrader.API
{
    public class userCalls
    {
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

        /// <summary>
		/// Implementation of update quiz scores
		/// Pass in a dictionary of parameters, use parameters names defined in API documnentation
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="courseId"></param>
        /// <param name="quizId"></param>
        /// <param name="studentId"></param>
		/// <param name="vars">List of each API parameter and associated value</param>
		/// <returns>returns a json object representing a quiz, will throw an exception</returns>
		public static async Task<dynamic> putQuizScore(string accessToken, string baseUrl, string courseID, string quizID, string studentID, Dictionary<string, string> vars)
        {
            //IF this fails, check https://community.canvaslms.com/thread/6062 for some massively important JSON context information, especially regarding required parameters
            dynamic result = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/submissions/:id";

            urlCommand = urlCommand.Replace(":course_id", courseID);
            urlCommand = urlCommand.Replace(":quiz_id", quizID);
            urlCommand = urlCommand.Replace(":id", studentID);

            urlCommand = clsHttpMethods.concatenateHttpVars(urlCommand, vars);

            using (HttpResponseMessage response = await clsHttpMethods.httpPUT(baseUrl, urlCommand, accessToken, null))
            {
                string rval = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
                {
                    throw new HttpRequestException(rval);
                }
                result = JsonConvert.DeserializeObject(rval);
            }

            return result;
        }


    }
}