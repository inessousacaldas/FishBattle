using AppDto;
using AppServices;
using UnityEngine;
public class NpcSceneMazeBoxUnit : TriggerNpcUnit
{
    private float _boxCoolDown = 120f;
    public override void DoTrigger()
    {
        waitingTrigger = false;
        touch = false;

        ShowOpening();
    }

    private void ShowOpening()
    {
        GameDebuger.TODO(@"_boxCoolDown = DataCache.GetStaticConfigValue(AppStaticConfigs.MAZE_OPEN_BOX_CD, 120);
        string taskName = 'OpenNpcSceneMazeBoxCoolDown';
        NpcSceneMazeBox npcBox = GetNpc() as NpcSceneMazeBox;
        if (npcBox != null)
        {
            taskName = 'OpenNpcSceneMazeBoxCoolDown_' + npcBox.boxType;
            //GameDebuger.Log('1.设置迷宫宝箱定时器名字: ' + taskName, 'orange');
        }

        if (JSTimer.DataMgr.IsCdExist(taskName))
        {
            float remainTime = JSTimer.DataMgr.GetRemainTime(taskName);
            TipManager.AddTip(string.Format('别急，你刚刚才拾取过宝箱，还是给别人留点机会吧！{0}秒后可再次开启', (int)remainTime));
            return;
        }

        MainUIViewController.DataMgr.SetMissionUsePropsProgress(true, '正在开启宝箱……', CancelOpen);
        JSTimer.DataMgr.SetupCoolDown('OpenNpcSceneMazeBox', 3f,
            (remainTime) => { MainUIViewController.DataMgr.SetMissionUsePropsProgress(1 - remainTime / 3f); }, () =>
            {
                MainUIViewController.DataMgr.SetMissionUsePropsProgress(false, "");

                WorldModel worldModel = WorldManager.DataMgr.GetModel();
                if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                {
                    TipManager.AddTip(string.Format('{0}已经消失了',_npcInfo.name));
                    return;
                }

        JSTimer.DataMgr.SetupCoolDown(taskName, _boxCoolDown, null, null);
                //GameDebuger.Log('2.开启迷宫宝箱定时器: ' + taskName,'orange');
                ServiceRequestAction.requestServer(SceneService.openBox(_npcInfo.npcStateDto.id),'openBox',null,e => //出错则立即刷新宝箱CD
        {
                    TipManager.AddTip(e.message);
                    //GameDebuger.Log(e.message);
                    JSTimer.DataMgr.CancelCd(taskName);
                    //GameDebuger.Log('3.取消迷宫宝箱定时器: ' + taskName + ' ,服务器报错:' + e.message,'orange');
                });

            }, 0);");
    }

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd("OpenNpcSceneMazeBox");
		GameDebuger.TODO(@"ServiceRequestAction.requestServer(SceneService.openBoxCancel(_npcInfo.npcStateDto.id), 'openBoxCancel');");
    }

    protected override bool NeedTrigger()
    {
        return true;
    }

    protected override void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0f, 0.35f, 0f);
        _boxCollider.size = new Vector3(1f, 0.7f, 0.7f);
        _unitGo.tag = GameTag.Tag_Npc;
    }

    protected override void AfterInit()
    {
        base.AfterInit();

        InitPlayerName();
    }
}