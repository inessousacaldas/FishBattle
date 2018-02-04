// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemsPageContainerController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************


using System;
using System.Collections.Generic;
using UniRx;

public interface IItemsPageContainerController
{
    UniRx.IObservable<int> PageMoveAsObserver { get; }
    UniRx.IObservable<int> OnItemClick { get; }

    void UpdateView(
        int curPageNum
        , int maxPageNum
        , int itemsCnt
        , IEnumerable<object> cellDatas
        , bool showLocks
        , int curSelect =-1
        , int tabIdx = -1
        , Action updateFinish = null);
}

/// <summary>
/// 背包的所有页数~
/// </summary>
public partial class ItemsPageContainerController
    :MonolessViewController<ItemsPageContainer>
    , IItemsPageContainerController
{
    private bool _showLock = false;
    private TabbtnManager tabMgr = null;
    private readonly List<ItemContainerController> _itemContainerControllerList = new List<ItemContainerController>(5);
    
    public UniRx.IObservable<int> OnItemDoubleClick {
        get { return itemDoubleClickEvt; }
    }

    private Subject<int> itemDoubleClickEvt = new Subject<int>();
    
    public UniRx.IObservable<int> OnItemClick {
        get { return itemClickEvt; }
    }

    private Subject<int> itemClickEvt = new Subject<int>();
    private CompositeDisposable _disposable = null;

    private static readonly string TimerName = "addPage";
    private int pageNum = 0;

    public Subject<int> pageChanged;
    public UniRx.IObservable<int> PageMoveAsObserver
    {
        get {return pageChanged; }
    }
        
    protected override void InitReactiveEvents()
    {
        _disposable = new CompositeDisposable();
        pageChanged = new Subject<int>();
        pageChanged.Hold(0);
        View.Container_UIScrollView.PressUpCallback = delegate
        {
            if (pageChanged.LastValue != View.pageOnCenter)
            {
                pageChanged.OnNext(View.pageOnCenter);
            }    
        };
    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
        if (tabMgr != null)
        {
            tabMgr.Dispose();
            tabMgr = null;
        }

        if (_disposable != null)
            _disposable.Dispose();
        itemClickEvt = itemClickEvt.CloseOnceNull();
        itemDoubleClickEvt = itemDoubleClickEvt.CloseOnceNull();
        pageChanged = pageChanged.CloseOnceNull();
    }

    /// <summary>
    /// 背包上面的Tab
    /// </summary>
    /// <param name="nameSet"></param>
    /// <param name="tabSelectIdx"></param>
    public void UpdateTabBtns(IEnumerable< ITabInfo> nameSet, int tabSelectIdx)
    {
        Func<int, ITabBtnController> func = i => { return AddChild<TabBtnWidgetController, TabBtnWidget>(
            View.TabsGroup_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
            , "Tabbtn_" + i);};

        if(tabMgr == null)
        {
            tabMgr = TabbtnManager.Create(nameSet, func);
            tabMgr.SetTabBtn(tabSelectIdx);
            tabMgr.SetBtnLblFont(20, "444244FF", 18, "B5BAB5FF");
            tabMgr.Stream.Subscribe(i => {
                _onTabbtnClickStream.OnNext(i);
            });
        }
        else
        {
            tabMgr.UpdateTabs(nameSet, tabSelectIdx);
        }
        View.TabsGroup_UIGrid.Reposition();

        
    }
    private UniRx.Subject<int> _onTabbtnClickStream = new Subject<int>();
    public UniRx.IObservable<int> OnTabbtnClick
    {
        get { return _onTabbtnClickStream; }
    }
        
    private Action _finish;
    public void UpdateView(
        int curPageNum
        , int maxPageNum
        , int itemsCnt
        , IEnumerable<object> cellDatas
        , bool showLocks
        , int curSelect = -1
        , int tabIdx = -1
        , Action updateFinish = null)
    {
        JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
        GameUtil.SafeRun(_finish);
        _finish = updateFinish;

        GameLog.Log_BAG("page------"+ curPageNum);
        _showLock = showLocks;

        View.PageGroup_UIPageGroup.UpdatePage(maxPageNum);

        UpdateItemContainer(itemsCnt, maxPageNum, cellDatas, delegate
        {
            if (this.gameObject.activeInHierarchy){
                //                View.PageMove(curPageNum);
                pageChanged.Hold(curPageNum);
                View.SetPage(curPageNum, maxPageNum);
            }
            
            GameUtil.SafeRun(_finish);
            _finish = null;
        });
        SetSelect(curSelect);
        if (tabMgr != null)
        {
            tabMgr.SetTabBtn(tabIdx);
        }
    }

    public void ShowPageGroup(bool isShow)
    {
        View.PageGroup_UIPageGroup.gameObject.SetActive(isShow);
    }
    //设置物品的选中
    public void SetSelect(int idx)
    {
        int page = GetPage(idx);
        _itemContainerControllerList.ForEachI(delegate(ItemContainerController controller, int i) {
            var _idx = i == page ? idx - ItemsContainerConst.PageCapability * page : -1;
            controller.SetSelect(_idx);
        });
    }

    /// <summary>
    /// 根据Index获取该Index所在的页码~
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    private int GetPage(int idx)
    {
        return  idx >= 0
           ? idx / ItemsContainerConst.PageCapability
           : -1;
    }
    public void SetMultipleSelect(IEnumerable<int> select)
    {
        //{page:idxSet}
        var indexDic = new Dictionary<int, List<int>>();
        select.ForEach(idx=>
        {
            var page = idx / ItemsContainerConst.PageCapability;
            if (indexDic.ContainsKey(page))
            {
                indexDic[page].AddIfNotExist(idx);
            }
            else
            {
                indexDic.Add(page, new List<int>(){idx});
            }
        });
        
        indexDic.ForEach(kv =>
        {
            var ctrl = _itemContainerControllerList.TryGetValue(kv.Key);
            if (ctrl != null)
                ctrl.SetMultipleSelect(kv.Value);
        });        
    }

    public void SetPage(int page){
        View.PageMove(page);
    }


    //对于长度不限的容器，pageNum <= 0
    private void UpdateItemContainer(
        int itemNum
        , int pageNum
        ,IEnumerable<object> items
        , Action onTimerFinish)
    {
//        GameUtil.Log_BAG("pageNum   " + pageNum);
        this.pageNum = pageNum > 0 ? pageNum : (itemNum + ItemsContainerConst.PageCapability) / ItemsContainerConst.PageCapability;
        var pageIdx = 0;

//        GameUtil.Log_BAG("page   " + page);
        startTimer(pageIdx, itemNum, items.ToList(), onTimerFinish);
    }
    /// <summary>
    /// 对更新页面进行分帧处理
    /// </summary>
    /// <param name="pageIdx"></param>
    /// <param name="itemNum"></param>
    /// <param name="items"></param>
    /// <param name="onTimerFinish"></param>
    private void startTimer(
        int pageIdx
        , int itemNum
        , List<object> items
        , Action onTimerFinish){

        if (pageIdx >= pageNum){
            HideBeyond(pageIdx);
            View.ItemContainerGrid_UIGrid.Reposition();
            GameUtil.SafeRun(onTimerFinish);
            return;
        }

        if (_itemContainerControllerList.Count > pageIdx)
        {
            UpdateItemContainerController(pageIdx, itemNum, items);
            pageIdx ++;
            startTimer(pageIdx, itemNum, items, onTimerFinish);
            return;
        }

        var timeGap = 0.03f;
        JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
        JSTimer.Instance.SetupCoolDown(TimerName + this.GetHashCode(), timeGap, null, delegate {
            AddIfNotExistPage(pageIdx);
            UpdateItemContainerController(pageIdx, itemNum, items);
            pageIdx ++;
            startTimer(pageIdx, itemNum, items, onTimerFinish);
        });
    }

    private void UpdateItemContainerController(int _pageIdx, int itemNum, List<object> items){
        var start = _pageIdx * ItemsContainerConst.PageCapability;

        var length = _showLock
            ? Math.Min(itemNum - start, ItemsContainerConst.PageCapability)
            : ItemsContainerConst.PageCapability;

        var l = 0;
        var pageItems = items.GetElememtsByRange(
            start
            , length
            , out l);

        UpdateItems(_pageIdx, length, pageItems);
    }

    private void UpdateItems(
        int curPage
        , int length
        , IEnumerable<object> dataSet)
    {
        ItemContainerController ctrl = null;
        _itemContainerControllerList.TryGetValue(curPage, out ctrl);
        if (ctrl == null)
        {
            GameDebuger.LogError("ctrl is null");
            return;
        }
        ctrl.Show();
        ctrl.UpdateView(dataSet, curPage * ItemsContainerConst.PageCapability, length);
        
    }

    private void AddIfNotExistPage(int curPage)
    {
        ItemContainerController container = null;
        _itemContainerControllerList.TryGetValue<ItemContainerController>(curPage, out container);

        if (container == null)
        {
            container = AddChild<ItemContainerController, ItemContainer>(
                View.ItemContainerGrid_UIGrid.gameObject
                , ItemContainer.NAME
                , curPage.ToString());
            var pageInfo = container.gameObject.GetMissingComponent<UIPageInfo>();
            pageInfo.page = curPage;
            _itemContainerControllerList.Add(container);
            View.ItemContainerGrid_UIGrid.Reposition();

            _disposable.Add(container.OnItemClick.Subscribe(i => {
                itemClickEvt.OnNext(curPage * ItemsContainerConst.PageCapability + i);
            } ));
            
            _disposable.Add(container.OnItemDoubleClick.Subscribe(
                    i=> itemDoubleClickEvt.OnNext(curPage * ItemsContainerConst.PageCapability + i)
                )
            );
        }
    }

    private void HideBeyond(int page)
    {
        if (_itemContainerControllerList.IsNullOrEmpty())
            return;
        var cnt = _itemContainerControllerList.Count;
        while (page < cnt)
        {
            _itemContainerControllerList[page].SetActive(false);
            page++;
        }
    }

    public ItemCellController GetItemCellCotnroller(int index)
    {
        int page = GetPage(index);
        var c_index =   index - ItemsContainerConst.PageCapability * page ;
        if(_itemContainerControllerList.Count <= page)
        {
            return null;
        }
        var cell = _itemContainerControllerList[page].GetCell(c_index);
        return cell;
    }
}