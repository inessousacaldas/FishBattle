// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MissionMainViewController.cs
// Author   : DM-PC092
// Created  : 9/5/2017 11:21:50 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public enum MisViewTab
{
    Accepted,
    CanAccess,
    DaylyLog,
    None
}

public partial interface IMissionMainViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabBtnClick(MisViewTab tabType,IMissionData data);
    UniRx.IObservable<SubMissionItemController> SubItemClick { get; }
    UniRx.IObservable<RecordAreaItemController> RecordAreaItemEvt { get; }
    UniRx.IObservable<RecordEvtItemController> EvtItemClick { get; }
    UniRx.IObservable<RecordDetailItemController> OnRecordDetailItem_UIButtonClick { get; }
    UniRx.IObservable<RecordDetailItemController> OnRecordStateBtnClick { get; }
    void OnEvtItemClick(RecordEvtItemController missionRecord, IMissionData data);
    void OnSubItemClick(SubMissionItemController mission);
    Mission CurMission { get; }
    void OnAreaClick(IMissionData data, RecordAreaItemController area);
    void OnRecordDetailBtnClick(IMissionData data);
    void ShowOrHideWindow(bool show);
    void OnRecordDetailItemClick(RecordDetailItemController item);
    void OnStateBtnClick(RecordDetailItemController item, IMissionData data);
    UniRx.IObservable<MissionTypeItemController> MisTypeItemClick { get; }
    void OnMisTypeItemClick(MissionTypeItemController item);
}
public partial class MissionMainViewController    {

    public enum RecordRewardType
    {
        None,
        Details,
        Receive,
        HasReceiveed
    }
    private Vector2[] areaPos = new Vector2[] { new Vector2(-306, 102), new Vector2(-175, -10), new Vector2(-394, -58), new Vector2(-198, -166), new Vector2(-381, -202) };
    private int curMisId = -1;
    private static readonly ITabInfo[] tabInfoList =
    {
        TabInfoData.Create((int)MisViewTab.Accepted,"已接"),
        TabInfoData.Create((int)MisViewTab.CanAccess,"可接"),
        TabInfoData.Create((int)MisViewTab.DaylyLog,"日志")
    };
    private MisViewTab curTab = MisViewTab.None;
    private TabbtnManager tabMgr = null;
    private List<MissionTypeItemController> typeItemList = new List<MissionTypeItemController>();
    private SubMissionItemController lastSubItem;
    private Mission curMission = null;

    //日记
    private List<RecordAreaItemController> recordAreaList = new List<RecordAreaItemController>(); //左侧地区列表管理
    private List<RecordEvtItemController> areaEventList = new List<RecordEvtItemController>();//右侧地区事件管理
    private List<ItemCellController> rewardList = new List<ItemCellController>(); //奖励物品管理
    private List<RecordDetailItemController> recordDetailItemList = new List<RecordDetailItemController>();//日志详情二级界面管理
    private List<ItemCellController> misRewardList = new List<ItemCellController>();
    private RecordAreaItemController curMissionAreaItem = null;
    private RecordEvtItemController curMissionRecord = null;
    private MissionTypeItemController curTypeItem = null;
    private string curAreaId = "";
    private int curEvtId = -1;
    private int curMisTypeIdx = -1;
    private RecordRewardType recordType = RecordRewardType.None;

    private bool clickByTab = true;

    #region 按钮初始化
    private Subject<MissionTypeItemController> misTypeItemEvt = new Subject<MissionTypeItemController>();
    public UniRx.IObservable<MissionTypeItemController> MisTypeItemClick { get { return misTypeItemEvt; } }
    private Subject<SubMissionItemController> subItemEvt = new Subject<SubMissionItemController>();
    public UniRx.IObservable<SubMissionItemController> SubItemClick { get { return subItemEvt; } }

    private Subject<RecordAreaItemController> recordAreaItemClk = new Subject<RecordAreaItemController>();
    public UniRx.IObservable<RecordAreaItemController> RecordAreaItemEvt { get { return recordAreaItemClk; } }

    private Subject<RecordEvtItemController> evtItemClick = new Subject<RecordEvtItemController>();
    public UniRx.IObservable<RecordEvtItemController> EvtItemClick { get { return evtItemClick; } }
    private Subject<RecordDetailItemController> recordDetailItem_UIButtonEvt = new Subject<RecordDetailItemController>();
    public UniRx.IObservable<RecordDetailItemController> OnRecordDetailItem_UIButtonClick { get { return recordDetailItem_UIButtonEvt; } }
    private Subject<RecordDetailItemController> recordStateBtnEvt = new Subject<RecordDetailItemController>();
    public UniRx.IObservable<RecordDetailItemController> OnRecordStateBtnClick { get { return recordStateBtnEvt; } }
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        CreateTabItem();
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
        curTab = MisViewTab.None;
        tabMgr = null;
        if (lastSubItem != null)
        {
            lastSubItem.UpdateViewByClick(false);
            lastSubItem = null;
        }
        curMission = null;
        curMisId = -1;
        if (typeItemList.Count > 0) typeItemList.Clear();
        if (recordAreaList.Count > 0) recordAreaList.Clear();
        if (areaEventList.Count > 0) areaEventList.Clear();
        if (rewardList.Count > 0) rewardList.Clear();
        if (misRewardList.Count > 0) misRewardList.Clear();
        curMissionAreaItem = null;
        curMissionRecord = null;
        curTypeItem = null;
        recordType = RecordRewardType.None;
        curAreaId = "";
        curEvtId = -1;
        curMisTypeIdx = -1;
        bool clickByTab = true;
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IMissionData data)
    {
        if(curTab == MisViewTab.DaylyLog)
        {
            ResetData();
            UpdateRecordLeft(data);
        }
        else
        {
            OnAcceptBtnClick(data);
        }
    }
    #region 初始化
    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
             View.TabGrid_UIGrid.gameObject,
             TabbtnPrefabPath.TabBtnWidget.ToString()
             );
        tabMgr = TabbtnManager.Create(tabInfoList, func);
    }
    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }
    #endregion

    private void ResetData()
    {
        curAreaId = "";
        curEvtId = -1;
    }
    public void OnTabBtnClick(MisViewTab tabType, IMissionData data)
    {
        curTab = tabType;
        clickByTab = true;
        switch (tabType)
        {
            case MisViewTab.Accepted:
                ShowBtn(true);
                OnAcceptBtnClick(data);
                break;
            case MisViewTab.CanAccess:
                ShowBtn(false);
                OnAcceptBtnClick(data);
                break;
            case MisViewTab.DaylyLog:
                OnRecordBtnClick(data);
                break;
                
        }
        View.table_UITable.Reposition();
    }

    private void ShowBtn(bool show)
    {
        //View.lblTitle_UILabel.text = show ? "已接任务" : "可接任务";
        SpringPanel.Begin(View.table_UITable.transform.parent.gameObject, new Vector3(-378, -32, 0), 8);
        View.CanAccessBtn_UIButton.gameObject.SetActive(!show);
        View.DropBtn_UIButton.gameObject.SetActive(show);
        View.GoonBtn_UIButton.gameObject.SetActive(show);
    }

    private void ShowRightPanel(bool show)
    {
        View.RightPanel_Trans.gameObject.SetActive(show);
        View.RecordPanel_Trans.gameObject.SetActive(!show);
    }

    private void OnRecordBtnClick(IMissionData data)
    {
        //View.lblTitle_UILabel.text = "工作日志";
        ShowRightPanel(false);
        UpdateRecordLeft(data);

    }

    #region ============================================日记============================================
    //日记区域
    private void UpdateRecordLeft(IMissionData data)
    {
        var dic = data.RecordDic;
        if (recordAreaList.Count == 0)
        {
            int idx = 0;
            dic.ForEach(_ =>
            {
                var item = AddChild<RecordAreaItemController, RecordAreaItem>(
                     View.recordLeft_Trans.gameObject,
                     RecordAreaItem.NAME
                     );
                if(idx<areaPos.Length)
                    item.transform.localPosition = areaPos[idx];
                item.UpdateRecordView(_.Value,idx);
                _disposable.Add(item.OnRecordAreaItem_UIButtonClick.Subscribe(e => recordAreaItemClk.OnNext(item)));
                recordAreaList.Add(item);
                if (idx == 0)
                    curMissionAreaItem = recordAreaList[0];
                idx++;
            });
        }
        if (recordAreaList.Count > 0)
            OnAreaClick(data, curMissionAreaItem);
    }
    //日记事件
    public void OnAreaClick(IMissionData data, RecordAreaItemController area)
    {
        bool change = false;
        List<MissionRecord> list = area.test(data);
        if (list == null) return;
        if (curMissionAreaItem != null)
        {
            curMissionAreaItem.UpdateTween(false);
            change = curMissionAreaItem.id != area.id;
        }
        area.UpdateTween(true);
        curMissionAreaItem = area;

        int poolCount = areaEventList.Count;
        int eventCount = list.Count;
        if(eventCount>0)
        {
            var id = list[0].area;
            if (id == curAreaId) return;
            curAreaId = id;
        }
        

        for (int i = 0; i < poolCount; i++)
        {
            areaEventList[i].gameObject.SetActive(i < eventCount);
            if (i < eventCount)
            {
                MissionRecord tMissionRecord = list[i];
                areaEventList[i].UpdateAreaEvtView(tMissionRecord,-1,data);
            }
        }

        for (int i = poolCount; i < eventCount; i++)
        {
            MissionRecord tMissionRecord = list[i];
            var item = AddChild<RecordEvtItemController, RecordEvtItem>(
                 View.evtGrid_UIGrid.gameObject,
                 RecordEvtItem.NAME
                 );
            item.UpdateAreaEvtView(tMissionRecord,i,data);
            _disposable.Add(item.OnRecordEvtItem_UIButtonClick.Subscribe(e =>
            {
                evtItemClick.OnNext(item);
            }
                ));
            areaEventList.Add(item);
        }

        bool showNoneBg = true;
        for(int i = 0,max = areaEventList.Count; i < max; i++)
        {
            var e = areaEventList[i];
            if (e.View.gameObject.activeSelf)
            {
                if (e.IsEvtOpen == true)
                {
                    showNoneBg = false;
                    break;
                }
            }
        }
        ShowRecordNoneBg(showNoneBg);
        if (showNoneBg) return;
        
        if (areaEventList.Count > 0 && curMissionRecord == null)
            OnEvtItemClick(areaEventList[0], data);
        else
        {
            if(change && areaEventList.Count > 0)
            {
                OnEvtItemClick(areaEventList[0], data);
            }
            else
            {
                OnEvtItemClick(curMissionRecord, data);
            }
        }
    }

    private void ShowRecordNoneBg(bool show)
    {
        View.RecordNoneBg_Trans.gameObject.SetActive(show);
        View.RecordRightBG_Trans.gameObject.SetActive(!show);
    }

    private void SetAreaEvtDepth(RecordEvtItemController curItem)
    {
        int max = 8;
        areaEventList.ForEach(e =>
        {
            if (e.idx == curItem.idx)
            {
                e.UpdateDepth(9);
            }
            else
            {
                e.UpdateDepth(max);
            }
            max--;
        });
    }
    public void OnEvtItemClick(RecordEvtItemController item, IMissionData data)
    {
        var missionRecord = item.MissionRecord;
        if (curEvtId == missionRecord.id) return;

        SetAreaEvtDepth(item);
        if (curMissionRecord != null)
            curMissionRecord.UpdateSprite(false);
        item.UpdateSprite(true);

        curMissionRecord = item;
        curEvtId = missionRecord.id;

        View.lblEvt_UILabel.text = item.IsEvtOpen ? missionRecord.name : "暂无记录";
        View.lblRecordDes_UILabel.text = item.IsEvtOpen ? missionRecord.desc : "暂无记录";
        UpdateRewardItem(missionRecord.reward,rewardList,View.rewardGrid_UIGrid);
        UpdateRewardPanel(missionRecord,data,item.IsEvtOpen);
        SpringPanel.Begin(View.rewardPanel, new Vector3(137, -153, 0), 8);
    }
    //奖励物品
    private void UpdateRewardItem(List<ItemDto> list, List<ItemCellController> poolList,UIGrid parent)
    {
        int poolCount = poolList.Count;
        int eventCount = list.Count;
        for (int i = 0; i < poolCount; i++)
        {
            poolList[i].gameObject.SetActive(i < eventCount);
            if (i < eventCount)
            {
                var tMissionRecord = list[i];
                poolList[i].UpdateView(tMissionRecord, parent.transform,i);
            }
        }

        for (int i = poolCount; i < eventCount; i++)
        {
            var tMissionRecord = list[i];
            var item = AddChild<ItemCellController, ItemCell>(
                 parent.gameObject,
                 ItemCell.Prefab_BagItemCell
                 );
            item.UpdateView(tMissionRecord, parent.transform,i);
            poolList.Add(item);
        }
        parent.Reposition();
    }
    private void UpdateRewardPanel(MissionRecord missionRecord,IMissionData data,bool isEvtOpen)
    {
        if (missionRecord.missionList.Count == 0 || isEvtOpen == false)
        {
            View.RecordBtn_UIButton.gameObject.SetActive(false);
            View.rewardGrid_UIGrid.gameObject.SetActive(false);
            recordType = RecordRewardType.None;
            return;
        }
        View.rewardGrid_UIGrid.gameObject.SetActive(true);
        View.RecordBtn_UIButton.gameObject.SetActive(true);
        bool isReceived = IsReceived(missionRecord, data);
        bool isAllFinished = IsAllFinished(missionRecord, data);
        if (isReceived)
            View.lblRecordBtn_UILabel.text = "已报告";
        else
            View.lblRecordBtn_UILabel.text = isAllFinished ? "领取报酬" : "任务详情";
        recordType = isReceived ? RecordRewardType.HasReceiveed : isAllFinished ? RecordRewardType.Receive : RecordRewardType.Details;
    }
    public void OnRecordDetailBtnClick(IMissionData data)
    {
        if(curMissionRecord == null)
        {
            GameDebuger.LogError("当前日志为空");
            return;
        }
        if(recordType == RecordRewardType.Details)
        {
            ShowOrHideWindow(true);
            UpdateRecordWindowView(data);
        }
        else if(recordType == RecordRewardType.Receive)
        {
            MissionDataMgr.MissionNetMsg.ReqReceiveRewardByRecord(curEvtId);
        }
        else if(recordType == RecordRewardType.HasReceiveed)
        {
            TipManager.AddTip("你已接取奖励");
        }
    }
    //更新日志窗口任务
    private void UpdateRecordWindowView(IMissionData data)
    {
        var missionList = curMissionRecord.MissionRecord.missionList;
        int poolCount = recordDetailItemList.Count;
        int count = missionList.Count;
        for(int i = 0; i < poolCount; i++)
        {
            recordDetailItemList[i].gameObject.SetActive(i < count);
            if (i < count)
            {
                var id = missionList[i];
                recordDetailItemList[i].UpdateView(id, data);
            }
        }
        for(int i = poolCount; i < count; i++)
        {
            var item = AddChild<RecordDetailItemController, RecordDetailItem>(
                View.Anchor_UIGrid.gameObject,
                RecordDetailItem.NAME
                );
            var id = missionList[i];
            item.UpdateView(id, data);
            _disposable.Add(item.OnRecordDetailItem_UIButtonClick.Subscribe(e => { recordDetailItem_UIButtonEvt.OnNext(item); }));
            _disposable.Add(item.OnState_UIButtonClick.Subscribe(e => { recordStateBtnEvt.OnNext(item); }));
            recordDetailItemList.Add(item);
        }
        View.Anchor_UIGrid.Reposition();
    }

    public void OnRecordDetailItemClick(RecordDetailItemController item)
    {
        item.SetParent(View.MissDetailDes_UISprite.transform);
        View.MissDetailDes_UISprite.gameObject.SetActive(true);
        View.MissDetailDes_UISprite.transform.localPosition = UnityEngine.Vector3.zero;
        if(item.Type != RecordDetailItemController.MissionDetailType.NoAccept)
        {
            View.lblDetailDes_UILabel.text = item.Des;
        }
        else
        {
            View.lblDetailDes_UILabel.text = item.Reason;
            View.MissDetailDes_UISprite.gameObject.SetActive(false);
        }
        int height = View.lblDetailDes_UILabel.height - 6;
        int rate = height / 18;
        View.MissDetailDes_UISprite.height = 28 * rate;
    }
    public void OnStateBtnClick(RecordDetailItemController item,IMissionData data)
    {
        if(item.Type == RecordDetailItemController.MissionDetailType.CanAccept)
        {
            OnTabBtnClick(MisViewTab.CanAccess, data);
            tabMgr.SetTabBtn((int)MisViewTab.CanAccess);
            ShowOrHideWindow(false);
        }
    }
    private bool IsReceived(MissionRecord missionRecord, IMissionData data)
    {
        var receiveList = data.RecordsList.ToList();
        int evtID = missionRecord.id;
        return (receiveList.Contains(evtID));
    }
    private bool IsAllFinished(MissionRecord missionRecord,IMissionData data)
    {
        var submitedList = data.GetPreMissionIDList();
        var misList = missionRecord.missionList;
        bool isAllFinished = true;
        for(int i = 0,max = misList.Count; i < max; i++)
        {
            //没有全部完成
            if (!submitedList.Contains(misList[i]))
            {
                isAllFinished = false;
                break;
            }
        }
        return isAllFinished;
    }
    public void ShowOrHideWindow(bool show)
    {
        View.BaseTipWindow_Trans.gameObject.SetActive(show);
        View.MissDetailDes_UISprite.gameObject.SetActive(false);
    }
    #endregion

    //已接可接按钮点击
    private void OnAcceptBtnClick(IMissionData data)
    {
        ShowRightPanel(true);
        var _missionTypeList = GetMainMissionMenuList(data);
        int poolCount = typeItemList.Count;
        int typeListCount = _missionTypeList.Count;
        for (int i = 0; i < poolCount; i++)
        {
            typeItemList[i].gameObject.SetActive(i < typeListCount);
            if (i < typeListCount)
            {
                MissionType tMissionType = _missionTypeList[i];
                typeItemList[i].UpdateView(tMissionType,data);
                UpdateSubItem(typeItemList[i], data);
            }
        }
        for(int i = poolCount, max = typeListCount; i < max; i++)
        {
            MissionType tMissionType = _missionTypeList[i];

            var item = AddChild<MissionTypeItemController, MissionTypeItem>(
                View.table_UITable.gameObject,
                MissionTypeItem.NAME
                );
            item.UpdateView(tMissionType,data);
            item.SetIndex(i);
            UpdateSubItem(item, data);
            _disposable.Add(item.MissionTypeItem_UIButtonClick.Subscribe(e => { misTypeItemEvt.OnNext(item); }));
            typeItemList.Add(item);
        }
        //默认展示第一个任务类型
        if (typeListCount > 0)
        {
            SetNoneMis(true);
            UIPlayTween tween = typeItemList[0].gameObject.GetComponent<UIPlayTween>();
            tween.playDirection = AnimationOrTween.Direction.Forward;
            tween.Play(true);
            tween.playDirection = AnimationOrTween.Direction.Toggle;
            OnMisTypeItemClick(typeItemList[0]);
            if (clickByTab)
            {
                if (typeItemList[0].SubItemList.Count > 0)
                {
                    OnSubItemClick(typeItemList[0].SubItemList[0]);
                    clickByTab = false;
                }
            }
        }
        else
        {
            //如果没有则展示其他内容TODO
            curMission = null;
            curMisId = -1;
            SetNoneMis(false);
        }
    }
    

    public void OnMisTypeItemClick(MissionTypeItemController item)
    {
        UpdateTypeItemState();
        if (item == null || curMisTypeIdx == item.Index) return;
        //if (curTypeItem != null) curTypeItem.UpdateClickView(false);
        item.gameObject.GetComponent<UIPlayTween>().enabled = true;
        curTypeItem = item;
        curMisTypeIdx = item.Index;
        //item.UpdateClickView(true);
        var list = item.SubItemList;
        if(lastSubItem !=null && lastSubItem.BelongItem == item) 
        {
            OnSubItemClick(lastSubItem);
        }
        else
        {
            if (list.Count > 0)
                OnSubItemClick(list[0]);
        }
    }
    private void SetNoneMis(bool show)
    {
        View.litRightPanel_Trans.gameObject.SetActive(show);
        View.NoneBg_Trans.gameObject.SetActive(!show);
    }
    
    /// <summary>
    /// 生成二级任务
    /// </summary>
    /// <param name="itemCtrl"></param>
    /// <param name="data"></param>
    private void UpdateSubItem(IMissionTypeItemController itemCtrl, IMissionData data)
    {
        List<Mission> tSubMissionMenuList = GetSubdivisionMenuList(itemCtrl.MissionType.id, data);
        
        int poolCount = itemCtrl.SubItemList.Count;
        int subMax = tSubMissionMenuList.Count;
        for(int i = 0; i < poolCount; i++)
        {
            if(i<subMax)
            {
                Mission mission = tSubMissionMenuList[i];
                itemCtrl.SubItemList[i].UpdateView(mission,itemCtrl,curTab,data);
                itemCtrl.SubItemList[i].SetParent(itemCtrl.Tween_UIGrid.transform);
            }
            else itemCtrl.SubItemList[i].SetParent(itemCtrl.PoolParent);
        }

        for (int i = poolCount; i < subMax; i++)
        {
            Mission mission = tSubMissionMenuList[i];
            //if(mission.type == itemCtrl.MissionType.id)
            //{
                var item = AddCachedChild<SubMissionItemController, SubMissionItem>(
                itemCtrl.Tween_UIGrid.gameObject,
                SubMissionItem.NAME
                );
                item.UpdateView(mission,itemCtrl,curTab,data);
                _disposable.Add(item.OnSubMissionItem_UIButtonClick.Subscribe(e => subItemEvt.OnNext(item)));
                itemCtrl.SubItemList.Add(item);
            //}
        }
        itemCtrl.Tween_UIGrid.Reposition();
    }

    public void OnSubItemClick(SubMissionItemController item)
    {
        var mission = item.SubItemMission;
        #region ITEMView
        if (curMisId == mission.id || mission == null) return;
        if (lastSubItem != null) lastSubItem.UpdateViewByClick(false);
        lastSubItem = item;
        item.UpdateViewByClick(true);
        #endregion
        curMission = mission;
        curMisId = mission.id;
        View.lblName_UILabel.text = MissionHelper.GetMissionTitleName(mission);
        View.lblTargetTips_UILabel.text = MissionHelper.GetCurTargetContent(mission,null, curTab);
        View.lblDesTips_UILabel.text = MissionHelper.GetCurDescriptionContent(mission,null,curTab);
        if (curTab == MisViewTab.Accepted)
            View.misRewardGrid.gameObject.SetActive(true);
        else
            View.misRewardGrid.gameObject.SetActive(mission.type < (int)MissionType.MissionTypeEnum.Faction);
        UpdateRewardItem(mission.rewards, misRewardList,View.misRewardGrid);
        OnMisTypeItemClick(item.BelongItem);
        SpringPanel.Begin(View.misRewardPanel, new Vector3(44, -154, 0), 8);
    }

    private void UpdateTypeItemState()
    {
        for(int i = 0,max= typeItemList.Count; i < max; i++)
        {
            var e = typeItemList[i];
            var tween = e.gameObject.GetComponent<UIPlayTween>();
            if (!e.SubItemList.Contains(lastSubItem) && e.View.tween_UIGrid.transform.localScale.y == 1 && e.gameObject.activeSelf)
            {
                tween.enabled = false;
                continue;
            }
            else
            {
                tween.enabled = true;
            }
        }
    }
    

    #region 获取一级菜单信息 TODO
    //获取一级菜单信息
    //TODO 目前只做了已接的任务数据处理,后面还有可接任务
    private List<MissionType> GetMainMissionMenuList(IMissionData data)
    {

        /*
        *  |-- 主线
        *  |-- 支线
        *  |-- 日常
        *  |-- 副本
        */
        List<Mission> subMissionMenuList = curTab == MisViewTab.Accepted ? data.GetExistSubMissionMenuList() : data.GetMissedSubMissionMenuList();
        List<MissionType> tMainMissionMenuList = new List<MissionType>();

        for(int i =0,max = subMissionMenuList.Count; i < max; i++)
        {
            Mission tMission = subMissionMenuList[i];
            if(tMission.missionType == null)
            {
                GameDebuger.LogError(string.Format("数据错误 -> ID: {0} | Name: {1}", tMission.name, tMission.id));
                return null;
            }

            //当前任务类型是否已存在列表中
            bool tExitsInList = false;
            for(int j = 0,len = tMainMissionMenuList.Count; j < len; j++)
            {
                if (MissionHelper.IsMainOrExtension(tMission))
                {
                    if (tMission.type == tMainMissionMenuList[j].id)
                    {
                        tExitsInList = true;
                        break;
                    }
                }
                else if(tMission.type >= (int)MissionType.MissionTypeEnum.Faction && tMission.type < (int)MissionType.MissionTypeEnum.Copy)
                {
                    //日常
                    if( tMainMissionMenuList[j].id >= (int)MissionType.MissionTypeEnum.Faction && tMainMissionMenuList[j].id < (int)MissionType.MissionTypeEnum.Copy)
                    {
                        tExitsInList = true;
                        break;
                    }
                }
                else if(tMission.type >= (int)MissionType.MissionTypeEnum.Copy && tMission.type <= (int)MissionType.MissionTypeEnum.CopyExtra)
                {
                    //副本任务
                    if (tMainMissionMenuList[j].id >= (int)MissionType.MissionTypeEnum.Copy && tMainMissionMenuList[j].id <= (int)MissionType.MissionTypeEnum.CopyExtra)
                    {
                        tExitsInList = true;
                        break;
                    }
                }
                else
                {
                    GameDebuger.OrangeDebugLog(String.Format("ERROR：未分大类任务 ID：{0} | Name：{1}-{2}",
                                               tMission.id, tMission.missionType.name, tMission.name));
                    break;
                }
            }

            if (!tExitsInList)
            {
                tMainMissionMenuList.Add(tMission.missionType);
            }
        }

        //排序
        tMainMissionMenuList.Sort((x,y) =>
        {
            return x.id.CompareTo(y.id);
        });

        return tMainMissionMenuList;
    }

    #endregion

    #region 获取二级菜单 TODO
    /// <summary>
    /// 获取二级菜单
    /// </summary>
    /// <param name="menuID"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private List<Mission> GetSubdivisionMenuList(int menuID, IMissionData data)
    {
        List<Mission> tSubMissionMenuList = curTab == MisViewTab.Accepted ? data.GetExistSubMissionMenuList() : data.GetMissedSubMissionMenuList();
        List<Mission> tSubdivisionMenuList = new List<Mission>();
        List<int> typeList = new List<int>();   //任务类型（MissionType）
        List<int> copyTypeList = new List<int>();//副本任务类型（Copy)
        for (int i = 0, len = tSubMissionMenuList.Count; i < len; i++)
        {
            Mission tMission = tSubMissionMenuList[i];
            MissionType.MissionTypeEnum missionType = (MissionType.MissionTypeEnum)tMission.type;
            MissionType.MissionTypeEnum menuType = (MissionType.MissionTypeEnum)menuID;
            if (missionType >= MissionType.MissionTypeEnum.Faction && menuType >= MissionType.MissionTypeEnum.Faction && missionType < MissionType.MissionTypeEnum.Copy && menuType < MissionType.MissionTypeEnum.Copy)
            {
                if (!typeList.Contains(tMission.type))
                {
                    tSubdivisionMenuList.Add(tMission);
                    typeList.Add(tMission.type);
                }
            }
            else if(missionType >= MissionType.MissionTypeEnum.Copy && menuType >= MissionType.MissionTypeEnum.Copy && missionType <= MissionType.MissionTypeEnum.CopyExtra && menuType <= MissionType.MissionTypeEnum.CopyExtra)
            {
                if(missionType == MissionType.MissionTypeEnum.Copy && menuType == MissionType.MissionTypeEnum.Copy)
                {
                    var copyList = data.GetCopyMisConfig;
                    var val = copyList.Find(e => e.id == tMission.id);
                    if (val != null)
                    {
                        if (!copyTypeList.Contains(val.copyId))
                        {
                            tSubdivisionMenuList.Add(tMission);
                            copyTypeList.Add(val.copyId);
                        }
                    }
                }
                else if(missionType == MissionType.MissionTypeEnum.CopyExtra && menuType == MissionType.MissionTypeEnum.CopyExtra)
                {
                    if (!typeList.Contains(tMission.type))
                    {
                        tSubdivisionMenuList.Add(tMission);
                        typeList.Add(tMission.type);
                    }
                }
            }
            else if (tMission.type == menuID)
            {
                tSubdivisionMenuList.Add(tMission);
            }
        }
        return tSubdivisionMenuList;
    }
    
    #endregion

    public Mission CurMission { get { return curMission; } }


}
