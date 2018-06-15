using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using LtiLibrary.Core;
using NLog;

namespace SupaSpeedGrader.Helpers
{
	public class oauthHelper : ltiLaunchParams
	{
		//NLog
		protected static Logger _logger = LogManager.GetCurrentClassLogger();

		public string _clientUrl = string.Empty;
		public string _consumerSecret = string.Empty;
		private bool _signatureVerified = false;

        public userAccessToken accessToken = null;

        //public HttpRequestBase Request = null;

        public Uri Url = null;
        public string host = null;

        public string uniqueSessionKey
		{
			get
			{
				return this.user_id + "." + this.custom_canvas_api_domain + "." + this.custom_canvas_course_id;
			}
		}

		public bool allowLtiLaunch
		{
			get
			{
				bool rval = false;
				if (!string.IsNullOrEmpty(this.roles))
				{
					string myRoles = this.roles.ToUpper();
					rval = ((myRoles.Contains("FACULTY") || myRoles.Contains("INSTRUCTOR") || myRoles.Contains("ADMINISTRATOR")));
				}
				return rval;
			}
		}

		public oauthHelper()
			: base()
		{

		}

		public oauthHelper(HttpRequestBase request)
			: base(request.Form)
		{
            //Request = request;
            Url = request.Url;
            if (request.UrlReferrer != null)
            {
                host = request.UrlReferrer.Host;
            }

       
			_clientUrl = request.UserHostAddress;

			// if the config doesn't contain our consumer secret, make sure the signature fails
			_consumerSecret = (ConfigurationManager.AppSettings["consumerSecret"] != null) ? ConfigurationManager.AppSettings["consumerSecret"] : Guid.NewGuid().ToString();

			_signatureVerified = (LtiLibrary.Core.OAuth.OAuthUtility.GenerateSignature(request.HttpMethod, request.Url, request.Form, _consumerSecret) == this.oauth_signature);
		}

		public bool verifyNonce()
		{
			bool rval = false;

			if (string.IsNullOrEmpty(oauth_nonce))
				rval = false;
			else
			{
				/*************************************************************
				 * 
				 * It will be up to you to determine how you want to validate your nonce
				 * 
				 * 
				 * **********************************************************/
				//use memcached to verify the nonce is unique
				//try
				//{
				//	if (_bypassElastiCache) return true;

				//	clsMemCached mem = new clsMemCached();
				//	string key = this.oauth_consumer_key + ":" + this.oauth_nonce;
				//	_logger.Debug("memCached Key: " + key);
				//	if (mem.getKeyValue(key) == null)
				//	{
				//		mem.setKeyValue(key, "1");
				//		rval = true;
				//	}
				//	else
				//	{
				//		rval = false;
				//	}
				//}
				//catch (Exception err)
				//{
				//	_logger.Error(err, "[EXCEPTION] verifyNonce() failed");
				//}

				/*************************************************************
				 * 
				 * For the purpose of demo and testing we will always return true
				 * 
				 * **********************************************************/
				rval = true;
			}

			return rval;
		}

		public bool verifyTimestamp()
		{
			bool rval = false;

			if (this.oauth_timestamp != string.Empty)
			{
				try
				{
					DateTime tstamp = new DateTime(1970, 1, 1, 0, 0, 0);
					tstamp = tstamp.AddSeconds(int.Parse(this.oauth_timestamp)).ToLocalTime();
					rval = ((DateTime.Now - tstamp).TotalMinutes < 5);
				}
				catch (Exception err)
				{
					_logger.Error(err, "[EXCEPTION] verifyTimeStamp(): ");
				}
			}

			return rval;
		}

		public bool verifySignature()
		{
			return _signatureVerified;
		}
	}
}