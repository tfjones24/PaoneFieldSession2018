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
using NLog;

namespace SupaSpeedGrader.API
{
    public class userCalls
    {
        /// <summary>
		/// Implementation of Get user-in-a-course-level participation data
		/// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.student_in_course_participation
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="canvasCourseId"></param>
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
    }
}