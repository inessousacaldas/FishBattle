using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AppDto;
using System;

/// <summary>
/// MissionDelegateFactory 享元工厂,增加 IMissionDelegate 后要在此注册
/// </summary>
public class MissionDelegateFactory{
    private Dictionary<int,IMissionDelegate> _missionDelegates;
    private IMissionDelegate _defaultMissionDelegate;

    public void Setup(MissionDataMgr.MissionData missionModel) {
        RegisterMissionDelegate(missionModel);
    }


    public void Dispose() {
        if(_missionDelegates != null) {
            foreach(IMissionDelegate missionDelegate in _missionDelegates.Values) {
                missionDelegate.Dispose();
            }
            _missionDelegates.Clear();
            _missionDelegates = null;
        }
    }

    public IMissionDelegate GetHandleMissionDelegate(int missionType) {
        IMissionDelegate missionDelegate;
        if(_missionDelegates.ContainsKey(missionType))
        {
            missionDelegate = _missionDelegates[missionType];
        }
        else {
#if UNITY_EDITOR
            MissionType tmp = DataCache.getDtoByCls<MissionType>(missionType);
            if(tmp != null)
            {
                GameDebuger.LogError(string.Format("该任务类型没有对应的 MissionDelegate Type:{0},Name:{1},使用默认的处理方法",missionType,tmp.name));
            }
#endif
            missionDelegate = _defaultMissionDelegate;
        }
        return missionDelegate;
    }

    private void RegisterMissionDelegate(MissionDataMgr.MissionData missionModel) {
        _defaultMissionDelegate = new DefaultMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.None);
        _missionDelegates = new Dictionary<int,IMissionDelegate>();
        Action<BaseMissionDelegate> addMissionDelegate = (missionDelegate) => {
            _missionDelegates.Add(missionDelegate.GetMissionType(),missionDelegate as IMissionDelegate);
        };

        addMissionDelegate(new DefaultMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Master));
        addMissionDelegate(new DefaultMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Branch));
        addMissionDelegate(new FactionMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Faction));
        addMissionDelegate(new GhostMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Ghost));
        addMissionDelegate(new TreasuryMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Treasury));
        addMissionDelegate(new UrgentMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Urgent));
        addMissionDelegate(new CopyMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Copy));
        addMissionDelegate(new CopyExtraMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.CopyExtra));
        addMissionDelegate(new GuildMissionDelegate(missionModel,(int)MissionType.MissionTypeEnum.Guild));
    }
}
