using AssetPipeline;
using System;
using UnityEngine;

namespace Assets.Scripts.MyGameScripts.Manager
{
    class UIComponentMgr
    {
        public static GameObject AddBgMask(BaseView view,Transform parent, bool bgMaskClose,Action onClick = null)
        {
            var bgMask = NGUITools.AddChild(parent.gameObject, ResourcePoolManager.Instance.LoadUI("ModuleBgBoxCollider"));

            if(bgMaskClose)
            {
                var button = bgMask.GetMissingComponent<UIEventTrigger>();
                EventDelegate.Set(button.onClick,() =>
                {
                    if(onClick == null)
                    {
                        if(view != null)
                        {
                            view.Hide();
                        }
                    }else
                    {
                        onClick.Invoke();
                    }
                    UnityEngine.Object.Destroy(bgMask);
                });
            }
            var uiWidget = bgMask.GetMissingComponent<UIWidget>();
            uiWidget.depth = -1;

            Vector2 size = NGUITools.screenSize;
            size *= UIRoot.GetPixelSizeAdjustment(view.gameObject);

            uiWidget.width = (int)size.x;
            uiWidget.height = (int)size.y;
            //uiWidget.autoResizeBoxCollider = true;
            //uiWidget.SetAnchor(view.gameObject,-10,-10,10,10);
            NGUITools.AddWidgetCollider(bgMask);
            return bgMask;
        }

        public static void ResetDepth(GameObject go,int depth)
        {
            var widgetList = go.GetComponentsInChildren<UIWidget>();
            for(var i = 0;i < widgetList.Length;i++)
            {
                widgetList[i].depth = depth;
            }
        }
    }
}
