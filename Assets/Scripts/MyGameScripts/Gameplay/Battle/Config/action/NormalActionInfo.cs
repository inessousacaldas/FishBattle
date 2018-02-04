public class NormalActionInfo : BaseActionInfo
{
	public const string TYPE = "normal";

	public float startTime; //action start time
	public float delayTime;//action delayed time

	//攻击动作是否可变化
	public bool AnimationChangeable = false;
	//（多段攻击的）攻击动作列表
	public string AttackerActions ;
	//（多段攻击的）攻击方向列表
	public string AttackerDirections;
	/**（多段攻击的）攻击时长列表*/
	public string AttackerDurations;
	static public BaseActionInfo ToBaseActionInfo(JsonActionInfo json)
	{
		NormalActionInfo info = new NormalActionInfo ();
		info.FillInfo (json);
		info.startTime = json.startTime;
		info.delayTime = json.delayTime;
		
		info.AnimationChangeable = json.AnimationChangeable;
		info.AttackerActions = json.AttackerActions;
		info.AttackerDirections = json.AttackerDirections;
		info.AttackerDurations = json.AttackerDurations;
		
		return info;
	}
}

