// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildMainViewController.cs
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:20 PM
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
using UniRx;

//未加入公会时，界面状态
public enum NotJoinViewState
{
    Join,
    Create,
    None
}
public partial interface IGuildMainViewController
{
    TabbtnManager TabMgr { get; }
    string CreateNameStr { get; }       //创建公会名字
    string CreateManifestoStr { get; }  //创建公会宣言
    UniRx.IObservable<bool> ScrollDragFinish { get; }   //拖动结束
    void OnGuildItemClick(GuildItemController dto);    //公会点击逻辑
    string SearchStr { get; }       //搜索
    int CurSelectGuild { get; }     //当前所选公会
    NotJoinViewState CurTab { get; set; } //当前界面状态
    UniRx.IObservable<string> SearchInputChange { get; }  //公会搜索输入
    bool FirstOpen { set; }
    GuildItemController SelItemCtrl { get; }
    GuildItemController TmpItemCtrl { get; set; }
}
public partial class GuildMainViewController    {

    private readonly ITabInfo[] tabInfoList =
    {
        TabInfoData.Create((int)NotJoinViewState.Join,"加入"),
        TabInfoData.Create((int)NotJoinViewState.Create,"创建")
    };
    private TabbtnManager tabMgr = null;
    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    private NotJoinViewState curTab = NotJoinViewState.None;
    public NotJoinViewState CurTab
    {
        get { return curTab; }
        set { curTab = value; }
    }
    private Dictionary<GameObject, GuildItemController> _guildItemDic = new Dictionary<GameObject, GuildItemController>();  // recycleList索引
    private IEnumerable<GuildBaseInfoDto> _guildItemData;                                           // recycleList所需数据
    private int curSelectGuild = -1;     //当前所选公会
    public int CurSelectGuild { get { return curSelectGuild; } }
    private GuildItemController lastSelItemCtrl = null; //当前选择
    private GuildItemController tmpSelItemCtrl = null;  //缓存当前选择

    private UniRx.Subject<bool> scrollDragFinish = new UniRx.Subject<bool>();
    public UniRx.IObservable<bool> ScrollDragFinish { get { return scrollDragFinish; } }
    private UniRx.Subject<string> searchInputChange = new UniRx.Subject<string>();
    public UniRx.IObservable<string> SearchInputChange { get { return searchInputChange; } }

    private bool firstOpen = true;
    public bool FirstOpen { set { firstOpen = value; } }

	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
             View.TabGrid_UIGrid.gameObject,
             TabbtnPrefabPath.TabBtnWidget.ToString());
        tabMgr = TabbtnManager.Create(tabInfoList, func);
        for (int i = 0; i < 10; i++)
        {
            var ctrl = AddChild<GuildItemController, GuildItem>(View.grid_UIGrid.gameObject, GuildItem.NAME,GuildItem.NAME + i);
            ctrl.UpdateBg(i);
            _disposable.Add(ctrl.OnGuildItem_UIButtonClick.Subscribe(e => guildItemCtrl.OnNext(ctrl)));
            _guildItemDic.Add(ctrl.gameObject, ctrl);
        }
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        View.grid_UIGrid.onUpdateItem += UpdateGuildItem;
        View.scrollPanle.onDragFinished += OnStopMoving;
        EventDelegate.Set(View.search_UIInput.onChange, SearchOnChange);
        EventDelegate.Set(View.manifestoBg_UIInput.onSubmit, OnSubmit);
        EventDelegate.Set(View.nameBg_UIInput.onSubmit, OnSubmit);
        EventDelegate.Set(View.search_UIInput.onSubmit, OnSubmit);
    }

    private void OnSubmit()
    {

    }
    protected override void RemoveCustomEvent ()
    {
        View.grid_UIGrid.onUpdateItem -= UpdateGuildItem;
        View.scrollPanle.onDragFinished -= OnStopMoving;
        EventDelegate.Remove(View.search_UIInput.onChange, SearchOnChange);
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        curTab = NotJoinViewState.None;
        _guildItemDic.Clear();
        _guildItemData = null;
        tabMgr = null;
        lastSelItemCtrl = null;
        tmpSelItemCtrl = null;
        curSelectGuild = -1;
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGuildMainData data)
    {
        ClearData();
        UpdateView(data);
    }

    private void ClearData()
    {
        curTab = NotJoinViewState.None;
    }

    //刷新界面
    public void UpdateView(IGuildMainData data)
    {
        ShowJoinOrCreate(data.JoinOrCreateState, data);
    }
    

    #region 加入与创建界面逻辑
    public void ShowJoinOrCreate(NotJoinViewState state,IGuildMainData data)
    {
        switch (state)
        {
            case NotJoinViewState.Join:
                OnJoinBtnClick(data);
                break;
            case NotJoinViewState.Create:
                OnCreateBtnClick(data);
                break;
        }
    }
    //加入
    private void OnJoinBtnClick(IGuildMainData data)
    {
        var list = data.GuildList;
        if (curTab == NotJoinViewState.Join) return;
        curTab = NotJoinViewState.Join;
        ShowRequestBtn(data.GuildState);

        if (list == null) return;
        bool showNotGuildView = list.ToList().Count == 0 ? true : false;
        ShowJoinOrCreate(true,showNotGuildView);

        int count = 0;
        list.ForEach(e => { count++; });
        _guildItemData = list;

        View.grid_UIGrid.UpdateDataCount(count, false);
        if (firstOpen)
        {
            GuildItemController item = null;
            foreach (var val in _guildItemDic)
            {
                item = val.Value;
                break;
            }
            OnGuildItemClick(item);
            firstOpen = false;
        }
        

        if(data.NeedReposition)
            SpringPanel.Begin(View.scrollPanle.gameObject, new Vector3(36, -25, 0), 8);
    }

    private void ShowRequestBtn(GuildState state)
    {
        bool show = state == GuildState.NotJoin;
        View.applyBtn_UIButton.gameObject.SetActive(show);
        View.oneKeyApply_UIButton.gameObject.SetActive(show);
        View.TabBg_Transform.gameObject.SetActive(show);
        View.TabGrid_UIGrid.gameObject.SetActive(show);
    }

    //创建
    private void OnCreateBtnClick(IGuildMainData data)
    {
        if (curTab == NotJoinViewState.Create) return;
        curTab = NotJoinViewState.Create;
        ShowJoinOrCreate(false);
        View.diamondeLabel_UILabel.text = data.CreateNeed(true)+"";
        View.goldLabel_UILabel.text = data.CreateNeed(false) +"";
        View.warningLabel.SetActive(data.CurServerGuildCount >= 100);
    }

    private void ShowJoinOrCreate(bool show,bool showNotGuild = false)
    {
        if (show)
        {
            View.NotGuild.SetActive(showNotGuild);
            View.Join.SetActive(!showNotGuild);
        }
        else
        {
            View.Join.SetActive(show);
        }
        View.Create.SetActive(!show);
    }

    //创建公会名字
    public string CreateNameStr
    {
        get { return View.nameBg_UIInput.value; }
    }

    //创建公会宣言
    public string CreateManifestoStr
    {
        get { return View.manifestoBg_UIInput.value; }
    }

    //RecycleList数据刷新
    private void UpdateGuildItem(GameObject go,int itemIndex,int dataIndex)
    {
        if (_guildItemData == null) return;
        GuildItemController item = null;
        if(_guildItemDic.TryGetValue(go,out item))
        {
            var info = _guildItemData.TryGetValue(dataIndex);
            if (info == null) return;
            item.UpdateView(info, curSelectGuild);
            if (curSelectGuild == info.showId && lastSelItemCtrl != item)
                lastSelItemCtrl = item;
        }
    }

    //公会点击
    public void OnGuildItemClick(GuildItemController ctrl)
    {
        if (ctrl == null) return;
        var dto = ctrl.ItemInfoDto;
        if (dto == null) return;
        if (lastSelItemCtrl != null)
        {
            if (curSelectGuild == dto.showId) return;
            lastSelItemCtrl.OnSel(false);
        }
        curSelectGuild = dto.showId;
        lastSelItemCtrl = ctrl;
        lastSelItemCtrl.OnSel(true);
        View.infoLabel_UILabel.text = dto.memo;
    }
    
    //搜索
    public string SearchStr
    {
        get { return View.search_UIInput.value; }
    }

    public GuildItemController SelItemCtrl { get { return lastSelItemCtrl; } }
    public GuildItemController TmpItemCtrl { get { return tmpSelItemCtrl; }set { tmpSelItemCtrl = value; } }
    #endregion

    private void OnStopMoving()
    {
        Vector3 constraint = View.scrollPanle.panel.CalculateConstrainOffset(
            View.scrollPanle.bounds.min
            , View.scrollPanle.bounds.max);
        
        bool isLock = constraint.y <= -0.001f;
        if (isLock) curTab = NotJoinViewState.None;
        scrollDragFinish.OnNext(isLock);
    }

    private void SearchOnChange()
    {
        searchInputChange.OnNext(SearchStr);
    }


}
