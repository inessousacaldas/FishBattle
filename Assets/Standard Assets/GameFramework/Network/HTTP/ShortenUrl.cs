using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LITJson;

public static class ShortenUrl
{
    public const string ShortenFormat = "http://api.t.sina.com.cn/short_url/shorten.json?source=2636646108&url_long={0}";
    public const string ShortKey = "url_short";
    public const string UnknowError = "unkonw error!";

    public static WWW GetTinyUrlWWW(string longUrl)
    {
        return new WWW(string.Format(ShortenFormat, WWW.EscapeURL(longUrl)));
    }


    public static Hashtable GetTinyUrlHashtable(WWW www)
    {
        if (www == null)
        {
            return null;
        }

        return JsonMapper.ToObject<List<Hashtable>>(www.text)[0];
    }


    public static string GetTinyUrlError(Hashtable tiny)
    {
        if (tiny == null)
        {
            return UnknowError;
        }

        return null;
    }


    public static string GetTinyUrl(Hashtable tiny)
    {
        if (tiny == null)
        {
            return null;
        }

        return tiny[ShortKey].ToString();
    }
}
