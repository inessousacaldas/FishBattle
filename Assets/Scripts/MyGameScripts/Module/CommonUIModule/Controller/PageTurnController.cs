// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PageTurnViewController.cs
// Author   : fish
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum ShowType
{
    blank,
    numType,
    singleNum,
    singleNum_Zero, //从0开始
    stringType,
}

public interface IPageTurn
{
    UniRx.IObservable<int> Stream { get; }
    int CurrentPage { get; }
    
    void InitData(
        int cur
        ,int min
        , int max
        , bool _isCycle = false
        , ShowType showType = ShowType.numType
        , IEnumerable<string> contentSet = null
        , bool showNameList = false
        , bool enableInput = false);
}

public partial class PageTurnViewController
    :MonolessViewController<PageTurnView>
, IPageTurn
{
    private enum RollDir
    {
        left,
        right
    }
    /// <summary>
    /// 小键盘位置显示的位置
    /// </summary>
    public enum InputerShowPos
    {
        Up,
        Down,
    }

    private Subject<int> stream;
    public UniRx.IObservable<int> Stream{get{ return stream;}}
    public UniRx.IObservable<int> OnMaxStream { get {
            return stream.Where(x => x >= _maxPage - 1 );
        } }
    private CompositeDisposable _disposable;

    private int _currentPage = 0;  //  程序员计数从0开始  －－ fish
    public int CurrentPage {
        get { return _currentPage; }
        private set{_currentPage = value;}
    }

    private int _maxPage;
    private int _minPage;
    private bool isCycle;
    private bool _showNameList;
    private ShowType _showType;
    private List<string> pageContentSet;
    private bool isShowInputter;
    private bool isShowPopTips;

    private SimpleNumberInputerController numberInputer;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        stream = new Subject<int>();
        stream.Hold(0);
        pageContentSet = new List<string>();
        _disposable = new CompositeDisposable();
        
    }

    protected override void OnDispose()
    {
        stream = stream.CloseOnceNull();
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnPageBtnClickAsObservable(View.LeftBtn_UIButton, RollDir.left);
        OnPageBtnClickAsObservable(View.RightBtn_UIButton, RollDir.right);
        if (_view.MaxBtnActivated)
        {
            _disposable.Add(_view.MaxBtn_UIButton.AsObservable().Subscribe(_ =>
            {
                if (CurrentPage != (_maxPage - 1))
                {
                    CurrentPage = _maxPage - 1;
                    stream.OnNext(CurrentPage);
                }
            }));    
        }

        if (_view.MixBtnActivated)
        {
            _disposable.Add(_view.MinBtn_UIButton.AsObservable().Subscribe(_ =>
            {
                if (CurrentPage != 0)
                {
                    CurrentPage = 0;
                    stream.OnNext(CurrentPage);
                }
            }));
        }
    }

    private void OnPageBtnClickAsObservable(UIButton btn, RollDir dir)
    {
        if (btn == null)
            return ;
        var list = UIEventListener.Get(btn.gameObject);
        if (list == null) {
            Debug.LogError("can not create UIEventListener!");
            return ;
        }

        _disposable.Add(btn.AsObservable().Subscribe(_=>{
            var change = ChangePage(dir);
            if (change)
            {
                stream.OnNext(CurrentPage);
                //UpdateView();
            }
                
        })
        );
    }

    //是否支持页面循环
    private bool ChangePage(RollDir dir)
    {
        var isChanged = false;
        switch (dir)
        {
            case RollDir.left:
            {
                if (isCycle == false)
                {
                    var tmp = Mathf.Max (_minPage, CurrentPage - 1);
                    isChanged = tmp != CurrentPage;
                    CurrentPage = tmp;
                }
                else
                {
                    CurrentPage = CurrentPage > _minPage ? CurrentPage - 1 : _maxPage - 1;
                    isChanged = true;
                }
            }
                break;
            case RollDir.right:
            {
                if (isCycle == false)
                {
                    var tmp = Mathf.Min (_maxPage - 1, CurrentPage + 1);
                    isChanged = tmp != CurrentPage;
                    CurrentPage = tmp;
                    if(CurrentPage == _maxPage - 1)
                        OnValueMaxHandler();
                }
                else
                {
                    CurrentPage = CurrentPage < _maxPage - 1 ? CurrentPage + 1 : _minPage;
                    isChanged = true;
                }
            }
                break;
        }

        return isChanged;
    }

    public void InitData(
        int cur = 0
        , int min = 0
        , int max = 0
        , bool _isCycle = false
        , ShowType showType = ShowType.numType
        , IEnumerable<string> contentSet = null
        , bool showNameList = false
        , bool enableInput = false)
    {
        isCycle = _isCycle;
        _showType = showType;
        CurrentPage = cur;
        _minPage = 0;
        _maxPage = max;
        _showNameList = showNameList;

        UpdateView(CurrentPage, _maxPage, contentSet);
        View.nameMenu_UIPopupList.enabled = _showNameList;
        
        if (_showNameList)
        {
             _disposable.Add(
                 _view.nameMenu_UIPopupList.OnValueChangedAsObservable()
                .Subscribe(
                    idx=>stream.OnNext(idx)
                ));
        }

        _view.input_UIInput.enabled = enableInput;
        if (enableInput)
        {
            _disposable.Add(_view.input_UIInput.OnSubmitAsObservable().Subscribe(
                str =>
                {
                    var temp = 0;
                    int.TryParse(str, out temp);
                    temp = Math.Min(temp, _maxPage - 1);
                    if (_currentPage == temp) return;
                    SetPageLabel();
                    stream.OnNext(_currentPage);
                }
            ));
        }
    }

    #region 使用方式为小键盘形式的~~
    /// <summary>
    /// 使用小键盘的形式初始化该控件
    /// </summary>
    /// <param name="cur">当前数值</param>
    /// <param name="max">最大数值</param>
    /// <param name="showType"></param>
    /// <param name="showNumberInputer">是否展示小键盘</param>
    /// <param name="pos">小键盘的位置</param>
    /// <param name="isShowPopTips">是否弹出提示 在数值满后</param>
    /// <param name="prefabPath">小键盘的 Prefab位置</param>
    public void InitData_NumberInputer(
        int cur = 0,
        int min = 0,
        int max = 9999,
        bool showNumberInputer = true,
        InputerShowPos pos =InputerShowPos.Up, 
        bool isShowPopTips = true,
        string prefabPath = SimpleNumberInputer.NAME)
    {
        //_disposable.Clear();
        _minPage = min;
        _maxPage = max + 1;
        if (cur < min)
            cur = min;
        else if (cur > max)
            cur = max;
        CurrentPage = cur;
        
        

        this.isShowPopTips = isShowPopTips;
        isShowInputter = showNumberInputer;

        if(View.nameMenu_UIPopupList.GetComponent<UIPopupList>() != null)
            View.nameMenu_UIPopupList.RemoveComponent<UIPopupList>();
        if (View.nameMenu_UIPopupList.GetComponent<UIInput>() != null)
            View.input_UIInput.RemoveComponent<UIInput>();

        UIEventListener.Get(View.nameMenu_UIPopupList.gameObject).onClick = null;

        UIEventListener.Get(View.nameMenu_UIPopupList.gameObject).onClick += x => {
            if (showNumberInputer)
            {
                var parentGo = GetSimpleInputerPos(pos);
                numberInputer = ProxySimpleNumberModule.Open();
                numberInputer.InitData(min, _maxPage - 1, parentGo, Stream);
                numberInputer.OnValueChangeStream.Subscribe(s =>
                {
                    this.CurrentPage = s;
                    this.stream.OnNext(s);
                });
            }
        };
        //局部监听~方便管理
        _disposable.Add(Stream.Subscribe(x=> {
            UpdateView();
            
        }));
        _disposable.Add(OnMaxStream.Subscribe(x => { OnValueMaxHandler(); }));
        UpdateView();

    }
    
    private void OnValueMaxHandler()
    {
        if(isShowPopTips)
            TipManager.AddTopTip("单次输入已达最大数量");
    }
    private GameObject GetSimpleInputerPos(InputerShowPos pos)
    {
        GameObject go = null;
        switch(pos)
        {
            case InputerShowPos.Up:
                go = _view.Up_Transform.gameObject;
                break;
            case InputerShowPos.Down:
                go = _view.Down_Transform.gameObject;
                break;
        }
        if(go==null)
        {
            go = this.gameObject;
            GameDebuger.LogError("PageTurn Prefab 缺失 " + pos.ToString());
        }
        return go;
    }

    /// <summary>
    /// 强制刷新当前的数量  小键盘专用
    /// </summary>
    /// <param name="curPage"></param>
    /// <param name="maxPage"></param>
    public void SetPageInfo(int curPage,int maxPage)
    {
        maxPage = Math.Max(0, maxPage);

        _maxPage = maxPage+1;

        if (curPage > maxPage)
            curPage = maxPage - 1;

        CurrentPage = Math.Max(0, curPage);

        UpdateView();
    }

    /// <summary>
    /// 小键盘专用UpdateView
    /// </summary>
    public void UpdateView()
    {
        if (View.Label_UILabel == null)
            return;

        View.Label_UILabel.text = CurrentPage.ToString();
    }
    #endregion

    //用特定接口类型数据刷新界面
    public void UpdateView(
        int curPage
        , int maxPage = 0
        , IEnumerable<string> contentSet = null)
    { 
        maxPage = Math.Max(0, maxPage);

        _maxPage = maxPage;

        if (curPage >= maxPage)
            curPage = maxPage - 1;

        CurrentPage = Math.Max(0, curPage);

        switch (_showType)
        {
            case ShowType.numType:
                pageContentSet.Clear();
                for(var i = 1; i <= maxPage; i++){
                    pageContentSet.Add(string.Format("{0}/{1}", i, _maxPage));
                }
                break;
            case ShowType.singleNum:
                for(var i = pageContentSet.Count; i < maxPage; i++){
                    pageContentSet.ReplaceOrAdd(i, (i + 1).ToString());
                }
                break;
            case ShowType.singleNum_Zero:
                for (var i = pageContentSet.Count; i <= maxPage; i++)
                {
                    pageContentSet.ReplaceOrAdd(i, (i).ToString());
                }
                break;
            default:
                if (_showType == ShowType.stringType || contentSet != null)
                {
                    contentSet.ForEachI(delegate(string str, int i)
                    {
                        pageContentSet.ReplaceOrAdd(i, str);
                    });
                }
                break;
        }

        if (_showNameList)
        {
            UpdateNameSet();
        }

        SetPageLabel();
    }

 
    private void UpdateNameSet()
    {
        if (View.nameMenu_UIPopupList == null)
            return;
        View.nameMenu_UIPopupList.items = pageContentSet;
        View.nameMenu_UIPopupList.Set(pageContentSet.TryGetValue(CurrentPage), false);
    }

    private void SetPageLabel()
    {
        if (View.inputLabel_UILabel == null)
            return;
        var str = string.Empty;
        pageContentSet.TryGetValue(CurrentPage, out str);

        if (string.IsNullOrEmpty(str))
        {
            switch (_showType)
            {
                case ShowType.numType:
                    str = "0/0";
                    break;
                case ShowType.singleNum:
                    str = "0";
                    break;
                case ShowType.singleNum_Zero:
                    str = "0";
                    break;
                default:
                    break;
            }    
        }

        View.inputLabel_UILabel.text = str;
    }

    public bool IsLastPage
    {
        get { return CurrentPage == _maxPage - 1; }
    }

    public bool IsFirstPage
    {
        get { return CurrentPage == 0;}
    }
}
