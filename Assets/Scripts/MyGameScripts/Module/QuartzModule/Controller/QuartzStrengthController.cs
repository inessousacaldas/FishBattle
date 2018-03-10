// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzStrengthController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;
using UnityEngine;

public partial interface IQuartzStrengthController
{
    UniRx.IObservable<Unit> OnWearingBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnBagBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnStrengthBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnBreakBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnTipBtn_UIButtonClick { get; }
    UniRx.IObservable<int> GetCellClick { get; }
    UniRx.IObservable<int> GetQuartzCellClick { get; }
    UniRx.IObservable<bool> GetChangeGroup { get; }
    UniRx.IObservable<SelectOrbmentData> GetSelectOrbmentHandler { get; }
    UniRx.IObservable<SelectOrbmentData> GetChangeTabHandler { get; }
}

public partial class QuartzStrengthController
{
    private CompositeDisposable _disposable;
    private OrbmentInfoDto _orbmentDto;
    private IQuartzStrengthData _data;
    private List<ItemCellController> _magicList = new List<ItemCellController>();
    private List<ItemCellController> _itemList = new List<ItemCellController>();    //背包中拥有的结晶回路
    private List<OrbmentInfoController> sevenInfoList = new List<OrbmentInfoController>();
    private List<UISprite> _linkList = new List<UISprite>();

    private const int ItemCellNum = 20; //默认加载20个item
    private int _seven = 7; //7种魔能属性
    private int _openOrbmentCell;   //当前可以开放的孔数量
    private int _needCash;
    private bool _enoughProps = false;

    //----------data-----------
    private List<QuartzStrengGrade> _quartzStrengList = new List<QuartzStrengGrade>();
    private List<QuartzElement> _quartzElements = new List<QuartzElement>(); 
    private int _propsId;   //消耗道具id
    private int _needCashNum;   //消耗货币
    private int _needPropsNum;  //消耗物品数量
    private bool _needBreak;    //需要强化
    private Props _props;
    private BagItemDto _curItemDto;
    private IEnumerable<BagItemDto> _allQuartzItems = new List<BagItemDto>();
    //-------------------------

    #region Subject
    private Subject<int> _bagCellClickEvt = new Subject<int>();
    private Subject<int> _quartzCellClickEvt = new Subject<int>();
    private Subject<bool> _changeGroupEvt = new Subject<bool>();
    private Subject<SelectOrbmentData> _selectOrbmentEvt = new Subject<SelectOrbmentData>();
    private Subject<SelectOrbmentData> _changeTab = new Subject<SelectOrbmentData>();

    public UniRx.IObservable<int> GetQuartzCellClick { get { return _quartzCellClickEvt; } }
    public UniRx.IObservable<int> GetCellClick { get { return _bagCellClickEvt; } }
    public UniRx.IObservable<bool> GetChangeGroup { get { return _changeGroupEvt; } }
    public UniRx.IObservable<SelectOrbmentData> GetSelectOrbmentHandler { get { return _selectOrbmentEvt; } }
    public UniRx.IObservable<SelectOrbmentData> GetChangeTabHandler { get { return _changeTab; } }
    #endregion

    private delegate void _func(ItemCellController item, int idx);

    private TabbtnManager _itemTabMgr;
    private readonly ITabInfo[] ItemTab =
    {
        TabInfoData.Create((int)ItemEnum.All, "全部")
    };

    private Dictionary<int, string> _orbmentBGDict = new Dictionary<int, string>
    {
        { 1, "icon_bg_read" }, {2, "icon_bg_blue"}, {3, "icon_bg_orange"}, {4, "icon_bg_green"},
        { 5, "icon_bg_black"}, { 6, "icon_bg_yellow"}, { 7, "icon_bg_gray"}
    };

    private Dictionary<int, string> _orbmentLine = new Dictionary<int, string>
    {
        { 0, "ConnectLine_fg_yellow"}, {1,"ConnectLine_fg_blue"}, { 2, "ConnectLine_fg_green"}, { 3, "ConnectLine_fg_red"}
    };

    private enum ItemEnum
    {
        All = 0
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();

        InitData();
        InitTab();
        InitLinkList();
        IsNullslot(true);

        _func f = (item, idx) =>
        {
            _disposable.Add(item.OnCellClick.Subscribe(_ =>
            {
                _quartzCellClickEvt.OnNext(idx + 1);
                var dto = _orbmentDto.slotsDto.Find(d => d.position == idx + 1);
                var index = _orbmentDto.orbment.openOrder.FindElementIdx(d => d == idx + 1);
                if (dto == null && index < _openOrbmentCell)    //选中没有添加结晶回路的格子之后跳转到属性界面,并且选中响应的格子
                {
                    var data = SelectOrbmentData.Create(-1, -1, QuartzDataMgr.TabEnum.Info);
                    _changeTab.OnNext(data);
                }
                UpdateInfo(idx + 1);
            }));
        };

        for (int i = 0; i < _view.QuartzGrid_Transform.childCount; i++)
        {
            var go = _view.QuartzGrid_Transform.GetChild(i);
            var con = AddController<ItemCellController, ItemCell>(go.gameObject);
            _magicList.Add(con);
            f(con, i);
        }

        for (int i = 0; i < _seven; i++)
        {
            var item = AddChild<OrbmentInfoController, OrbmentInfo>(_view.OrbmentGrid_UIGrid.gameObject,
                OrbmentInfo.NAME);
            sevenInfoList.Add(item);
        }

        UpdateItemCellList();
    }

    private void InitData()
    {
        _allQuartzItems = BackpackDataMgr.DataMgr.GetQuartzItems() == null? 
            new List<BagItemDto>() : BackpackDataMgr.DataMgr.GetQuartzItems();

        _quartzStrengList = DataCache.getArrayByCls<QuartzStrengGrade>();
        _quartzElements = DataCache.getArrayByCls<QuartzElement>();
        
        _needCashNum = DataCache.GetStaticConfigValue(AppStaticConfigs.QUARTZ_STRENG_CONSUME_SILVER);
        _openOrbmentCell = DataCache.getDtoByCls<BracerGrade>(ModelManager.Player.GetBracerGrade).slotsCount;
    }

    private void UpdateItemCellList()
    {
        _func fun = (item, idx) =>
        {
            _disposable.Add(item.OnCellClick.Subscribe(_ =>
            {
                if (idx < _allQuartzItems.Count())
                {
                    _itemList.ForEachI((d,i)=> { d.isSelect = i == idx; });
                    _bagCellClickEvt.OnNext(idx);
                    UpdateInfo(_allQuartzItems.TryGetValue(idx));
                    IsNullslot(_allQuartzItems.TryGetValue(idx) == null);
                }
            }));
        };

        //每次添加一排(4个)格子,而不是每次增加1个格子
        var max = _allQuartzItems.Count() > ItemCellNum ? Mathf.Ceil((float)_allQuartzItems.Count() / 4f) * 4  : ItemCellNum;
        for (int i = _view.ItemGrid_UIGrid.transform.childCount; i < max; i++)
        {
            var con = AddChild<ItemCellController, ItemCell>(_view.ItemGrid_UIGrid.gameObject, ItemCell.NAME);
            _itemList.Add(con);
            if (i < _allQuartzItems.Count())
                con.ExpantOrbmentItem(_allQuartzItems.TryGetValue(i));
            else
                con.UpdateView();
            fun(con, i);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(BackpackDataMgr.Stream.SubscribeAndFire(_ =>
        {
            var propsNum = BackpackDataMgr.DataMgr.GetItemCountByItemID(_propsId);
            _view.NeedPropNum_UILabel.text = string.Format("{0}/{1}", propsNum, _needPropsNum);
            _view.AddBtn_UIButton.gameObject.SetActive(_needPropsNum > propsNum);
            _view.PropIcon_UISprite.isGrey = propsNum < _needPropsNum;
            _enoughProps = propsNum >= _needPropsNum;
        }));
        EventDelegate.Add(_view.WearingBtn_UIButton.onClick, () =>
        {
            _changeGroupEvt.OnNext(false);
            ShowOrHideGroup(false);
        });
        EventDelegate.Add(_view.BagBtn_UIButton.onClick, () =>
        {
            _changeGroupEvt.OnNext(true);
            ShowOrHideGroup(true);
            UpdateItemList();
        });
        EventDelegate.Add(_view.StrengthBtn_UIButton.onClick, OnStrengthBtnClick);
        EventDelegate.Add(_view.BreakBtn_UIButton.onClick, OnBreakBtnClick);
        EventDelegate.Add(_view.TipBtn_UIButton.onClick, OnTipBtnClick);
        EventDelegate.Add(_view.AddBtn_UIButton.onClick, OnAddBtnClick);
        EventDelegate.Add(_view.PropIcon_UIButton.onClick, OnPropIconClick);

        _disposable.Add(PlayerModel.Stream.SubscribeAndFire(UpdatePlayerInfo));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void InitTab()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                _view.TabGrid_UIGrid.gameObject
                , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
                , "Tabbtn_" + i);

        _itemTabMgr = TabbtnManager.Create(ItemTab, func);

        _disposable.Add(_itemTabMgr.Stream.Subscribe(pageIdx =>
        {
        }));

        _view.TabGrid_UIGrid.Reposition();

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

    private void ShowOrHideGroup(bool b)
    {
        _view.ItemExpandGroup.SetActive(b);
        _view.QuartzGroup.SetActive(!b);
    }

    private void ShowOrHideBtn(bool b, bool hide = false)
    {
        if (!hide)
        {
            _view.BreakBtn_UIButton.gameObject.SetActive(hide);
            _view.StrengthBtn_UIButton.gameObject.SetActive(hide);
            return;
        }
        _view.BreakBtn_UIButton.gameObject.SetActive(!b);
        _view.StrengthBtn_UIButton.gameObject.SetActive(b);
    }

    private void OnStrengthBtnClick()
    {
        if (_curItemDto == null)
        {
            TipManager.AddTip("选中一个结晶回路进行强化操作");
            return;
        }

        if (!_enoughProps)
        {
            ProxyTips.OpenTipsWithGeneralItem(_props, new Vector3(-209, 38, 0));
            ShowGainWay();
            TipManager.AddTip(string.Format("{0}不足", _props.name));
            return;
        }

        var cash = _needCashNum * GetpropNum((_curItemDto.extra as QuartzExtraDto).strengGrade + 1, _curItemDto.item.quality);
        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, cash, () =>
        {
            if (_orbmentDto.ownId < 0)
            {
                var playerId = _data.IsBagGroup ? 0 : ModelManager.Player.GetPlayerId();
                QuartzDataMgr.QuartzNetMsg.Quartz_Streng(playerId, _curItemDto.uniqueId);
            }
            else
                QuartzDataMgr.QuartzNetMsg.Quartz_Streng(_data.IsBagGroup ? 0 : _orbmentDto.ownId, _curItemDto.uniqueId);
        });
    }

    private void OnBreakBtnClick()
    {
        if (_curItemDto == null)
        {
            TipManager.AddTip("选中一个结晶回路进行突破操作");
            return;
        }

        if (!_enoughProps)
        {
            ProxyTips.OpenTipsWithGeneralItem(_props, new Vector3(-209, 38, 0));
            ShowGainWay();
            TipManager.AddTip(string.Format("{0}不足", _props.name));
            return;
        }

        var data = DataCache.getDtoByCls<QuartzQuality>(_curItemDto.item.quality);
        var cash = data.breakSilver;
        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, cash, () =>
        {
            if (_orbmentDto.ownId < 0)
            {
                var playerId = _data.IsBagGroup ? 0 : ModelManager.Player.GetPlayerId();
                QuartzDataMgr.QuartzNetMsg.Quartz_Break(playerId, _curItemDto.uniqueId);
            }
            else
                QuartzDataMgr.QuartzNetMsg.Quartz_Break(_data.IsBagGroup ? 0 : _orbmentDto.ownId, _curItemDto.uniqueId);
        });
    }

    private void OnTipBtnClick()
    {
        ProxyTips.OpenTextTips(14, new Vector3(300, -76, 0));
    }

    private void OnAddBtnClick()
    {
        ProxyTips.OpenTipsWithGeneralItem(_props, new Vector3(-209, 38, 0));
        ShowGainWay();
    }

    private void OnPropIconClick()
    {
        ProxyTips.OpenTipsWithGeneralItem(_props, new Vector3(-209, 38, 0));
        ShowGainWay();
    }

    private void ShowGainWay()
    {
        GainWayTipsViewController.OpenGainWayTip(_props.id, new Vector3(209, 38, 0), ProxyQuartz.CloseQuartzMainView);
    }

    //选中的结晶回路孔是否为空
    private void IsNullslot(bool b)
    {
        _view.TipLb.SetActive(b);
        _view.ItemInfoGroup.SetActive(!b);
    }
    
    public void UpdateDataAndView(IQuartzStrengthData data)
    {
        _data = data;
        _orbmentDto = data.GetCurOrbmentInfoDto;
        _allQuartzItems = BackpackDataMgr.DataMgr.GetQuartzItems() == null ?
            new List<BagItemDto>() : BackpackDataMgr.DataMgr.GetQuartzItems();

        SetCurItemInfo();
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

        //在从物品tip进入强化界面
        if (data.SelectOrbment != null && data.SelectOrbment.GetItemId != -1)
        {
            ShowOrHideGroup(true);
            var dto = _allQuartzItems.Find(d=>d.bagId == data.SelectOrbment.GetBagId 
                        && d.itemId == data.SelectOrbment.GetItemId);
            UpdateInfo(dto);
        }
        else
        {
            ShowOrHideGroup(data.IsBagGroup);
            if (data.CurQuartzPos >= 0)
                UpdateInfo(data.CurQuartzPos);      //导力器表盘
            else
                UpdateInfo(_allQuartzItems.TryGetValue(data.CurBagPos));    //背包
        }
 
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

        UpdateItemCellList();
        UpdateItemList();
        _selectOrbmentEvt.OnNext(null); //如果是从物品tips强化按钮跳转到当前界面的话会有一个缓存数据,打开界面后清空该缓存数据
    }

    private void SetCurItemInfo()
    {
        if (_data.IsBagGroup)
            _curItemDto = _allQuartzItems.Count() > 0 ? _allQuartzItems.TryGetValue(0) : null;
        else
            _curItemDto = _orbmentDto.slotsDto.Count > 0 ? _orbmentDto.slotsDto[0].bagItemDto : null;
    }

    private void UpdateInfo(int pos)
    {
        _magicList.ForEachI((com, i) =>
        {
            com.isSelect = i == pos - 1;
        });

        var dto = _orbmentDto.slotsDto.Find(d => d.position == pos);
        IsNullslot(dto == null);
        if (dto == null)
            return;
        
        UpdateInfo(dto.bagItemDto);
    }

    private void UpdateInfo(BagItemDto itemDto)
    {
        IsNullslot(itemDto == null);
        if (itemDto == null) return;

        var extraDto = itemDto.extra as QuartzExtraDto;
        if (extraDto == null)
        {
            TipManager.AddTip("========数据有误======");
            return;
        }

        _curItemDto = itemDto;
        SetOrbmentIcon(itemDto);
        _view.NameLb_UILabel.text = QuartzHelper.GetItemName(itemDto);
        _view.FightNumLb_UILabel.text = string.Format("战斗力:{0}", 0);

        UpdateDescLb(extraDto);
        UpdateProps(itemDto, extraDto);
        UpdateStarState(itemDto);
        UpdateSevenInfoList(extraDto);

        _view.ScollerView_UIScrollView.ResetPosition();
        _view.Table_UITable.Reposition();

       _itemList.ForEachI((item, idx) =>
       {
           var data = item.GetData();
           item.isSelect = data == null ? false : data.index == itemDto.index;
       });
    }

    private void SetOrbmentIcon(BagItemDto itemDto)
    {
        QuartzExtraDto extraDto = itemDto.extra as QuartzExtraDto;
        int quartzId = extraDto.baseProperties.Count > 0 ? extraDto.baseProperties[0].propId : extraDto.passiveSkill;
        var QuartzBase = DataCache.getDtoByCls<QuartzBaseProperty>(quartzId);
        var quartz = DataCache.getDtoByCls<GeneralItem>(itemDto.itemId) as Quartz;
        _view.Icon_UISprite.spriteName = QuartzBase == null ? "" : QuartzBase.icon;
        _view.Magic_UISprite.spriteName = quartz == null ? "" : quartz.icon;
        _view.IconBG_UISprite.spriteName = quartz == null ? "item_ib_1" : string.Format("item_ib_{0}", quartz.quality);
    }

    private void UpdateSevenInfoList(QuartzExtraDto extraDto)
    {
        sevenInfoList.ForEachI((item, idx) =>
        {
            var d = extraDto.quartzProperties.Find(s => s.elementId == idx + 1);
            item.gameObject.SetActive(d != null);
            if(d != null)
                item.SetInfo(d);
        });
        _view.OrbmentGrid_UIGrid.Reposition();
    }

    private void UpdateStarState(BagItemDto itemDto)
    {
        for (int i = 0; i < _view.StarGrid_Transform.childCount; i++)
        {
            var star = _view.StarGrid_Transform.GetChild(i);
            star.gameObject.SetActive(i < itemDto.item.quality);
        }
    }

    private void UpdateProps(BagItemDto itemDto, QuartzExtraDto extraDto)
    {
        _needBreak = IsNeedBreak(extraDto);
        if (_needBreak)
        {
            var data = DataCache.getDtoByCls<QuartzQuality>(itemDto.item.quality);
            _propsId = _quartzElements[extraDto.elementId - 1].itemId;
            _props = DataCache.getDtoByCls<GeneralItem>(_propsId) as Props;
            _needCash = data.breakSilver;
        }
        else
        {
            _propsId = DataCache.GetStaticConfigValue(AppStaticConfigs.QUARTZ_STRENG_ITEM_ID);
            _props = DataCache.getDtoByCls<GeneralItem>(_propsId) as Props;
            _needCash = _needCashNum * GetpropNum(extraDto.strengGrade + 1, itemDto.item.quality);
        }

        if(_props != null)
            UIHelper.SetItemIcon(_view.PropIcon_UISprite, _props.icon);

        if (IsMaxGrade(extraDto))
        {
            _view.NeedCaseLb_UILabel.text = "-";
            _view.NeedPropNum_UILabel.text = "- / -";
            _view.PropIcon_UISprite.isGrey = false;
            ShowOrHideBtn(false);
        }
        else
        {
            var enough = (ModelManager.Player.GetPlayerWealthSilver() - _needCash) > 0;
            _view.NeedCaseLb_UILabel.text = enough ?
                _needCash.ToString().WrapColor(ColorConstantV3.Color_White):
                _needCash.ToString().WrapColor(ColorConstantV3.Color_Red_Str);

            var propsNum = BackpackDataMgr.DataMgr.GetItemCountByItemID(_propsId);
            var needNum = GetpropNum(extraDto.strengGrade + 1, itemDto.item.quality);
            _enoughProps = propsNum >= needNum;
            _needPropsNum = needNum;
            _view.NeedPropNum_UILabel.text = string.Format("{0}/{1}", propsNum, needNum);

            _view.AddBtn_UIButton.gameObject.SetActive(GetpropNum(extraDto.strengGrade + 1, itemDto.item.quality) > propsNum);
            ShowOrHideBtn(!IsNeedBreak(extraDto), true);
            _view.PropIcon_UISprite.isGrey = propsNum < GetpropNum(extraDto.strengGrade + 1, itemDto.item.quality);
        }

        _view.ScollerView_UIScrollView.ResetPosition();

       
    }

    private void UpdateDescLb(QuartzExtraDto extraDto)
    {
        StringBuilder str = new StringBuilder();
        extraDto.baseProperties.ForEachI((d, idx)=>
        {
            var n = DataCache.getDtoByCls<CharacterAbility>(d.propId);
            string txt = "";
            if (n.per)
                txt = string.Format("{0}:{1}", GlobalAttr.GetAttrName(d.propId), string.Format("{0}%", d.propValue * 100));
            else
                txt = string.Format("{0}:{1}", GlobalAttr.GetAttrName(d.propId), d.propValue);

            if (idx == extraDto.baseProperties.Count - 1)
                str.Append(txt);
            else
                str.AppendLine(txt);
        });
        _view.BaseDescLb_UILabel.text = str.ToString();

        StringBuilder secondStr = new StringBuilder();
        extraDto.secondProperties.ForEach(d =>
        {
            var n = DataCache.getDtoByCls<CharacterAbility>(d.propId);
            string txt;
            if (n.per)
                txt = string.Format("{0}:{1}", GlobalAttr.GetAttrName(d.propId), string.Format("{0}%", d.propValue * 100));
            else
                txt = string.Format("{0}:{1}", GlobalAttr.GetAttrName(d.propId), d.propValue);
            secondStr.AppendLine(txt);
        });
        _view.StrengthDescLb_UILabel.text = secondStr.ToString();
    }

    private void UpdateItemList()
    {
        _itemList.ForEachI((item, idx) =>
        {
            var dto = _allQuartzItems.TryGetValue(idx);
            if (dto != null)
                item.ExpantOrbmentItem(dto);
            else
                item.UpdateView();
        });

        _view.ItemGrid_UIGrid.Reposition();
        _view.ScollerView_UIScrollView.ResetPosition();
    }

    private int GetpropNum(int lv, int star)
    {
        if (_needBreak)
        {
            var data = DataCache.getDtoByCls<QuartzQuality>(star);
            return data == null ? 0 : data.breakCount;
        }

        if (lv >= _quartzStrengList.Count)
            return _quartzStrengList[_quartzStrengList.Count - 1].material[star - 1].count;

        return _quartzStrengList[lv].material[star - 1].count;
    }

    private bool IsNeedBreak(QuartzExtraDto dto)
    {
        if (dto.strengGrade < _quartzStrengList.Count)
        {
            var breakGradeLimit = _quartzStrengList[dto.strengGrade].breakGradeLimit;
            return breakGradeLimit == null ? false : dto.breakGrade < breakGradeLimit;
        }
        else
            return false;
    }

    private bool IsMaxGrade(QuartzExtraDto dto)
    {
        return dto.strengGrade == _quartzStrengList.Count - 1 &&
               dto.breakGrade == _quartzStrengList[_quartzStrengList.Count - 1].breakGradeLimit;
    }

    private void UpdatePlayerInfo(IPlayerModel model)
    {
        bool enough = model.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER) > _needCash;
        _view.NeedCaseLb_UILabel.text = enough ? 
            _needCash.ToString().WrapColor(ColorConstantV3.Color_White) :
            _needCash.ToString().WrapColor(ColorConstantV3.Color_Red_Str);
    }
}
