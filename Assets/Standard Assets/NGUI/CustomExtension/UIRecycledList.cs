//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script makes it possible for a scroll view to wrap its content, creating endless scroll views.
///     Usage: simply attach this script underneath your scroll view where you would normally place a UIGrid:
///     + Scroll View
///     |- UIRecycledList
///     |-- Item 1
///     |-- Item 2
///     |-- Item 3
/// </summary>
public class UIRecycledList : MonoBehaviour
{
    #region Delegates

    public delegate void OnUpdateItem(GameObject go, int itemIndex, int realIndex);

    #endregion

    private readonly List<Transform> mChildren = new List<Transform>();

    private int _dataCount = -1;
    private int _lastDataCount = -1; //记录最近一次dataList数量

    public bool cullContent = true;

    /// <summary>
    ///     Width or height of the child items for positioning purposes.
    /// </summary>
    public int itemSize = 100;

    /// <summary>
    /// The width of each of the cells.
    /// </summary>
    public float cellWidth = 200f;

    /// <summary>
    /// The height of each of the cells.
    /// </summary>
    public float cellHeight = 200f;

    public int maxPerLine = 0;

    private Vector3 panelVec = new Vector3(0, 0, 0);

    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }
    public Arrangement arrangement = Arrangement.Horizontal;

    public enum SortingEnum
    {
        None,
        Alphabetical, //通过字母排序
        Horizontal,
        Vertical
    }

    public SortingEnum sorting = SortingEnum.None;

    private bool mFirstTime = true;
    private bool mHorizontal;
    private UIScrollView mScrollView;
    private UIPanel mPanel;

    private Transform mTrans;

    /// <summary>
    ///     Callback that will be called every time an item needs to have its content updated.
    ///     The 'wrapIndex' is the index within the child list, and 'realIndex' is the index using position logic.
    /// </summary>
    public OnUpdateItem onUpdateItem;

    public UIScrollView ScrollView
    {
        get
        {
            if (mScrollView == null)
                CacheScrollView();
            return mScrollView;
        }
    }

    public UIPanel Panel
    {
        get
        {
            if (mPanel == null)
                CacheScrollView();
            return mPanel;
        }
    }

    /// <summary>
    ///     数据列表数量发生变更时调用此方法
    /// </summary>
    /// <param name="dataCount"></param>
    /// <param name="reset"></param>
    public void UpdateDataCount(int dataCount, bool reset)
    {
        _dataCount = dataCount;
        if (reset)
        {
            if (mChildren.Count > 0)
            {
                for (int i = 0; i < mChildren.Count; i++)
                {
                    mChildren[i].localPosition = Vector3.zero;
                }
            }
            SortBasedOnScrollMovement();
            Panel.cachedTransform.localPosition = panelVec;
            Panel.clipOffset = Vector2.zero;
            _lastDataCount = -1;
        }
        else
        {
            SortBasedOnScrollMovement();
            WrapContent();
            _lastDataCount = dataCount;
        }
    }

    public void MoveTo(int index, bool useSpring = false)
    {
        var cr = Panel.clipOffset;
        var panelPos = Panel.cachedTransform.localPosition;
        if (mHorizontal)
        {
            cr.x = index * itemSize;
            panelPos.x = -index * itemSize;
        }
        else
        {
            cr.y = -index * itemSize;
            panelPos.y = index * itemSize;
        }

        MoveTo(cr, panelPos, useSpring);
    }

    public void MoveTo(Vector2 clipOffset, Vector3 panelPos, bool useSpring = false)
    {
        if (useSpring)
        {
            SpringPanel.Begin(Panel.cachedGameObject, panelPos, 6f);
        }
        else
        {
            Panel.cachedTransform.localPosition = panelPos;
            Panel.clipOffset = clipOffset;
            if (ScrollView != null) ScrollView.UpdateScrollbars(false);
            WrapContent();
        }
    }

    /// <summary>
    ///     Initialize everything and register a callback with the UIPanel to be notified when the clipping region moves.
    /// </summary>
    protected virtual void Start()
    {
        SortBasedOnScrollMovement();
        WrapContent();
        mFirstTime = false;
    }

    /// <summary>
    ///     Callback triggered by the UIPanel when its clipping region moves (for example when it's being scrolled).
    /// </summary>
    protected virtual void OnMove(UIPanel panel)
    {
        WrapContent();
    }

    /// <summary>
    ///     Immediately reposition all children.
    /// </summary>
    [ContextMenu("Sort Based on Scroll Movement")]
    public void SortBasedOnScrollMovement()
    {
        if (!CacheScrollView()) return;

        // Cache all children and place them in order
        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
            mChildren.Add(mTrans.GetChild(i));

        // Sort the list of children so that they are in order
        if (sorting == SortingEnum.Alphabetical) mChildren.Sort(UIGrid.SortByName);
        else if (sorting == SortingEnum.Horizontal) mChildren.Sort(UIGrid.SortHorizontal);
        else if (sorting == SortingEnum.Vertical) mChildren.Sort(UIGrid.SortVertical);
        else
        {
            //todo其他模式
        }
        ResetChildPositions();
    }

    /// <summary>
    ///     Immediately reposition all children, sorting them alphabetically.
    /// </summary>
    [ContextMenu("Sort Alphabetically")]
    public void SortAlphabetically()
    {
        if (!CacheScrollView()) return;

        // Cache all children and place them in order
        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
            mChildren.Add(mTrans.GetChild(i));

        // Sort the list of children so that they are in order
        mChildren.Sort(UIGrid.SortByName);
        ResetChildPositions();
    }

    /// <summary>
    ///     Cache the scroll view and return 'false' if the scroll view is not found.
    /// </summary>
    protected bool CacheScrollView()
    {
        if (mFirstTime)
        {
            mTrans = transform;
            mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
            mScrollView = mPanel.GetComponent<UIScrollView>();
            if (mPanel != null) mPanel.onClipMove = OnMove;

            panelVec = mPanel.cachedTransform.localPosition;
        }
        if (mScrollView == null) return false;
        if (mScrollView.movement == UIScrollView.Movement.Horizontal) mHorizontal = true;
        else if (mScrollView.movement == UIScrollView.Movement.Vertical) mHorizontal = false;
        else return false;
        return true;
    }

    /// <summary>
    ///     Helper function that resets the position of all the children.
    /// </summary>
    private void ResetChildPositions()
    {
        int x = 0;
        int y = 0;
        int maxX = 0;
        int maxY = 0;

        if (cullContent)
        {
            var firstChildPos = mChildren.Count > 0 ? mChildren[0].localPosition : Vector3.zero;
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                var t = mChildren[i];
                if(maxPerLine == 0)
                    t.localPosition = mHorizontal
                    ? new Vector3(firstChildPos.x + i * itemSize, 0f, 0f)
                    : new Vector3(0f, firstChildPos.y - i * itemSize, 0f);
                else
                    t.localPosition = (arrangement == Arrangement.Horizontal)
                        ? new Vector3(cellWidth * x, -cellHeight * y, 0f)
                        : new Vector3(cellWidth * y, -cellHeight * x, 0f);

                UpdateItem(t, i);

                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }

            //数据列表删减了元素，面板回滚移动距离
            if (_dataCount != -1 && _lastDataCount > _dataCount)
            {
                var cr = Panel.clipOffset;
                bool needSpring = false;
                if (mHorizontal && cr.x > 0f) needSpring = true;
                else if (!mHorizontal && cr.y < 0f) needSpring = true;

                if (needSpring)
                {
                    var offset = new Vector3(0, 0, 0);
                    if (maxPerLine == 0)
                        offset = new Vector3(-cr.x, -cr.y, 0f) +
                            (_lastDataCount - _dataCount) * itemSize * new Vector3(1f, -1f, 0f);
                    else
                    {
                        var lastRow = Mathf.CeilToInt(_lastDataCount / maxPerLine);
                        var nowRow = Mathf.CeilToInt(_dataCount / maxPerLine);
                        offset = new Vector3(-cr.x, -cr.y, 0f);
                        offset.x = -cr.x + cellWidth * (lastRow - nowRow);
                        offset.y = -cr.y + cellHeight * (lastRow - nowRow) * (-1f);
                    }
                    
                    if (!mScrollView.canMoveHorizontally) offset.x = Panel.cachedTransform.localPosition.x;
                    if (!mScrollView.canMoveVertically) offset.y = Panel.cachedTransform.localPosition.y;

                    //限制滚动回ScrollView起点
                    if (offset.x < 0f) offset.x = 0f;
                    else if (offset.y < 0f) offset.y = 0f;

                    SpringPanel.Begin(Panel.cachedGameObject, offset, 6f);
                }
            }
        }
        else
        {
            var firstChildPos = mChildren.Count > 0 ? mChildren[0].localPosition : Vector3.zero;
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                var t = mChildren[i];
                if(maxPerLine == 0)
                    t.localPosition = mHorizontal
                        ? new Vector3(firstChildPos.x + i * itemSize, 0f, 0f)
                        : new Vector3(0f, firstChildPos.y - i * itemSize, 0f);
                else
                {
                    var column = (arrangement == Arrangement.Horizontal) ? i % maxPerLine : Mathf.FloorToInt(i / maxPerLine);
                    var row = (arrangement == Arrangement.Horizontal) ? Mathf.FloorToInt(i / maxPerLine) : i % maxPerLine;
                    t.localPosition = new Vector3(column * cellWidth, row * cellHeight);
                }
                
                UpdateItem(t, i);
            }
        }
    }

    /// <summary>
    ///     Wrap all content, repositioning all children as needed.
    /// </summary>
    public void WrapContent()
    {
        if (_dataCount == -1) return;

        float extents = 0.0f;
        if(maxPerLine == 0)
            extents = itemSize*mChildren.Count*0.5f;
        else
        {
            var row = (arrangement == Arrangement.Horizontal) ? Mathf.CeilToInt(mChildren.Count / maxPerLine) : maxPerLine;
            var column = (arrangement == Arrangement.Horizontal) ? maxPerLine : Mathf.CeilToInt(mChildren.Count / maxPerLine);
            extents = (arrangement == Arrangement.Horizontal) ? row * cellHeight : column * cellWidth;
            extents /= 2;
        }
        
        var corners = Panel.worldCorners;

        for (int i = 0; i < 4; ++i)
        {
            var v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }

        var center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        bool allWithinRange = true;
        float ext2 = extents*2f;

        if (mHorizontal)
        {
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                var t = mChildren[i];
                float distance = t.localPosition.x - center.x;

                if (distance < -extents)
                {
                    var pos = t.localPosition;
                    pos.x += ext2;
                    int realIndex = 0;
                    if (maxPerLine == 0)
                        realIndex = Mathf.RoundToInt(pos.x/itemSize);
                    else
                        realIndex = (arrangement == Arrangement.Horizontal)
                        ? -Mathf.FloorToInt(pos.y / cellHeight) * maxPerLine + Mathf.FloorToInt(pos.x / cellWidth)
                        : Mathf.FloorToInt(pos.x / cellWidth) * maxPerLine - Mathf.FloorToInt(pos.y / cellHeight);

                    if (0 <= realIndex && realIndex <= _dataCount - 1)
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (distance > extents)
                {
                    var pos = t.localPosition;
                    pos.x -= ext2;
                    int realIndex = 0;
                    if(maxPerLine == 0)
                        realIndex = Mathf.RoundToInt(pos.x/itemSize);
                    else
                        realIndex = (arrangement == Arrangement.Horizontal)
                        ? -Mathf.FloorToInt(pos.y / cellHeight) * maxPerLine + Mathf.FloorToInt(pos.x / cellWidth)
                        : Mathf.FloorToInt(pos.x / cellWidth) * maxPerLine - Mathf.FloorToInt(pos.y / cellHeight);

                    if (0 <= realIndex && realIndex <= _dataCount - 1)
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);
            }
        }
        else
        {
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                var t = mChildren[i];
                float distance = t.localPosition.y - center.y;

                if (distance < -extents)
                {
                    var pos = t.localPosition;
                    pos.y += ext2;
                    int realIndex = 0;
                    if (maxPerLine == 0)
                        realIndex = -Mathf.RoundToInt(pos.y/itemSize);
                    else
                        realIndex = (arrangement == Arrangement.Horizontal)
                        ? -Mathf.FloorToInt(pos.y / cellHeight) * maxPerLine + Mathf.FloorToInt(pos.x / cellWidth)
                        : Mathf.FloorToInt(pos.x / cellWidth) * maxPerLine - Mathf.FloorToInt(pos.y / cellHeight);
                    //NGUIDebug.Log("======================<" + realIndex);
                    if (0 <= realIndex && realIndex <= _dataCount - 1)
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (distance > extents)
                {
                    var pos = t.localPosition;
                    pos.y -= ext2;
                    int realIndex = 0;
                    if (maxPerLine == 0)
                        realIndex = -Mathf.RoundToInt(pos.y/itemSize);
                    else
                        realIndex = (arrangement == Arrangement.Horizontal)
                        ? -Mathf.FloorToInt(pos.y / cellHeight) * maxPerLine + Mathf.FloorToInt(pos.x / cellWidth)
                        : Mathf.FloorToInt(pos.x / cellWidth) * maxPerLine - Mathf.FloorToInt(pos.y / cellHeight);

                    //NGUIDebug.Log("======================>" + realIndex);
                    if (0 <= realIndex && realIndex <= _dataCount - 1)
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                    else allWithinRange = false;
                }
                else if (mFirstTime) UpdateItem(t, i);
            }
        }
        mScrollView.restrictWithinPanel = !allWithinRange;
        mScrollView.InvalidateBounds();
    }

    /// <summary>
    ///     Want to update the content of items as they are scrolled? Override this function.
    /// </summary>
    protected virtual void UpdateItem(Transform item, int index)
    {
        var itemGo = item.gameObject;
        if (onUpdateItem != null && _dataCount != -1)
        {
            int realIndex = 0;
            if (maxPerLine == 0)
                realIndex = mHorizontal
                    ? Mathf.RoundToInt(item.localPosition.x / itemSize)
                    : -Mathf.RoundToInt(item.localPosition.y / itemSize);
            else
                realIndex = (arrangement == Arrangement.Horizontal)
                ? - Mathf.FloorToInt(item.localPosition.y / cellHeight) * maxPerLine + Mathf.FloorToInt(item.localPosition.x / cellWidth)
                : Mathf.FloorToInt(item.localPosition.x / cellWidth) * maxPerLine - Mathf.FloorToInt(item.localPosition.y / cellHeight);

            onUpdateItem(itemGo, index, realIndex);
            //只显示源数据列表范围内的
            if (cullContent && _dataCount != -1)
            {
                itemGo.SetActive(0 <= realIndex && realIndex < _dataCount);
            }
        }
        else
            itemGo.SetActive(false);
    }

    public void UpdateChildrenData()
    {
        for(int i = 0; i < mChildren.Count; i++)
        {
            UpdateItem(mChildren[i], i);
        }
    }
}