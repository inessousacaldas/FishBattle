// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface ICrewSkillViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(CrewSkillTab index, ICrewSkillData data);
    void InitWindow();
    void ShowWindowsView(ICrewSkillVO data);

    void ShowWindowsView(PsvItemData data, ICrewSkillData _data);

    void UpdateSkillDescript(ICrewSkillItemController itemCtrl);
    UniRx.IObservable<ICrewSkillWindowController> GetBGClickEvt { get; }
    ICrewSkillPassiveTipsController GetPsvTipCtrl { get; }
    ICrewSkillCraftsTipsController GetCraftsTipCtrl { get; }
    ICrewCraftSkillViewController GetCraftsViewCtrl { get; }
    ICrewMagicSkillViewController GetMagicViewCtrl { get; }
    ICrewPassiveSkillViewController GetPsvViewCtrl { get; }
    UniRx.IObservable<int> PsvTabMgr { get; }
    UniRx.IObservable<Unit> PsvBtnAdd { get; }
    UniRx.IObservable<Unit> PsvBtnMinus{ get; }
    UniRx.IObservable<Unit> PsvBtnForget { get; }
    UniRx.IObservable<Unit> PsvBtnMax { get; }
    UniRx.IObservable<Unit> PsvBtnUp { get; }
    UniRx.IObservable<Unit> PsvBtnUse { get; }
    UniRx.IObservable<string> InputValueChange { get; }
    UniRx.IObservable<ICrewSkillPassiveTipsController> PsvBtnLearn { get; }
    UniRx.IObservable<Unit> PsvBtnForgetSure { get; }
    UniRx.IObservable<Unit> PsvBtnCancel { get; }
    UniRx.IObservable<Unit> PsvBtnBlackBG { get; }
    UniRx.IObservable<Unit> CraftsBtnUp { get; }
    UniRx.IObservable<Unit> MagicBtn { get; }
}


public partial class CrewSkillViewController
{
    private static readonly ITabInfo[] tabInfoList =
    {
        TabInfoData.Create((int)CrewSkillTab.Crafts,"战技")
       ,TabInfoData.Create((int)CrewSkillTab.Passive,"魔法")
       ,TabInfoData.Create((int)CrewSkillTab.Magic,"技巧")
       
    };
    private TabbtnManager tabMgr = null;
    private BaseView curView;

    private CrewCraftSkillViewController craftsCtrl;               //战技
    private CrewMagicSkillViewController magicCtrl;                 //魔法
    private CrewPassiveSkillViewController passiveCtrl;             //技巧
    private CrewSkillTrainingViewController trainingCtrl;           //研修

    private CrewSkillMagicTipsController magicTipCtrl;
    private CrewSkillCraftsTipsController craftsTipCtrl;
    private CrewSkillPassiveTipsController psvTipCtrl;
    private BaseView curTipView;

    private ICrewSkillWindowController IWindowCtrl;

    private CrewSkillTab skillTab = CrewSkillTab.Crafts;

    private CompositeDisposable _disposable;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
        CreateTabItem();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
        curView = null;
        craftsCtrl = null;
        magicCtrl = null;
        trainingCtrl = null;
        IWindowCtrl = null;
        curTipView = null;
        craftsTipCtrl = null;
        magicTipCtrl = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    

    private void CreateTabItem()
    {
        tabMgr = TabbtnManager.Create(tabInfoList, i => AddTabBtn(i, View.tabBtn_UIGrid.gameObject, "TabBtnWidget_CrewSkill", "Tabbtn_"));
        tabMgr.SetBtnLblFont(selectColor: "000000",normalColor: ColorConstantV3.Color_VerticalUnSelectColor2_Str);
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, string tabPath, string name)
    {
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath
            , name + i);
        return ctrl;
    }

    private Subject<ICrewSkillWindowController> _bgClickEvt = new Subject<ICrewSkillWindowController>();
    public UniRx.IObservable<ICrewSkillWindowController> GetBGClickEvt { get { return _bgClickEvt; } }
    public void InitWindow()
    {
        if (IWindowCtrl == null)
        {
            var window = AddChild<CrewSkillWindowController, CrewSkillWindow>(
            View.windows_Transform.gameObject,
            CrewSkillWindow.NAME
            );
            _disposable.Add(window.OnbigBG_UIButtonClick.Subscribe(_ => _bgClickEvt.OnNext(window)));
            IWindowCtrl = window;
        }
        IWindowCtrl.HideWindow();
    }

    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    private void OnIndexChange(int idx)
    {
        idx -= 1;
        skillTab = (CrewSkillTab)idx;
        tabMgr.SetTabBtn(idx);
    }
    public void OnTabChange(CrewSkillTab index,ICrewSkillData data)
    {
        skillTab = index;
        int crewID = CrewSkillHelper.CrewID;
        CreateAllPanel(data, crewID);

        //--策划需求,取消滑动翻页
//        if (View.CrewSkillContent_PageScrollView.onChangePage == null)
//        {
//            View.CrewSkillContent_PageScrollView.onChangePage += OnIndexChange;
//        }
        switch (index)
        {
            case CrewSkillTab.Crafts:
                if (curView != null)
                {
                    curView = craftsCtrl.View;
                    //View.CrewSkillContent_PageScrollView.SkipToPage(1, true);
                }
                curView = craftsCtrl.View;
                break;
            case CrewSkillTab.Magic:
                curView = magicCtrl.View;
                //View.CrewSkillContent_PageScrollView.SkipToPage(2, true);
                break;
            case CrewSkillTab.Passive:
                curView = passiveCtrl.View;
                //View.CrewSkillContent_PageScrollView.SkipToPage(3, true);
                break;
        }

        craftsCtrl.gameObject.SetActive(index == CrewSkillTab.Crafts);
        magicCtrl.gameObject.SetActive(index == CrewSkillTab.Magic);
        passiveCtrl.gameObject.SetActive(index == CrewSkillTab.Passive);
    }

    //为了适应左右滑动
    //private subject<int> skillCtrl = new subject<int>();
   // public iobservable SkillCtrlClickEvt {
        //get { return skillCtrl; }
//}
    private void CreateAllPanel(ICrewSkillData data, int crewID)
    {
        if (craftsCtrl == null)
        {
            craftsCtrl = AddChild<CrewCraftSkillViewController, CrewCraftSkillView>(
                View.CraftsView_Transform.gameObject,
                CrewCraftSkillView.NAME
            );
            craftsCtrl.SetData(data);
            craftsCtrl.UpdateView(crewID);
        }
        if (magicCtrl == null)
        {
            magicCtrl = AddChild<CrewMagicSkillViewController, CrewMagicSkillView>(
                View.MagicView_Transform.gameObject,
                CrewMagicSkillView.NAME
                );
            magicCtrl.SetData(data);
            magicCtrl.UpdateView(crewID);
        }
        if (passiveCtrl == null)
        {
            passiveCtrl = AddChild<CrewPassiveSkillViewController, CrewPassiveSkillView>(
                View.TechnicView_Transform.gameObject,
                CrewPassiveSkillView.NAME
                );
            passiveCtrl.SetData(data);
            passiveCtrl.UpdateView(crewID);
        }
    }

    //通过CrewMainViewController里的UpdateDataAndView更新数据
    public void UpdateDataAndView(int id)
    {
        if (craftsCtrl != null)
        {
            craftsCtrl.UpdateView(id);
        }
        if (magicCtrl != null)
        {
            magicCtrl.UpdateView(id);
        }
        if (passiveCtrl != null)
        {
            passiveCtrl.UpdateView(id);
        }
        UpdateWindowView();
    }

    #region 战技按钮初始化

    private Subject<Unit> craftsBtnUp = new Subject<Unit>();
    public UniRx.IObservable<Unit> CraftsBtnUp { get { return craftsBtnUp; } }

    private Subject<Unit> magicBtn = new Subject<Unit>();
    public UniRx.IObservable<Unit> MagicBtn { get { return magicBtn; } }

    #endregion

    /// <summary>
    /// 本来预想所有数据都继承ICrewSkillVO接口，因为前面数据结构设计的原因，导致后面没能够统一接口
    /// 后期可以优化，先实现功能
    /// </summary>
    /// <param name="skillVO"></param>
    public void ShowWindowsView(ICrewSkillVO skillVO)
    {
        //IWindowCtrl = CrewSkillDataMgr.DataMgr.GetWindonwCtrl;
        var curSkillTab = CrewSkillDataMgr.DataMgr.MainTab;
        if (IWindowCtrl == null || skillVO == null) return;
        IWindowCtrl.ShowWindow();
        if (curTipView != null) curTipView.Hide();
        switch (curSkillTab)
        {
            case CrewSkillTab.Crafts:
                IWindowCtrl.UpdateView("战技升级");
                GameDebuger.LogError("战技升级");
                //if (craftsTipCtrl == null)
                //{
                //    craftsTipCtrl = AddChild<CrewSkillCraftsTipsController, CrewSkillCraftsTips>(
                //        IWindowCtrl.Trans.gameObject,
                //        CrewSkillCraftsTips.NAME
                //        );
                //    _disposable.Add(craftsTipCtrl.OnbtnUp_UIButtonClick.Subscribe(e => craftsBtnUp.OnNext(e)));
                //}
                //craftsTipCtrl.UpdateView(skillVO);
                
                //curTipView = craftsTipCtrl.View;
                break;
            case CrewSkillTab.Magic:
                IWindowCtrl.UpdateView("详细属性");
                if (magicTipCtrl == null)
                {
                    magicTipCtrl = AddChild<CrewSkillMagicTipsController, CrewSkillMagicTips>(
                        IWindowCtrl.Trans.gameObject,
                        CrewSkillMagicTips.NAME
                        );
                    _disposable.Add(magicTipCtrl.OnbtnUp_UIButtonClick.Subscribe(e => magicBtn.OnNext(e)));
                }
                magicTipCtrl.UpdateView(skillVO);
                curTipView = magicTipCtrl.View;
                break;

        }
        if (curTipView != null) curTipView.Show();
    }

    public void UpdateSkillDescript(ICrewSkillItemController itemCtrl)
    {
        var curSkillTab = CrewSkillDataMgr.DataMgr.MainTab;
        //if (itemCtrl.SkillVO == null) return;
        switch (curSkillTab)
        {
            case CrewSkillTab.Crafts:
                //IWindowCtrl.UpdateView("战技升级");

                //if (itemCtrl.SkillVO == null) return;
                craftsCtrl.UpdateSkillDescripe(itemCtrl as CrewSkillItemController);
                break;
            case CrewSkillTab.Magic:
                //IWindowCtrl.UpdateView("详细属性");

                magicCtrl.UpdateSkillDescripe(itemCtrl as CrewSkillItemController);
                break;
            case CrewSkillTab.Passive:


                break;

        }
    }

    #region 技巧弹窗初始化
    private Subject<int> psvTabMgr = new Subject<int>();
    public UniRx.IObservable<int> PsvTabMgr { get { return psvTabMgr; } }

    private Subject<Unit> psvBtnAdd = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnAdd { get { return psvBtnAdd; } }

    private Subject<Unit> psvBtnMinus = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnMinus { get { return psvBtnMinus; } }

    private Subject<Unit> psvBtnForget = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnForget { get { return psvBtnForget; } }

    private Subject<Unit> psvBtnMax = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnMax { get { return psvBtnMax; } }

    private Subject<Unit> psvBtnUp = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnUp { get { return psvBtnUp; } }

    private Subject<Unit> psvBtnUse = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnUse { get { return psvBtnUse; } }

    private Subject<string> inputValueChange = new Subject<string>();
    public UniRx.IObservable<string> InputValueChange { get { return inputValueChange; } }

    private Subject<ICrewSkillPassiveTipsController> psvBtnLearn = new Subject<ICrewSkillPassiveTipsController>();
    public UniRx.IObservable<ICrewSkillPassiveTipsController> PsvBtnLearn { get { return psvBtnLearn; } }

    private Subject<Unit> psvBtnForgetSure = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnForgetSure { get { return psvBtnForgetSure; } }

    private Subject<Unit> psvBtnCancel = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnCancel { get { return psvBtnCancel; } }

    private Subject<Unit> psvBtnBlackBG = new Subject<Unit>();
    public UniRx.IObservable<Unit> PsvBtnBlackBG { get { return psvBtnBlackBG; } }
    #endregion

    //因为前面设计技巧数据是直接取自于dto，所以无法沿用上面的接口。
    public void ShowWindowsView(PsvItemData data, ICrewSkillData _data)
    {
        if (data.state == PassiveState.NeedItem)
        {
            passiveCtrl.ShowSkillBookView();
            passiveCtrl.OnTabChange(PassiveType.All, _data);
        }
        else if (data.state == PassiveState.HaveItem)
        {
            passiveCtrl.ShowSkillDescriptView();
        }

    }
    
    //当窗口打开时，也需要刷新界面数据
    public void UpdateWindowView()
    {
        switch (skillTab)
        {
            case CrewSkillTab.Crafts:
                if (craftsTipCtrl != null && IWindowCtrl != null && IWindowCtrl.IsShow) 
                {
                    craftsTipCtrl.UpdateView(CrewSkillDataMgr.DataMgr.CraftsData.curSelCrafVO);
                }
                break;
            case CrewSkillTab.Magic:
                break;
            case CrewSkillTab.Passive:
                if (psvTipCtrl != null && IWindowCtrl != null && IWindowCtrl.IsShow)
                {
                    switch (psvTipCtrl.WindowType)
                    {
                        case PsvWindowType.Backpack:
                            //当点击了学习，并且成功了。则跳转界面刷新数据
                            psvTipCtrl.UpdateBackView(IWindowCtrl);
                            break;
                        case PsvWindowType.Property:
                            //当遗忘成功或是升级，刷新数据
                            psvTipCtrl.UpdatePropertyView(IWindowCtrl);
                            break;
                    }
                }
                break;
        }
    }

    public ICrewSkillPassiveTipsController GetPsvTipCtrl
    {
        get { return psvTipCtrl; }
    }
    public ICrewSkillCraftsTipsController GetCraftsTipCtrl
    {
        get { return craftsTipCtrl; }
    }
    public ICrewCraftSkillViewController GetCraftsViewCtrl
    {
        get { return craftsCtrl; }
    }

    public ICrewMagicSkillViewController GetMagicViewCtrl
    {
        get { return magicCtrl; }
    }
    public ICrewPassiveSkillViewController GetPsvViewCtrl
    {
        get { return passiveCtrl; }
    }
}
