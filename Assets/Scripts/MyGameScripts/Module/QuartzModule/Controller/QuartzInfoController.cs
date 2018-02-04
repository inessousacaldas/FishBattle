// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzInfoController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using MyGameScripts.Gameplay.Player;
using UnityEngine;

public interface ISelectOrbmentData
{
    int GetBagId { get; }
    int GetItemId { get; }
    QuartzDataMgr.TabEnum GetTab { get; }
}

public class SelectOrbmentData : ISelectOrbmentData
{
    private int _bagId;
    private int _itemId;
    private QuartzDataMgr.TabEnum _tab;

    public int GetBagId { get { return _bagId; } }
    public int GetItemId { get { return _itemId; } }
    public QuartzDataMgr.TabEnum GetTab { get { return _tab; } }

    public static SelectOrbmentData Create(int bagId, int itemId, QuartzDataMgr.TabEnum tab)
    {
        SelectOrbmentData data = new SelectOrbmentData();
        data._bagId = bagId;
        data._itemId = itemId;
        data._tab = tab;
        return data;
    }
}

public partial interface IQuartzInfoController
{
    UniRx.IObservable<SelectOrbmentData> GetChangeTabHandler { get; }
    UniRx.IObservable<int> GetCellClick { get; }
}

public partial class QuartzInfoController
{
    private List<ItemCellController> _magicList = new List<ItemCellController>();
    private List<AttrLabelController> _baseInfoList = new List<AttrLabelController>();
    private List<AttrLabelController> _warInfoList = new List<AttrLabelController>();
    private List<QuartzMagicSliderController> _sliderList = new List<QuartzMagicSliderController>();
    private List<ItemCellController> _wearList = new List<ItemCellController>();
    private List<ItemCellController> _allList = new List<ItemCellController>();
    private List<Magic> _magicInfoList = new List<Magic>();
    private List<Crew> _allCrewList = new List<Crew>();
    private List<UISprite> _linkList = new List<UISprite>();
    private Dictionary<int, int> _magicOpenDic = new Dictionary<int, int>();

    private ItemExpantViewController _itemExpantController;
    private delegate void _func(ItemCellController item, int idx);

    private const int WearMax = 8;    //装备魔法上限
    private const int AllMagicMax = 8;    //暂定所有导力魔法上限
    private const int SliderMax = 6;    //结晶回路链最多6个
    private const int BaseInfoMax = 4;    //基础属性数量
    private const int WearInfoMax = 12;    //战斗属性数量
    private const int _detailInfoLevel = 3;   //3代表详细属性界面显示内容

    public static Comparison<CharacterAbility> _comparison = null;
    private TabbtnManager tabMgr;
    private readonly ITabInfo[] TeamTabInfos =
    {
        TabInfoData.Create((int) TabEnum.Info, "详细属性"),
        TabInfoData.Create((int) TabEnum.Magic, "装备魔法"),
    };

    private TabbtnManager _itemTabMgr;
    private readonly ITabInfo[] ItemExpandTab =
    {
        TabInfoData.Create((int)ItemEnum.All, "全部")
    };

    private int _curPos = 0;
    private int _openOrbmentCell;   //当前可以开放的孔数量
    private int _openWearItem;      //当前开放可安装的魔法格子
    private bool _isOpen = true;

    private OrbmentInfoDto _orbmentDto;
    private CompositeDisposable _disposable;
    private IEnumerable<BagItemDto> _bagItemDtos;

    private Dictionary<int, string> _orbmentBGDict = new Dictionary<int, string>
    {
        { 1, "icon_bg_read" }, {2, "icon_bg_blue"}, {3, "icon_bg_orange"}, {4, "icon_bg_green"},
        { 5, "icon_bg_black"}, { 6, "icon_bg_yellow"}, { 7, "icon_bg_gray"}
    };

    private Dictionary<int, string> _orbmentLine = new Dictionary<int, string>
    {
        { 0, "ConnectLine_fg_yellow"}, {1,"ConnectLine_fg_blue"}, { 2, "ConnectLine_fg_green"}, { 3, "ConnectLine_fg_red"}
    };

    private enum TabEnum
    {
        Info = 0,
        Magic = 1
    }

    private enum ItemEnum
    {
        All = 0
    }

    #region Subscribe
    private Subject<SelectOrbmentData> _changeTab = new Subject<SelectOrbmentData>();
    public UniRx.IObservable<SelectOrbmentData> GetChangeTabHandler { get { return _changeTab; } }

    private Subject<int> _bagCellClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetCellClick { get { return _bagCellClickEvt; } }
    #endregion

    public void UpdateDataAndView(IQuartzInfoData data)
    {
        _orbmentDto = data.GetCurOrbmentInfoDto;
        if (_orbmentDto == null)
            return;

        var playerLv = _orbmentDto.grade;
        _magicOpenDic.ForEach(d =>
        {
            if (playerLv >= d.Key)
                _openWearItem = d.Value;
        });

        _wearList.ForEachI((item, idx) =>
        {
            if (idx < _openWearItem)
            {
                if (idx < _orbmentDto.magic.Count)
                {
                    var magic = _magicInfoList.Find(s => s.id == _orbmentDto.magic[idx]);
                    item.UpdateView(magic, false);
                }
                else
                    item.UpdateView();
            }
            else
                item.ItemLock();

            item.Mark_UISprite = true;
        });

        _allList.ForEachI((item, idx) =>
        {
            if (idx < _orbmentDto.ownMagic.Count)
            {
                item.gameObject.SetActive(true);
                var magic = _magicInfoList.Find(s => s.id == _orbmentDto.ownMagic[idx]);
                var b = _orbmentDto.magic.Find(s => s == magic.id);
                item.UpdateView(magic, b > 0);
            }
            else
            {
                item.UpdateView();
                if (idx >= AllMagicMax)
                    item.gameObject.SetActive(false);
            }
            item.Mark_UISprite = true;
        });

        _orbmentDto.orbment.openOrder.ForEachI((pos, idx) =>
        {
            if (idx < _openOrbmentCell)
                _magicList[pos - 1].SetOrbmentNullItem(false);
            else
                _magicList[pos - 1].SetOrbmentNullItem(true);
        });
        _orbmentDto.slotsDto.ForEachI((d, i) =>
        {
            _magicList[d.position - 1].SetOrbmentItem(d.bagItemDto);
        });
        _magicList.ForEachI((item, idx) =>
        {
            var limit = _orbmentDto.orbment.slotsElementLimit.Find(d => d.position == idx + 1);
            item.SetOrbmentLimit(limit == null ? "" : _orbmentBGDict[limit.elementId]);
        });

        _linkList.ForEach(con => con.gameObject.SetActive(false));
        _orbmentDto.orbment.links.ForEachI((str, idx) =>
        {
            str.Split(',').ForEach(n =>
            {
                int i;
                bool b = int.TryParse(n, out i);
                if (b)
                {
                    _linkList[i].spriteName = _orbmentLine[idx];
                    _linkList[i].gameObject.SetActive(true);
                }
            });
        });

        CharacterPropertyDto propertyDto = new CharacterPropertyDto();
        _baseInfoList.ForEachI((item, idx) =>
        {
            var id = GlobalAttr.PANTNER_BASE_ATTRS[idx];
            var dto = _orbmentDto.properties.Find(d => d.propId == id);
            if (dto != null)
                item.SetInfo(dto);
            else
            {
                propertyDto.propId = id;
                propertyDto.propValue = 0;
                item.SetInfo(propertyDto);
            }
        });

        _warInfoList.ForEachI((item, idx) =>
        {
            var id = GlobalAttr.SECOND_ATTRS[idx];
            var dto = _orbmentDto.properties.Find(d => d.propId == id);
            if (dto != null)
                item.SetInfo(dto);
            else
            {
                propertyDto.propId = id;
                propertyDto.propValue = 0;
                item.SetInfo(propertyDto);
            }

        });

        _sliderList.ForEachI((item, idx) =>
        {
            item.gameObject.SetActive(idx < _orbmentDto.quartzPropertyDtos.Count);

            if (idx < _orbmentDto.quartzPropertyDtos.Count)
                item.UpdateView(_orbmentDto.quartzPropertyDtos[idx], idx);
            else
                item.UpdateView();
        });

        SetSelectQuartz(data);
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _disposable = new CompositeDisposable();

        InitTab();
        InitItemTab();
        InitData();
        InitInfoList();
        InitEquipList();
        InitQuartzList();
        InitMagicInfoList();
        InitSliderInfoList();
        InitItemExpantView();
        InitLinkList();
        _bagItemDtos = BackpackDataMgr.DataMgr.GetQuartzItems();
        _itemExpantController.InitCellList(_bagItemDtos);
    }

    private void InitTab()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                _view.TabGrid_UIGrid.gameObject
                , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
                , "Tabbtn_" + i);

        tabMgr = TabbtnManager.Create(TeamTabInfos, func);

        _disposable.Add(tabMgr.Stream.Subscribe(pageIdx =>
        {
            ShowOrHideChildView(pageIdx == (int)TabEnum.Info);
        }));

        _view.TabGrid_UIGrid.Reposition();

        tabMgr.SetBtnLblFont(20, "2e2e2e", 18, "bdbdbd");
    }

    private void InitItemTab()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                _view.ItemExpandTab_UIGrid.gameObject
                , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
                , "Tabbtn_" + i);

        _itemTabMgr = TabbtnManager.Create(ItemExpandTab, func);

        _disposable.Add(_itemTabMgr.Stream.Subscribe(pageIdx =>
        {
        }));

        _view.ItemExpandTab_UIGrid.Reposition();

        _itemTabMgr.SetBtnLblFont(20, "000000", 18, "bdbdbd");
    }

    private void InitLinkList()
    {
        for (int i = 0; i < _view.LinkGrid_Transform.childCount; i++)
        {
            var line = _view.LinkGrid_Transform.GetChild(i).GetComponent<UISprite>();
            _linkList.Add(line);
        }
    }

    private void InitData()
    {
        var list = DataCache.getArrayByCls<Magic>();
        list.ForEach(data =>
        {
            if (data is Magic)
                _magicInfoList.Add(data);
        });
        _openOrbmentCell = DataCache.getDtoByCls<BracerGrade>(ModelManager.Player.GetBracerGrade).slotsCount;

        var content = DataCache.GetStaticConfigValues(AppStaticConfigs.MAGIC_CELL_OPEN_GRADE);
        _magicOpenDic = RoleSkillUtils.ParseAttr(content, ';', '|');

        var crewlist = DataCache.getArrayByCls<GeneralCharactor>();
        crewlist.ForEach(d =>
        {
            if (d is Crew)
                _allCrewList.Add(d as Crew);
        });
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _disposable.Add(HighInfoBtn_UIButtonEvt.Subscribe(_ => { OnHighInfoBtnClick(); }));
        _disposable.Add(ArrowBtn_UIButtonEvt.Subscribe(_ => OnArrowBtnClick()));
        _disposable.Add(OnBaseInfoBtn_UIButtonClick.Subscribe(_ => OnBaseInfoBtnClick()));
        _disposable.Add(OnSecondInfoBtn_UIButtonClick.Subscribe(_ => OnSecondInfoBtnClick()));
        _disposable.Add(_itemExpantController.GetItemClickHandler.Subscribe(idx =>
        {
            ShowItemTips(idx);
        }));

        _disposable.Add(_itemExpantController.GetClickHandler.Subscribe(_ =>
        {
            var data = SelectOrbmentData.Create(-1, -1, QuartzDataMgr.TabEnum.Forge);
            _changeTab.OnNext(data);
        }));
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        UICamera.onClick -= OnCameraClick;
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    private void InitItemExpantView()
    {
        _itemExpantController = AddController<ItemExpantViewController, ItemExpantView>(_view.ItemExpantView.gameObject);
    }

    //属性
    private void InitInfoList()
    {
        for (int i = 0; i < BaseInfoMax; i++)
        {
            var item = _view.BaseGrid_UIGrid.transform.GetChild(i);
            var baseInfo = AddController<AttrLabelController, AttrLabel>(item.gameObject);
            _baseInfoList.Add(baseInfo);
        }

        for (int i = 0; i < WearInfoMax; i++)
        {
            var item = _view.WarGrid_UIGrid.transform.GetChild(i);
            var warInfo = AddController<AttrLabelController, AttrLabel>(item.gameObject);
            _warInfoList.Add(warInfo);
        }
    }

    //结晶回路表盘
    private void InitQuartzList()
    {
        _func f = delegate (ItemCellController item, int idx)
        {
            item.OnCellClick.Subscribe(_ =>
            {
                var pos = _orbmentDto.orbment.openOrder.FindIndex(d => d == idx);
                if (pos >= _openOrbmentCell)
                {
                    TipManager.AddTip("该结晶回路孔尚未解锁");
                    return;
                }
                _curPos = idx;
                if (_orbmentDto.slotsDto.Count == 0 ||
                    _orbmentDto.slotsDto.Find(d => d.position == idx) == null)
                    ShowItemExpand(true);
                else
                {
                    var dto = _orbmentDto.slotsDto.Find(d => d.position == idx);
                    QuartzInfo(dto.bagItemDto, new Vector3(-29, 165, 0));
                    ShowItemExpand(false);
                }
                _magicList.ForEachI((com, i) =>
                {
                    com.isSelect = i == idx - 1;
                });
                _bagCellClickEvt.OnNext(idx);
            });
        };

        for (int i = 0; i < _view.QuartzGrid_Transform.childCount; i++)
        {
            var go = _view.QuartzGrid_Transform.GetChild(i);
            var controller = AddController<ItemCellController, ItemCell>(go.gameObject);
            _magicList.Add(controller);
            f(controller, i + 1);
        }
    }

    //装备中的导力魔法
    private void InitEquipList()
    {
        _func f = delegate (ItemCellController item, int idx)
        {
            item.OnCellClick.Subscribe(_ =>
            {
                if (idx >= _openWearItem)
                {
                    TipManager.AddTip("尚未开放该魔法技能孔");
                    return;
                }
                if (idx < _orbmentDto.magic.Count)
                {
                    _wearList.ForEachI((d, i) => { d.isSelect = i == idx; });
                    _allList.ForEach(d => { d.isSelect = false; });
                    EquipMagicInfo(new Vector3(-233, 162, 0), _orbmentDto.magic[idx]);

                }
            });
        };

        for (int i = 0; i < WearMax; i++)
        {
            var item = _view.EquipGrid_UIGrid.transform.GetChild(i);
            var con = AddController<ItemCellController, ItemCell>(item.gameObject);
            _wearList.Add(con);
            f(con, i);
        }
        _view.EquipGrid_UIGrid.Reposition();
    }

    //未装备的导力魔法
    private void InitMagicInfoList()
    {
        _func f = delegate (ItemCellController item, int idx)
        {
            item.OnCellClick.Subscribe(_ =>
            {
                if (idx < _orbmentDto.ownMagic.Count)
                {
                    _allList.ForEachI((d, i) => { d.isSelect = i == idx; });
                    _wearList.ForEach(d => { d.isSelect = false; });
                    EquipMagicInfo(new Vector3(-233, -54, 0), _orbmentDto.ownMagic[idx]);
                }
            });
        };

        for (int i = 0; i < AllMagicMax; i++)
        {
            var item = _view.AllGrid_UIGrid.transform.GetChild(i);
            var controller = AddController<ItemCellController, ItemCell>(item.gameObject);
            _allList.Add(controller);
            f(controller, i);
        }
        _view.AllGrid_UIGrid.Reposition();
    }

    //结晶回路链
    private void InitSliderInfoList()
    {
        for (int i = 0; i < SliderMax; i++)
        {
            var slider = AddChild<QuartzMagicSliderController, QuartzMagicSlider>(
                _view.MagicInfoGrid_UIGrid.gameObject, QuartzMagicSlider.NAME);
            _sliderList.Add(slider);
        }
        _view.MagicInfoGrid_UIGrid.Reposition();
    }

    private void ShowOrHideChildView(bool b)
    {
        _view.ItemInfoGroup.SetActive(b);
        _view.EquipGroup.SetActive(!b);
    }

    #region 高级属性
    private void OnHighInfoBtnClick()
    {
        var controller = UIModuleManager.Instance.OpenFunModule<DetailInfoController>(DetailInfoView.NAME, UILayerType.SubModule, true);
        if (_orbmentDto.ownId != ModelManager.Player.GetPlayerId())
            controller.SetTitleInfo("伙伴属性", GetCrewGrow());
        List<CharacterAbility> characterAbility = DataCache.getArrayByCls<CharacterAbility>();
        List<CharacterAbility> list = new List<CharacterAbility>();
        characterAbility.ForEach(data =>
        {
            if (data.level == _detailInfoLevel)
                list.Add(data);
        });
        DictSort(list);

        controller.SetPosition(new Vector3(-217, -32, 0));
        controller.SetCrewDetailInfo(list, _orbmentDto.properties);
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
        var crewInfo = DataCache.getDtoByCls<GeneralCharactor>(_orbmentDto.crewId);
        CharacterPropertyDto dto = new CharacterPropertyDto();
        dto.propId = GlobalAttr.Crew_Grow;
        dto.propValue = (crewInfo as Crew).growthRate * (float)((1 + _orbmentDto.phase / 5) * 0.1) + _orbmentDto.extraGrow;
        return dto;
    }
    #endregion

    private void OnArrowBtnClick()
    {
        _isOpen = !_isOpen;

        _view.ArrowBtn_UIButton.sprite.flip = _isOpen ?
            UIBasicSprite.Flip.Horizontally : UIBasicSprite.Flip.Vertically;

        _view.ScrollWidget.SetActive(_isOpen);
        _view.ScrollView_UIScrollView.gameObject.SetActive(_isOpen);
        _view.Texture_Transform.localPosition = _isOpen ? new Vector3(-241, 40, 0) : new Vector3(-241, -15, 0);
        _view.QuartzGrid_Transform.localPosition = _isOpen ? new Vector3(-167, 40, 0) : new Vector3(-167, -15, 0);
        _view.LinkGrid_Transform.localPosition = _isOpen ? Vector3.zero : new Vector3(0, -55, 0);
    }

    private void OnBaseInfoBtnClick()
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
        var crew = _allCrewList.Find(d => d.id == _orbmentDto.crewId);
        ctrl.Init(PlayerPropertyTipType.BaseType, GlobalAttr.PANTNER_BASE_ATTRS, _orbmentDto.crewId, crew == null ? -1 : crew.personality);
        if (_orbmentDto.crewId == -1)
            ctrl.AddMagicprops(ModelManager.Player.GetSlotsElementLimit, "", ModelManager.Player.FactionID);
        else
            ctrl.AddMagicCrewProps(_orbmentDto.quartzProprtyId, crew.typeIcon);
        ctrl.SetPostion(new Vector3(-229, -32, 0));
    }

    private void OnSecondInfoBtnClick()
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
        ctrl.Init(PlayerPropertyTipType.FightType, GlobalAttr.SECOND_ATTRS_TIPS);
        ctrl.SetPostion(new Vector3(-229, -32, 0));
    }

    private void EquipMagicInfo(Vector3 pos, int magicId)
    {
        var isWear = _orbmentDto.magic.FindIndex(d => d == magicId) >= 0;
        string btnName = isWear ? "卸下" : "装备";
        var ctrl = ProxyTips.OpenSkillTips(magicId, btnName, "", () =>
        {
            var playerId = ModelManager.Player.GetPlayerId();
            Magic magic = _magicInfoList.Find(s => s.id == magicId);
            if (isWear)
            {
                if (_orbmentDto.ownId < 0)
                    QuartzDataMgr.QuartzNetMsg.Magic_TakeOff(playerId, magic.skillMapId);
                else
                    QuartzDataMgr.QuartzNetMsg.Magic_TakeOff(_orbmentDto.ownId, magic.skillMapId);
            }
            else
            {
                if (_orbmentDto.magic.Find(d => d == magicId) != 0)
                {
                    TipManager.AddTip("该魔法已装备");
                    return;
                }
                if (_orbmentDto.ownId < 0)
                    QuartzDataMgr.QuartzNetMsg.Magic_Wear(playerId, 0, magic.id);
                else
                    QuartzDataMgr.QuartzNetMsg.Magic_Wear(_orbmentDto.ownId, 0, magic.id);
            }
        });
        ctrl.SetTipsPosition(pos);
    }

    private void QuartzInfo(BagItemDto itemDto, Vector3 pos)
    {
        string leftBtnName = "卸下";
        var playerId = ModelManager.Player.GetPlayerId();
        var dto = QuartzDataMgr.DataMgr.GetCurOrbmentInfoDto;
        var tipCtl = ProxyTips.OpenQuartzTips(itemDto, left: leftBtnName, leftClick: () =>
        {
            if (dto.ownId < 0)
                QuartzDataMgr.QuartzNetMsg.Quartz_TakeOff(playerId, _curPos);
            else
                QuartzDataMgr.QuartzNetMsg.Quartz_TakeOff(dto.ownId, _curPos);
        });
        tipCtl.SetTipsPosition(pos);
    }

    private void BagQuartzInfo(BagItemDto itemDto, Vector3 pos)
    {
        var leftBtnName = "强化";
        var rightBtnName = "装备";
        var playerId = ModelManager.Player.GetPlayerId();
        var dto = QuartzDataMgr.DataMgr.GetCurOrbmentInfoDto;

        var tipCtl = ProxyTips.OpenQuartzTips(itemDto, left: leftBtnName, right: rightBtnName,
            leftClick: () =>
            {
                var data = SelectOrbmentData.Create(itemDto.bagId, itemDto.itemId, QuartzDataMgr.TabEnum.Strength);
                _changeTab.OnNext(data);
            },
            rightClick: () => {
                if (dto.ownId < 0)
                    QuartzDataMgr.QuartzNetMsg.Quartz_Wear(playerId, itemDto.uniqueId, _curPos);
                else
                    QuartzDataMgr.QuartzNetMsg.Quartz_Wear(dto.ownId, itemDto.uniqueId, _curPos);
            });
        tipCtl.SetTipsPosition(pos);
    }

    private void ShowItemExpand(bool b)
    {
        _view.InfoGroup.SetActive(!b);
        _view.ItemExpandGroup.SetActive(b);
        if (b)
        {
            _bagItemDtos = BackpackDataMgr.DataMgr.GetQuartzItems();
            _itemExpantController.InitCellList(_bagItemDtos);
        }
    }

    //打开物品详细属性界面
    private void ShowItemTips(int idx)
    {
        if (_bagItemDtos == null || idx >= _bagItemDtos.Count())
            return;

        var itemData = _bagItemDtos.TryGetValue(idx);
        if (itemData != null)
            BagQuartzInfo(itemData, new Vector3(-223, 165, 0));
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != _view.ItemExpandGroup_UIPanel
            && _view.ItemExpandGroup.activeSelf
            && panel != _itemExpantController.GetItemPanel)
            ShowItemExpand(false);
    }

    private void SetSelectQuartz(IQuartzInfoData data)
    {
        _magicList.ForEachI((item, idx) =>
        {
            item.isSelect = idx == data.CurQuartzPos - 1;
        });
    }

    public void ChangeTab(QuartzDataMgr.TabEnum type)
    {
        switch (type)
        {
            case QuartzDataMgr.TabEnum.InfoMagic:
                ShowOrHideChildView(false);
                tabMgr.SetTabBtn(1);
                break;
        }
    }
}
