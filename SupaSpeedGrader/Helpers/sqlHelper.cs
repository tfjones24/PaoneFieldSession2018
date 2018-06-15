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
		private static string _camsConnectionString = (HomeController.devMode) ? System.Configuration.ConfigurationManager.ConnectionStrings["devMode"].ToString() : System.Configuration.ConfigurationManager.ConnectionStrings["oauth2"].ToString();

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

	}
}