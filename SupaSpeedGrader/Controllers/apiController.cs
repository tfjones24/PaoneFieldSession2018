﻿using Newtonsoft.Json.Linq;
using NLog;
using SupaSpeedGrader.API;
using SupaSpeedGrader.Helpers;
using SupaSpeedGrader.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SupaSpeedGrader.Controllers
{
    public class apiController : Controller
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<ActionResult> getQuestions(string quiz, string state)
        {
            _logger.Error("Pulling in a saved state: " + state);
            oauthHelper oauth = Newtonsoft.Json.JsonConvert.DeserializeObject<oauthHelper>(sqlHelper.getStateJson(Guid.Parse(state)));

            oauth.accessToken = sqlHelper.getUserAccessToken(long.Parse(oauth.custom_canvas_user_id));
            _logger.Error("State loaded: " + state);

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
                        return Json(new { Result = "AUTHFAIL" });

                    }
                    _logger.Error("token renewed! state: " + state);

                }

            }
            _logger.Error("Calling for quiz report");

            JObject rvalQuizReportMake = await userCalls.createQuizReport(oauth.accessToken.accessToken, "https://" + oauth.host, oauth.custom_canvas_course_id, quiz);

            bool gotET = false;
            bool mayFailed = true;
            string reportLink = "";
            int count = 0;

            while (!gotET)
            {
                System.Threading.Thread.Sleep(1000);
                count++;
                if (count > 12)
                {
                    gotET = true;
                    mayFailed = true;
                }

                rvalQuizReportMake = await userCalls.createQuizReport(oauth.accessToken.accessToken, "https://" + oauth.host, oauth.custom_canvas_course_id, quiz);
                try
                {
                    reportLink = rvalQuizReportMake.Last.Previous.First.Value<string>("url");
                    gotET = true;
                }
                catch
                {

                }
            }

            if (mayFailed && reportLink == "")
            {
                _logger.Error("Call for quiz report failed in " + count + ": " + reportLink);
                return Json(new { Result = "FAILED" });
            }

            //reportLink = rvalQuizReportMake.Last.Previous.First.Value<string>("url");

            string localFileName = "C:\\qqg_temp_data\\"+quiz+"_report.csv";

            WebClient webClient = new WebClient();
            webClient.DownloadFile(reportLink, localFileName);

            _logger.Error("Call for quiz report returned in "+ count + ": " + reportLink);

            var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(localFileName);
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(new string[] { "," });
            List<string[]> dataToParse = new List<string[]>();
            while (!parser.EndOfData)
            {
                string[] row = parser.ReadFields();
                /* do something */
                _logger.Error("Here's a row: " + row.ToString());
                dataToParse.Add(row);
            }

            _logger.Error("Let's do a parse! Quiz: " + quiz);

            if (dataToParse[0][0] != "name" && dataToParse[0][1] != "id")
            {
                _logger.Error("Call for quiz report failed because: 00=" + dataToParse[0][0] + " 01=" + dataToParse[0][1] + " 10=" + dataToParse[1][0]);
                return Json(new { Result = "FAILED" });
            }

            //TODO: Parse CSV into SQL database
            //SQL idea:
            /*  drop table of quizid_courseid
             *  create table of quizid_courseid
             *  columns are:
             *      questionid
             *      possible score
             *      repeat following for all students:
             *          studentid:respone
             *          studentid:score
             *          studentid:comment
             * 
             *  fill table per column
             * 
             *  functions needed for SQL:
             *      recreate table quizid_courseid
             *      create columns
             *      update studentid submission for questionid
             *      get studentid submission for questionid
             */

            if (dataToParse.Count > 1)
            {
                // Generate a list of student IDs
                List<string> studentIDList = new List<string>();
                for (int x = 1; x < dataToParse.Count; x++)
                {
                    studentIDList.Add(dataToParse[x][1]);
                }

                // Convert to array because something?
                string[] students = studentIDList.ToArray();

                // Create that table!
                bool didcreate = sqlHelper.createQuizTable(quiz, oauth.custom_canvas_course_id, students);

                

                // Okay, table made, let's parse!
                string toReturn = "{ \"questionArray\": [";
                int z = 0;
                // Let's go by questions, across the columns
                for (int x = 7; x < dataToParse[0].Length - 3; x = x + 2)
                {
                    // Grab the question data
                    //TODO: remove : and everything after
                    string question = dataToParse[0][x];
                    string score = dataToParse[0][x + 1];



                    // Remove everything after : including :
                    string questionText = question.Substring(question.IndexOf(":")+1);
                    question = question.Substring(0, question.IndexOf(":"));
                    if (z > 0)
                    {
                        toReturn = toReturn + ", ";
                    }
                    toReturn = toReturn + " {\"name\": \"" + questionText + "\", \"id\": \"" + question + "\"}";
                    z++;

                    // Store our list of students
                    sqlHelper.updateListStudentSQL(quiz, oauth.custom_canvas_course_id, question, students);

                    // Now grab and upload each student reponse
                    for (int y = 1; y < dataToParse.Count; y++)
                    {
                        string studentResponse = dataToParse[y][x];
                        string studentScore = dataToParse[y][x + 1];

                        bool doThatUpdate = sqlHelper.updateStudentSubmissionSQL(quiz, oauth.custom_canvas_course_id, question, questionText, score, students[y - 1], studentScore, studentResponse, "");
                    }
                }

                toReturn = toReturn + "] }";
                // Okay, data is parsed, so lets..pass off a list of questions!
                // Make sure to include id's...

                _logger.Error("We goo! Quiz: " + quiz);



                //TODO: Return a list of questions in order w/ names and ids
                return Json(new { Result = "SUCCESS", Questions = toReturn });
                //return Content(toReturn, "application/json");
            }
            else
            {
                //TODO: No submissions....
                _logger.Error("We goo! Quiz: " + quiz);

                //TODO: Return a list of questions in order w/ names and ids
                return Json(new { Result = "SUCCESS", Questions = new { } });
            }


        }
    }
}
