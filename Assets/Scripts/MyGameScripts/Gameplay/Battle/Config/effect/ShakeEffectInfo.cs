//using UnityEngine;

using UnityEngine;

public partial class ShakeEffectInfo : BaseEffectInfo
{
    public const string TYPE = "Shake";

    public float duaration;
    // 强度
    public Vector3 intensity;
    // 是否击中
    public bool isHit;
    /**第几次攻击或第几个受击，-1为每次攻击或受击*/
    public int PlayIndex;

    static public BaseEffectInfo ToBaseEffectInfo(JsonEffectInfo json)
    {
        ShakeEffectInfo info = new ShakeEffectInfo();
        info.FillInfo(json);
        info.duaration = json.delayTime;
        info.intensity = json.intensity;
        info.isHit = json.isHit;
        info.PlayIndex = json.PlayIndex;
        return info;
    }
}