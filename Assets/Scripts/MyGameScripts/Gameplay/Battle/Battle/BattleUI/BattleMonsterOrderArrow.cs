using System;
using AssetPipeline;
using UnityEngine;

public class BattleMonsterOrderArrow : MonoBehaviour
{
    public Action _onFinish;

    public UISprite arrow;

    public static BattleMonsterOrderArrow CreateNew(MonsterController mc, Action onFinish)
    {
        var monsterOrderArrowPrefab = ResourcePoolManager.Instance.LoadUI("MonsterOrderAarrow") as GameObject;
        var monsterOrderArrowGO = NGUITools.AddChild(LayerManager.Root.BattleUIHUDPanel.cachedGameObject,
            monsterOrderArrowPrefab);
        var follower = monsterOrderArrowGO.AddComponent<UIFollowTarget>();

        follower.gameCamera = LayerManager.Root.BattleCamera_Camera;
        follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;

        var tf = mc.GetMountHUD();

        if (tf == null)
        {
            tf = mc.gameObject.transform;
            follower.offset = new Vector3(0f, -1.2f, 0);
        }
        else
        {
            follower.offset = new Vector3(0f, -0.2f, 0);
        }

        follower.target = tf;
        var com = monsterOrderArrowGO.AddMissingComponent<BattleMonsterOrderArrow>();
		com.InitView();
        com.arrow.fillAmount = 0;
        com._onFinish = onFinish;
        return com;
    }

    public void InitView()
    {
        arrow = gameObject.transform.Find("bottomSprite").GetComponent<UISprite>();
    }

    private void Update()
    {
        if (arrow.cachedGameObject.activeInHierarchy)
        {
            arrow.fillAmount += Time.deltaTime*3.5f;

            if (arrow.fillAmount >= 1)
            {
                if (_onFinish != null)
                {
                    _onFinish();
                }
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}