using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class NpcViewPool
{
    public readonly NpcViewManager master;
    private Queue<GameObject> npcViewPool;

    private bool UsePool
    {
        get { return WorldView.UsePool; }
    }
    public NpcViewPool(NpcViewManager _master)
    {
        this.master = _master;
        npcViewPool = new Queue<GameObject>(GameDisplayManager.MaxNpcViewPoolCount);
    }

    public ModelVisibleChecker CreateNpcViewGo()
    {
        GameObject _unitGo = NGUITools.AddChild(LayerManager.Root.WorldActors);
        ModelVisibleChecker checker = _unitGo.GetMissingComponent<ModelVisibleChecker>();
        return checker;
    }
    public void DespawnPlayerView(GameObject npcView)
    {
        if (npcView == null) return;
        if (UsePool && npcViewPool.Count < GameDisplayManager.MaxNpcViewPoolCount)
        {
            npcView.SetActive(false);
            npcView.name = "NPCViewX";
            npcViewPool.Enqueue(npcView);
        }
        else
        {
            //超过当前场景最大玩家数量,直接删除
            Object.Destroy(npcView);
        }
    }

    public ModelVisibleChecker SpawnPlayerView()
    {
        ModelVisibleChecker modelVisibleChecker = null;
        if (npcViewPool.Count > 0)
        {
            GameObject go = npcViewPool.Dequeue();
            modelVisibleChecker = go.GetMissingComponent<ModelVisibleChecker>();
        }
        else
        {
            modelVisibleChecker = CreateNpcViewGo();
        }
        modelVisibleChecker.gameObject.SetActive(true);
        return modelVisibleChecker;

    }
}

