// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PlayerPropertyViewController.cs
// Author   : Cilu
// Created  : 8/28/2017 11:18:56 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using MyGameScripts.Gameplay.Player;
using AppDto;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial interface IPlayerPropertyViewController
{
    TabbtnManager TabBtnMgr { get; }
    void OnHandleChangeName(string name);
}

public partial class PlayerPropertyViewController {

    private TabbtnManager tabBtnMgr;
    private Func<int, ITabBtnController> func;

    public TabbtnManager TabBtnMgr { get { return tabBtnMgr; } }


    //初始化子view ctrl

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitView();

        func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                View.TabBtnTable_UIGrid.gameObject
                , TabbtnPrefabPath.TabBtnWidget.ToString()
                , "Tabbtn_" + i);

        var PlayerProTabSet = new List<ITabInfo>(2) { TabInfoData.Create((int)PlayerViewTab.PropertyView, "属性") };
        if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_50))
            PlayerProTabSet.Insert((int)PlayerViewTab.InfoView, TabInfoData.Create((int)PlayerViewTab.InfoView, "信息"));
        tabBtnMgr = TabbtnManager.Create(PlayerProTabSet, func);
        //TabBtnMgr.SetBtnHide((int)PlayerViewTab.InfoView);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        
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

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IPlayerPropertyData data)
    {
        UpdateName(data);
        InitFightProPanel(data);
        tabBtnMgr.SetTabBtn((int)data.CurTab);
    }

    private void UpdateName(IPlayerPropertyData data)
    {
        View.PropertyNameLabel_UILabel.text = string.IsNullOrEmpty(data.ResultName) ? ModelManager.Player.GetPlayerName() : data.ResultName;
    }

    private ModelDisplayController modelDisplayController;
    private List<FightPropertyItemController> fightPropertyItemCtr;
    public static Comparison<CharacterAbility> _comparison = null;

    private void InitView()
    {
        //人物
        if (fightPropertyItemCtr == null)
            fightPropertyItemCtr = new List<FightPropertyItemController>();

        modelDisplayController = AddChild<ModelDisplayController, ModelDisplayUIComponent>(View.HeroModelParent, ModelDisplayUIComponent.NAME);
        modelDisplayController.Init(300, 300);
        modelDisplayController.SetBoxCollider(300, 300);
        modelDisplayController.SetupMainRoleModel();
        modelDisplayController.SetBoxColliderEnabled(true);
        modelDisplayController.SetPosition(new Vector3(0, -29, 0));

        //基本信息
        View.PropertyNameLabel_UILabel.text = ModelManager.Player.GetPlayerName();
        View.LevelLabel_UILabel.text = ModelManager.Player.GetPlayerLevel().ToString();
        UIHelper.SetCommonIcon(View.MagicIcon_UISprite, GlobalAttr.MAGICICON[ModelManager.Player.GetSlotsElementLimit]);
        UIHelper.SetCommonIcon(View.FactionIcon_UISprite, "faction_" + ModelManager.Player.FactionID);
        //经验
        ExpGrade grade = DataCache.getDtoByCls<ExpGrade>(ModelManager.Player.GetPlayerLevel() + 1); //当前等级到下一级经验 so +1 (满级越界要判断)
        long needExp = grade == null ? DataCache.getDtoByCls<ExpGrade>(ModelManager.Player.GetPlayerLevel()).mainCharactorExp : grade.mainCharactorExp;
        long curExp = ModelManager.Player.GetPlayerExp();
        if(grade == null)
            curExp = curExp > needExp * 10 ? needExp * 10 : curExp;

        View.ExpLabel_UILabel.text = string.Format("{0}/{1}", curExp, needExp);
        View.ExpSlider_UISlider.value = needExp > 0 ? (float)curExp / (float)needExp : 0;

        //战斗属性
        foreach (var propertyId in GlobalAttr.SECOND_ATTRS)
        {
            CharacterAbility tCharacterAbility = DataCache.getDtoByCls<CharacterAbility>(propertyId);
            FightPropertyItemController fightProCtr = AddChild<FightPropertyItemController, FightPropertyItem>(View.FightPropertyTable_UITable.gameObject, FightPropertyItem.NAME);
            fightPropertyItemCtr.Add(fightProCtr);
        }
    }
     
    //刷新属性
    private void InitFightProPanel(IPlayerPropertyData data)
    {
        //基本属性
        View.CorporeityNum_UILabel.text = Mathf.FloorToInt(data.GetPropertyById((int)CharactorDto.CharacterPropertyType.PHYSIQUE)).ToString();
        View.PowerNum_UILabel.text = Mathf.FloorToInt(data.GetPropertyById((int)CharactorDto.CharacterPropertyType.POWER)).ToString();
        View.SpiritNum_UILabel.text = Mathf.FloorToInt(data.GetPropertyById((int)CharactorDto.CharacterPropertyType.SPIRIT)).ToString();
        View.MagicNum_UILabel.text = Mathf.FloorToInt(data.GetPropertyById((int)CharactorDto.CharacterPropertyType.MAGIC)).ToString();

        //战斗属性
        var index = 0;
        foreach (var propertyId in GlobalAttr.SECOND_ATTRS)
        {
            CharacterAbility tCharacterAbility = DataCache.getDtoByCls<CharacterAbility>(propertyId);
            fightPropertyItemCtr[index].Init(tCharacterAbility.name, (int)data.GetPropertyById(propertyId));
            index++;
        }
        
        View.FightPropertyTable_UITable.Reposition();
    }

    public void OnHandleChangeName(string name)
    {
        View.PropertyNameLabel_UILabel.text = name;
    }
}
