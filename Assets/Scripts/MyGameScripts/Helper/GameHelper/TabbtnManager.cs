// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BackpackWinUIController.cs
// Author   : fish
// Created  : 2016/11/5 
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;

public enum TabbtnPrefabPath
{
	TabBtnWidget,
    TabBtnWidget_H1,
    TabBtnWidget_H2,
    TabBtnWidget_H3_SHORT,
    TabBtnWidget_H4,
    TabBtnWidget_S1,
    TabBtnWidget_S2,
    TabBtnWidget_S3,
    ExpressionBtn,
    TabBtnWidget_ChatTab,
    TabBtnShop_2,
    TabBtnExchange,
    TabBtnBattle,
    TabBtnWidget_AssistCrew,
}

public interface ITabInfo
{
	string Name { get; }
	int EnumValue { get; }
}

class TabInfoData : ITabInfo
{
	private string _name;
	private int _enumValue;

	public static TabInfoData Create(int value, string name)
	{
		var t = new TabInfoData();
		t._name = name;
		t._enumValue = value;
		return t;
	}

	private TabInfoData()
	{
	}

	public string Name {
		get { return _name; }
	}
	public int EnumValue {
		get { return _enumValue; }
	}
}

public class TabbtnManager
{
    private int curIdx = -1;
    private Func<int, ITabBtnController> createCtrl;
    private CompositeDisposable _disposable = null;

    public IObservableExpand<int> Stream {
        get { return stream; }
    }
    private Subject<int> stream;
    private List<ITabBtnController> _tabBtnCtrlList = new List<ITabBtnController> ();

	public static TabbtnManager Create(
		IEnumerable<ITabInfo> info
	, Func<int, ITabBtnController> createCtrl
	, int idx = 0){
		var mgr = new TabbtnManager ();
        mgr._disposable = new CompositeDisposable();
        mgr.stream = new Subject<int>();
        mgr.stream.Hold(mgr.curIdx);
        mgr.UpdateTabs(info, createCtrl, idx);
		
		return mgr;
	}

	private TabbtnManager ( )
	{
		
	}

    public void UpdateTabs(
        IEnumerable<ITabInfo> info
        ,int idx)
    {
        UpdateTabs(info,createCtrl,idx);
    }


    public void UpdateTabs(
		IEnumerable<ITabInfo> info
		, Func<int, ITabBtnController> _createCtrl
		, int idx)
	{
		var cnt = 0;
        createCtrl = _createCtrl;

        info.ForEachI(delegate(ITabInfo tabInfo, int i)
		{
			var ctrl = this._tabBtnCtrlList.TryGetValue(i);
			if (ctrl == null)
			ctrl = GameUtil.SafeRun(cnt, createCtrl);
			if (ctrl != null)
			{
				ctrl.Show();
				ctrl.SetBtnLbl(tabInfo.Name);
				this.AddTabCtrl(ctrl);
				cnt++;
			}
		});

		for (int i = cnt, len= _tabBtnCtrlList.Count; i < len; i ++)
		{
			var ctrl = _tabBtnCtrlList.TryGetValue(i);
			if (ctrl != null) ctrl.Hide();
		}

		if (idx < cnt)
			SetTabBtn(idx);
	}

    public void SetBtnHide(int index)
    {
        var ctrl = this._tabBtnCtrlList.TryGetValue(index);
        if (ctrl != null)
            ctrl.Hide();
    }

	private void AddTabCtrl(ITabBtnController ctrl)
    {
        if (ctrl == null) return;
        var idx = _tabBtnCtrlList.IndexOf(ctrl);
        if (idx < 0)
        {
            _tabBtnCtrlList.Add(ctrl);
            idx = _tabBtnCtrlList.Count - 1;
        }
        _disposable.Add(ctrl.OnTabClick.Subscribe(_ =>
        {
            SetTabBtn(idx);
	        curIdx = idx;

            if (curIdx != stream.LastValue)
		        stream.OnNext(curIdx);
        }));
    }

    public int GetCurSelectedInx {
        get { return curIdx;}
    }
    public void SetTabBtn(int selectIndex)
    {
        if (_tabBtnCtrlList.Count <= selectIndex
        || (selectIndex == curIdx))
            return;

        stream.Hold(curIdx);
        curIdx = selectIndex;

		for (int i = 0; i < _tabBtnCtrlList.Count; ++i)
		{
			_tabBtnCtrlList[i].SetSelected(i == selectIndex);
		    _tabBtnCtrlList[i].SetBtnDepth(i == selectIndex ? 9 : 9 - i);     //选中状态深度最高,然后从左到右依次减少
		}
	}

    public void SetBtnLblFont(
        int selectSize = 22,
        string selectColor = ColorConstantV3.Color_VerticalSelectColor_Str,
        int normalSize = 20,
        string normalColor = ColorConstantV3.Color_VerticalUnSelectColor_Str)
    {
        _tabBtnCtrlList.ForEach(btn =>
        {
            btn.SetBtnLblFont(selectSize, selectColor, normalSize, normalColor);
        });
    }


    public void Dispose()
	{
        createCtrl = null;
        _disposable.Dispose();
	    _tabBtnCtrlList.Clear ();
        
	}
}
	