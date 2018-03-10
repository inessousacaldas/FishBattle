// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewCraftSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using System.Collections.Generic;
using UniRx;

public partial interface ICrewCraftSkillViewController
{
    void SetData(ICrewSkillData data);
    void UpdateView(int id);

    UniRx.IObservable<ICrewSkillItemController> CraftsSkillUpClick { get; }

    UniRx.IObservable<ICrewSkillItemController> CraftsSkillIconClick { get; }

    void UpdateSkillDescripe(CrewSkillItemController skillItem);
    CrewSkillItemController CurSkillItem { get; }

}

public partial class CrewCraftSkillViewController
{

    private Subject<ICrewSkillItemController> _craftsSkillUpClick = new Subject<ICrewSkillItemController>();
    public UniRx.IObservable<ICrewSkillItemController> CraftsSkillUpClick { get { return _craftsSkillUpClick; } }

    private Subject<ICrewSkillItemController> _craftsSkillIconClick = new Subject<ICrewSkillItemController>();
    public UniRx.IObservable<ICrewSkillItemController> CraftsSkillIconClick { get { return _craftsSkillIconClick; } }

    private CrewSkillItemController _curSkillItem;
    public CrewSkillItemController CurSkillItem { get { return _curSkillItem; } }

    private ICrewSkillData _data;
    private RoleSkillRangeController _rangeCtrl;//技能范围

    private int _curCraftsID = 0;//当前点击的战技ID
    private int _curCrewID = 0;
    private CompositeDisposable _disposable;

    private List<CrewSkillItemController> _skillItemList = new List<CrewSkillItemController>();

    private ItemCellController _reward1;
    private ItemCellController _reward2;
    private ItemCellController _reward3;



    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _disposable.Add(OnSkillUpgradeBtn_UIButtonClick.Subscribe(e => { _craftsSkillUpClick.OnNext(CurSkillItem); }));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _rangeCtrl = null;
        _data = null;
        _skillItemList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void SetData(ICrewSkillData data)
    {
        _data = data;
    }

    public void UpdateView(int crewId)
    {

        var list = _data.GetSkillCrafts(crewId);
        if (list == null) return;
        CreateSkillItem();
        UpdateSkillItem(list);

        if (_curSkillItem == null || _curCrewID != crewId)
            UpdateSkillDescripe(_skillItemList[0]);
        else
            UpdateSkillDescripe(_curSkillItem);

        _curCrewID = crewId;
    }

    public void CreateSkillItem()
    {
        if (_skillItemList.Count > 0) return;
        for (int i = 0; i < _view.SkillIconGroup_Transform.childCount; i++)
        {
            var lb = _view.SkillIconGroup_Transform.GetChild(i).gameObject;
            var com = AddController<CrewSkillItemController, CrewSkillItem>(lb);
            _skillItemList.Add(com);
            _disposable.Add(com.OnCrewSkillItem_UIButtonClick.Subscribe(_ => { _craftsSkillIconClick.OnNext(com); }));
        }
    }
    public void UpdateSkillItem(List<CrewSkillCraftsVO> list)
    {
        _skillItemList.ForEachI((ctrl, i) =>
        {
            if (i > list.Count)
            {
                ctrl.View.gameObject.SetActive(false);
            }
            else
            {
                ctrl.UpdateView(list[i]);
                ctrl.View.gameObject.SetActive(true);
            }
        });
    }



    public void UpdateSkillDescripe(CrewSkillItemController skillItem)
    {
        if (skillItem.SkillVO == null) return;
        _curSkillItem = skillItem;
        CrewSkillCraftsVO skillVO = skillItem.SkillVO as CrewSkillCraftsVO;
        _view.SkillNameLbl_UILabel.text = skillVO.Name;
        _view.SkillTypeLbl_UILabel.text = "战技";
        _view.SkillAfterLbl_UILabel.text = skillVO.SkillTimeAfter;
        _view.SkillNatureLbl_UILabel.text = skillVO.SkillType;
        _view.SkillScopeLbl_UILabel.text = skillVO.Scope;
        _view.SkillConsumeLbl_UILabel.text = skillVO.cfgVO.consume + "CP";
        _view.SkillBeforeLbl_UILabel.text = skillVO.SkillTimeBefore;
        _view.SkillEffectLabel_UILabel.text = RoleSkillUtils.Formula(skillVO.SkillDes, skillVO.Grade);
        _view.SkillLevelLbl_UILabel.text = string.Format("等级：{0}级", skillVO.Grade.ToString());

        int grade = CrewViewDataMgr.DataMgr.GetCurCrewGrade;
        int limit = skillVO.Grade * 5 + skillVO.cfgVO.playerGradeLimit;
        if (limit > grade)
            View.SkillLimitLbl_UILabel.text = "[ff0000]" + limit + "级[-]";
        else
            View.SkillLimitLbl_UILabel.text = "[ffffff]" + limit + "级[-]";
        UpdateUpgradeMaterial(skillVO);
        UpdateRightRange(skillVO.cfgVO.scopeId);
    }
    private void UpdateRightRange(int scopeId)
    {
        CrewSkillCraftsData tmp = CrewSkillHelper.GetCraftsData();
        var scopeVO = tmp.GetScopeByID(scopeId);
        var type = tmp.GetScopeTarType(scopeId);
        if (_rangeCtrl == null)
        {
            _rangeCtrl = RoleSkillRangeController.Show(View.rangeTrans_Transform.gameObject, scopeVO.scopeIndex, type);
            _rangeCtrl.transform.localPosition = new UnityEngine.Vector3(0, -17);
            _rangeCtrl.transform.localScale = UnityEngine.Vector3.one;
        }
        else
        {
            _rangeCtrl.Show(scopeVO.scopeIndex, type);
        }
    }

    private void UpdateUpgradeMaterial(CrewSkillCraftsVO crafts)
    {
        CrewCraftsGrade cost = CrewSkillDataMgr.DataMgr.CraftsData.GetCostByGradeDto(crafts);
        if (cost.silver > 0)
        {
            var item = DataCache.getDtoByCls<GeneralItem>((int)AppVirtualItem.VirtualItemEnum.SILVER);
            if (_reward3 == null)
            {
                _reward3 = AddChild<ItemCellController, ItemCell>(View.MaterialGrid_UIGrid.gameObject, ItemCell.Prefab_ItemCell);
                int count = (int)ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
                _reward3.UpdateViewInCrewSkill(item, count, cost.silver,font:_view.SkillAfterLbl_UILabel.bitmapFont);
            }
            _reward3.Show();
        }
        else
        {
            if (_reward3 != null)
            {
                _reward3.View.Hide();
            }
        }

        if (cost.itemCount1 > 0)
        {
            if (_reward1 == null)
            {
                _reward1 = AddChild<ItemCellController, ItemCell>(
                    View.MaterialGrid_UIGrid.gameObject,
                    ItemCell.Prefab_ItemCell
                );
            }
            _reward1.Show();
            _reward1.UpdateViewInCrewSkill(crafts.cfgVO.item1, BackpackDataMgr.DataMgr.GetItemCountByItemID(crafts.cfgVO.consumeBook1), cost.itemCount1, font: _view.SkillAfterLbl_UILabel.bitmapFont);
        }
        else
        {
            if (_reward1 != null)
            {
                _reward1.View.Hide();
            }
        }

        if (cost.itemCount2 > 0)
        {
            if (_reward2 == null)
            {
                _reward2 = AddChild<ItemCellController, ItemCell>(
                    View.MaterialGrid_UIGrid.gameObject,
                    ItemCell.Prefab_ItemCell
                );
            }
            _reward2.Show();
            _reward2.UpdateViewInCrewSkill(crafts.cfgVO.item2, BackpackDataMgr.DataMgr.GetItemCountByItemID(crafts.cfgVO.consumeBook2), cost.itemCount2, font: _view.SkillAfterLbl_UILabel.bitmapFont);
        }
        else
        {
            if (_reward2 != null)
            {
                _reward2.View.Hide();
            }
        }

        _view.MaterialGrid_UIGrid.Reposition();
    }

}
