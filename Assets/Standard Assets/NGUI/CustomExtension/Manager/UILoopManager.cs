using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Standard_Assets.NGUI.CustomExtension.Manager {
    public class UILoopManager {
            private struct CacheAction{
            public bool isAdd; //是添加还是删除
            public object key;
            public Action action;
        }

        private struct CacheTimeout{
            public bool isAdd; //是添加还是删除
            public int key;
            public float endTime;
            public Delegate action;
            public object[] args;
        }

        private static readonly Dictionary<object, Action> frameDict; //帧执行
        private static readonly Dictionary<object, Action> timerDict; //每80毫秒执行
        private static readonly Dictionary<object, Action> secondDict; //秒执行
        private static readonly Dictionary<int, CacheTimeout> timeoutDict; //延时执行

        private static readonly List<CacheAction> frameCache; //记录要增删的action，等遍历执行完再增删，确保执行过程中字典不被修改
        private static readonly List<CacheAction> timerCache;
        private static readonly List<CacheAction> secondCache;
        private static readonly List<CacheTimeout> timeoutCache;
        private static float timerLastTime = 0;
        private static float secondLastTime = 0;
        private static int delayIDKey; //每次生成setTimeout的Key

        static UILoopManager() {
            frameDict = new Dictionary<object, Action>();
            timerDict = new Dictionary<object, Action>();
            secondDict = new Dictionary<object, Action>();
            timeoutDict = new Dictionary<int, CacheTimeout>();
            frameCache = new List<CacheAction>();
            timerCache = new List<CacheAction>();
            secondCache = new List<CacheAction>();
            timeoutCache = new List<CacheTimeout>();
        }

        public static void FrameLoop(){
            if (frameCache.Count > 0){
                for (int i = 0; i < frameCache.Count; i++){
                    CacheAction c = frameCache[i];
                    if (c.isAdd == true){
                        if (frameDict.ContainsKey(c.key) == false){
                            frameDict.Add(c.key, c.action);
                        }
                    }
                    else{
                        if (frameDict.ContainsKey(c.key) == true){
                            frameDict.Remove(c.key);
                        }
                    }
                }
                frameCache.Clear();
            }
            IEnumerator enumerator = frameDict.Values.GetEnumerator();
            while (enumerator.MoveNext()){
                Action a = enumerator.Current as Action;
                try {
                if (a != null){
                    a.DynamicInvoke();
                }
                } catch (Exception e) {
                    throw (e);
                }
            }

            /////以上处理帧循环函数，以下预更新timer和second字典/////////
            if (timerCache.Count > 0){
                for (int i = 0; i < timerCache.Count; i++){
                    CacheAction c = timerCache[i];
                    if (c.isAdd == true){
                        if (timerDict.ContainsKey(c.key) == false){
                            timerDict.Add(c.key, c.action);
                        }
                    }
                    else{
                        if (timerDict.ContainsKey(c.key) == true){
                            timerDict.Remove(c.key);
                        }
                    }
                }
                timerCache.Clear();
            }
            //////////////////
            if (secondCache.Count > 0){
                for (int i = 0; i < secondCache.Count; i++){
                    CacheAction c = secondCache[i];
                    if (c.isAdd == true){
                        if (secondDict.ContainsKey(c.key) == false){
                            secondDict.Add(c.key, c.action);
                        }
                    }
                    else{
                        if (secondDict.ContainsKey(c.key) == true){
                            secondDict.Remove(c.key);
                        }
                    }
                }
                secondCache.Clear();
            }
            //////////////////////////
            if (timeoutCache.Count > 0){
                for (int i = 0; i < timeoutCache.Count; i++){
                    CacheTimeout c = timeoutCache[i];
                    if (c.isAdd == true){
                        if (timeoutDict.ContainsKey(c.key) == false){
                            timeoutDict.Add(c.key, c);
                        }
                    }
                    else{
                        if (timeoutDict.ContainsKey(c.key) == true){
                            timeoutDict.Remove(c.key);
                        }
                    }
                }
                timeoutCache.Clear();
            }
            ///////////以下时间到了执行timer和second函数字典////////////////
            float now = Time.time;
            if (now - timerLastTime > 0.08f){
                timerLastTime = now;
                OnTimerLoop();
                OnTimeoutLoop();
            }
            if (now - secondLastTime > 1){
                secondLastTime = now;
                OnSecondLoop();
            }
        }

        private static void OnSecondLoop(){
            IEnumerator enumerator = secondDict.Values.GetEnumerator();
            while (enumerator.MoveNext()){
                Action a = enumerator.Current as Action;
                if (a != null){
                    a.DynamicInvoke();
                }
            }
        }

        private static void OnTimerLoop(){
            IEnumerator enumerator = timerDict.Values.GetEnumerator();
            while (enumerator.MoveNext()){
                Action a = enumerator.Current as Action;
                if (a != null){
                    a.DynamicInvoke();
                }
            }
        }

        private static void OnTimeoutLoop(){
            IEnumerator enumerator = timeoutDict.Values.GetEnumerator();
            float now = Time.time;
            while (enumerator.MoveNext()){
                CacheTimeout ct = (CacheTimeout) enumerator.Current;
                if (now >= ct.endTime){
                    try{
                        if (ct.args == null){
                            ct.action.DynamicInvoke();
                        }
                        else{
                            ct.action.DynamicInvoke(ct.args);
                        }
                    }
                    catch{
                        //TopTip.addTip(ct.action.Method.Name + "执行出错");
                    }
                    ct.isAdd = false;
                    timeoutCache.Add(ct);
                }
            }
        }

        private static bool CheckInFrame(object key){
            if (frameCache.Count > 0){
                for (int i = frameCache.Count - 1; i >= 0; i--){
                    if (frameCache[i].key == key){
                        return frameCache[i].isAdd;
                    }
                }
            }

            return frameDict.ContainsKey(key);
        }

        private static bool CheckInTimer(object key){
            if (timerCache.Count > 0){
                for (int i = timerCache.Count - 1; i >= 0; i--){
                    if (timerCache[i].key == key){
                        return timerCache[i].isAdd;
                    }
                }
            }

            return timerDict.ContainsKey(key);
        }

        private static bool CheckInSecond(object key){
            if (secondCache.Count > 0){
                for (int i = secondCache.Count - 1; i >= 0; i--){
                    if (secondCache[i].key == key){
                        return secondCache[i].isAdd;
                    }
                }
            }
            return secondDict.ContainsKey(key);
        }

        public static void AddToFrame(object key, Action a){
            if (CheckInFrame(key) == true) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = true;
            ca.action = a;
            frameCache.Add(ca);
        }

        public static void RemoveFromFrame(object key){
            if (CheckInFrame(key) == false) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = false;
            frameCache.Add(ca);
        }

        public static void AddToTimer(object key, Action a){
            if (CheckInTimer(key) == true) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = true;
            ca.action = a;
            timerCache.Add(ca);
        }

        public static void RemoveFromTimer(object key){
            if (CheckInTimer(key) == false) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = false;
            timerCache.Add(ca);
        }

        public static void AddToSecond(object key, Action a){
            if (CheckInSecond(key) == true) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = true;
            ca.action = a;
            secondCache.Add(ca);
        }

        public static void RemoveFromSecond(object key){
            if (CheckInSecond(key) == false) return;
            CacheAction ca = new CacheAction();
            ca.key = key;
            ca.isAdd = false;
            secondCache.Add(ca);
        }

        public static int SetTimeout(Action a, float delay, params object[] args){
            return AddToimeout(a, delay, args);
        }

        public static int SetTimeout<T1>(Action<T1> a, float delay, params object[] args){
            return AddToimeout(a, delay, args);
        }

        public static int SetTimeout<T1, T2>(Action<T1, T2> a, float delay, params object[] args){
            return AddToimeout(a, delay, args);
        }

        public static int SetTimeout<T1, T2, T3>(Action<T1, T2, T3> a, float delay, params object[] args){
            return AddToimeout(a, delay, args);
        }

        public static int SetTimeout<T1, T2, T3, T4>(Action<T1, T2, T3, T4> a, float delay, params object[] args){
            return AddToimeout(a, delay, args);
        }

        public static void ClearTimeout(int key){
            CacheTimeout ct = new CacheTimeout();
            ct.key = key;
            ct.isAdd = false;
            timeoutCache.Add(ct);
        }

        private static int AddToimeout(Delegate a, float delay, params object[] args){
            if (delay <= 0.001){
                if (a == null) return 0;
                try{
                    if (args == null){
                        a.DynamicInvoke();
                    }
                    else{
                        a.DynamicInvoke(args);
                    }
                }
                catch{
                    //TopTip.addTip(a.Method.Name + "执行出错");
                }
                return 0;
            }
            delayIDKey++;
            CacheTimeout ct = new CacheTimeout();
            ct.isAdd = true;
            ct.key = delayIDKey;
            ct.action = a;
            ct.endTime = Time.time + delay;
            ct.args = args;
            timeoutCache.Add(ct);
            return delayIDKey;
        }

    }
}
