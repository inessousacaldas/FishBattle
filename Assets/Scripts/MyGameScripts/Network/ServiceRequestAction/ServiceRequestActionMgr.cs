using System;
using System.Collections.Generic;

public class ServiceRequestActionMgr
{
    private static Dictionary<int, ServiceRequestAction> _requestActionDic;
    private static int _mSerialNum = 1;

    public static void Setup()
    {
        _requestActionDic = new Dictionary<int, ServiceRequestAction>();
    }

    public static ServiceRequestAction Remove(int serial)
    {
        if (_requestActionDic == null || !_requestActionDic.ContainsKey(serial))
            return null;

        var requestAction = _requestActionDic[serial];
        _requestActionDic.Remove(serial);
        return requestAction;
    }

    public static void Add(ServiceRequestAction requestAction)
    {
        if (requestAction == null)
            return;

        //每次加入新请求时,清空一下过时的请求
        CheckOutTimeoutAction();

        int serialNum = _mSerialNum++;
        //防止serialNum溢出Int32.MaxValue
        if (serialNum == Int32.MaxValue)
        {
            _mSerialNum = 1;
        }

        requestAction.request.serial = serialNum;
        _requestActionDic[serialNum] = requestAction;
    }

    private static void CheckOutTimeoutAction()
    {
        //删除1分钟以前的保留请求
        long checkTime = SystemTimeManager.Instance.GetUTCTimeStamp() - 30000;
        var removeKeys = new List<int>();
        foreach (var item in _requestActionDic)
        {
            int serial = item.Key;
            var requestAction = item.Value;
            if (checkTime > requestAction.SendTimeStamp)
            {
                removeKeys.Add(serial);
				GameDebuger.LogError(string.Format("Remove TimeOut action={0} serial={1}，协议响应超时，如非断点请检查原因", requestAction.request.action, serial));
            }
        }

        if (removeKeys.Count > 0)
        {
            for (int i = 0; i < removeKeys.Count; i++)
            {
                _requestActionDic.Remove(removeKeys[i]);
            }
        }
    }

    public static void CleanAllCallback(bool withTimeout = true)
    {
		if (withTimeout)
		{
			CheckOutTimeoutAction();
		}

        if (_requestActionDic != null)
        {
            if (withTimeout)
            {
                foreach (var callBack in _requestActionDic.Values)
                {
                    callBack.onTimeout();
                }
            }

            _requestActionDic.Clear();
        }
    }

    public static void ResetSerialNum()
    {
        _mSerialNum = 1;
        GameDebuger.TODO(" ModelManager.GameAnaly.Dispose();");
    }

    public static void Dispose()
    {
        CleanAllCallback();
        ResetSerialNum();
    }
}