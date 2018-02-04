// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecipeItemViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial class RecipeItemViewController
{
    private const int count = 4;
    private List<UIButton> _itemList = new List<UIButton>();
    private List<int> _itemIdList = new List<int>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _itemList.Add(View.Item_1_UIButton);
        _itemList.Add(View.Item_2_UIButton);
        _itemList.Add(View.Item_3_UIButton);
        _itemList.Add(View.Item_4_UIButton);

        for(int i=0;i<count;i++)
        {
            _itemIdList.Add(0);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Item_1_UIButtonEvt.Subscribe(_ =>
        {
            ShowTips(_itemIdList[0]);
        });

        Item_2_UIButtonEvt.Subscribe(_ =>
        {
            ShowTips(_itemIdList[1]);
        });

        Item_3_UIButtonEvt.Subscribe(_ =>
        {
            ShowTips(_itemIdList[2]);
        });

        Item_4_UIButtonEvt.Subscribe(_ =>
        {
            ShowTips(_itemIdList[3]);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void ShowTips(int id )
    {
        var generalItem = ItemHelper.GetGeneralItemByItemId(id);
        ProxyTips.OpenTipsWithGeneralItem(generalItem, new Vector3(-407, 159, 0));
    }

    public void UpdateView(AssistSkillMakeConsume data)
    {
        this.gameObject.SetActive(true);
        UIHelper.SetOtherIcon(View.RecepeIcon_UISprite, data.icon);
        View.RecepeName_UILabel.text = data.name;

        int index = 0;
        data.gradeMaitchItem.ForEachI((maitchItem,i) =>
        {
            maitchItem.itemId.ForEach(id =>
            {
                if (index >= count)
                    return;

                _itemIdList[index] = id;
                _itemList[index].gameObject.SetActive(true);
                UIHelper.SetItemIcon(_itemList[index].sprite, ItemHelper.GetItemIcon(id));
                index++;
            });
        });

        for(int i=count;i>index;i--)
        {
            _itemList[i - 1].gameObject.SetActive(false);
        }
    }

}
