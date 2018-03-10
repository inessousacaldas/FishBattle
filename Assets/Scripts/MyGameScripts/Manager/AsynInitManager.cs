// Author fish 2017.6.19
using System;
using System.Collections.Generic;

namespace Asyn
{
    public interface IAsynInit
    {
        void StartAsynInit(Action<IAsynInit> onComplete);
    }

    public abstract class AbstractAsynInit : IAsynInit
    {
        public AbstractAsynInit()
        {
            AsynInitManager.Register(this);
        }

        public abstract void StartAsynInit(Action<IAsynInit> onComplete);
    }

    public class AsynInitManager
    {
        private static List<IAsynInit> _handlerList = new List<IAsynInit>();
        public static void Register(IAsynInit handler)
        {
            if (handler == null) throw new ArgumentException(handler.ToString());
            _handlerList.Add(handler);//重复注册就会抛异常
        }

        public static void StartInit()
        {
            if (_handlerList.IsNullOrEmpty())
            {
                OnAllComplete();
                return;
            }
            foreach (var handler in _handlerList.ToArray())//为了避免handler执行同步回掉导致在迭代器里面删除元素，用ToArray复制一份来执行
            {
                handler.StartAsynInit(OneComplete);
            }
        }

        private static void OneComplete(IAsynInit handler)
        {
            if (handler == null) return;
            if (!_handlerList.Remove(handler))
            {
                GameDebuger.LogWarning(string.Format("警告：有一个模块的数据层没有添加注册：{0} ",handler.GetType()));
            }
            else
            {
                GameDebuger.LogWarning(string.Format("模块数据层初始化完成: {0} is finished，还剩{1}个还没初始化:{2}", handler.GetType(),_handlerList.Count,GetHandlerName()));
            }

            CheckAllComplete();
        }

        private static void CheckAllComplete()
        {
            if (_handlerList.Count <= 0 && OnAllComplete != null)
            {
                OnAllComplete();
                GameDebuger.Log("成功初始化模块数据层，可以进入游戏了");
            }
        }

        public static event Action OnAllComplete;

        private static string GetHandlerName()
        {
            var msg = "";
            for(var i = 0;i < _handlerList.Count;i++)
            {
                msg += _handlerList[i].GetType() + ",";
            }
            return msg;
        }
    }
}