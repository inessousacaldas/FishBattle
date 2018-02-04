// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SmithItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;
using System;
using Spine;
using UnityEngine;

public class SmithItemVo
{
    public Props props;
    public int needCount;
    public int currentCount;
}
public partial class SmithItemCellController
{
    private IDisposable _disposable;
    public ItemCellController itemCellCtrl;
    private int _itemId;
    private Vector3 _tipsPos = Vector3.zero;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        itemCellCtrl = AddChild<ItemCellController, ItemCell>(View.ItemCellAnchor, ItemCell.Prefab_BagItemCell);
        itemCellCtrl.SetShowTips(false);
        //默认按钮不可点击
        View.SmithItem_UIButton.enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        //加号默认隐藏
        View.AddBg_UISprite.enabled = false;
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _disposable = itemCellCtrl.OnCellClick.Subscribe(_ =>
        {
            var itemDto = DataCache.getDtoByCls<GeneralItem>(_itemId);
            if (itemDto != null)
            {
                GainWayTipsViewController.OpenGainWayTip(itemDto.id, new Vector3(50, 21, 0), ProxyEquipmentMain.CloseMainView);
                ProxyTips.OpenTipsWithGeneralItem(itemDto, new Vector3(-363, 41, 0));
            }
        });
    }


    protected override void OnDispose()
    {
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetTipsPos(Vector3 pos)
    {
        _tipsPos = pos;
    }

    public void UpdateViewData(SmithItemVo vo)
    {
        _itemId = vo.props.id;
        itemCellCtrl.Show();
        itemCellCtrl.UpdateView(vo.props);
        var curCntStr = vo.currentCount.ToString();
        if (vo.currentCount < vo.needCount)
            curCntStr = curCntStr.WrapColor(ColorConstantV3.Color_Red);
        View.CountLbl_UILabel.text = curCntStr + "/" + vo.needCount;
        if (vo.props != null)
            View.NameLbl_UILabel.text = vo.props.name.WrapColor(ItemHelper.GetItemNameColorByRank(vo.props.quality));
    }

    public void UpdateViewData(AppItem props)
    {
        if(props == null)
        {
            SetEmpty();
            return;
        }
        _itemId = props.id;
        itemCellCtrl.Show();
        itemCellCtrl.UpdateView(props);
        View.NameLbl_UILabel.text = props.name.WrapColor(ItemHelper.GetItemNameColorByRank( props.quality));
        View.CountLbl_UILabel.text = "";
    }
    public void SetEmpty()
    {
        View.NameLbl_UILabel.text = "";
        View.CountLbl_UILabel.text = "";
        itemCellCtrl.Hide();
    }

    public void SetGrey(bool grey, string iconName)
    {
        
        View.NameLbl_UILabel.text = "";
        //View.CountLbl_UILabel.text = "0/0".WrapColor("000000");
        View.CountLbl_UILabel.text = "";
        itemCellCtrl.SetIconGrey(grey, iconName);
        itemCellCtrl.SetBorderGrey(grey);
    }

    public void SetButtonEnable(bool b)
    {
        View.AddBg_UISprite.enabled = true;
        View.SmithItem_UIButton.enabled = b;
        this.gameObject.GetComponent<BoxCollider>().enabled = b;
    }
}
