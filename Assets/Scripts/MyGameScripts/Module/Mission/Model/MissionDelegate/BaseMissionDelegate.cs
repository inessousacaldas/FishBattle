using AppDto;
using System.Collections.Generic;

public class BaseMissionDelegate{
    protected MissionDataMgr.MissionData _model;
    protected MissionType _missionType;
    protected List<int> _repeatTypes;
    private static Dictionary<int,List<int>> missionTypeToRepeatTypes;

    // 客户端处理任务是根据 MissionType 处理的,服务器处理任务是根据 Mission.repeatType 处理的,这里做个映射关系
    public BaseMissionDelegate(MissionDataMgr.MissionData model,int missionType) {
        _model = model;
        _missionType = DataCache.getDtoByCls<MissionType>(missionType);
        //_repeatTypes = 
    }


    /// <summary>
    /// 返回任务TypeID
    /// </summary>
    /// <returns></returns>
    public int GetMissionType() {
        if(_missionType != null)
            return _missionType.id;
        return (int)MissionType.MissionTypeEnum.None;
    }

    /// <summary>
    /// 这里是为了避免每一个 BaseMissionDelegate 的子类都遍历一次任务
    /// </summary>
    /// <param name="missintType"></param>
    /// <returns></returns>
    //public List<int> MissionTypeToRepeatTypes(int missionType) {
    //    if(missionTypeToRepeatTypes == null) {
    //        missionTypeToRepeatTypes = new Dictionary<int,List<int>>();
    //        List<Mission> missions = DataCache.getArrayByCls<Mission>();
    //        for(int i = 0;i<missions.Count;i++) {
    //            if(missionTypeToRepeatTypes.ContainsKey(missions[i].type)) {
    //                if(missionTypeToRepeatTypes[missions[i].type].IndexOf(missions[i].rewards))
    //            }
    //        }
    //    }
    //}
}
