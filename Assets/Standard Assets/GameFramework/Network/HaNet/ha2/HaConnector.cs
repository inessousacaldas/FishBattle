//#define HandleCountMgrDebug

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Timer = System.Timers.Timer;

/**
 * HaConnector负责连接网络，并在出错的时候，抛出错误事件(event)
 * 
 * client连接ha网络的次序是：
 * 1. 接入ha.net (系统会初始化沙箱请求)
 * 2. 发送 SSL_OPEN_REQ，建立加密通道
 * 3. 发送 JOIN，顺利接入到HA网络
 * 4. join以后，可以发送以下指令
 *    a. 查询service所在节点
 *    b. 发送信息到某个节点上
 *    c. get/set节点的属性
 *    d. 离开ha.net
 */

public class HaConnector : MonoBehaviour
{
    private Socket _socket;
    private uint _state; // 自己的当前state
    private ProtoByteArray _receiveBytes; // 数据的缓冲区
    private byte[] _receiveBytesPool;
    private ARC4 _erc4; // rc4支持(for encode)
    private ARC4 _drc4; // rc4支持(for decode)

    private Timer _connectWaitingTimer; // for connect timeout
    private Timer _keepaliveSendTimer;
    private Timer _keepaliveCheckTimer;

    public Action<uint, string> OnLeaveEvent;
    public Action<ByteArray, bool> OnMessageEvent;
    public Action OnJoinEvent;
    public Action<ServiceInfo> OnServiceEvent;
    public Action OnTimeOutEvent;
    public Action<uint> OnCloseEvent;
    public Action OnLoginState;
    public Action<uint> OnStateEvent;

    private EventQueue _eventObjQueue;
    private List<SendObject> _sendObjPoolList;

    private static readonly int RECEIVE_BUFFER_SIZE = 8192;
    private static readonly int HEAD_COUNT = 16;

    private static readonly int MIN_OF_SIZE = 20; //接受数据小于最小数据包大小，可能数据异常
    private static readonly int OUT_OF_SIZE = 1024*1024; //接受数据超出检测大小上限，很可能数据异常

    private static readonly int TIME_keepaliveSend = 3*1000;
    private static readonly int TIME_keepaliveCheck = 20*1000;
    private static readonly int TIME_connectWaiting = 5*1000;

    private const int DEFAULT_HANDLECOUNT = 2;
    private int _maxHandleCount = DEFAULT_HANDLECOUNT;    //每帧处理消息最大数量

    private bool ignoreHandle = false;
    public void IgnoreMaxHandleMgr()
    {
        _maxHandleCount = 100;
        ignoreHandle = true;
    }
    public void ResetMaxHandleCount()
    {
        _maxHandleCount = DEFAULT_HANDLECOUNT;
        ignoreHandle = false;
    }
    public static long TotalReceiveBytes;
    public static long TotalReceiveCount;
    public static long TotalSendBytes;
    public static long TotalSendCount;

    private List<int> _tryPorts;

    private bool _waitForAck; //等待数据接收

    private static bool HEAD_ENCODE = true;

    private int _connectServerAccessId;
    private HandleCountMgr handleCountMgr;
    private void Awake()
    {
        HaApplicationContext.setConnector(this);
        _state = (uint)HaStage.DISCONNECT; // bug?
        _sendObjPoolList = new List<SendObject>();
        _eventObjQueue = new EventQueue();
        handleCountMgr = new HandleCountMgr(this);
    }

    private void OnApplicationPause(bool paused)
    {
        Log("HaConnector OnApplicationPause " + paused);

        if (!paused)
        {
            bool needCleanEventQueue = false;
            foreach (EventObject obj in _eventObjQueue)
            {
                switch (obj.type)
                {
                    case EventObject.Event_Close:
                        if (OnCloseEvent != null)
                        {
                            OnCloseEvent(obj.state);
                        }
                        needCleanEventQueue = true;
                        break;
                    case EventObject.Event_Leave:
                        if (OnLeaveEvent != null)
                        {
                            OnLeaveEvent(obj.state, obj.msg);
                        }
                        needCleanEventQueue = true;
                        break;
                    case EventObject.Event_State:
                        if (OnStateEvent != null)
                        {
                            OnStateEvent(obj.state);
                        }
                        needCleanEventQueue = true;
                        break;
                }
            }

            if (needCleanEventQueue)
            {
                Log("CleanEventQueue");
                _eventObjQueue.Clear();
            }
        }
    }

    

    private void Update()
    {
        int runCount = 0;
        if (!ignoreHandle)
        {
            handleCountMgr.Reset();
        }
        while (_eventObjQueue.Count > 0)
        {
            EventObject obj = null;
            obj = _eventObjQueue.Dequeue();

            if (obj != null)
            {
                HandleEventObj(obj);
            }

            runCount++;

            if (!(ignoreHandle ? runCount < _maxHandleCount : handleCountMgr.CheckCanRun(runCount)))
            {
                break;
            }
        }
    }

    private void HandleEventObj(EventObject obj)
    {
        switch (obj.type)
        {
            case EventObject.Event_Close:
                if (OnCloseEvent != null)
                {
                    OnCloseEvent(obj.state);
                }
                break;
            case EventObject.Event_Joined:
                if (OnJoinEvent != null)
                {
                    OnJoinEvent();
                }
                break;
            case EventObject.Event_Leave:
                if (OnLeaveEvent != null)
                {
                    OnLeaveEvent(obj.state, obj.msg);
                }
                break;
            case EventObject.Event_Message:
                if (OnMessageEvent != null)
                {
                    OnMessageEvent(obj.byteArray,obj.compress);
                }
                break;
            case EventObject.Event_Service:
                if (OnServiceEvent != null)
                {
                    ServiceInfo selectInfo = null;
                    foreach (var info in obj.services)
                    {
                        if (info.id == _connectServerAccessId.ToString())
                        {
                            selectInfo = info;
                            break;
                        }
                    }
                    OnServiceEvent(selectInfo);
                }
                break;
            case EventObject.Event_State:
                if (OnStateEvent != null)
                {
                    OnStateEvent(obj.state);
                }
                break;
            case EventObject.Event_Timeout:
                if (OnTimeOutEvent != null)
                {
                    OnTimeOutEvent();
                }
                break;
            case EventObject.Event_SSL_OPEN_RES:
                // 和ha.net交换版本
                var ver = new VerInstruction();
                ver.execute(this);
                break;
            case EventObject.Event_VER:
                var join = new JoinInstruction();

                var config = HaApplicationContext.getConfiguration();

                join.setProperties(config.getClientProperties());
                join.setClientId(config.getClientId());
                join.setName(config.getClientName());

                var ips = new ArrayList(); // TODO fixme
                ips.Add("127.0.0.1");

                join.setLocalIps(ips);
                join.setHost("127.0.0.1");

                join.execute(this);
                break;
        }
    }

    // C#  -----------------------------------------------------------------------------------------
    public delegate void CallBack_Send(bool success, Erro_Socket error, string exception);

    public delegate void CallBack_Disconnect(bool success, Erro_Socket error, string exception);

    public CallBack_Send callBack_Send;
    public CallBack_Disconnect callBack_Disconnect;

    private Erro_Socket error_Socket = Erro_Socket.SUCCESS;

    public enum Erro_Socket
    {
        SUCCESS = 0, //成功
        SOCKET_NULL = 2, //套接字为空
        SOCKET_UNCONNECT = 3, //套接字未连接
        CONNECT_UNSUCCESS_UNKNOW = 4, //连接失败未知错误
        CONNECT_CONNECED_ERROR = 5, //重复连接错误
        SEND_UNSUCCESS_UNKNOW = 6, //fa送失败未知错误
        RECEIVE_UNSUCCESS_UNKNOW = 7, //收消息未知错误
        DISCONNECT_UNSUCCESS_UNKNOW = 8 //断开连接未知错误
    }

    //--------------------------------------------------------------
    // 属性访问接口
    //--------------------------------------------------------------
    public uint getState()
    {
        return _state;
    }

    public void setState(uint state)
    {
        _state = state;

        //新的HA改为在JOINED的时候可以开启keepalive
        if (_state == HaStage.JOINED)
        {
            // 启动定时器
            StartKeepalive();

#if !UNITY_EDITOR
            StartKeepaliveChecker();
#endif
        }

        var obj = new EventObject(EventObject.Event_State);
        obj.state = _state;
        AddEventObj(obj);

        Log("HA setState=" + (int) state);
    }

    public void setRc4key(ByteArray k)
    {
        if (k == null)
        {
            Log("setRc4key null");
            _erc4 = null;
            _drc4 = null;
            return;
        }

        _erc4 = new ARC4();
        _drc4 = new ARC4();

#if UNITY_EDITOR
        //编辑器模式下为空，连内网，如果需要调试外网，则需要修改这里
        string sessionKey = "";
#else
//打包模式需要跟运维约定HA的加密key，不同项目的key不同
        string sessionKey = HaApplicationContext.getConfiguration().GetSessionKey();
        #endif
        
        byte[] newKey = System.Text.Encoding.Default.GetBytes(sessionKey + System.Text.Encoding.Default.GetString(k.bytes));
        
        _erc4.init(new ProtoByteArray(newKey));
        _drc4.init(new ProtoByteArray(newKey));
    }

    public void Close()
    {
        Log("HaConnector close");

        HaApplicationContext.setConnector(null);

        TotalReceiveBytes = 0;
        TotalReceiveCount = 0;
        TotalSendBytes = 0;
        TotalSendCount = 0;

        _waitForAck = false;

        lock (_sendObjPoolList)
        {
            _sendObjPoolList.Clear();
        }

        if (_connectAsynsResult != null)
        {
            _connectAsynsResult.AsyncWaitHandle.Close();
            _connectAsynsResult = null;
        }

        _sending = false;

        StopConnectWaitingTimer();
        StopKeepaliveSendTimer();
        StopKeepaliveCheckerTimer();

        try
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
                _thread = null;
                Debug.Log("Tryjing to abort");
            }
        }
		catch (Exception e)
        {
			Debug.Log("Aborting thread failed\n" + e);
        }

        if (_socket != null)
        {
			try
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Disconnect(false);
				_socket.Close();
			}	
			catch (Exception e)
			{
				Debug.Log("socket Close failed\n" + e);
			}
            _socket = null;

            _receiveBytesPool = null;
            _receiveBytes = null;

            Log("socket status = " + getState());
            setState((uint) HaStage.DISCONNECT);
        }
    }

    private void StopKeepaliveSendTimer()
    {
        if (_keepaliveSendTimer != null)
        {
            _keepaliveSendTimer.Elapsed -= keepaliveSendHandler;
            if (_keepaliveSendTimer == null)
            {
                return;
            }
            _keepaliveSendTimer.Stop();
            _keepaliveSendTimer = null;
        }
    }

    private void StopKeepaliveCheckerTimer()
    {
        if (_keepaliveCheckTimer != null)
        {
            _keepaliveCheckTimer.Elapsed -= keepaliveCheckerHandler;
            if (_keepaliveCheckTimer == null)
            {
                return;
            }
            _keepaliveCheckTimer.Stop();
            _keepaliveCheckTimer = null;
        }
    }

    //--------------------------------------------------------------
    // 把信息写到远端的网络
    //--------------------------------------------------------------

    public void WriteBytes(ProtoByteArray bytes)
    {
        if (_state == HaStage.DISCONNECT)
        {
            Log("writeBytes DISCONNECT");
            return;
        }

        // 把信息写到远端的网络，并且flush
        if (_socket != null && _socket.Connected)
        {
            uint bufSize = bytes.Length;
            Async_Send(bytes, bufSize, null);
            TotalSendBytes += bufSize;
            TotalSendCount += 1;
        }
        else
        {
            Log("[ERROR]往已经关闭的Socket写入");
        }
    }

    //--------------------------------------------------------------------------------------------------------
    private bool _sending;

	private void Async_Send(ProtoByteArray sendByteArray, uint bufSize, CallBack_Send sendCallback)
    {
		var sendObject = new SendObject(sendByteArray, bufSize, sendCallback);

        lock (_sendObjPoolList)
        {
            _sendObjPoolList.Add(sendObject);
        }

        if (_sending == false)
        {
            SendObjectToSocket();
        }
    }

    private void SendObjectToSocket()
    {
        SendObject sendObject = null;

        lock (_sendObjPoolList)
        {
            if (_sendObjPoolList.Count > 0)
            {
                sendObject = _sendObjPoolList[0];
                _sendObjPoolList.RemoveAt(0);
            }
        }

        if (sendObject == null)
        {
            return;
        }

        error_Socket = Erro_Socket.SUCCESS;
        callBack_Send = sendObject.callBack_Send;

        if (_socket == null)
        {
            error_Socket = Erro_Socket.SOCKET_NULL;
            Log("套接字为空，fa送失败");
            callBack_Send(false, error_Socket, "");
        }
        else if (!_socket.Connected)
        {
            error_Socket = Erro_Socket.SOCKET_UNCONNECT;
            Log("未连接，fa送失败");
            callBack_Send(false, error_Socket, "");
        }
        else
        {
            _sending = true;
            LogWarning("socket.BeginSend bufferLen=" + sendObject.sendBuffer.Length);

			// 首先是根据当前的状态，决定是否加密
			if (_erc4 != null)
			{
				//HaConnector.Log("encrypted bytes length:" + bytes.Length.ToString() );
				if (HEAD_ENCODE)
				{
					_erc4.encrypt(sendObject.sendBuffer, 0, 0);
				}
				else
				{
					_erc4.encrypt(sendObject.sendBuffer, 0, HEAD_COUNT);
				}
			}
			
			try
			{
				_socket.BeginSend(sendObject.sendBuffer.GetBuffer(), 0, (int) sendObject.bufSize, SocketFlags.None, sendCallback,
                    _socket);
            }
            catch (Exception e)
            {
                Log("send message Exception\n" + e);
            }
        }
    }

    private void sendCallback(IAsyncResult asyncSend)
    {
        //HaConnector.Log("sendCallback");

        try
        {
            asyncSend.AsyncWaitHandle.Close();

            var clientSocket = (Socket) asyncSend.AsyncState;

            clientSocket.EndSend(asyncSend);
            //int bytesSent = clientSocket.EndSend(asyncSend);
            LogWarning("bytes sent.");

            if (callBack_Send != null) //回调
                callBack_Send(true, error_Socket, "");
        }
        catch (Exception e)
        {
            Log("sendCallback Exception=" + e.Message);
        }

        _sending = false;
        SendObjectToSocket();
    }

    //--------------------------------------------------------------------------------------------------------

    public void StartKeepalive()
    {
        Log("startKeepalive()");

        if (_keepaliveSendTimer == null)
        {
            _keepaliveSendTimer = new Timer(TIME_keepaliveSend);
        }

        if (_keepaliveSendTimer.Enabled)
        {
            return;
        }

        _keepaliveSendTimer.Elapsed += keepaliveSendHandler;
        _keepaliveSendTimer.AutoReset = true;
        _keepaliveSendTimer.Enabled = true;
    }

    private void StartKeepaliveChecker()
    {
#if UNITY_EDITOR
        return;
#endif
        Log("StartKeepaliveChecker()");
        _waitForAck = true;

        if (_keepaliveCheckTimer == null)
        {
            _keepaliveCheckTimer = new Timer(TIME_keepaliveCheck);
            _keepaliveCheckTimer.Elapsed += keepaliveCheckerHandler;
            _keepaliveCheckTimer.AutoReset = true;
            _keepaliveCheckTimer.Enabled = true;
        }
    }

    private void keepaliveSendHandler(object source, ElapsedEventArgs e)
    {
        if (_state >= HaStage.JOINED)
        {
            var keepalive = new KeepaliveInstruction();
            keepalive.execute(this);
        }
        else
        {
            if (_keepaliveSendTimer == null)
            {
                return;
            }
            _keepaliveSendTimer.Stop();
        }
    }

    private void keepaliveCheckerHandler(object source, ElapsedEventArgs e)
    {
        if (_waitForAck)
        {
            LogError("Close_status_KeepaliveOutTime");
            CloseSocket(EventObject.Close_status_KeepaliveOutTime);
        }
        else
        {
            _waitForAck = true;
        }
    }

    /**
	 * 允许把自己的属性暴露到ha.net中 
	 * 
	 * @param	name
	 * @param	obj
	 * @return
	 */

    //public bool LinkPropertyObject(string name, IConigurable obj)
    //{
    //    return false;
    //}

    //public void UnlinkPropertyObject(string name)
    //{
    //}

    /**
	 * 获取某个节点的属性信息，只有接入ha.net中才会生效
	 * 
	 * @param	name
	 * @return
	 */

    //public string GetProperty(string name)
    //{
    //    return null;
    //}

    /**
	 * 设置某个节点的属性信息，如果不是特殊的情况（如服务器赋予权限），
	 * client是不能其他节点的属性的
	 * 
	 * @param	name
	 * @param	value
	 */

    //public void SetProperty(string name, string value)
    //{
    //}

    /**
	 * 加入ha.net网络
	 */

    public void Join(int haVer, string tryPortParam, int connectServerAccessId)
    {
        _tryPorts = tryPortParam.ConvertToList<int>(',', int.Parse);
        _connectServerAccessId = connectServerAccessId;

        GameStopwatch.Begin(HA_SSL_OPEN_RES_TIMER);
        GameStopwatch.Begin(HA_VER_TIMER);
        GameStopwatch.Begin(HA_JOIN_RES_TIMER);

        var config = HaApplicationContext.getConfiguration();

        string ip = config.getHost();
        int port = config.getPort();

        if (_tryPorts.Contains(port))
        {
            _tryPorts.Remove(port);
        }

        if (haVer == 0)
        {
            HEAD_ENCODE = false;
        }
        else
        {
            HEAD_ENCODE = true;
        }
        HEAD_ENCODE = true;
        Debug.Log("HEAD_ENCODE=" + HEAD_ENCODE);

        Async_Connect(ip, port);
    }

    private IAsyncResult _connectAsynsResult;

    private void Async_Connect(string ip, int port)
    {
        if (_socket != null)
        {
            Close();
        }

        error_Socket = Erro_Socket.SUCCESS;

		//arc的初始化放到new socket之前
		_erc4 = null;
		_drc4 = null;

        if (_socket != null && _socket.Connected)
        {
            LogError("Close_status_StarConnectClose");
            CloseSocket(EventObject.Close_status_StarConnectClose);
            //connectHandler(false, Erro_Socket.CONNECT_CONNECED_ERROR, "");//重复连接错误
        }
        else if (_socket == null || !_socket.Connected)
        {
            //			HaConnector.Log("_socket.BeginConnect");

            //采用TCP方式连接  
            IPAddress[] hostIps = null;
            // 没有网络的时候不是返回空数组，而是丢出异常
            try
            {
                hostIps = Dns.GetHostAddresses(ip);
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("有可能是断网：{0}", e.Message));
            }
            // 打印方便回看数据
            if (hostIps != null)
            {
                for (int i = 0; i < hostIps.Length; i++)
                {
                    Debug.Log(string.Format("{0} {1}", hostIps[i], hostIps[i].AddressFamily));
                }
            }
            var netType = hostIps != null && hostIps.Length > 0 ? hostIps[0].AddressFamily : AddressFamily.InterNetwork;

            _socket = new Socket(netType, SocketType.Stream, ProtocolType.Tcp);
            _socket.Blocking = true;
            //_socket.ReceiveBufferSize = 64*1024;
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 120*1000);

            try
            {
                Debug.Log(string.Format("Connect ip={0} port={1} serviceId={2}", ip, port, _connectServerAccessId));
                //异步连接,连接成功调用connectCallback方法  
                if (_connectAsynsResult != null)
                {
                    _connectAsynsResult.AsyncWaitHandle.Close();
                }
                _connectAsynsResult = _socket.BeginConnect(ip, port, connectCallback, _socket);
            }
            catch (Exception e)
            {
                Log("_socket.BeginConnect\n" + e);
            }


            if (_connectWaitingTimer == null)
            {
                _connectWaitingTimer = new Timer(TIME_connectWaiting);
                _connectWaitingTimer.Elapsed += OnConnectWaitingTimerElapsed;
                _connectWaitingTimer.Enabled = true;
            }
        }
    }

    private void OnConnectWaitingTimerElapsed(object source, ElapsedEventArgs e)
    {
        StopConnectWaitingTimer();

        if (!_socket.Connected)
        {
            Log("connect Time Out");

            if (_tryPorts.Count > 0)
            {
                Close();
                Log("try next ports");
                var config = HaApplicationContext.getConfiguration();
                string ip = config.getHost();
                int port = _tryPorts[0];
                _tryPorts.RemoveAt(0);
                Async_Connect(ip, port);
            }
            else
            {
                //超时
                LogError("Close_status_ConnectOutTime");
                CloseSocket(EventObject.Close_status_ConnectOutTime);
            }
        }
    }

    private void StopConnectWaitingTimer()
    {
        if (_connectAsynsResult != null)
        {
            _connectAsynsResult.AsyncWaitHandle.Close();
            _connectAsynsResult = null;
        }

        if (_connectWaitingTimer != null)
        {
            _connectWaitingTimer.Elapsed -= OnConnectWaitingTimerElapsed;
            if (_connectWaitingTimer == null)
            {
                return;
            }
            _connectWaitingTimer.Stop();
            _connectWaitingTimer = null;
        }
    }

    private Thread _thread;

    private void connectCallback(IAsyncResult asyncConnect)
    {
        if (_socket == null)
        {
            return;
        }

        if (_socket.Connected)
        {
            LogWarning("connect success");

            StopConnectWaitingTimer();

            //与socket建立连接成功，开启线程接受服务端数据。  
            _thread = new Thread(ReceiveSorket);
            _thread.IsBackground = true;
            _thread.Start();

            SendSSLOpen();
        }
        else
        {
            LogWarning("connect fail");
            OnConnectWaitingTimerElapsed(null, null);
        }
    }

    private int _len;

    private void ReceiveSorket()
    {
        LogWarning("ReceiveSorket");

        _receiveBytesPool = new byte[RECEIVE_BUFFER_SIZE];
        _receiveBytes = new ProtoByteArray();

        //在这个线程中接受服务器返回的数据  
        while (true)
        {
            if (_socket == null)
            {
                //与服务器断开连接跳出循环
                break;
            }

            if (!_socket.Connected)
            {
                //与服务器断开连接跳出循环  
                LogError("Close_status_GameConnectClose");
                CloseSocket(EventObject.Close_status_GameConnectClose);
                break;
            }
            try
            {
                //接受数据保存至bytes当中  
                //byte[] bytes = new byte[RECEIVE_BUFFER_SIZE];
                //Receive方法中会一直等待服务端回发消息  
                //如果没有回发会一直在这里等着。  
                int i = _socket.Receive(_receiveBytesPool);
                if (i <= 0)
                {
                    LogError("Close_status_ReceiveNull");
                    CloseSocket(EventObject.Close_status_ReceiveNull);
                    break;
                }

                _len = i;

                LogWarning("_socket.Receive len = " + i);

                _receiveBytes.WriteBytes(_receiveBytesPool, 0, i);
                if (_drc4 != null)
                {
                    int len = (int)_receiveBytes.Length;
                    _drc4.encrypt(_receiveBytes, len, len - i);
                }
                dataHandler();
            }
            catch (ThreadAbortException e)
            {
                //HaConnector.LogError("Close_status_ThreadAbortException\n" + e);
                //CloseSocket(EventObject.Close_status_ThreadAbortException);
                break;
            }
            catch (SocketException e)
            {
                LogError("Close_status_SocketException\n" + e);
                if (e.SocketErrorCode == SocketError.WouldBlock ||
                    e.SocketErrorCode == SocketError.IOPending ||
                    e.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                {
                    // socket buffer is probably empty, wait and try again
                    Thread.Sleep(30);
                }
                else
                {
                    CloseSocket(EventObject.Close_status_SocketException);
                    break;
                }
            }
            catch (Exception e)
            {
                LogError("Close_status_OtherException\n" + e);
                CloseSocket(EventObject.Close_status_OtherException);
                break;
            }
        }
    }

    public void CloseSocket(uint code)
    {
        Debug.Log("!!!! CloseSocket code = " + code);
        ClearEventObj();
        DispatchCloseEvent(code);
        Close();
    }

    private void DispatchCloseEvent(uint code)
    {
        var obj = new EventObject(EventObject.Event_Close);
        obj.state = code;
        AddEventObj(obj);
    }

    //----------------------------------------------------------------------------------------------------------------------

    /**
	 * 查询服务所在节点
	 * 
	 * @param	type
	 */

    public void QueryServices(string type)
    {
        var config = HaApplicationContext.getConfiguration();
        var query = new ServiceQueryRequestInstruction();

        query.setProperties(config.getClientProperties());
        query.setType(type);
        query.execute(this);
    }

    /**
	 * 发送消息到目标节点
	 * 
	 * @param	id 			目标节点
	 * @param	type 		各种标志位，目前，可以用0x1表示losable，即该消息可丢弃
	 * @param	message		具体的业务信息
	 */

    public void SendMessage(string id, uint type, ProtoByteArray message)
    {
        var send = new DirectMessageInstruction();
        //message.compress();
        send.setTargetId(uint.Parse(id));
        send.setRouteType(type);
        send.setMessage(message);
        send.execute(this);
    }

    private void SendSSLOpen()
    {
        var config = HaApplicationContext.getConfiguration();

        var sslopen = new SSLOpenRequestInstruction();
        sslopen.setProperties(config.getCreateChannelProperties());
        sslopen.setSupportEncryptMethods(config.getSupportEncryptMethods());
        sslopen.setAllowedMethod(config.getAllowedEncryptMethod());
        sslopen.setEncryptKey(config.getEncryptKey());
        sslopen.execute(this);
    }

    private void dataHandler()
    {
        if (HEAD_ENCODE)
        {
            DataHandlerWithHeadEncode();
        }
        else
        {
            DataHandlerWithoutHeadEncode();
        }
    }

    private void DataHandlerWithHeadEncode()
    {
        if (_receiveBytes == null)
        {
            LogWarning("_receiveBytes == null");
            return;
        }

        int len = (int) _receiveBytes.Length;

        // 这里需要处理分包组包的情况，并根据解码结果，把消息转换成为有含义的指令执行
        if (len > 0)
        {
            LogWarning("Receive Length:" + len);

            if (len < 4)
            {
                //整体长度不足，等后续数据到位处理
                _receiveBytes.Position = len;
				LogWarning(string.Format("len={0} < 4 整体长度不足，等后续数据到位处理", len));
                return;
            }

            _receiveBytes.Position = 0;

            int size = (int) _receiveBytes.ReadUnsignedInt(); //这个长度是整个数据包的长度
            LogWarning("Size = " + size);

            if (size <= MIN_OF_SIZE)
            {
				LogWarning("Close_status_MinOfSize = " + size + " len=" + len + " minLen=" + MIN_OF_SIZE);
                CloseSocket(EventObject.Close_status_MinOfSize);
                return;
            }

            if (size >= OUT_OF_SIZE)
            {
				LogWarning("Close_status_OutOfSize = " + size + " len=" + len + " maxLen=" + OUT_OF_SIZE);
                CloseSocket(EventObject.Close_status_OutOfSize);
                return;
            }

            if (size > len)
            {
                //包体长度不足，等数据
                _receiveBytes.Position = len;
				LogWarning(string.Format("size={0} > len={1} 包体长度不足，等数据", size, len));
                return;
            }

            _receiveBytes.Position = 0;

            var bytesBuffer = new byte[size];
            _receiveBytes.ReadBytes(bytesBuffer, 0, (uint) size);

            var bytes = new ProtoByteArray(bytesBuffer);

            // 当内容过多时，会把剩余的内容拷贝到一个临时缓存
            var buffer = new byte[len - size];
            var bytes2 = new ProtoByteArray(buffer);
            if (size < len)
            {
                _receiveBytes.Position = size;
                _receiveBytes.ReadBytes(bytes2.GetBuffer(), 0, (uint) (len - size));
            }

            readByteBuff(bytes);

            _receiveBytes = bytes2;

            if (size < len)
            {
				//LogWarning(string.Format("size={0} < len={1} 后续还有数据包， 继续分析", size, len));
                //后续还有数据包， 继续分析
                dataHandler();
            }
            else
            {
				//LogWarning("读取后 len = 0");
            }
        }
        else
        {
			//LogWarning("len = 0");
        }
    }

    private void DataHandlerWithoutHeadEncode()
    {
        _receiveBytes.Position = 0;

        int len = (int) _receiveBytes.bytesAvailable;

        LogWarning("Receive Length:" + len);

        // 这里需要处理分包组包的情况，并根据解码结果，把消息转换成为有含义的指令执行
        if (len > HEAD_COUNT)
        {
            int size = (int) _receiveBytes.ReadUnsignedInt(); //这个长度是整个数据包的长度
            LogWarning("Size = " + size);
            if (size > len)
            {
                _receiveBytes.Position = len;
                return;
            }

            // 首先是解码
            if (_drc4 != null)
            {
                _receiveBytes.Position = 0;
                _drc4.encrypt(_receiveBytes, size, HEAD_COUNT);
                //string tmp = ""; for ( int i = 0; i < _bytes.Length; ++i )	{ tmp += _bytes.GetBuffer()[i].ToString("x2") + " "; } HaConnector.Log( "receive byte:" + tmp );
            }

            _receiveBytes.Position = 0;

            var bytesBuffer = new byte[size];
            _receiveBytes.ReadBytes(bytesBuffer, 0, (uint) size);

            var bytes = new ProtoByteArray(bytesBuffer);

            // 当内容过多时，会把剩余的内容拷贝到一个临时缓存
            var buffer = new byte[len - size];
            var bytes2 = new ProtoByteArray(buffer);
            if (size < len)
            {
                _receiveBytes.Position = size;
                _receiveBytes.ReadBytes(bytes2.GetBuffer(), 0, (uint) (len - size));
            }

            readByteBuff(bytes);

            _receiveBytes = bytes2;

            if (size < len)
            {
                dataHandler();
            }
        }
    }

    /**
	 *byteBuff读取处理 
	 * 
	 */

    private void readByteBuff(ProtoByteArray bytes)
    {
        _waitForAck = false;

        LogWarning("ReadByteBuff " + bytes.bytesAvailable + " byte");
        TotalReceiveBytes += bytes.bytesAvailable;
        TotalReceiveCount += 1;

        // 根据当前业务，触发调度逻辑
        bytes.Position = 12;
        var type = (InstructionDefine) bytes.ReadShort();
        bytes.Position = 0;

        LogWarning("type=" + type);

        switch (type)
        {
            case InstructionDefine.SSL_OPEN_RES:
                LogWarning("[Receive SSL_OPEN_RES]");
                GameStopwatch.End(HA_SSL_OPEN_RES_TIMER);
                Log(GameStopwatch.DumpInfo(HA_SSL_OPEN_RES_TIMER));
                var sslopen = new SSLOpenResponseInstruction();
                sslopen.fromBytes(bytes);
                sslopen.execute(this);
                break;

            case InstructionDefine.VER:
                LogWarning("[Receive VER]");
                GameStopwatch.End(HA_VER_TIMER);
                Log(GameStopwatch.DumpInfo(HA_VER_TIMER));
                var ver = new VerAckInstruction();
                ver.fromBytes(bytes);
                ver.execute(this);
                break;

            case InstructionDefine.JOIN_RES:
                LogWarning("[Receive JOIN_RES]");
                GameStopwatch.End(HA_JOIN_RES_TIMER);
                Log(GameStopwatch.DumpInfo(HA_JOIN_RES_TIMER));
                var joinack = new JoinAckInstruction();
                joinack.fromBytes(bytes);
                joinack.execute(this);
                break;

            case InstructionDefine.LEAVE_EVENT:
                LogWarning("[Receive LEAVE_EVENT]");
                var leave = new LeaveEventInstruction();
                leave.fromBytes(bytes);
                leave.execute(this);
                break;

            case InstructionDefine.SERVICE_QUERY_RES:
                LogWarning("[Receive SERVICE_QUERY_RES]");
                var service = new ServiceQueryResponseInstruction();
                service.fromBytes(bytes);
                service.execute(this);
                break;

            case InstructionDefine.DIRECT_MESSAGE:
                var direct = new DirectMessageReceivedInstruction();
                direct.fromBytes(bytes);
                direct.execute(this);
                break;

            case InstructionDefine.GROUP_MESSAGE:
                var group = new GroupMessageReceivedInstruction();
                group.fromBytes(bytes);
                group.execute(this);
                break;

            case InstructionDefine.STATE_EVENT:
                LogWarning("[Receive STATE_EVENT]");
                var stateEventNofity = new StateEventNotifyInstruction();
                stateEventNofity.fromBytes(bytes);
                stateEventNofity.execute(this);
                break;

            case InstructionDefine.PING_ACK:
                LogWarning("[Receive PING_ACK]");
                break;
            case InstructionDefine.KEEPALIVE_ACK:
                LogWarning("[Receive KEEPALIVE_ACK]");
                break;
            case InstructionDefine.GET_PROPERTY_RES:
                LogWarning("[Receive GET_PROPERTY_RES]");
                break;
            case InstructionDefine.SET_PROPERTY_RES:
                LogWarning("[Receive SET_PROPERTY_RES]");
                break;

            default:
                LogWarning("not support instruction " + type);
                break;
        }
    }

    public void AddEventObj(EventObject obj)
    {
        if (_socket != null)
        {
            _eventObjQueue.Enqueue(obj);
        }
    }

    public void ClearEventObj()
    {
        if (_socket != null)
        {
            _eventObjQueue.Clear();
        }
    }

    #region HA调试信息

    /// <summary>
    ///     -1--不显示任何Log信息
    ///     0--只显示Error信息
    ///     1--Error,Warning
    ///     2--Error,Warning,Log
    /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE
    public static int ShowLogLevel = 0;
#else
	public static int ShowLogLevel = 0;
#endif

    private const string HA_SSL_OPEN_RES_TIMER = "HA_SSL_OPEN_RES";
    private const string HA_VER_TIMER = "HA_VER";
    private const string HA_JOIN_RES_TIMER = "HA_JOIN_RES";

    public static void LogWarning(object obj)
    {
        if (ShowLogLevel >= 2)
        {
            Debug.LogWarning(obj);
        }
    }

    public static void Log(object obj)
    {
        if (ShowLogLevel >= 1)
        {
            Debug.Log(obj);
        }
    }

    public static void LogError(object obj)
    {
        if (ShowLogLevel >= 0)
        {
            Debug.LogError(obj);
        }
    }
#if HandleCountMgrDebug
    public void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 25;
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(20f);
        GUILayout.Label(string.Format("数量{0} 时间{1} tick{2}", handleCountMgr._runCount, handleCountMgr._milliseconds, handleCountMgr._tick), guiStyle);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
#endif
    #endregion

    private class SendObject
    {
        internal readonly uint bufSize;
        internal readonly CallBack_Send callBack_Send;
		internal readonly ProtoByteArray sendBuffer;

		internal SendObject(ProtoByteArray _sendBuffer, uint _bufSize, CallBack_Send _callBack_Send)
        {
            sendBuffer = _sendBuffer;
            bufSize = _bufSize;
            callBack_Send = _callBack_Send;
        }
    }
    public class EventQueue : IEnumerable<EventObject>
    {
        private readonly Queue<EventObject> eventObjectsQueue = new Queue<EventObject>();

        public void Enqueue(EventObject eventObject)
        {
            lock (eventObjectsQueue)
            {
                eventObjectsQueue.Enqueue(eventObject);
            }
        }

        public EventObject Dequeue()
        {
            EventObject eventObject = null;
            lock (eventObjectsQueue)
            {
                eventObject = eventObjectsQueue.Dequeue();
            }
            return eventObject;
        }

        public void Clear()
        {
            lock (eventObjectsQueue)
            {
                eventObjectsQueue.Clear();
            }
        }

        public IEnumerator<EventObject> GetEnumerator()
        {
            return eventObjectsQueue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return eventObjectsQueue.GetEnumerator();
        }

        public int Count
        {
            get
            {
                lock (eventObjectsQueue)
                {
                    return eventObjectsQueue.Count;
                }
            }
        }


    }
    private class HandleCountMgr
    {
        private HaConnector master;
        private int defaultHandlecount;
        private long limitMilliseconds;
        private Stopwatch stopwatch;
#if HandleCountMgrDebug
        public int _runCount;
        public long _milliseconds;
        public long _tick;
#endif
        public HandleCountMgr(HaConnector haConnector)
        {
            master = haConnector;
            defaultHandlecount = DEFAULT_HANDLECOUNT;
            limitMilliseconds = 3;
            stopwatch = Stopwatch.StartNew();
        }

        public void Reset()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public bool CheckCanRun(int runCount)
        {
            bool canrun = stopwatch.ElapsedMilliseconds < limitMilliseconds || runCount < defaultHandlecount;
#if HandleCountMgrDebug
            if (canrun == false || true)
            {
                if (runCount != 0)
                {
                    _runCount = runCount;
                    _milliseconds = stopwatch.ElapsedMilliseconds;
                    _tick = stopwatch.ElapsedTicks;
                }
            }
#endif
            return canrun;
        }

    }
}