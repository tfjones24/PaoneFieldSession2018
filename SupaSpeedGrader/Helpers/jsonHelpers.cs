using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupaSpeedGrader.Helpers
{
    public class jsonHelpers
    {
        /// <summary>
		/// Get the value of a specific key in a JObject
		/// </summary>
		/// <param name="yourJArray">Your JObject</param>
		/// <param name="key">Key name to find</param>
		/// <returns>returns a string, empty if the key was not found</returns>
        public static string GetJObjectValue(JObject yourJArray, string key)
        {
            foreach (KeyValuePair<string, JToken> keyValuePair in yourJArray)
            {
                if (key == keyValuePair.Key)
                {
                    return keyValuePair.Value.ToString();
                }
            }
            return String.Empty;
        }
    }
}