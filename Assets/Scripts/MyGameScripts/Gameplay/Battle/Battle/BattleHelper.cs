
using AppDto;
using AppServices;
using UnityEngine;

/// <summary>
/// 为战斗提供一些接口和辅助方法 （manager和controller文件太大了）
/// @MarsZ 2016-08-08 14:56:28
/// </summary>

public class BattleHelper
{
	//	观战类型
	public enum BattleWatchType : int
	{
		WatchType_Nothing = 0,
		WatchType_Duel,
		WatchType_CSPK,
	}

	/// <summary>
	/// 观战某玩家的战斗
	/// </summary>
	/// <param name="pPlayerUid">P player uid.</param>
	/// <param name="pSuccessCallBack">P success call back. 满足观战条件，请求观战后的回调。</param>
	public static void WatchPlayerBattle(long pPlayerUid, int pTargetSceneID = 0, long pBattleID = 0, BattleWatchType watchType = BattleWatchType.WatchType_Nothing, System.Action pSuccessCallBack = null, System.Action pFailCallBack = null)
	{
        if (BattleDataManager.DataMgr.IsInBattle)
		{
			TipManager.AddTip("你正在战斗状态，无法观战");
			return;
		}

		//	不同场景服无法获取指定玩家信息，这里战斗状态由服务器判断和提示
		//if (WorldManager.Instance.GetModel().GetPlayerBattleStatus(pPlayerUid)) {

        GameDebuger.TODO(@"
if (ModelManager.Team.IsFollowLeader())
        {
            TipManager.AddTip(""你处于归队状态，无法观战"");
        }        
");

		var judgmentSceneIDSta = true;
		if (pTargetSceneID > 0)
		{
            var tSceneID = WorldManager.Instance.GetModel() == null? ModelManager.Player.GetPlayer().sceneId : WorldManager.Instance.GetModel().GetSceneId();
//            int tSceneID = WorldManager.Instance.GetModel().GetSceneId();// legacy 2017-02-22 15:32:40
			judgmentSceneIDSta = tSceneID == DataCache.getDtoByCls<SceneMap>(pTargetSceneID).id;
		}
		
        GameDebuger.TODO(@"
        if (judgmentSceneIDSta)
        {
            if (pBattleID > 0)
                ServiceRequestAction.requestServer(CommandService.watchBattleByBattleId(pPlayerUid, pBattleID), ""watchBattleByBattleId"",(e)=>
                {
                    if (null != pSuccessCallBack)
                        pSuccessCallBack();
                },(e) =>
                    {
                        TipManager.AddTip(e.message);
                        if (pFailCallBack != null)
                            pFailCallBack();
                    });
            else
                ServiceRequestAction.requestServer(CommandService.watchBattle(pPlayerUid));

            
        } else {
            switch (watchType) {
            case BattleWatchType.WatchType_Duel:
                Npc tTargetNpc = DataCache.getDtoByCls<Npc>(ModelManager.DuelData.speciteNpcID);
                SceneMap tTargetSceneMap = DataCache.getDtoByCls<SceneMap>(pTargetSceneID);

                TipManager.AddTip(string.Format(""请前往{0}{1}处观战"", tTargetSceneMap.name.WrapColor(ColorConstantV3.Color_Green_Str), tTargetNpc.name.WrapColor(ColorConstantV3.Color_Blue_Str)));
                break;
            case BattleWatchType.WatchType_CSPK:
                //  这里不需要如是操作。2016-09-23 18:04:12
                break;
            }
        }
        //} else {
        //  TipManager.AddTip(""对方不在战斗状态， 无法观战"");
        //}        
");
	}
	
	public static string GetSkillEffectName(Skill skill, string skillName)
	{
		if (skillName != "")
		{
			if (skillName.Contains("skill_") == false && skillName.Contains("game_") == false)
			{
				int effId = skill.clientEffectType;

				if (effId == 0)
				{
					effId = skill.id;
				}

				skillName = string.Format("skill_eff_{0}_{1}", effId, skillName);
			}

			skillName = skillName.ToLower();

			//string skillPath = PathHelper.GetEffectPath(skillName);
		}

		return skillName;
	}

	public static void ShowBGTexture(float pPlayTime, float pDuration, string pBGTextureName)
	{
//		JSTimer.Instance.SetupCoolDown("ShowBGTexture", pPlayTime, null, () =>
//		{
//			UITexture tUITexture = LayerManager.Root.Battle2dBg_UITexture;
//			if (null == tUITexture)
//				return;
//			Texture tOrignTexture = tUITexture.mainTexture;
//			float tOrignAlpha = tUITexture.alpha;
//			AssetPipeline.ResourcePoolManager.Instance.LoadImage(pBGTextureName, (e) =>
//			{
//				if (null == tUITexture)
//					return;
//				tUITexture.alpha = 1F;
//				tUITexture.mainTexture = e as Texture2D;
//				JSTimer.Instance.SetupCoolDown("ShowBGTexture", pDuration, null, () =>
//				{
//					if (null == tUITexture)
//						return;
//					UIHelper.DisposeUITexture(tUITexture);
//					tUITexture.mainTexture = tOrignTexture;
//					tUITexture.alpha = tOrignAlpha;
//				});
//			}, () =>
//			{
//				GameDebuger.LogError(string.Format("[Error]设置战斗背景图失败，没有对应的资源(name:{0})！", pBGTextureName));
//			});
//		});
	}
}