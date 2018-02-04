using AssetPipeline;
using UnityEngine;

public class BattleMonsterOrder : MonoBehaviour
{
    public UILabel label;

    public static BattleMonsterOrder CreateNew(MonsterController mc)
    {
        var monsterOrderPrefab = ResourcePoolManager.Instance.LoadUI("MonsterOrder") as GameObject;
        var monsterOrderGO = NGUITools.AddChild(LayerManager.Root.BattleUIHUDPanel.cachedGameObject, monsterOrderPrefab);
        var follower = monsterOrderGO.GetMissingComponent<UIFollowTarget>();

        follower.gameCamera = LayerManager.Root.BattleCamera_Camera;
        follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;

        var monsterOrder = monsterOrderGO.GetMissingComponent<BattleMonsterOrder>();
        monsterOrder.UpdateFollowTarget(mc);
		monsterOrder.InitView();
        monsterOrder.showOrder("");
        return monsterOrder;
    }

    public void UpdateFollowTarget(MonsterController mc)
    {
        var follower = gameObject.GetComponent<UIFollowTarget>();
        if (follower == null)
        {
            return;
        }

        var tf = mc.GetMountHUD();

        if (tf == null)
        {
            tf = mc.gameObject.transform;
            follower.offset = new Vector3(0, -2f, 0);
        }
        else
        {
            follower.offset = new Vector3(0, -1f, 0);
        }

        follower.target = tf;
    }

    private void InitView()
    {
        label = transform.Find("bgSprite/NameLabel").GetComponent<UILabel>();
    }

    public void showOrder(string order)
    {
        label.text = "";
        if (string.IsNullOrEmpty(order))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            label.text = order;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}