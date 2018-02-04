using System;
using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 有限循环移动。用于数据到尾不需要继续显示头数据的情况。
/// </summary>
/// <param name ="content">物体循环移动，数据不循环。</param>
/// <returns></returns>
public class LimitedCycleMove
{
    private enum moveDir
    {
        up,
        down,
        left,
        right,
        zero
    }

    private List<GameObject> mCells;
    private UIPanel mPanel;
    private UIScrollView mScrollView;
     private BoxCollider mBoxCollider;

    private Vector3 OffsetInitPos;

    private bool IsDrag;
    private bool IsMovetoDoing;

    private float CellWidth;
    private float CellHeight;
    private int ColLimit;
    private int RowCount;

    private float mPanelWidth;
    private float mPanelHeight;

    private int mDataCount;
    private int mCreateCount;

    private int mHandRowCellIndex;
    private int mEndRowCellIndex;

    private float mMoveCount;
    private int ShowRow;
    private int ShowCol;
    private float MoveIndex;
    private float distance;

    private int mHandRowDataIndex;
    private int mEndRowDataIndex;

    private Vector3 centerPos;

    private UIScrollView.Movement MoveMent;
    private moveDir NowDragDir;

    private Vector3 MoveToOffset;
    private Vector2 MoveToPos;

    public delegate void UpdateData(GameObject go, int CellIndex, int DataIndex);

    private event UpdateData UpdateDataEvent;

    private int AddCol = 4;
    public int InitData(int DataCount, UIPanel panel, UIScrollView scrollView, float Cellwidth = 100f,
        float Cellheigh = 80f, int colLimit = 3)
    {
        mPanel = panel;
        
        mScrollView = scrollView;
        MoveMent = mScrollView.movement;


        IsMovetoDoing = false;
        mScrollView.onDragFinished += () =>
        {
            if (IsMovetoDoing)
            {
                mPanel.clipOffset = MoveToOffset;
                mPanel.transform.localPosition = MoveToPos;
                IsMovetoDoing = false;
            }
        };

        mDataCount = DataCount;

        CellWidth = Cellwidth;
        CellHeight = Cellheigh;
        ColLimit = colLimit;

        mPanelWidth = panel.GetViewSize().x;
        mPanelHeight = panel.GetViewSize().y;
        
        mCreateCount = mDataCount;
        if (MoveMent == UIScrollView.Movement.Vertical)
        {
            RowCount = (int) (((float) mDataCount/ColLimit) + 0.5f);

            ShowRow = (int) ((mPanelHeight/CellHeight) + 0.5f);

            int rowCount = RowCount;
            IsDrag = RowCount > 1 && RowCount > ShowRow;

            RowCount = ShowRow + AddCol;
            mCreateCount = RowCount*ColLimit;

            MoveIndex = ShowRow - 1;
            mEndRowCellIndex = mCreateCount - ColLimit;
        }
        else if (MoveMent == UIScrollView.Movement.Horizontal)
        {
            ColLimit = mDataCount;
            RowCount = 1;

            ShowCol = (int) ((mPanelWidth/CellWidth) + 0.5f);
            IsDrag = ColLimit > 1 && ColLimit > ShowRow;
            mCreateCount = ShowCol + AddCol;

            MoveIndex = ShowCol - 1;
            mEndRowCellIndex = mCreateCount - 1;
        }

       // mScrollView.disableDragIfFits = mCreateCount > DataCount;

        return mCreateCount;
    }

    public void InitCells(ref List<GameObject> cells, UpdateData callback, Vector3 offsetInit, BoxCollider box = null)
    {
        NowDragDir = moveDir.zero;
        centerPos = Vector3.zero;
        mMoveCount = 0;

        mHandRowCellIndex = 0;

        mHandRowDataIndex = mHandRowCellIndex;
        mEndRowDataIndex = mEndRowCellIndex;

        OffsetInitPos = offsetInit;

        mCells = cells;
        InitCellPosition();

        mBoxCollider = box;
        UpsetBoxCollider();

        SetUpdateDataCallBack(callback);
        AllCellUpdateFormTop();
    }

    private void UpsetBoxCollider()
    {
        if (mBoxCollider == null)
            return;

        float OffsetWidht = Mathf.Abs(OffsetInitPos.x);
        float OffsetHeight = Mathf.Abs(OffsetInitPos.y);
        int AllRow = (int)Mathf.Round( mDataCount/ColLimit+0.5f);
        mBoxCollider.size = new Vector3(CellWidth * ColLimit + OffsetWidht, CellHeight * AllRow + OffsetHeight);
        mBoxCollider.center = new Vector3(-(mPanelWidth - CellWidth * ColLimit ), (mPanelHeight - CellHeight * AllRow)) * 0.5f;
        mBoxCollider.center += new Vector3(OffsetWidht, -OffsetHeight);
    }

    public void UpdateDataCount(int DataCount, bool IsMoveToTop = false)
    {
        if (IsMoveToTop)
        {
            MoveTo(0);
        }

        mDataCount = DataCount;
        if (MoveMent == UIScrollView.Movement.Horizontal)
        {
            ColLimit = mDataCount;
        }
       // mScrollView.disableDragIfFits = mCreateCount > DataCount;

        UpdatePanelShowData();
        UpsetBoxCollider();
    }

    private void InitCellPosition()
    {
        mCells[0].transform.parent.localPosition = Vector3.zero;
        Vector3 Pos = new Vector3(-mPanelWidth, mPanelHeight, 0)*0.5f;
        Pos += new Vector3(CellWidth, -CellHeight, 0)*0.5f;
        Pos += OffsetInitPos;

        int Index = 0;
        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColLimit; col++)
            {
                if (Index == mCells.Count)
                    break;

                mCells[Index].transform.localPosition = Pos + new Vector3(CellWidth*col, -CellHeight*row);
                Index += 1;
            }
        }
    }

    public void UpdatePanelShowData()
    {
        for (int CellIndex = mHandRowCellIndex, dataIndex = mHandRowDataIndex, mCellsCount = mCells.Count,UpdateIndex = 0;
            UpdateIndex < mCellsCount;
            dataIndex++, UpdateIndex++)
        {
            CellUpdate(CellIndex, dataIndex);
            CellIndex += 1;
            CellIndex = CellIndex == mCellsCount ? 0 : CellIndex;
        }
    }

    public void UpdateDataIndex(int DataIndex)
    {
        int EndDataIndex = MoveMent == UIScrollView.Movement.Vertical ? mEndRowDataIndex + ColLimit-1 : mEndRowDataIndex;
        if (DataIndex >= mHandRowDataIndex && DataIndex <= EndDataIndex)
        {
            for (int CellIndex = mHandRowCellIndex,dataIndex = mHandRowDataIndex,mCellsCount = mCells.Count; dataIndex <= EndDataIndex; dataIndex++)
            {
                if (dataIndex == DataIndex)
                {
                    CellUpdate(CellIndex, dataIndex);
                    break;
                }
                CellIndex += 1;
                CellIndex = CellIndex == mCellsCount ? 0 : CellIndex;
            }
        }
    }

    public void SetUpdateDataCallBack(UpdateData callback)
    {
        UpdateDataEvent = callback;
        mPanel.onClipMove = MoveEvent;
    }

    private void AllCellUpdateFormTop()
    {
        if(mHandRowDataIndex!=0)
            MoveTo(0);

        for (int i = 0, DataIndex = 0; i < mCells.Count; i++, DataIndex++)
        {
            CellUpdate(i, DataIndex);
        }
    }

    private void MoveStart()
    {
        mCells[0].transform.parent.gameObject.SetActive(false);
    }

    private void MoveEnd()
    {
        mCells[0].transform.parent.gameObject.SetActive(true);
    }

    public void MoveTo(int Index, bool useSpring = false)
    {
        if (Index < mDataCount && Index >= 0)
        {
            MoveStart();
            IsMovetoDoing = true;

            mScrollView.DisableSpring();
           
            int DataIndex = 0;
            MoveToOffset = Vector3.zero;
            MoveToPos = Vector3.zero;

            Vector2 dragAmount = Vector2.zero;

            if (MoveMent == UIScrollView.Movement.Horizontal)
            {
                DataIndex = Index;
                MoveToOffset.x = CellWidth * DataIndex;

                int ShowEndIndex = mDataCount - ShowCol;
                if (DataIndex > ShowEndIndex && ShowEndIndex> 0)
                    MoveToOffset.x -= mPanelWidth - CellWidth;

                MoveToPos.x = -MoveToOffset.x;
                dragAmount.x = (float)MoveToPos.x / (CellWidth * DataIndex - (mPanelWidth - CellWidth));
            }
            else if (MoveMent == UIScrollView.Movement.Vertical)
            {
                DataIndex = (int)(Index / ColLimit);
                MoveToOffset.y = -CellHeight * DataIndex;

                int ShowEndIndex = (int)((mDataCount / ColLimit) + 0.5f) - ShowRow;
                if (DataIndex > ShowEndIndex && ShowEndIndex>0)
                    MoveToOffset.y += mPanelHeight - CellHeight;

                MoveToPos.y = -MoveToOffset.y;
                dragAmount.y = (float)MoveToPos.y / (-CellHeight * DataIndex + (mPanelHeight - CellHeight));

            }

            if (mPanel.clipOffset.magnitude == MoveToOffset.magnitude)
            {
                IsMovetoDoing = false;
                MoveEnd();
                return;
            }

            if (useSpring)
            {
                //mScrollView.SetDragAmount(dragAmount.x, dragAmount.y, false);
                // mScrollView.
                SpringPanel.Begin(mPanel.cachedGameObject, MoveToPos, 10);
                IsMovetoDoing = false;
            }
            else
            {
                mPanel.clipOffset = MoveToOffset;
                mPanel.transform.localPosition = MoveToPos;

                if (mScrollView.shouldMoveVertically || mScrollView.shouldMoveHorizontally)
                    mScrollView.Press(false);
                else
                    IsMovetoDoing = false;
            }
            MoveEnd();
        }
    }
    
    private void CellUpdate(int Index, int DataIndex)
    {
        if (DataIndex < mDataCount)
        {
            mCells[Index].SetActive(true);
            if(UpdateDataEvent!=null)
                UpdateDataEvent(mCells[Index], Index, DataIndex);
        }
        else
            mCells[Index].SetActive(false);
    }

    private void MoveEvent(UIPanel panel)
    {
        if (!IsDrag)
            return;

        Vector3 PanelClipOffset = mPanel.clipOffset;
        if (MoveMent == UIScrollView.Movement.Horizontal)
        {
            distance = PanelClipOffset.x - centerPos.x;
            if (distance >= CellWidth)
            {
                do
                {
                    MoveIndex += 1;
                    mMoveCount += 1;
                    centerPos.x = mMoveCount*CellWidth;
                    if (MoveIndex == mCreateCount - 1 && mEndRowDataIndex < mDataCount)
                    {
                        MoveIndex -= 1;
                        Vector3 initPos = mCells[mEndRowCellIndex].transform.localPosition;

                        UpdateHandandEndDataIndex(moveDir.left);

                        mCells[mHandRowCellIndex].transform.localPosition = initPos + new Vector3(CellWidth, 0, 0);
                        CellUpdate(mHandRowCellIndex, mEndRowDataIndex);

                        UpdateHandandEndCellIndex(moveDir.left);
                    }
                    distance = mPanel.clipOffset.x - centerPos.x;
                } while ((distance >= CellWidth));

            }
            else if (-distance >= CellWidth )
            {
                do
                {
                    MoveIndex -= 1;
                    mMoveCount -= 1;
                    centerPos.x = mMoveCount*CellWidth;
                    if (MoveIndex == ShowCol - 1 && mHandRowDataIndex > 0)
                    {
                        MoveIndex += 1;
                        Vector3 initPos = mCells[mHandRowCellIndex].transform.localPosition;

                        UpdateHandandEndDataIndex(moveDir.right);

                        mCells[mEndRowCellIndex].transform.localPosition = initPos + new Vector3(-CellWidth, 0, 0);
                        CellUpdate(mEndRowCellIndex, mHandRowDataIndex);

                        UpdateHandandEndCellIndex(moveDir.right);
                    }

                    distance = mPanel.clipOffset.x - centerPos.x;
                } while (-distance >= CellWidth);
            }
        }
        else if (MoveMent == UIScrollView.Movement.Vertical)
        {
            distance = PanelClipOffset.y - centerPos.y;
            if (-distance >= CellHeight)
            {
                do
                {
                    MoveIndex += 1;
                    mMoveCount -= 1;
                    centerPos.y = mMoveCount*CellHeight;
                    if (MoveIndex == RowCount - 1 && mEndRowDataIndex < mDataCount)
                    {
                        MoveIndex -= 1;

                        Vector3 initPos = mCells[mEndRowCellIndex].transform.localPosition;
                        initPos += new Vector3(0, -CellHeight, 0);

                        UpdateHandandEndDataIndex(moveDir.down);
                        for (int i = 0; i < ColLimit; i++)
                        {
                            mCells[mHandRowCellIndex + i].transform.localPosition = initPos + new Vector3(CellWidth*i, 0, 0);
                            CellUpdate(mHandRowCellIndex + i, mEndRowDataIndex + i);
                        }
                        UpdateHandandEndCellIndex(moveDir.down);
                    }
                    distance = PanelClipOffset.y - centerPos.y;
                } while (-distance >= CellHeight);
            }
            else if (distance >= CellHeight)
            {
                do
                {
                    MoveIndex -= 1;
                    mMoveCount += 1;
                    centerPos.y = mMoveCount*CellHeight;
                    if (MoveIndex == ShowRow - 1 && mHandRowDataIndex > 0)
                    {
                        MoveIndex += 1;

                        Vector3 initPos = mCells[mHandRowCellIndex].transform.localPosition;
                        initPos += new Vector3(0, CellHeight, 0);
                        UpdateHandandEndDataIndex(moveDir.up);
                        for (int i = 0; i < ColLimit; i++)
                        {
                            mCells[mEndRowCellIndex + i].transform.localPosition = initPos + new Vector3(CellWidth*i, 0, 0);
                            CellUpdate(mEndRowCellIndex + i, mHandRowDataIndex  + i);
                        }
                        UpdateHandandEndCellIndex(moveDir.up);
                    }
                    distance = PanelClipOffset.y - centerPos.y;

                } while (distance >= CellHeight);
            }
        }
        
    }

    private void UpdateHandandEndCellIndex(moveDir dir)
    {
        if (dir == moveDir.down)
        {
            int hand = mHandRowCellIndex;
            mEndRowCellIndex = mHandRowCellIndex;

            hand = hand + ColLimit;
            hand = hand >= mCreateCount ? 0 : hand;
            mHandRowCellIndex = hand;

        }
        else if (dir == moveDir.up)
        {
            mHandRowCellIndex = mEndRowCellIndex;

            int end = mEndRowCellIndex;
            end = end - ColLimit;
            end = end < 0 ? mCreateCount - ColLimit : end;
            mEndRowCellIndex = end;
        }
        else if (dir == moveDir.left)
        {
            mEndRowCellIndex = mHandRowCellIndex;

            int hand = mHandRowCellIndex+1;
            hand = hand < mCreateCount ? hand : 0;
            mHandRowCellIndex = hand;
        }
        else if (dir == moveDir.right)
        {
            mHandRowCellIndex = mEndRowCellIndex;

            int end = mEndRowCellIndex -1;
            end = end >= 0 ? end : mCreateCount - 1;
            mEndRowCellIndex = end;
        }
    }


    private void UpdateHandandEndDataIndex(moveDir dir)
    {
        if (dir == moveDir.down)
        {
            mHandRowDataIndex += ColLimit;
            mEndRowDataIndex += ColLimit;
        }
        else if (dir == moveDir.up)
        {
            mHandRowDataIndex -= ColLimit;
            mEndRowDataIndex -= ColLimit;
        }else if (dir == moveDir.left)
        {
            mHandRowDataIndex += 1;
            mEndRowDataIndex += 1;
        }
        else if (dir == moveDir.right)
        {
            mHandRowDataIndex -= 1;
            mEndRowDataIndex -= 1;
        }
    }

    public void Dispose()
    {
        MoveTo(0);
        IsMovetoDoing = false;
    }

}