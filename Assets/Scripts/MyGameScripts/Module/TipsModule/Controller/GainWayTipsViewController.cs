// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GainWayTipsViewController.cs
// Author   : xush
// Created  : 12/29/2017 3:03:22 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IGainWayTipsViewController
{
    void SetGainWayGrid(IEnumerable<int> list);
    void SetTipPos(Vector3 pos);
    void SetPropsId(int id);
}

public partial class GainWayTipsViewController
{
    private CompositeDisposable _disposable;
    private int _propsId;
    private int _recGainWayId;
    private List<BaseItemController> _itemList = new List<BaseItemController>();
    private List<GuideGainWay> _gainWayList = new List<GuideGainWay>();
    private static Action _callback = null;

    public static void OpenGainWayTip(int itemId, Vector3 pos, Action callback = null)
    {
        var props = DataCache.getDtoByCls<GeneralItem>(itemId) as Props;
        if (props == null || props.gainWayIds.IsNullOrEmpty()) return;

        var gainCtrl = GainWayTipsViewController.Show<GainWayTipsViewController>(
            GainWayTipsView.NAME,
            UILayerType.ThreeModule,
            false, false);

        gainCtrl.SetPropsId(props.id);
        gainCtrl.SetGainWayGrid(props.gainWayIds);
        gainCtrl.SetTipPos(pos);
        _callback = callback;
    }

    public static IGainWayTipsViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IGainWayTipsViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IGainWayTipsViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _gainWayList = DataCache.getArrayByCls<GuideGainWay>();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    UICamera.onClick += OnCameraClick;
	}

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnCameraClick;
    }
        
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    public void SetPropsId(int id)
    {
        _propsId = id;
        var appItem = DataCache.getDtoByCls<GeneralItem>(_propsId) as AppItem;
        _recGainWayId = appItem == null ? -1 : appItem.recGainWayId;
    }

    public void SetGainWayGrid(IEnumerable<int> list)
    {
        list.ForEachI((id, idx) =>
        {
            var item = AddChild<BaseItemController, BaseItem>(_view.ItemGrid_UIGrid.gameObject, BaseItem.NAME);
            _itemList.Add(item);
            _disposable.Add(item.OnBaseItem_UIButtonClick.Subscribe(_ =>
            {
                var guideGain = _gainWayList.Find(d => d.id == id);
                if (guideGain == null)
                {
                    GameDebuger.LogError("数据异常,请检查");
                    return;
                }
                SmartGuideHelper.GuideTo(guideGain.smartGuideId, _propsId, _callback);
            }));
            item.GainWayItemInfo(id);
            item.ShowMarkSprite = id == _recGainWayId;
        });

        _view.ItemGrid_UIGrid.Reposition();
        _view.ScrollView_UIScrollView.ResetPosition();
    }

    public void SetTipPos(Vector3 pos)
    {
        _view.Pos_Transform.localPosition = pos;
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != _view.ScrollView_UIScrollView.panel
            && panel != _view.GainWayTipsView_UIPanel)
            UIModuleManager.Instance.CloseModule(GainWayTipsView.NAME);
    }
}
