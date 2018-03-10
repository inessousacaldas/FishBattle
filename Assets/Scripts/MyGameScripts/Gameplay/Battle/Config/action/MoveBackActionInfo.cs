public partial class MoveBackActionInfo : BaseActionInfo
{
	public const string TYPE = "moveBack";
	
	public float time; //move'speed


	public override void FillInfo(JsonActionInfo info)
	{
		base.FillInfo(info);
		time = info.time;
	}
}

