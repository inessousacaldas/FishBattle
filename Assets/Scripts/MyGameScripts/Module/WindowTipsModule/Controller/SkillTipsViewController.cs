using UnityEngine;
using AppDto;
/// <summary>
/// 技能提示
/// </summary>
public class SkillTipsViewController : MonoViewController<SkillTipsView>
{
    private const int AddSpace = 150;

    protected override void RegistCustomEvent()
    {
        UICamera.onClick += ClickEventHandler;
    }

    protected override void OnDispose()
    {
        UICamera.onClick -= ClickEventHandler;
    }

    /// <summary>
    /// Show the specified skillObj and anchor. 
    /// support ScenarioSkill or AssistSkillVo or Skill
    /// </summary>
    /// <param name="skillObj">Skill object.</param>
    /// <param name="anchor">Anchor.</param>
    public void Show(object skillObj, GameObject anchor, string extText = "")
    {
		

        string skillName = "";
        string skillDesc = "";
        string skillIcon = "0";

        GameDebuger.TODO(@"if (skillObj is ScenarioSkill)
        {
            ScenarioSkill scenarioSkill = skillObj as ScenarioSkill;
            skillName = scenarioSkill.name;
            skillDesc = scenarioSkill.memo;
            skillIcon = scenarioSkill.icon;
        }
        else if (skillObj is AssistSkillVo)
        {
            AssistSkillVo assistSkill = skillObj as AssistSkillVo;
            skillName = assistSkill.name;
            skillDesc = assistSkill.memo;
            skillIcon = assistSkill.icon;
        }
        else"); 
        if (skillObj is Skill)
        {
            Skill skillInfo = skillObj as Skill;
            skillName = skillInfo.name;
            skillDesc = skillInfo.shortDescription;
            skillIcon = skillInfo.icon;
        }
        else
        {
            Debug.LogError("SkillTipsViewController.show don't support " + skillObj.ToString());
        }
        //TO UP
        GameDebuger.TODO(@"else if (skillObj is CrewSkillInfo)
        {
            CrewSkillInfo crewSkill = skillObj as CrewSkillInfo;
            skillName = crewSkill.skill.name;
            skillDesc = crewSkill.skill.crewDescription;
            skillDesc = System.Text.RegularExpressions.Regex.Replace(skillDesc, @'\w+等级达到\[Lv\]级', string.Format('需要伙伴达到{0}级', crewSkill.acquireLevel));
            skillIcon = crewSkill.skill.icon;
        }
        else if (skillObj is RidePassiveNotify)     //坐骑通用被动技能 (为空报错，完成通用技能学习，在修复）
        {
            RidePassiveNotify ridePassiveSkill = skillObj as RidePassiveNotify;
            skillName = ridePassiveSkill.passiveSkill.name;
            skillDesc = ridePassiveSkill.passiveSkill.description;
            skillIcon = ridePassiveSkill.passiveSkill.icon;
        }
        else if (skillObj is RideFactionPassiveNotify) //坐骑门派被动技能
        {
            RideFactionPassiveNotify rideFactionPassiveSkill = skillObj as RideFactionPassiveNotify;
            MountFactionPassiveSkill skill = ModelManager.Mount.GetMountFactionPassiveSkillDic()[rideFactionPassiveSkill.passiveSkillId];
            skillName = skill.name;
            skillDesc = skill.description;
            skillIcon = skill.icon;
        }
        else if (skillObj is FactionSkill)
        {
            FactionSkill factionSkill = skillObj as FactionSkill;
            skillName = factionSkill.name;
            skillDesc = ModelManager.FactionSkill.GetFactionShowTips(factionSkill);
            skillIcon = ModelManager.FactionSkill.GetFactionSkillIcon(factionSkill);
        }");

        View.NameLabel.text = skillName;
        View.ExtLabel.text = extText;
        View.SkillDescriptionLbl.text = skillDesc;
        UIHelper.SetSkillIcon(View.IconSprite, skillIcon);

        View.ContentBg.height = AddSpace + View.SkillDescriptionLbl.height;
		
        View.posAnchor.container = anchor;
        View.posAnchor.pixelOffset = new Vector2(-View.ContentBg.width / 2, 0);
        View.posAnchor.Update();
		
        UIPanel panel = UIPanel.Find(View.ContentBg.cachedTransform);
        panel.ConstrainTargetToBounds(View.ContentBg.cachedTransform, true);
    }

//    public void Show(SkillInfo skillInfo, int lv, bool isLearned, GameObject anchor)
//    {
//        View.NameLabel.text = skillInfo.skill.name + (isLearned ? "" : "[FF0000]（未掌握）[-]");
//        if (isLearned)
//            View.SkillDescriptionLbl.text = skillInfo.skill.description.Replace("[Lv]", lv.ToString());
//        else
//            View.SkillDescriptionLbl.text = skillInfo.skill.description.Replace("[Lv]", lv.ToString().WrapColor(ColorConstantV3.Color_Red_Str));
//
//        UIHelper.SetSkillIcon(View.IconSprite, skillInfo.skill.icon.ToString());
//
//        View.ContentBg.height = AddSpace + View.SkillDescriptionLbl.height;
//
//        View.posAnchor.container = anchor;
//        View.posAnchor.pixelOffset = new Vector2(-View.ContentBg.width / 2, View.ContentBg.height - 20);
//        View.posAnchor.Update();
//
//        UIPanel panel = UIPanel.Find(View.ContentBg.cachedTransform);
//        panel.ConstrainTargetToBounds(View.ContentBg.cachedTransform, true);
//    }

    private void ClickEventHandler(GameObject go)
    {
        CloseView();
    }

    private void CloseView()
    {
        ProxyTipsModule.CloseSkillTips();
    }
}
