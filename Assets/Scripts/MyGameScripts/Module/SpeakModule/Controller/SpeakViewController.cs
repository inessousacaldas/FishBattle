// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SpeakViewController.cs
// Author   : DM-PC092
// Created  : 7/21/2017 10:17:48 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SpeakViewData : ISpeakData
{
    private string _message;

    public string GetMessage { get { return _message; }    }

    public static SpeakViewData Create(string message)
    {
        SpeakViewData data = new SpeakViewData();
        data._message = message;
        return data;
    }
}

public interface ISpeakData
{
    string GetMessage { get; }
}

public partial interface ISpeakViewController
{

    string Message { get; }

    void AddHistoryMsg();

    void SendMessage();

    void SetHistoryPanelState();
}
public partial class SpeakViewController
{
    private static List<string> HistoryMsg = new List<string>();
    private static List<string> DefaultMsg = new List<string>();

    private int _tempHisCnt = 0;

    private string _tempMessaga = "";
    private string _channelStr = "";
    private static CompositeDisposable _disposable;

    private ISpeakData _data;
    private static Action<ISpeakData> _callback;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        DefaultMsg.Add("路况良好，快上车！");
        DefaultMsg.Add("来人啦(≧▽≦)/~！！！");
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        _view.Input_UIInput.value = "";
        _view.Input_UIInput.savedAs = "";
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    UICamera.onClick += OnCameraClick;
	}

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnCameraClick;
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ISpeakViewData data){

    }

    public void SendMessage()
    {
        if (!View.GuideChannel_UIToggle.value&&!View.TeamChannel_UIToggle.value)
        {
            TipManager.AddTip("至少选择一个喊话频道");
            return;
        }
        if (string.IsNullOrEmpty(View.Input_UIInput.value))
        {
            var i = UnityEngine.Random.Range(0, 2);
            _tempMessaga = DefaultMsg[i];
        }
        else
            _tempMessaga = View.Input_UIInput.value;

        HistoryMsg.Add(_tempMessaga);
        View.Input_UIInput.value = "";
        AddHistoryMsg();
        if (View.TeamChannel_UIToggle.value)
        {
            _channelStr = string.Format(_channelStr + (int) SpeakChannel.team + ",");
        }
        if (View.GuideChannel_UIToggle.value)
        {
            _channelStr = string.Format(_channelStr + (int)SpeakChannel.guide + ",");
        }
        
        TeamDataMgr.TeamNetMsg.SpeakOut(_channelStr,_tempMessaga, () => { ProxySpeakViewModule.Close(); });
    }

    public void AddHistoryMsg()
    {
        if (HistoryMsg.Count == 0)
            return;

        for (int i = _tempHisCnt; i < HistoryMsg.Count; i++)
        {
            var HisItem = AddChild<HistoryItemController, HistoryItemPre>(
                       View.HistoryTable_UITable.gameObject,
                       HistoryItemPre.NAME,
                        HistoryItemPre.NAME);
            HisItem.View.Label_UILabel.text = HistoryMsg[i];
            _disposable.Add(HisItem.View.HistoryItemPre_UIDragScrollView.gameObject.OnClickAsObservable().Subscribe(_ =>
            {
                View.Input_UIInput.value = HisItem.View.Label_UILabel.text;
                _view.HistoryPanel_UIPanel.gameObject.SetActive(false);
            }));
        }

        _tempHisCnt = HistoryMsg.Count;
        View.HistoryTable_UITable.Reposition();
    }

    void Update()
    {
        if (_view.Input_UIInput.value != string.Empty)
        {
            _view.Residue_UILabel.text = string.Format("剩余字数: {0}",
                _view.Input_UIInput.characterLimit - _view.Input_UIInput.value.Length);
        }
        else
            _view.Residue_UILabel.text = string.Format("剩余字数: {0}", 30);
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.HistoryPanel_UIPanel.gameObject.activeSelf
            && go.name != _view.HistoryBtn_UIButton.gameObject.name
            && panel != _view.HistoryPanel_UIPanel
            && panel != _view.ScrollView_UIScrollView.panel)
            _view.HistoryPanel_UIPanel.gameObject.SetActive(false);
    }

    public void SetHistoryPanelState()
    {
        View.HistoryPanel_UIPanel.gameObject.SetActive(!_view.HistoryPanel_UIPanel.gameObject.activeSelf);
    }

    public string Message
    {
        get { return _tempMessaga; }
    }

}
