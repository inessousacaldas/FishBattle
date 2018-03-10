// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamApplicationViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public class MyClass
    {
        public TeamApplicationUnitedItemController Ctrl
        {
            get
            {
                return ctrl;
            }
            set
            {
                ctrl = value;
            }
        }
        private TeamApplicationUnitedItemController ctrl;
        public int DataIndex
        {
            get
            {
                return dataIndex;
            }
            set
            {
                dataIndex = value;
            }
        }

        private int dataIndex;

        public static MyClass Create(TeamApplicationUnitedItemController _ctrl, int _dataIndex)
        {
            var c = new MyClass();
            c.ctrl = _ctrl;
            c.dataIndex = _dataIndex;
            return c;
        }

        private MyClass()
        {

        }
    }

public partial interface ITeamApplicationViewController
{
    UniRx.IObservable<int> OnSubject { get; }
    void RefreshItemList(IEnumerable<TeamRequestNotify> data, Tuple<GameObject, int, int> tuple);
    UniRx.IObservable<Tuple<GameObject, int, int>> ListStream { get; }
//    void SetApplicationItem(ITeamData data);
}

public partial class TeamApplicationViewController
{
    private CompositeDisposable _disposable;

    private Subject<int> _onSubject = new Subject<int>();
    
    private Subject<Tuple<GameObject, int, int>> _listStream = new Subject<Tuple<GameObject, int, int>>();

    private Dictionary<GameObject, TeamApplicationUnitedItemController> _itemDic = new Dictionary<GameObject, TeamApplicationUnitedItemController>();

    public UniRx.IObservable<Tuple<GameObject, int, int>> ListStream
    {
        get { return _listStream; }
    }
    
    private static int ColNum = 2;        //每行2个

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        _disposable = new CompositeDisposable();
    }
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        InitItem();
    }

    private void InitItem()
    {
        for (int i = 0; i < TeamDataMgr.ApplicationShowItemNum; ++i)
        {
            var ctrl = AddChild<TeamApplicationUnitedItemController, TeamApplicationUnitedItem>(
                View.ItemGrid_GO.gameObject
                , TeamApplicationUnitedItem.NAME
                , "TeamApplication_" + i);

            _itemDic.Add(ctrl.gameObject, ctrl);
            var d = ctrl.GetSubject.Subscribe(idx =>
            {
                _onSubject.OnNext(idx);
            });

            _disposable.Add(d);
        }      
    }
    
    //机器自动生成的事件订阅
    protected override void RegistCustomEvent()
    {
        View.itemGrid_UIRecycledList.onUpdateItem = delegate(GameObject item, int itemIndex, int dataIndex)
        {
            _listStream.OnNext(Tuple.Create(item, itemIndex, dataIndex));
        };        
    }  
    
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _listStream = _listStream.CloseOnceNull();
        _onSubject = _onSubject.CloseOnceNull();

        base.OnDispose();
    }

    public UniRx.IObservable<int> OnSubject
    {
        get { return _onSubject; }
    }

    public void RefreshItemList(IEnumerable<TeamRequestNotify> data, Tuple<GameObject, int, int> tuple)
    { 
        if (data == null || tuple == null)
            return;

        var item = _itemDic[tuple.p1];
        item.UpdateItemInfo(data, tuple.p3);
    }

    protected override void UpdateDataAndView(ITeamData data)
    {
        var _data = data.TeamApplyViewData;
        if (View == null)
            return;
        View.teamInfoLbl_UILabel.text = string.Format("当前队伍：{0}/4", _data.GetMemberCount());

        var _cnt = _data.GetApplicationCnt() % ColNum == 1
            ? Mathf.CeilToInt(_data.GetApplicationCnt() / ColNum) + 1
            : Mathf.CeilToInt(_data.GetApplicationCnt() / ColNum);
        var RealCnt = _cnt > TeamDataMgr.TeamDefaultShowApplyCnt ? TeamDataMgr.TeamDefaultShowApplyCnt : _cnt;
        View.itemGrid_UIRecycledList.UpdateDataCount(RealCnt, true);
        View.ScrollView_UIScrollView.ResetPosition();

    }

}