// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentAttrContentController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
public class BaseEquipmentAttrContentVo
{
    public EquipmentDto equipment;
    //title
    public string title;
    /// <summary>
    /// 空则打开空面板~
    /// </summary>
    public bool isEmpty;
    /// <summary>
    /// 空显示的名字
    /// </summary>
    public string emptyTitle;
    public BaseEquipmentAttrContentVo(string title, bool isEmpty, string emptyTitle = "")
    {
        this.title = title;
        this.isEmpty = isEmpty;
        this.emptyTitle = emptyTitle;
    }

    protected void UpdateInfo(EquipmentDto dto)
    {
        this.equipment = dto;
    }

    public void SetEmpty(bool isEmpty,string emptyStr)
    {
        this.isEmpty = isEmpty;
        this.emptyTitle = emptyStr;
    }
}
/// <summary>
/// 洗练
/// </summary>
public class EquipmentResetAttrContentVo: BaseEquipmentAttrContentVo
{
    /// <summary>
    /// 属性列表
    /// </summary>
    List<ResetAttrItemVo> resetAttrVolist;
    public IEnumerable<ResetAttrItemVo> ResetAttrVolist
    {
        get { return resetAttrVolist; }
    } 
    public EquipmentResetAttrContentVo(string title,bool isEmpty,string emptyTitle = ""):base(title,isEmpty,emptyTitle) {
        resetAttrVolist = new List<ResetAttrItemVo>();
    }
    public void UpdateAttrItemVo(EquipmentDto dto, List<ResetAttrItemVo> list)
    {
        base.UpdateInfo(dto);
        resetAttrVolist.Clear();
        if(list!=null)
            resetAttrVolist.AddRange(list);
    }
}

/// <summary>
/// 纹章
/// </summary>
public class EquipmentMedallionContentVo: BaseEquipmentAttrContentVo
{
    public List<string> effectStrList = new List<string>();
    public List<string> iconNameStrList = new List<string>();
    public Dictionary<int, float> idToEffect = new Dictionary<int, float>();
    string tipsCapacity;

    int usedCap = 0;
    float propsTimes = 1.0f;
    public EquipmentMedallionContentVo(string title, bool isEmpty, string emptyTitle = ""):base(title,isEmpty,emptyTitle)
    {

    }

    public void UpdateAttrItemVo(EquipmentDto equipment,MedallionDto mdedallionDto)
    {
        base.UpdateInfo(equipment);
        #region 铭刻符属性数据计算
        if (mdedallionDto == null)
            return;
        var itemData = ItemHelper.GetGeneralItemByItemId(mdedallionDto.itemId);
        //MedallionDto mdedallionDto = equipment.property.medallion;
        List<EngraveDto> engravesList = mdedallionDto.engraves;
        effectStrList.Clear();
        iconNameStrList.Clear();
        idToEffect.Clear();
        usedCap = 0;
        propsTimes = 1.0f;
        engravesList.ForEach(ItemDto =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(ItemDto.itemId);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            //强化铭刻符提升属性总倍数
            if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
            {
                propsTimes *= ItemDto.effect;
            }
            else if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY) //相同属性叠加
            {
                if (idToEffect.ContainsKey(ItemDto.itemId))
                    idToEffect[ItemDto.itemId] = idToEffect[ItemDto.itemId] + ItemDto.effect;
                else
                    idToEffect.Add(ItemDto.itemId, ItemDto.effect);
            }
            //圣能、Icon
            usedCap += ItemDto.occupation;
            iconNameStrList.Add(localData.icon);
        });

        idToEffect.ForEach(item =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(item.Key);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            effectStrList.Add(DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name + "+" + ((int)(item.Value * propsTimes)).ToString());
        });

        tipsCapacity = string.Format("圣能{0}/{1}", usedCap, (itemData as MedallionProps).capacity);
        effectStrList.Add(tipsCapacity);
        #endregion
    }
}

public class EquipmentResetAttrContentCtrl: BaseEquipmentAttrContentController
{
    List<ResetAttrItemController> resetAttrCtrls = new List<ResetAttrItemController>();
    protected override void AfterInitView()
    {
        base.AfterInitView();
        for (int i = 0; i < 7; i++)
        {
            var ctrl = AddChild<ResetAttrItemController, ResetAttrItem>(View.AttrContent, ResetAttrItem.NAME);
            resetAttrCtrls.Add(ctrl);
        }
        View.AttrContent_UITable.Reposition();
    }
    public void UpdateViewData(EquipmentResetAttrContentVo vo)
    {
        base.UpdateBaseInfo(vo);
        if (vo.isEmpty)
        {
            View.AttrContent.gameObject.SetActive(false);
            return;
        }
        else
        {
            View.AttrContent.gameObject.SetActive(true);
        }

        resetAttrCtrls.ForEach(x => x.Hide());
        if (!vo.isEmpty)
        {
            vo.ResetAttrVolist.ForEachI((x, i) =>
            {
                resetAttrCtrls[i].UpdateView(x);
                resetAttrCtrls[i].Show();
            });
            View.AttrContent_UITable.Reposition();
        }      
    }
}

public class EquipmentMedallionAttrContentCtrl: BaseEquipmentAttrContentController
{
    List<GoodsTipsLabelItemController> goodsTipsLabelCtrls = new List<GoodsTipsLabelItemController>();
    GoodsTipsRuneViewController goodTipsRuneCtrl;
    protected override void AfterInitView()
    {
        base.AfterInitView();
    }

    public void UpdateViewData(EquipmentMedallionContentVo vo)
    {

        base.UpdateBaseInfo(vo);

        

        if (vo.isEmpty)
        {
            View.AttrContent.gameObject.SetActive(false);
            //goodsTipsLabelCtrls.ForEach(x => x.Hide());
            //if (goodTipsRuneCtrl != null)
            //    goodTipsRuneCtrl.Hide();
            return;
        }
        else
        {
            View.AttrContent.gameObject.SetActive(true);
            

        }
            

        goodsTipsLabelCtrls.ForEach(x => x.Hide());
        vo.effectStrList.ForEachI((x, i) => {
            if(goodsTipsLabelCtrls.Count <= i)
            {
                var GoCtrl = AddChild<GoodsTipsLabelItemController, GoodsTipsLabelItem>(View.AttrContent, GoodsTipsLabelItem.NAME);
                goodsTipsLabelCtrls.Add(GoCtrl);
            }
            var ctrl = goodsTipsLabelCtrls[i];
            ctrl.UpdateView(x);
            ctrl.Show();
        });

        if (goodTipsRuneCtrl == null)
            goodTipsRuneCtrl = AddChild<GoodsTipsRuneViewController, GoodsTipsRuneView>(View.AttrContent, GoodsTipsRuneView.NAME);
        goodTipsRuneCtrl.UpdateView("印记", vo.iconNameStrList);
        goodTipsRuneCtrl.Show();
        goodTipsRuneCtrl.transform.SetAsLastSibling();

        View.AttrContent_UITable.Reposition();
    }
}


public partial class BaseEquipmentAttrContentController
{
    
    protected ItemCellController itemCellCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
        
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    protected void UpdateBaseInfo(BaseEquipmentAttrContentVo vo)
    {
        if (itemCellCtrl == null)
        {
            itemCellCtrl = AddChild<ItemCellController, ItemCell>(View.ItemCellAnchor, ItemCell.Prefab_BagItemCell);
        }
        View.title_UILabel.text = vo.title;
        if (vo.isEmpty)
        {
            View.NullTipLbl_UILabel.gameObject.SetActive(true);
            View.NullTipLbl_UILabel.text = vo.emptyTitle;
            View.ItemCellAnchor.gameObject.SetActive(false);
        }
        else
        {
            var equipment = vo.equipment;
           
            View.NullTipLbl_UILabel.text = "";
            View.NullTipLbl_UILabel.gameObject.SetActive(false);
            SetIconInfo(equipment);
        }
    }
    
    public void SetIconInfo(EquipmentDto equipment)
    {
        itemCellCtrl.UpdateEquipView(equipment);
        View.ItemName_UILabel.text = equipment.equip.name.WrapColor(ItemHelper.GetItemNameColorByRank(equipment.property.quality));
        View.ItemLv_UILabel.text = (equipment.equip as Equipment).grade + "级";
        View.ItemCellAnchor.gameObject.SetActive(true);
    }
    public void SetIconActive(bool active)
    {
        View.ItemCellAnchor.gameObject.SetActive(active);
    }
}
