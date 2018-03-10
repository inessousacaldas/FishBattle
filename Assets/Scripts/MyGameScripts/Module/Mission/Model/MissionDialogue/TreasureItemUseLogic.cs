using UnityEngine;
using System.Collections;
using AppDto;
using System.Collections.Generic;

public class TreasureItemUseLogic{
    private List<BagItemDto> mBagtoList;
    private BagItemDto mCurBagItem;
    MissionDataMgr.MissionData mMissionData;
    public TreasureItemUseLogic(MissionDataMgr.MissionData _model) {
        mMissionData = _model;
    }

    public void usePropByPos(IEnumerable<BagItemDto> bagtoList,BagItemDto CurBagItem) {
        if(BattleDataManager.DataMgr.IsInBattle) {
            TipManager.AddTip("正在战斗状态中，不能进行此操作！");
            return;
        }
        if(!TeamDataMgr.DataMgr.IsLeader())
        {
            if(TeamDataMgr.DataMgr.HasTeam())
            {
                var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                if(val != null)
                {
                    if(val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                    {
                        TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                        return;
                    }
                }
            }
        }
        if(CurBagItem == null || CurBagItem.extra == null)
        {
            TipManager.AddTip("物品不存在");
            return;
        }
        if(mCurBagItem == null)
            mCurBagItem = CurBagItem;
        mBagtoList = bagtoList.ToList();
        PropsExtraDto_17 extraDto = CurBagItem.extra as PropsExtraDto_17;
        var heroView = WorldManager.Instance.GetHeroView();
        if(heroView != null && WorldManager.Instance.GetModel().GetSceneId() == extraDto.sceneId)
        {
            Vector3 a = heroView.transform.position;
            Vector3 b = SceneHelper.GetPositionInScene(extraDto.x, extraDto.y, extraDto.z);
            float distance = Vector3.Distance(a,b);
            if(distance <= 1)
            {
                UseTreasureItem();
            }
            else
            {
                GoTreausreMapPosition(extraDto);
            }
        }
        else {
            GoTreausreMapPosition(extraDto);
        }
    }

    #region 正在挖矿
    private void UseTreasureItem() {
        if(!TeamDataMgr.DataMgr.IsLeader())
        {
            if(TeamDataMgr.DataMgr.HasTeam())
            {
                var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                if(val != null)
                {
                    if(val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                    {
                        TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                        return;
                    }
                }
            }
        }
        ProxyProgressBar.ShowProgressBar("1:" + mCurBagItem.item.icon,"",null,"__PopupUseMissionProps",delegate
        {
            BackpackDataMgr.BackPackNetMsg.BackpackApply(mCurBagItem.index,1);
            List<BagItemDto> tBagtoList = mBagtoList.ToList();
            BagItemDto tCurBagItem = null;
            tBagtoList.Remove(mCurBagItem);
            if(tBagtoList.Count > 0)
            {
                for(int i = 0;i < tBagtoList.Count;i++)
                {
                    if((tBagtoList[i].extra as PropsExtraDto_17).sceneId == ModelManager.Player.GetPlayer().sceneId)
                    {
                        tCurBagItem = tBagtoList[i];
                        break;
                    }
                }
                if(tCurBagItem == null)
                {
                    tCurBagItem = tBagtoList[0];
                }
                mBagtoList = tBagtoList;
                mCurBagItem = tCurBagItem;
                usePropByPos(tBagtoList,tCurBagItem);
            }
            else
            {
                mCurBagItem = null;
                if(mBagtoList != null)
                {
                    mBagtoList.Clear();
                    mBagtoList = null;
                }
            }

        },3f);
    }
    #endregion


    #region 移动到藏宝图
    private void GoTreausreMapPosition(PropsExtraDto_17 packItem) {
        string msg = string.Format("藏宝点在{0}，正在进行寻路",
                DataCache.getDtoByCls<SceneMap>(packItem.sceneId).name);
        TipManager.AddTip(msg);

        var npc = new Npc();
        npc.sceneId = packItem.sceneId;
        npc.x = packItem.x;
        npc.y = packItem.y;
        npc.z = packItem.z;
        JoystickModule.Instance.CanControlByPlayer(false);
        JSTimer.Instance.SetupCoolDown(
          "callback"
          ,0.2f
          ,null
          ,delegate
          {
              WorldManager.Instance.FlyToByNpc(npc,0,() =>
              {
                  //AppMissionItem tAppMissionItem = new AppMissionItem();
                  //tAppMissionItem.name = mCurBagItem.item.name;
                  //tAppMissionItem.icon = "1:" + mCurBagItem.item.icon;
                  //tAppMissionItem.usedTip = "";
                  UseTreasureItem();
              });
          });
    }
    #endregion
}
