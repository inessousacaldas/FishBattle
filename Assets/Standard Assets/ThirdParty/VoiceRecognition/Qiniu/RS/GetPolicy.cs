using System;
using Qiniu.Auth.digest;
using Qiniu.Conf;

namespace Qiniu.RS
{
    /// <summary>
    ///     GetPolicy
    /// </summary>
    public class GetPolicy
    {
        public static string MakeRequest(string baseUrl, uint expires = 3600, Mac mac = null)
        {
            if (mac == null)
            {
                mac = new Mac(Config.ACCESS_KEY, Config.Encoding.GetBytes(Config.SECRET_KEY));
            }

            uint deadline = (uint) ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000)/10000000 + expires);
            if (baseUrl.Contains("?"))
            {
                baseUrl += "&e=";
            }
            else
            {
                baseUrl += "?e=";
            }
            baseUrl += deadline;
            string token = mac.Sign(Config.Encoding.GetBytes(baseUrl));
            return string.Format("{0}&token={1}", baseUrl, token);
        }

        public static string MakeRequest(string baseUrl, uint expires, string token)
        {
            uint deadline = (uint) ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000)/10000000 + expires);
            if (baseUrl.Contains("?"))
            {
                baseUrl += "&e=";
            }
            else
            {
                baseUrl += "?e=";
            }
            baseUrl += deadline;
            return string.Format("{0}&token={1}", baseUrl, token);
        }

        public static string MakeBaseUrl(string domain, string key)
        {
            key = Uri.EscapeUriString(key);
            return string.Format("http://{0}/{1}", domain, key);
        }
    }
}