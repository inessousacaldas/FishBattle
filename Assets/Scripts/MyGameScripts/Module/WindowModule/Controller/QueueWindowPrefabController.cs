using System;
using UnityEngine;

public class QueueWindowPrefabController : MonoViewController<QueueWindowPrefab>
{

    public void Open(string serverName, int queuePosition, int waitTime)
    {
        

        UpdateData(serverName, queuePosition, waitTime);
    }

    public void UpdateData(string serverName, int queuePosition, int waitTime)
    {
        View.ServerName_UILabel.text = serverName;
        View.QueuePositionLbl_UILabel.text = string.Format("队列位置:{0}", queuePosition);
        TimeSpan span = TimeSpan.FromSeconds(waitTime);

        string txt = "";
        if (span.Days != 0)
        {
            txt += span.Days + "天";
        }
        if (span.Hours != 0)
        {
            txt += span.Hours + "小时";
        }
        if (span.Minutes != 0)
        {
            txt += span.Minutes + "分";
        }
        txt += span.Seconds + "秒";
//        Debug.LogError(span.Days + "," + span.Hours + ","+ span.Minutes + "," + span.Seconds);
        View.WaitTimeLbl_UILabel.text = string.Format("预计等待时间:{0}", txt);
    }


    protected override void RegistCustomEvent()
    {
        base.RegistCustomEvent();
        EventDelegate.Set(View.ChangeServerBtn_UIButton.onClick, OnChangeServerBtnClick);
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnCloseBtnClick);
    }

    private System.Action _loginAction;

    public void SetCloseTime(float time, System.Action finishCallBack)
    {
        View.QueuePositionLbl_UILabel.text = "请在倒计时结束前进入游戏";
        View.ChangeServerBtnNameLbl_UILabel.text = "进入游戏";
        _loginAction = finishCallBack;
        EventDelegate.Set(View.ChangeServerBtn_UIButton.onClick, Login);
        AddOrResetCDTask("CloseLoginQueueWindow", time, UpdateCloseTimeOfView, OnCloseBtnClick);
    }

    private void Login()
    {
        if (_loginAction != null)
        {
            _loginAction();
            ProxyWindowModule.CloseQueueWindow();
        }        
    }

    private void UpdateCloseTimeOfView(float time)
    { 
        TimeSpan span = TimeSpan.FromSeconds(time);
        string txt = "";
        if (span.Days != 0)
        {
            txt += span.Days + "天";
        }
        if (span.Hours != 0)
        {
            txt += span.Hours + "小时";
        }
        if (span.Minutes != 0)
        {
            txt += span.Minutes + "分";
        }
        txt += span.Seconds + "秒";
//        Debug.LogError(span.Days + "," + span.Hours + "," + span.Minutes + "," + span.Seconds);
        if (View != null)
        {
            View.WaitTimeLbl_UILabel.text = "倒计时:" + txt;
        }
    }

    private void CloseTimeFinish()
    {
        
    }

    private void OnChangeServerBtnClick()
    {
        OnCloseBtnClick();
    }

    private void OnCloseBtnClick()
    {
        ExitGameScript.Instance.HanderRelogin();
        ProxyWindowModule.CloseQueueWindow();
    }
}
