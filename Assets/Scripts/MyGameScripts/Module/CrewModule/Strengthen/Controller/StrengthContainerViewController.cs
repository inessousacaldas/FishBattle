// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  StrengthContainerViewController.cs
// Author   : wujunjie
// Created  : 8/10/2017 7:46:59 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public partial interface IStrengthContainerViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(CrewStrengthenTab index, ICrewSkillTrainData data);
    void UpdateView(CrewInfoDto data, Crew crew, int chips);
    void OnCheckDetailBtnClick();
    UniRx.IObservable<Unit> OnDevelopEffectBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> GetEnterSaveHandler { get; }
    void ShowWindows(bool show,ICrewSkillTrainData data = null,int id = -1);

    void OpenLeft();
    void OpenMiddle();
    void OpenRight();
    void StrengthTips();
    void OnDevelopTipBtnClick();
}
public partial class StrengthContainerViewController :
    MonolessViewController<StrengthContainerView>
    , IStrengthContainerViewController
{
    public UniRx.IObservable<Unit> OnDevelopEffectBtn_UIButtonClick { get { return null; } }

    // 界面初始化完成之后的一些后续初始化工作

    private static List<ITabInfo> tabInfoList = new List<ITabInfo>
    {
        TabInfoData.Create((int)CrewStrengthenTab.Phase,"强化")
       ,TabInfoData.Create((int)CrewStrengthenTab.Raise,"进阶")
       ,TabInfoData.Create((int)CrewStrengthenTab.Craft,"研修")

    };
    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }
    private TabbtnManager tabMgr = null;

    private CrewInfoDto _data;
    private MasterialItemController _strengthenMasterial;

    private readonly int _strengthenNum = 2;
    private readonly int _developeNum = 3;
    private readonly int _developeMasterialNum = 3;  //进阶材料格子数量
    private readonly int _progressBarNum = 4;
    private readonly int _phaseNum = 5;   //提升一个品质需要进阶的次数
    private int _MaxPhase;   //最大进阶次数
    private int _maxRaise;

    private List<CrewPhase> _allDevelopList = new List<CrewPhase>();
    private List<CrewRaise> _allStrengthList = new List<CrewRaise>();
    private List<Crew> _allCrewList = new List<Crew>();
    private List<DevelopInfoItemController> _strengthenList = new List<DevelopInfoItemController>();
    private List<DevelopInfoItemController> _developList = new List<DevelopInfoItemController>();
    private List<MasterialItemController> _developMasterialList = new List<MasterialItemController>();

    private List<Transform> _strenLightList = new List<Transform>();
    private List<Transform> _strenYelLineList = new List<Transform>();
    private List<Transform> _devLightList = new List<Transform>();

    private CrewSkillTrainingViewController trainingCtrl;           //研修

    private Subject<Unit> _enterSaveEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetEnterSaveHandler { get { return _enterSaveEvt; } }

    private void CreateTabItem()
    {
        tabMgr = TabbtnManager.Create(tabInfoList, i => AddTabBtn(i, View.StrengthenPageGrid_UIGrid.gameObject, "TabBtnWidget_CrewSkill", "Tabbtn_"));
        tabMgr.SetBtnLblFont(selectColor: "000000", normalColor: "ffffff");
    }
    private ITabBtnController AddTabBtn(int i, GameObject parent, string tabPath, string name)
    {
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath
            , name + i);
        return ctrl;
    }

    public void OnTabChange(CrewStrengthenTab index, ICrewSkillTrainData data)
    {
        View.PartnerStrengthenView.gameObject.SetActive(index == CrewStrengthenTab.Phase);
        View.PartnerDevelopView.gameObject.SetActive(index == CrewStrengthenTab.Raise);
        View.TrainView_Transform.gameObject.SetActive(index == CrewStrengthenTab.Craft);
    }

    #region 研修

    public CrewSkillTrainingViewController InitCrewSkillTrain()
    {
        if (trainingCtrl == null)
        {
            trainingCtrl = AddChild<CrewSkillTrainingViewController, CrewSkillTrainingView>(
                View.TrainView_Transform.gameObject,
                CrewSkillTrainingView.NAME
                );
            return trainingCtrl;
        }
        return null;
    }
    public void UpdateTrainView(ICrewSkillTrainData trainData, int id)
    {
        if (trainingCtrl != null)
        {
            trainingCtrl.UpdateView(trainData, id);
        }
        ShowWindows(false);
    }

    public void ShowWindows(bool show, ICrewSkillTrainData data = null, int id = -1)
    {

        if (data != null)
        {
            var list = data.GetTrainList(id);
            if (list == null || list.aftCraDto.Count == 0)
            {
                TipManager.AddTip("当前没有研修技能");
                return;
            }
        }

        if (!show) return;
        var controller = ProxyBaseWinModule.Open();
        var title = "保存研修";
        var txt = "保存后将替换掉现有战技和成长率，确定保存吗？";
        BaseTipData tipData = BaseTipData.Create(title, txt, 0, () => { _enterSaveEvt.OnNext(new Unit()); }, null);
        controller.InitView(tipData);
    }

    public void OpenLeft()
    {
        View.DevelopStrengthGrid_PageScrollView.SkipToPage(1, true);
    }

    public void OpenMiddle()
    {
        View.DevelopStrengthGrid_PageScrollView.SkipToPage(2, true);
    }

    public void OpenRight()
    {
        View.DevelopStrengthGrid_PageScrollView.SkipToPage(3, true);
    }

    public void StrengthTips()
    {
        ProxyTips.OpenTextTips(5, new Vector3(174, 125, 0));
    }
    #endregion
    public void UpdateView(CrewInfoDto data, Crew crew, int chips)
    {
        _data = data;
        UpdateStrengthenData(data, crew, chips);
        UpdateStrengthenInfo(data);
        UpdateConsumeMasterial(data);
        UpdateDevelopInfo(data);
        UpdateDevelopData(data);
    }

    private void UpdateDevelopInfo(CrewInfoDto data)
    {
        if (data == null)
            return;

        CrewPhase _beforePhase = _allDevelopList.TryGetValue(data.phase);
        List<CharacterPropertyDto> properties = data.properties;

        _developList.ForEachI((item, indx) =>
        {
            if (data.phase == _MaxPhase)
            {
                properties.ForEach(datas =>
                {
                    if (datas.propId == _beforePhase.addProperties[indx])
                        item.SetBestInfo(datas.propId, datas.propValue);
                });
            }
            else
            {
                if (data.nextPhaseProperties.TryGetValue(indx) != null
                    && data.phaseProperties.TryGetValue(indx) != null)
                    item.SetInfo(data.nextPhaseProperties[indx], data.phaseProperties[indx]);
            }
        });
    }
    private void UpdateDevelopData(CrewInfoDto data)
    {
        if (data == null)
            return;

        SetDevQualityIcon(data.quality);
        if (_devLightList.Count == 0)
        {
            for (int i = 0; i < View.devLigth.transform.childCount; i++)
            {
                _devLightList.Add(View.devLigth.transform.GetChild(i));
            }
        }
        int phase = data.phase % _phaseNum;

        if (data.phase == _MaxPhase)
        {
            _devLightList.ForEach(light => { light.gameObject.SetActive(true); });
        }
        else if (phase == 0)
        {
            _devLightList.ForEach(light => { light.gameObject.SetActive(false); });
        }
        else
        {

            _devLightList.ForEachI((light, idx) =>
            {
                light.gameObject.SetActive(phase > idx);

            });
        }


    }
    private void SetDevQualityIcon(int quality)
    {

        string iconName = "";
        switch (quality)
        {
            case 3:iconName = "icon";break;
            case 4:iconName = "icon_1";break;
            case 5: iconName = "icon_2";break;
            case 6:iconName = "icon_3";break;
        }
        _view.QualityIcon_UISprite.spriteName = iconName;
    }

    private void UpdateConsumeMasterial(CrewInfoDto data)
    {
        if (data == null)
            return;
        View.BestDevelopLab.SetActive(data.phase == _MaxPhase);
        View.DevelopButton_UIButton.gameObject.SetActive(data.phase != _MaxPhase);
        View.DevConsume.SetActive(data.phase != _MaxPhase);
        View.DevLightGroup.transform.localPosition = data.phase != _MaxPhase ?
           Vector3.zero : new Vector3(0, -40, 0);

        CrewPhase _beforePhase = _allDevelopList.TryGetValue(data.phase);
        var afterPhaseNum = data.phase + 1;

        if (data.phase == _MaxPhase)
            afterPhaseNum = data.phase;

        CrewPhase _afterPhaseItem = _allDevelopList.TryGetValue(afterPhaseNum);
        int _maxMasterialIdx = 1;  //最大消耗材料索引值
        if (_afterPhaseItem.itemId.Count > _maxMasterialIdx)
        {
            _developMasterialList.ForEachI((item, index) =>
            {
                if (index > _maxMasterialIdx)
                {
                    item.gameObject.SetActive(true);
                    item.SetSilverInfo(_beforePhase.silver);
                }
                else
                    item.SetInfo(_afterPhaseItem.itemId[index], _afterPhaseItem.amount[index]);
            });
        }
        else
        {
            _developMasterialList.ForEachI((item, idx) =>
            {
                if (idx == _maxMasterialIdx)
                {
                    item.SetSilverInfo(_afterPhaseItem.silver);
                }
                else if (idx > _maxMasterialIdx)
                {
                    item.gameObject.SetActive(false);
                }
                else
                    item.SetInfo(_afterPhaseItem.itemId[idx], _afterPhaseItem.amount[idx]);
            });
        }
    }


    private void UpdateStrengthenData(CrewInfoDto data, Crew crew, int chips)
    {
        if (data == null)
            return;
        View.StrengthButton_UIButton.gameObject.SetActive(data.raise != _maxRaise);
        View.Consume.SetActive(data.raise != _maxRaise);
        View.BestStrengthLab.gameObject.SetActive(data.raise == _maxRaise);

        if (data != null)
        {
            string crewIcon = _allCrewList.Find(d => d.id == data.crewId).icon;

            View.BeforeStrength_UILabel.text = "强化+" + data.raise;
            View.AfterStrength_UILabel.text = data.raise == _maxRaise ? "强化+" + _maxRaise : "强化+" + (data.raise + 1);
        }
        if (_strenLightList.Count == 0)
        {
            for(int i = 0; i < View.LightGroup.transform.childCount; i++)
            { 
                _strenLightList.Add(View.LightGroup.transform.GetChild(i));
            }
            for(int i = 0; i < View.YellowLineGroup.transform.childCount; i++)
            {
                _strenYelLineList.Add(View.YellowLineGroup.transform.GetChild(i));
            }
        }
        _strenLightList.ForEachI((light, idx) =>
        {
            light.gameObject.SetActive(idx < data.raise);
        });
        _strenYelLineList.ForEachI((line, idx) =>
        {
            line.gameObject.SetActive(data.raise > 1 && idx < data.raise - 1);
        });

        if (_allStrengthList.TryGetValue(data.raise) == null)
            return;

        var afterRaise = data.raise == _maxRaise ? data.raise : data.raise + 1;
        CrewRaise raise = _allStrengthList.TryGetValue(afterRaise);
        if (raise == null)
        {
            GameDebuger.LogError(string.Format("CrewRaise找不到{0},请检查(这一行不算报错)", afterRaise));
            return;
        }
        raise.chips.Split(',').ForEachI((f, idx) =>
        {
            int lv;
            bool b = int.TryParse(f.Split(':')[0], out lv);
            if (b && crew.rare == lv)
            {
                int cout;
                bool _b = int.TryParse(f.Split(':')[1], out cout);
                if (_b)
                    _strengthenMasterial.SetStrengthInfo(cout, crew, chips);
            }
        });
    }

    private void UpdateStrengthenInfo(CrewInfoDto data)
    {
        if (data == null) return;

        CrewRaise _beforeRaise = _allStrengthList.TryGetValue(data.raise);
        var afterRaise = data.raise == _maxRaise ? data.raise : data.raise + 1;
        CrewRaise _afterRaise = _allStrengthList.TryGetValue(afterRaise);

        if (data.raise == _maxRaise)
            _strengthenList[0].SetStrengthenInfo(_beforeRaise, _beforeRaise);
        else
            _strengthenList[0].SetStrengthenInfo(_beforeRaise, _afterRaise);

        _strengthenList[1].SetCombatInfo(Mathf.Ceil(data.power), Mathf.Ceil(data.nextRaisePower));
    }

    public void OnCheckDetailBtnClick()
    {
        var controller = UIModuleManager.Instance.OpenFunModule<DetailInfoController>(DetailInfoView.NAME, UILayerType.SubModule, true);

        controller.InitTitle("强化效果");
        controller.SetEffectLb(_data.nextRaiseProperties.Count, _data.nextRaiseProperties, _data.properties);
        controller.SetPosition(new Vector3(-136, -20, 0));
    }

    public void OnDevelopTipBtnClick()
    {
        View.DevelopTipsGroup.SetActive(true);
        UICamera.onPress += OnDevelopTipViewClose;
    }
    public void OnDevelopTipViewClose(GameObject go,bool isPress)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != null)
        {
            if (panel.gameObject != _view.DevelopTipsGroup)
            {
                View.DevelopTipsGroup.SetActive(false);
                UICamera.onPress -= OnDevelopTipViewClose;
            }
        }
    }
    private void OnDevelopTipViewClick()
    {

    }

    protected override void AfterInitView()
    {
        CreateTabItem();

        InitData();
        InitStrengthenList();
        InitDevelopList();
        InitDevelopMasterialList();
        InitStrengthMasterial();
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
        _strengthenList.Clear();
        _developList.Clear();
        _developMasterialList.Clear();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据

    private void InitData()
    {
        _MaxPhase = DataCache.GetStaticConfigValue(AppStaticConfigs.CREW_PHASE_MAX_TIME, 0);
        _maxRaise = DataCache.GetStaticConfigValue(AppStaticConfigs.CREW_RAISE_MAX_TIME, 0);

        CrewPhase _zeroCrewPhase = new CrewPhase();
        _zeroCrewPhase.id = 0;
        _zeroCrewPhase.itemId = new List<int>() { 500001 };
        _zeroCrewPhase.amount = null;
        _zeroCrewPhase.silver = 100;
        _zeroCrewPhase.addProperties = new List<int>() { 201, 202, 204 };
        _allDevelopList.Add(_zeroCrewPhase);
        DataCache.getArrayByCls<CrewPhase>().ForEach(d => { _allDevelopList.Add(d); });

        CrewRaise _zeroCrewRaise = new CrewRaise();
        _zeroCrewRaise.id = 0;
        _zeroCrewRaise.chips = "3:0,4:0,5:0,6:0";
        _zeroCrewRaise.gradeLimit = 0;
        _zeroCrewRaise.ratio = 0;
        _allStrengthList.Add(_zeroCrewRaise);
        DataCache.getArrayByCls<CrewRaise>().ForEach(c => _allStrengthList.Add(c));
        DataCache.getArrayByCls<GeneralCharactor>().ForEach(d =>
        {
            if (d is Crew)
                _allCrewList.Add(d as Crew);
        });
    }

    private void InitStrengthenList()
    {
        for (int i = 0; i < _strengthenNum; i++)
        {
            var com = AddChild<DevelopInfoItemController, DevelopInfoItem>(
                _view.StrengthInfoGrid_UIGrid.gameObject
                , DevelopInfoItem.NAME);
            _strengthenList.Add(com);
        }
    }

    private void InitDevelopList()
    {
        for (int i = 0; i < _developeNum; i++)
        {
            var item = AddChild<DevelopInfoItemController, DevelopInfoItem>(
                _view.InfoTable_UITable.gameObject
                , DevelopInfoItem.NAME);
            _developList.Add(item);
        }
    }

    private void InitDevelopMasterialList()
    {
        for (int i = 0; i < _developeMasterialNum; i++)
        {
            var lb = _view.ExpendGrid_Transform.GetChild(i).gameObject;
            var com = AddController<MasterialItemController, MasterialItem>(lb);
            _developMasterialList.Add(com);
        }
    }

    private void InitStrengthMasterial()
    {
        _strengthenMasterial = AddChild<MasterialItemController, MasterialItem>(
           _view.StrengthMasAnchor_Transform.gameObject
           , MasterialItem.NAME);
    }

    private string SetLabelString(int quality)
    {
        string color = "";
        switch (quality)
        {
            case (int)CrewQuality.Blue:
                color = "蓝色";
                break;
            case (int)CrewQuality.Purple:
                color = "紫色";
                break;
            case (int)CrewQuality.Orange:
                color = "橙色";
                break;
            case (int)CrewQuality.Red:
                color = "红色";
                break;
        }
        return color;
    }
}
