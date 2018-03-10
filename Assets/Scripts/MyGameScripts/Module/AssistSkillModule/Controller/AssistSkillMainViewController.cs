// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistSkillMainViewController.cs
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;

public partial interface IAssistSkillMainViewController
{
    event Action<AssistViewTab, IMonolessViewController> OnChildCtrlAdd;
    TabbtnManager TabbtnMgr { get; }
}
public partial class AssistSkillMainViewController    {
    private AssistChooseViewController _chooseViewCtrl = null;
    private AssistLearnViewController _learnViewCtrl = null;
    private AssistLearnCookViewController _learnCookCtrl = null;
    private AssistLearnForceViewController _learnForceCtrl = null;
    private AssistCookUpgradeViewController _cookUpgradeCtrl = null;
    private AssistCookProductViewController _cookProductCtrl = null;
    private AssistForceUpgradeViewController _forceUpgradeCtrl = null;
    private AssistForceProductViewController _forceProductCtrl = null;
    private AssistDelegateMainViewController _delegateMainCtrl = null;
    private AssistDelegateCrewViewController _delegateCrewCtrl = null;
    private List<IMonolessViewController> _assistSKillCtrlList = new List<IMonolessViewController>();
    private List<IMonolessViewController> _assistDelegateCtrlList = new List<IMonolessViewController>();

    IMonolessViewController curCtrl;
    public event Action<AssistViewTab, IMonolessViewController> OnChildCtrlAdd;

    TabbtnManager tabbtnMgr;
    public TabbtnManager TabbtnMgr { get { return tabbtnMgr; } }
    private Func<int, ITabBtnController> func;
//    RedPointController _redPointCtrl;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            View.Grid_UITable.gameObject
            , TabbtnPrefabPath.TabBtnWidget.ToString()
            , "Tabbtn_" + i);

        tabbtnMgr = TabbtnManager.Create(AssistSkillMainDataMgr.AssistSkillMainData._RightViewTabInfos, func);

        //红点测试xjd
//        _redPointCtrl = AddCachedChild<RedPointController, RedPointView>(View.DelegateRedPos, RedPointView.NAME);
//        _redPointCtrl.InitView(2, 301);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        //红点测试xjd
        _disposable.Add(RedPointDataMgr.Stream.Subscribe(e =>
        {
            
        }));
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        OnChildCtrlAdd = null;
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        
    }

    private void ShowAssistSkillView(bool isShow, IAssistSkillMainData data)
    {
        if(!isShow)
        {
            _assistSKillCtrlList.ForEach(itemCtrl=>
            {
                if (itemCtrl != null)
                    itemCtrl.Hide();
            });

            return;
        }

        if (!data.IsResp)
            return;

        //初次进入或者遗忘后的 选择界面
        if (data.CurTab == AssistViewTab.ChooseView)
        {
            if (_chooseViewCtrl == null)
            {
                _chooseViewCtrl = AddChild<AssistChooseViewController, AssistChooseView>(View.MainContent_Transform.gameObject, AssistChooseView.NAME);
                OnChildCtrlAdd(AssistViewTab.ChooseView, _chooseViewCtrl);
                _assistSKillCtrlList.Add(_chooseViewCtrl);
            }
            if (curCtrl != null)
                curCtrl.Hide();
            if (_learnViewCtrl != null)
                _learnViewCtrl.Hide();

            curCtrl = _chooseViewCtrl;
            _chooseViewCtrl.Show();
        }
        else
        {
            //学习界面 包括选择技能前后的界面
            if (_learnViewCtrl == null)
            {
                _learnViewCtrl = AddChild<AssistLearnViewController, AssistLearnView>(View.MainContent_Transform.gameObject, AssistLearnView.NAME);
                OnChildCtrlAdd(AssistViewTab.LearnBaseView, _learnViewCtrl);
                _assistSKillCtrlList.Add(_learnViewCtrl);
            }
            _learnViewCtrl.Show();
            _learnViewCtrl.UpdateView(data);

            #region 右侧界面
            var lastCtrl = curCtrl;
            switch (data.CurTab)
            {
                case AssistViewTab.LearnCookView:
                    {
                        if (_learnCookCtrl == null)
                        {
                            _learnCookCtrl = AddChild<AssistLearnCookViewController, AssistLearnCookView>(View.MainContent_Transform.gameObject, AssistLearnCookView.NAME);
                            OnChildCtrlAdd(AssistViewTab.LearnCookView, _learnCookCtrl);
                        }
                        _learnCookCtrl.UpdateView(data);
                        SetCurCtrlView(_learnCookCtrl, false);
                    }
                    break;
                case AssistViewTab.LearnForceView:
                    {
                        if (_learnForceCtrl == null)
                        {
                            _learnForceCtrl = AddChild<AssistLearnForceViewController, AssistLearnForceView>(View.MainContent_Transform.gameObject, AssistLearnForceView.NAME);
                            OnChildCtrlAdd(AssistViewTab.LearnForceView, _learnForceCtrl);
                        }
                        _learnForceCtrl.UpdateView(data);
                        SetCurCtrlView(_learnForceCtrl, false);
                    }
                    break;
                case AssistViewTab.CookUpGradeView:
                    {
                        if (_cookUpgradeCtrl == null)
                        {
                            _cookUpgradeCtrl = AddChild<AssistCookUpgradeViewController, AssistCookUpgradeView>(View.MainContent_Transform.gameObject, AssistCookUpgradeView.NAME);
                            OnChildCtrlAdd(AssistViewTab.CookUpGradeView, _cookUpgradeCtrl);
                        }
                        _cookUpgradeCtrl.UpdateView(data);
                        SetCurCtrlView(_cookUpgradeCtrl, true);
                    }
                    break;
                case AssistViewTab.CookProductView:
                    {
                        if (_cookProductCtrl == null)
                        {
                            _cookProductCtrl = AddChild<AssistCookProductViewController, AssistCookProductView>(View.MainContent_Transform.gameObject, AssistCookProductView.NAME);
                            OnChildCtrlAdd(AssistViewTab.CookProductView, _cookProductCtrl);
                        }
                        _cookProductCtrl.UpdateView(data);
                        SetCurCtrlView(_cookProductCtrl, true);
                    }
                    break;
                case AssistViewTab.ForceUpGradeView:
                    {
                        if (_forceUpgradeCtrl == null)
                        {
                            _forceUpgradeCtrl = AddChild<AssistForceUpgradeViewController, AssistForceUpgradeView>(View.MainContent_Transform.gameObject, AssistForceUpgradeView.NAME);
                            OnChildCtrlAdd(AssistViewTab.ForceUpGradeView, _forceUpgradeCtrl);
                        }
                        _forceUpgradeCtrl.UpdateView(data);
                        SetCurCtrlView(_forceUpgradeCtrl, true);
                    }
                    break;
                case AssistViewTab.ForceProductView:
                    {
                        if (_forceProductCtrl == null)
                        {
                            _forceProductCtrl = AddChild<AssistForceProductViewController, AssistForceProductView>(View.MainContent_Transform.gameObject, AssistForceProductView.NAME);
                            OnChildCtrlAdd(AssistViewTab.ForceProductView, _forceProductCtrl);
                        }
                        _forceProductCtrl.UpdateView(data);
                        SetCurCtrlView(_forceProductCtrl, true);
                    }
                    break;
            }
            #endregion

            if (lastCtrl == curCtrl)
                return;
            else
            {
                if (lastCtrl != null)
                    lastCtrl.Hide();
                curCtrl.Show();
            }
        }
    }

    private void ShowAssistDelegateView(bool isShow, IAssistSkillMainData data)
    {
        if(!isShow)
        {
            _assistDelegateCtrlList.ForEach(itemCtrl =>
            {
                if (itemCtrl != null)
                    itemCtrl.Hide();
            });

            return;
        }

        var lastCtrl = curCtrl;
        switch (data.CurDelegateTab)
        {
            case AssistViewTab.AssistDelegateMain:
                if (_delegateMainCtrl == null)
                {
                    _delegateMainCtrl = AddChild<AssistDelegateMainViewController, AssistDelegateMainView>(View.MainContent_Transform.gameObject, AssistDelegateMainView.NAME);
                    OnChildCtrlAdd(AssistViewTab.AssistDelegateMain, _delegateMainCtrl);
                    _assistDelegateCtrlList.Add(_delegateMainCtrl);
                }
                _delegateMainCtrl.UpdateView(data);
                curCtrl = _delegateMainCtrl;
                break;
        }

        if (lastCtrl == curCtrl)
            return;
        else
        {
            if (lastCtrl != null)
                lastCtrl.Hide();
            curCtrl.Show();
        }
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IAssistSkillMainData data)
    {
        tabbtnMgr.SetTabBtn((int)data.CurRightTab);

        if (data.CurRightTab == RightViewTab.AssistSkillView)  //生活技能
        {
            ShowAssistSkillView(true, data);
            ShowAssistDelegateView(false, data);
        }
        else if(data.CurRightTab == RightViewTab.AssistDelegateView)   //委托任务
        {
            ShowAssistSkillView(false, data);
            ShowAssistDelegateView(true, data);

            //红点测试xjd
//            RedPointDataMgr.DataMgr.UpdateSingleData(2, false);
        }
    }
    
    public void SetCurCtrlView(IMonolessViewController ctrl, bool isShowtab)
    {
        curCtrl = ctrl;
        _learnViewCtrl.SetIsShowTab(isShowtab);
        _assistSKillCtrlList.Add(ctrl);
    }
}
