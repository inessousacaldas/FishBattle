using AppServices;
using UnityEngine;
public class WorldBossPreciousBoxUnit : TriggerNpcUnit
{

    public override void DoTrigger()
    {
        waitingTrigger = false;
        touch = false;

        ShowOpening();
    }

    private void ShowOpening()
    {
        if (JSTimer.Instance.IsCdExist("OpenWorldBossPreciousBoxCoolDown"))
        {
            float remainTime = JSTimer.Instance.GetRemainTime("OpenWorldBossPreciousBoxCoolDown");
            TipManager.AddTip(string.Format("别急，你刚刚才拾取过宝箱，还是给别人留点机会吧！{0}秒后可再次开启", (int)remainTime));
            return;
        }

        GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, '正在开启宝箱……', CancelOpen);
        JSTimer.Instance.SetupCoolDown('OpenWorldBossPreciousBox', 3f,
            (remainTime) => { MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime / 3f); }, () =>
            {
                MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");

                WorldModel worldModel = WorldManager.Instance.GetModel();
                if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                {
                    TipManager.AddTip(string.Format('{0}已经消失了',_npcInfo.name));
                    return;
                }

        JSTimer.Instance.SetupCoolDown('OpenWorldBossPreciousBoxCoolDown', 30f, null, null);
        ServiceRequestAction.requestServer(SceneService.openBox(_npcInfo.npcStateDto.id),'openBox',null,e => //出错则立即刷新宝箱CD
        {
                    TipManager.AddTip(e.message);
                    JSTimer.Instance.CancelCd('OpenWorldBossPreciousBoxCoolDown');
        });

            }, 0);");
    }

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd("OpenWorldBossPreciousBox");
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