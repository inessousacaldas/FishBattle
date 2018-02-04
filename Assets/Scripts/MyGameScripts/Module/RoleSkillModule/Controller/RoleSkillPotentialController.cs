// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillPotentialViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View;
using Assets.Scripts.MyGameScripts.UI;
using System.Collections.Generic;
using UnityEngine;

public partial interface IRoleSkillPotentialViewController
{
}

public partial class RoleSkillPotentialViewController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        View.itemList = new List<RoleSkillPotentialItem>(6);
        for(var i = 1;i <= View.itemList.Capacity;i++)
        {
            var itemTrans = View.GetChildComponentByName<Transform>("SkillPotentialItem" + i);
            var item = BaseView.Create<RoleSkillPotentialItem>(itemTrans);
            View.itemList.Add(item);
            EventUtil.AddClick<RoleSkillPotentialItem>(itemTrans.gameObject,RoleSkillDataMgr.RoleSkillPotentialViewLogic.OnSelectItem,item);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    public void UpdateView(IRoleSkillPotentialData data)
    {
        var limitList = data.LimitList;
        int index = 0;
        limitList.ForEach(e =>
        {
            var item = View.itemList[index];
            item.SetData(e.Key, data);
            index++;
        });
        if(data.LastItem == null)
        {
            if(View.itemList[0].vo != null)
            {
                //data.LastItem = View.itemList[0];
                RoleSkillDataMgr.RoleSkillPotentialViewLogic.OnSelectItem(View.itemList[0]);
            }
        }
        else
        {
            if(data.LastItem.vo != null)
            {
                View.lblDesc_UILabel.text = data.LastItem.vo.cfgVO.desc;
                View.lblEffect_UILabel.text = data.GetEffectByLevel(data.LastItem.vo,data.LastItem.vo.Level);
                if(data.LastItem.vo.Level < data.MaxLevel)
                {
                    View.lblEffect2_UILabel.text = data.GetEffectByLevel(data.LastItem.vo,data.LastItem.vo.Level + 1);
                }
                else
                {
                    View.lblEffect2_UILabel.text = "-";
                }
            }
        }
    }

    public void UpdatePlayerInfo(IRoleSkillPotentialData data,IPlayerModel model)
    {
        if(data.LastItem != null && data.LastItem.vo != null)
        {
            var cost = data.GetCostByID(data.LastItem.vo.id);
            var silver = model.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
            //int lv = data.LastItem.vo.infoDto.grade;
            int lv2 = data.GetLevelByID(data.LastItem.vo.id);
            int serverMax = ModelManager.Player.ServerGrade + 5;
            int serverMin = ModelManager.Player.ServerGrade - 5;
            float tmpCost = 0;
            if (lv2 >= serverMax)
                tmpCost = cost * 1.5f;
            else if (lv2 <= serverMin)
                tmpCost = cost * 0.5f;
            else
                tmpCost = cost;
            
            cost = Mathf.CeilToInt(tmpCost);
            View.lblCost_UILabel.text = cost == -1 ? "-" : cost.ToString();
            View.lblHave_UILabel.text = HtmlUtil.Font2(silver.ToString(), silver >= cost ? ColorConstantV3.Color_White_Str : GameColor.RED);
            //string.Format("{0}\n{1}",(cost == -1 ? "-": cost.ToString()),HtmlUtil.Font2(silver.ToString(),silver > cost ? GameColor.MONEY_0 : GameColor.RED));

        }
    }



    protected override void OnDispose()
    {
        View.itemList.ForEach(e =>
        {
            e.DisposeClient();
        });
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        
    }

}
