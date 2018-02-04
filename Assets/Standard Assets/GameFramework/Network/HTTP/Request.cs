//#define DONT_USE_WEB_LIB


using System;
using System.Collections;

//#if (!UNITY_IPHONE && !DONT_USE_WEB_LIB)
//using System.Web;
//#endif
using System.Threading;
using System.Net;
using System.IO;

using UnityEngine;
using System.Collections.Generic;
using LITJson;

public class Request
{
    //HTTP 获取方式
    public static string HTTP_GET_METHOD  = "GET";

    public  static string HTTP_POST_METHOD = "POST";


    //当前使用的方法
    private string httpMethod = HTTP_GET_METHOD;

    //URL
    private string url;

    public string requestURL { get { return url; } }


    /// <summary>
    /// http header
    /// </summary>
    private Hashtable headers;

    private Dictionary<string, string> _headerDic; 
    public Dictionary<string, string> Headers
    {
	    get
	    {
            if (_headerDic == null && headers != null)
            {
                _headerDic = new Dictionary<string, string>();
                foreach (string headerKey in headers.Keys)
                {
                    _headerDic[headerKey] = (string)headers[headerKey];
                }
            }

            return _headerDic;
        }
    }

    //是否正在传输
    private bool sent = false;

    //是否完成
    private bool _isDone = true;
    public bool isDone { get { return _isDone; } }

    //加载进度
	public float progress = 0.0f;


    //是否需要LZMA解压
    private bool lzma_uncompress = false;

    //异常
    public Exception exception = null;

    //最多重连数量
    private int MAXIMUMREDIRECTS = 10;

    //获取字节数
    private byte[] _bytes = null;

    //程序退出
    public bool isAppQuit = false;
    public byte[] bytes
    {
        get
        {
            if (isDone)
            {
                return _bytes;
            }
            else
            {
                return null;
            }
        }
        set
        {
            _bytes = value;
        }
    }

    public Request(string method, string url, Hashtable headers = null)
    {
        this.httpMethod = method;
        this.url = url;
        this.headers = headers;
    }

    /// <summary>
    /// 解压数据
    /// </summary>
    public void Uncompress()
    {
        if (sent)
        {
            throw new InvalidOperationException("Request has already completed.");
        }

        sent = true;
        _isDone = false;

        //对象池进行接受
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object t)
        {
            try
            {
				_bytes = ZipLibUtils.Uncompress(_bytes);
            }
            catch (Exception e)
            {
                exception = e;
            }

            _isDone = true;

        }));
    }

    /// <summary>
    /// 接收数据
    /// Unity_IPHONE版本去除web库的使用
    /// </summary>
    public void Receive( bool UnCompress = false )
    {
    }

    /// <summary>
    /// 片段接收处理函数
    /// <param bytes[]>接受到的信息</param>
    /// <param bool >是否接受完毕</param>
    /// </summary>
    private System.Action<byte[], bool> _dataReceiveHander = null;
    private long _receiveLen = 1024; //1KB


    /// <summary>
    /// 分片加载模式， 
    /// </summary>
    public void ReceiveFragments( System.Action<byte[], bool> dataReceiveHander, long lenLimit )
    {
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    public void Send( byte[] data )
    {
    }
}
