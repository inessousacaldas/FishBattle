using AppDto;
using AppServices;
using UnityEngine;
public class NpcSceneGrassUnit : TriggerNpcUnit
{
    private float _boxCoolDown = 10f;
    public override void DoTrigger()
    {
        waitingTrigger = false;
        touch = false;

        ShowOpening();
    }

    private void ShowOpening()
    {
        string taskName = "NpcSceneGrassCoolDown";
        //NpcSceneGrass npcBox = GetNpc() as NpcSceneGrass;

        if (JSTimer.Instance.IsCdExist(taskName))
        {
            float remainTime = JSTimer.Instance.GetRemainTime(taskName);
            TipManager.AddTip(string.Format("别急，你刚刚才拾取过仙草，还是给别人留点机会吧！{0}秒后可再次采集", (int)remainTime));
            return;
        }

        GameDebuger.TODO(@"if (WorldManager.DataMgr.GetModel().GetSceneDto().guildId != 0 &&
            WorldManager.DataMgr.GetModel().GetSceneDto().guildId != ModelManager.Player.GetGuildId())
        {
            TipManager.AddTip('少侠不是本帮派成员。');
            return;
        }");

        GameDebuger.TODO(@"if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_HundredGrassValley, false))
        {
            TipManager.AddTip(string.Format('少侠等级未达到{0}级。', FunctionOpenHelper.GetFunctionOpenLv(FunctionOpen.FunctionOpenEnum_HundredGrassValley)));
            return;
        }

        MainUIViewController.DataMgr.SetMissionUsePropsProgress(true, '正在采集仙草……', CancelOpen);
        JSTimer.DataMgr.SetupCoolDown('OpenNpcSceneGrass', 3f,
            (remainTime) => { MainUIViewController.DataMgr.SetMissionUsePropsProgress(1 - remainTime / 3f); }, () =>
            {
                MainUIViewController.DataMgr.SetMissionUsePropsProgress(false, "");

                WorldModel worldModel = WorldManager.DataMgr.GetModel();
                if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                {
                    TipManager.AddTip(string.Format('{0}已经消失了', _npcInfo.name));
                    return;
                }

                JSTimer.DataMgr.SetupCoolDown(taskName, _boxCoolDown, null, null);
                ServiceRequestAction.requestServer(SceneService.openBox(_npcInfo.npcStateDto.id), 'openBox', null, e => //出错则立即刷新宝箱CD
                {
                    TipManager.AddTip(e.message);
                    JSTimer.DataMgr.CancelCd(taskName);
                });

            }, 0);");
    }

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd("OpenNpcSceneGrass");
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