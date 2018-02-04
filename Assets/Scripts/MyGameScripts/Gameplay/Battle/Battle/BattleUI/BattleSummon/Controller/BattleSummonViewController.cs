// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleSummonViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;
using UnityEngine;

public interface IBattleSummomData
{
    IEnumerable<IBattleCrewData> GetCrewDataList { get; }
    int GetFightTimes { get; }
    int Total { get; }
}

public class BattleSummonData: IBattleSummomData
{
    private List<IBattleCrewData> _crewDataList;
    private int _fightedTimes;  // 已召唤次数
    
    public IEnumerable<IBattleCrewData> GetCrewDataList {get { return _crewDataList; } }
    public int GetFightTimes { get { return _fightedTimes; } }
    public int Total {
        get { return _crewDataList.TryGetLength(); }
    }

    public static IBattleSummomData Create(List<IBattleCrewData> datalist, int times)
    {
        BattleSummonData data = new BattleSummonData();
        data._crewDataList = datalist;
        data._fightedTimes = times;
        return data;
    }
}

public partial interface IBattleSummonViewController
{
    UniRx.IObservable<Unit> GetCloseHandler { get; }
    void OpenSummonView(IBattleSummomData data);
    UniRx.IObservable<long> OnSummonBtnHandler { get; }
}

public partial class BattleSummonViewController
{
    private CompositeDisposable _disposable;

    private long _selectCrewId;
    private Subject<long> _SummonBtnEvt = new Subject<long>();
    public UniRx.IObservable<long> OnSummonBtnHandler { get { return _SummonBtnEvt; } }
    private Subject<Unit> _closeEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetCloseHandler { get { return _closeEvt; } }

    private List<BattleCrewItemController> _crewItemList = new List<BattleCrewItemController>();
    private List<AttrLabelController> _attrList = new List<AttrLabelController>();
    private List<SkillItemController> _skillItemList = new List<SkillItemController>();
    private List<SkillItemController> _magicList = new List<SkillItemController>();

    private List<Skill> _allSkillList = new List<Skill>();
    private List<Magic> _allMagicList = new List<Magic>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();

        _allSkillList = DataCache.getArrayByCls<Skill>();
        _allMagicList = DataCache.getArrayByCls<Magic>();

        for (int i = 0; i < _view.InfoGrid_UIGrid.transform.childCount; i++)
        {
            var go = _view.InfoGrid_UIGrid.GetChild(i);
            var item = AddController<AttrLabelController, AttrLabel>(go.gameObject);
            _attrList.Add(item);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnSummonButton_UIButtonClick.Subscribe(_ => { _SummonBtnEvt.OnNext(_selectCrewId);}));
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        UICamera.onClick -= OnCameraClick;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void OpenSummonView(IBattleSummomData data)
    {
        var datalist = data.GetCrewDataList;
        for (var i = _crewItemList.Count; i < datalist.Count(); i++)
        {
            var item = AddCachedChild<BattleCrewItemController, BattleCrewItem>(_view.PetListGrid_UIGrid.gameObject,
                BattleCrewItem.NAME);

            var index = i;
            var crewInfoDto = datalist.TryGetValue(index);
            item.SetBattleCrewInfo(crewInfoDto);
            _crewItemList.Add(item);
            _disposable.Add(item.OnClickHandler.Subscribe(_ =>
            {
                ShowCrewInfo(crewInfoDto.GetCrewInfoDto);
                _crewItemList.ForEachI((go, idx) => { go.IsSelect(index == idx); });
            }));
        }
        _view.PetListGrid_UIGrid.Reposition();
        ShowCrewInfo(datalist.TryGetValue(0).GetCrewInfoDto);

        _view.BattleInfoLabel_UILabel.text = string.Format("本场已出战:{0}/{1}", data.GetFightTimes, data.Total);
    }

    private void ShowCrewInfo(CrewInfoDto dto)
    {
        if (dto == null || dto.id == _selectCrewId)
            return;

        var crew = DataCache.getDtoByCls<GeneralCharactor>(dto.crewId) as Crew;
        if (crew == null)
        {
            GameDebuger.LogError(string.Format("Crew表找不到{0}", dto.crewId));
            return;
        }

        _selectCrewId = dto.id;
        _view.QuartzSpr_UISprite.spriteName = crew.typeIcon;
        _view.AttributeSpr_UISprite.spriteName = GlobalAttr.GetMagicIcon(dto.slotsElementLimit);
        _view.NameLb_UILabel.text = crew.name;
        _view.PowerLb_UILabel.text = string.Format("战力{0}", (int) dto.power);
        _view.LvLb_UILabel.text = dto.grade.ToString();
        UIHelper.SetPetIcon(_view.IconSpr_UISprite, crew.icon);
        SetProperties(dto);
        InitSkillList(dto.crewSkillsDto.craftsGradeDtos);   //战技
        InitMagicList(dto.crewSkillsDto.magic);         //魔法

    }

    private void SetProperties(CrewInfoDto infoDto)
    {
        _attrList.ForEachI((item, idx) =>
        {
            var dto = infoDto.properties.Find(s => s.propId == GlobalAttr.SECOND_ATTRS[idx]);
            if (dto != null)
                item.SetInfo(dto);
            else
            {
                CharacterPropertyDto propertyDto = new CharacterPropertyDto();
                propertyDto.propId = GlobalAttr.SECOND_ATTRS[idx];
                propertyDto.propValue = 0;
                item.SetInfo(propertyDto);
            }
        });
    }

    private void InitSkillList(IEnumerable<CraftsGradeDto> skillList)
    {
        for (int i = _skillItemList.Count; i < skillList.Count(); i++)
        {
            var item = AddChild<SkillItemController, SkillItem>(_view.SkillGrid_UIGrid.gameObject, SkillItem.NAME);
            _skillItemList.Add(item);
            item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            item.HideNameLb();
        }

        _skillItemList.ForEachI((item, idx) =>
        {
            if (idx < skillList.Count())
            {
                item.gameObject.SetActive(true);
                var Skill = _allSkillList.Find(d => d.id == skillList.TryGetValue(idx).id);
                item.UpdateView(Skill);
            }else
                item.gameObject.SetActive(false);
            
        });
        _view.SkillGrid_UIGrid.Reposition();
    }

    private void InitMagicList(IEnumerable<int> magicList)
    {
        for (int i = _magicList.Count; i < magicList.Count(); i++)
        {
            var item = AddChild<SkillItemController, SkillItem>(_view.MagicGrid_UIGrid.gameObject, SkillItem.NAME);
            _magicList.Add(item);
            item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            item.HideNameLb();
        }

        _magicList.ForEachI((item, idx) =>
        {
            if (idx < magicList.Count())
            {
                item.gameObject.SetActive(true);
                var magic = _allMagicList.Find(d => d.id == magicList.TryGetValue(idx));
                item.UpdateView(magic);
            }
            else
                item.gameObject.SetActive(false);
        });
        _view.MagicGrid_UIGrid.Reposition();
    }

    private void OnCameraClick(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (panel != _view.BattleSummonView_UIPanel
            && panel != _view.ScrollView_UIScrollView.panel)
           _closeEvt.OnNext(new Unit());
    }
}
