//#define DONT_USE_WEB_LIB

using System;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;
using System.IO;

public class SimpleWWW : MonoBehaviour
{
	/// <summary>
	/// HTTP连接模式
	/// 	Continued_Connect :　 持续的连接
	/// 	Short_Connect	  :   短连接
	/// </summary>
	public enum ConnectionType
	{
		Continued_Connect = 0,
		Short_Connect     = 1
	}

    public void Receive(Request request, System.Action<byte[]> bytesDlegate, System.Action<float> progressDlegate = null, System.Action<Exception> errorDlegate = null, bool uncompress = false,ConnectionType connetType = ConnectionType.Continued_Connect)
    {
        StartCoroutine(_Receive(request, bytesDlegate, progressDlegate, errorDlegate, uncompress, connetType));
    }


	public void Send( Request request , byte[] data, System.Action action = null, System.Action<Exception> errorDlegate = null)
	{
		StartCoroutine(_Sent(request, data, action, errorDlegate));		
	}
	
	//最多重连数量
	private readonly int MAXIMUMREDIRECTS = 3;

	private Dictionary<Request, long> _RequestTimeoutMaps;

	void Awake()
	{
		_RequestTimeoutMaps = new Dictionary<Request, long>();
	}

	IEnumerator _Receive(Request request, System.Action<byte[]> bytesDelegate, System.Action<float> progressDlegate = null, System.Action<Exception> errorDlegate = null,  bool uncompress = false,ConnectionType connetType = ConnectionType.Continued_Connect )
	{
		if ( request != null )
		{
			int index = AddRequstList(request);


			WWW www = null;  
			int retry = 0;
			while (retry++ < MAXIMUMREDIRECTS)
			{
				request.progress = 0f;

				AddRequestTimeout(request);

				if( connetType == ConnectionType.Continued_Connect )
				{
					www = new WWW( request.requestURL, null, request.Headers);
				}
				else
				{
					var postHeader = new Dictionary<string, string>();
					if (request.Headers != null)
					{
						foreach (var header in request.Headers)
						{
							postHeader[header.Key] = header.Value;
						}
					}
					postHeader.Add("Connection", "close");
					www = new WWW( request.requestURL, null, postHeader );
				}					

				bool timeOut = false;

				while (!www.isDone)
				{
					if (CheckRequestTimeout(request, www.progress))
					{
						timeOut = true;
						break;
					}

					if (progressDlegate != null) progressDlegate(www.progress);
					yield return new WaitForEndOfFrame();
				}


				if( !timeOut && www.isDone && www.error == null )
				{
					if (www.text.StartsWith("<html>"))
					{
						CdnReportHelper.Report(request.requestURL, www.text);
						if (errorDlegate != null) errorDlegate( new Exception( www.text ));
					}
					else
					{
						request.bytes = www.bytes;

						if (uncompress)
						{
							request.Uncompress();
							while (!request.isDone)
							{
								yield return new WaitForEndOfFrame();
							}
						}					

						if (progressDlegate != null) progressDlegate(1.0f);
						if (bytesDelegate   != null) bytesDelegate(request.bytes);
					}

					break;
				}
				else
				{
					if( retry >= MAXIMUMREDIRECTS)
					{
						if (timeOut)
						{
							CdnReportHelper.Report(request.requestURL, "请求超时");
							if (errorDlegate != null) errorDlegate( new Exception( "请求超时" ));
						}
						else
						{
							CdnReportHelper.Report(request.requestURL, www.error);
							if (errorDlegate != null) errorDlegate( new Exception( www.error ));
						}
					}
					else
					{
						if (timeOut)
						{
							Debug.Log( string.Format( "TimeOut Try again Link url : {0} , time : {1}", request.requestURL, retry ));
						}
						else
						{
							Debug.Log( string.Format( "Try again Link url : {0} , time : {1}", request.requestURL, retry ));
						}
						yield return new WaitForSeconds( 1.0f );
					}
				}
			}

			if( www != null )
			{
				www.Dispose();
				www = null;				
			}
			RemoveRequestTimeout(request);
			ClearRequstList(index);
		}
	}

	private bool CheckRequestTimeout(Request request, float progress=0f)
	{
		if (_RequestTimeoutMaps.ContainsKey(request))
		{
			long time = _RequestTimeoutMaps[request];
			double passTime = (DateTime.Now.Ticks - time)/ 10000000.0; 
			float checkTime = 15f; //检测超时时间5秒
			if (request.progress != progress)
			{
				_RequestTimeoutMaps[request] = DateTime.Now.Ticks;
				request.progress = progress;
			}
			if (passTime > checkTime)
			{
				Debug.Log(string.Format("passTime={0} checkTime={1}", passTime, checkTime));
				return true;
			}
		}

		return false;
	}

	private void AddRequestTimeout(Request request)
	{
		if (_RequestTimeoutMaps.ContainsKey(request))
		{
			_RequestTimeoutMaps[request] = DateTime.Now.Ticks;
		}
		else
		{
			_RequestTimeoutMaps.Add(request, DateTime.Now.Ticks);
		}
	}

	private void RemoveRequestTimeout(Request request)
	{
		if (_RequestTimeoutMaps.ContainsKey(request))
		{
			_RequestTimeoutMaps.Remove(request);
		}
	}

	IEnumerator _Sent(Request request, byte[] data, System.Action action = null, System.Action<Exception> errorDlegate = null)
	{
		int index = AddRequstList(request);

		var postHeader = new Dictionary<string, string>();
		postHeader.Add("Content-Type", "application/octet-stream");
		postHeader.Add("Content-Length", data.Length.ToString());
		postHeader.Add("Connection", "close");

		WWW sendWWW = new WWW( request.requestURL, data, postHeader);


		yield return sendWWW;

		if( sendWWW.isDone && sendWWW.error == null )
		{
			if ( action != null ) action();
		}
		else
		{
			if (errorDlegate != null) errorDlegate(new Exception( sendWWW.error ));
			throw request.exception;
		}		

		ClearRequstList(index);
	}

	List<System.Action> onQuit = new List<System.Action>();
	public void OnQuit(System.Action fn)
	{
		onQuit.Add(fn);
	}

	Request[] requestList = new Request[20];
	public int AddRequstList( Request request )
	{
		for( int i = 0 ; i < requestList.Length ; i ++ )
		{
			if ( requestList[i] == null )
			{
				requestList[i] = request;
				return i;
			}
		}

		return -1;
	}

	public void ClearRequstList( int index )
	{
		if (index == -1) return;
		if (index < requestList.Length)
		{
			requestList[index] = null;
		}
	}

	void OnApplicationQuit()
	{
		for (int i = 0; i < requestList.Length; i++)
		{
			if (requestList[i] != null)
			{
				requestList[i].isAppQuit = true;
			}
		}
	}
}
