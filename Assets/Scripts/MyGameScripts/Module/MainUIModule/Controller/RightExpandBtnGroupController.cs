// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RightExpandBtnGroupController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Security.Policy;
using Assets.Scripts.MyGameScripts.Module.SkillModule;
using UniRx;
using AppDto;
using UnityEngine;

public partial  interface IRightExpandBtnGroupController
{
    UniRx.IObservable<bool> RefreshPos { get; }
    void ShowOrHideBtn(bool b);
    void RefreshTweenPos(bool b);
}

public partial class RightExpandBtnGroupController : IRightExpandBtnGroupController
{
    // 界面初始化完成之后的一些后续初始化工作
    private CompositeDisposable _disposable;

    private bool _pullBtnState = false; //默认收起状态

    private Subject<bool> _refreshPos;

    public UniRx.IObservable<bool> RefreshPos
    {
        get { return _refreshPos; }
    }

    protected override void AfterInitView()
    {
        _disposable = new CompositeDisposable();
        _refreshPos = new Subject<bool>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _disposable.Add(OnPullBtn_UIButtonClick.Subscribe(_ =>
        {
            OnPullBtnClick();
        }));

        _disposable.Add(OnButton_Ranking_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Trade_UIButtonClick.Subscribe(_ => ProxyTrade.OpenTradeView()));
        _disposable.Add(OnButton_Store_UIButtonClick.Subscribe(_ => { ProxyShop.OpenShop(); }));
        _disposable.Add(OnButton_CardBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_UpBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_FirstBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_GuideBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_WelfareBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Daily_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Skill_UIButtonClick.Subscribe(_ => { ProxyRoleSkill.OpenPanel(); }));
        _disposable.Add(OnButton_Pack_UIButtonClick.Subscribe(_ => { ProxyBackpack.OpenBackpack(); }));
        _disposable.Add(OnButton_Guild_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_TempBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Quartz_UIButtonClick.Subscribe(_ => ProxyQuartz.OpenQuartzMainView()));
        _disposable.Add(OnButton_Equip_UIButtonClick.Subscribe(_ => ProxyEquipmentMain.Open()));
        _disposable.Add(OnButton_lifeskill_UIButtonClick.Subscribe(_ => { ProxyAssistSkillMain.OpenAssistSkillModule(); }));
        _disposable.Add(OnButton_Achievement_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Collect_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_Faction_UIButtonClick.Subscribe(_ => { TipManager.AddTip("===敬请期待==="); }));
        _disposable.Add(OnButton_AttackMan_UIButtonClick.Subscribe(_ => { ProxyBracerMainView.Open(); }));
        _disposable.Add(OnButton_Recruit_UIButtonClick.Subscribe(_ => { ProxyCrewReCruit.Open(); }));
        _disposable.Add(OnButton_Crew_UIButtonClick.Subscribe(_ => CrewProxy.OpenCrewMainView()));
    }

    protected override void OnDispose()
    {
        _refreshPos = _refreshPos.CloseOnceNull();
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    private void OnPullBtnClick()
    {
        _refreshPos.OnNext(_pullBtnState);
    }

    public void RefreshTweenPos(bool b)
    {
        _view.RightExpandBtnGroup_TweenPosition.Play(b);
        _view.PullBtn_Sprite.flip = b ? UIBasicSprite.Flip.Vertically : UIBasicSprite.Flip.Horizontally;
        _pullBtnState = !b;
    }

    public void ShowOrHideBtn(bool b)
    {
        _view.PullBtn_UIButton.gameObject.SetActive(b);
        this.gameObject.SetActive(b);
    }

    public void SetFuncOpen()
    {
        View.Button_Ranking_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_32));
        //View.Button_Trade_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_18));
        View.Button_Equip_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_41));
        //View.Button_Store_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_39));
        View.Button_Crew_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_10));
        View.Button_Recruit_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_44));
        View.Button_AttackMan_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_27));
        View.Button_lifeskill_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_42));

        View.Grid_UIGrid.Reposition();
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Grid_UIGrid.transform);
        View.BackGround_UISprite.width = 138 + 12 + (int)b.size.x;
        View.BackGround_1_UISprite.transform.localPosition = new Vector3(-166 -12 - (int)b.size.x, View.BackGround_1_UISprite.transform.localPosition.y);
        View.PullBtn_UIButton.transform.localPosition = new Vector3(-171 -12 - (int)b.size.x, View.BackGround_1_UISprite.transform.localPosition.y);
    }
}
