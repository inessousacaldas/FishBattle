using UnityEngine;
using System.Collections;
using AppDto;

public class NpcSceneGoldBoxUnit : TriggerNpcUnit
{
    private const string HasKeyContent = "是否使用金宝箱钥匙开启金宝箱？";
    private const string WithoutKeyContent = "是否花费{0}{1}购买金钥匙开启金宝箱？";
    private const float OpenWaitTime = 3f;
    private const string OpenGoldBox = "OpenGoldBox";
    private const string IsOpenByOtherTipStr = "该宝箱已经被捷足先登啦";

    public override void DoTrigger()
    {

        waitingTrigger = false;
        touch = false;

        ModelManager.Player.StopAutoNav();

        int spendCopper = DataCache.GetStaticConfigValue(AppStaticConfigs.SCENE_GOLD_BOX_CONSUME_COPPER, 200);

        string contentMsgStr = "";
        bool hasKey = false;
        //如果有钥匙进度条
        GameDebuger.TODO(@"if (ModelManager.Backpack.CheckHasGoldKey())
        {
            contentMsgStr = HasKeyContent;
            hasKey = true;
        }
        else");
        {                                                                                                                                                                                                                                                                                                  
            contentMsgStr = string.Format(WithoutKeyContent, spendCopper, ItemIconConst.Ingot);
        }
        //没有就弹框购买
        ProxyWindowModule.OpenConfirmWindow(contentMsgStr,
                                              "",
                                              () =>
                                              {
                                                  ShowOpening(spendCopper, hasKey);
                                              }, null, UIWidget.Pivot.Center);
    }

    private void ShowOpening(int spendCopper,bool hasKey)
    {
        if (!hasKey)
        {
            if (ModelManager.Player.isEnoughIngot(spendCopper, true))
            {
                OpenBox();
            }   
        }
        else
        {
            OpenBox();
        }
    }

    private void OpenBox()
    {
		WorldModel worldModel = WorldManager.Instance.GetModel();
        if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
		{
			TipManager.AddTip(IsOpenByOtherTipStr);
			return;
		}

        GameDebuger.TODO(@"ModelManager.SceneMonster.NpcFunctionToEnterBattle(_npcInfo.npcStateDto,OnBoxOpened);");
    }

	private void OnBoxOpened()
	{
		GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, '正在开启金宝箱……', CancelOpen);
        JSTimer.Instance.SetupCoolDown(OpenGoldBox, OpenWaitTime,
            (remainTime) => { MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime/ OpenWaitTime); }, () =>
            {
                MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");
            }, 0);");
	}

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd(OpenGoldBox);
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