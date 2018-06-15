using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace SupaSpeedGrader.Helpers
{
	/// <summary>
	/// Use thie helper class to simplify serialization/deserialization of the token data
	/// </summary>
	public class userAccessToken
	{
		public long userId = 0;
		public string accessToken = string.Empty;
		public string refreshToken = string.Empty;
		public int tokenLife = 0;
		public DateTime tstamp = DateTime.Now;
		public string responseUrl = string.Empty;

        public bool staticKey = false;

		/// <summary>
		/// Default constructor
		/// </summary>
		public userAccessToken()
		{
		}

		/// <summary>
		/// Constructor to handle DataRow from cache db
		/// </summary>
		/// <param name="dr">DataRow from the cache database</param>
		public userAccessToken(DataRow dr)
		{
			long.TryParse(dr["userId"].ToString(), out userId);
			accessToken = dr["accessToken"].ToString();
			refreshToken = dr["refreshToken"].ToString();
			int.TryParse(dr["tokenLife"].ToString(), out tokenLife);
			DateTime.TryParse(dr["tstamp"].ToString(), out tstamp);
			responseUrl = dr["responseUrl"].ToString();
		}

		/// <summary>
		/// Process the json blob returned by Canvas to retrieve the user token values
		/// </summary>
		/// <param name="json"></param>
		public userAccessToken(string json, string responseUrl)
		{
			dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
			if (obj.user != null && obj.user.id != null) long.TryParse(obj.user.id.ToString(), out userId);
			if (obj.access_token != null) accessToken = obj.access_token.ToString();
			if (obj.refresh_token != null) refreshToken = obj.refresh_token.ToString();
			if (obj.expires_in != null) int.TryParse(obj.expires_in.ToString(), out tokenLife);
			tstamp = DateTime.Now;
			this.responseUrl = responseUrl;
			sqlHelper.storeUserAccessToken(this);
		}

        /// <summary>
		/// Create a hardcoded access token
		/// </summary>
		/// <param name="token">Access key generated in Canvas</param>
		public userAccessToken(string token, long userID, int tokenLife)
        {
            accessToken = token;
            userId = userID;
            this.tokenLife = tokenLife;

            staticKey = true;

            sqlHelper.storeUserAccessToken(this);
        }

        /// <summary>
        /// Our Canvas token is only good for [tokenLive] seconds
        /// Calculate the time since we last refreshed.
        /// </summary>
        public bool tokenRefreshRequired
		{
			get
			{
				bool rval = false;

                if (staticKey == true)
                    return false;

				if (refreshToken == null)
					return true;

				if (tstamp < DateTime.Now.AddSeconds(-1 * tokenLife))
					return true;

				return rval;
			}
		}

	}
}