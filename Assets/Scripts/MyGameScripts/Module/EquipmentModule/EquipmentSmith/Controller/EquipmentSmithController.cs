// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentSmithController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
public partial interface IEquipmentSmithController
{
    S3PopupListController PopUp_factionCtrl { get; }
    S3PopupListController PopUp_gradeCtrl { get; }
    S3PopupListController PopUp_quliatyCtrl { get; }
    UniRx.IObservable<bool> OnClickFastSmithToggle{ get; }
    UniRx.IObservable<int> OnClickSmithCell { get; }
    UniRx.IObservable<Unit> SmithHandler { get; }
}
public partial class EquipmentSmithController
{
    S3PopUpItemData OnChoiceData = new S3PopUpItemData() { bgSprite = "tab5_choose", fontSize = 20, rect = new Vector2(96, 34) };
    S3PopUpItemData OnNormalData = new S3PopUpItemData() { bgSprite = "tab5_normal", fontSize = 20, rect = new Vector2(96, 34) };

    S3PopupListController _popUp_factionCtrl;
    S3PopupListController _popUp_gradeCtrl;
    S3PopupListController _popUp_quliatyCtrl;

    ItemCellController eqItemCellCtrl;
    List<EquipmentSmithCellController> eqSmithCellCtrls = new List<EquipmentSmithCellController>();
    List<SmithItemCellController> smithItemCellCtrls = new List<SmithItemCellController>();

    ModelDisplayController modeldisplayer;
    private IEquipmentSmithViewData _data;

    List<UILabel> previewAttrLabels = new List<UILabel>();

    Subject<Unit> _smithEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> SmithHandler { get { return _smithEvt; } }

    //展示的个数
    const int ShowCellCount = 5;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        InitPopUpList();
        for(int i=0;i< ShowCellCount; i++)
        {
            var ctrl = AddChild<EquipmentSmithCellController,EquipmentSmithCell >(View.EquipmentGrid_UITable.gameObject, EquipmentSmithCell.NAME);
            eqSmithCellCtrls.Add(ctrl);
            ctrl.OnEquipmentSmithCell_UIButtonClick.Subscribe(x => { clickSmithCellStream.OnNext(ctrl.index); });
        }
        //快捷Toggle按钮赋值
        _onClickFastSmithToggle = View.FastSmithTg_UIToggle.OnValueChangedAsObservable();

        
        if(modeldisplayer == null)
        {
            modeldisplayer = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
               View.ModeDisplayAnchor
           , ModelDisplayUIComponent.NAME);
            modeldisplayer.Init(280, 280);
            modeldisplayer.SetBoxCollider(260, 350);
            modeldisplayer.SetupMainRoleModel();//, _mode == BackpackMode.FashionMode);//ModelManager.Player.TransformModelId
            modeldisplayer.CleanUpCustomAnimations();
        }

        //
        for(int i=0;i<3;i++)
        {
            previewAttrLabels.Add(View.AttrContent.FindScript<UILabel>("Attr_" + i));
        }
       
    }
    
    private void InitPopUpList()
    {
        //选择职业的
        var faction_dic = DataCache.getArrayByCls<Faction>();
        List<PopUpItemInfo> factionPopUpNameList = new List<PopUpItemInfo>();
        faction_dic.ForEach(x =>
        {
            if(x.open)
                factionPopUpNameList.Add(new PopUpItemInfo(x.name, x.id));
        });
        _popUp_factionCtrl = AddController<S3PopupListController, S3PopupList>(View.Popup_faction);
        _popUp_factionCtrl.InitData(S3PopupItem.PREFAB_EQUIPMENT, factionPopUpNameList, OnChoiceData, OnNormalData,42);
        //=======选择等级====
        int playerGrade = ModelManager.Player.GetPlayerLevel();
        //===TODO:应该按照玩家等级
        _popUp_gradeCtrl = AddController<S3PopupListController, S3PopupList>(View.Popup_grade);
        List<PopUpItemInfo> gradePopupNameList = new List<PopUpItemInfo>();
        int tempGrade = 70;
        for (int i = EquipmentMainDataMgr.EquipmentSmithViewData.MinChoiceGrade; i <= tempGrade; i += EquipmentMainDataMgr.EquipmentSmithViewData.GradeStep)
        {
            gradePopupNameList.Add(new PopUpItemInfo(i + "级", i));
        }
        _popUp_gradeCtrl.InitData(S3PopupItem.PREFAB_EQUIPMENT, gradePopupNameList, OnChoiceData, OnNormalData,42);
        //====选择质量====
        List<PopUpItemInfo> qualityPopupNameList = new List<PopUpItemInfo>();
        var euipmentQuality = DataCache.getArrayByCls<EquipmentQuality>();
        euipmentQuality.ForEach(x =>
            {
                if (x.id <= 2)
                    return;
                qualityPopupNameList.Add(new PopUpItemInfo(x.name, x.id));
            }
        );
        _popUp_quliatyCtrl = AddController<S3PopupListController, S3PopupList>(View.Popup_quality);
        _popUp_quliatyCtrl.InitData(S3PopupItem.PREFAB_EQUIPMENT, qualityPopupNameList, OnChoiceData, OnNormalData);
    }
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.SmithBtn_UIButton.onClick, () =>
        {
            for (int i = 0; i < _data.CurSmithItems.Count(); i++)
            {
                var item = _data.CurSmithItems.TryGetValue(i);
                if (item.currentCount < item.needCount)
                {
                    TipManager.AddTip(string.Format("{0}不足", item.props.name));
                    ShowItemGainWay(item.props.id);
                    ShowItemTips(item.props.id);
                    return;
                }
            }
            _smithEvt.OnNext(new Unit());
        });
    }
    

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    protected override void OnDispose()
    {

    }
   
    public void UpdateViewData(IEquipmentSmithViewData data)
    {
        UpdatePopUpList(data);
        _data = data;

        int len = -1;
        data.CurEquipmentCells.ForEachI((x,i) => {
            int index = i;
            len = i;
            eqSmithCellCtrls[i].UpdateViewData(index,x,x==data.curSelectEquipement);
            eqSmithCellCtrls[i].Show();
        });
        eqSmithCellCtrls.ForEachI((x,i) =>
        {
            if (i > len)
                x.Hide();
        });
        View.EquipmentGrid_UITable.Reposition();
        
        

        UpdateChoice(data);
    }
    private void UpdatePopUpList(IEquipmentSmithViewData data)
    {
        //================================更新PopUpList====================
        PopUp_factionCtrl.SetChoice(data.CurSelectFaction,true);
        PopUp_gradeCtrl.SetChoice(data.CurSelcetGrade,true);
        PopUp_quliatyCtrl.SetChoice(data.CurSelectQuality,true);
    }

    private void UpdateChoice(IEquipmentSmithViewData data)
    {
        if (data == null)
            return;
        if (eqItemCellCtrl == null)
            eqItemCellCtrl = AddChild<ItemCellController, ItemCell>(View.EquipmentIconAnchor, ItemCell.Prefab_BagItemCell);
        if(data.curSelectEquipement != null)
        {
            eqItemCellCtrl.UpdateView(data.curSelectEquipement);
            UIHelper.SetItemQualityIcon(eqItemCellCtrl.Border, data.CurSelectQuality);
            View.EquipmentName_UILabel.text = data.curSelectEquipement.name.WrapColor(ItemHelper.GetItemNameColorByRank(data.CurSelectQuality));
            View.EquipmentLv_UILabel.text = data.curSelectEquipement.grade +"级";

            var ownerMoney = ModelManager.IPlayer.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
            string color = "[FFFFFF]";
            if (ownerMoney < data.curSelectEquipement.smithSilver)
                color = "[FF0000]";
            View.SmithMoney_UILabel.text = string.Format("{0}", color + data.curSelectEquipement.smithSilver);
            UIHelper.SetAppVirtualItemIcon(View.SmithMoneyIcon_UISprite, AppVirtualItem.VirtualItemEnum.SILVER);
        }
        


        //=================更新锻造材料===================
        smithItemCellCtrls.ForEach(x => x.Hide());
        data.CurSmithItems.ForEachI((x, i) =>
        {
            if(smithItemCellCtrls.Count <= i)
            {
                var ctrl = AddChild<SmithItemCellController, SmithItemCell>(View.SmithItemAnchor_UITable.gameObject, SmithItemCell.NAME);
                smithItemCellCtrls.Add(ctrl);
            }
            smithItemCellCtrls[i].UpdateViewData(x);
            smithItemCellCtrls[i].Show();
        });
        //下一帧再执行
        View.SmithItemAnchor_UITable.Reposition();

        

        //如果为橙装 或者 红装，则不支持快捷打造
        if (data.CurSelectQuality == (int)AppItem.QualityEnum.ORANGE || data.CurSelectQuality == (int)AppItem.QualityEnum.RED)
        {
            View.FastSmithTg_UIToggle.gameObject.SetActive(false);
        }
        else
        {
            View.FastSmithTg_UIToggle.gameObject.SetActive(true); 
        }
        View.FastSmithTg_UIToggle.Set(data.FastSmith, false);

        SetEquipmentPreviewAttr(data);
    }

    //物品属性预览
    private void SetEquipmentPreviewAttr(IEquipmentSmithViewData data)
    {
        var equipment = data.curSelectEquipement;
        var quality = data.CurSelectQuality;
        if(equipment == null)
        {
            return;
        }
        
        //如果为蓝装 或者 紫装，才显示提示按钮，和神器值
        if (data.CurSelectQuality == (int)AppItem.QualityEnum.BLUE || data.CurSelectQuality == (int)AppItem.QualityEnum.PURPLE)
        {
            View.tipsBtn_UIButton.gameObject.SetActive(true);
            View.AtrifactContent.gameObject.SetActive(true);
            var maxValue= data.GetEquipmentAtrifactMaxValue(data.CurSelcetGrade, equipment.partType[0], data.CurSelectQuality);
            var curValue = data.GetGetEquipmentAtrifactCurValue(data.CurSelcetGrade, equipment.partType[0], data.CurSelectQuality);
            View.ArtifactProcessBar_UISlider.value = (float)curValue / (float)maxValue;
            View.ArtifactCount_UILabel.text = string.Format("{0}/{1}",curValue,maxValue);
        }
        else
        {
            View.AtrifactContent.gameObject.SetActive(false);
            View.tipsBtn_UIButton.gameObject.SetActive(false);
        }

        var AttrList = EquipmentMainDataMgr.DataMgr.GetEquipmentPropertyRange_BaseProperty(equipment.id, quality);
        //饰品比较特殊
        if (equipment.partType.Contains((int)Equipment.PartType.AccOne)|| equipment.partType.Contains((int)Equipment.PartType.AccTwo))
        {
            previewAttrLabels.ForEach(x => x.gameObject.SetActive(false));
            string res = string.Format("随机一级属性 {0}~{1}",AttrList[0].minValue,AttrList[0].maxValue);
            previewAttrLabels[0].text = res;
            previewAttrLabels[0].gameObject.SetActive(true);


        }
        else
        {
            previewAttrLabels.ForEach(x => x.gameObject.SetActive(false));
            AttrList.ForEachI((x, i) =>
            {
                var cb = DataCache.getDtoByCls<CharacterAbility>(x.abilityId);
                string res = string.Format("{0}:{1}~{2}", cb.name, x.minValue, x.maxValue);
                previewAttrLabels[i].text = res;
                previewAttrLabels[i].gameObject.SetActive(true);
            });
        }

        var table = View.AttrContent.GetComponent<UITable>();
        if(table != null)
        {
            table.Reposition();
        }

        View.AttrTipsLbl_UILabel.text = "提示:随机1~2条附加属性".WrapColor(ColorConstantV3.Color_Blue);
    }

    private void ShowItemGainWay(int itemId)
    {
        GainWayTipsViewController.OpenGainWayTip(itemId, new Vector3(50, 21, 0),  ProxyEquipmentMain.CloseMainView);
    }

    private void ShowItemTips(int itemId)
    {
        var itemDto = DataCache.getDtoByCls<GeneralItem>(itemId);
        ProxyTips.OpenTipsWithGeneralItem(itemDto, new Vector3(-363, 41, 0));
    }

    protected override void OnShow()
    {
        base.OnShow();
        //下一帧再执行
        //JSTimer.Instance.SetNextFrame(() =>
        //{
        //    View.EquipmentGrid_UITable.Reposition();
        //    View.SmithItemAnchor_UITable.Reposition();
        //});
        //View.EquipmentGrid_UITable.Reposition();
        //View.SmithItemAnchor_UITable.Reposition();
    }
    protected override void OnHide()
    {
        base.OnHide();


    }


    public S3PopupListController PopUp_factionCtrl { get { return _popUp_factionCtrl; } }
    public S3PopupListController PopUp_gradeCtrl { get { return _popUp_gradeCtrl; } }
    public S3PopupListController PopUp_quliatyCtrl { get { return _popUp_quliatyCtrl; } }

    Subject<bool> _onClickFastSmithToggle;
    public UniRx.IObservable<bool> OnClickFastSmithToggle { get { return _onClickFastSmithToggle; } }

    Subject<int> clickSmithCellStream = new Subject<int>();
    public IObservable<int> OnClickSmithCell { get { return clickSmithCellStream; } }
}
