// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildHasJoinViewController.cs
// Author   : DM-PC092
// Created  : 1/12/2018 5:42:00 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using AppDto;
using UniRx;



public partial interface IGuildHasJoinViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabClick(GuildTab tab);  //分页按钮点击
    IGuildInfoViewController GuildInfoViewCtrl { get; }     //信息界面
    IGuildManageViewController GuildManageViewCtrl { get; } //管理界面
    IGuildBuildViewController GuildBuildViewCtrl { get; }   //建筑界面
    IGuildWelfareViewController GuildWeilfareViewCtrl { get; }
    IEnumerable<ITabInfo> FilterList { get; }
}
public partial class GuildHasJoinViewController
{
    private readonly ITabInfo[] tabInfoList =
    {
        TabInfoData.Create((int)GuildTab.InfoView,"信息"),
        TabInfoData.Create((int)GuildTab.ManageView,"管理"),
        TabInfoData.Create((int)GuildTab.ActivityView,"活动"),
        TabInfoData.Create((int)GuildTab.WelfareView,"福利"),
        TabInfoData.Create((int)GuildTab.BuildView,"建筑")
    };
    private TabbtnManager tabMgr = null;
    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }
    private GuildTab curTab = GuildTab.None;
    private UnityEngine.GameObject lastGo = null;   //上一次打开的界面
    //信息
    
    private IGuildInfoViewController guildInfoViewCtrl;
    public IGuildInfoViewController GuildInfoViewCtrl { get { return guildInfoViewCtrl; } }
    //管理

    private IGuildManageViewController guildManageViewCtrl;
    public IGuildManageViewController GuildManageViewCtrl { get { return guildManageViewCtrl; } }
    //建筑

    private IGuildBuildViewController guildBuildViewCtrl;
    public IGuildBuildViewController GuildBuildViewCtrl { get { return guildBuildViewCtrl; } }
    //福利
    private IGuildWelfareViewController guildWelfareViewCtrl;
    public IGuildWelfareViewController GuildWeilfareViewCtrl { get { return guildWelfareViewCtrl; } }


    private IEnumerable<ITabInfo> filterList = null;
    public IEnumerable<ITabInfo> FilterList { get { return filterList; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        filterList = GetOpenTab();
        Func<int, ITabBtnController> func1 = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            View.TabGrid_UIGrid.gameObject,
            TabbtnPrefabPath.TabBtnWidget.ToString()
        );
        tabMgr = TabbtnManager.Create(filterList, func1);
    }

    private IEnumerable<ITabInfo> GetOpenTab()
    {
        IEnumerable<ITabInfo> tabs = null;
        tabs = tabInfoList.Filter(e => IsFuncOpen((GuildTab)e.EnumValue));
        return tabs == null ? tabInfoList : tabs;
    }
    private bool IsFuncOpen(GuildTab tab)
    {
        switch (tab)
        {
            //case GuildTab.ManageView:
                //return FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_81);
            case GuildTab.ActivityView:
                return FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_79);
            case GuildTab.WelfareView:
               // return FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_78);
            case GuildTab.BuildView:
                return FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_80);
            default:return true;
        }
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
        curTab = GuildTab.None;
        tabMgr = null;
        lastGo = null;
        guildInfoViewCtrl = null;
        guildManageViewCtrl = null;
        filterList = null;
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGuildMainData data)
    {
        UpdateView(data);
    }

    public void OnTabClick(GuildTab tab)
    {
        //GuildTab tab = (GuildTab)filterList.TryGetValue(idx).EnumValue;
        curTab = tab;
        if (lastGo != null) lastGo.SetActive(false);
        switch (tab)
        {
            case GuildTab.InfoView:
                lastGo = View.InfoView;
                break;
            case GuildTab.ManageView:
                lastGo = View.ManagerView;
                break;
            case GuildTab.ActivityView:
                lastGo = View.ActivityView;
                break;
            case GuildTab.WelfareView:
                lastGo = View.WelfareView;
                break;
            case GuildTab.BuildView:
                lastGo = View.BuildingView;
                break;
        }
        lastGo.SetActive(true);
    }

    private void UpdateView(IGuildMainData data)
    {
        if (!View._gameObject.activeSelf) return;
        switch (curTab)
        {
            case GuildTab.InfoView:
                if(guildInfoViewCtrl == null)
                {
                    guildInfoViewCtrl = AddChild<GuildInfoViewController, GuildInfoView>(View.InfoView, GuildInfoView.NAME, GuildInfoView.NAME);
                    _disposable.Add(guildInfoViewCtrl.OnguildInfoBtn_UIButtonClick.Subscribe(e => guildInfoBtn_UIButtonEvt.OnNext(e)));//公会信息点击
                    _disposable.Add(guildInfoViewCtrl.OnmemberInfoBtn_UIButtonClick.Subscribe(e => memberInfoBtn_UIButtonEvt.OnNext(e)));//成员信息点击
                    _disposable.Add(guildInfoViewCtrl.OncloseBtn_UIButtonClick.Subscribe(e => modifyCloseBtn_UIButtonEvt.OnNext(e)));   //修改职位关闭按钮
                    _disposable.Add(guildInfoViewCtrl.OnappointBtn_UIButtonClick.Subscribe(e => appointBtn_UIButtonEvt.OnNext(e)));//确定委任点击
                    _disposable.Add(guildInfoViewCtrl.OnmemberBtn_UIButtonClick.Subscribe(e => memberBtn_UIButtonEvt.OnNext(e)));//成员排序
                    _disposable.Add(guildInfoViewCtrl.OngradeBtn_UIButtonClick.Subscribe(e => gradeBtn_UIButtonEvt.OnNext(e)));//等级排序
                    _disposable.Add(guildInfoViewCtrl.Onjob_UIButtonClick.Subscribe(e => job_UIButtonEvt.OnNext(e)));   //职位排序
                    _disposable.Add(guildInfoViewCtrl.Oncontribution_UIButtonClick.Subscribe(e => contribution_UIButtonEvt.OnNext(e)));//贡献度排序
                    _disposable.Add(guildInfoViewCtrl.Ononline_UIButtonClick.Subscribe(e => online_UIButtonEvt.OnNext(e))); //在线排序
                    _disposable.Add(guildInfoViewCtrl.OnsendMesBtn_UIButtonClick.Subscribe(e => sendMesBtn_UIButtonEvt.OnNext(e))); //发送消息
                    _disposable.Add(guildInfoViewCtrl.OnaddFriendBtn_UIButtonClick.Subscribe(e => addFriendBtn_UIButtonEvt.OnNext(e))); //添加好友
                    _disposable.Add(guildInfoViewCtrl.OnmodifyJobBtn_UIButtonClick.Subscribe(e => modifyJobBtn_UIButtonEvt.OnNext(e)));//修改职位
                    _disposable.Add(guildInfoViewCtrl.OnkickMemBtn_UIButtonClick.Subscribe(e => kickMemBtn_UIButtonEvt.OnNext(e)));//开除成员
                    _disposable.Add(guildInfoViewCtrl.OnguildListBtn_UIButtonClick.Subscribe(e => guildListBtn_UIButtonEvt.OnNext(e)));//势力列表
                    _disposable.Add(guildInfoViewCtrl.OnleaveBtn_UIButtonClick.Subscribe(e => leaveBtn_UIButtonEvt.OnNext(e)));//离开公会
                    _disposable.Add(guildInfoViewCtrl.OnrequestListBtn_UIButtonClick.Subscribe(e => requestListBtn_UIButtonEvt.OnNext(e)));//申请列表
                    _disposable.Add(guildInfoViewCtrl.OnbackGuildBtn_UIButtonClick.Subscribe(e => backGuildBtn_UIButtonEvt.OnNext(e)));//回到公会
                    _disposable.Add(guildInfoViewCtrl.MemItemClick.Subscribe(e => memItemEvt.OnNext(e)));   //公会成员点击
                    _disposable.Add(guildInfoViewCtrl.OnrCloseBtn_UIButtonClick.Subscribe(e => rCloseBtn_UIButtonEvt.OnNext(e)));   //关闭申请列表
                    _disposable.Add(guildInfoViewCtrl.OnrClearListBtn_UIButtonClick.Subscribe(e => rClearListBtn_UIButtonEvt.OnNext(e)));   //清除申请列表
                    _disposable.Add(guildInfoViewCtrl.OnrOneKeyAccept_UIButtonClick.Subscribe(e => rOneKeyAccept_UIButtonEvt.OnNext(e)));   //一键接收
                    _disposable.Add(guildInfoViewCtrl.modifyJobClick.Subscribe(e => modifyJobEvt.OnNext(e)));   //修改职位
                    _disposable.Add(guildInfoViewCtrl.OnrequesterStatEvt.Subscribe(e => requesterStatEvt.OnNext(e)));   //申请列表Item接收点击
                    _disposable.Add(guildInfoViewCtrl.MemDragBottom.Subscribe(e => memDragBottom.OnNext(e)));       //成员列表向下拉取
                    _disposable.Add(guildInfoViewCtrl.ReqDragBottom.Subscribe(e => reqDragBottom.OnNext(e)));       //申请列表向下拉取
                }
                guildInfoViewCtrl.UpdateView(data);
                break;
            case GuildTab.ManageView:
                if (guildManageViewCtrl == null)
                {
                    guildManageViewCtrl = AddChild<GuildManageViewController, GuildManageView>(View.ManagerView, GuildManageView.NAME, GuildManageView.NAME);
                    _disposable.Add(guildManageViewCtrl.OnNoticeModificationBtn_UIButtonClick.Subscribe(e => NoticeModificationBtn_UIButtonEvt.OnNext(e))); //公告修改
                    _disposable.Add(guildManageViewCtrl.OnManifestoModificationBtn_UIButtonClick.Subscribe(e => ManifestoModificationBtn_UIButtonEvt.OnNext(e)));//宣言修改
                    _disposable.Add(guildManageViewCtrl.OnMoreMessageLabel_UIButtonClick.Subscribe(e => MoreMessageLabel_UIButtonEvt.OnNext(e)));   //公会事件
                }
                guildManageViewCtrl.UpdateView(data);
                break;
            case GuildTab.ActivityView:
                break;
            case GuildTab.WelfareView:
                if(guildWelfareViewCtrl == null)
                {
                    guildWelfareViewCtrl = AddChild<GuildWelfareViewController, GuildWelfareView>(View.WelfareView, GuildWelfareView.NAME, GuildWelfareView.NAME);
                    _disposable.Add(guildWelfareViewCtrl.OnexplainBtn_UIButtonClick.Subscribe(e => welfareExplainBtn_UIButtonEvt.OnNext(e)));
                    _disposable.Add(guildWelfareViewCtrl.OncheckBtn_UIButtonClick.Subscribe(e => welfareCheckBtn_UIButtonEvt.OnNext(e)));
                }
                guildWelfareViewCtrl.UpdateView(data);
                break;
            case GuildTab.BuildView:
                if (guildBuildViewCtrl == null)
                {
                    guildBuildViewCtrl = AddChild<GuildBuildViewController, GuildBuildView>(View.BuildingView, GuildBuildView.NAME, GuildBuildView.NAME);
                    _disposable.Add(guildBuildViewCtrl.Onbg_UIButtonClick.Subscribe(e => bg_UIButtonEvt.OnNext(e)));    //背景点击
                    _disposable.Add(guildBuildViewCtrl.OncheckBtn_UIButtonClick.Subscribe(e => checkBtn_UIButtonEvt.OnNext(e)));    //查看按钮点击
                    _disposable.Add(guildBuildViewCtrl.OnexplainBtn_UIButtonClick.Subscribe(e => explainBtn_UIButtonEvt.OnNext(e)));//问号按钮点击
                    _disposable.Add(guildBuildViewCtrl.OnupgradeBtn_UIButtonClick.Subscribe(e => upgradeBtn_UIButtonEvt.OnNext(e)));//升级点击
                }
                guildBuildViewCtrl.UpdateView(data);
                break;
        }
    }
    
}
