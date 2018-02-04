using UnityEngine;

public abstract class GridArrange
{
    public enum ArrangeType
    {
        Vertical = 1,          //上下移动时
        HorizontalRow = 2,    //左右列移动时
    }

    public static GridArrange Create(ArrangeType pType, GridWrapContent pGrid)
    {
        GridArrange tGridArrange = null;
        switch (pType)
        {
            case ArrangeType.Vertical:
                tGridArrange = new GAVertical(pGrid);
                break;

            case ArrangeType.HorizontalRow:
                tGridArrange = new GAHorizontalRow(pGrid);
                break;

            default:
#if UNITY_EDITOR
                Debug.LogError("匹配不到合适的排序类型， ArrangeType = " + pType);
#endif
                break;
        }

        return tGridArrange;
    }

    protected GridWrapContent mGridWrapContent;

    protected int mMaxCellInstanceCount;  //根据显示范围和缓冲范围计算出需要实例化的数

    public int MaxCellInstanceCount
    {
        get { return mMaxCellInstanceCount; }
    }

    public GridArrange(GridWrapContent pGridWrapContent)
    {
        mGridWrapContent = pGridWrapContent;
    }

    public virtual void InitData()
    {
    }

    //设置占位符的一些参数
    public virtual void SetOccupiedWidget(UIWidget pWidget)
    {
        pWidget.pivot = UIWidget.Pivot.TopLeft;
        pWidget.transform.position = mGridWrapContent.mWorldCorner[1];
    }

     //根据索引获取以挂载在UIGrid的GameObject的坐标系的本地坐标
     public virtual  Vector3 GetLoaclPosByIndex(int i)
    {
        return Vector3.one;
    }
	
        //已挂载这个组件的transform为坐标系， 根据位置获取数据索引， 获取不到， 则该位置不正确
     public virtual bool GetDataIndexByLocalPos(Vector3 pLocalPos, ref int pDataIndex)
    {
        return false;
    }

    //用于当Cell超出显示和缓冲的范围后， 需要对其偏移的距离， 坐标系为this.transform
     public virtual Vector3 GetOffsetDistance(Vector3 pWPos)
    {
        return Vector3.one;
    }

        //是否超出了限制的范围
     public virtual bool IsOutOfLimitRegion(Vector3 pWPos)
    {
        return false;
    }

    //绘制辅助线
    public virtual void DrawHleperInfo()
    {
        
    }
}
