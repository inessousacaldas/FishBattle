using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public class ContainerController<C> where C: IViewController
    {
        protected bool _hasSetup = false;
        protected List<C> _itemList = new List<C>();
        protected Func<C> _createFunc;
        protected Action<C> _disposeAction;

        public List<C> ItemList
        {
            get { return _itemList; }
        }

        public C CurrentItem
        {
            get
            {
                return _useCount > 0
                    ? _itemList[_useCount - 1]
                    : default(C);
            }
        }

        protected int _useCount;

        public int UseCount
        {
            get { return _useCount; }
        }

        public virtual void SetData(Func<C> createFunc, Action<C> disposeAction = null)
        {
            _createFunc = createFunc;
            _disposeAction = disposeAction;

            Setup();
        }

        protected virtual void Setup()
        {
            if (!_hasSetup)
            {
                _hasSetup = true;
                Init();
            }
        }

        protected virtual void Init()
        {
            _useCount = 0;
        }

        public virtual void Dispose()
        {
            if (_hasSetup)
            {
                _hasSetup = false;
                OnDispose();
            }
        }

        protected virtual void OnDispose()
        {
            if (_disposeAction != null)
            {
                for (int i = _itemList.Count - 1; i >= 0; i--)
                {
                    _disposeAction(_itemList[i]);
                }
            }
            _itemList.Clear();
            _useCount = 0;
            _createFunc = null;
            _disposeAction = null;
        }


        protected virtual void AddItem(int index)
        {
            if (!IsUseCache(index))
            {
                var item = _createFunc();
                _itemList.Add(item);
            }

            _useCount++;
            CurrentItem.BaseView.gameObject.SetActive(true);
        }

        public virtual void UpdateItemList(int num, Action<int, C> setDataAction)
        {
            StartAdding();

            for (int i = 0; i < num; i++)
            {
                AddItem(i);
                setDataAction(i, CurrentItem);
            }

            EndAdding();
        }

        /// <summary>
        /// 检查是否需要往里面塞东西
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual bool IsUseCache(int index)
        {
            return index < _itemList.Count;
        }


        protected virtual void StartAdding()
        {
            _useCount = 0;
        }


        protected virtual void EndAdding()
        {
            for (int i = _itemList.Count - 1; i >= _useCount; i--)
            {
                _itemList[i].BaseView.gameObject.SetActive(false);
            }
        }
    }
}
#endif