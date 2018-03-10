using System;
using System.Collections.Generic;

public class JsonActionInfo
{
	#region BaseActionInfo
	public string type;//动作类型
	public string name; // 播放动作名//
	public int rotateX;
	public int rotateY;
	public int rotateZ;
	#endregion

	#region MoveActionInfo
	public float time;
	public float distance;
	public bool center;
	#endregion

	#region MoveBackActionInfo
	//public float time; //move'speed
	#endregion

	#region MoveBackActionInfo
	public float startTime;//action start time
	public float delayTime;//action delayed time
	
	//攻击动作是否可变化
	public bool AnimationChangeable = false;
	//（多段攻击的）攻击动作列表
	public string AttackerActions ;
	//（多段攻击的）攻击方向列表
	public string AttackerDirections ;
	/**（多段攻击的）攻击时长列表*/
	public string AttackerDurations;
	#endregion

	public List<JsonEffectInfo> effects = new List<JsonEffectInfo>();

    /// <summary>
    /// 如果 type 存的是类名的话，这玩意也可以省掉
    /// </summary>
    private static readonly Dictionary<string, Type> ActionInfoExchangeDict = new Dictionary<string, Type>()
    {
        {MoveActionInfo.TYPE, typeof(MoveActionInfo) },
        {MoveBackActionInfo.TYPE, typeof(MoveBackActionInfo) },
        {NormalActionInfo.TYPE, typeof(NormalActionInfo) },
    };


    public BaseActionInfo ToBaseActionInfo()
    {
        Type realType;
        BaseActionInfo info = null;
        if (ActionInfoExchangeDict.TryGetValue(type, out realType))
        {
            info = (BaseActionInfo)Activator.CreateInstance(realType);
	        //bug: FillInfo must be virtual to handle different inherit types!
            info.FillInfo(this);
        }
        return info;
    }
}

