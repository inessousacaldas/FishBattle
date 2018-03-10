// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FormationUpdateViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public class FormationUpdateMaterialsData
{
    private int _materialsId;
    private int _needCount;
    private int _hadCount;

    public int GetMaterialId { get { return _materialsId; } }
    public int GetNeedCount { get { return _needCount; } }
    public int GetHadCount { get { return _hadCount; } }

    

    public static FormationUpdateMaterialsData Create(int id, int hadCount, int needCount)
    {
        FormationUpdateMaterialsData data = new FormationUpdateMaterialsData();
        data._materialsId = id;
        data._needCount = needCount;
        data._hadCount = hadCount;
        return data;
    }
}

public partial interface IFormationUpdateViewController
{
    UniRx.IObservable<int> GetOnClickHandler { get; }
}

public partial class FormationUpdateViewController
{
    private CompositeDisposable _disposable;
    private const int Materials = 4;    //升级所需材料种类
    private int _curFormationId;
    private PageTurnViewController _pageTurn;
    private IEnumerable<Formation> _formations;
    private List<ItemCellController> _cellList;
    private List<FormationGrade> _formationGradeList = new List<FormationGrade>(); 

    private Subject<int> _onClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetOnClickHandler { get { return _onClickEvt; } }  

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitCellItem();
        InitTurnPage();
    }

    protected override void UpdateDataAndView(ITeamFormationData data)
    {
        var fid = data.CurUpFradeFormation.id;
        var curLev = data.GetFormationLevel(fid);
        UpdateView(data.CurUpFradeFormation, curLev);
        UpdateMaterials(fid, curLev);
        //LearnFormation(curLev == 0);    //0级为学习
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        _disposable.Add(OnUpgradeBtn_UIButtonClick.Subscribe(_ => { _onClickEvt.OnNext(_curFormationId);}));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        _disposable = new CompositeDisposable();
        _cellList = new List<ItemCellController>(Materials);
        _formationGradeList = DataCache.getArrayByCls<FormationGrade>();
    }

    private void InitTurnPage()
    {
        _formations = TeamFormationDataMgr.DataMgr.GetAllFormationInfo()
            .Filter(s => s.id != (int) Formation.FormationType.Regular).ToList();

        _pageTurn = AddController<PageTurnViewController, PageTurnView>(_view.PageTurnView);
        var max = _formations.Count();

        var curPage = _formations.FindElementIdx(d => d.id == TeamFormationDataMgr.DataMgr.GetUpGradeFormation().id);
        _pageTurn.InitData(
            curPage
            , 0
            , max
            , false
            , ShowType.blank);
        _disposable.Add(_pageTurn.Stream.Subscribe(page => UpdatePage(page)));
        UpdatePage(curPage);    //第一次打开界面设置一次数据
    }

    private void InitCellItem()
    {
        for (int i = 0; i < Materials; i++)
        {
            var go = _view.ItemGrid_UIGrid.GetChild(i);
            var item = AddController<ItemCellController, ItemCell>(go.gameObject);
            item.CanDisplayCount(false);
            item.UpdateViewWithNull();
            _cellList.Add(item);
        }
        
        _view.ItemGrid_UIGrid.Reposition();
    }

    private void UpdateMaterials(int id, int lv)
    {
        if (lv == _formationGradeList.Count)
            return;

        var materials = FormationHelper.GetMaterial(id, lv + 1);    //下一级
        materials.ForEachI((m, i)=>
        {
            var cell = _cellList.TryGetValue(i);
            cell.gameObject.SetActive(i < _cellList.Count && m.GetNeedCount > 0);
            if (i < _cellList.Count)
            {
                var gItem = ItemHelper.GetGeneralItemByItemId(m.GetMaterialId);
                cell.UpdateView(gItem);
                if(m.GetNeedCount > m.GetHadCount)
                    cell.SetNameLabel(string.Format("{0}/{1}", m.GetHadCount, m.GetNeedCount).WrapColor(ColorConstantV3.Color_Red_Str));
                else
                    cell.SetNameLabel(string.Format("{0}/{1}", m.GetHadCount, m.GetNeedCount).WrapColor("ffffff"));
            }
        });
        _view.ItemGrid_UIGrid.Reposition();
    }

    public void UpdatePage(int page)
    {
        _formations.ForEachI((data, i) =>
        {
            if (i == page)
            {
                _curFormationId = data.id;
                var lv = TeamFormationDataMgr.DataMgr.GetFormationLevel(data.id);
                UpdateView(data, lv);
                UpdateMaterials(data.id, lv);
                //LearnFormation(lv == 0);    //0级为学习
            }
        });
    }

    public void UpdateView(Formation formation, int curLev)
    {
        var maxLev = _formationGradeList.Count; //队形最大等级
        var playerLv = ModelManager.Player.GetPlayerLevel();
        var curMaxLev = 1;  //玩家当前等级下队形升级上限

        _formationGradeList.ForEach(d =>
        {
            if (d.gradeLimit <= playerLv)
                curMaxLev = d.id;
        });
        
        _view.UpgradeLabel_UILabel.gameObject.SetActive(false);
        _view.UpgradeBtn_UIButton.gameObject.SetActive(true);

        _view.formationIcon_UISprite.spriteName = string.Format("formationicon_{0}", formation.id);
        _view.NextFormationIcon_UISprite.spriteName = string.Format(string.Format("formationicon_{0}", formation.id));

        _view.formationName_UILabel.text = string.Format("{0}·{1}级", formation.name, curLev == 0 ? 1 : curLev);
        _view.nextFormationName_UILabel.text = string.Format("{0}·{1}级", formation.name, curLev + 1);
        IsMaxLevel(curLev == maxLev);

        if (curLev == 0)
        {
            _view.tipLabel_UILabel.text = "学习队形需要消耗以下材料：";
            _view.upgradeBtnLabel_UILabel.text = "学习";
            _view.NectFormationIcon.SetActive(false);
            _view.Arrow.SetActive(false);
            _view.formationIcon.transform.localPosition = new Vector3(0, -20, 0);
        }
        else if (curLev < Math.Min(maxLev, curMaxLev))
        {
            _view.tipLabel_UILabel.text = "升级队形需要消耗以下材料：";
            _view.upgradeBtnLabel_UILabel.text = "升级";
            _view.NectFormationIcon.SetActive(true);
            _view.Arrow.SetActive(true);
            _view.formationIcon.transform.localPosition = new Vector3(-95, -20, 0);
        }
        else if (curLev == curMaxLev && curMaxLev < maxLev)
        {
            _view.NectFormationIcon.SetActive(true);
            _view.Arrow.SetActive(true);
            _view.tipLabel_UILabel.text = "升级队形需要消耗以下材料：";
            _view.UpgradeLabel_UILabel.text = string.Format("{0}级后可继续升级队形", FormationHelper.GetPlayerLev(curLev + 1));
            _view.UpgradeLabel_UILabel.gameObject.SetActive(true);
            _view.UpgradeBtn_UIButton.gameObject.SetActive(false);
            _view.formationIcon.transform.localPosition = new Vector3(-95, -20, 0);
        }
        else if (curLev == maxLev)
        {
            _view.NectFormationIcon.SetActive(false);
            _view.Arrow.SetActive(false);
            _view.tipLabel_UILabel.text = string.Empty;
            _view.formationIcon.transform.localPosition = new Vector3(0, -20, 0);
        }
    }

    private void IsMaxLevel(bool b)
    {
        _view.ItemGrid_UIGrid.gameObject.SetActive(!b);
        _view.tipLabel_UILabel.gameObject.SetActive(!b);
        _view.UpgradeBtn_UIButton.gameObject.SetActive(!b);
        _view.MaxLevelLabel_UILabel.gameObject.SetActive(b);
    }
}
