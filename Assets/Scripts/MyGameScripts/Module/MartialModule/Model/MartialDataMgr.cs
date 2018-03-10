// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class MartialDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<KungfuActivityInfo>(UpdateKungFuInfo));
        _disposable.Add(NotifyListenerRegister.RegistListener<BattleReadyNotify>(OpenBattleReadyView));
        _disposable.Add(WorldManager.WorkdModelStream.Subscribe(e =>
        {
            var sceneDto = WorldManager.Instance.GetModel().GetSceneDto();
            if (sceneDto == null) return;
            if (sceneDto.sceneMap.type != (int) SceneMap.SceneType.Kungfu)
                ProxyMartial.CloseMartialView();
            else
            {
                if (e.SceneId == DataMgr._data.SceneId)
                    return;
                var hasteam = TeamDataMgr.DataMgr.HasTeam();
                if (!hasteam)
                {
                    var ctrlWin = ProxyBaseWinModule.Open();
                    var title = "武术大会";
                    var txt = "你当前没有队伍,是否需要匹配队伍";
                    BaseTipData data = BaseTipData.Create(title, txt, 0, () =>
                    {
                        var target = DataCache.getDtoByCls<TeamActionTarget>(1201);
                        var matchData = TeamMatchTargetData.Create(target.id, target.maxGrade, target.minGrade, true);
                        TeamDataMgr.TeamNetMsg.AutoMatchTeam(matchData, true);
                    }, null);
                    ctrlWin.InitView(data);
                }
            }
            DataMgr._data.SceneId = e.SceneId;
        }));
    }
    
    private void OnDispose(){
            
    }

    private void UpdateKungFuInfo(KungfuActivityInfo info)
    {
        DataMgr._data.ActivityInfo = info;
        var t = info.endAt - SystemTimeManager.Instance.GetUTCTimeStamp();
        DataMgr._data.EndAtTime = (int)t /1000;
        DataMgr._data.UpdateEndTime();
        DataMgr._data.UpdateActiveState(info.state);
        //GameDebuger.Log("======KungfuActivityInfo========" + DataMgr._data.EndAtTime);
        stream.OnNext(_data);   //通知主界面刷新活动按钮状态
    }

    public IMartialData GetMartialData()
    {
        return _data;
    }

    private static void OpenBattleReadyView(BattleReadyNotify notify)
    {
        ProxyMartial.CloseMartialView();
        var controller = UIModuleManager.Instance.OpenFunModule<BattleReadyViewController>(BattleReadyView.NAME,
            UILayerType.SubModule, true, false);
        controller.OpenBattleReadyView(notify);
    }

    public bool IsInMartialScene()
    {
        var sceneDto = WorldManager.Instance.GetModel().GetSceneDto();
        if (sceneDto == null) return false;
        return sceneDto.sceneMap.type == (int) SceneMap.SceneType.Kungfu;
    }
}
