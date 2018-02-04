public class ShowBGTextureEffectInfo: BaseEffectInfo
{
    public const string TYPE = "BGIMG";

    /**背景贴图显示持续时间，时间到后会还原为原来的状态*/
    public float delayTime;
    /**要显示的背景贴图名字，必须是Assets/GameResources/ArtResources/Images/BattleBG中的texture的名字*/
    public string bgTexture;

    static public BaseEffectInfo ToBaseEffectInfo(JsonEffectInfo json)
    {
        ShowBGTextureEffectInfo info = new ShowBGTextureEffectInfo();
        info.FillInfo(json);
        info.delayTime = json.delayTime;
        info.bgTexture = json.bgTexture;
        return info;
    }
}