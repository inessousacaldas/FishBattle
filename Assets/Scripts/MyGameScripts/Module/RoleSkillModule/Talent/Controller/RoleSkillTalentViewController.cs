// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTalentViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Manager;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View;
using Assets.Scripts.MyGameScripts.UI;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IRoleSkillTalentViewController
{
    void ShowTalentGradeView(RoleSkillTalentSingleItemController singleItemCtrl);
}
public partial class RoleSkillTalentViewController
{
    private List<RoleSkillTalentItemController> itemList;
    private List<RoleSkillTalentSingleItemController> recommendList;
    private RoleSkillTalentGradeViewController talentGradeViewCtrl;
    private Dictionary<int, RoleSkillTalentSingleItemController> talentItemDic = new Dictionary<int, RoleSkillTalentSingleItemController>();
    private bool setFirst = false;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        
    }

    private void CreateItem(Dictionary<int,RoleSkillTalentItemVO> dataList)
    {
        if(itemList != null) return;
        itemList = new List<RoleSkillTalentItemController>();
        var eNum = dataList.GetEnumerator();
        var i =0;
        var height = 177;
        while(eNum.MoveNext())
        {
            var level = eNum.Current.Key;
            var itemVO = eNum.Current.Value;
            var itemCtrl = AddChild<RoleSkillTalentItemController,RoleSkillTalentItem>(View.Container_UIWidget.gameObject,RoleSkillTalentItem.NAME,RoleSkillTalentItem.NAME);
            itemList.Add(itemCtrl);
            var h = height - 111 * i;
            itemCtrl.View.transform.localPosition = new Vector3(1, h);
            itemCtrl.UpdateView(itemVO,AddToDic,i);
            i++;
        }
        //View.Container_UIWidget.height = Math.Abs(height);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {

    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        talentGradeViewCtrl = null;
        if(itemList != null)
        {
            itemList.Clear();
        }
        itemList = null;
        recommendList.Clear();
        recommendList = null;
        talentItemDic.Clear();
        setFirst = false;
    }

    // 业务逻辑数据刷新
    public void UpdateView(IRoleSkillTalentData data)
    {
        UpdateItem(data);
        View.lblFactionName_UILabel.text = ModelManager.Player.GetPlayer().faction.name;
        View.lblFactionDesc_UILabel.text = ModelManager.Player.GetPlayer().faction.description;
        if (talentGradeViewCtrl != null && talentGradeViewCtrl.IsActive())
        {
            if(data.LastItem!=null)
                talentGradeViewCtrl.UpdateView(data.LastItem.vo);
        }
    }

    private void UpdateItem(IRoleSkillTalentData data)
    {
        CreateItem(data.ListItem);
        for(var i = 0;i < itemList.Count;i++)
        {
            var item = itemList[i];
            item.UpdateView(data.ListItem[item.curVO.level], AddToDic, i);
        }
        View.lblFactionName_UILabel.text = ModelManager.Player.FactionName;
        View.lblFactionDesc_UILabel.text = ModelManager.Player.GetPlayer().faction.description;
        UpdateRecomend(data);
        UpdateItemArrow();
    }

    private void UpdateRecomend(IRoleSkillTalentData data)
    {
        if(recommendList != null) return;
        recommendList = new List<RoleSkillTalentSingleItemController>();
        for(var i = 0;i < data.RecommendList.Count;i++)
        {
            var itemCtrl = AddChild<RoleSkillTalentSingleItemController,RoleSkillTalentSingleItem>(View.rightContent_Transform.gameObject,RoleSkillTalentSingleItem.NAME,RoleSkillTalentSingleItem.NAME);
            recommendList.Add(itemCtrl);
            itemCtrl.View.transform.localPosition = new UnityEngine.Vector3(30 + 95 * i,-56);
            itemCtrl.UpdateReCommendView(data.GetCfgVOById(data.RecommendList[i]));
        }
    }

    public void UpdatePlayerInfo(IRoleSkillTalentData data,IPlayerModel model)
    {
        var curTalent = model.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.TALENTPOINT);
        View.lblHas_UILabel.text = HtmlUtil.Font2(curTalent.ToString(),"96ff00");
    }

    public void ShowTalentGradeView(RoleSkillTalentSingleItemController singleItemCtrl)
    {
        GameObject mask = null;
        if(talentGradeViewCtrl == null)
        {
            talentGradeViewCtrl = AddChild<RoleSkillTalentGradeViewController,RoleSkillTalentGradeView>(View.gameObject,RoleSkillTalentGradeView.NAME,RoleSkillTalentGradeView.NAME);
            RoleSkillDataMgr.RoleSkillTalentViewLogic.InitGradeCtrl(talentGradeViewCtrl);
            mask = UIComponentMgr.AddBgMask(talentGradeViewCtrl.View,View.transform,true,()=> 
            {
                talentGradeViewCtrl.View.Hide();
                if (RoleSkillDataMgr.DataMgr.TalentData.lastItem != null) RoleSkillDataMgr.DataMgr.TalentData.lastItem.SetSelected(false);
            });
        }else
        {
            if(talentGradeViewCtrl.IsActive() == false)
            {
                mask = UIComponentMgr.AddBgMask(talentGradeViewCtrl.View,View.transform,true, () => 
                {
                    talentGradeViewCtrl.View.Hide();
                    if (RoleSkillDataMgr.DataMgr.TalentData.lastItem != null) RoleSkillDataMgr.DataMgr.TalentData.lastItem.SetSelected(false);
                });
                talentGradeViewCtrl.Show();
            }
        }
        if(mask!=null)
        {
            mask.GetComponent<UIWidget>().depth = 100;
            mask.GetComponent<UISprite>().alpha = 1f / 255f;
        }
        var pos = singleItemCtrl.transform.localPosition;
        //pos.x += 30 - 109 - 200 + singleItemCtrl.transform.parent.localPosition.x;
        //pos.y += 30 + 250 - 94 + singleItemCtrl.transform.parent.localPosition.y;
        pos.x = -19;
        pos.y = 174;
        talentGradeViewCtrl.transform.localPosition = pos;
        talentGradeViewCtrl.UpdateView(singleItemCtrl.vo);
    }

    public void HideTalentGradeView()
    {
        if(talentGradeViewCtrl != null)
        {
            talentGradeViewCtrl.Hide();
        }
    }

    private void AddToDic(int id,RoleSkillTalentSingleItemController ctrl)
    {
        if (!talentItemDic.ContainsKey(id))
        {
            talentItemDic.Add(id, ctrl);
        }
    }
    private void UpdateItemArrow()
    {
        if (setFirst) return;
        setFirst = true;
        talentItemDic.ForEach(e =>
        {
            int id = e.Value.vo.cfgVO.beforeTalentId;
            if (id != 0)
            {
                if (talentItemDic.ContainsKey(id))
                {
                    var item = talentItemDic[id];
                    int nowIdx = e.Key;
                    item.UpdateArrow(nowIdx);
                }
            }
        });
    }
}
