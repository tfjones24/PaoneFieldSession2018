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
         * Gets a list of all the questions in the given quiz, to be used for question selection.  
         * 
        */
        public static async Task<dynamic> getListQuestionsInQuiz(string accessToken, string baseUrl, string canvasCourseId, string quizId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/quizzes/:quiz_id/questions";

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


    }
}