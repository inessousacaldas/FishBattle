// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamInvitationViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface ITeamInvitationViewController
{
    IObservable<TeamDto> OnBtnClickEvt { get;}
    IObservable<TeamInvitationNotify> OnNotifyEvt { get; } 
}
public partial class TeamInvitationViewController
{
    private const int ItemCnt = 6;
    private const int ListCount = 8;    //一页显示8个

    private ITeamData _data;
    private List<TeamEasyGroupItemController> itemList = new List<TeamEasyGroupItemController>(ItemCnt);
    private List<UISprite> _memberList = new List<UISprite>();

    private Dictionary<GameObject, TeamEasyGroupItemController> testList =
        new Dictionary<GameObject, TeamEasyGroupItemController>();

    private Subject<TeamDto> _teamDtoClickEvt = new Subject<TeamDto>();
    public IObservable<TeamDto> OnBtnClickEvt{get { return _teamDtoClickEvt; }}

    private Subject<TeamInvitationNotify> _notifyClickEvt = new Subject<TeamInvitationNotify>(); 
    public IObservable<TeamInvitationNotify>OnNotifyEvt { get { return _notifyClickEvt; } } 

    private static CompositeDisposable _disposable;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _view.ItemGrid_UIRecycledList.gameObject.SetActive(false);
        InitInviteItem();

        for (int i = 0; i < _view.memberGrid_Transform.childCount; i++)
        {
            var item = _view.memberGrid_Transform.GetChild(0).GetComponent<UISprite>();
            _memberList.Add(item);
        }
    }

    private void InitInviteItem()
    {
        for (int i = 0; i < ItemCnt; ++i)
        {
            var com = AddChild<TeamEasyGroupItemController, TeamEasyGroupInfoItem>(
                View.ItemUIRecycledList.gameObject.gameObject
                , TeamEasyGroupInfoItem.NAME
                , TeamEasyGroupInfoItem.NAME + i
            );
            
            _disposable.Add(com.OnItemClickEvent.Subscribe(dto =>
            {
                _teamDtoClickEvt.OnNext(dto);
            }));

            _disposable.Add(com.OnItemClick.Subscribe(dto =>
            {
                _notifyClickEvt.OnNext(dto);
            }));

            testList.Add(com.gameObject, com);
        }
    }

    //机器自动生成的事件订阅
    protected override void RegistCustomEvent()
    {
        View.ItemUIRecycledList.onUpdateItem = delegate(GameObject item, int itemIdx, int dataIdx)
        {
            TeamEasyGroupItemController cell;
            if (testList.TryGetValue(item, out cell))
            {
                cell.UpdateData(_data.TeamInviteViewData.GetTeamInvitationNotifyByIndex(dataIdx));
            }
        };
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        View.ItemUIRecycledList.onUpdateItem = null;
        _disposable = _disposable.CloseOnceNull();
        _teamDtoClickEvt = _teamDtoClickEvt.CloseOnceNull();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        if(_disposable == null)
            _disposable = new CompositeDisposable();
    }

    protected override void UpdateDataAndView(ITeamData data)
    {
        if (data == null || data.TeamInviteViewData == null)
            return;

        _data = data;
        _view.ItemGrid_UIRecycledList.gameObject.SetActive(true);
        var count = data.TeamInviteViewData.GetInvitationCount() > ListCount
            ? ListCount
            : data.TeamInviteViewData.GetInvitationCount();

        _memberList.ForEachI((item, idx)=>
        {
            int n = data.GetTeamDto == null ? 0 : data.GetTeamDto.members.Count;
            item.gameObject.SetActive(idx < n);
        });

        _view.ItemUIRecycledList.UpdateDataCount(count, false);
        _view.ScrollView.ResetPosition();
    }
}
