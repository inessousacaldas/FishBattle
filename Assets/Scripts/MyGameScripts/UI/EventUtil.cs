using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MyGameScripts.UI
{
    public class EventUtil {
        private static Dictionary<GameObject, List<UIEventListener.VoidDelegate>> clickDic = new Dictionary<GameObject, List<UIEventListener.VoidDelegate>>();

        public static void AddClick<T>(UIButton btn,Action<T> handler,T arg = null) where T : class
        {
            AddClick(btn.gameObject, handler,arg);
        }

        public static void AddClick<T>(GameObject go,Action<T> handler,T arg = null) where T : class
        {
            if(clickDic.ContainsKey(go) == false)
            {
                clickDic[go] = new List<UIEventListener.VoidDelegate>();
            }
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
                UIEventListener.Get(go).onClick += de;
        }

        public static void RemoveClick(GameObject go, UIEventListener.VoidDelegate handler) {
            UIEventListener.Get(go).onClick -= handler;
        }

        public static UIEventListener.VoidDelegate AddDoubleClick(GameObject go, Action<object> handler, object arg = null) {
            UIEventListener.VoidDelegate de = delegate(GameObject go1) {
                handler.DynamicInvoke(arg);
            };
            UIEventListener.Get(go).onDoubleClick = de;
            return de;
        }

        public static void RemoveDoubleClick(GameObject go, UIEventListener.VoidDelegate handler) {
            UIEventListener.Get(go).onDoubleClick -= handler;
        }
    }
}
