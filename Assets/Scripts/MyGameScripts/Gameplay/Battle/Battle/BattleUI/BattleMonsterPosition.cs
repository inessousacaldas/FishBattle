// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : BattleMonsterPosition.cs
// Author   : senkay <senkay@126.com>
// Created  : 11/05/2015 
// Porpuse  : 
// **********************************************************************
//

using AssetPipeline;
using UnityEngine;

public class BattleMonsterPosition : MonoBehaviour
{
    public UILabel label;

    public static BattleMonsterPosition CreateNew(MonsterController mc)
    {
        var monsterOrderPrefab = ResourcePoolManager.Instance.LoadUI("MonsterPosition") as GameObject;
        var monsterOrderGO = NGUITools.AddChild(LayerManager.Root.BattleUIHUDPanel.cachedGameObject, monsterOrderPrefab);
        var follower = monsterOrderGO.GetMissingComponent<UIFollowTarget>();

        follower.gameCamera = LayerManager.Root.BattleCamera_Camera;
        follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;

        var monsterPosition = monsterOrderGO.GetMissingComponent<BattleMonsterPosition>();
        monsterPosition.UpdateFollowTarget(mc);
		monsterPosition.InitView();
        return monsterPosition;
    }

    public void UpdateFollowTarget(MonsterController mc)
    {
        var follower = gameObject.GetComponent<UIFollowTarget>();
        if (follower == null)
        {
            return;
        }

        var tf = mc.GetMountShadow();

        if (tf == null)
        {
            tf = mc.gameObject.transform;
            follower.offset = new Vector3(0, 0.15f, 0);
        }
        else
        {
            follower.offset = new Vector3(0, 0.15f, 0);
        }

        follower.target = tf;
    }

    private void InitView()
    {
        label = transform.Find("PositionLabel").GetComponent<UILabel>();
    }

    public void showPosition(int position)
    {
        label.text = position.ToString();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}