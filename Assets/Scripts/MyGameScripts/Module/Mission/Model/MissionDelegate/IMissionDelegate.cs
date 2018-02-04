using UnityEngine;
using System.Collections;
using AppDto;

/// <summary>
///  增加 IMissionDelegate 后要 MissionDelegateFactory
/// </summary>
public interface IMissionDelegate {

    /// <summary>
    /// 接受任务,处理判断某个 MissionType 任务能不能被接受
    /// 比如等级未到,活动任务未到活动时间等
    /// </summary>
    /// <param name="MissionId"></param>
    /// <returns></returns>
    bool AcceptMission();
    /// <summary>
    /// 不同的 MissionType 在接受任务后,会有不同的操作
    /// </summary>
    /// <param name="dto"></param>
    void AcceptMissionFinish(PlayerMissionDto dto);

    void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto);
    /// <summary>
    /// 完成任务后,会根据 MissionType 进行表现
    /// 涉及到巡逻任务的,要在这里调用 StopAutoFram
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="submitDto"></param>
    void FinishMission(PlayerMissionDto dto,SubmitDto submitDto);

    /// <summary>
    /// 根据任务寻路,返回 true 有处理寻路逻辑,返回 False 没有处理寻路逻辑
    /// 这个接口和 ISubmitDelegate.FindToNpc 相结合,处理任务寻路
    /// </summary>
    /// <param name="mission"></param>
    /// <returns></returns>
    bool FindToNpc(Mission mission);

    /// <summary>
    /// 放弃任务
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="winTips">该返回不为 string.Empty 时候,需要弹出对话框确认</param>
    /// <returns>True可以放弃任务,false不能放弃任务</returns>
    bool DropMission(Mission mission,out string winTips);
    void Dispose();
}
