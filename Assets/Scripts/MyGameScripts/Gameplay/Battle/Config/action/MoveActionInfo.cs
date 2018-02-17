/*
public enum VictimTag
{
	First,//第一个受击者
	Last,//最后一个受击者
	Next,//下一个受击者
}
*/

public partial class MoveActionInfo : BaseActionInfo
{
	public const string TYPE = "move";
//public VictimTag target;//移动到的目标
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
}