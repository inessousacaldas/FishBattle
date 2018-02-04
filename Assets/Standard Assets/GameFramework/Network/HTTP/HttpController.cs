// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  HttpController.cs
// Author   : wenlin
// Created  : 2013/6/8 16:11:18
// Purpose  : 
// **********************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using LITJson;
using System.Text.RegularExpressions;

/// <summary>
/// HttpController 尽量在子线程进行调用
/// </summary>

public class HttpController
{
    private static HttpController _instance = null;
    public static HttpController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HttpController();
            }
            return _instance;
        }
    }

    private HttpController() {}

    //HTTP 获取方式
    private static string HTTP_GET_METHOD  = "Get";
    private static string HTTP_POST_METHOD = "Post"; 


    //Http 控制
    private SimpleWWW _httpController = null;

	//host映射表，对应IP
	private Dictionary<string, string> _HostMaps;

    public void Setup()
    {
        GameObject go = new GameObject();
        GameObject.DontDestroyOnLoad(go);
		go.name = "SimpleWWW";

        _httpController = go.AddComponent<SimpleWWW>();
		_HostMaps = new Dictionary<string, string>();
    }

	/// <summary>
	/// HTTP 下载资源（ 非主线程版本， 可以在任意线程调用 ）
	/// </summary>
	/// <param name="url"> URL地址 </param>
	/// <param name="httpCallBack"> 下载后的回调 </param>
	/// <param name="isNeedUncompress"> 是否需要解压操作</param>
	public bool DownLoad(string uri,
		System.Action<ByteArray> downLoadFinishCallBack,
		System.Action<float> progressCallBack = null,
		System.Action<Exception> errorCallBack = null,
		bool isNeedUncompress = false,
		SimpleWWW.ConnectionType type = SimpleWWW.ConnectionType.Continued_Connect,
        Hashtable headers = null
	)
	{
		if (_httpController == null)
		{
			Debug.Log("Http Controller is NULL ");
			return false;
		}

		//这里先处理host的解析判断

		//获取uri中的host
		string host = StringHelper.GetHostFromUrl(uri);
		if (string.IsNullOrEmpty(host))
		{
			Debug.Log("host is null url=" + uri);
			errorCallBack(new Exception("host is null url=" + uri));
			return false;
		}
		else
		{
			if (_HostMaps.ContainsKey(host))
			{
				string ip = _HostMaps[host];
				if (string.IsNullOrEmpty(ip))
				{
					RequestWithUrl(uri,
						downLoadFinishCallBack, 
						progressCallBack, 
						errorCallBack, 
						isNeedUncompress, 
						type, 
						headers);					
				}
				else
				{
					RequestWithIpAndHost(uri, ip, host, 
						downLoadFinishCallBack, 
						progressCallBack, 
						errorCallBack, 
						isNeedUncompress, 
						type, 
						headers);
				}
			}
			else
			{
				//本地解析Host
				string hostAdress = GetHostAddress(host);
				if (string.IsNullOrEmpty(hostAdress))
				{
					Debug.Log("本地dns解析失败，尝试用httpdns host=" + host);
					//如果本地DNS解析失败，则通过第三方dns解析
					GetHttpDns(host, delegate(string ip){
						if (string.IsNullOrEmpty(ip))
						{
							Debug.Log("httpdns解析失败 host=" + host);
							errorCallBack(new Exception("httpdns解析失败 host=" + host));
						}
						else
						{
							_HostMaps.Add(host, ip);
							RequestWithIpAndHost(uri, ip, host, 
								downLoadFinishCallBack, 
								progressCallBack, 
								errorCallBack, 
								isNeedUncompress, 
								type, 
								headers);
						}
					});
				}
				else
				{
					_HostMaps.Add(host, "");
					RequestWithUrl(uri,
						downLoadFinishCallBack, 
						progressCallBack, 
						errorCallBack, 
						isNeedUncompress, 
						type, 
						headers);
				}
			}

			return true;			
		}
	}

	public void RequestWithIpAndHost(string uri,string ip, string host,
		System.Action<ByteArray> downLoadFinishCallBack,
		System.Action<float> progressCallBack = null,
		System.Action<Exception> errorCallBack = null,
		bool isNeedUncompress = false,
		SimpleWWW.ConnectionType type = SimpleWWW.ConnectionType.Continued_Connect,
		Hashtable headers = null
	)
	{
		Debug.Log("url= " + uri + " ip=" + ip);

		string newUri = uri;

		//如果是ipv4地址，则用IP方式访问
		if (ip.Contains("."))
		{
			newUri = uri.Replace(host, ip);
			if (headers == null)
			{
				headers = new Hashtable();
			}
			headers.Add("Host", host);
		}

		Request request = new Request(Request.HTTP_GET_METHOD, newUri, headers);
		if (request != null)
		{
			try
			{
				_httpController.Receive(request, delegate(byte[] bytes) {
					downLoadFinishCallBack(new ByteArray(bytes));
				}, 
				progressCallBack, 
				delegate(Exception e) {
					RemoveHostMap(uri);
					Debug.LogError(string.Format("RequestException url={0} Exception={1}", uri, e.ToString()));
					if (errorCallBack != null)
					{
						errorCallBack(e);
					}
				}, 
				isNeedUncompress, 
				type);
			}
			catch (Exception e)
			{
				RemoveHostMap(uri);
				Debug.LogException(e);
				throw e;
				if (errorCallBack != null)
				{
					errorCallBack(e);
				}
			}
		}
	}

	private void RemoveHostMap(string url)
	{
		string host = StringHelper.GetHostFromUrl(url);
		if (_HostMaps.ContainsKey(host))
		{
			_HostMaps.Remove(host);
		}
	}

	public void RequestWithUrl(string uri,
		System.Action<ByteArray> downLoadFinishCallBack,
		System.Action<float> progressCallBack = null,
		System.Action<Exception> errorCallBack = null,
		bool isNeedUncompress = false,
		SimpleWWW.ConnectionType type = SimpleWWW.ConnectionType.Continued_Connect,
		Hashtable headers = null
	)
	{
		Debug.Log("url= " + uri);
		Request request = new Request(Request.HTTP_GET_METHOD, uri, headers);
		if (request != null)
		{
			try
			{
				_httpController.Receive(request, delegate(byte[] bytes) {
					downLoadFinishCallBack(new ByteArray(bytes));
				}, 
				progressCallBack, 
				delegate(Exception e) {
					RemoveHostMap(uri);
					Debug.LogError(string.Format("RequestException url={0} Exception={1}", uri, e.ToString()));
					if (errorCallBack != null)
					{
						errorCallBack(e);
					}
				}, 
				isNeedUncompress, 
				type);
			}
			catch (Exception e)
			{
				RemoveHostMap(uri);
				Debug.LogException(e);
				throw e;
				if (errorCallBack != null)
				{
					errorCallBack(e);
				}
			}
		}
	}

	private string GetHostAddress(string host)
	{
		try
		{
			var ipEntry = System.Net.Dns.GetHostEntry(host);
			var addrs = ipEntry.AddressList;
			string addr = addrs != null && addrs.Length > 0 ? addrs[0].ToString() : "";
			Debug.Log(string.Format("GetHostAddress host={0} address={1}", host, addr));
			return addr;
		}
		catch (Exception e)
		{
			Debug.Log(string.Format("有可能是断网：{0}", e.Message));
			return "";
		}
	}

	private void GetHttpDns(string host, Action<string> callback)
	{
		string url  = string.Format("http://119.29.29.29/d?dn={0}", host);

		Request request = new Request(Request.HTTP_GET_METHOD, url, null);
		if (request != null)
		{
			try
			{
				_httpController.Receive(request, delegate(byte[] bytes) {
					string ips = new ByteArray(bytes).ToUTF8String ();
					string ip = "";
					if (!string.IsNullOrEmpty(ips))
					{
						//这里的ip返回可能会有多个值，所以要进行下处理， 取第一个
						string[] ipSplit = ips.Split(';');
						if (ipSplit.Length > 0)
						{
							ip = ipSplit[0];
						}
					}

					Debug.Log(string.Format("GetHttpDns host={0} ips={1} ip={2}", host, ips, ip));
					callback(ip);
				}, null, delegate {
					Debug.Log(string.Format("GetHttpDns host={0} error", host));
					callback("");
				}, false, SimpleWWW.ConnectionType.Short_Connect);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				Debug.Log(string.Format("GetHttpDns host={0} Exception", host));
				callback("");
			}
		}
		else
		{
			Debug.Log("GetHttpDns error");
			callback("");			
		}
	}


    /// <summary>
    /// HTTP 上传操作
    /// </summary>
    /// <param name="uri">URL地址</param>
    /// <param name="uploadFinishCallBack">上传结束后的回调</param>
    /// <returns></returns>
	/// 没有使用
    public bool UpLoad( string url, byte[] dataString, System.Action uploadFinishCallBack = null, System.Action<Exception> errorCallBack = null )
    {
		if ( _httpController == null )
		{
			Debug.Log( "Http Controller is null ");
			return false;
		}


		Request request = new Request( Request.HTTP_POST_METHOD, url );
		if ( request != null )
		{
			try
			{
				_httpController.Send(request, dataString, uploadFinishCallBack, errorCallBack);
			}
			catch( Exception e)
			{
				Debug.LogException(e);
				throw e;
			}
		}
		return true;
    }
}
