using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SupaSpeedGrader.Controllers;

namespace SupaSpeedGrader.Helpers
{
    // This helper was coiped in for the purpose of simplifying the SQL database management.
	public class sqlHelper
	{
        //Retrieve from Web.config the SQL server connection string, including address and auth method
		private static string _camsConnectionString = (HomeController.devMode) ? System.Configuration.ConfigurationManager.ConnectionStrings["devMode"].ToString() : System.Configuration.ConfigurationManager.ConnectionStrings["oauth2"].ToString();


        /*  The following methods are all related to properly using the OAuth workflow
         *  Most of this is provided by Garth Egbert and his wonderful tutorial here: https://community.canvaslms.com/groups/canvas-developers/blog/2017/04/04/net-oauth2-example
         *  Comments are my own -William Brown
         */

        /// <summary>
		/// Function to retrieve a stored state JSON, provided a state id of type Guid
		/// </summary>
        public static string getStateJson(Guid stateId)
		{
			string rval = string.Empty;

			string sql = "select statejson from statecache where stateId = '" + stateId.ToString() + "'";
			using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
			{
				dbcon.Open();
				using (SqlCommand cmd = new SqlCommand("dbo.getStateJson", dbcon))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sql;

					DataSet ds = new DataSet();
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					adapter.Fill(ds);

					if (ds != null && ds.Tables[0].Rows.Count == 1)
					{
						rval = ds.Tables[0].Rows[0][0].ToString();
					}
				}
			}

			return rval;
		}

        /// <summary>
		/// Function to store a state JSON, provided a state id
		/// </summary>
		public static bool storeState(Guid stateId, string json)
		{
			bool rval = true;

			string sql = string.Empty;
			string stateJson = getStateJson(stateId);
			using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
			{
				dbcon.Open();
				using (SqlCommand cmd = new SqlCommand("dbo.storeStateJson", dbcon))
				{
					cmd.CommandType = CommandType.Text;

					if(string.IsNullOrEmpty(stateJson))
					{
						sql = string.Format("insert into statecache (stateid, statejson) values ('{0}', '{1}')", stateId.ToString(), json);
					}
					else
					{
						sql = string.Format("update statecache set statejson = '{0}' where stateid = '{1}'", json, stateId.ToString());
					}
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}

			return rval;
		}

        /// <summary>
		/// Function to retrieve a stored state JSON, provided a state id already as a string
		/// </summary>
        public static string getStateJson(string stateId)
        {
            string rval = string.Empty;

            string sql = "select statejson from statecache where stateId = '" + stateId + "'";
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getStateJson", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    if (ds != null && ds.Tables[0].Rows.Count == 1)
                    {
                        rval = ds.Tables[0].Rows[0][0].ToString();
                    }
                    
                }
            }

            return rval;
        }

        /// <summary>
		/// Function to retrieve a stored state JSON, sorta, provided a state id in string form
        /// This is used mostly for debugging, try to avoid using it
		/// </summary>
        public static string getStateJsonQuery(string stateId)
        {
            string rval = string.Empty;

            string sql = "select statejson from statecache where stateId = '" + stateId + "'";
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getStateJson", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    rval = ds.ToString();


                }
            }

            return rval;
        }

        /// <summary>
		/// Function to store a state JSON, provided a state id of type string
		/// </summary>
        public static bool storeState(string stateId, string json)
        {
            bool rval = true;

            string sql = string.Empty;
            string stateJson = getStateJson(stateId);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.storeStateJson", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    if (string.IsNullOrEmpty(stateJson))
                    {
                        sql = string.Format("insert into statecache (stateid, statejson) values ('{0}', '{1}')", stateId, json);
                    }
                    else
                    {
                        sql = string.Format("update statecache set statejson = '{0}' where stateid = '{1}'", json, stateId);
                    }
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        /// <summary>
		/// Function to retrieve a stored access token, provided a user id
		/// </summary>
        public static userAccessToken getUserAccessToken(long userId)
		{
			userAccessToken rval = null;

			string sql = "select * from accessTokenCache where userid = '" + userId.ToString() + "'";
			using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
			{
				dbcon.Open();
				using (SqlCommand cmd = new SqlCommand("dbo.getUserAccessToken", dbcon))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sql;

					DataSet ds = new DataSet();
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					adapter.Fill(ds);

					if (ds != null && ds.Tables[0].Rows.Count == 1)
					{
						//we found an existing token, return the token helper object
						rval = new userAccessToken(ds.Tables[0].Rows[0]);
					}
				}
			}

			return rval;
		}

        /// <summary>
		/// Function to store an access token. The user id is part of the userAccessToken object
		/// </summary>
		public static bool storeUserAccessToken(userAccessToken accessToken)
		{
			bool rval = true;

			string sql = string.Empty;
			userAccessToken cacheToken = getUserAccessToken(accessToken.userId);
			using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
			{
				dbcon.Open();
				using (SqlCommand cmd = new SqlCommand("dbo.storeUserAccessToken", dbcon))
				{
					cmd.CommandType = CommandType.Text;

					if (cacheToken == null)
					{
						sql = string.Format("insert into accesstokencache (userid, accesstoken, refreshtoken, tokenlife, tstamp, responseUrl) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", accessToken.userId, accessToken.accessToken, accessToken.refreshToken, accessToken.tokenLife, accessToken.tstamp.ToString(), accessToken.responseUrl);
					}
					else
					{
						sql = string.Format("update accesstokencache set accesstoken='{0}', tokenlife='{1}', tstamp='{2}', responseUrl='{4}' where userid = '{3}'", accessToken.accessToken, accessToken.tokenLife, accessToken.tstamp.ToString(), accessToken.userId, accessToken.responseUrl);
					}
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}

			return rval;
		}



        /// <summary>
        /// Function to delete a stored access token, provided a user id
        /// </summary>
        public static bool deleteUserAccessToken(string userId)
		{
			bool rval = true;

			string sql = string.Empty;
			using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
			{
				dbcon.Open();
				using (SqlCommand cmd = new SqlCommand("dbo.deleteUserAccessToken", dbcon))
				{
					cmd.CommandType = CommandType.Text;
					sql = string.Format("delete accesstokencache where userid = '" + userId + "'");
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}

			return rval;
        }



        /*  The following functions act as our "SQL API" to store and retrieve
         *  parsed submission information taken from quiz reports generated
         *  by the Canvas API. While the basic code structure is followed 
         *  from the above calls, all queries and stored information was 
         *  created by yours truly. -William Brown
         */
        //TODO: Create functions to implement plan in apicontroller

        /// <summary>
		/// Function to (re)create a table to store quiz submissions obtained from 
        /// the reports API. Tables are named quizID_courseID, and all columns are 
        /// of type string primarily for flexibility. studentIDs should be passed 
        /// as a string array, with each entry being a single student.
        /// 
        /// Column formatting is as follows:
        ///     questionID      (autogenerated, do not include)
        ///     possible score  (autogenerated, do not include)
        ///     following repeat for all students, replacing studentID with numerical ID per student:
        ///         studentID:response
        ///         studentID:score
        ///         studentID:comment
        /// </summary>
        public static bool createQuizTable(string quizID, string courseID, string[] studentIDs)
        {
            bool rval = true;

            string sql = string.Empty;

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.deleteQuizTable", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    sql = string.Format("drop table if exists quiz{0}_{1}", quizID, courseID);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand("dbo.createQuizTable", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    sql = string.Format("create table quiz{0}_{1}( questionID integer PRIMARY KEY, questionText text, maxScore text, studentList text", quizID, courseID);

                    for (int i = 0; i < studentIDs.Length; i++)
                    {
                        sql = sql + string.Format(", response_{0} text, score_{0} text, comment_{0} text, submission_{0} text", studentIDs[i]);
                    }
                    sql = sql + " )";

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        public static string getSubmissionID(string quiz, string course, string question, string student)
        {
            string rval = null;

            string sql = string.Format("select submission_{3} from quiz{0}_{1} where questionID = {2}", quiz, course, question, student);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getStudentList", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 1)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[0].ToString()))
                        {
                            // Oh it does! Quick return it!

                            return ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval =  "";
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static bool updateStudentSubmissionSQL(string quizID, string courseID, string questionID, string questionText, string maxScore, string studentID, string studentScore, string studentResponse, string comment)
        {
            bool rval = true;

            string sql = string.Empty;
            string[] studentSub = getStudentSubmissionSQL(quizID, courseID, questionID, studentID);

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.updateStudentSubmission", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    if (studentSub == null)
                    {
                        sql = string.Format("insert into quiz{0}_{1} (questionID, maxScore, response_{2}, score_{2}, comment_{2}, questionText) values ('{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", quizID, courseID, studentID, questionID, maxScore, studentResponse, studentScore, comment, questionText);
                    }
                    else
                    {
                        sql = string.Format("update quiz{0}_{1} set response_{2}='{3}', score_{2}='{4}', comment_{2}='{5}', maxScore='{7}', questionText='{8}' where questionID = '{6}'", quizID, courseID, studentID, studentResponse, studentScore, comment, questionID, maxScore, questionText);
                    }
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        public static bool updateStudentSubmissionSQL(string quizID, string courseID, string questionID, string studentID, string studentScore, string comment)
        {
            bool rval = true;

            string sql = string.Empty;

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.updateStudentSubmission", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    sql = string.Format("update quiz{0}_{1} set score_{2}='{3}', comment_{2}='{4}' where questionID = '{5}'", quizID, courseID, studentID, studentScore, comment, questionID);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        public static bool updateListStudentSQL(string quizID, string courseID, string questionID, string[] students)
        {
            bool rval = true;

            string sql = string.Empty;
            string[] studentSub = getStudentSubmissionSQL(quizID, courseID, questionID, students[0]);

            //TODO: convert students to studentJSON or something
            string studentsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(students);

            //string[] test = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(studentsJSON);

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.updateStudentList", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    if (studentSub == null)
                    {
                        sql = string.Format("insert into quiz{0}_{1} (questionID, studentList) values ('{2}', '{3}')", quizID, courseID, questionID, studentsJSON);
                    }
                    else
                    {
                        sql = string.Format("update quiz{0}_{1} set studentList='{2}' where questionID = '{3}'", quizID, courseID, studentsJSON, questionID);
                    }
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        public static string[] getStudentListSQL(string quizID, string courseID, string questionID)
        {
            string[] rval = null;

            string sql = string.Format("select studentList from quiz{0}_{1} where questionID = {2}", quizID, courseID, questionID);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getStudentList", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 1)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[0].ToString()))
                        {
                            // Oh it does! Quick return it!

                            return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(ds.Tables[0].Rows[0].ItemArray[0].ToString());
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval = new string[] { "" };
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static string[] getStudentSubmissionSQL(string quizID, string courseID, string questionID, string studentID)
        {
            string[] rval = null;

            string sql = string.Format("select response_{3},  score_{3}, comment_{3} from quiz{0}_{1} where questionID = {2}", quizID, courseID, questionID, studentID);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getStudentSubmission", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 3)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[1].ToString()))
                        {
                            // Oh it does! Quick return it!
                            return new string[] { ds.Tables[0].Rows[0].ItemArray[0].ToString(), ds.Tables[0].Rows[0].ItemArray[1].ToString(), ds.Tables[0].Rows[0].ItemArray[2].ToString() };
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval = new string[] { "", "", "" };
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static string getQuestionMaxScore(string quizID, string courseID, string questionID)
        {
            string rval = null;

            string sql = string.Format("select maxScore from quiz{0}_{1} where questionID = {2}", quizID, courseID, questionID);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getQuestionMaxScore", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 1)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[0].ToString()))
                        {
                            // Oh it does! Quick return it!
                            return ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval = "";
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static string getQuestionName(string quizID, string courseID, string questionID)
        {
            string rval = null;

            string sql = string.Format("select questionText from quiz{0}_{1} where questionID = {2}", quizID, courseID, questionID);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getQuestionText", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 1)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[0].ToString()))
                        {
                            // Oh it does! Quick return it!
                            return ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval = "";
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static string getNumberQuestions(string quizID, string courseID)
        {
            string rval = null;

            string sql = string.Format("SELECT COUNT(*) FROM quiz{0}_{1}", quizID, courseID);
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getCountQuestions", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    // Check to see if the row exists at all
                    if (ds != null && ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0].ItemArray.Length == 1)
                    {
                        // Oh it does! Wait, does this submission exist?
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0].ItemArray[0].ToString()))
                        {
                            // Oh it does! Quick return it!
                            return ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                        // Fuck, it doesn't :( return an empty array tho, like our dreams and our database
                        rval = "";
                        //we found an existing token, return the shit
                    }
                }
            }

            return rval;
        }

        public static bool updateStudentSubmissionID(string quiz, string course, string question, string student, string submissionID)
        {
            bool rval = true;

            string sql = string.Empty;

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.updateStudentList", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    sql = string.Format("update quiz{0}_{1} set submission_{2}='{3}' where questionID = '{4}'", quiz, course, student, submissionID, question);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }



        //The following functions store and retrieve rubrics from their own table

        /// <summary>
        /// Function to store a rubric name and JSON, provided an id
        /// </summary>
        public static bool storeRubric(string id, string name, string json, string questionCount)
        {
            bool rval = true;

            string sql = string.Empty;
            string rubricJson = getRubricJson(id);

            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.storeRubricJson", dbcon))
                {
                    cmd.CommandType = CommandType.Text;

                    if (string.IsNullOrEmpty(rubricJson))
                    {
                        sql = string.Format("insert into rubrics (id, name, json) values ({2}, '{0}', '{1}')", name, json, id);
                    }
                    else
                    {
                        sql = string.Format("update rubrics set json = '{0}', name = '{1}' where id = {2}", json, name, id);
                    }
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            return rval;
        }

        /// <summary>
		/// Function to retrieve a stored rubric JSON, provided an id 
        public static string getRubricJson(string id)
        {
            string rval = string.Empty;

            string sql = "select json from rubrics where id = " + id;
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getRubricJson", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    if (ds != null && ds.Tables[0].Rows.Count == 1)
                    {
                        rval = ds.Tables[0].Rows[0][0].ToString();
                    }

                }
            }

            return rval;
        }

        /// <summary>
		/// Function to retrieve a stored rubric name, provided an id 
		/// </summary>
        public static string getRubricName(string id)
        {
            string rval = string.Empty;

            string sql = "select name from rubrics where id = " + id;
            using (SqlConnection dbcon = new SqlConnection(_camsConnectionString))
            {
                dbcon.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.getRubricName", dbcon))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                    if (ds != null && ds.Tables[0].Rows.Count == 1)
                    {
                        rval = ds.Tables[0].Rows[0][0].ToString();
                    }

                }
            }

            return rval;
        }
    }
}