using AppDto;
using System;

public class DemoSkillHelper
{
    public static int GetSkillActionPoint(Skill pSkill)
    {
        return 0;
//        return null != pSkill ? pSkill.actionPoint : 0;
    }

    public static string GetSkillActionPointLimitTip(long pMonsterUID, int tSkillId)
    {
        var tSkill = DataCache.getDtoByCls<Skill>(tSkillId);
        tSkillId = tSkillId < 10 ? (tSkillId + 3000) : tSkillId;
        var limitTip = "";
//        if (!BattleDataManager.DataMgr.BattleDemo.IsSkillPointEnoughToUseSkill(pMonsterUID, tSkill))
//            limitTip = string.Format("需要{0}技能点", tSkill.actionPoint);
        return limitTip;
    }

    public static string GetSkillActionPointLimitTip(long pMonsterUID, Skill pSkill)
    {
        var limitTip = "";
        if (null != pSkill)
        {
//            if (!BattleDataManager.DataMgr.BattleDemo.IsSkillPointEnoughToUseSkill(pMonsterUID, pSkill))
//                limitTip = string.Format("需要{0}技能点", pSkill.actionPoint); 
        }
        return limitTip;
    }

    public static int GetSkillSP(VideoSoldier pVideoSoldier, int tSkillId)
    {
        var tSkill = DataCache.getDtoByCls<Skill>(tSkillId);
        return tSkill == null ? 0 : tSkill.consume;
    }

    public static string GetSkillSPLimitTip(MonsterController pMonsterController, Skill pSkill)
    {
        var limitTip = string.Empty;

        if (null == pSkill || pSkill.consume == 0) return limitTip;
        var value = pSkill.consume;
//        value = (int)((float)value * pMonsterController.videoSoldier.spendSpDiscountRate);
        if (pMonsterController.currentCp < Math.Abs(value))
        {
            limitTip = string.Format("需要{0}愤怒", Math.Abs(value));
        }

        return limitTip;
    }

    public static string GetVideoRoundSkillShortDesc(VideoRound pVideoRound)
    {
        if (null == pVideoRound)
            return string.Empty;
        if (null == pVideoRound.skillActions || pVideoRound.skillActions.Count <= 0)
        {
            GameDebuger.LogError(string.Format("[错误]战斗回合数据 VideoRound 有误，VideoRound.skillActions 长度{0}问题！", (pVideoRound.skillActions != null) ? "有" : "无"));
            return string.Empty; 
        }
        var tVideoSkillAction = pVideoRound.skillActions[0];

        if (null == tVideoSkillAction || null == tVideoSkillAction.skill)
            return string.Empty;
        return tVideoSkillAction.skill.name;
    }

    public static bool IsSuperSkill(Skill pSkill)
    {
        GameDebuger.LogError("[TEMP]是否奥义技能");
        return null != pSkill && pSkill.id % 3 == 0;
    }

    public static bool IsSuperSkill(int pSkillId)
    {
        var tSkill = DataCache.getDtoByCls<Skill>(pSkillId);
        return IsSuperSkill(tSkill);
    }
}