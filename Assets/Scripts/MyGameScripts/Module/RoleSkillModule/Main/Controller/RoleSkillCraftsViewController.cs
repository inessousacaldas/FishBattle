// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillCraftsViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Manager;
using Assets.Scripts.MyGameScripts.UI;
using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;
public interface IRoleSkillCraftsViewController
{
    UniRx.IObservable<Unit> OnbtnUp_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnWidget_UIButtonClick { get; }
    void UpdateOnSctrlClick(IRoleSkillMainData data);
    void UpdateRight(IRoleSkillMainData data, RoleSkillCraftsItemController itemCtrl);
    RoleSkillCraftsItemController ScraftsItemCtrl { get; }
}

public partial class RoleSkillCraftsViewController
{
    private Vector3[] posList = new Vector3[]
    {
        new Vector3(-15f ,73.7f),new Vector3(52.8f ,-5.1f),new Vector3(52.3f ,-100f ),new Vector3(-15f ,-179f),
        new Vector3(-140f ,-179f),new Vector3(-206.7f ,-100f),new Vector3(-207.7f ,-5.1f),new Vector3(-140f,73.7f)
    };

    private Dictionary<int,RoleSkillCraftsItemController> itemList = new Dictionary<int, RoleSkillCraftsItemController>();
    public RoleSkillCraftsItemController defaultItem;
    private RoleSkillCraftsItemController sCraftsItem;
    private GameObject maskGO;


    private RoleSkillRangeController rangeCtrl;//技能范围
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        if(itemList != null && itemList.Count > 0)
        {
            itemList.Clear();
            itemList = null;
        }
        defaultItem = null;
        sCraftsItem = null;
        rangeCtrl = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    private void CreateItem(IRoleSkillMainData data)
    {
        if(itemList.Count > 0) return;
        var dict = data.CraftsDict;
        var eNum = dict.GetEnumerator();
        var i = 0;
        while(eNum.MoveNext())
        {
            var key = eNum.Current.Key;
            var value = eNum.Current.Value;
            var itemCtrl = AddChild<RoleSkillCraftsItemController,RoleSkillCraftsItem>(View.leftContent_Transform.gameObject,RoleSkillCraftsItem.NAME,RoleSkillCraftsItem.NAME);
            itemList[key] = itemCtrl;
            itemCtrl.View.transform.parent = View.itemTrans_Trans;
            itemCtrl.View.transform.localPosition = posList[value.cfgVO.postIndex -1];//index从1开始，所以要减1
            RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(itemCtrl,this);
            if(i == 0)
            {
                data.CurSelCtrl = itemCtrl;
                data.CurSelCtrl.SetSel(true);
            }
            i++;
        }
        sCraftsItem = AddChild<RoleSkillCraftsItemController,RoleSkillCraftsItem>(View.leftContent_Transform.gameObject,RoleSkillCraftsItem.NAME,RoleSkillCraftsItem.NAME);
        sCraftsItem.View.transform.localPosition = new Vector3(-103.4f, -88.3f);
        sCraftsItem.View.transform.localScale = Vector3.one;
        sCraftsItem.View.lblNum_UILabel.text = "";
        sCraftsItem.isSCraftsCtrl = true;
        RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(sCraftsItem,this);
    }

    public void UpdateView(IRoleSkillMainData data)
    {
        View.btnWidget_UIButton.gameObject.SetActive(false);
        View.lblFactionName_UILabel.text = ModelManager.Player.FactionName;
        UIHelper.SetCommonIcon(View.factionBg_UISprite, "faction_" + ModelManager.Player.FactionID);
        CreateItem(data);
        UpdatItem(data,data.CraftsDict);
        UpdateRight(data,data.CurSelCtrl);
        //UpdateMask(data);
    }

    private void UpdatItem(IRoleSkillMainData data,Dictionary<int,RoleSkillCraftsVO> dict)
    {
        foreach(var id in dict.Keys)
        {
            var itemCtrl = itemList[id];
            itemCtrl.UpdateView(dict[id],data.SCraftsState);
            if(data.CurSCrafts == id)
            {
                sCraftsItem.UpdateSCraftsView(dict[id]);
            }
        }
        sCraftsItem.View.lblName_UILabel.text = data.SCraftsState == RoleSkillMainSCraftsState.Normal ? "快捷S技" : "选择快捷S技";
    }

    public void UpdateRight(IRoleSkillMainData data,RoleSkillCraftsItemController itemCtrl)
    {
        if(itemCtrl != null)
        {
            View.lblCraftsName_UILabel.text = itemCtrl.vo.Name;
            View.lblDesc_UILabel.text = data.GetCraftsDesc(itemCtrl.vo.id);
            UpdateGrade(data,itemCtrl.vo);
            UpdateRange(data,itemCtrl.vo);
        }
    }

    private void UpdateGrade(IRoleSkillMainData data,RoleSkillCraftsVO itemVO)
    {
        
        var costGrade = data.GetCostByGradeDto(itemVO);
        if(itemVO.Grade == itemVO.cfgVO.maxGrade) {
            View.goldSp_Trans.gameObject.SetActive(false);
            View.lblUpGrade_UILabel.text = "升级条件：-\n\n\n升级消耗：-";
            View.itemUpGrade1_Transform.gameObject.SetActive(false);
            View.itemUpGrade2_Transform.gameObject.SetActive(false);
        }
        else
        {
            int limit = itemVO.LimitLevel;
            int playerLv = ModelManager.Player.GetPlayerLevel();
            View.goldSp_Trans.gameObject.SetActive(costGrade.silver > 0);
            View.lblUpGrade_UILabel.text = string.Format("升级条件：{0}\n\n\n升级消耗：        {1}", HtmlUtil.Font2(limit + "级", playerLv >= limit ? GameColor.WHITE : GameColor.RED), 
                costGrade.silver > 0 ? HtmlUtil.Font2(costGrade.silver, ModelManager.Player.GetPlayerWealthSilver() >= costGrade.silver ? GameColor.WHITE : GameColor.RED) : "");
            View.itemUpGrade1_Transform.gameObject.SetActive(costGrade.itemCount1 > 0);
            View.itemUpGrade2_Transform.gameObject.SetActive(costGrade.itemCount2 > 0);
            if(itemVO.cfgVO.consumeBook1 > 0)
            {
                UIHelper.SetItemIcon(View.itemUpGrade1_Icon_UISprite, itemVO.cfgVO.item1.icon);
                View.itemUpGrade1_Count_UILabel.text = costGrade.itemCount1.ToString();
            }
            if(itemVO.cfgVO.consumeBook2 > 0)
            {
                UIHelper.SetItemIcon(View.itemUpGrade2_Icon_UISprite, itemVO.cfgVO.item2.icon);
                View.itemUpGrade2_Count_UILabel.text = costGrade.itemCount2.ToString();
            }
        }
    }

    private void UpdateRange(IRoleSkillMainData data,RoleSkillCraftsVO itemVO)
    {
        var scopeVO = DataCache.getDtoByCls<SkillScope>(itemVO.cfgVO.scopeId);
        var type = data.GetScopeTarType(itemVO.cfgVO.scopeId);
        if(rangeCtrl == null)
        {
           rangeCtrl = RoleSkillRangeController.Show(View.rightContent_Transform.gameObject,scopeVO.scopeIndex,type);
            rangeCtrl.transform.localPosition = new Vector3(352f,91.1f);
        }else
        {
            rangeCtrl.Show(scopeVO.scopeIndex,type);
        }
    }

    private void UpdateMask(IRoleSkillMainData data)
    {
        if(maskGO == null && data.SCraftsState == RoleSkillMainSCraftsState.Select)
        {
            maskGO = UIComponentMgr.AddBgMask(View,View.transform,true,RoleSkillDataMgr.RoleSkillMainViewLogic.OnCloseSCraftsState);
        }
    }

    //S战技点击处理
    public void UpdateOnSctrlClick(IRoleSkillMainData data)
    {
        if(data.SCraftsState == RoleSkillMainSCraftsState.Select)
        {
            View.btnWidget_UIButton.gameObject.SetActive(true);
            itemList.ForEach(e =>
            {
                e.Value.UpdateOnSctrlClick(data,this);
            });
        }
        else
        {
            View.btnWidget_UIButton.gameObject.SetActive(false);
            itemList.ForEach(e =>
            {
                e.Value.UpdateViewDetail(e.Value.vo);
                e.Value.UpdateSel(data);
            });
        }
    }

    public RoleSkillCraftsItemController ScraftsItemCtrl { get { return sCraftsItem != null ? sCraftsItem : null; } }
}
