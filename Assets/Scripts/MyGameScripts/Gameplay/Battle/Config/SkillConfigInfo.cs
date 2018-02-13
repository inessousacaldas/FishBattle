using System.Collections.Generic;
using Newtonsoft.Json;

public class SkillConfigInfo
 {
 	public int id = 0;
 	public string name = "";
 	
 	[JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
 	public List<BaseActionInfo> attackerActions;
 	[JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
 	public List<BaseActionInfo> injurerActions;	
 }