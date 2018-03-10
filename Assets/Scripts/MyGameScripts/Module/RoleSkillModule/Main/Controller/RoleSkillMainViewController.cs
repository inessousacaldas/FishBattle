// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillMainViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public interface IRoleSkillMainViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(Skill.SkillEnum index,IRoleSkillMainData data);
}
public partial class RoleSkillMainViewController
{

    public static readonly ITabInfo[] tabInfoList =
                {
                    TabInfoData.Create((int)Skill.SkillEnum.Crafts, "战技")
                    , TabInfoData.Create((int)Skill.SkillEnum.Magic, "魔法")
                };

    private TabbtnManager tabMgr = null;
    private BaseView curView;
    private RoleSkillCraftsViewController craftsCtrl;
    private RoleSkillMagicViewController magicCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        CreateTabItem();

    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_UIGrid.gameObject
                    , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
                    , "Tabbtn_" + i);

        tabMgr = TabbtnManager.Create(tabInfoList,func);
    }

    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        tabMgr = null;
        if(craftsCtrl != null)
        {
            //craftsCtrl.Dispose();
            craftsCtrl = null;
        }

        if(magicCtrl != null)
        {
            //magicCtrl.Dispose();
            magicCtrl = null;
        }
        curView = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(Skill.SkillEnum index,IRoleSkillMainData data)
    {
        switch(index)
        {
            case Skill.SkillEnum.Crafts:
                craftsCtrl.UpdateView(data);
                break;
            case Skill.SkillEnum.Magic:
                magicCtrl.UpdateView(data);
                break;
        }
    }

    public void OnTabChange(Skill.SkillEnum index,IRoleSkillMainData data)
    {
        if(data.CurTab != index)
        {
            if(curView != null)
            {
                curView.Hide();
                curView = null;
            }
        }
        switch(index)
        {
            case Skill.SkillEnum.Crafts:
                if(craftsCtrl == null)
                {
                    craftsCtrl = AddChild<RoleSkillCraftsViewController,RoleSkillCraftsView>(View.Content_Transform.gameObject,RoleSkillCraftsView.NAME,RoleSkillCraftsView.NAME);
                    RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(craftsCtrl);
                }
                curView = craftsCtrl.View;
                craftsCtrl.UpdateView(data);
                break;
            case Skill.SkillEnum.Magic:
                if(magicCtrl == null)
                {
                    magicCtrl = AddChild<RoleSkillMagicViewController,RoleSkillMagicView>(View.Content_Transform.gameObject,RoleSkillMagicView.NAME,RoleSkillMagicView.NAME);
                    RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(magicCtrl);
                }
                curView = magicCtrl.View;
                magicCtrl.UpdateView(data);
                break;
        }
        if(curView != null)
        {
            curView.Show();
        }
    }
}
