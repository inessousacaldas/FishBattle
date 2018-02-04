using System;

/// <summary>
/// 多动作多方向战斗组件
/// 配置方式：
///   仅需配置 BattleConfig.bytes。目标技能的攻击者行动中，normal动作里选中“攻击动作可变”（AnimationChangeable为true），设置“动作列表”（AttackerActions），设置“方向列表”（AttackerDirections）即可；
///     如需配置每段攻击的持续时长，增强打击感，请设置“时长列表”（AttackerDurations）
/// 注意：技能表中对应技能的攻击次数要跟技能编辑里设置的动作列表中的动作的数目一致。
/// Redmine单地址：
///   http://oa.cilugame.com/redmine/issues/18374
/// @MarsZ 2017年07月11日10:35:58
/// </summary>
public class BattleMultiAnimaAttackerCMPT
{
    /**多段受击中受击者受击动作等播放时长，太长没有打击感*/
    public const float MIDDLE_INJURE_DURATION = 0.1f;

    private string[] mAttackerActions;
    private string[] mAttackerDirections;
    private string[] mAttackerDurations;

    public bool NeedCutMiddleAnimation;
    public bool AttackerIdleStateEnable;

    private int mSkillId;

    public void PreUpdateBaseActionInfo(NormalActionInfo pNormalActionInfo, int pSkillID)
    {
        Dispose();
        mSkillId = pSkillID;
        if (null == pNormalActionInfo || !pNormalActionInfo.AnimationChangeable)
            return;
        UnityEngine.Assertions.Assert.IsNotNull(pNormalActionInfo.AttackerActions, string.Format("攻击动作可变的技能({0})中配置的动作列表 AttackerActions 不得为空！", mSkillId));
        UnityEngine.Assertions.Assert.IsNotNull(pNormalActionInfo.AttackerDirections, string.Format("攻击动作可变的技能({0})中配置的方向列表 AttackerDirections 不得为空！", mSkillId));
        UnityEngine.Assertions.Assert.IsNotNull(pNormalActionInfo.AttackerDurations, string.Format("攻击动作可变的技能({0})中配置的时长列表 AttackerDurations 不得为空！", mSkillId));
        mAttackerActions = pNormalActionInfo.AttackerActions.Split(',');
        mAttackerDirections = pNormalActionInfo.AttackerDirections.Split(',');
        mAttackerDurations = pNormalActionInfo.AttackerDurations.Split(',');


        NeedCutMiddleAnimation = null != mAttackerActions && mAttackerActions.Length > 1;
    }

    public void UpdateAttackerAction(
        MonsterController pAttackerMC
        , long pInjurerId
        , NormalActionInfo node
        , int pCurrentFinishRound
        ,bool pLastAction
        , out ModelHelper.AnimType pSuitableAnimation
        , out float pActionDuration)
    {
        //GameDebuger.LogError(string.Format("UpdateAttackerAction pAttackerMC:{0}, pInjurerId:{1}, node:{2}, pCurrentFinishRound:{3}", pAttackerMC, pInjurerId, node, pCurrentFinishRound));
        pSuitableAnimation = ModelHelper.AnimType.invalid;
        pActionDuration = 0;
        AttackerIdleStateEnable = true;
        if (null == node || null == pAttackerMC)
            return;
        pActionDuration = node.delayTime;
        pSuitableAnimation = EnumParserHelper.TryParse(node.name, ModelHelper.AnimType.battle) ;

//        if (pSuitableAnimation == ModelHelper.Anim_randomAttack)
//            pSuitableAnimation = ModelDisplayController.AttackAnimationClipList.Random();

        if (pInjurerId <= 0 || null == mAttackerActions || mAttackerActions.Length <= 0)
            return;
        int tActionDirection = (int)UIWidget.Pivot.Right;
        
        var temp = GetAttackerAnimation(node, pCurrentFinishRound);
        temp = string.IsNullOrEmpty(temp) ? node.name : temp;
        pSuitableAnimation = EnumParserHelper.TryParse(temp, ModelHelper.AnimType.battle) ;
        
        tActionDirection = GetAttackerDirection(node, pCurrentFinishRound);
        pActionDuration = GetAttackerDuration(node, pCurrentFinishRound);

        UpdateMCPositionByDirection(pAttackerMC, pInjurerId, tActionDirection);
        AttackerIdleStateEnable = pLastAction || pCurrentFinishRound == mAttackerDurations.Length - 1 ;
    }

    private string GetAttackerAnimation(NormalActionInfo node, int pCurrentFinishRound)
    {
        if (mAttackerActions.Length <= pCurrentFinishRound)
        {
//            GameDebuger.LogError(string.Format("[Error]攻击动作可变的技能({0})中配置的动作的数目跟实际攻击次数不匹配，使用默认动作{1}！mCurrentFinishRound:{2}", 
//                    mSkillId, ModelHelper.Anim_attack1.ToString(), pCurrentFinishRound));
            return ModelHelper.AnimToString(ModelHelper.AnimType.attack);
        }
        return mAttackerActions[pCurrentFinishRound];
    }

    private int GetAttackerDirection(NormalActionInfo node, int pCurrentFinishRound)
    {
        if (mAttackerDirections.Length <= pCurrentFinishRound)
        {
            GameDebuger.LogError(string.Format("[Error]攻击动作可变的技能({0})中配置的方向的数目跟实际攻击次数不匹配，使用默认方向{1}！mCurrentFinishRound:{2}", mSkillId, "1", pCurrentFinishRound));
            return (int)UIWidget.Pivot.Right;
        }
        return mAttackerDirections[pCurrentFinishRound].ToInt();
    }

    private float GetAttackerDuration(NormalActionInfo node, int pCurrentFinishRound)
    {
        if (mAttackerDurations.Length <= pCurrentFinishRound)
        {
            GameDebuger.LogError(string.Format("[Error]攻击动作可变的技能({0})中配置的时长的数目跟实际攻击次数不匹配，使用默认时长{1}！mCurrentFinishRound:{2}", mSkillId, "0", pCurrentFinishRound));
            return 0f;
        }
        return mAttackerDurations[pCurrentFinishRound].ToFloat();
    }

    private void UpdateMCPositionByDirection(MonsterController pAttackerMC, long pInjurerId, int pActionDirection)
    {
        pAttackerMC.UpdateMCPositionByDirection(BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(pInjurerId), (ModelDirection)pActionDirection);
    }

    public void Dispose()
    {
        mSkillId = 0;

        NeedCutMiddleAnimation = false;
        AttackerIdleStateEnable = true;

        if (null != mAttackerActions)
            mAttackerActions = null;
        if (null != mAttackerDirections)
            mAttackerDirections = null;
        if (null != mAttackerDurations)
            mAttackerDurations = null;
    }
}

/// <summary>
/// 模型方向
/// </summary>
public enum ModelDirection
{
    Right = 0,
    Top = 1,
    Left = 2,
    Bottom = 3
}