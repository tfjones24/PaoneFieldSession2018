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
        /**
         * Gets a list of all the quizes in the given course, to be used for quiz selection.  
         * TODO we need to taransfer the quiz ID that is selected into the next function to get that specific quiz
        */
        public static async Task<dynamic> getListQuizzesInCourse(string accessToken, string baseUrl, string canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes";

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

        /**
         * Quiz submission IDs and student ids need to be tied somehow.  
         * This function should get the answers for a specific quiz submission, which should be tied to the student.  It will also contain the correct answers depending on the question type
        */
        public static async Task<dynamic> getQuizSubmissionQuestions(string accessToken, string baseUrl, string canvasQuizSubmissionId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/quiz_submissions/:quiz_submission_id/questions";

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

    }
}