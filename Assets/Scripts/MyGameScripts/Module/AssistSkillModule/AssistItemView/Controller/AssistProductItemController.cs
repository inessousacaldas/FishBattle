// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistProductItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using UnityEngine;

public partial class AssistProductItemController
{
    private int _id = 0;
    private bool _isTips = true;
    private bool _isRecipe = false;
    private bool _virtual = false;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        AssistProductItem_UIButtonEvt.Subscribe(_ =>
        {
            string str = ItemHelper.GetItemName(_id) == null ? "" : ItemHelper.GetItemName(_id);
            if(_isRecipe)
            {
                var ctrl = RecipeTipsController.Show<RecipeTipsController>(RecipeTips.NAME, UILayerType.ThreeModule, false, true);
                ctrl.InitTips(_id);
                ctrl.SetPosition(new Vector3(-276, 129));
            }
            else if(_virtual)
            {
                var ctrl = ProxyTips.OpenGeneralItemTips(ItemHelper.GetGeneralItemByItemId(_id) as AppVirtualItem);
                ctrl.SetTipsPosition(new Vector3(-407, 159, 0));
            }
            else
            {
                if (_isTips)
                {
                    var generalItem = ItemHelper.GetGeneralItemByItemId(_id);
                    ProxyTips.OpenTipsWithGeneralItem(generalItem, new Vector3(-407, 159, 0));
                }
                else
                    clickItemStream.OnNext(_id);
            }
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(int id, string str = "", bool isRecipe = false, string icon = "")
    {
        this.gameObject.SetActive(true);

        _id = id;
        _isRecipe = isRecipe;
        if(string.IsNullOrEmpty(icon))
            UIHelper.SetItemIcon(View.AssistProductItem_UIButton.sprite, ItemHelper.GetItemIcon(id));
        else
            UIHelper.SetOtherIcon(View.AssistProductItem_UIButton.sprite, icon);

        if (!string.IsNullOrEmpty(str))
        {
            View.Label_UILabel.text = str;
            View.Label_UILabel.gameObject.SetActive(true);
        }

        View.AssistProductItem_UIButton.sprite.isGrey = false;
        View.Lock_UISprite.gameObject.SetActive(false);
    }

    public void SetIsTips(bool isTips)
    {
        _isTips = isTips;
    }

    public void SetIsChosed(bool isChosed)
    {
        View.Chosed_UISprite.gameObject.SetActive(isChosed);
    }

    public void IsLock()
    {
        View.AssistProductItem_UIButton.sprite.isGrey = true;
        View.Lock_UISprite.gameObject.SetActive(true);
    }

    public void SetScale()
    {
        View.AssistProductItem_UIButton.sprite.width = 50;
        View.AssistProductItem_UIButton.sprite.height = 50;

        View.Bg_UISprite.width = 60;
        View.Bg_UISprite.height = 60;

        View.Chosed_UISprite.width = 65;
        View.Chosed_UISprite.height = 65;

        View.Lock_UISprite.width = 30;
        View.Lock_UISprite.height = 30;
    }

    public void SetVirtualIcon(int id)
    {
        _virtual = true;
        UIHelper.SetAppVirtualItemIcon(View.AssistProductItem_UIButton.sprite, (AppVirtualItem.VirtualItemEnum)id);
    }

    UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }

}
