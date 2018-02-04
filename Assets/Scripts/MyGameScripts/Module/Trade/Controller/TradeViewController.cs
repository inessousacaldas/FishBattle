// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TradeViewController.cs
// Author   : CL-PC007
// Created  : 7/1/2017 10:17:33 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface ITradeViewController
{
	IPitchViewController GetPitchCtrl { get; }
    ICmomerceViewController GetCmomerceCtrl { get; }
    TabbtnManager GetTabMgr { get; }

	void UpdateTabPage(TradeTab page);
}

public partial class TradeViewController
{
	private IPitchViewController _pitchCtrl;
    private ICmomerceViewController _cmomerceCtrl;
    public IPitchViewController GetPitchCtrl {get { return _pitchCtrl; }}
    public ICmomerceViewController GetCmomerceCtrl { get { return _cmomerceCtrl; } }

    private ITradeData _data;

    private TabbtnManager tabMgr;
	public TabbtnManager GetTabMgr { get { return tabMgr; } }
	
	private readonly ITabInfo[] TradeTabInfos =
	{
		TabInfoData.Create((int)TradeTab.Cmomerce, "商会"),
		TabInfoData.Create((int)TradeTab.Pitch, "摆摊"),
		//TabInfoData.Create((int)TradeTab.Auction, "拍卖"),
		//TabInfoData.Create((int)TradeTab.Sales,"寄售")
	};

	// 界面初始化完成之后的一些后续初始化工作
	protected override void AfterInitView ()
	{
		InitTab();
        InitCmomerceView();
        InitPitchView();
        UpdateTabPage(TradeTab.Cmomerce);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
	{
	
	}

	protected override void RemoveCustomEvent ()
	{
	}
	
	protected override void OnDispose()
	{
		base.OnDispose();
	}

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
		
	}

	// 业务逻辑数据刷新
	protected override void UpdateDataAndView(ITradeData data)
	{
	    _data = data;
        switch (data.CurTab)
	    {
            case TradeTab.Cmomerce:
                _cmomerceCtrl.UpdateDataAndView(data);
                break;
            case TradeTab.Pitch:
	            _pitchCtrl.UpdateDataAndView(data);
                break;
            case TradeTab.Auction:
	            break;
            case TradeTab.Sales:
	            break;
	    }
	}

	private void InitTab()
	{
		tabMgr = TabbtnManager.Create(TradeTabInfos, GetFunc());
	}
	
	public Func<int, ITabBtnController> GetFunc()
	{
		Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
			_view.TabGrid_UIGrid.gameObject
			, TabbtnPrefabPath.TabBtnWidget.ToString()
			, "Tabbtn_" + i);

		_view.TabGrid_UIGrid.Reposition();
		return func;
	}
	
	private void InitPitchView()
	{
		_pitchCtrl = AddChild<PitchViewController, PitchView>(_view.PitchGroup.gameObject, PitchView.NAME);
	}

    private void InitCmomerceView()
    {
        _cmomerceCtrl = AddChild<CmomerceViewController, CmomerceView>(_view.CmomerceGroup.gameObject, CmomerceView.NAME);
    }


    public void UpdateTabPage(TradeTab page)
	{
		HideAllGroup();
		
		switch (page)
		{
			case TradeTab.Cmomerce:
				_view.CmomerceGroup.SetActive(true);
                _cmomerceCtrl.UpdateDataAndView(_data);
                break;
			case TradeTab.Pitch:
				_view.PitchGroup.SetActive(true);
				break;
			case TradeTab.Auction:
				_view.AuctionGroup.SetActive(true);
				break;
			case TradeTab.Sales:
				_view.SalesGroup.SetActive(true);
				break;
		}
	}

	private void HideAllGroup()
	{
		_view.AuctionGroup.SetActive(false);
		_view.PitchGroup.SetActive(false);
		_view.SalesGroup.SetActive(false);
		_view.CmomerceGroup.SetActive(false);
	}
}
