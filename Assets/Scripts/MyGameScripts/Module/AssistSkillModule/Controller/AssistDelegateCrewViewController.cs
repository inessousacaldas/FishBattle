// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistDelegateCrewViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IAssistDelegateCrewViewController
{
    //Subject<AssistCrewItemController.CrewChoseClickEvent> OnDelegateChoseCrewStream { get; }
    void UpdateView(IEnumerable<CrewShortDto> crewInfoList = null, IEnumerable<long> choseIdList = null, int curMissionId = 0, IEnumerable<long> ingCrewIdList = null);
}

public partial class AssistDelegateCrewViewController
{
    public static IAssistDelegateCrewViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IAssistDelegateCrewViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IAssistDelegateCrewViewController;

        return controller;
    }

    private List<AssistCrewItemController> _crewCtrlList = new List<AssistCrewItemController>();
    //private Subject<AssistCrewItemController.CrewChoseClickEvent> _choseCrewStream = new Subject<AssistCrewItemController.CrewChoseClickEvent>();
    //public Subject<AssistCrewItemController.CrewChoseClickEvent> OnDelegateChoseCrewStream { get { return _choseCrewStream; } }
    CompositeDisposable _disposable = new CompositeDisposable();
    private Dictionary<int, GeneralCharactor> _crewInfoList = DataCache.getDicByCls<GeneralCharactor>();
    private UISprite _lastChoseTypeSprite;
    private TabbtnManager tabBtnMgr;
    private Func<int, ITabBtnController> func;
    private int _curTab = 0;
    private int _curMissionId = 0;
    private List<CrewShortDto> _myCrewInfoList = new List<CrewShortDto>();
    private List<long> _choseIdList = new List<long>();
    private List<long> _ingCrewIdList = new List<long>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        tabBtnMgr = TabbtnManager.Create(
            AssistSkillMainDataMgr.AssistSkillMainData._ChoseCrewTabInfos
            , AddChannelTabBtn
            , 0
        );
        View.BtnGrid_UIGrid.Reposition();
    }

    private ITabBtnController AddChannelTabBtn(int i)
    {
        return AddTabBtn(i, _view.BtnGrid_UIGrid.gameObject, TabbtnPrefabPath.TabBtnWidget_ChatTab, "Tabbtn_");
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, TabbtnPrefabPath tabPath, string name)
    {
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath.ToString()
            , name + i);

        ctrl.SetBtnImages("betton_b", "betton_a");
        ctrl.SetBtnLblFont(selectColor: "65480b", normalColor: "636262");
        return ctrl;
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            UIModuleManager.Instance.CloseModule(AssistDelegateCrewView.NAME);
        });

        tabBtnMgr.Stream.Subscribe(i =>
        {
            _curTab = i;
            UpdateView();
        });

        ConfirmBtn_UIButtonEvt.Subscribe(_ =>
        {
            AssistSkillMainDataMgr.DataMgr.ResetChoseCrewList(_choseIdList);
            UIModuleManager.Instance.CloseModule(AssistDelegateCrewView.NAME);
        });
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }    

    public void UpdateView(IEnumerable<CrewShortDto> crewInfoList=null, IEnumerable<long> choseIdList = null, int curMissionId=0, IEnumerable<long> ingCrewIdList = null)
    {
        _disposable.Clear();
        if (crewInfoList != null)
            _myCrewInfoList = crewInfoList.ToList();
        if (choseIdList != null )
            _choseIdList = choseIdList.ToList();
        if (curMissionId != 0)
            _curMissionId = curMissionId;
        if (ingCrewIdList != null)
            _ingCrewIdList = ingCrewIdList.ToList();
        if (_myCrewInfoList.IsNullOrEmpty() || _curMissionId == 0)
            return;

        _crewCtrlList.ForEach(item => { item.Hide(); });
        _myCrewInfoList.ForEachI((info, index) =>
        {
            if (_crewInfoList.ContainsKey(info.crewId)
            && (_curTab == (int)PropertyType.All || (_crewInfoList[info.crewId] as Crew).property == _curTab))
            {
                var ctrl = AddCrewItemIfNotExist(index);
                ctrl.UpdateView(info, _crewInfoList[info.crewId] as Crew, _choseIdList.Contains(info.id), _curMissionId, _ingCrewIdList.Contains(info.id));
                ctrl.Show();
                _disposable.Add(ctrl.OnClickChoseStream.Subscribe(crewEvent =>
                {
                    if (crewEvent.isSelect && !_choseIdList.Contains(crewEvent.id))
                    {
                        if (_choseIdList.Count == 4)
                        {
                            TipManager.AddTip("最多选择4名伙伴");
                            ctrl.SetIsChose(false);
                        }
                        else
                            _choseIdList.Add(crewEvent.id);
                    }
                    else if (!crewEvent.isSelect && _choseIdList.Contains(crewEvent.id))
                        _choseIdList.Remove(crewEvent.id);
                }));
            }
        });

        View.IconGrid_UIGrid.Reposition();
    }

    private AssistCrewItemController AddCrewItemIfNotExist(int idx)
    {
        AssistCrewItemController ctrl = null;
        _crewCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<AssistCrewItemController, AssistCrewItem>(View.IconGrid_UIGrid.gameObject, AssistCrewItem.NAME);
            _crewCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
