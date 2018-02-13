using System.Collections.Generic;
using Newtonsoft.Json;

public class BattleConfigInfo
{
	public string time = "";
	[JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
	public List<SkillConfigInfo> list;
}