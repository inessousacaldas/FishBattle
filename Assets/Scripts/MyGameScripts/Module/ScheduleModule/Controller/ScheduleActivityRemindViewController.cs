// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityRemindViewController.cs
// Author   : xjd
// Created  : 1/19/2018 5:45:16 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IScheduleActivityRemindViewController
{

}
public partial class ScheduleActivityRemindViewController    {

    public static IScheduleActivityRemindViewController Show<T>(
        string moduleName
        , UILayerType layerType
        , bool addBgMask
        , bool bgMaskClose = true)
        where T : MonoController, IScheduleActivityRemindViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IScheduleActivityRemindViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        UpdateView();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            _pushActivityDic.ForEach(itemPair =>
            {
                ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqActivityNotify(itemPair.Key, itemPair.Value);
                if (itemPair.Value)
                    ModelManager.Player.RemoveScheduleCancelId(itemPair.Key);
                else
                    ModelManager.Player.AddScheduleCancelId(itemPair.Key);
            });

            UIModuleManager.Instance.CloseModule(ScheduleActivityRemindView.NAME);
            ScheduleMainViewDataMgr.DataMgr.UpdateCancelList();
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

    private List<ScheduleActivityRemindItemController> _remindItemCtrlList = new List<ScheduleActivityRemindItemController>();
    private Dictionary<int, ScheduleActivity> _scheduleActivityDic = DataCache.getDicByCls<ScheduleActivity>();
    private Dictionary<int, bool> _pushActivityDic = new Dictionary<int, bool>();
    CompositeDisposable _disposable = new CompositeDisposable();
    private void UpdateView()
    {
        _disposable.Clear();
        var cancelList = ScheduleMainViewDataMgr.DataMgr.GetCancleList();
        _remindItemCtrlList.ForEach(item => { item.Hide(); });
        _scheduleActivityDic.ForEachI((itemData, index) =>
        {
            if(itemData.Value.deliver)
            {
                var ctrl = AddRemindItemIfNotExist(index);
                ctrl.UpdateView(itemData.Value, cancelList.ToList().Contains(itemData.Value.id));
                ctrl.Show();

                _disposable.Add(ctrl.OnClickItemStream.Subscribe(isChose =>
                {
                    if (_pushActivityDic.ContainsKey(itemData.Value.id))
                        _pushActivityDic[itemData.Value.id] = isChose;
                    else
                        _pushActivityDic.Add(itemData.Value.id, isChose);
                }));
            }
        });

        View.Grid_UIGrid.Reposition();
    }

    private ScheduleActivityRemindItemController AddRemindItemIfNotExist(int idx)
    {
        ScheduleActivityRemindItemController ctrl = null;
        _remindItemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleActivityRemindItemController, ScheduleActivityRemindItem>(View.Grid_UIGrid.gameObject, ScheduleActivityRemindItem.NAME);
            _remindItemCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
