using System.Collections.Generic;
using Newtonsoft.Json;

public enum ActionInitiator
{
	Attacker,//攻击者
	Victim,//受击者
	//Pet,//宠物
}

//动作模板实例化模式
public enum ActionSchemeInstantiationMode
{
	Seq,//按服务器发送目标的顺序串联
	Par,//多目标并联
	//LastVictim,//上一个受击者
}

public partial class BaseActionInfo
{
	public string type;//动作类型
	public ActionInitiator initiator;//执行动作的对象：攻击者，受击者，宠物
	public ActionSchemeInstantiationMode instMode;//动作模板实例化模式
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
