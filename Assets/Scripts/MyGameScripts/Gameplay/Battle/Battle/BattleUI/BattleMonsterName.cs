// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleMonsterName.cs
// Author   : SK
// Created  : 2014/5/29
// Purpose  : 
// **********************************************************************

using AssetPipeline;
using UnityEngine;

public class BattleMonsterName : MonoBehaviour
{
    public UILabel label;

    public static BattleMonsterName CreateNew(MonsterController mc)
    {
        if (mc == null) return null;
        var monsterNamePrefab = ResourcePoolManager.Instance.LoadUI("MonsterName");
        var monsterNameGO = NGUITools.AddChild(LayerManager.Root.BattleUIHUDPanel.cachedGameObject, monsterNamePrefab);
        var follower = monsterNameGO.AddComponent<UIFollowTarget>();

        follower.gameCamera = LayerManager.Root.BattleCamera_Camera;
        follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;

        var monsterName = monsterNameGO.AddMissingComponent<BattleMonsterName>();
        monsterName.SetupView();
        monsterName.UpdateFollowTarget(mc);
        monsterName.Setup(mc);
        return monsterName;
    }

    public void SetupView()
    {
        label = gameObject.transform.Find("NameLabel").GetComponent<UILabel>();
    }

    public void UpdateFollowTarget(MonsterController mc)
    {
        var follower = gameObject.GetComponent<UIFollowTarget>();

        if (follower == null)
        {
            return;
        }

        var mount = mc.GetMountShadow();

        if (mount == null)
        {
            mount = mc.gameObject.transform;
        }
        else
        {
            mount.localRotation = Quaternion.identity;
            mount.localScale = Vector3.one;
        }


        follower.target = mount;
        follower.offset = new Vector3(0, -0.4f, 0);
    }

    public void Setup(MonsterController mc)
    {
        var showStr = mc.videoSoldier.name;
        GameDebuger.TODO(@"switch (mc.videoSoldier.monsterType)
        {
            case Monster.MonsterType_Boss:
                if (!showStr.Contains('头领'))
                {
                    showStr = showStr + '头领';
                }
                break;
            case Monster.MonsterType_Baobao:
                if (!showStr.Contains('宝宝'))
                {
                    showStr = showStr + '宝宝';
                }
                break;
            case Monster.MonsterType_Mutate:
                if (!showStr.Contains('变异'))
                {
                    showStr = '变异' + showStr;
                }
                break;
        }");

        label.fontSize = 14;
        label.text = "[b]" + showStr;
        if (mc.IsPet() || mc.IsMainCharactor())
        {
            label.color = ColorConstant.Color_Battle_Player_Name;
        }
        else
        {
            label.color = ColorConstant.Color_Battle_Enemy_Name;
        }
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }

    
}