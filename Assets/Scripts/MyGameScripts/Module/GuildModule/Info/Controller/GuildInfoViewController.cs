// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildInfoViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using UnityEngine;
using AppDto;
using UniRx;
public partial interface IGuildInfoViewController
{
    void UpdateView(IGuildMainData data);   //更新数据
    void OnTabBtnClick(MemOrGuildEnum tab); //切换按钮
    void OnMemItemClick(IGuildMemItemController item);//成员点击
    long SelMemId { get; }
    void ShowRequestList(bool show);    //显示申请列表
    void ShowModifyJobPanel(IGuildMainData data);   // 显示修改职位
    UniRx.IObservable<guildModifyJobBtnController> modifyJobClick { get; }
    void OnModifyBtnClick(guildModifyJobBtnController ctrl);    //职位点击
    guildModifyJobBtnController ModifyJobPos { get; }   //当前选择职位
    IGuildMemItemController LastSelItemCtrl { get; }    //当前选择的人
    bool FirstOpen { set; }     //用于初始化成员点击
    UniRx.IObservable<bool> MemDragBottom { get; }  
    UniRx.IObservable<bool> ReqDragBottom { get; }
}
public partial class GuildInfoViewController
{
    private CompositeDisposable _disposable;
    private Dictionary<GameObject, IGuildMemItemController> _memItemDic = new Dictionary<GameObject, IGuildMemItemController>();  // recycleList索引
    private IEnumerable<GuildMemberDto> _memItemData;           // 成员recycleList所需数据

    private long selMemId = 0;  //  用于刷新recycle时，区分被选中Item
    public long SelMemId { get { return selMemId; } }
    private bool isFirstOpen = true;   //第一次打开界面,用于公会信息和成员信息分页按钮点击
    private IGuildMemItemController lastSelItemCtrl = null;     //上一次选择的成员Item
    public IGuildMemItemController LastSelItemCtrl { get { return lastSelItemCtrl; } }
    private MemOrGuildEnum curMOGTab = MemOrGuildEnum.None;

    private Dictionary<GameObject, IGuildRequesterController> _requesterItemDic = new Dictionary<GameObject, IGuildRequesterController>();//申请recycleList索引
    private IEnumerable<GuildApprovalDto> _requestItemData;       //申请recycleList所需数据

    private List<guildModifyJobBtnController> modifyJobList;        //公会职位列表
    private Subject<guildModifyJobBtnController> modifyJobEvt = new Subject<guildModifyJobBtnController>();   //公会职位点击
    public UniRx.IObservable<guildModifyJobBtnController> modifyJobClick { get { return modifyJobEvt; } }
    private guildModifyJobBtnController lastModifyBtn;

    private bool firstOpen = true;      //第一次打开界面，用于成员点击
    public bool FirstOpen { set { firstOpen = value; } }

    private Subject<bool> memDragBottom = new Subject<bool>();                  //成员列表向下拉数据
    public UniRx.IObservable<bool> MemDragBottom { get { return memDragBottom; } }

    private Subject<bool> reqDragBottom = new Subject<bool>();                  //申请列表向下拉数据
    public UniRx.IObservable<bool> ReqDragBottom { get { return reqDragBottom; } }


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        for (int i = 0; i < 10; i++)
        {
            var ctrl = AddChild<GuildMemItemController, GuildMemItem>(View.memGrid_UIRecycledList.gameObject, GuildMemItem.NAME, GuildMemItem.NAME + i);
            ctrl.UpdateBg(i);
            _disposable.Add(ctrl.OnGuildMemItem_UIButtonClick.Subscribe(e => memItemEvt.OnNext(ctrl)));
            _memItemDic.Add(ctrl.gameObject, ctrl);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        View.memGrid_UIRecycledList.onUpdateItem += UpdateGuildItem;
        View.requestGrid_UIRecycledList.onUpdateItem += UpdateRequestItem;
        View.memPanel_UIScrollView.onDragFinished += OnMemStopMoving;
        View.requestPanel_UIScrollView.onDragFinished += OnReqStopMoving;
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _memItemDic.Clear();
        _memItemData = null;
        selMemId = 0;
        isFirstOpen = true;
        lastSelItemCtrl = null;
        curMOGTab = MemOrGuildEnum.None;
        _requestItemData = null;
        modifyJobList = null;
        lastModifyBtn = null;
        firstOpen = true;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        View.memGrid_UIRecycledList.onUpdateItem -= UpdateGuildItem;
        View.requestGrid_UIRecycledList.onUpdateItem -= UpdateRequestItem;
        View.memPanel_UIScrollView.onDragFinished -= OnMemStopMoving;
        View.requestPanel_UIScrollView.onDragFinished -= OnReqStopMoving;
    }

    public void UpdateView(IGuildMainData data)
    {
        var list = data.GuildMemberList;
        int count = 0;
        list.ForEach(e => count++);
        _memItemData = list;
        View.memGrid_UIRecycledList.UpdateDataCount(count, false);

        if (firstOpen)
        {
            IGuildMemItemController item = null;
            foreach (var val in _memItemDic)
            {
                item = val.Value;
                break;
            }
            OnMemItemClick(item);
        }
        
        UpdateGuildInfo(data);
        UpdateRequestListView(data);    //更新请求列表
    }

    //成员点击
    public void OnMemItemClick(IGuildMemItemController item)
    {
        if (item == null) return;
        var dto = item.MemberDto;
        if (dto == null) return;
        //第一次打开显示公会信息，其余每次点击都显示成员信息
        if (isFirstOpen)
        {
            OnTabBtnClick(MemOrGuildEnum.Guild);
            isFirstOpen = false;
        }
        else
        {
            OnTabBtnClick(MemOrGuildEnum.Mem);
        }
        ShowModifyBtn(dto);
        if (lastSelItemCtrl != null)
        {
            if (selMemId == item.Idx) return;
            lastSelItemCtrl.OnSel(false);
        }
        selMemId = dto.id;
        lastSelItemCtrl = item;
        lastSelItemCtrl.OnSel(true);
        UpdateMemInfo(item);
        firstOpen = false;
    }

    private void ShowModifyBtn(GuildMemberDto dto)
    {
        View.kickMemBtn_UIButton.gameObject.SetActive(dto.id != ModelManager.Player.GetPlayerId());
    }

    //成员信息刷新
    public void UpdateMemInfo(IGuildMemItemController item)
    {
        var dto = item.MemberDto;
        UIHelper.SetPetIcon(View.memIcon_UISprite, (dto.gender == 1 ? 101 : 103).ToString());
        UIHelper.SetCommonIcon(View.factionIcon_UISprite, "faction_" + dto.factionId);
        View.memName_UILabel.text = dto.name;
        View.memIdLabel_UILabel.text = dto.id.ToString();
        var str = DateUtil.GetDateStr(dto.joinTime,"yyyy/MM/dd");
        View.joinTimeLabel_UILabel.text = str;
    }

    //势力信息刷新
    public void UpdateGuildInfo(IGuildMainData data)
    {
        var guild = data.GuildBaseInfo;
        if (guild == null) return;
        View.guildNameLabel_UILabel.text = guild.name;
        View.guildBossLabel_UILabel.text = guild.bossName;
        View.guildMemLabel_UILabel.text = guild.memberCount + "/" + guild.maxMemberCount;
        View.guildLvLabel_UILabel.text = guild.grade.ToString();
        View.guildIdLabel_UILabel.text = guild.showId.ToString();
    }

    //成员信息点击 势力信息点击(只与UI相关)
    public void OnTabBtnClick(MemOrGuildEnum tab)
    {
        switch (tab)
        {
            case MemOrGuildEnum.Guild:
                if (curMOGTab == MemOrGuildEnum.Guild)
                    return;
                break;
            case MemOrGuildEnum.Mem:
                if (curMOGTab == MemOrGuildEnum.Mem)
                    return;
                break;
        }
        UpdateGOMTab(tab);
    }

    public void ShowRequestList(bool show)
    {
        View.RequestList.SetActive(show);
    }


    private void UpdateGOMTab(MemOrGuildEnum tab)
    {
        curMOGTab = tab;
        bool selGuild = tab == MemOrGuildEnum.Guild;
        View.guildInfoBtn_UISprite.spriteName = selGuild ? "Tab_2_On" : "Tab_2_Off";
        View.memberInfoBtn_UISprite.spriteName = !selGuild ? "Tab_2_On" : "Tab_2_Off";
        View.guildInfo_UILabel.fontSize = selGuild ? 20 : 18;
        View.memberInfo_UILabel.fontSize = !selGuild ? 20 : 18;
        View.guildInfo_UILabel.color = selGuild ? ColorExt.HexStrToColor("2D2D2D") : ColorExt.HexStrToColor("B8B8B8");
        View.memberInfo_UILabel.color = !selGuild ? ColorExt.HexStrToColor("2D2D2D") : ColorExt.HexStrToColor("B8B8B8");
        View.guildInfoBtn_UISprite.depth = selGuild ? 2 : 1;
        View.memberInfoBtn_UISprite.depth = !selGuild ? 2 : 1;
        View.guildInfo.SetActive(selGuild);
        View.memberInfo.SetActive(!selGuild);
    }
    //成员RecycleList数据刷新
    private void UpdateGuildItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_memItemData == null) return;
        IGuildMemItemController item = null;
        if (_memItemDic.TryGetValue(go, out item))
        {
            var info = _memItemData.TryGetValue(dataIndex);
            if (info == null) return;
            item.UpdateView(info, selMemId);
        }
    }

    private void OnMemStopMoving()
    {
        Vector3 constraint = View.memPanel_UIScrollView.panel.CalculateConstrainOffset(
            View.memPanel_UIScrollView.bounds.min
            , View.memPanel_UIScrollView.bounds.max);

        bool isBottomLock = constraint.y <= -0.001f;
        bool isUpLock = constraint.y >= 1;
        memDragBottom.OnNext(isBottomLock);
    }

    //更新申请列表
    private void UpdateRequestListView(IGuildMainData data)
    {
        if (!View.RequestList.activeSelf) return;
        CreateRequsetListItem();
        var list = data.GuildApprovalList;
        if (list == null) return;
        int count = list.ToList().Count;
        _requestItemData = list;
        View.requestGrid_UIRecycledList.UpdateDataCount(count, false);
    }
    private void OnReqStopMoving()
    {
        Vector3 constraint = View.requestPanel_UIScrollView.panel.CalculateConstrainOffset(
            View.requestPanel_UIScrollView.bounds.min
            , View.requestPanel_UIScrollView.bounds.max);

        bool isLock = constraint.y <= -0.001f;
        reqDragBottom.OnNext(isLock);
    }
    private void CreateRequsetListItem()
    {
        if (_requesterItemDic.Count > 0) return;
        for (int i = 0; i < 10; i++)
        {
            var ctrl = AddChild<GuildRequesterController, GuildRequester>(View.requestGrid_UIRecycledList.gameObject, GuildRequester.NAME, GuildRequester.NAME + i);
            _disposable.Add(ctrl.OnrOperateBtn_UIButtonClick.Subscribe(e => requesterStatEvt.OnNext(ctrl)));
            _requesterItemDic.Add(ctrl.gameObject, ctrl);
        }
    }

    //更新申请列表recycle数据
    private void UpdateRequestItem(GameObject go,int itemIndex,int dataIndex)
    {
        if (_requestItemData == null) return;
        IGuildRequesterController item = null;
        if(_requesterItemDic.TryGetValue(go,out item))
        {
            var info = _requestItemData.TryGetValue(dataIndex);
            if (info == null) return;
            item.UpdateView(info);
        }
    }


    //显示职位修改
    public void ShowModifyJobPanel(IGuildMainData data)
    {
        View.ModifyJob.SetActive(!View.ModifyJob.activeSelf);
        if (View.ModifyJob.activeSelf)
        {
            if (modifyJobList == null)
                modifyJobList = new List<guildModifyJobBtnController>();
            var list = data.GuildPosition;
            GuildMemberDto member;
            IEnumerable<GuildPosition> filterList;
            if (lastSelItemCtrl != null)
            {
                member = lastSelItemCtrl.MemberDto;
                filterList = list.Filter(e => (member.gender == e.gender || e.gender == 2) && IsShowBossBtn(e) && HasAuthority(data,e));
                View.mMemName_UILabel.text = member.name;
                View.mMemContributionLabel_UILabel.text = member.totalCbute.ToString();
                View.mJobLabel_UILabel.text = list.Find(e => e.id == member.position).name;
                UIHelper.SetPetIcon(View.mMemIcon_UISprite, (member.gender == 1 ? 101 : 103).ToString());
                UIHelper.SetCommonIcon(View.mFactionIcon_UISprite, "faction_" + member.factionId);
            }
            else
            {//防止lastSelItemCtrl为空，后面做按钮点击拦截
                filterList = list;
            }
            int poolCount = modifyJobList.Count;
            int count = filterList.ToList().Count;
            for (int i = 0; i < poolCount; i++)
            {
                modifyJobList[i].gameObject.SetActive(i < count);
                if (i < count)
                {
                    var dto = filterList.TryGetValue(i);
                    modifyJobList[i].UpdateView(dto);
                }
            }
            for (; poolCount < count; poolCount++)
            {
                var dto = filterList.TryGetValue(poolCount);
                var ctrl = AddChild<guildModifyJobBtnController, guildModifyJobBtn>(View.ModifyGrid_UIGrid.gameObject, guildModifyJobBtn.NAME);
                ctrl.UpdateView(dto);
                _disposable.Add(ctrl.OnguildModifyJobBtn_UIButtonClick.Subscribe(f => modifyJobEvt.OnNext(ctrl)));
                modifyJobList.Add(ctrl);
            }
            View.ModifyGrid_UIGrid.Reposition();
        }
        else
        {
            if (lastModifyBtn != null)
            {
                lastModifyBtn.UpdateBtnState(false);
                lastModifyBtn = null;
            }
        }
    }

    //只有副会长显示会长按钮
    private bool IsShowBossBtn(GuildPosition pos)
    {
        var member = lastSelItemCtrl.MemberDto;
        if (pos.id == (int)GuildPosition.GuildPositionEnum.Boss)
        {
            return member.position == (int)GuildPosition.GuildPositionEnum.ViceBoss;
        }
        return true;
    }

    //筛选可以修改的职位
    private bool HasAuthority(IGuildMainData data, GuildPosition pos)
    {
        var guildList = data.GuildPosition;
        var selfInfo = data.PlayerGuildInfo;
        if (selfInfo != null)
        {
            var val = guildList.Find(e => e.id == selfInfo.positionId);
            if (val != null)
            {
                return val.appoints.Contains(pos.id);
            }
        }
        return true;
    }
    //点击职位
    public void OnModifyBtnClick(guildModifyJobBtnController ctrl)
    {
        if (ctrl == null) return;
        if (lastModifyBtn != null)
        {
            if (lastModifyBtn == ctrl)
                return;
            lastModifyBtn.UpdateBtnState(false);
        }

        lastModifyBtn = ctrl;
        lastModifyBtn.UpdateBtnState(true);
    }

    //修改职位
    public guildModifyJobBtnController ModifyJobPos
    {
        get
        {
            return lastModifyBtn;
        }
    }
}
