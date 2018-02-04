using System;
using UnityEngine;
using AppDto;

/// <summary>
/// 各种提示
/// @MarsZ 2017-03-25 18:08:32
/// </summary>
public static class ProxyTipsModule
{
    #region 技能提示

    /// <summary>
    /// Shows the tips.
    /// support ScenarioSkill or AssistSkillVo or Skill or skillId
    /// </summary>
    /// <param name="skillObj">Skill object.</param>
    /// <param name="anchor">Anchor.</param>
    public static void ShowSkillTips(object skillObj, GameObject anchor, string extText = "")
    {
        GameObject view = UIModuleManager.Instance.OpenFunModule(SkillTipsView.NAME, UILayerType.FiveModule, false);
        var controller = view.GetMissingComponent<SkillTipsViewController>();
        controller.Show(skillObj, anchor, extText);
    }

    //    public static string SkillId2TypeText(int type)
    //    {
    //        string text = "";
    //        if (type == Skill.PetSkillTypeEnum_Low)
    //            text = "低级技能";
    //        else if (type == Skill.PetSkillTypeEnum_High)
    //            text = "高级技能";
    //        else if (type == Skill.PetSkillTypeEnum_Special)
    //            text = "特殊技能";
    //        else if (type == Skill.PetSkillTypeEnum_Talent)
    //            text = "天赋技能";
    //
    //        if (!string.IsNullOrEmpty(text))
    //            text = "类型：" + text;
    //        return text;
    //    }

    public static void ShowSkillTips(int skillId, GameObject anchor)
    {
        Skill skill = DataCache.getDtoByCls<Skill>(skillId);
        if (skill != null)
        {
            GameDebuger.TODO(@"if (skill is PetSkill)
            {
                PetSkill petSkill = skill as PetSkill;
                if (petSkill == null)
                    return;
                ShowSkillTips(skill, anchor, SkillId2TypeText(petSkill.type));
            }
            else");
            {
                ShowSkillTips(skill, anchor);
            }
        }
    }

    //    public static void ShowSkillTips(SkillInfo skillInfo, int lv, bool isLearned, GameObject anchor)
    //    {
    //        GameObject view = UIModuleManager.Instance.OpenFunModule(SkillTipsView.NAME, UILayerType.FiveModule, false);
    //        var controller = view.GetMissingComponent<SkillTipsViewController>();
    //
    //        controller.Show(skillInfo, lv, isLearned, anchor);
    //    }

    public static void CloseSkillTips()
    {
        UIModuleManager.Instance.CloseModule(SkillTipsView.NAME);
    }

    #endregion
    
    public static void ShowConsumerTips(
        ConsumerTipsViewData data
        , Action<bool> confirmCB
        , Action<bool> cancelCB)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<ConsumerTipsViewController>(
            ConsumerTipsView.NAME
            , UILayerType.Dialogue
            , false);
        
        ctrl.UpdateView(data);
        ctrl.optCallback = confirmCB;
        ctrl.cancelCallback = confirmCB;
    }
}