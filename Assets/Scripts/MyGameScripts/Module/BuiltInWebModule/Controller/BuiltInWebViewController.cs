using UnityEngine;
using System.Collections;
using System;

public class BuiltInWebViewController : MonoViewController<BuiltInWebView>
{
    private UniWebView _uniWebView;


    public void Open(string url)
    {
        GameDebuger.Log("GameSpirit URL:" + url);
        _uniWebView = UniWebViewExtHelper.CreateUniWebView(View.subview_UISprite, url);
        if (_uniWebView != null)
        {
            _uniWebView.OnLoadComplete += uniWebViewCompleteHandler;
            _uniWebView.OnReceivedMessage  += _uniWebView_OnReceivedMessage;
            _uniWebView.zoomEnable = true;
            _uniWebView.SetUseWideViewPort(true);
            _uniWebView.SetUseLoadWithOverviewMode(true);
            _uniWebView.Load();
        }
    }

    void _uniWebView_OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
    {
        GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.BuiltInWebView_OnReceivedMessage, message);");
    }

    private void uniWebViewCompleteHandler(UniWebView view, bool b, string message)
    {
        if (!b)
        {
            GameDebuger.Log(message);
        }
        if (_uniWebView != null)
        {
            _uniWebView.Show();
        }
    }

    override protected void RegistCustomEvent()
    {
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnClickCloseButton);
        EventDelegate.Set(View.RefreshBtn_UIButton.onClick, OnRefreshBtnClick);
        EventDelegate.Set(View.GoBackBtn_UIButton.onClick, OnGoBackBtnClick);
        EventDelegate.Set(View.GoForwardBtn_UIButton.onClick, OnGoForwardBtnClick);
    }

    private void OnRefreshBtnClick()
    {
        _uniWebView.Reload();
    }
    private void OnGoBackBtnClick()
    {
        _uniWebView.GoBack();
    }
    private void OnGoForwardBtnClick()
    {
        _uniWebView.GoForward();
    }

    private void OnClickCloseButton()
    {
        ProxyBuiltInWebModule.Close();
    }

    override protected void OnDispose()
    {
        if (_uniWebView != null)
        {
            _uniWebView.OnLoadComplete -= uniWebViewCompleteHandler;
            _uniWebView.OnReceivedMessage  -= _uniWebView_OnReceivedMessage;
        }
        _uniWebView = null;
    }
}
