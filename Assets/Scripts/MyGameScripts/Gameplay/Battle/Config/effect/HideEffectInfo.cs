public partial class HideEffectInfo : BaseEffectInfo
{
	public const string TYPE = "Hide";

    public float delayTime;

    static public BaseEffectInfo ToBaseEffectInfo(JsonEffectInfo json)
	{
        HideEffectInfo info = new HideEffectInfo();
		info.FillInfo (json);
        info.delayTime = json.delayTime;
        return info;
	}
}

