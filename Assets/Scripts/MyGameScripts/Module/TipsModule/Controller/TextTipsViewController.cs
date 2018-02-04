// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TextTipsViewController.cs
// Author   : xjd
// Created  : 12/18/2017 10:39:47 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using AppDto;

public partial interface ITextTipsViewController
{
    void UpdateText(int id, Vector3 pos, bool isUp = false, params string[] strParam);
}

public partial class TextTipsViewController    {

    public static ITextTipsViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, ITextTipsViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as ITextTipsViewController;
            
        return controller;        
    }

    private JSTimer.TimerTask _textTipsViewTimer;
    private Dictionary<int, TextTips> _textTipsDic = DataCache.getDicByCls<TextTips>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        //5秒自动关闭
        //_textTipsViewTimer = JSTimer.Instance.SetupTimer("TextTipsView", () =>
        //{
        //    Close();
        //}, 5f, false);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;   
    }

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnClose;
    }
        
    protected override void OnDispose()
    {
        if(_textTipsViewTimer != null)
        {
            _textTipsViewTimer.Cancel();
            _textTipsViewTimer = null;
        }
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    public void UpdateText(int id, Vector3 pos, bool isUp=false, params string[] strParam)
    {
        if (!_textTipsDic.ContainsKey(id))
        {
            GameDebuger.Log("textTips表没有这个id");
            Close();
            return;
        }

        View.Text_UILabel.text = string.Format(_textTipsDic[id].content, strParam);
        //文字左右与背景预留宽度 24
        View.Bg_UISprite.width = View.Text_UILabel.width + 24;
        //文字上下与背景预留宽度 14
        View.Bg_UISprite.height = View.Text_UILabel.height + 14;
        View.Text_UILabel.transform.localPosition = new Vector3(-View.Bg_UISprite.width / 2 + 12, View.Text_UILabel.transform.localPosition.y);
        View.Bg_UISprite.transform.localPosition = new Vector3(View.Bg_UISprite.transform.localPosition.x, View.Bg_UISprite.height/2);

        if (isUp)
        {
            View.Text_UILabel.pivot = UIWidget.Pivot.BottomLeft;
            View.Bg_UISprite.pivot = UIWidget.Pivot.Bottom;
            View.Text_UILabel.transform.localPosition = new Vector3(-View.Bg_UISprite.width / 2 + 12, 2);
            View.Bg_UISprite.GetComponent<BoxCollider>().center = new Vector3(0, View.Bg_UISprite.height / 2);
        }
        else
        {
            View.Text_UILabel.pivot = UIWidget.Pivot.TopLeft;
            View.Bg_UISprite.pivot = UIWidget.Pivot.Top;
            View.Text_UILabel.transform.localPosition = new Vector3(-View.Bg_UISprite.width / 2 + 12, View.Text_UILabel.transform.localPosition.y);
            View.Bg_UISprite.GetComponent<BoxCollider>().center = new Vector3(0, -View.Bg_UISprite.height / 2);
        }

        //设定位置导致Tips部分超出屏幕 todo xjd
        //var panel = GetComponent<UIPanel>();
        //if (pos.x - View.Bg_UISprite.width / 2 < -panel.width / 2)
        //    pos.x = -panel.width / 2 + View.Bg_UISprite.width / 2;

        //if (pos.x > panel.width / 2)
        //    pos.x = panel.width / 2 - View.Bg_UISprite.width / 2;

        //if (pos.y > panel.height / 2)
        //    pos.y = panel.height / 2;

        //if (pos.y - View.Bg_UISprite.height < -panel.height / 2)
        //    pos.y = View.Bg_UISprite.height;

        View.Bg_UISprite.transform.localPosition = pos;
    }

    public void OnClose(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
            Close();
    }

    private void Close()
    {
        UIModuleManager.Instance.CloseModule(TextTipsView.NAME);
    }
}
