using UnityEngine;
using System.Collections;

//由左上角开始由上到下排列Cell, 注意index 从 0 开始
//      0  4  8
//      1  5  9
//      2  6  10
//      3  7  11
public class GAHorizontalRow : GAHorizontal {

    public GAHorizontalRow(GridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
        
    }


    public override void SetOccupiedWidget(UIWidget pWidget)
    {
        base.SetOccupiedWidget(pWidget);

        pWidget.width = (int) (mDataRow*mGridWrapContent.mCellWidth);
    }

    //根据索引获取以挂载在UIGrid的GameObject的坐标系的本地坐标, index 从 0 开始
    public override Vector3 GetLoaclPosByIndex(int i)
    {
        int tLine = i %  mGridWrapContent.mColumnLimit;
        int tRow = i / mGridWrapContent.mColumnLimit;    

        //说明, 在UI制作中， Grid对子物体的排序， 第一个都是以（0， 0， 0） 开始的， 所以行，列的索引都是左上角从 0 开始计算
        return new Vector3(tRow * mGridWrapContent.mCellWidth, -tLine * mGridWrapContent.mCellHeight, 0f);
    }

    //已挂载这个组件的transform为坐标系， 根据位置获取数据索引， 获取不到， 则该位置不正确
    public override bool GetDataIndexByLocalPos(Vector3 pLocalPos, ref int pDataIndex)
    {
        if (pLocalPos.y > 0 || pLocalPos.x < 0)
            return false;

        pLocalPos.y = Mathf.Abs(pLocalPos.y);
        pDataIndex = Mathf.FloorToInt(pLocalPos.x / mGridWrapContent.mCellWidth * mGridWrapContent.mColumnLimit)  
                   + Mathf.FloorToInt(pLocalPos.y / mGridWrapContent.mCellHeight);

        return true;
    }

    //用于当Cell超出显示和缓冲的范围后， 需要对其偏移的距离， 坐标系为this.transform
    public override Vector3 GetOffsetDistance(Vector3 pWPos)
    {
        Vector3 tDiff = pWPos - mGridWrapContent.mVisuableCenter;

        if (tDiff.x > 0f) //偏右
        {
            return new Vector2( -mXMoveUnit,  0f);
        }

        //偏左
        return new Vector2(mXMoveUnit, 0f);

    }

    //是否超出了限制的范围
    public override bool IsOutOfLimitRegion(Vector3 pWPos)
    {
        return pWPos.x < mXMinWPos || pWPos.x > mXMaxWPos;
    }
}
