using System.Collections.Generic;

public class JsonSkillConfigInfo
{
	public int id = 0;
	public string name = "";
	
	public List<JsonActionInfo> attackerActions = new List<JsonActionInfo>();
	public List<JsonActionInfo> injurerActions = new List<JsonActionInfo>();

	public SkillConfigInfo ToSkillConfigInfo()
	{
		SkillConfigInfo info = new SkillConfigInfo ();
		info.id = this.id;
		info.name = this.name;

		info.attackerActions = ToBaseActionInfoList (attackerActions);
		info.injurerActions = ToBaseActionInfoList (injurerActions);

		return info;
	}

	private List<BaseActionInfo> ToBaseActionInfoList(List<JsonActionInfo> jsonList)
	{
		List<BaseActionInfo> list = new List<BaseActionInfo> ();

		for (int i=0,len=jsonList.Count; i<len; i++)
		{
			JsonActionInfo json = jsonList[i];
			list.Add(json.ToBaseActionInfo());
		}

		return list;
	}
}