using UnityEngine;
using System.Collections;
using AppDto;


public class MissionOption  {
    private Mission _mission=null;
    public Mission mission {
        get { return _mission; }
    }


    private bool _isExits=false;    ////该任务是否已接
    public bool isExis
    {
        get { return _isExits; }
    }


    public MissionOption(Mission mission,bool isExits)
    {
        _mission = mission;
        _isExits = isExits;
    }
}
