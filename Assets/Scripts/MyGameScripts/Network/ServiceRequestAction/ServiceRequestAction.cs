using System.Collections.Generic;
using AppDto;
using UnityEngine;

/**
 * 
 * @author senkay
 */

public class ServiceRequestAction
{
    #region Delegates

    public delegate void OnRequestError(ErrorResponse errorResponse);

    public delegate void OnRequestSuccess(GeneralResponse e);

    #endregion

    public enum RequestLockType
    {
        LockScreen, //锁屏
        LockAction, //锁请求，key为action
        LockActionAndParam, //锁请求，key为action+param
        NoLock //不锁屏
    }

    //记录请求锁的Key,防止某些接口短时间内多次发送请求
    private static readonly HashSet<string> _lockRequestKeySet = new HashSet<string>();

    public readonly GeneralRequest request;
//    public readonly byte requestType;//legacy 2016-12-30 10:12:54
    private string _lockKey;

    private OnRequestError _onError;
    private OnRequestSuccess _onSuccess;

    //客户端限制开启，可以在客户端业务检查条件上加上这个标记，通过这个开关切换是否做客户端过滤来达到方便测试服务器接口的目
    /* 示例用法如下：
    if (PassNameLenght(name) == false && ServerRequestCheck)
    {
        TipManager.AddTip("角色名字长度只能为2~5个文字");
        return;
    }
    else
    {
        requestServer(PlayerLoginService.propsRename(name), "", (e)=>{
            ModelManager.Player.UpdatePlayerName(name);
        });
    }

    另外，这个也控制了FunctionOpen的检查开关
    */
    public static bool ServerRequestCheck = true;

    //客户端请求延时开关，可以让客户端发出去的请求延后3秒才真正发出，用来调试不锁屏导致的业务操作异常
    public static bool ServerRequestDelay = false;

    //客户端锁请求开关，默认开启，针对需要返回的请求，相同类型和参数的请求需要等待有返回后才可以发出新的
    public static bool ServerRequestLock = true;

    //是否开启单机模式，即：是否直接返回结果而不等待实际请求服务端的返回
    public static bool SimulateNet =  false;

    private string _tip;

    public ServiceRequestAction(GeneralRequest request)
    {
        this.request = request;
//        this.requestType = requestType;
    }

    public long SendTimeStamp { get; private set; }

    private void ShowTips(string tip)
    {
        _tip = tip;

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(tip))
        {
            if (!GameSetting.Release)
            {
                RequestLoadingTip.Show(_tip, true);
            }
        }
#endif
    }

    private void HideTips()
    {
        _lockRequestKeySet.Remove(_lockKey);

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(_tip))
        {
            if (!GameSetting.Release)
            {
                RequestLoadingTip.Stop(_tip);
            }
        }
#endif
    }

    private void Send()
    {
        if (_onSuccess != null || _onError != null)
        {
            SendTimeStamp = SystemTimeManager.Instance.GetUTCTimeStamp();
            ServiceRequestActionMgr.Add(this);
            GameDebuger.TODO(" ModelManager.GameAnaly.StartRequestAction(request.action, request.serial);");
            SocketManager.Instance.SendRequest(request/**, requestType*/);
        }
        else
        {
            //无需请求回调的,将serial设置为-1
            request.serial = -1;
            SocketManager.Instance.SendRequest(request/**, requestType*/);
        }
    }

    /// <summary>
    /// 根据GeneralRequest的信息,生成唯一的key,作为请求锁
    /// </summary>
    /// <param name="request"></param>
    /// <param name="withParams"></param>
    /// <returns></returns>
    private static string GenerateRequestKey(GeneralRequest request, bool withParams)
    {
        string key = request.action;
        if (withParams)
        {
            for (int i = 0; i < request.xparams.Count; i++)
            {
                key += request.xparams[i];
            }
        }

        return key;
    }

    private static string AddRequestKey(GeneralRequest request, bool withParams)
    {
        string key = GenerateRequestKey(request, withParams);
        if (_lockRequestKeySet.Contains(key))
        {
            return "";
        }
        _lockRequestKeySet.Add(key);
        return key;
    }

    #region Callback Event

    public void onSuccess(GeneralResponse generalResponse)
    {
        HideTips();

        if (_onSuccess != null)
            _onSuccess(generalResponse);
    }

    public void onError(ErrorResponse errorResponse)
    {
        HideTips();

        if (_onError != null)
        {
            _onError(errorResponse);
        }
        else
        {
            TipManager.AddTopTip(errorResponse.message);
        }

        GameDebuger.Log(
            string.Format("ErrorResponse: action={0} error={1} id={2} serialId={3}", request.action, errorResponse.message,errorResponse.id,errorResponse.serialId)
                .WrapColorWithLog("yellow"));
    }

    public void onTimeout()
    {
        HideTips();

        GameDebuger.Log("onTimeout: action=" + request.action + " error=" + "请求超时");
        var errorResponse = new ErrorResponse();
        errorResponse.id = -1;
        errorResponse.message = "请求超时";

        if (_onError != null)
        {
            _onError(errorResponse);
        }
    }

    #endregion

    #region 请求服务器接口

    private static void requestServer(GeneralRequest request,
        RequestLockType lockType = RequestLockType.LockActionAndParam,
        OnRequestSuccess onSuccess = null,
        OnRequestError onRequestError = null)
    {
        if (ServerRequestLock == false || onSuccess == null)
        {
            lockType = RequestLockType.NoLock;
        }

        if (onSuccess != null || onRequestError != null)
        {
            request.needResponse = true;
        }
        else
        {
            request.needResponse = false;
        }

        var action = new ServiceRequestAction(request);

        if (onSuccess != null)
        {
            if (lockType == RequestLockType.LockAction || lockType == RequestLockType.LockActionAndParam ||
                lockType == RequestLockType.LockScreen)
            {
                string key = AddRequestKey(request, lockType != RequestLockType.LockAction);
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }
                action._lockKey = key;
            }
        }

        if (onSuccess != null)
            action._onSuccess = onSuccess;

        if (onRequestError != null)
            action._onError = onRequestError;

        string tip = "";

        if (lockType == RequestLockType.LockScreen)
        {
            tip = GenerateRequestKey(request, true);
        }

        action.ShowTips(tip);

        if (SocketManager.IsOnLink)
        {
            //延迟发送请求,模拟测试网络连接异常的情况
            if (ServerRequestDelay)
            {
                JSTimer.Instance.SetupCoolDown(GenerateRequestKey(action.request, true) + Random.Range(1, 9999999),
                    3f, null, action.Send);
            }
            else
            {
                action.Send();
            }
        }
        else
        {
            action.onTimeout();
        }
    }

    /// <summary>
    ///     Requests the server.
    ///     string tip != ""   并且  onSuccess != null     对应   LockScreen
    ///     string tip == ""   并且  onSuccess != null    对应  LockActionAndParam
    ///     onSuccess == null  对应  NoLock
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="tip">Tip.</param>
    /// <param name="onSuccess">On success.</param>
    /// <param name="onRequestError">On error.</param>
    public static void requestServer(GeneralRequest request, string tip = "",
        OnRequestSuccess onSuccess = null,
        OnRequestError onRequestError = null)
    {
        var lockType = RequestLockType.NoLock;
        if (string.IsNullOrEmpty(tip))
        {
            lockType = RequestLockType.LockActionAndParam;
        }
        else
        {
            lockType = RequestLockType.LockScreen;
        }
        requestServer(request, lockType, onSuccess, onRequestError);
    }

    //考虑SimulateSuccessCallBack开关，其开启时直接返回success的回掉，关闭时走正常通讯流程。
    public static void requestServerWithSimulate(GeneralRequest request, string tip = "",
        OnRequestSuccess onSuccess = null,
        OnRequestError onRequestError = null)
    {
        if (ServiceRequestAction.SimulateNet)
            onSuccess(null);
        else
            ServiceRequestAction.requestServer(request, tip, onSuccess, onRequestError);
    }
    #endregion
}