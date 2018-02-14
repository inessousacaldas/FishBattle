using AppDto;
using Fish;

public class BaseEffectInfo
{
	public string type;
	public float playTime;

	public float randomTime;
	
	public void FillInfo(JsonEffectInfo info)
	{
		type = info.type;
		playTime = info.playTime;
	}

	public virtual IBattlePlayCtl Interprete(BaseActionInfo actInfo, SkillConfigInfo skillCfg, Skill skill,
		VideoSkillAction vsAct)
	{
		return null;
	}
}