using UnityEngine;
using System.Collections;

//该类用于 UIScrollview 是上下移动的情况
public abstract class GAHorizontal : GridArrange
{

    public int mRow;   //显示用的列数
    public int mDataRow;    //数据占的列数

    //cell 的位置范围限制， 超出该范围的Cell 需要调整位置
    protected float mXMinWPos;
    protected float mXMaxWPos;

    protected int mXMoveUnit = 0;     //移动单位， 以像素为单位

    protected GAHorizontal(GridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
        
    }


    public override void InitData()
    {
        base.InitData();

        Vector3[] tWorldCorner = mGridWrapContent.mPanel.worldCorners;

        //针对UI设置时数值设置可能不太精确的情况进行处理， 规范化必要数值
        float tDeficiencX = mGridWrapContent.mVisuableSize.x%mGridWrapContent.mCellWidth;
        float tVisuableX = mGridWrapContent.mVisuableSize.x;
        if (tDeficiencX <= mGridWrapContent.mCellWidth / 10f)
        {
            tVisuableX = mGridWrapContent.mVisuableSize.x - tDeficiencX;
        }

        int tVisuableRow = Mathf.CeilToInt(tVisuableX / mGridWrapContent.mCellWidth);    //可见区域所包含的列数
        mRow = tVisuableRow + 2;     // 多加的2列， 这样移动的时候才不穿帮
        mDataRow = Mathf.CeilToInt((float)mGridWrapContent.mCellsConfig.mCount / mGridWrapContent.mColumnLimit);

        //位置临界值计算， 说明：用户在可视化调整后，Cell的位置将被调整好， 第一个Cell他的本地坐标都是（0,0,0）, 临界值的计算就是以此为基准， 叠加偏移数值
        Vector3 tParentWPos = mGridWrapContent.transform.position;

        float tCellWidthInWorld = mGridWrapContent.transform.TransformVector(mGridWrapContent.mCellWidth, 0f, 0f).x;
        mXMinWPos = tParentWPos.x - tCellWidthInWorld;
        mXMaxWPos = tParentWPos.x + tCellWidthInWorld * tVisuableRow;

        //初始化父类中的变量
        mMaxCellInstanceCount = mRow * mGridWrapContent.mColumnLimit;
        mXMoveUnit = (int)(mRow * mGridWrapContent.mCellWidth);
    }

    //绘制辅助信息
    public override void DrawHleperInfo()
    {
        base.DrawHleperInfo();
        //左边的线
        Debug.DrawLine(new Vector3(mXMinWPos, mGridWrapContent.transform.position.y - 100, mGridWrapContent.transform.position.z),
                       new Vector3(mXMinWPos, mGridWrapContent.transform.position.y + 100, mGridWrapContent.transform.position.z));
        //右边的线
        Debug.DrawLine(new Vector3(mXMaxWPos, mGridWrapContent.transform.position.y - 100, mGridWrapContent.transform.position.z),
                       new Vector3(mXMaxWPos, mGridWrapContent.transform.position.y + 100, mGridWrapContent.transform.position.z));
    }
}
