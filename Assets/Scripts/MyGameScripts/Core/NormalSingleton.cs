
//namespace BaseClassNS
//{
//    public abstract class NormalSingleton<T> where T : NormalSingleton<T>, new()
//    {
//        protected static T _instance;

//        public static T Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    lock (typeof(T))
//                    {
//                        if (_instance == null)
//                        {
//                            _instance = new T();
//                            _instance.Init();
//                            _instance.Reset();
//                        }
//                    }
//                }
//                return _instance;
//            }
//        }

//        public static bool HasInstance
//        {
//            get { return _instance != null; }
//        }


//        public virtual void Setup()
//        { }

//        protected virtual void Init() { }


//        /// <summary>
//        /// 释放某些资源之类
//        /// </summary>
//        protected virtual void Relesase() { }


//        public static void DestroyInstance()
//        {
//            if (_instance != null)
//            {
//                _instance.Relesase();
//                _instance = null;
//            }
//        }


//        /// <summary>
//        /// 重置某些值到初始状态，但是像一些委托之类的不应该进行重置
//        /// </summary>
//        public virtual void Reset() { }
//    }
//}