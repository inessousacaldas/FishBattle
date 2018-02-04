using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetPipeline;

[AddComponentMenu("NGUI/Interaction/PageScrollView")]
[RequireComponent(typeof(UIPageGrid))]
public class PageScrollView : MonoBehaviour
{

    public float springStrength = 8.0f;
    public float nextPageThreshold = 0f;

    public UIGrid pageFlagGrid;

    public UIAtlas PageFlagAtlas
    {
        get { return pageFlagAtlas; }
        set
        {
            if (value != pageFlagAtlas)
            {
                AssetManager.RemoveAtlasRef(pageFlagAtlas, this);
                pageFlagAtlas = value;
                AssetManager.AddAtlasRef(pageFlagAtlas, this);
            }
        }
    }
    [SerializeField]
    private UIAtlas pageFlagAtlas;

    //改成public,方便在做预制的时候用不同的标签页
    public string onPageFlagName = "page_choice";
    public string offPageFlagName = "page_under";

    private int curPage = 1;
    public int CurPage
    {
        get
        {
            return curPage;
        }
    }

    private int maxPage;
    public int MaxPage
    {
        get
        {
            return maxPage;
        }
    }

    private int elementsPerPage;
    public int ElementsPerPage
    {
        get
        {
            return elementsPerPage;
        }
    }

    private UIScrollView mScrollView;
    private Vector3 startingPanelPosition;

    private UIPageGrid mPageGrid;
    public UIPageGrid PageGrid
    {
        get
        {
            return mPageGrid;
        }
    }

    private Transform mTrans;

    public delegate void OnChangePage(int curPage);
    public OnChangePage onChangePage;

    // Use this for initialization
    void Awake()
    {
        AssetManager.AddAtlasRef(pageFlagAtlas, this);
        if (mScrollView == null)
        {
            mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
            if (mScrollView == null)
            {
                Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " object in order to work", this);
                enabled = false;
                return;
            }
            mScrollView.scrollWheelFactor = 0f;
            nextPageThreshold = Mathf.Abs(nextPageThreshold);

            //禁用以下选项，为了保证可以按页切换
            mScrollView.restrictWithinPanel = false;
            mScrollView.usePageScroll = true;
            startingPanelPosition = mScrollView.transform.localPosition;

            mTrans = this.transform;
            mPageGrid = this.GetComponent<UIPageGrid>();
            elementsPerPage = mPageGrid.maxRow * mPageGrid.maxCol;
            mPageGrid.onReposition = OnRepositionItem;
        }
    }

    void OnEnable()
    {
        if (mScrollView != null)
        {
            mScrollView.onDragStarted += OnDragStart;
            mScrollView.onDragFinished += OnDragFinished;
        }
    }

    void OnDisable()
    {
        if (mScrollView != null)
        {
            mScrollView.onDragStarted -= OnDragStart;
            mScrollView.onDragFinished -= OnDragFinished;
        }
    }

    //UIPageGrid重置时重新设置最大页数，隐藏的Item不计算
    void OnRepositionItem()
    {
        maxPage = Mathf.CeilToInt((float)mPageGrid.childCount / (float)elementsPerPage);
        UpdatePageFlagCount();
    }

    private Vector2 mLastClipOffset;
    void OnDragStart()
    {
        mLastClipOffset = mScrollView.panel.clipOffset;
    }

    void OnDragFinished()
    {
        Vector2 draggedOffset = mScrollView.panel.clipOffset - mLastClipOffset;
        int draggedPage = 0;
        float delta = 0f;
        if (mPageGrid.arrangement == UIPageGrid.Arrangement.Horizontal)
        {
            draggedPage = Mathf.CeilToInt(Mathf.Abs(draggedOffset.x) / (mPageGrid.cellWidth * mPageGrid.maxCol));
            delta = draggedOffset.x;
        }
        else
        {
            draggedPage = Mathf.CeilToInt(Mathf.Abs(draggedOffset.y) / (mPageGrid.cellHeight * mPageGrid.maxRow));
            delta = -draggedOffset.y;
        }

        //		Debug.LogError("Dragged Delta: "+delta);
        if (enabled)
        {
            if (delta > nextPageThreshold)
                SkipToPage(curPage + draggedPage, true);
            else if (delta < -nextPageThreshold)
                SkipToPage(curPage - draggedPage, true);
            else
                SkipToPage(curPage, true);
        }
    }

    public void NextPage()
    {
        if (mScrollView != null)
        {
            SkipToPage(curPage + 1, true);
        }
    }

    public void PreviousPage()
    {
        if (mScrollView != null)
        {
            SkipToPage(curPage - 1, true);
        }
    }

    public void SkipToPage(int targetPage, bool hasSpring)
    {
        if (mScrollView == null) return;

        curPage = Mathf.Clamp(targetPage, 1, maxPage);
        Vector3 nextScrollPos = Vector3.zero;
        if (mPageGrid.arrangement == UIPageGrid.Arrangement.Horizontal)
            nextScrollPos = new Vector3((mPageGrid.cellWidth + mPageGrid.pageGap) * mPageGrid.maxCol * (curPage - 1), 0.0f, 0.0f);
        else
            nextScrollPos = new Vector3(0f, -(mPageGrid.cellHeight + mPageGrid.pageGap) * mPageGrid.maxRow * (curPage - 1), 0f);

        Vector3 targetPos = startingPanelPosition - nextScrollPos;

        SpringPanel sp = mScrollView.GetComponent<SpringPanel>();
        if (sp != null) sp.enabled = false;

        if (hasSpring)
        {
            SpringPanel.Begin(mScrollView.panel.cachedGameObject, targetPos, springStrength);
        }
        else
        {
            Vector3 before = mScrollView.panel.cachedTransform.localPosition;
            mScrollView.panel.cachedTransform.localPosition = targetPos;

            Vector3 offset = targetPos - before;
            Vector2 cr = mScrollView.panel.clipOffset;
            cr.x -= offset.x;
            cr.y -= offset.y;
            mScrollView.panel.clipOffset = cr;
        }

        ChangePageFlagState();

        if (onChangePage != null)
            onChangePage(curPage);
    }

    private List<UISprite> _pageFlagList = new List<UISprite>();
    public void UpdatePageFlagCount()
    {
        if (pageFlagGrid == null) return;

        Transform pageGridTrans = pageFlagGrid.transform;
        for (int i = 0; i < pageGridTrans.childCount; ++i)
        {
            Transform child = pageGridTrans.GetChild(i);
            child.gameObject.SetActive(false);
            UISprite sprite = child.GetComponent<UISprite>();
            //增加条件 && !_pageFlagList.Contains(sprite)  若有问题 todo xjd
            if (sprite != null && !_pageFlagList.Contains(sprite))
                _pageFlagList.Add(sprite);
        }

        if (pageFlagAtlas == null || maxPage == 1) return;

        if (_pageFlagList.Count < maxPage)
        {
            for (int i = _pageFlagList.Count; i < maxPage; ++i)
            {
                UISprite sprite = NGUITools.AddSprite(pageFlagGrid.gameObject, pageFlagAtlas, offPageFlagName);
                sprite.cachedGameObject.name = "pageSprite";
                sprite.depth = 4;
                sprite.MakePixelPerfect();
                _pageFlagList.Add(sprite);
            }
        }

        for (int i = 0; i < maxPage; ++i)
        {
            _pageFlagList[i].cachedGameObject.SetActive(true);
        }

        ChangePageFlagState();
        pageFlagGrid.Reposition();
    }

    private void ChangePageFlagState()
    {
        if (pageFlagGrid == null || pageFlagAtlas == null || maxPage == 1) return;

        for (int i = 0; i < maxPage; ++i)
        {
            if (curPage == i + 1)
            {
                _pageFlagList[i].spriteName = onPageFlagName;
            }
            else
                _pageFlagList[i].spriteName = offPageFlagName;
        }
    }

    void OnDestroy()
    {
        AssetManager.RemoveAtlasRef(pageFlagAtlas, this);
    }
}