using UnityEngine;
using AppDto;

public class PreciousBoxUnit : TriggerNpcUnit {
	
    public override void DoTrigger()
    {

        waitingTrigger = false;
        touch = false;

        ModelManager.Player.StopAutoNav();

        GameDebuger.TODO(@"if (ModelManager.Spell.IsCurrSpellMaxExp())
        {

            ProxyManager.Window.OpenConfirmWindow(string.Format('你的[37F605]{0}[-]等级已经达到上限，请更换修炼类型，否则无法获得#exp2',
                                                               ModelManager.Spell.GetSelectSpellName()), "", () =>
                                                               {
                                                                   //打开修炼界面
                                                                   ProxyManager.Skill.OpenPracticeSkill();
                                                               });

        }
        else
        {
            int spendCopper = DataCache.GetStaticConfigValue(AppStaticConfigs.SCENE_PRECIOUS_BOX_CONSUME_COPPER, 35000);

            if (ProxyManager.TreasureMap.openPreciousBoxCount == 0)
            {
                ProxyManager.Window.OpenConfirmWindow(string.Format('开启银宝箱可获得最高300点修炼经验{0}，是否花费{1}{2}开启？（当前修炼类型：[00ff00]{3}[-]）', ItemIconConst.Exp2,spendCopper,ItemIconConst.Copper, ModelManager.Spell.GetSelectSpellName()),
                                                    "",
                                                    () =>
                                                    {
                                                        ShowOpening(spendCopper);
                                                    });
            }
            else
            {
                ShowOpening(spendCopper);
            }

        }");
    }

    private void ShowOpening(int spendCopper)
    {
//        WorldModel worldModel = WorldManager.Instance.GetModel();
        //        if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
//        {
//            TipManager.AddTip("银宝箱已经消失了...");
//            return;
//        }

        if (ModelManager.Player.isEnoughCopper(spendCopper, true))
        {
            GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, '正在开启宝箱……',CancelOpen);
            JSTimer.Instance.SetupCoolDown('OpenPreciousBox', 3f,
                (remainTime) => { MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime/3f); }, () =>
                {
                    MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");

                    WorldModel worldModel = WorldManager.Instance.GetModel();
                    if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                    {
                        TipManager.AddTip('银宝箱已经消失了');
                        return;
                    }

                    ProxyManager.TreasureMap.openPreciousBoxCount = 1;
                    ModelManager.SceneMonster.NpcFunctionToEnterBattle(_npcInfo.npcStateDto);
                }, 0);");
        }
    }

    private void CancelOpen()
    {
        JSTimer.Instance.CancelCd("OpenPreciousBox");
		GameDebuger.TODO(@"ServiceRequestAction.requestServer(AppServices.SceneService.openBoxCancel(_npcInfo.npcStateDto.id), 'openBoxCancel');");
    }


    protected override bool NeedTrigger(){
		return true;
	}

	protected override void SetupBoxCollider ()
	{
		_boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
		_boxCollider.isTrigger = true;
		_boxCollider.center = new Vector3(0f, 0.35f, 0f);
		_boxCollider.size = new Vector3(1f, 0.7f, 0.7f);
		_unitGo.tag = GameTag.Tag_Npc;
	}

	
	protected override void AfterInit() {
		base.AfterInit();

		InitPlayerName ();
	}
}
