// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentEmbedController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using System;
using System.Collections.Generic;
using AppDto;

public partial interface IEquipmentEmbedController
{
    /// <summary>
    /// 装备选择部位
    /// </summary>
    UniRx.IObservable<EquipmentEmbedCellVo> OnEquipmentChoiceStream { get; }

    /// <summary>
    /// 点击宝石孔
    /// </summary>
    UniRx.IObservable<EquipmentEmbedHoleVo> OnClickEmbedHole { get; }

    /// <summary>
    /// 点击宝石选择
    /// </summary>
    UniRx.IObservable<GeneralItem> OnClickEmbedItem { get; }
}
public partial class EquipmentEmbedController
{

    

    Subject<EquipmentEmbedCellVo> _onEquipmentChoiceStream = new Subject<EquipmentEmbedCellVo>();
    public UniRx.IObservable<EquipmentEmbedCellVo> OnEquipmentChoiceStream { get { return _onEquipmentChoiceStream; } }

    Subject<EquipmentEmbedHoleVo> _onClickEmbedHole = new Subject<EquipmentEmbedHoleVo>();

    public UniRx.IObservable<EquipmentEmbedHoleVo> OnClickEmbedHole
    {
        get { return _onClickEmbedHole; }
    }

    //Subject<BagItemDto> _onClickEmbedItem = new Subject<BagItemDto>();
    public UniRx.IObservable<GeneralItem> OnClickEmbedItem { get { return eq_goodsChoiceContent.OnClickGoodsStream; } }
    List<EquipmentEmbedCellController> partCellCtrts = new List<EquipmentEmbedCellController>();

    List<EquipmentEmbedHoleController> embedHoleCtrls = new List<EquipmentEmbedHoleController>();

    List<Eq_GoodItemController> embedItemCtrls = new List<Eq_GoodItemController>();

    Eq_GoodsChoiceContentController eq_goodsChoiceContent;
    /// <summary>
    /// 最多只有4个属性
    /// </summary>
    List<UILabel> propertysPools = new List<UILabel>();

    CompositeDisposable _disposabe;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposabe = new CompositeDisposable();
        embedHoleCtrls.Add( AddChild<EquipmentEmbedHoleController, EquipmentEmbedHole>(View.UpAnchor, EquipmentEmbedHole.NAME));
        embedHoleCtrls.Add( AddChild<EquipmentEmbedHoleController, EquipmentEmbedHole>(View.RightAnchor, EquipmentEmbedHole.NAME));
        embedHoleCtrls.Add( AddChild<EquipmentEmbedHoleController, EquipmentEmbedHole>(View.DownAnchor, EquipmentEmbedHole.NAME));
        embedHoleCtrls.Add( AddChild<EquipmentEmbedHoleController, EquipmentEmbedHole>(View.LeftAnchor, EquipmentEmbedHole.NAME));

        embedHoleCtrls.ForEachI((x, i) => {
            x.OnBg_UIButtonClick.Subscribe(_ => {
                var tempData = x.data;
                _onClickEmbedHole.OnNext(tempData);
            });
        });

        propertysPools.AddRange(View.AttrContent_UIGrid.GetComponentsInChildren<UILabel>(true));

        eq_goodsChoiceContent = AddChild<Eq_GoodsChoiceContentController, Eq_GoodsChoiceContent>(View.RightContent, Eq_GoodsChoiceContent.NAME);
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
    public void UpdateViewData(IEquipmentEmbedViewData data)
    {
        _disposabe.Clear();
        UpdateLeftContent(data);
        UpdateMiddleCotent(data);
        UpdateRightContent(data);
    }

    private void UpdateLeftContent(IEquipmentEmbedViewData data)
    {
       
        partCellCtrts.ForEach(x => x.Hide());
        data.EmbedCellPartItems.ForEachI((x, i) =>
        {
            if (partCellCtrts.Count <= i)
            {
                var ctrl = AddChild<EquipmentEmbedCellController, EquipmentEmbedCell>(View.LeftContent, EquipmentEmbedCell.NAME);
                partCellCtrts.Add(ctrl);
            }
            var equipment = data.CurEquipments.Find(g => g.partType == (int)x.part);
            partCellCtrts[i].UpdateView(x, equipment, data.CurChoicePartVo == x);
            partCellCtrts[i].Show();
            var tempData = x;
            _disposabe.Add(partCellCtrts[i].OnEquipmentEmbedCell_UIButtonClick.Subscribe(_ =>
            {
                _onEquipmentChoiceStream.OnNext(tempData);
            }));
        });
        View.LeftContent.GetComponent<UIGrid>().Reposition();   
    }

    private void UpdateMiddleCotent(IEquipmentEmbedViewData data)
    {
        
        foreach(var x in embedHoleCtrls)
        {

        }
        embedHoleCtrls.ForEachI((x, i) => {
            var isChoice = data.CurChoiceHoleVo.holeInfo.holePos == data.EmbedItemVos[i].holeInfo.holePos;
            x.UpdateViewData(data.EmbedItemVos[i],isChoice);
        });

        propertysPools.ForEach(x => x.gameObject.SetActive(false));
        data.CurChoicePartVo.TotalProperty.ForEachI((x, i) => {
            var cbConfig = DataCache.getDtoByCls<CharacterAbility>(x.propId);
            var cbValue = x.propValue;
            string text = string.Format("{0}+{1}",cbConfig.name,cbValue);
            propertysPools[i].text = text;
            propertysPools[i].gameObject.SetActive(true);
        });
    }

    private void UpdateRightContent(IEquipmentEmbedViewData data)
    {
        eq_goodsChoiceContent.UpdateView(data.propsList);
        eq_goodsChoiceContent.View.EmptyPanel.SetActive(data.propsList.ToList().IsNullOrEmpty());
    }
}
