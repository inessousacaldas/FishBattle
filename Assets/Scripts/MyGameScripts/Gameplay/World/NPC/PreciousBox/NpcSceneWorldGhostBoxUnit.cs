using UnityEngine;
using System.Collections;

public class NpcSceneWorldGhostBoxUnit : TriggerNpcUnit {
    private const string _coolDownName = "openWorldGhostBox";
    private const string _openedStr = "{0}已经消失";
    private const string _openNumLimit = "一人一个宝箱，少侠不要拿多了哦~";
    private const string _openLimit = "必须参与讨伐才有资格开启宝箱，下次再接再厉哦~";
    private const string _openingBoxStr = "正在开启{0}...";
    public override void DoTrigger()
    {
        touch = false;
        waitingTrigger = false;
        OpenBox();
    }

    public void OpenBox()
    {
        GameDebuger.TODO(@"if (!ModelManager.SnowWorldBoss.CanOpenBox)
        {
            TipManager.AddTip(string.Format(_openNumLimit, _npcInfo.name));
            return;
        }
        else
        {
            //参与战斗次数
            if (ModelManager.SceneMonster._curWorldGhostCount > 0)
            {
                //开启宝箱
                string tMsg = string.Format(_openingBoxStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str));
                MainUIViewController.Instance.SetMissionUsePropsProgress(true, tMsg, CancelOpen);

                JSTimer.Instance.SetupCoolDown(_coolDownName, 3.0f, remainTime =>
                {
                    MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime / 3.0f);
                    
                }, () =>
                {
                    MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");

                    //已经消失（被他人开启）
                    WorldModel worldModel = WorldManager.Instance.GetModel();
                    if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                    {
                        TipManager.AddTip(string.Format(_openedStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
                        return;
                    }

                    ModelManager.SnowWorldBoss.OpenBox(_npcInfo.npcStateDto);
                }, 0);
            }
            else
            {
                TipManager.AddTip(_openLimit);
            }
        }");
    }

    private void OpenBoxSuccessCallback()
    {
        string tMsg = string.Format(_openingBoxStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str));
        GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, tMsg);

        JSTimer.Instance.SetupCoolDown(_coolDownName, 3.0f, (remainTime) =>
        {
            MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime / 3.0f);

        },() =>
         {
             MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");
         });");
    }

    private void CancelOpen()
    {
        TipManager.AddTip("你放弃了宝箱");
        JSTimer.Instance.CancelCd(_coolDownName);
    }

    protected override bool NeedTrigger()
    {
        return true;
    }

    public override void Destroy()
    {
        base.Destroy();
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
