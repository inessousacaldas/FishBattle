// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExChangeMainViewController.cs
// Author   : Zijian
// Created  : 8/24/2017 2:54:17 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using UnityEngine;

public partial interface IExChangeMainViewController
{
    TabbtnManager TabMgr { get; }
    UniRx.IObservable<int> OnSelectCountStream { get; }
    void InitDataBefore(int expectItemId);
}
public partial class ExChangeMainViewController    {


    TabbtnManager tabmgr;
    public TabbtnManager TabMgr { get { return tabmgr; } }

    ITabInfo[] tabInfos = new ITabInfo[]
    {
        TabInfoData.Create((int)ExChangeUseTabType.UseMira,"使用米拉"),
        TabInfoData.Create((int)ExChangeUseTabType.UseDiamondAndBindDiamond,"使用钻石"),
    };
    PageTurnViewController ptctrl;
    public UniRx.IObservable<int> OnSelectCountStream { get { return ptctrl.Stream; } }
    
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        ptctrl = AddController<PageTurnViewController, PageTurnView>(View.PageTurnView);
        ptctrl.InitData_NumberInputer(1, 1, 9999, true, PageTurnViewController.InputerShowPos.Down);
    }
    public void InitDataBefore(int expectItemId)
    {
        if(expectItemId == (int)AppVirtualItem.VirtualItemEnum.GOLD || expectItemId == (int)AppVirtualItem.VirtualItemEnum.SILVER)
            CreatTabItem();
    }
    private void CreatTabItem()
    {
        //Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
        //            View.tab_content
        //            , TabbtnPrefabPath.TabBtnWidget_H2.ToString()
        //            , "Tabbtn_" + i);
        //tabmgr = TabbtnManager.Create(tabInfos, func);
        tabmgr = TabbtnManager.Create(tabInfos, i => AddTabBtn(i, View.tab_content, TabbtnPrefabPath.TabBtnExchange, "Tabbtn_"), 0);
    }

    class SpecialTabBtnWidgetController : TabBtnWidgetController
    {
        private bool isSelcet = false;

        public void SetUnselectFlip()
        {
            _view.TabBtnWidget_UISprite.flip = UIBasicSprite.Flip.Horizontally;
        }
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, TabbtnPrefabPath tabPath, string name)
    {
        var ctrl = AddChild<SpecialTabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath.ToString()
            , name + i);

        ctrl.SetBtnImages("bg_12", "btn_chosetype");
        ctrl.SetBtnLblFont(selectSize:20, selectColor: "585858", normalSize:20, normalColor: "e0e0e0");
        if(i == 1)
            ctrl.SetUnselectFlip();

        return ctrl;
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.Input_UIInput.onChange, OnInputChange);
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private void OnInputChange()
    {
        //if(StringHelper.ToInt(View.Input_UIInput.value) > ExChangeMainDataMgr.DataMgr.GetRemainMiraCount())

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IExChangeMainData data)
    {
        TabMgr.SetTabBtn((int)data.CurTab);

        var expectItem = ItemHelper.GetGeneralItemByItemId(data.ExchangeId);
        var exchangeItem = ItemHelper.GetGeneralItemByItemId(data.CurUseExChangeId);

        View.ExpectItemTips_UILabel.text = string.Format("兑换{0}次数", expectItem.name);
        View.From_ItemNumber_UILabel.text = data.CurSelectCount > data.GetWealth(data.CurTab) ? data.CurSelectCount.ToString().WrapColor(ColorConstantV3.Color_Red)
            : data.CurSelectCount.ToString().WrapColor("F1F1F1");
        UIHelper.SetAppVirtualItemIcon(View.From_ItemIcon_UISprite, (AppVirtualItem.VirtualItemEnum)data.CurUseExChangeId);
        UIHelper.SetAppVirtualItemIcon(View.To_ItemIcon_UISprite, (AppVirtualItem.VirtualItemEnum)data.ExchangeId);
        View.To_ItemNumber_UILabel.text = data.ExpectTotalCount.ToString();
        if (data.CurUseExChangeId == (int)AppVirtualItem.VirtualItemEnum.BINDDIAMOND)
        {
            UIHelper.SetAppVirtualItemIcon(View.From_ItemIcon_UISprite, AppVirtualItem.VirtualItemEnum.DIAMOND);
            View.FromItem_TipLabel_UILabel.gameObject.SetActive(true);
        }
        else
            View.FromItem_TipLabel_UILabel.gameObject.SetActive(false);

        //=====================显示身上剩余的货币======================
        View.FromItemRemainLabel_UILabel.text = string.Format("剩余{0}", exchangeItem.name);
        UIHelper.SetAppVirtualItemIcon(View.FromItem_RemainIcon_UISprite, (AppVirtualItem.VirtualItemEnum)data.CurUseExChangeId);
        
        var ownerMoney = ModelManager.Player.GetPlayerWealth((AppVirtualItem.VirtualItemEnum)exchangeItem.id);
        View.FromItem_RemainNumber_UILabel.text = ownerMoney.ToString();

        if (data.ExchangeId == (int)AppVirtualItem.VirtualItemEnum.MIRA)
        {
            View.FromItem_RemainNumber2_UILabel.gameObject.SetActive(false);
            View.FromItemRemainLabel2_UILabel.gameObject.SetActive(false);
            View.FromItem_TipLabel_UILabel.gameObject.SetActive(false);
        }
        else
        {
            View.FromItem_RemainNumber2_UILabel.gameObject.SetActive(true);
            View.FromItemRemainLabel2_UILabel.gameObject.SetActive(true);

            if (data.CurTab == ExChangeUseTabType.UseDiamondAndBindDiamond)
            {
                View.TipsButton_UIButton.gameObject.SetActive(false);
                View.FromItemRemainLabel2_UILabel.text = "剩余钻石";
                UIHelper.SetAppVirtualItemIcon(View.FromItem_RemainIcon2_UISprite, AppVirtualItem.VirtualItemEnum.DIAMOND);
                View.FromItem_RemainNumber2_UILabel.text = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.DIAMOND).ToString();
            }
            else if (data.CurTab == ExChangeUseTabType.UseMira)
            {
                View.TipsButton_UIButton.gameObject.SetActive(true);
                View.FromItemRemainLabel2_UILabel.text = "可兑换米拉量";
                UIHelper.SetAppVirtualItemIcon(View.FromItem_RemainIcon2_UISprite, AppVirtualItem.VirtualItemEnum.MIRA);
                View.FromItem_RemainNumber2_UILabel.text = data.RemainConvertMiraCount.ToString();
            }

            //剩余货币位置对齐 比较两行text长度
            //var maxWidth = View.FromItemRemainLabel_UILabel.width > View.FromItemRemainLabel2_UILabel.width ?
            //    View.FromItemRemainLabel_UILabel.width : View.FromItemRemainLabel2_UILabel.width;
            //View.FromItem_RemainIcon_UISprite.transform.localPosition = new Vector3(View.FromItemRemainLabel_UILabel.transform.localPosition.x + maxWidth
            //    , View.FromItem_RemainIcon_UISprite.transform.localPosition.y);
            //View.FromItem_RemainNumber_UILabel.transform.localPosition = new Vector3(View.FromItem_RemainIcon_UISprite.transform.localPosition.x + View.FromItem_RemainIcon_UISprite.width
            //    , View.FromItem_RemainNumber_UILabel.transform.localPosition.y);

            //View.FromItem_RemainIcon2_UISprite.transform.localPosition = new Vector3(View.FromItemRemainLabel2_UILabel.transform.localPosition.x + maxWidth
            //    , View.FromItem_RemainIcon2_UISprite.transform.localPosition.y);
            //View.FromItem_RemainNumber2_UILabel.transform.localPosition = new Vector3(View.FromItem_RemainIcon2_UISprite.transform.localPosition.x + View.FromItem_RemainIcon2_UISprite.width, 
            //    View.FromItem_RemainNumber2_UILabel.transform.localPosition.y);

            //可兑换米拉不足时 按钮为灰色
            View.ExchangeButton_UIButton.sprite.isGrey = data.CurTab == ExChangeUseTabType.UseMira && ExChangeMainDataMgr.DataMgr.GetRemainMiraCount() <= 0;
        }
    }
}
