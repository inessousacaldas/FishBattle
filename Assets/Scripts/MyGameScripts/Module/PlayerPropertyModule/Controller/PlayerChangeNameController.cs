// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PlayerChangeNameController.cs
// Author   : xjd
// Created  : 11/2/2017 4:15:37 PM
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial interface IPlayerChangeNameController
{
    UniRx.IObservable<string> OnClickRenameStream { get; }
}
public partial class PlayerChangeNameController    {

    private string _name;

    public static IPlayerChangeNameController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IPlayerChangeNameController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IPlayerChangeNameController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.Input_UIInput.onChange, OnInputChange);

        OnLeftBtn_UIButtonClick.Subscribe(_ =>
        {
            Close();
        });

        OnRightBtn_UIButtonClick.Subscribe(_ =>
        {
            string error = AppStringHelper.ValidateStrLength(_name, 4);
            if (!string.IsNullOrEmpty(error))
            {
                TipManager.AddTip(error);
                return;
            }
            if (_name.IndexOf(" ") >= 0)
            {
                TipManager.AddTip("输入的名字中不能有空格");
                return;
            }
            var controller = ProxyBaseWinModule.Open();
            var title = "改名";
            string txt = "";
            var playerLv = ModelManager.Player.GetPlayerLevel();
            int needcash = Math.Min(playerLv * 5 + 250, 1000);

            //100012改名券id,需要配表,暂时写死
            var propItem = BackpackDataMgr.DataMgr.GetItemByItemID(100012);
            if (propItem != null)
            {
                var prop = DataCache.getDtoByCls<GeneralItem>(100012) as Props;
                if (prop == null)
                {
                    GameDebuger.LogError(string.Format("请检查props表,不存在{0}", 100012));
                    return;
                }
                txt = string.Format("你是否消耗[c][{0}]{1}*1[-][/c],把名字改成{2}",
                    ItemHelper.GetItemNameColorByRank(prop.quality),
                    prop.name,
                    _name.WrapColor(ColorConstantV3.Color_Green_Strong_Str));
            }
            else
                txt = string.Format("你是否消耗{0}钻石,把名字改成{1}", 
                    needcash.WrapColor(ColorConstantV3.Color_Green_Strong_Str), 
                    _name.WrapColor(ColorConstantV3.Color_Green_Strong_Str));

            BaseTipData tipData = BaseTipData.Create(title, txt, 0, () =>
            {
                var diamond = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.DIAMOND);
                if (propItem == null && diamond < needcash)
                {
                    TipManager.AddTip("钻石数量不足");
                    return;
                }
                clickRenameStream.OnNext(_name);
                Close();
            }, null);

            controller.InitView(tipData);
        });
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
        _name = View.Input_UIInput.value;
    }

    private void Close()
    {
        UIModuleManager.Instance.CloseModule(PlayerChangeName.NAME);
    }

    readonly UniRx.Subject<string> clickRenameStream = new UniRx.Subject<string>();
    public UniRx.IObservable<string> OnClickRenameStream
    {
        get { return clickRenameStream; }
    }
}
