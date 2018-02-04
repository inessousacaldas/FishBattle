using System.Collections.Generic;

/// <summary>
/// 战斗自动挂机设置数据
/// @MarsZ 2017-04-01 17:38:28
/// </summary>
public class BattleAutoFightConfigInfo
{
    //超时自动挂机
    public bool AutoFightIfOutTime = false;
    //自动战斗超时时间
    public int AutoFightTime = 30;
    //是否使用候补技能
    public bool UseSubSkill = false;
    //主角自动技能
    public int MainRoleAutoSkill;
    //主将自动技能
    public int MainPetAutoSkill;
    //主角候补技能
    public int MainRoleSubSkill;
    //主将候补技能
    public int MainPetSubSkill;
    //自动召唤武将列表
    public List<long> AutoSummonHeroList;
    //是否跟随集火
    public bool AutoTarget;
}