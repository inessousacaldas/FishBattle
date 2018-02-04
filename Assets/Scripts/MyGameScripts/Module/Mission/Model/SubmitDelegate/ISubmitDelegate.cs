using UnityEngine;
using System.Collections;
using AppDto;

/// <summary>
///  增加 ISubmitDelegate 后要 SubmitDelegateFactory
/// </summary>
public interface ISubmitDelegate {
    /// <summary>
    /// 获取 submitDto 相关的npc
    /// </summary>
    /// <param name="submitDto"></param>
    /// <param name="isGetSubmitNpc"></param>
    /// <returns></returns>
    NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc);

    /// <summary>
    /// 判断是否需要延迟到战斗后处理数据,默认是 true
    /// 目前只有收集类物品,战斗中通过摆摊购买or战斗获得时候,而且是"非完成状态下(SubmitDto.finish = fals)",才允许为 false
    /// </summary>
    /// <returns></returns>
    bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify);

    /// <summary>
    /// 完成条件更新,任务进行中
    /// submitDto.count change and submitDto.count smaller than submitDto.needCount
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="submitDto"></param>
    void SubmitConditionUpdate(Mission mission,SubmitDto submitDto);

    /// <summary>
    /// 完成条件达到,但是任务还没提交
    /// submitDto.count change and submitDto.count == submitDto.needCount and submitDto.finish == false
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="submitDto"></param>
    void SubmitConditionReach(Mission mission,SubmitDto submitDto);

    /// <summary>
    /// 完成条件已经完成
    /// submitDto.ount change and submitDto.count == submitDto.needCount and submitDto.finish == true
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="submitDto"></param>
    void SubmitConditionFinish(Mission mission,SubmitDto submitDto);

    /// <summary>
    /// 尝试完 Mission 的 SubmitDto 条件
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="submitDto"></param>
    /// <param name="npc"></param>
    /// <param name="battleIndex"></param>
    void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex);

    /// <summary>
    /// 根据任务寻路,返回 true 有处理寻路逻辑,返回 False 没有处理寻路逻辑
    /// 这个接口和 IMissionDelegate.FindToNpc 相结合,处理任务寻路
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="submitDto"></param>
    /// <returns></returns>
    bool FindToNpc(Mission mission,SubmitDto submitDto);

    /// <summary>
    /// 清理完成条件相关生成的资源
    /// </summary>
    /// <param name="submitDto"></param>
    void SubmitClear(SubmitDto submitDto);

}
