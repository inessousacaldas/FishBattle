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
}