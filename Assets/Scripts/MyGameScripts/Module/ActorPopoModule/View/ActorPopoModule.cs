using UnityEngine;
using System.Collections.Generic;

public class ProxyActorPopoModule
{

    private const string NAME = "ActorPopoModuleView";

    private static Dictionary< long, ActorPopoModuleViewController > _popoMaps; //原数据
    private static GameObject _actorPopoPanel;

    public static int ActorPopoCount = 0;

    /// <summary>
    /// Open the specified popoId, tran, msg, showCamera and offY.
    /// </summary>
    /// <param name="popoId">聊天泡泡唯一ID</param>
    /// <param name="tran">泡泡对位位置</param>
    /// <param name="msg">内容</param>
    /// <param name="showCamera">显示用的摄像机 LayerManager.Instance.BattleCamera or LayerManager.Root.SceneCamera</param>
    /// <param name="offY">Y轴偏移</param>
    public static void Open(long popoId,Transform target,string msg,Camera showCamera,float offY = 0,float delayToClose = 4f)
    {
        if(target == null)
        {
            return;
        }

        if(_popoMaps == null)
        {
            _popoMaps = new Dictionary<long,ActorPopoModuleViewController>();
        }

        if(_actorPopoPanel == null)
        {
            _actorPopoPanel = NGUITools.AddChild(LayerManager.Root.UIModuleRoot);
            _actorPopoPanel.name = "ActorPopoPanel";
            UIPanel panel = _actorPopoPanel.GetMissingComponent<UIPanel> ();
            panel.depth = -8;
        }

        GameObject modulePrefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI( NAME ) as GameObject;
        GameObject module = NGUITools.AddChild(_actorPopoPanel,modulePrefab);

        if(_popoMaps.ContainsKey(popoId))
        {
            Close(popoId);
        }
        var controller = module.AddMissingComponent<ActorPopoModuleViewController>();
        controller.Open(popoId,target,msg,showCamera,offY,delayToClose);
        ActorPopoCount++;
        _popoMaps.Add(popoId,controller);
    }

    public static void Close(long popoId)
    {
        if(_popoMaps != null && _popoMaps.ContainsKey(popoId))
        {
            ActorPopoModuleViewController module = _popoMaps[popoId];
            GameObject.Destroy(module.gameObject);
            module.Dispose();
            _popoMaps.Remove(popoId);
        }
    }

    public static void CloseAll()
    {
        if(_popoMaps != null)
        {
            foreach(var popoMapKey in _popoMaps.Keys)
            {
                ActorPopoModuleViewController module = _popoMaps[popoMapKey];
                GameObject.Destroy(module.gameObject);
                module.Dispose();
            }
            _popoMaps.Clear();
        }
        ActorPopoCount = 0;
    }

}
