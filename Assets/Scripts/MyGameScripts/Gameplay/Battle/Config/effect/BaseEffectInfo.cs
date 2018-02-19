public partial class BaseEffectInfo
{
	public string type;
	public float playTime;

	public float randomTime;
	
	public void FillInfo(JsonEffectInfo info)
	{
		type = info.type;
		playTime = info.playTime;
	}
}