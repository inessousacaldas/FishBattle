// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingViewController.cs
// Author   : DM-PC092
// Created  : 1/16/2018 2:27:53 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IRankingViewController
{
    IRankingPageController RankingPageCtrl { get; }
}

public partial class RankingViewController
{

    IRankData _data;
    private RankingPageController _rankingPage;
    public IRankingPageController RankingPageCtrl { get { return _rankingPage; } }
    #region
    private enum PageType
    {
        RankPage = 0,
        AppellationPage = 1
    }

    public TabbtnManager GetTabMgr { get { return tabMgr; } }
    private TabbtnManager tabMgr;
    public static readonly ITabInfo[] TeamTabInfos =
    {
        TabInfoData.Create((int) PageType.RankPage, "排行榜"),
        //TabInfoData.Create((int) PageType.AppellationPage, "称谓")
    };

    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitTabBtn();
        InitCmomerceView();
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {

    }

    protected override void RemoveCustomEvent()
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
    protected override void UpdateDataAndView(IRankData data)
    {
        _data = data;
        _rankingPage.UpdateDataAndView(_data);
    }

    private void InitCmomerceView()
    {
        _rankingPage = AddChild<RankingPageController, RankingPage>(_view.SubRankingGroup, RankingPage.NAME);
    }

    private void InitTabBtn()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabGrid_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget.ToString()
            , "Tabbtn_" + i);

        _view.TabGrid_UIGrid.Reposition();

        tabMgr = TabbtnManager.Create(TeamTabInfos, func);


    }

}


