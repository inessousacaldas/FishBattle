// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPointController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IRedPointController
{
    void Create(RedPointType evt, bool isShowNum);

    void Create(RedPointType[] evtArr, bool isShowNum);
}

public partial class RedPointController
{
    private RedPointType[] redPointEvtArr = null;
    private bool isShowNum = false;

    private IDisposable _disposable;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    public void Create(
        RedPointType evt
        , bool isShowNum = false){
        var arr = new RedPointType[1]{ evt}; 
        Create(arr, isShowNum);
    }
    public void Create(
        RedPointType[] evtArr
        , bool isShowNum = false){

        this.redPointEvtArr = evtArr;
        this.isShowNum = isShowNum;

        var data = RedPointDataMgr.Stream.LastValue;
        UpdateView (data);
        _disposable = RedPointDataMgr.Stream.Subscribe(UpdateView);
    }

    private void UpdateView(IRedPointData data){
        if (data == null)
        {
            Hide();
            return;
        }

        var info = RedPointDataMgr.Stream.LastValue.GetMergeData (redPointEvtArr);
        redPointEvtArr.ForEach (s=>GameLog.LogRedPoint("evtArr elememt = "+ s.ToString()));
        info.Print("AfterInitView data sample---");
        
        if (info == null || !info.IsShow()) {
            Hide();
        } else {
            Show();
            _view.Num_UILabel.gameObject.SetActive (isShowNum);
            _view.Num_UILabel.text = info.num.ToString ();
        }
    }

    protected override void OnDispose ()
    {
        _disposable = _disposable.DisposeNotNull();
    }

}
