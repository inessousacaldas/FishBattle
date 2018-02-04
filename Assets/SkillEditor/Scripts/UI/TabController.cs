using System;
using System.Collections.Generic;
#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public static class TabConst
    {
        public const int NoneSelected = -1;
        public const int DefaultSelected = 0;
    }

    public class TabController<T> where T : ITabItemController
    {
        private bool _hasSetup = false;
        private const int NoneInitSelected = -2;

        private int _currentSelectedIndex;

        /// <summary>
        /// 是否可以多次点选同一个物体
        /// 一般只有那种点击之后会增加数量的才需要开启
        /// </summary>
        private bool _canTriggerSame;
        private bool _canBeNone;

        public int CurrentSelectedIndex
        {
            get { return _currentSelectedIndex; }
        }

        private List<T> _tabList = new List<T>();
        public List<T> TabList
        {
            get { return _tabList; }
        }

        public T CurrentSelected
        {
            get
            {
                return _currentSelectedIndex >= 0 && _currentSelectedIndex < _tabList.Count
                    ? _tabList[_currentSelectedIndex]
                    : default(T);
            }
        }

        private Action<int, int> _selectedAction;
        private Func<int, bool> _checkFunc;
        private bool _needCalback;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="selctedCallback"></param>
        /// <param name="check"></param>
        public void SetData(Action<int, int> selcted = null, bool canTriggerSame = false, bool canBeNone = false,
            Func<int, bool> check = null)
        {
            _selectedAction = selcted;
            _checkFunc = check;
            _canTriggerSame = canTriggerSame;
            _canBeNone = canBeNone;

            Setup();
        }

        private void Setup()
        {
            if (!_hasSetup)
            {
                _hasSetup = true;
                Init();
            }
        }


        private void Init()
        {
            _needCalback = true;

            ClearTab();
        }

        /// <summary>
        /// 不是一定要调用的
        /// 对于只执行一次Setup的，可以不执行
        /// </summary>
        public void Dispose()
        {
            if (_hasSetup)
            {
                _hasSetup = false;
                OnDispose();
            }
        }


        private void OnDispose()
        {
            _checkFunc = null;
            _selectedAction = null;
            _needCalback = false;
            ClearTab();
        }

        public void UpdateTabList(List<T> tabList, int num)
        {
            ClearTab();
            for (int i = 0; i < num; i++)
            {
                AddTab(tabList[i], i);
            }
        }

        private void AddTab(T tab, int index)
        {
            _tabList.Add(tab);
            tab.SetSelected(false);
            tab.SetOnClickAction(index, SetSelected);
        }

        private void ClearTab()
        {
            for (int i = _tabList.Count - 1; i >= 0; i--)
            {
                _tabList[i].SetOnClickAction(TabConst.NoneSelected, null);
            }
            _tabList.Clear();
            _currentSelectedIndex = NoneInitSelected;
        }

        public void SetSelected(int index)
        {
            if (index == _currentSelectedIndex && !_canTriggerSame)
            {
                return;
            }

            if (_checkFunc == null ||
                _checkFunc(index))
            {
                var lIndex = _currentSelectedIndex;
                if (lIndex != index)
                {
                    if (lIndex >= 0
                        && lIndex < _tabList.Count)
                    {
                        _tabList[lIndex].SetSelected(false);
                    }

                    if (index >= 0 &&
                        index < _tabList.Count)
                    {
                        _tabList[index].SetSelected(true);
                        _currentSelectedIndex = index;
                    }
                    else
                    {
                        _currentSelectedIndex = TabConst.NoneSelected;
                    }
                }
                else if (_canBeNone)
                {
                    // 取消选中
                    _tabList[index].SetSelected(false);
                    _currentSelectedIndex = TabConst.NoneSelected;
                }

                if (_needCalback && _selectedAction != null && _tabList.Count != 0)
                {
                    _selectedAction(_currentSelectedIndex, lIndex);
                }
            }
        }
    }
}
#endif