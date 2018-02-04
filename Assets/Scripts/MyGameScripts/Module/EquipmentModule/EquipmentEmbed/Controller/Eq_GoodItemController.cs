// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmbedItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;

public class Eq_GoodItemVo
{
    public AppItem item;
    public int count;
    public string des; //描述
    public Eq_GoodItemVo(AppItem item,int count,string des)
    {
        this.item = item;
        this.count = count;
        this.des = des;
    }
    //灰色屏蔽
    public bool isGrey;
}
public partial class Eq_GoodItemController
{
    ItemCellController IconCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        IconCtrl = AddChild<ItemCellController, ItemCell>(View.ItemCell,ItemCell.Prefab_BagItemCell);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateDataView(Eq_GoodItemVo vo)
    {
        View.NameLbl_UILabel.text = vo.item.name;
        View.DesLbl_UILabel.text = vo.des;
        IconCtrl.UpdateView(vo.item, vo.count);
    }
    public void UpdateDataView(BagItemDto dto)
    {
        View.NameLbl_UILabel.text = dto.item.name;
        var props = dto.item as Props;
        var embedParam = props.propsParam as PropsParam_1;
        var cbConfig = DataCache.getDtoByCls<CharacterAbility>(embedParam.caId);
        View.DesLbl_UILabel.text = string.Format("{0}+{1}", cbConfig.name,embedParam.value);

        IconCtrl.UpdateView(dto.item,dto.count);
    }
}
