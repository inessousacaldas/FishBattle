using System.Collections.Generic;
using AppDto;
using UnityEngine;

public class WorldMapController : MonoViewController<WorldMapView>
{
    #region IViewController Members

    /// <summary>
    ///     Registers the event.
    ///     DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.CloseButton.onClick, OnCloseButtonClick);
    }

    #endregion

    private void OnCloseButtonClick()
    {
        ProxyWorldMapModule.CloseWorldMap();
    }

    protected override void AfterInitView()
    {
        List<SceneMap> sceneMaps = DataCache.getArrayByCls<SceneMap>();

        List<SceneMap> scene2dList = new List<SceneMap>();
        List<SceneMap> scene3dList = new List<SceneMap>();

        if (sceneMaps != null)
        {
            GameObject tableRoot = View.WorldTable_UITable.gameObject;
            for (int i = 0; i < sceneMaps.Count; i++)
            {
                SceneMap map = sceneMaps[i];
                if (map.name.Contains("2D"))
                {
                    scene2dList.Add(map);
                }
                else
                {
                    scene3dList.Add(map);
                }
            }

            for (int i = 0; i < scene2dList.Count; i++)
            {
                SceneMap map = scene2dList[i];

                //			if (map.type != 8 && map.type != 9)
                //			{
                GameObject btn = AddCachedChild(tableRoot, "BaseSmallButton");
                InitMapBtn(btn, map);
                //}
            }

            for (int i = 0; i < scene3dList.Count; i++)
            {
                SceneMap map = scene3dList[i];
				
                //			if (map.type != 8 && map.type != 9)
                //			{
                GameObject btn = AddCachedChild(tableRoot, "BaseSmallButton");
                btn.AddMissingComponent<UIDragScrollView>();
                InitMapBtn(btn, map);
                //}
            }
        }
    }

    private void InitMapBtn(GameObject btn, SceneMap map)
    {
        btn.name = "Map_" + map.id;
        btn.GetComponentInChildren<UILabel>().text = map.name;
        UIButton uiButton = btn.GetComponent<UIButton>();
        EventDelegate.Set(uiButton.onClick, () =>
            {
                WorldManager.Instance.Enter(map.id, false);
                ProxyWorldMapModule.CloseWorldMap();
            });
    }
}