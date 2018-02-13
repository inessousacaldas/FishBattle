using System.Collections.Generic;
using Newtonsoft.Json;

public class BaseActionInfo
{
	public string type;//动作类型
	public string name; // 播放动作名//
	public int rotateX;
	public int rotateY;
	public int rotateZ;
	
	/**锚点跟随动作移动*/
	public bool mountFollow;
	
	[JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
	public List<BaseEffectInfo> effects;

	public virtual void FillInfo(JsonActionInfo info)
	{
		type = info.type;
		name = info.name;
		rotateX = info.rotateX;
		rotateY = info.rotateY;
		rotateZ = info.rotateZ;

		effects = ToBaseActionInfoList (info.effects);
	}

	private List<BaseEffectInfo> ToBaseActionInfoList(List<JsonEffectInfo> jsonList)
	{
		List<BaseEffectInfo> list = new List<BaseEffectInfo> ();

		for(int i=0,len=jsonList.Count; i<len; i++)
		{
			JsonEffectInfo json = jsonList[i];
			list.Add(json.ToBaseEffectInfo());
		}
		
		return list;
	}
}
