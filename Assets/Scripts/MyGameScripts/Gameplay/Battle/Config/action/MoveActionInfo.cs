using System.Collections.Generic;
using AppDto;
using Fish;

public class MoveActionInfo : BaseActionInfo
{
	public const string TYPE = "move";

	public float time;
	public float distance;
	public bool center;

	public override void FillInfo(JsonActionInfo json)
	{
		base.FillInfo(json);
		time = json.time;
		distance = json.distance;
		center = json.center;		
	}

	public override IBattlePlayCtl Interprete(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
	{
		var allEff = new List<IBattlePlayCtl>();
		for (var i = 0; i < effects.Count; i++)
		{
			var eff = effects[i];
			var effCtl = eff.Interprete(this,skillCfg,skill,vsAct);
			if (effCtl == null) continue;
			allEff.Add(effCtl);
		}
		var palEff= ParallCompositePlayCtl.Create(allEff);
		//TODO
		return null;
	}
}