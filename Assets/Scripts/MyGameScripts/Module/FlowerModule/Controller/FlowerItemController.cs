// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial class FlowerItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnBg_UIButtonClick.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });

        OnIconBg_UIButtonClick.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
            var tipsCtrl = ProxyTips.OpenGeneralItemTips(ItemHelper.GetGeneralItemByItemId(_id));
            tipsCtrl.SetTipsPosition(new UnityEngine.Vector3(-106, -17));
            GainWayTipsViewController.OpenGainWayTip(_id, new UnityEngine.Vector3(290, -46));
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private int _id = 0;
    public void UpdateView(int id, bool isChose)
    {
        _id = id;
        var itemData = ItemHelper.GetGeneralItemByItemId(id) as Props;
        if (itemData == null || itemData.propsParam as PropsParam_16 == null) return;

        UIHelper.SetItemIcon(View.Icon_UISprite, itemData.icon);
        View.Name_UILabel.text = itemData.name;
        View.DegreeLabel_UILabel.text = string.Format("+{0}", (itemData.propsParam as PropsParam_16).degree);
        View.ChoseBg_UISprite.enabled = isChose;
        View.CountLabel_UILabel.text = BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(id).ToString();
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
