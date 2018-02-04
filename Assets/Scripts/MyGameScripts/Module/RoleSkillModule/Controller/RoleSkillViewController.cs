// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IRoleSkillViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(RoleSkillTab index,IRoleSkillData data);
    void ResetDepth();
}
public partial class RoleSkillViewController
{

    public static readonly ITabInfo[] tabInfoList =
                {
                    TabInfoData.Create((int)RoleSkillTab.Skill, "技能")
                    , TabInfoData.Create((int)RoleSkillTab.Potential, "潜能")
                    , TabInfoData.Create((int)RoleSkillTab.Talent, "天赋")
                    , TabInfoData.Create((int)RoleSkillTab.Sepciality, "专精")
                };

    private TabbtnManager tabMgr = null;
    private BaseView curView;
    private RoleSkillMainViewController mainViewCtrl;
    private RoleSkillPotentialViewController potentialViewCtrl;
    private RoleSkillTalentViewController talentViewCtrl;
    private RoleSkillSpecialityViewController specViewCtrl;


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        CreateTabItem();

    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_UIGrid.gameObject
                    , TabbtnPrefabPath.TabBtnWidget.ToString()
                    , "Tabbtn_" + i);

        tabMgr = TabbtnManager.Create(tabInfoList,func);
    }

    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        if(_disposable != null)
        {
            _disposable.Add(PlayerModel.Stream.SubscribeAndFire(UpdatePlayerInfo));
        }
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        mainViewCtrl = null;
        potentialViewCtrl = null;
        talentViewCtrl = null;
        specViewCtrl = null;
        curView = null;
        tabMgr = null;
        //RoleSkillDataMgr.RoleSkillViewLogic.DisposeModule();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IRoleSkillData data)
    {
        UpdateView(data.MainTab,data);
    }

    public void UpdateView(RoleSkillTab index,IRoleSkillData data)
    {
        switch(index)
        {
            case RoleSkillTab.Skill:
                mainViewCtrl.UpdateView(data.MainData.CurTab,data.MainData);
                break;
            case RoleSkillTab.Potential:
                potentialViewCtrl.UpdateView(data.PotentialData);
                ModelManager.Player.FireData();
                break;
            case RoleSkillTab.Talent:
                talentViewCtrl.UpdateView(data.TalentData);
                ModelManager.Player.FireData();
                break;
            case RoleSkillTab.Sepciality:
                specViewCtrl.UpdateView(data.SpecData);
                ModelManager.Player.FireData();
                break;
        }
    }

    public void OnTabChange(RoleSkillTab index,IRoleSkillData data)
    {
        if(data.MainTab != index)
        {
            if(curView != null)
            {
                curView.Hide();
                curView = null;
            }
        }
        switch(index)
        {
            case RoleSkillTab.Skill:
                if(mainViewCtrl == null)
                {
                    mainViewCtrl = AddChild<RoleSkillMainViewController,RoleSkillMainView>(View.Content_Transform.gameObject,RoleSkillMainView.NAME,RoleSkillMainView.NAME);
                    RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(mainViewCtrl);
                }
                RoleSkillDataMgr.RoleSkillNetMsg.ReqSkillInfo();
                curView = mainViewCtrl.View;
                break;
            case RoleSkillTab.Potential:
                if(potentialViewCtrl == null)
                {
                    potentialViewCtrl = AddChild<RoleSkillPotentialViewController,RoleSkillPotentialView>(View.Content_Transform.gameObject,RoleSkillPotentialView.NAME,RoleSkillPotentialView.NAME);
                    RoleSkillDataMgr.RoleSkillPotentialViewLogic.InitReactiveEvents(potentialViewCtrl);
                }
                curView = potentialViewCtrl.View;
                RoleSkillDataMgr.RoleSkillNetMsg.ReqPotentialInfo();
                ModelManager.Player.FireData();
                break;
            case RoleSkillTab.Talent:
                if(talentViewCtrl == null)
                {
                    talentViewCtrl = AddChild<RoleSkillTalentViewController,RoleSkillTalentView>(View.Content_Transform.gameObject,RoleSkillTalentView.NAME,RoleSkillTalentView.NAME);
                    RoleSkillDataMgr.RoleSkillTalentViewLogic.InitReactiveEvents(talentViewCtrl);
                }
                curView = talentViewCtrl.View;
                RoleSkillDataMgr.RoleSkillNetMsg.ReqTalentInfo();
                ModelManager.Player.FireData();
                break;
            case RoleSkillTab.Sepciality:
                if(specViewCtrl == null)
                {
                    specViewCtrl = AddChild<RoleSkillSpecialityViewController,RoleSkillSpecialityView>(View.Content_Transform.gameObject,RoleSkillSpecialityView.NAME,RoleSkillSpecialityView.NAME);
                    RoleSkillDataMgr.RoleSkillSpecialityViewLogic.InitReactiveEvents(specViewCtrl);
                }
                curView = specViewCtrl.View;
                RoleSkillDataMgr.RoleSkillNetMsg.ReqSpecialityInfo();
                break;
        }
        if(curView != null)
        {
            curView.Show();
        }
    }

    private void UpdatePlayerInfo(IPlayerModel model)
    {
        if(potentialViewCtrl != null)
        {
            potentialViewCtrl.UpdatePlayerInfo(_dataUpdator.LastValue.PotentialData,model);
        }

        if(talentViewCtrl != null)
        {
            talentViewCtrl.UpdatePlayerInfo(_dataUpdator.LastValue.TalentData,model);
        }
    }

    public void ResetDepth()
    {
        if (View == null) return;
        GameObjectExt.ResetDepth(View.gameObject,View.GetComponent<UIPanel>().depth);
    }
}
