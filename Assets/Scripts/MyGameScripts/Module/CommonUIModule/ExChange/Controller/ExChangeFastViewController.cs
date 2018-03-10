// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExChangeFastViewController.cs
// Author   : Zijian
// Created  : 8/24/2017 11:01:43 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;
using System.Collections.Generic;

public partial interface IExChangeFastViewController
{
    void UpdateView(int exchangeId, long lackNum, Action callback=null);
}
public partial class ExChangeFastViewController    {
    public static IExChangeFastViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IExChangeFastViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IExChangeFastViewController;
            
        return controller;        
    }

    private int _exchangeId = -1;
    private long _lackNum = 0;
    private int _exchangeCount = 0;
    private string _toggleTipsText = "{0}不足时自动使用{1}进行兑换";
    private Action _callback = null;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

	// 客户端自定义代码
	protected override void RegistCustomEvent()
    {
        MilaButton_UIButtonEvt.Subscribe(_ =>
        {
            if(ExChangeMainDataMgr.DataMgr.GetRemainMiraCount() <= 0)
            {
                TipManager.AddTip("兑换数量已达上限");
                return;
            }

            if (View.AutoExChangeToggle_UIToggle.value)
                ExChangeMainDataMgr.DataMgr.SetNotShowView(_exchangeId);

            var scale = ExChangeHelper.GetConvertScale((int)VirtualItemEnum.MIRA, _exchangeId);
            var count = (int)Math.Ceiling((float)_lackNum / (float)scale);
            if (ExChangeHelper.CheckIsNeedExchange(VirtualItemEnum.MIRA, count))
            {
                TipManager.AddTip("米拉不够，请先兑换米拉");
            } 
            else
            {
                ExChangeMainDataMgr.ExChangeMainNetMsg.ReqCurrencyConvert((int)VirtualItemEnum.MIRA, _exchangeId, count, false , ()=>
                {
                    var leftCount = ExChangeMainDataMgr.DataMgr.GetRemainMiraCount()-count>0 ? ExChangeMainDataMgr.DataMgr.GetRemainMiraCount()-count : 0;
                    TipManager.AddTip(string.Format("今天可兑换米拉量剩余{0}", leftCount));
                    GameUtil.SafeRun(_callback);
                });
            }
        });

        diamondButton_UIButtonEvt.Subscribe(_ => 
        {
            if (View.AutoExChangeToggle_UIToggle.value)
                ExChangeMainDataMgr.DataMgr.SetNotShowView(_exchangeId);

            int scale = ExChangeHelper.GetConvertScale((int)VirtualItemEnum.DIAMOND, _exchangeId);
            int count = (int)Math.Ceiling((float)_lackNum / (float)scale);

            //钻石兑换的时候请求协议为true 就可以绑钻不够自动消耗钻石
            if (_exchangeId == (int)VirtualItemEnum.MIRA)
                ExChangeMainDataMgr.ExChangeMainNetMsg.ReqCurrencyConvert((int)VirtualItemEnum.DIAMOND, _exchangeId, count, true, _callback);
            else
                ExChangeMainDataMgr.ExChangeMainNetMsg.ReqCurrencyConvert((int)VirtualItemEnum.BINDDIAMOND, _exchangeId, count, true, _callback);
        });

        CloseBtn_UIButtonEvt.Subscribe(_ => 
        {
            ProxyExChangeMain.CloseExChangeFast();
        });

        TipsBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("1.每天只能消耗一定量的米拉兑换其他货币\n2.等级越高，可兑换米拉量越多");
        });
    }

    protected override void RemoveCustomEvent()
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

    public void UpdateView(int exchangeId, long lackNum, Action callback = null)
    {
        if ((ItemHelper.GetGeneralItemByItemId(exchangeId) as AppVirtualItem) == null)
            return;

        var appvirtualItem = ItemHelper.GetGeneralItemByItemId(exchangeId) as AppVirtualItem;
        _exchangeId = exchangeId;
        _lackNum = lackNum;

        string lackItemIcon = ItemHelper.GetItemIcon(exchangeId);
        if (exchangeId == (int)VirtualItemEnum.MIRA)
        {
            View.Mira.SetActive(false);
            View.TipsBtn_UIButton.gameObject.SetActive(false);
            View.Diamond.transform.localPosition = new UnityEngine.Vector3(3.7f, View.Diamond.transform.localPosition.y);
            View.ToggleTips_UILabel.text = string.Format(_toggleTipsText, appvirtualItem.name, "钻石");
        }
        else
        {
            View.Mira.SetActive(true);
            View.TipsBtn_UIButton.gameObject.SetActive(true);
            View.RemainNumber_0_UILabel.gameObject.SetActive(true);
            View.RemainIcon_0_UISprite.gameObject.SetActive(true);
            View.Diamond.transform.localPosition = new UnityEngine.Vector3(100, View.Diamond.transform.localPosition.y);

            int scaleMira = ExChangeHelper.GetConvertScale((int)VirtualItemEnum.MIRA, (int)exchangeId);
            //需要米拉数量
            long needCount = (long)Math.Ceiling((float)lackNum / (float)scaleMira);
            View.NeedNumber_mira_UILabel.text = ModelManager.Player.GetPlayerWealthById((int)VirtualItemEnum.MIRA) < needCount ?
                needCount.ToString().WrapColor(ColorConstantV3.Color_Red) : needCount.ToString();

            View.MiraButton_UIButton.sprite.isGrey = ExChangeMainDataMgr.DataMgr.GetRemainMiraCount() <= 0 ? true : false;
            UIHelper.SetAppVirtualItemIcon(View.NeedIcon_mira_UISprite, VirtualItemEnum.MIRA);
            View.ToggleTips_UILabel.text = string.Format(View.ToggleTips_UILabel.text, appvirtualItem.name, "米拉和钻石");
        }

        //兑换所需钻石
        int scale = ExChangeHelper.GetConvertScale((int)VirtualItemEnum.DIAMOND, (int)exchangeId);
        //需要钻石数量
        long nextExpendCount = (long)Math.Ceiling((float)lackNum / (float)scale);
        View.NeedNumber_diamond_UILabel.text = ExChangeMainDataMgr.DataMgr.GetWealth(ExChangeUseTabType.UseDiamondAndBindDiamond) < nextExpendCount ?
                nextExpendCount.ToString().WrapColor(ColorConstantV3.Color_Red) : nextExpendCount.ToString();
        UIHelper.SetAppVirtualItemIcon(View.NeedIcon_diamond_UISprite, VirtualItemEnum.DIAMOND);

        //剩余米拉和钻石
        View.RemainNumber_0_UILabel.text = ModelManager.Player.GetPlayerWealth(VirtualItemEnum.BINDDIAMOND).ToString();
        View.RemainNumber_1_UILabel.text = ModelManager.Player.GetPlayerWealth(VirtualItemEnum.DIAMOND).ToString();
        View.LackNumber_UILabel.text = lackNum.ToString();
        UIHelper.SetAppVirtualItemIcon(View.RemainIcon_0_UISprite, VirtualItemEnum.BINDDIAMOND);
        UIHelper.SetAppVirtualItemIcon(View.RemainIcon_1_UISprite, VirtualItemEnum.DIAMOND);
        UIHelper.SetAppVirtualItemIcon(View.LackIcon_UISprite, (VirtualItemEnum)exchangeId);
        _callback = callback;
    }
}
