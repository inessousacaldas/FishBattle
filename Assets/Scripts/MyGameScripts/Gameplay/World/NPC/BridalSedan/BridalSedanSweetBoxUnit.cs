using UnityEngine;

public class BridalSedanSweetBoxUnit : TriggerNpcUnit
{
    private const string PickupCandies = "pickupCandies";
    public override void DoTrigger()
    {
        waitingTrigger = false;
        touch = false;
        ModelManager.Player.StopAutoNav();

        ShowOpening();
    }

    private void ShowOpening()
    {
        GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, '拾取喜糖中...', CancelOpen);
        JSTimer.Instance.SetupCoolDown(PickupCandies, 1f,
            (remainTime) => { MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime / 1f); },
            () =>
            {
                MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");

                WorldModel worldModel = WorldManager.Instance.GetModel();
                if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                {
                    TipManager.AddTip('手太慢了，没抢到');
                    return;
                }
                ModelManager.SceneMonster.NpcFunctionToEnterBattle(_npcInfo.npcStateDto);
            }, 0);");
    }

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd(PickupCandies);
		GameDebuger.TODO(@"ServiceRequestAction.requestServer(AppServices.SceneService.openBoxCancel(_npcInfo.npcStateDto.id), 'openBoxCancel');");
    }


    protected override bool NeedTrigger()
    {
        return true;
    }

    protected override void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0f, 0.25f, 0f);
        _boxCollider.size = new Vector3(1f, 0.8f, 0.8f);
        _unitGo.tag = GameTag.Tag_Npc;
    }


    protected override void AfterInit()
    {
        base.AfterInit();

        InitPlayerName();
    }
}
