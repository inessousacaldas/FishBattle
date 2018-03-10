/**
 *Socket通讯管理
 * @author senkay
 *
 */

using UnityEngine;
using AppDto;
using System;
using System.Collections.Generic;

public class SocketManager
{
	private static readonly SocketManager _instance = new SocketManager ();
	
	public static SocketManager Instance {
		get {
			return _instance;
		}
	}

	public event Action OnHAConnected;
	public event Action<string> OnHaError;
	public event Action<uint> OnHaCloseed;
	public event Action OnLeaveEvent;
	public event Action<uint> OnStateEvent;

    public static readonly string ERROR_time_out = "链接超时";
	public static readonly string ERROR_socket_error = "网络错误";
	public static readonly string ERROR_socket_close = "网络已断开";
	public static readonly string ERROR_sid_error = "用户账号错误";
	public static readonly string ERROR_user_invalid = "用户无效";

	private static int MAX_GAME_HEARTBEATTIME = 120;
	private static int MAX_CONNECT_HEARTBEATTIME = 120;
	
	private const string HA_SESSION_KEY = "UTzpWM8jZdgHFgN8";
	
	private int _checkMaxHearbeaTime = MAX_CONNECT_HEARTBEATTIME;

	public static bool IsOnLink = false;

	private GameObject _haConnectorGO = null;
	private HaConnector _haConnector  = null;
	
	private int _heartBeatTime = 0;
	
	private string _appServerId="";
	private string _appServerVer="";
	private ServiceInfo _currentServiceInfo;
	private GameServerInfo _currentServerInfo;
	
	private Dictionary<string, List<MessageProcessor>> _processorMaps;

	public SocketManager()
	{
		_processorMaps = new Dictionary<string, List<MessageProcessor>>();
	}

	private void OnTimerFinish()
	{
		if (IsOnLink == false && _checkMaxHearbeaTime == MAX_GAME_HEARTBEATTIME)
		{
			_heartBeatTime = 0;
			JSTimer.Instance.CancelCd("SocketTimeoutCheckTimer");
			return;
		}

		_heartBeatTime += 1;
		if (_heartBeatTime > _checkMaxHearbeaTime)
		{
			GameDebuger.Log("onTimer to CloseSocket");
			
			JSTimer.Instance.CancelCd("SocketTimeoutCheckTimer");

			if (_checkMaxHearbeaTime == MAX_GAME_HEARTBEATTIME)
			{
				Close(false);
			}
			else
			{
				Close(true);
			}
		}
		else
		{
			JSTimer.Instance.SetupCoolDown("SocketTimeoutCheckTimer",1f,null,OnTimerFinish,1f);
		}
	}

	public void Setup()
	{
		//CoolDownManager.Instance.SetupCoolDown("SocketTimeoutCheckTimer",1f,null,OnTimerFinish,1f);

		_heartBeatTime = 0;
		_checkMaxHearbeaTime = MAX_CONNECT_HEARTBEATTIME;

		_appServerVer = GameSetting.HA_SERVICE_MAIN_TYPE;

		if (_haConnectorGO == null)
		{
			_haConnectorGO = new GameObject("HaConnector");
			GameObject.DontDestroyOnLoad( _haConnectorGO );
		}
	}

	void HandleOnLeaveEvent (uint state, string reason)
	{
		GameDebuger.Log(string.Format("HandleOnLeaveEvent state={0} reason={1}", state, reason));

		LoginManager.LeaveState = state;

		RemoveHaConnector();

		if (OnLeaveEvent != null)
		{
			OnLeaveEvent();
		}
	}

	void HandleOnStateEvent(uint state)
	{
		GameDebuger.Log("HandleOnStateEvent status = " + state);

		if (OnStateEvent != null)
		{
			OnStateEvent(state);
		}
	}

	void HandleOnCloseEvent (uint status)
	{
		LoginManager.CloseState = status;

		GameDebuger.Log("HandleOnCloseEvent status = " + status);

		RemoveHaConnector();

		if (OnHaCloseed != null)
		{
			OnHaCloseed(status);
		}
	}
	
	void HandleOnMessageEvent (ByteArray byteArray,bool compress)
	{
        _heartBeatTime = 0;

	    //ZipLibUtils.AllMessageCount++;
        object readObj = JsHelper.ParseProtoObj(byteArray, compress);
        OnReceiveDto(readObj, byteArray.Length);
    }

    void HandleOnJoinEvent ()
	{
		_heartBeatTime = 0;
		_checkMaxHearbeaTime = MAX_GAME_HEARTBEATTIME;
		//CoolDownManager.Instance.SetupCoolDown("SocketTimeoutCheckTimer",1f,null,OnTimerFinish,1f);

		_haConnector.QueryServices(_appServerVer);
	}
	
	void HandleOnServiceEvent (ServiceInfo selectInfo)
	{
		if (selectInfo != null)
		{
			SetCurrentAppServerInfo(selectInfo);
		}
		else
		{
			Debug.Log("找不到服务器 " + _currentServerInfo.GetServerUID());
			_haConnector.Close();
			HandlerHaError(ERROR_socket_close);
		}
	}
	
	private void SetCurrentAppServerInfo(ServiceInfo serviceInfo)
	{
		_currentServiceInfo = serviceInfo;
		_appServerId = serviceInfo.id;
		IsOnLink = true;
		
		if (OnHAConnected != null)
		{
			OnHAConnected();
		}
	}
	
	private void HandlerHaError(string msg)
	{
		GameDebuger.Log("HandlerHaError " + msg);

		if (OnHaError != null)
		{
			OnHaError(msg);
		}
	}

	public void Destroy()
	{
		JSTimer.Instance.CancelCd("SocketTimeoutCheckTimer");

		ServiceRequestActionMgr.ResetSerialNum();
	}
	
	public void Connect( GameServerInfo serverInfo)
	{
		_currentServerInfo = serverInfo;

		//CoolDownManager.Instance.SetupCoolDown("SocketTimeoutCheckTimer",1f,null,OnTimerFinish,1f);
		_heartBeatTime = 0;
		_checkMaxHearbeaTime = MAX_CONNECT_HEARTBEATTIME;

		HaApplicationContext.getConfiguration().setHost(serverInfo.host);
		HaApplicationContext.getConfiguration().setPort(serverInfo.port);
	
		HaApplicationContext.getConfiguration().SetSessionKey(HA_SESSION_KEY);
		
		SetupHAConnecter();

		_haConnector.Join(serverInfo.haVer, GameSetting.HA_TRY_PORTS, serverInfo.serviceId);
	}

	private void SetupHAConnecter()
	{
		if (_haConnector == null)
		{
			Debug.Log("SetupHAConnecter");

			_haConnector = _haConnectorGO.GetMissingComponent<HaConnector>();
			_haConnector.OnJoinEvent += HandleOnJoinEvent;
			_haConnector.OnServiceEvent += HandleOnServiceEvent;
			_haConnector.OnMessageEvent += HandleOnMessageEvent;
			_haConnector.OnCloseEvent += HandleOnCloseEvent;
			_haConnector.OnLeaveEvent += HandleOnLeaveEvent;
			_haConnector.OnStateEvent += HandleOnStateEvent;
		}	
	}

	public bool IsSetup()
	{
		return _haConnector != null;
	}

	public HaConnector GetHaConnector()
	{
		return _haConnector;
	}
	
	public void SendRequest(GeneralRequest request/**, byte requestType*/)
	{
		if(!IsJoinedToHA()) {
			GameDebuger.Log("have not Joined to ha yet!!");
			return;
		}
		if(request == null) {
			GameDebuger.Log("IhomeRequest is null!!");
			return;
		}
		
		ProtoByteArray ba = new ProtoByteArray();
		
		if (!IsSkipAction(request))
		{
            GameDebuger.Log( ("[SEND] " + GetRequestDebugInfo(request/**, requestType*/)).WrapColorWithLog() );
		}

        //		ba.WriteByte( requestType );
        JsHelper.EncodeProtoObj(ba, request);

		_haConnector.SendMessage(_appServerId, 0, ba);
        if(GMDataMgr.isOpenDtoConn)
        {
            GMDataMgr.DataMgr.DtoConnData.AddSendDto(new GMDtoConnVO() { dto = request,size = (int)ba.Length,stackTrace = GameDebuger.GetStackTrace("") });
        }
    }

	private string GetRequestDebugInfo(GeneralRequest request/**, byte requestType*/)
	{
		string info = string.Format("action={0} serial={1} needResponse={2}", request.action, request.serial/**, requestType*/, request.needResponse);
        for (int i = 0; i < request.xparams.Count; i++)
        {
            info += " " + request.xparams[i];
        }
		return info;
	}

	private bool IsSkipAction(GeneralRequest request)
	{
        if (request.action.Contains("PlanWalk") || request.action.Contains("VerifyWalk"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsConnectedToHA()
	{
		return _haConnector != null && _haConnector.getState() >= HaStage.CONNECTED;
	}

	public bool IsJoinedToHA()
	{
		return _haConnector != null && _haConnector.getState() >= HaStage.JOINED;
	}

	public void AddMessageProcessor( MessageProcessor processor)
	{
		string type = processor.getEventType();
		
		List<MessageProcessor> handlerList;
		if (_processorMaps.ContainsKey(type)){
			handlerList = _processorMaps[type];
		}else{
			handlerList = new List<MessageProcessor>();
			_processorMaps.Add(processor.getEventType(), handlerList);
		}
        if(!handlerList.Contains(processor))        
            handlerList.Add(processor);

    }
	
	public void RemoveMessageProcessor( MessageProcessor processor)
	{
		string type = processor.getEventType();
		
		if (_processorMaps.ContainsKey(type)){
			List<MessageProcessor> list = _processorMaps[type];
			list.Remove(processor);
			processor.Dispose();
			processor = null;
		}
	}

    private void OnReceiveDto(object message, int len)
	{
	    if (message == null)
	    {
	        GameDebuger.LogError("XSocket onData = null");
	        return;
	    }

	    string messageType = message.GetType().FullName;

        var generalResponse = message as GeneralResponse;

		if (GameSetting.LogType == GameSetting.DebugInfoType.Verbose)
		{
			if (generalResponse != null)
			{
				GameDebuger.Log(
					string.Format("[RECEIVE] type:{0} len:{1} serial:{2}", messageType, len, generalResponse.serial).WrapColorWithLog());	
			}
			else
            {
				GameDebuger.Log(
					string.Format("[RECEIVE] type:{0} len:{1}", messageType, len).WrapColorWithLog());
			}
		}

        //ServiceRequestAction返回的Dto
        if (generalResponse != null && generalResponse.serial != 0)
	    {
            int serial = generalResponse.serial;
            GameDebuger.TODO(" ModelManager.GameAnaly.ReturnRequestAction(serial);");

            var requestAction = ServiceRequestActionMgr.Remove(serial);
            var errorResponse = generalResponse as ErrorResponse;
            if (requestAction != null)
            {

                if (errorResponse != null)
                {
                    requestAction.onError(errorResponse);
                }
                else
                {
                    requestAction.onSuccess(generalResponse);
                }
            }
            else
            {
                if (errorResponse != null)
                {
                    GameDebuger.Log(string.Format("ErrorResponse serial:{0},id:{1},serialId:{2},message:{3} " , 
                        errorResponse.serial,errorResponse.id,errorResponse.serialId,errorResponse.message));
                    TipManager.AddTip(errorResponse.message);
                }
                else
                {
                    GameDebuger.LogError("未监听响应协议(Callback) " + messageType + " ,serial id = " + serial);
                }
            }
        }else
        {
            //Notify
            GameDebuger.TODO("ModelManager.GameAnaly.AddNotifyAnaly(message);");

            List<MessageProcessor> processors = null;
            _processorMaps.TryGetValue(messageType,out processors);
            if(processors != null)
            {
                for(int i = 0;i < processors.Count;i++)
                {
                    var processor = processors[i];
                    processor.ProcessMsg(message);
                }
            }
            else
            {
                GameDebuger.LogWarning("未监听下发协议(Notify) " + messageType);
            }
        }
        if(GMDataMgr.isOpenDtoConn) {
            GMDataMgr.DataMgr.DtoConnData.AddSendDto(new GMDtoConnVO() { dto = message,size = len,stackTrace = GameDebuger.GetStackTrace("") });
        }
    }

	private void RemoveHaConnector()
	{
		SocketManager.IsOnLink = false;

		ServiceRequestActionMgr.Dispose();
		RequestLoadingTip.Reset();

		if (_haConnector != null)
		{
			Debug.Log("RemoveComponent HaConnector");

			_haConnector.OnJoinEvent -= HandleOnJoinEvent;
			_haConnector.OnServiceEvent -= HandleOnServiceEvent;
			_haConnector.OnMessageEvent -= HandleOnMessageEvent;
			_haConnector.OnCloseEvent -= HandleOnCloseEvent;
			_haConnector.OnLeaveEvent -= HandleOnLeaveEvent;
			_haConnector.OnStateEvent -= HandleOnStateEvent;

			_haConnector.transform.RemoveComponent<HaConnector>();
			_haConnector = null;
		}
	}

	public void Close( bool needDispatcher = false )
	{
		if (_haConnector != null)
		{
			_haConnector.CloseSocket(EventObject.Close_status_unkonwn);

//			_haConnector.Close();
//
//			ActionCallbackManager.CleanAllCallback();
//			RequestLoadingTip.Reset();
//			
//			ResetSerialNum();
//			//IsOnLink = false;
//
//			if ( needDispatcher )
//			{
//				if (OnHaCloseed != null)
//				{
//					OnHaCloseed(EventObject.Close_status_unkonwn);
//				}
//			}
		}
	}
}
