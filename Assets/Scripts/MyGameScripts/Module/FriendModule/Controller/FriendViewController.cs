// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using AppDto;
using UnityEngine;

public partial interface IFriendViewController
{
    UniRx.IObservable<FriendInfoDto> OnFriendChatStream { get; }
}

public partial class FriendViewController
{
    private TabbtnManager tabBtnMgr;
    private Func<int, ITabBtnController> func;
    public TabbtnManager TabBtnMgr { get { return tabBtnMgr; } }

    private List<FriendItemController> _friendItemList = new List<FriendItemController>();

    private CompositeDisposable _disposable = null;

    private Subject<FriendInfoDto> friendChatStream = new Subject<FriendInfoDto>();
    public UniRx.IObservable<FriendInfoDto> OnFriendChatStream
    {
        get { return friendChatStream; }
    }

    private const int FriendMaxCount = 40;
    private const int BlackMaxCount = 50;
    private const int BoxCount = 6; //每页显示6个好友
    private int _friendItemHeight = 0;
    private FriendViewTab _lastTab = FriendViewTab.MyFriend;

    protected override void AfterInitView()
    {
        tabBtnMgr = TabbtnManager.Create(
            FriendDataMgr.FriendData._TabInfos
            , AddChannelTabBtn
            , 0
        );
        View.LeftTable_UITable.Reposition();

        if (_disposable == null)
            _disposable = new CompositeDisposable();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {         
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    private ITabBtnController AddChannelTabBtn(int i)
    {
        return AddTabBtn(i, _view.LeftTable_UITable.gameObject, TabbtnPrefabPath.TabBtnWidget_ChatTab, "Tabbtn_");
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, TabbtnPrefabPath tabPath, string name)
    {
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath.ToString()
            , name + i);

        ctrl.SetBtnImages("betton_b", "betton_a");
        ctrl.SetBtnLblFont(selectColor: "65480b", normalColor: "636262");
        return ctrl;
    }

    public void UpdateView(IFriendData data, bool isDefault=false)
    {
        var infoList = new List<FriendInfoDto>();
        tabBtnMgr.SetTabBtn((int)data.CurTab);

        switch (data.CurTab)
        {
            case FriendViewTab.MyFriend:
                {
                    infoList = data.CacheFriendInfoDtos;
                    View.Label_UILabel.gameObject.SetActive(true);
                    View.Label_UILabel.text = "好友：";
                    View.FriendLabel_UILabel.text = string.Format("{0}/{1}", infoList.Count, FriendMaxCount);
                    break;
                }
            case FriendViewTab.BlackFriend:
                {
                    infoList = data.CacheBlackList;
                    View.Label_UILabel.gameObject.SetActive(true);
                    View.Label_UILabel.text = "黑名单：";
                    View.FriendLabel_UILabel.text = string.Format("{0}/{1}", infoList.Count, BlackMaxCount);
                    break;
                }
            case FriendViewTab.RecentlyTeammates:
                {
                    infoList = data.CacheTeammateList;
                    View.Label_UILabel.gameObject.SetActive(false);
                    break;
                }
            case FriendViewTab.AddFriend:
                {
                    infoList = new List<FriendInfoDto>();
                    View.Label_UILabel.gameObject.SetActive(false);
                    //tabBtnMgr.SetTabBtn((int)data.CurTab);
                    tabBtnMgr.SetTabBtn(-1);

                    if (!UIModuleManager.Instance.IsModuleOpened(SearchFriendView.NAME))
                    {
                        SearchFriendDataMgr.SearchFriendViewLogic.Open();
                        SearchFriendDataMgr.SearchFriendNetMsg.ReqRecommendFriendList();
                        //ProxySociality.CloseChatInfoView();
                    }

                    break;
                }
        }

        var itemCount = 0;
        _friendItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());
        _disposable.Clear();

        infoList.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddFriendItemIfNotExist(index);
            itemCtrl.UpdateView(data.CurTab, itemDto);
            itemCtrl.Show();
            if (_friendItemHeight <= 0)
                _friendItemHeight = _friendItemList[0].GetHeigt();

            _disposable.Add(itemCtrl.OnClickItemStream.Subscribe(item =>
            {
                var ctrl = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME, UILayerType.ThreeModule, false, true);
                ctrl.UpdateView(itemDto);
                ctrl.SetPosition(new Vector3(120, 210));
            }));

            _disposable.Add(itemCtrl.OnClickChatStream.Subscribe(item =>
            {
                friendChatStream.OnNext(itemDto);
            }));
        });

        View.ItemTable_UITable.Reposition();
        //删除或拉黑好友后 自动刷新位置
        if (infoList.Count > 6 && View.RightScrollView_UIScrollView.panel.clipOffset.y < _friendItemHeight * (6- infoList.Count))
        {
            _view.RightScrollView_UIScrollView.ResetPosition();
            View.RightScrollView_UIScrollView.SetDragAmount(0f, 1f, false);
        }
            
        if (_lastTab != data.CurTab)
            View.RightScrollView_UIScrollView.ResetPosition();

        if (infoList.Count <= 6)
        {
            View.RightScrollView_UIScrollView.transform.localPosition = new Vector3(View.RightScrollView_UIScrollView.transform.localPosition.x, -21);
            View.RightScrollView_UIScrollView.panel.clipOffset = new Vector2(0, 0);
            View.RightScrollView_UIScrollView.SetDragAmount(0f, 0f, true);
        }

        _lastTab = data.CurTab;
    }

    private FriendItemController AddFriendItemIfNotExist(int idx)
    {
        FriendItemController ctrl = null;
        _friendItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<FriendItemController, FriendItem>(View.ItemTable_UITable.gameObject, FriendItem.NAME);
            _friendItemList.Add(ctrl);
        }

        return ctrl;
    }
}

