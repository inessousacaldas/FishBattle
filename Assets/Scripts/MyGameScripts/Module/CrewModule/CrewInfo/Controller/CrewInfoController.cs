using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;

public partial interface ICrewInfoController
{
    void UpdateView(CrewInfoDto data, int crewId = 0);
    void SetGoActive(bool b);
}

public partial class CrewInfoController:
    MonolessViewController<CrewInfoView>
    ,ICrewInfoController
{
    private CompositeDisposable _disposable;
    private CrewInfoDto _data;
    private Crew _crew;
    
    private int _maxCrewSkill = 4;    //未获得伙伴最多显示4个技能
    private int _detailInfoLevel = 3;   //3代表详细属性界面显示内容
	
    private List<AttrLabelController> _baseAttrList = new List<AttrLabelController>();
    private List<AttrLabelController> _secondAttrList = new List<AttrLabelController>();
    private List<ItemCellController> _skillList = new List<ItemCellController>(); 
    private CrewPropPointController _propPoint;

    public static Comparison<CharacterAbility> _comparison = null;
    private readonly string[] PersonalityType = {"", "正直", "勇敢", "谨慎", "冷静"};
    private readonly string[] AstralSprite = {"AstralFigure_ancillary",
        "AstralFigure_control", "AstralFigure_magic", "AstralFigure_strength"};

    private enum CrewBookTab
    {
        Info = 1,
        Quartz = 2
    }

    public void UpdateView(CrewInfoDto data, int crewId = 0)
    {
        ShowInfoGroup(data == null);
        if (data == null)
        {
            SetCrewInfo(crewId);
            return;
        }
        
        _data = data;

        UpdateBaseAttr();
        UpdateSecondAttr();

        _view.CharacterLb_UILabel.text = PersonalityType[data.personality];
    }

    private void UpdateBaseAttr()
    {
        CharacterPropertyDto propertyDto = new CharacterPropertyDto();
        _baseAttrList.ForEachI((item, idx) =>
        {
            var dto = _data.properties.Find(s => s.propId == GlobalAttr.PANTNER_BASE_ATTRS[idx]);
            if (dto != null)
                item.SetInfo(dto);
            else
            {
                propertyDto.propId = GlobalAttr.PANTNER_BASE_ATTRS[idx];
                propertyDto.propValue = 0;
                item.SetInfo(propertyDto);
            }
        });
    }

    private void UpdateSecondAttr()
    {
        CharacterPropertyDto propertyDto = new CharacterPropertyDto();
        _secondAttrList.ForEachI((item, idx) =>
        {
            var dto = _data.properties.Find(s => s.propId == GlobalAttr.SECOND_ATTRS[idx]);
            if (dto != null)
                item.SetInfo(dto);
            else
            {
                propertyDto.propId = GlobalAttr.SECOND_ATTRS[idx];
                propertyDto.propValue = 0;
                item.SetInfo(propertyDto);
            }
        });
    }
    
    protected override void AfterInitView()
    {
        _disposable = new CompositeDisposable();
        InitBaseAttrList();
        InitSecondAttrList();
        _view.SelfPartnerInfoGroup.gameObject.SetActive(false);
        for (int i = 0; i < _view.SkillGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.SkillGrid_UIGrid.transform.GetChild(i).gameObject;
            var controller = AddController<ItemCellController, ItemCell>(item);
            _skillList.Add(controller);
            var index = i;
            _disposable.Add(item.OnClickAsObservable().Subscribe(_ => { OnSkillIconClick(index); }));
        }
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(_view.DetailInfoBtn_UIButton.onClick, OnDetailInfoBtnOnClick);
        EventDelegate.Add(_view.pageSprite_UIButton.onClick, () => { OnChangePageClick(CrewBookTab.Info); });
        EventDelegate.Add(_view.pageSprite_1_UIButton.onClick, () => { OnChangePageClick(CrewBookTab.Quartz); });
        EventDelegate.Add(_view.BaseInfoBtn_UIButton.onClick, () => OnBaseInfoBtnClick());
        EventDelegate.Add(_view.SecondInfoBtn_UIButton.onClick, () => OnSecondInfoBtnClick());
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _baseAttrList.Clear();
        _secondAttrList.Clear();

        _data = null;
    }

    private void OnDetailInfoBtnOnClick()
    {
        var controller = UIModuleManager.Instance.OpenFunModule<DetailInfoController>(DetailInfoView.NAME, UILayerType.SubModule, true);
        controller.SetTitleInfo("伙伴属性", GetCrewGrow());
        List<CharacterAbility> characterAbility = DataCache.getArrayByCls<CharacterAbility>();
        List<CharacterAbility> list = new List<CharacterAbility>();
        characterAbility.ForEach(data =>
        {
            if(data.level == _detailInfoLevel)
                list.Add(data);
        });
        DictSort(list);

        controller.SetPosition(new Vector3(-252, -32, 0));
        controller.SetCrewDetailInfo(list, _data.properties);
    }

    private List<CharacterAbility> DictSort(List<CharacterAbility> dict)
    {
        if (_comparison == null)
        {
            _comparison = (a, b) =>
            {
                if (a.type == b.type)
                    return a.typeSort.CompareTo(b.typeSort);
                else
                    return a.type.CompareTo(b.type);
            };
        }
        dict.Sort(_comparison);
        return dict;
    }

    private CharacterPropertyDto GetCrewGrow()
    {
        var crewInfo = DataCache.getDtoByCls<GeneralCharactor>(_data.crewId);
        CharacterPropertyDto dto = new CharacterPropertyDto();
        dto.propId = GlobalAttr.Crew_Grow;

        //基础成长率*(1+INT(进阶次数/5)*10%)+研修成长率
        //策划要求/1000
        dto.propValue = ((crewInfo as Crew).growthRate * (float)(1 + Mathf.Floor((float)_data.phase / 5f) * 0.1f) + _data.extraGrow) / (float)1000;
        return dto;
    }
    
    private void InitBaseAttrList()
    {
        for (int i = 0; i < _view.BaseGrid_UIGrid.transform.childCount; i++)
        {
            var lb = _view.BaseGrid_UIGrid.transform.GetChild(i).gameObject;
            var com = AddController<AttrLabelController, AttrLabel>(lb);
            _baseAttrList.Add(com);
        }
    }

    private void InitSecondAttrList()
    {
        for (int i = 0; i < _view.SecondGrid_UIGrid.transform.childCount; i++)
        {
            var lb = _view.SecondGrid_UIGrid.transform.GetChild(i).gameObject;
            var com = AddController<AttrLabelController, AttrLabel>(lb);
            _secondAttrList.Add(com);
        }
    }

    //未获得的伙伴
    private void SetCrewInfo(int crewId)
    {
        if (crewId == 0)
            return;

        _crew = DataCache.getDtoByCls<GeneralCharactor>(crewId) as Crew;
        _skillList.ForEachI((item, idx) =>
        {
            if (idx >= _crew.crafts.Count)
                item.UpdateView();
            else
            {
                var skill = DataCache.getDtoByCls<Skill>(_crew.crafts[idx]) as Crafts;
                if(skill != null)
                    item.UpdateView(skill);
                else
                {
                    GameDebuger.Log("检查id: " + _crew.crafts[idx] + "是否存在于Crafts表中");
                }
            }
        });

        _view.AstralSprite_UISprite.spriteName = AstralSprite[_crew.property - 1];
    }

    private void OnSkillIconClick(int idx)
    {
        var skillId = _crew.crafts[idx];
        SkillTipsController tip = ProxyTips.OpenSkillTips(skillId);
        Vector3 tPos = new Vector3(-204 + 87*idx, 22, 0);
        tip.SetTipsPosition(tPos);
    }

    private void ShowInfoGroup(bool b)
    {
        _view.ShowPartnerInfoGroup.gameObject.SetActive(b);
        _view.SelfPartnerInfoGroup.gameObject.SetActive(!b);
        _view.pageFlagGrid_UIGrid.gameObject.SetActive(b);
    }

    private void OnChangePageClick(CrewBookTab page)
    {
        _view.PageGrid_PageScrollView.SkipToPage((int) page, true);
        _view.ShowTitleLb_UILabel.text = page == CrewBookTab.Info ? "属性偏向" : "导力器";
        _view.PropPoint.SetActive(page == CrewBookTab.Info);
        _view.CrewQuartz.SetActive(page == CrewBookTab.Quartz);
        UpdateInfoTabState(page);
    }

    private void UpdateInfoTabState(CrewBookTab tab)
    {
        View.InfoTabLb_UILabel.fontSize = tab == CrewBookTab.Info ? 22 : 20;
        View.QuartzTabLb_UILabel.fontSize = tab == CrewBookTab.Quartz? 22 : 20;
        View.InfoTabLb_UILabel.text = tab == CrewBookTab.Info
            ? "偏向".WrapColor(ColorConstantV3.Color_VerticalSelectColor_Str)
            : "偏向".WrapColor(ColorConstantV3.Color_VerticalUnSelectColor2_Str);
        View.QuartzTabLb_UILabel.text = tab == CrewBookTab.Quartz
            ? "导力器".WrapColor(ColorConstantV3.Color_VerticalSelectColor_Str)
            : "导力器".WrapColor(ColorConstantV3.Color_VerticalUnSelectColor2_Str);
        View.pageSprite_UIButton.sprite.depth = tab == CrewBookTab.Info ? 9 : 8;
        View.pageSprite_1_UIButton.sprite.depth = tab == CrewBookTab.Quartz ? 9 : 8;
        View.pageSprite_UIButton.sprite.spriteName = tab == CrewBookTab.Info ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_1_UIButton.sprite.spriteName = tab == CrewBookTab.Quartz ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_UIButton.normalSprite = tab == CrewBookTab.Info ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_1_UIButton.normalSprite = tab == CrewBookTab.Quartz ? "Tab_2_On" : "Tab_2_Off";
    }

    public void SetGoActive(bool b)
    {
        gameObject.SetActive(b);
    }

    private void OnBaseInfoBtnClick()
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
        var Crew = DataCache.getDtoByCls<GeneralCharactor>(_data.crewId) as Crew;
        ctrl.Init(PlayerPropertyTipType.BaseType, GlobalAttr.PANTNER_BASE_ATTRS, _data.crewId, _data.personality);
        ctrl.AddMagicCrewProps(_data.slotsElementLimit, Crew == null ? "" : Crew.typeIcon);
        ctrl.SetPostion(new Vector3(-229, -32, 0));
    }

    private void OnSecondInfoBtnClick()
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
        ctrl.Init(PlayerPropertyTipType.FightType, GlobalAttr.SECOND_ATTRS_TIPS);
        ctrl.SetPostion(new Vector3(-229, -32, 0));
    }
}
