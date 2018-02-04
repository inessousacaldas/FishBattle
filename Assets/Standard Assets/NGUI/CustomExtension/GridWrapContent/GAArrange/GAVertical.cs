using UnityEngine;
using System.Collections;

//该类用于 UIScrollview 是上下移动的情况
//由左上角开始由左到右排列Cell, 注意index 从 0 开始
//      0   1   2
//      3   4   5
//      6   7   8
//      9   10  11
public class GAVertical : GridArrange
{

    public int mLine;   //显示用行数
    public int mDataLine;   //数据占的行数

    //cell 的位置范围限制， 超出该范围的Cell 需要调整位置
    private float mYMinWPos;      
    private float mYMaxWPos;

    protected int mYMoveUnit = 0;     //移动单位， 以像素为单位


    public GAVertical(GridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
        
    }

    public override void InitData()
    {
        base.InitData();

        Vector3[] tWorldCorner = mGridWrapContent.mPanel.worldCorners;

        //针对UI设置时数值设置可能不太精确的情况进行处理， 规范化必要数值
        float tDeficiency = mGridWrapContent.mVisuableSize.y % mGridWrapContent.mCellHeight;
        float tVisuableY = mGridWrapContent.mVisuableSize.y;
        if (tDeficiency <= mGridWrapContent.mCellHeight/10f)
        {
           tVisuableY = mGridWrapContent.mVisuableSize.y - tDeficiency;
        }

        int tVisuableLine = Mathf.CeilToInt(tVisuableY/mGridWrapContent.mCellHeight);   //可见区域所包含的行数
        mLine = tVisuableLine + 2;     // 多加的2行， 这样移动的时候才不穿帮
        mDataLine = Mathf.CeilToInt((float)mGridWrapContent.mCellsConfig.mCount / mGridWrapContent.mColumnLimit);

        //位置临界值计算， 说明：用户在可视化调整后，Cell的位置将被调整好， 第一个Cell他的本地坐标都是（0,0,0）, 临界值的计算就是以此为基准， 叠加偏移数值
        Vector3 tParentWPos = mGridWrapContent.transform.position;

        float tCellHeightInWorld = mGridWrapContent.transform.TransformVector(0, mGridWrapContent.mCellHeight, 0f).y;
        mYMinWPos = tParentWPos.y - tCellHeightInWorld * tVisuableLine;
        mYMaxWPos = tParentWPos.y + tCellHeightInWorld;

        //初始化父类中的变量Y
        mMaxCellInstanceCount = mLine * mGridWrapContent.mColumnLimit;
        mYMoveUnit = (int)(mLine * mGridWrapContent.mCellHeight);
    }

    public override void SetOccupiedWidget(UIWidget pWidget)
    {
        base.SetOccupiedWidget(pWidget);

        pWidget.height = (int)(mDataLine * mGridWrapContent.mCellHeight);
    }

    //根据索引获取以挂载在UIGrid的GameObject的坐标系的本地坐标, index 从 0 开始
    public override Vector3 GetLoaclPosByIndex(int i)
    {
        int tLine = i/mGridWrapContent.mColumnLimit;
        int tRow = i%mGridWrapContent.mColumnLimit;

        //说明, 在UI制作中， Grid对子物体的排序， 第一个都是以（0， 0， 0） 开始的, 所以行，列的索引都是左上角从 0 开始计算
        return new Vector3(tRow * mGridWrapContent.mCellWidth, -tLine * mGridWrapContent.mCellHeight, 0f);
    }

    //已挂载这个组件的transform为坐标系， 根据位置获取数据索引， 获取不到， 则该位置不正确
    public override bool GetDataIndexByLocalPos(Vector3 pLocalPos, ref int pDataIndex)
    {
        if (pLocalPos.y > 0 || pLocalPos.x < 0)
            return false;

        pLocalPos.y = Mathf.Abs(pLocalPos.y);
        pDataIndex = Mathf.FloorToInt(pLocalPos.x / mGridWrapContent.mCellWidth )
                   + Mathf.FloorToInt(pLocalPos.y / mGridWrapContent.mCellHeight * mGridWrapContent.mColumnLimit);

        return true;
    }

    //用于当Cell超出显示和缓冲的范围后， 需要对其偏移的距离， 坐标系为this.transform
    public override Vector3 GetOffsetDistance(Vector3 pWPos)
    {
        Vector3 tDiff = pWPos - mGridWrapContent.mVisuableCenter;

        if (tDiff.y > 0f) //偏上
        {
            return new Vector2(0f, -mYMoveUnit);
        }

        //偏下
        return new Vector2(0f, mYMoveUnit);

    }

    //是否超出了限制的范围
    public override bool IsOutOfLimitRegion(Vector3 pWPos)
    {
        return pWPos.y < mYMinWPos || pWPos.y > mYMaxWPos;
    }

    //绘制辅助信息
    public override void DrawHleperInfo()
    {
        base.DrawHleperInfo();
        //限制距离绘制辅助线
        Debug.DrawLine(new Vector3( mGridWrapContent.transform.position.x- 100, mYMinWPos, mGridWrapContent.transform.position.z),
                       new Vector3( mGridWrapContent.transform.position.x + 100, mYMinWPos, mGridWrapContent.transform.position.z));
        Debug.DrawLine(new Vector3(mGridWrapContent.transform.position.x - 100, mYMaxWPos, mGridWrapContent.transform.position.z),
                       new Vector3(mGridWrapContent.transform.position.x + 100, mYMaxWPos, mGridWrapContent.transform.position.z));
    }
}
