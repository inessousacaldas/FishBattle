// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistDelegateFriendViewController.cs
// Author   : xjd
// Created  : 11/10/2017 8:52:05 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;

public partial interface IAssistDelegateFriendViewController
{
    void UpdateView(IEnumerable<long> helpedFriendList = null);
}

public partial class AssistDelegateFriendViewController    {

    public static IAssistDelegateFriendViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IAssistDelegateFriendViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IAssistDelegateFriendViewController;
            
        return controller;        
    }

    CompositeDisposable _disposable = new CompositeDisposable();
    private List<AssistFriendItemController> _itemCtrlList = new List<AssistFriendItemController>();

    TabbtnManager tabBtnMgr;
    private Func<int, ITabBtnController> func;
    private int _curTab = 0;
    private List<long> _helpedFriendList = new List<long>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            View.TabGird_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
            , "Tabbtn_" + i);

        tabBtnMgr = TabbtnManager.Create(AssistSkillMainDataMgr.AssistSkillMainData._ChoseFriendTabInfos, func);
        tabBtnMgr.SetBtnLblFont(20, "2e2e2e", 18, "bdbdbd");
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            Close();
        });

        tabBtnMgr.Stream.Subscribe(i =>
        {
            _curTab = i;
            UpdateView();
        });
    }

    protected override void RemoveCustomEvent ()
    {
        
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    public void UpdateView(IEnumerable<long> helpedFriendList = null)
    {
        if(FriendDataMgr.DataMgr.GetMyFriendList().ToList().IsNullOrEmpty())
        {
            View.NoFriendTips.gameObject.SetActive(true);
            UIHelper.SetUITexture(View.Texture, "npc_311", false);
            return;
        }

        if (helpedFriendList != null)
            _helpedFriendList = helpedFriendList.ToList();

        View.NoFriendTips.gameObject.SetActive(false);
        _disposable.Clear();
        _itemCtrlList.ForEach(item => { item.Hide(); });

        switch (_curTab)
        {
            case (int)AssistFriendViewTab.DelegateFriend:
                var friendList = FriendDataMgr.DataMgr.GetMyFriendList();
                friendList.ForEachI((infoItem, index) =>
                {
                    var ctrl = AddPlayerCellIfNotExist(index);
                    ctrl.UpdateView(infoItem, _helpedFriendList.Contains(infoItem.friendId));
                    ctrl.Show();

                    _disposable.Add(ctrl.OnClickInviteStream.Subscribe(friendId =>
                    {
                        AssistSkillMainDataMgr.DataMgr.SetChoseFriendId(friendId);
                        Close();
                     
                        if (_helpedFriendList.Contains(friendId))
                        {
                            TipManager.AddTip("该玩家今天已协助过你了");
                        }
                    }));
                });
                break;
            case (int)AssistFriendViewTab.DelegateGuild:
                break;
        }
    }

    private AssistFriendItemController AddPlayerCellIfNotExist(int idx)
    {
        AssistFriendItemController ctrl = null;
        _itemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<AssistFriendItemController, AssistFriendItem>(View.Grid_UIGrid.gameObject, AssistFriendItem.NAME);
            _itemCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    private void Close()
    { 
        ProxyAssistSkillMain.CloseDelegateFriendView();
    }
}
