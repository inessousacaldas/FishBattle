// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillCraftsTipsController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;

public interface ICrewSkillCraftsTipsController
{
    UniRx.IObservable<Unit> OnbtnUp_UIButtonClick { get; }
    
}
public partial class CrewSkillCraftsTipsController
{
    private RoleSkillRangeController rangeCtrl;

    private ItemCellController reward1;
    private ItemCellController reward2;
    private ItemCellController reward3;
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
        base.OnDispose();
        rangeCtrl = null;
        reward1 = null;
        reward2 = null;
        reward3 = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(ICrewSkillVO data)
    {
        CrewSkillCraftsVO crafts = data as CrewSkillCraftsVO;
        if (crafts != null || crafts.cfgVO != null)
        {

            // UIHelper.SetSkillIcon(View.itemIcon_UISprite, crafts.Icon);
            UIHelper.SetUITexture(View.itemIcon_UISprite, crafts.Icon, false);
            UIHelper.SetSkillQualityIcon(View.bg_UISprite, crafts.cfgVO.quality);
            View.IsSIcon_Transform.gameObject.SetActive(crafts.IsSuperCrafts);
            View.lblGrade_UILabel.text = crafts.Grade.ToString();
            View.lblName_UILabel.text = crafts.Name;
            View.lblType_UILabel.text = "战技";
            View.lblAfter_UILabel.text = crafts.SkillTimeAfter;
            View.lblBefor_UILabel.text = crafts.SkillTimeBefore;
            View.lblCp_UILabel.text = crafts.cfgVO.consume + "CP";
            View.lblType2_UILabel.text = "[ff0000]"+crafts.SkillType+"[-]";
            View.lblScope_UILabel.text = crafts.Scope;
            int grade = CrewViewDataMgr.DataMgr.GetCurCrewGrade;
            int limit = crafts.Grade * 5 + crafts.cfgVO.playerGradeLimit;
            if (limit > grade)
                View.lblLimit_UILabel.text = "[ff0000]" + limit + "级[-]";
            else
                View.lblLimit_UILabel.text = "[272020]" + limit + "级[-]";

            View.lblEff_UILabel.text = "[272020]技能效果: [-][174181]" + RoleSkillUtils.Formula(crafts.SkillDes, crafts.Grade) + "[-]";
            //View.lblEff_UILabel.text = "技能效果: " + crafts.SkillDes;
            UpdateRightRange(crafts);
            CrewCraftsGrade cost = CrewSkillDataMgr.DataMgr.CraftsData.GetCostByGradeDto(crafts);
           
            if (cost.silver > 0)
            {
                var item = DataCache.getDtoByCls<GeneralItem>((int)AppVirtualItem.VirtualItemEnum.SILVER);
                if(reward3 == null)
                {
                    reward3 = AddChild<ItemCellController, ItemCell>(View.rewardTrans_Transform.gameObject, ItemCell.Prefab_ItemUseCell);
                    int count =  (int)ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
                    reward3.UpdateViewInCrewSkill(item, count, cost.silver);
                }
                reward3.Show();
            }
            else
            {
                if (reward3 != null)
                {
                    reward3.View.Hide();
                }
            }

            if (cost.itemCount1 > 0)
            {
                if (reward1 == null)
                {
                    reward1 = AddChild<ItemCellController, ItemCell>(
                        View.rewardTrans_Transform.gameObject,
                        ItemCell.Prefab_ItemUseCell
                    );
                }
                reward1.Show();
                reward1.UpdateViewInCrewSkill(crafts.cfgVO.item1, BackpackDataMgr.DataMgr.GetItemCountByItemID(crafts.cfgVO.consumeBook1), cost.itemCount1);
            }
            else
            {
                if (reward1 != null)
                {
                    reward1.View.Hide();
                }
            }

            if (cost.itemCount2 > 0)
            {
                if (reward2 == null)
                {
                    reward2 = AddChild<ItemCellController, ItemCell>(
                        View.rewardTrans_Transform.gameObject,
                        ItemCell.Prefab_ItemUseCell
                    );
                }
                reward2.Show();
                reward2.UpdateViewInCrewSkill(crafts.cfgVO.item2, BackpackDataMgr.DataMgr.GetItemCountByItemID(crafts.cfgVO.consumeBook2), cost.itemCount2);
            }
            else
            {
                if (reward2 != null)
                {
                    reward2.View.Hide();
                }
            }
        }
        View.rewardTrans_Transform.enabled = true;
    }

    private void UpdateRightRange(CrewSkillCraftsVO craftsVo)
    {
        CrewSkillCraftsData tmp = CrewSkillHelper.GetCraftsData();
        var scopeVO = tmp.GetScopeByID(craftsVo.cfgVO.scopeId);
        var type = tmp.GetScopeTarType(craftsVo.cfgVO.scopeId);
        if (rangeCtrl == null)
        {
            rangeCtrl = RoleSkillRangeController.Show(View.rangeTrans_Transform.gameObject, scopeVO.scopeIndex, type);
            rangeCtrl.transform.localPosition = new UnityEngine.Vector3(0, 0);
            rangeCtrl.transform.localScale = UnityEngine.Vector3.one;
        }
        else
        {
            rangeCtrl.Show(scopeVO.scopeIndex, type);
        }
    }

    public bool IsShow
    {
        get { return View.gameObject.activeSelf; }
    }
}
