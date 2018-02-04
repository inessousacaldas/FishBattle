// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ItemCellController.cs
// Author   : willson
// Created  : 2015/1/12 
// Porpuse  : 
// **********************************************************************
using System;
using AppDto;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

public partial class ItemCell
{
    public const string Prefab_BagItemCell = "BagItemCell";
    public const string Prefab_ItemCell = "ItemCell";
    public const string Prefab_ItemUseCell = "ItemUseCell";
}

public enum ItemCellTipsBtnType
{
    Null = 0, 
    BackpackType = 1, //背包
    Ware = 2, //仓库
    Temppack = 3, //临时背包
}

public partial class ItemCellController : MonolessViewController<ItemCell>, IItemCellController
{

    private bool _alwaysDisplayCount;
    private bool _canDisplayCount;

    #region tipsData
    private BagItemDto _dto;
    private EquipmentDto _equipmentDto;
    //_generalItem必须初始化 否则无法显示tips
    private GeneralItem _generalItem;
    private int _itemType;
    private IBaseTipsController _tipsCtrl;
    public IBaseTipsController GetTips { get { return _tipsCtrl; } }
    private Vector3 _tipsShowPos = new Vector3(0.0f, 0.0f);
    private bool _isLeft;
    private bool _isShowTipsOnClick = true; //是否显示Tips
    private ItemCellTipsBtnType _tipsType; //用于显示Tips按钮
    private Action<ItemCellController> _onClickCallBack;
    #endregion


    #region Event
    private Subject<Unit> cellClickEvent;
    public UniRx.IObservable<Unit> OnCellClick
    {
        get { return cellClickEvent; }
    }

    private Subject<Unit> cellDoubleClickEvent;
    public UniRx.IObservable<Unit> OnCellDoubleClick
    {
        get { return cellDoubleClickEvent; }
    }
    #endregion
    private int _packId;
    private int _itemCount;
    public int ItemCount
    {
        get { return _itemCount; }
        set
        {
            _itemCount = value;
            if (_canDisplayCount)
            {
                if (mHideWhenCountZero)
                    View.CountLabel_UILabel.text = value <= 1 ? "" : string.Format("[b]{0}[-]", _itemCount);
                else
                    View.CountLabel_UILabel.text = string.Format("[b]{0}[-]", _itemCount);
            }
        }
    }

    public void SetCountTxt(int cnt, string formatStr)
    {
        _itemCount = cnt;
        if (_view.CountLabel_UILabel != null)
            View.CountLabel_UILabel.text = string.Format(formatStr, cnt);
    }

    public void SetNameLabel(string name)
    {
        var nameLabel = this.gameObject.FindScript<UILabel>("nameLabel");
        if (nameLabel != null)
            nameLabel.text = name;
    }
    public void SetBorderSprite(string spriteName, string atlasName = "")
    {
        View.BorderSprite_UISprite.spriteName = spriteName;
        if (string.IsNullOrEmpty(atlasName))
            return;

        GameObject atlasPrefab =
                        AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
        if (atlasPrefab != null)
        {
            var atlas = atlasPrefab.GetComponent<UIAtlas>();
            View.BorderSprite_UISprite.atlas = atlas;
        }
    }

    public void SetSelectSprite(string spriteName, string atlasName = "")
    {
        View.SelectSprite_UISprite.spriteName = spriteName;
        if (string.IsNullOrEmpty(atlasName))
            return;

        GameObject atlasPrefab =
                        AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
        if (atlasPrefab != null)
        {
            var atlas = atlasPrefab.GetComponent<UIAtlas>();
            View.SelectSprite_UISprite.atlas = atlas;
        }
    }

    public void SetNameLabel(string itemName, string colStr)
    {
        var nameStr = itemName.WrapColor(colStr);
        SetNameLabel(nameStr);
    }
    public UIScrollView ContainerScrollView
    {
        set { View.ItemCell_UIDragScrollView.scrollView = value; }
    }

    //设置物品数量，且无条件显示数量
    public void SetItemAcountNoLimit(int count)
    {
        _itemCount = count;
        View.CountLabel_UILabel.text = string.Format("[b]{0}[-]", _itemCount);
    }

    private bool mHideWhenCountZero = true;

    public bool HideWhenCountZero
    {
        get
        {
            return mHideWhenCountZero;
        }
        set
        {
            if (mHideWhenCountZero != value)
            {
                mHideWhenCountZero = value;
                if (ItemCount <= 0)
                {
                    View.CountLabel_UILabel.text = value ? string.Empty : "[b]0[-]";
                }
            }
        }
    }

    private UnityEngine.Color mItemCountLabelColor;

    public UnityEngine.Color ItemCountLabelColor
    {
        get
        {
            return mItemCountLabelColor;
        }
        set
        {
            if (null != _view && View.CountLabel_UILabel.color != value)
            {
                mItemCountLabelColor = value;
                View.CountLabel_UILabel.color = mItemCountLabelColor;
            }
        }
    }

    public bool BingBing { set { _view.BingingSpr_UISprite.gameObject.SetActive(value);} }

    public UISprite Bg
    {
        get { return View.ItemCell_UISprite; }
    }
    public UISprite Border
    {
        get
        {
            return View.BorderSprite_UISprite;
        }
    }
    public bool isSelect
    {
        get
        {
            if (View != null)
                return View.SelectSprite_UISprite.enabled;
            return false;
        }
        set
        {
            if (View != null)
            {
                View.SelectSprite_UISprite.enabled = value;
                View.SelectSprite_UISprite.gameObject.SetActive(true);
            }
        }
    }

    public bool Mark_UISprite
    {
        get { return _view.MarkCSPK_UISprite; }
        set { _view.MarkCSPK_UISprite.gameObject.SetActive(value);}
    }

    #region Update the color of the border sprite.


    #endregion

    public bool enabledIconSprite
    {
        get { return View.IconSprite_UISprite.enabled; }
        set
        {
            if (View.IconSprite_UISprite.enabled != value)
            {
                View.IconSprite_UISprite.enabled = value;
            }
        }
    }

    protected override void AfterInitView()
    {
        GameDebuger.TODO(@"_packId = AppItem.PackEnum_Unknown;");
        _canDisplayCount = true;
        _alwaysDisplayCount = false;



        UpdateCSPKMark();

        GameDebuger.TODO(@"ItemHelper.UpdateEquipItemBorderColor(_view.BorderSprite_UISprite, null);");
        if (View.SelectSprite_UISprite.enabled)
            View.SelectSprite_UISprite.enabled = false;
        View.CountLabel_UILabel.text = "";
        View.IconSprite_UISprite.enabled = false;
        View.redSpr.SetActive(false);
        View.LockSprite_UISprite.enabled = false;
        View.WearSprite_UISprite.enabled = false;
        //if(View.TempNameLbl_UILabel != null)
        //    View.TempNameLbl_UILabel.text = "";

        cellClickEvent = this.gameObject.OnClickAsObservable();
        cellDoubleClickEvent = this.gameObject.OnDoubleClickAsObservable();

        //Tips默认位置
        //SetDefaultTipsPosition();
    }

    //双击关闭tips
    private void CloseTips()
    {
        if (_tipsCtrl != null)
            _tipsCtrl.Close();
    }

    protected override void RegistCustomEvent()
    {
        cellClickEvent.Subscribe(_ => { OnClickItemCell(); });
        //EventDelegate.Set(View.ItemCell_UIEventTrigger.onClick, OnClickItemCell);
    }
    protected override void RemoveCustomEvent()
    {
        //EventDelegate.Remove(View.ItemCell_UIEventTrigger.onClick, OnClickItemCell);
    }
    public int GetPackId()
    {
        return _packId;
    }

    public void SetPackId(int packId)
    {
        _packId = packId;
    }

    public BagItemDto GetData()
    {
        return _dto;
    }

    public void AlwaysDisplayCount(bool b)
    {
        _alwaysDisplayCount = b;
    }

    public void CanDisplayCount(bool b)
    {
        _canDisplayCount = b;
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }

    public void UpdateView()
    {
        View.IconSprite_UISprite.enabled = false;
        View.LockSprite_UISprite.enabled = false;
        View.WearSprite_UISprite.enabled = false;
        View.WearSprite_UISprite.gameObject.SetActive(false);
        View.redSpr.gameObject.SetActive(false);
        View.SelectSprite_UISprite.gameObject.SetActive(false);
        View.CountLabel_UILabel.text = "";
        View.BorderSprite_UISprite.spriteName = "item_ib_bg";
        View.MarkCSPK_UISprite.gameObject.SetActive(false);
        if(View.IconTex_UITexture != null)
            View.IconTex_UITexture.gameObject.SetActive(false);
        if(View.BingingSpr_UISprite != null)
            View.BingingSpr_UISprite.gameObject.SetActive(false);
    }

    public void UpdateView(BagItemDto dto)
    {
        if (dto == null) return;
        _dto = dto;
        _generalItem = dto.item;
        _itemType = _dto.item.itemType;

        UpdateView(dto, false);
    }

    #region 主界面获得奖励物品Tips
    public void UpdateView(ItemDto dto, Transform trans = null, int idx = 0,bool show = false)
    {
        UpdateView();
        _isShowTipsOnClick = true;
        //View.CountLabel_UILabel.text = dto.count.ToString();
        var item = dto.item;
        _generalItem = dto.item;

        if (trans != null)
        {
            _tipsShowPos = new Vector3(trans.localPosition.x + idx * 88, trans.localPosition.y + 73, 0);
        }
        if (item is AppItem)
        {
            var val = item as AppItem;
            UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.item.icon);
            SetQuality(val.quality);
            _itemType = val.itemType;
        }
        else if (item is AppVirtualItem)
        {
            var val = item as AppVirtualItem;
            UIHelper.SetAppVirtualItemIcon(View.IconSprite_UISprite, (AppVirtualItem.VirtualItemEnum)val.id, true);
            SetQuality(val.quality);
            _itemType = val.itemType;
        }
        View.BorderSprite_UISprite.enabled = true;
        View.IconSprite_UISprite.enabled = true;
        if(show)
            View.CountLabel_UILabel.text = dto.count.ToString();
    }
    #endregion
    public void UpdateMissonItemView(ItemDto dto) {
        //UpdateView();
        if(_view.BingingSpr_UISprite != null)
            _view.BingingSpr_UISprite.gameObject.SetActive(false);
        _itemType = (int)AppItem.ItemTypeEnum.MissionItem;
        _generalItem = dto.item;
        View.CountLabel_UILabel.text = dto.count.ToString();
        //UIHelper.SetItemIcon(View.IconSprite_UISprite,dto.item.icon);
        SetUsePopsData(dto.item.icon, dto.item.name);
        if(dto.item as AppMissionItem != null)
        {
            View.BorderSprite_UISprite.enabled = true;
            SetQuality((dto.item as AppMissionItem).quality);
        }
        View.MarkCSPK_UISprite.gameObject.SetActive(false);
    }

    #region 人物技能专精道具图标
    public void UpdateSkillSpecialView(GeneralItem item,int count)
    {
        UpdateView();
        _isShowTipsOnClick = true;
        _generalItem = item;
        UIHelper.SetItemIcon(View.IconSprite_UISprite, item.icon);
        if (item is AppItem)
        {
            var val = item as AppItem;
            SetQuality(val.quality);
            _itemType = val.itemType;
        }
        View.IconSprite_UISprite.enabled = true;
        View.BorderSprite_UISprite.enabled = true;
        if (count == 0)
        {
            View.LockSprite_UISprite.enabled = true;
            View.IconSprite_UISprite.isGrey = true;
        }
        else
        {
            View.LockSprite_UISprite.enabled = false;
            View.IconSprite_UISprite.isGrey = false;
            View.CountLabel_UILabel.text = count.ToString();
        }
    }
    #endregion

    #region 导力器

    public void SetOrbmentNullItem(bool isLock)
    {
        _view.LockSprite_UISprite.spriteName = isLock ? "Ect_Lock" : "Ect_Add3";
        _view.BorderSprite_UISprite.spriteName = "bg_ringicon";
        _view.BorderSprite_UISprite.gameObject.SetActive(true);
        _view.IconSprite_UISprite.gameObject.SetActive(false);
        _view.MarkCSPK_UISprite.gameObject.SetActive(false);
        _view.LockSprite_UISprite.enabled = true;
    }

    public void SetOrbmentItem(BagItemDto dto)
    {
        _dto = dto;
        _generalItem = dto.item;
        View.IconSprite_UISprite.enabled = true;
        _view.LockSprite_UISprite.enabled = false;
        View.IconSprite_UISprite.gameObject.SetActive(true);
        _view.MarkCSPK_UISprite.gameObject.SetActive(true);

        QuartzExtraDto extraDto = dto.extra as QuartzExtraDto;
        int quartzId = extraDto.baseProperties.Count > 0 ? extraDto.baseProperties[0].propId : extraDto.passiveSkill;
        var QuartzBase = DataCache.getDtoByCls<QuartzBaseProperty>(quartzId);
        var quartz = DataCache.getDtoByCls<GeneralItem>(dto.itemId) as Quartz;
        UIHelper.SetQuartyIcon(_view.IconSprite_UISprite, QuartzBase == null ? "" : QuartzBase.icon);
        UIHelper.SetQuartyIcon(_view.MarkCSPK_UISprite, quartz == null ? "" : quartz.icon);

        _view.IconSprite_UISprite.MakePixelPerfect();
    }

    //设置结晶回路颜色限制
    public void SetOrbmentLimit(string str)
    {
        if (string.IsNullOrEmpty(str))
            _view.BorderSprite_UISprite.spriteName = "bg_ringicon";
        else
            _view.BorderSprite_UISprite.spriteName = str;
    }

    public void UpdateView(Magic magic, bool wearing = false)
    {
        View.IconTex_UITexture.gameObject.SetActive(false);
        UIHelper.SetUITexture(_view.IconTex_UITexture, magic.icon, false);
        View.CountLabel_UILabel.text = magic.name;
        View.WearSprite_UISprite.enabled = wearing;
        View.WearSprite_UISprite.gameObject.SetActive(wearing);
        View.IconTex_UITexture.gameObject.SetActive(true);
        View.MarkCSPK_UISprite.gameObject.SetActive(true);
    }

    public void ExpantOrbmentItem(BagItemDto dto)
    {
        SetOrbmentItem(dto);

        var quartz = DataCache.getDtoByCls<GeneralItem>(dto.itemId) as Quartz;
        _view.BorderSprite_UISprite.spriteName = quartz == null ? "item_ib_1" : string.Format("item_ib_{0}", quartz.quality);
    }

    public void QuartzForgeItem(ItemDto dto, int hadCount)
    {
        string color = dto.count > hadCount ? ColorConstantV3.Color_PaleRed_Str : ColorConstantV3.Color_White_Str;
        View.CountLabel_UILabel.text = string.Format("{0}/{1}", hadCount, dto.count).WrapColor(color);
        UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.item.icon);
        _view.LockSprite_UISprite.spriteName = "Ect_Add3";
        _view.LockSprite_UISprite.gameObject.SetActive(dto.count > hadCount);
        _view.LockSprite_UISprite.enabled = true;
        _view.IconSprite_UISprite.isGrey = dto.count > hadCount;
        _view.IconSprite_UISprite.enabled = true;
    }

    public void ItemLock()
    {
        UpdateView();
        _view.LockSprite_UISprite.gameObject.SetActive(true);
        _view.LockSprite_UISprite.enabled = true;
    }
    #endregion

    #region 交易所

    public void SetTradeItemCell(BagItemDto dto)
    {
        UpdateView();
        _view.CountLabel_UILabel.text = dto.count.ToString();
        UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.item.icon);
        _view.IconSprite_UISprite.enabled = true;
        _view.BorderSprite_UISprite.spriteName = string.Format("item_ib_{0}", dto.item.quality);
    }
    #endregion

    #region 生活委托
    public void UpdateView(GeneralItem dto, string str, bool isCenter=false)
    {
        if (dto == null) return;

        _generalItem = dto;
        View.CountLabel_UILabel.text = str;
        if(isCenter)
        {
            View.CountLabel_UILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            View.CountLabel_UILabel.pivot = UIWidget.Pivot.Center;
            View.CountLabel_UILabel.transform.localPosition = new Vector3(0, View.CountLabel_UILabel.transform.localPosition.y);
        }
        View.IconSprite_UISprite.enabled = true;
        UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.icon);
        if (dto is AppItem)
        {
            var appitem = dto as AppItem;
            _itemType = appitem.itemType;            
            View.BorderSprite_UISprite.enabled = true;
            SetQuality(appitem.quality);
        }
        else if (dto is AppVirtualItem)
        {
            var appVirtualitem = dto as AppVirtualItem;
            _itemType = appVirtualitem.itemType;
            View.BorderSprite_UISprite.enabled = true;
            SetQuality(appVirtualitem.quality);
        }
    }
    #endregion

    private void SetUsePopsData(string icon,string label)
    {
        string[] icons = icon.Split(':');
        switch(Int32.Parse(icons[0]))
        {
            case 1:
                UIHelper.SetItemIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 2:
                UIHelper.SetPetIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 3:
                UIHelper.SetSkillIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 4:
                UIHelper.SetOtherIcon(View.IconSprite_UISprite,icons[1]);
                break;
        }
        View.IconSprite_UISprite.enabled = true;
        //if(View.TempNameLbl_UILabel != null)
            //View.TempNameLbl_UILabel.text = label;
    }

    #region 伙伴技能

    public void UpdateView(Skill skill)
    {
        //UpdateView();
        //UIHelper.SetSkillIcon(_view.IconSprite_UISprite, skill.icon);
        UIHelper.SetUITexture(_view.IconTex_UITexture, skill.icon,false);
        _view.CountLabel_UILabel.text = skill.name;
        _view.IconSprite_UISprite.enabled = true;
    }
    #endregion
    //用特定接口类型数据刷新界面
    public void UpdateView(GeneralItem dto, int count = 0, bool isLock = false)
    {
        _generalItem = dto;
        if (dto != null)
        {
            ItemCount = count;
            View.IconSprite_UISprite.enabled = true;
            UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.icon);
            if (dto is AppItem)
            {
                _itemType = (dto as AppItem).itemType;

                var rDto = dto as AppItem;
                //UIHelper.SetItemQualityIcon(View.BorderSprite_UISprite, rDto.quality);
                SetQuality(rDto.quality);
                View.BorderSprite_UISprite.enabled = true;
            }
            else if (dto is AppVirtualItem)
                _itemType = (dto as AppVirtualItem).itemType;
        }
        else
        {
            UpdateViewWithNull();
        }

        View.LockSprite_UISprite.enabled = isLock;
    }

    public void UpdateView(BagItemDto dto, bool _isSelect, bool isLock)
    {
        if (dto == null) return;
        _dto = dto;
        _generalItem = dto.item;
        _itemType = dto.item.itemType;
        ItemCount = dto.count;
        View.IconSprite_UISprite.enabled = true;
        View.BorderSprite_UISprite.enabled = true;
        if (_dto.item != null)
            UIHelper.SetItemIcon(View.IconSprite_UISprite, _dto.item.icon);

        View.LockSprite_UISprite.enabled = isLock;
        View.CountLabel_UILabel.text = ItemCount.ToString();
        isSelect = _isSelect;
    }
    public void UpdateView(BagItemDto dto, bool isLock)
    {
        _dto = dto;
        _generalItem = dto.item;

        GameDebuger.TODO(@"OnlyUsedInCSPK = ItemHelper.IsOnlyInCSPK(_dto);");
        OnlyUsedInCSPK = false;
        UpdateCSPKMark();

        if (_dto != null)
        {
            _itemType = dto.item.itemType;
            ItemCount = _dto.count;
            View.IconSprite_UISprite.enabled = true;

            if (_dto.item != null)
            {
                View.BorderSprite_UISprite.enabled = true;
                if (_dto.item.itemType == (int)AppItem.ItemTypeEnum.Equipment)
                {
                    var equipmentDto = EquipmentMainDataMgr.DataMgr.MakeEquipmentDto(dto);
                    UpdateEquipView(equipmentDto);
                }
                else if (_dto.item.itemType == (int)AppItem.ItemTypeEnum.Quartz)
                {
                    _view.MarkCSPK_UISprite.gameObject.SetActive(true);
                    UIHelper.SetQuartyIcon(View.MarkCSPK_UISprite, _dto.item as Quartz==null ? "" : (_dto.item as Quartz).icon);
                    var quartzExtraDto = _dto.extra as QuartzExtraDto;
                    if(quartzExtraDto != null)
                    {
                        int quartzId = quartzExtraDto.baseProperties.Count > 0 ? quartzExtraDto.baseProperties[0].propId : quartzExtraDto.passiveSkill;
                        var quartzIcon = DataCache.getDtoByCls<QuartzBaseProperty>(quartzId);
                        if (quartzIcon != null)
                            UIHelper.SetQuartyIcon(View.IconSprite_UISprite, quartzIcon.icon);
                    }
                    SetQuality(_dto.item.quality);
                }
                else
                {
                    
                    UIHelper.SetItemIcon(View.IconSprite_UISprite, _dto.item.icon);
                    SetQuality(_dto.item.quality);
                    //UIHelper.SetItemQualityIcon(View.BorderSprite_UISprite, _dto.item.quality);
                }
                //if (View.TempNameLbl_UILabel != null)
                   // View.TempNameLbl_UILabel.text = dto.item.name;
            }
            else if (_dto.itemId < 100)
            {
                // 虚物
                var item = DataCache.getDtoByCls<GeneralItem>(_dto.itemId) as AppVirtualItem;
                if (item != null)
                {
                    UIHelper.SetItemIcon(View.IconSprite_UISprite, item.icon);
                }
            }
            View.LockSprite_UISprite.enabled = isLock;
            View.CountLabel_UILabel.text = ItemCount.ToString();
            if (View.BingingSpr_UISprite != null)
                View.BingingSpr_UISprite.gameObject.SetActive(dto.circulationType == (int)BagItemDto.CirculationType.Bind);
        }
        else
        {
            UpdateViewWithNull();
        }
    }

    /// <summary>
    /// 在背包上显示伙伴碎片
    /// </summary>
    /// <param name="chipDto"></param>
    /// <param name="isLock"></param>
    public void UpdateView(CrewChipDto chipDto, bool isLock)
    {
        if (chipDto == null) return;
        //碎片数据不属于背包 做判断0处理
        if (chipDto.chipAmount <= 0)
            return;
        _generalItem = chipDto.chip;
        _itemType = (int)AppItem.ItemTypeEnum.CrewChipDto;

        //var crew = DataCache.getDtoByCls<GeneralItem>(chipDto.chipId) as AppVirtualItem;
        //if (crew == null) return;
        var chipItem = chipDto.chip as AppVirtualItem;
        UIHelper.SetItemIcon(View.IconSprite_UISprite, chipDto.chip.icon);
        SetQuality(chipItem.quality);
        View.IconSprite_UISprite.enabled = true;
        View.BorderSprite_UISprite.enabled = true;
        View.CountLabel_UILabel.text = chipDto.chipAmount.ToString();
        if (View.BingingSpr_UISprite != null)
            View.BingingSpr_UISprite.gameObject.SetActive(false);
        //View.TempNameLbl_UILabel.text = chipDto.chip.name;
    }

    /// <summary>
    /// ItemTips使用
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="count"></param>
    /// <param name="needCound"></param>
    /// <param name="isLock"></param>
    public void UpdateView_ItemUse(GeneralItem dto, int count = 0,int needCound = 0, bool isLock = false)
    {
        _generalItem = dto;
        if (dto != null)
        {
            ItemCount = count;
            View.IconSprite_UISprite.enabled = true;
            UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.icon);
            if (dto is AppItem)
            {
                var rDto = dto as AppItem;
                _itemType = rDto.itemType;

                //UIHelper.SetItemQualityIcon(View.BorderSprite_UISprite, rDto.quality);
                SetQuality(rDto.quality);
                View.BorderSprite_UISprite.enabled = true;
                var defalutColor = "[383838]";
                var color = count >= needCound ? ColorConstant.Color_Tips_Enough_Str : ColorConstant.Color_Tips_NotEnough_Str;
                View.CountLabel_UILabel.text = string.Format("{3}{2}{0}[-]/{1}[-]", count, needCound, color, defalutColor);
            } else if(dto is VirtualItem)
            {
                _itemType = (dto as AppVirtualItem).itemType;

                var defalutColor = "[383838]";
                var color = count >= needCound ? ColorConstant.Color_Tips_Enough_Str : ColorConstant.Color_Tips_NotEnough_Str;
                View.CountLabel_UILabel.text = string.Format("{2}{1}{0}[-][-]", needCound, color,defalutColor);
                View.BorderSprite_UISprite.enabled = false;
                UIHelper.SetAppVirtualItemIcon(View.IconSprite_UISprite, (AppVirtualItem.VirtualItemEnum)dto.id);
            }

            //View.IconSprite_UISprite.MakePixelPerfect();
            
        }
        else
        {
            UpdateViewWithNull();
        }

        View.LockSprite_UISprite.enabled = isLock;
    }

    public void UpdateViewInCrewSkill(GeneralItem dto, int count = 0, int needCound = 0)
    {
        _isShowTipsOnClick = true;
        _generalItem = dto;
        if (dto != null)
        {
            ItemCount = count;
            View.IconSprite_UISprite.enabled = true;
            View.BorderSprite_UISprite.enabled = true;
            UIHelper.SetItemIcon(View.IconSprite_UISprite, dto.icon);
            string color = "[ffffff]";
            if (dto is AppItem)
            {
                var rDto = dto as AppItem;
                _itemType = rDto.itemType;
                SetQuality(rDto.quality);
                color = count >= needCound ? ColorConstant.Color_Tips_Enough_Str : ColorConstant.Color_Tips_NotEnough_Str;
            }
            else if (dto is VirtualItem)
            {
                var vDto = dto as AppVirtualItem;
                _itemType = vDto.itemType;
                color = count >= needCound ? ColorConstant.Color_Tips_Enough_Str : ColorConstant.Color_Tips_NotEnough_Str;
                UIHelper.SetAppVirtualItemIcon(View.IconSprite_UISprite, (AppVirtualItem.VirtualItemEnum)dto.id);
                SetQuality(vDto.quality);
                View.IconSprite_UISprite.MakePixelPerfect();
            }
            View.CountLabel_UILabel.text = string.Format("{2}{0}[-]/{1}", count, needCound, color);
        }
        else
        {
            UpdateViewWithNull();
        }
    }

    public void UpdateViewWithNull(bool isLock = false){

        _dto =null;
        _equipmentDto = null;
        _generalItem = null;
        _itemType = 0;
        if (View.TempNameLbl_UILabel != null)
            View.TempNameLbl_UILabel.text = "";
        View.CountLabel_UILabel.text = "";
        View.IconSprite_UISprite.enabled = false;
        View.WearSprite_UISprite.enabled = false;
        View.BorderSprite_UISprite.enabled = false;
        View.redSpr.SetActive(false);
        View.LockSprite_UISprite.enabled = isLock;
        View.MarkCSPK_UISprite.gameObject.SetActive(false);
        if (View.BingingSpr_UISprite != null)
            View.BingingSpr_UISprite.gameObject.SetActive(false);
    }

    public void UpdateView(tempdto dto, bool isLock = false){
    }

    /// <summary>
    /// 更新装备的格子~
    /// </summary>
    /// <param name="equipmentDto"></param>
    public void UpdateEquipView(EquipmentDto equipmentDto=null,bool isEquip = false)
    {
        ItemCount = 1;

        if (equipmentDto != null)
        {
            View.IconSprite_UISprite.enabled = true;
            View.BorderSprite_UISprite.enabled = true;
            var appitem = equipmentDto.equip as Equipment;
            UIHelper.SetItemIcon(View.IconSprite_UISprite, appitem.icon);
            SetQuality(equipmentDto.property.quality);
            View.WearSprite_UISprite.enabled = isEquip; //是否已经装备

            _equipmentDto = equipmentDto;
            _itemType = (int)AppItem.ItemTypeEnum.Equipment;
            _generalItem = ItemHelper.GetGeneralItemByItemId(equipmentDto.equipId);
        }
        else
        {
            View.IconSprite_UISprite.enabled = false;
            View.BorderSprite_UISprite.enabled = false;
        }
        
        //UIHelper.SetItemIcon(View.IconSprite_UISprite, "EqPart_" + (int)partType);
    }

    public void UpdateView(BagItemDto dto, Action<ItemCellController> onClickCallBack)
    {
        _dto = dto;
        _generalItem = dto.item;

        UpdateView(dto, false);
        _onClickCallBack = onClickCallBack;
        _itemType = dto.item.itemType;
    }

    private void OnClickItemCell()
    {
        if (_onClickCallBack != null)
            _onClickCallBack(this);

        if(_isShowTipsOnClick && _generalItem != null)
        {
            switch(_itemType)
            {
                case (int)AppItem.ItemTypeEnum.MissionItem:
                case (int)AppItem.ItemTypeEnum.VirtualItem:
                case (int)AppItem.ItemTypeEnum.CrewChipDto:
                case (int)AppItem.ItemTypeEnum.PointType:
                case (int)AppItem.ItemTypeEnum.Experience:
                case (int)AppItem.ItemTypeEnum.Vigour:
                case (int)AppItem.ItemTypeEnum.Props:
                case (int)AppItem.ItemTypeEnum.Embed:
                case (int)AppItem.ItemTypeEnum.Engrave:
                case (int)AppItem.ItemTypeEnum.SkillBook:
                case (int)AppItem.ItemTypeEnum.TreasureMap:
                case (int)AppItem.ItemTypeEnum.PassiveSkillBook:
                case (int)AppItem.ItemTypeEnum.Horn:
                case (int)AppItem.ItemTypeEnum.GiftPackage:
                case (int)AppItem.ItemTypeEnum.ContestInvitation:
                case (int)AppItem.ItemTypeEnum.FormationBook:
                case (int)AppItem.ItemTypeEnum.CrewGift:
                    _tipsCtrl = ProxyTips.OpenTipsWithGeneralItem(_generalItem, _tipsShowPos);
                    break;
                case (int)AppItem.ItemTypeEnum.CarryCook:
                case (int)AppItem.ItemTypeEnum.GrailCook:
                    if (_dto == null)
                        ProxyTips.OpenTipsWithGeneralItem(_generalItem, _tipsShowPos);
                    else
                        _tipsCtrl = ProxyTips.OpenAssistSkillPropTips(_dto);
                    _tipsCtrl.SetTipsPosition(_tipsShowPos, _isLeft);
                    break;
                case (int)AppItem.ItemTypeEnum.Medallion:
                    if (_dto == null)
                        ProxyTips.OpenTipsWithGeneralItem(_generalItem, _tipsShowPos);
                    else
                        _tipsCtrl = ProxyTips.OpenMedallionTips(_dto);
                    _tipsCtrl.SetTipsPosition(_tipsShowPos, _isLeft);
                    break;
                case (int)AppItem.ItemTypeEnum.Equipment:
                    //_tipsCtrl = ProxyTips.OpenEquipmentTips_FromBag(_equipmentDto,isShowCompare:true);
                    if (_equipmentDto == null)
                    {
                        ProxyTips.OpenTipsWithGeneralItem(_generalItem, _tipsShowPos);
                        return;
                    }
                    var isEquip = EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(_equipmentDto);
                    var curEquipmentDto = EquipmentMainDataMgr.DataMgr.GetSameEquipmentByPart(_equipmentDto);
                    //装备在身上
                    if (isEquip && curEquipmentDto!=null)
                    {
                        var embedVo = EquipmentMainDataMgr.DataMgr.GetEmbedInfoByPart((Equipment.PartType)curEquipmentDto.partType);
                        _tipsCtrl = ProxyTips.OpenEquipmentTips(_equipmentDto, embedVo);
                        _tipsCtrl.SetTipsPosition(_tipsShowPos, false);
                        //_tipsCtrl.SwapCompare_MainPostion();
                    }
                    else
                    {
                        _tipsCtrl = ProxyTips.OpenEquipmentTips(_equipmentDto, null);
                        _tipsCtrl.SetTipsPosition(_tipsShowPos, false);
                    }
                    break;
                case (int)AppItem.ItemTypeEnum.Quartz:
                    if (_dto == null)
                    {
                        ProxyTips.OpenTipsWithGeneralItem(_generalItem, _tipsShowPos);
                        return;
                    }
                    _tipsCtrl = ProxyTips.OpenQuartzTips(_dto);
                    _tipsCtrl.SetTipsPosition(_tipsShowPos, _isLeft);
                    break;
            }
            SetTipsBtnDetail();
        }
    }

    #region Tips相关设置
    private void SetDefaultTipsPosition()
    {
        View.ItemCell_UISprite.pivot = UIWidget.Pivot.Center;
        var uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        var screenPos = uiCamera.WorldToScreenPoint(transform.position);
        _tipsShowPos = new Vector3(screenPos.x + View.ItemCell_UISprite.width/2 - uiCamera.pixelWidth/2,
            screenPos.y - uiCamera.pixelHeight / 2);
    }
    public void SetShowTips(bool isShow)
    {
        _isShowTipsOnClick = isShow;
    }
    public void SetTipsPosition(Vector3 pos, bool isLeft=true)
    {
        _tipsShowPos = pos;
        _isLeft = isLeft;
    }
    //设置tips按钮响应
    private void SetTipsBtnDetail()
    {
        if (_itemType == 0 || _generalItem == null) return;
        if (BackpackView.NAME == UIModuleManager.Instance.TopModule)
        {
            if(BackpackDataMgr.DataMgr.GetBackpackViewTab() == BackpackViewTab.Backpack)
                _tipsType = ItemCellTipsBtnType.BackpackType;
            else if(BackpackDataMgr.DataMgr.GetBackpackViewTab() == BackpackViewTab.Warehouse)
                _tipsType = ItemCellTipsBtnType.Ware;
        }
        else if(TempBackPackView.NAME == UIModuleManager.Instance.TopModule)
            _tipsType = ItemCellTipsBtnType.Temppack;

        if (_tipsType == ItemCellTipsBtnType.Null) return;

        TipsBtnPanelViewController btnCtrl = null;
        switch (_tipsType)
        {
            case ItemCellTipsBtnType.BackpackType:
                //WARNING： 使用时  逻辑id优先 所以在props和gift两个类型中都有相关的处理
                switch (_itemType)
                {
                    //暂时注释 无功能 不显示 todo xjd
                    //case (int)AppItem.ItemTypeEnum.Virtual:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.MissionItem:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.VirtualItem:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.CrewChipDto:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.PointType:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.Experience:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.Vigour:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    case (int)AppItem.ItemTypeEnum.Props:
                        //logicid 8 9 10 13 14 都是礼包 
                        if (_dto == null) return;
                        var props = _dto.item as Props;
                        if (props == null) return;
                        if (props.logicId >= 8 && props.logicId < 11 || props.logicId == 13 
                            || props.logicId == 14 || props.logicId == 20)
                        {
                            _tipsCtrl.SetBtnPressClose(false);
                            btnCtrl = _tipsCtrl.SetBtnView("使用", "", () =>
                            {
                                BackpackDataMgr.BackPackNetMsg.BackpackApply(_dto.index, 1, callback: () =>
                                {
                                    if(_dto == null || _generalItem == null)
                                        _tipsCtrl.Close();
                                });
                            });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        else if(props.logicId == 16) //鲜花
                        {
                            btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => 
                            {
                                ProxyFlowerMainView.Open(null, _dto.itemId);
                                UIModuleManager.Instance.CloseModule(BackpackView.NAME);
                            });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        break;
                    //case (int)AppItem.ItemTypeEnum.Embed:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.Medallion:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.Engrave:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.SkillBook:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    //case (int)AppItem.ItemTypeEnum.CarryCook:
                    //    btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { TipManager.AddTip("对应的操作 "); });
                    //    _tipsCtrl.ReSetAllPos(btnCtrl);
                    //    break;
                    case (int)AppItem.ItemTypeEnum.GrailCook:
                        if (_dto == null) return;
                        btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { BackpackDataMgr.BackPackNetMsg.BackpackApply(_dto.index, 1); });
                        _tipsCtrl.ReSetAllPos(btnCtrl);
                        break;
                    case (int)AppItem.ItemTypeEnum.Equipment:
                        if (_equipmentDto == null) return;
                        var isEquip = EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(_equipmentDto);
                        //装备在身上
                        if (isEquip)
                        {
                            btnCtrl = _tipsCtrl.SetBtnView("卸下", "", () => { EquipmentMainDataMgr.EquipmentMainNetMsg.ReqTakeOffEquipment(_equipmentDto); });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        else
                        {
                            btnCtrl = _tipsCtrl.SetBtnView("装备", "分解", () => { EquipmentMainDataMgr.EquipmentMainNetMsg.ReqEquip_Wear(_equipmentDto); },
                                () => { BackpackDataMgr.BackPackNetMsg.ResolveEquipment(_equipmentDto.equipUid.ToString()); });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        break;
                    case (int)AppItem.ItemTypeEnum.Quartz:
                        if (_dto == null) return;
                        btnCtrl = _tipsCtrl.SetBtnView("分解", "", () => { BackpackDataMgr.BackPackNetMsg.ResolveQuartz(_dto.uniqueId.ToString()); });
                        _tipsCtrl.ReSetAllPos(btnCtrl);
                        break;
                    case (int)AppItem.ItemTypeEnum.TreasureMap:
                        if (_dto == null || _dto.item as Props == null) return;
                        var propsDto = _dto.item as Props;
                        if (propsDto.logicId == 17 && _dto.extra as PropsExtraDto_17 != null)
                        {
                            btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => 
                            {
                                MissionDataMgr.DataMgr.TreasureMission(_dto);
                                UIModuleManager.Instance.CloseModule(BackpackView.NAME);
                            });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        else if(propsDto.logicId == 18)
                        {
                            btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { ProxyTreasureMission.Open(); });
                            _tipsCtrl.ReSetAllPos(btnCtrl);
                        }
                        break;
                    case (int)AppItem.ItemTypeEnum.PassiveSkillBook:
                        break;
                    case (int)AppItem.ItemTypeEnum.Horn:
                        break;
                    case (int)AppItem.ItemTypeEnum.GiftPackage:
                        if (_dto == null) return;
                        btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { BackpackDataMgr.BackPackNetMsg.BackpackApply(_dto.index, 1); });
                        _tipsCtrl.ReSetAllPos(btnCtrl);
                        break;
                    case (int)AppItem.ItemTypeEnum.ContestInvitation:
                        if(_dto == null) return;
                        btnCtrl = _tipsCtrl.SetBtnView("使用", "", () => { ProxyContest.Open(_dto); });
                        _tipsCtrl.ReSetAllPos(btnCtrl);
                        break;
                    case (int)AppItem.ItemTypeEnum.FormationBook:
                        break;
                    case (int)AppItem.ItemTypeEnum.CrewGift:
                        break;
                }
                break;
            case ItemCellTipsBtnType.Ware:
                cellDoubleClickEvent.Subscribe(_ => { CloseTips(); });
                if (_dto.bagId == (int)AppItem.BagEnum.Backpack)
                {
                    btnCtrl = _tipsCtrl.SetBtnView("存入", "", () => { BackpackDataMgr.BackPackNetMsg.ReqMoveItemToWareHouse(_dto.index, BackpackDataMgr.DataMgr.CurWareHousePage); });
                    _tipsCtrl.SetTipsPosition(new Vector3(-284, 168, 0));
                }
                else if (_dto.bagId == (int)AppItem.BagEnum.Warehouse)
                {
                    btnCtrl = _tipsCtrl.SetBtnView("取出", "", () => { BackpackDataMgr.BackPackNetMsg.ReqMoveItemToBackPack(_dto.index); });
                    _tipsCtrl.SetTipsPosition(new Vector3(-63, 168, 0));
                } 
                _tipsCtrl.ReSetAllPos(btnCtrl);
                
                break;
            case ItemCellTipsBtnType.Temppack:
                cellDoubleClickEvent.Subscribe(_ => { CloseTips(); });
                if (_dto.bagId == (int)AppItem.BagEnum.Temppack)
                    btnCtrl = _tipsCtrl.SetBtnView("转移", "", () => { BackpackDataMgr.BackPackNetMsg.TransItemFromTempBagToBack(_dto.index); });
                _tipsCtrl.ReSetAllPos(btnCtrl);
                break;
        }
    }
    #endregion

    public void SetOtherIcon(string icon)
    {
        UIHelper.SetOtherIcon(View.IconSprite_UISprite, icon);
        UpdateIcon();
    }
    /// <summary>
    /// 设置为灰度图
    /// </summary>
    public void SetIconGrey(bool isGrey, string iconName ="")
    {
        View.IconSprite_UISprite.enabled = true;
        View.IconSprite_UISprite.isGrey = isGrey;
        
        if(!iconName.Equals( string.Empty))
            UIHelper.SetItemIcon(View.IconSprite_UISprite,iconName);
    }

    public void SetBorderGrey(bool isGrey)
    {
        if (View.BorderSprite_UISprite != null)
            View.BorderSprite_UISprite.isGrey = isGrey;
    }
    public void SetQuality(int quality)
    {
        UIHelper.SetItemQualityIcon(View.BorderSprite_UISprite, quality);
    }
    private void UpdateIcon()
    {
        View.IconSprite_UISprite.MakePixelPerfect();
        View.IconSprite_UISprite.enabled = true;
    }

    #region 是否跨服

    private bool mOnlyUsedInCSPK = false;

    public bool OnlyUsedInCSPK
    {
        get
        {
            return mOnlyUsedInCSPK;
        }
        set
        {
            mOnlyUsedInCSPK = value;
            UpdateCSPKMark();
        }
    }

    private void UpdateCSPKMark()
    {
        if (null != _view && null != _view.gameObject)
        {
            if (View.MarkCSPK_UISprite.gameObject.activeSelf != OnlyUsedInCSPK)
            {
                View.MarkCSPK_UISprite.gameObject.SetActive(OnlyUsedInCSPK);
            }
        }
    }

    protected override void OnDispose()
    {
        _isShowTipsOnClick = false;
        _itemType = 0;
        _tipsType = ItemCellTipsBtnType.Null;
        _generalItem = null;
        _dto = null;
        cellClickEvent = cellClickEvent.CloseOnceNull();
        cellDoubleClickEvent = cellDoubleClickEvent.CloseOnceNull();
        if (View.IconTex_UITexture != null && View.IconTex_UITexture.mainTexture != null)
            Resources.UnloadAsset(View.IconTex_UITexture.mainTexture);
    }
    #endregion
}