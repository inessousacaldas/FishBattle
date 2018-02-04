using GamePlot;
using AppDto;

public class MissionStoryPlotDelegate {
    private IMissionData _model;
    private System.Action _storyMissionCallback = null;

    public MissionStoryPlotDelegate(IMissionData model)
    {
        _model = model;
    }

    //	========================================剧情相关=======================================
    /// <summary>
    /// mission:任务 | acceptAndSubmit:是否接任务（true：是 false：否） callback:回调 -- Stories the plot in mission.
    /// </summary>
    /// <param name="missionID">Mission I.</param>
    /// <param name="acceptAndSubmit">If set to <c>true</c> accept and submit.</param>
    /// <param name="callback">Callback.</param>
    public void StoryPlotInMission(Mission mission,bool acceptAndSubmit)
    {
        //接受和完成任务判断处理
        int triggerType = acceptAndSubmit ? (int)Plot.PlotTriggerType.AcceptMission:(int)Plot.PlotTriggerType.SubmitMission;
        if(GamePlotManager.Instance.ContainsPlot(triggerType,mission.id))
        {
            StoryPlot(triggerType,mission.id);
        }
    }


    //任务系统开始调用剧情播放统一接口
    public void StoryPlotInTollgateId(int tollgateID)
    {
        StoryPlot((int)Plot.PlotTriggerType.TollgateWin,tollgateID);
    }

    private void StoryPlot(int type,int param)
    {
        if(BattleDataManager.DataMgr.IsInBattle)
        {
            //战斗中需要显示剧情动画
        }
        else
        {
            GamePlotManager.Instance.TriggerPlot(type,param);
        }
    }

}
