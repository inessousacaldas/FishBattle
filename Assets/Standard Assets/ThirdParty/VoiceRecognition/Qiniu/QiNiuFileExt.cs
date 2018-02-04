using System;
using UnityEngine;
using Qiniu.Auth.digest;
using Qiniu.Conf;
using Qiniu.IO;
using Qiniu.RS;
using Qiniu.Util;

public static class QiNiuFileExt
{
	public static string NewKey
	{
		get
		{
			string key = Guid.NewGuid().ToString();
			//if (string.IsNullOrEmpty(key))
			//{
			//    //GameDebuger.Log("SaveVoiceToQiniu return key = " + _voiceKey);
			//    key = PlayerModel.Instance.GetPlayerId() + "-" + UnityEngine.Random.Range(10000, 99999);
			//}
			return key;
		}
	}


	public static string PutFileBuf(byte[] buf, int bufSize, string scope, string key = null, bool overwrite = true, string mimeType = null, Action<bool, string> OnPutFinished = null, int crc32 = 123)
	{
		var target = new IOClient();
		if (string.IsNullOrEmpty(key))
			key = NewKey;

		var extra = new PutExtra(); // TODO: 初始化为适当的值
		if (!string.IsNullOrEmpty(mimeType))
		{
			extra.MimeType = mimeType;
		}
		if (crc32 != 0)
		{
			extra.Crc32 = crc32;
			extra.CheckCrc = CheckCrcType.CHECK;
		}

		target.PutFinished += (o, e) =>
		{
			if (e.OK)
			{
				//RSHelper.RSDel(Bucket, key);
				Debug.Log("send: Hello, Qiniu Cloud!");
			}

			if (OnPutFinished != null)
				OnPutFinished(e.OK, key);
		};

		var token = "";
		if (!string.IsNullOrEmpty(Config.ACCESS_KEY) && !string.IsNullOrEmpty(Config.SECRET_KEY))
		{
			if (overwrite)
			{
				scope = string.Format("{0}:{1}", scope, key);
			}
			var put = new PutPolicy(scope);
			token = put.Token();
		}
#pragma warning disable 0168
        var ret = target.Put(token, key, StreamEx.ToStream(buf, bufSize), extra);
#pragma warning restore
        return key;
	}


	public static string GetFileUrl(string domain, string key, string accessKey = null, string secretKey = null)
	{
        if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(key))
            return "";
		var baseUrl = GetPolicy.MakeBaseUrl(domain, key);
		Mac mac = null;
		if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
		{
			mac = new Mac(accessKey, Config.Encoding.GetBytes(secretKey));
		}
		var actualUrl = GetPolicy.MakeRequest(baseUrl, 3600, mac);

		return actualUrl;
	}
}
