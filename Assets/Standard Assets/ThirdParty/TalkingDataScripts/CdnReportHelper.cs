// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : CdnReportHelper.cs
// Author   : senkay <senkay@126.com>
// Created  : 7/30/2016 
// Porpuse  : 
// **********************************************************************
//
using System;
using System.Text.RegularExpressions;
using System.Net;
using UnityEngine;
using System.Collections.Generic;

public class CdnReportHelper
{
	//http://localdev84.h1.cilugame.com/h1/spreadc/cdn/report.json?nodeIp=192.168.1.1:192.168.1.2&requestUrl=123&msg=xxxx
	private static string reportUrl;
	
	public static void Setup(string url)
	{
		reportUrl = url;
	}
	
	public static void Report(string url, string msg = "")
	{
		string error = string.Format("Load url:{0} error:{1}", url, msg);
		Debug.Log(error);
		
		if (string.IsNullOrEmpty(reportUrl))
		{
			return;
		}
		
		//如果自身请求失败， 则忽略上报记录
		if (url.Contains(reportUrl))
		{
			return;
		}
		
		List<string> ipList = new List<string>();
		
		try
		{
            string hostName = StringHelper.GetHostFromUrl(url);
            IPHostEntry host = Dns.GetHostEntry(hostName);
			for (int i=0; i<host.AddressList.Length; i++)
			{
				IPAddress ip = host.AddressList[i];
				ipList.Add(ip.ToString());
			}
		}
		catch(Exception e)
		{
			Debug.Log("CdnReportHelper GetHostByNameException=" + e.ToString());
		}
		
		string ips = string.Join(",", ipList.ToArray());
		
		string requestUrl = string.Format(reportUrl + "?nodeIp={0}&requestUrl={1}&msg={2}", ips, WWW.EscapeURL(url), WWW.EscapeURL(msg));
		
		Debug.Log("CdnReportHelper requestUrl=" + requestUrl);
		
		HttpController.Instance.DownLoad(requestUrl, delegate(ByteArray byteArray)
		                                 {
			string str = byteArray.ToEncodingString();
			Debug.Log("CdnReportHelper " + str);
		}, null, delegate
		{
			//GameDebuger.Log(obj.ToString());
		}, false, SimpleWWW.ConnectionType.Short_Connect);
	}
}

