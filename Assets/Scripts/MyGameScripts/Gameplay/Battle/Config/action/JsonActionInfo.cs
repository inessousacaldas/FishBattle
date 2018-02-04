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

    public BaseActionInfo ToBaseActionInfo()
    {
        BaseActionInfo info = null;
        switch (type){
            case MoveActionInfo.TYPE:
                info = MoveActionInfo.ToBaseActionInfo(this);
                break;
            case MoveBackActionInfo.TYPE:
                info = MoveBackActionInfo.ToBaseActionInfo(this);
                break;
            case NormalActionInfo.TYPE:
                info = NormalActionInfo.ToBaseActionInfo(this);
                break;
        }
        return info;
    }
}

