using System.IO;
using System.Net;
using Qiniu.RPC;

namespace Qiniu.Auth
{
    public class PutAuthClient : Client
    {
        public PutAuthClient(string upToken)
        {
            UpToken = upToken;
        }

        public string UpToken { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        public override void SetAuth(HttpWebRequest request, Stream body)
        {
            string authHead = "UpToken " + UpToken;
            request.Headers.Add("Authorization", authHead);
        }
    }
}