using System;
using UnityEngine;
using System.Collections;

public class QRCodeEnsureViewController : MonoViewController<QRCodeEnsureView>
{
    private Action _closeCallback;
    private string _sid;

    public void SetData(Action closeCallback, string sid)
    {
        _closeCallback = closeCallback;
        _sid = sid;

        
    }

    protected override void AfterInitView ()
    {
        View.TipLabel_UILabel.text = string.Format("{0}电脑微端登录", GameSetting.GameName);
       
    }

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent();
        EventDelegate.Set(View.ReturnBtn_UIButton.onClick, OnReturnBtnClick);
        EventDelegate.Set(View.EnsureBtn_UIButton.onClick, OnEnsureBtnClick);
    }


    private void OnEnsureBtnClick()
    {
        ServiceProviderManager.RequestQRCodeEnsureLogin(_sid, WinGameSetting.Data.ToBase64UrlSafeJson(), dto =>
        {
            if (View == null || dto == null)
            {
                return;
            }

            if (dto.code == 0)
            {
                ProxyWindowModule.OpenMessageWindow("登录成功");
                OnReturnBtnClick();
            }
            else if (dto.code == 1103)
            {
                ProxyWindowModule.OpenMessageWindow("二维码会话(sid)失效");
                OnReturnBtnClick();
            }
            else if (dto.code == 1104)
            {
                ProxyWindowModule.OpenMessageWindow("账号登录会话(token)失效");
                OnReturnBtnClick();
            }
            else
            {
                ProxyWindowModule.OpenMessageWindow("登录失败");
                OnReturnBtnClick();
            }
        });

    }

    private void OnReturnBtnClick()
    {
        ProxyQRCodeModule.CloseQRCodeEnsureView();
        if (_closeCallback != null)
        {
            _closeCallback();
            _closeCallback = null;
        }
    }
}
