// ***************************************************************************
// 创建: 2016/10/2
// 作用：通过修改Cell 的坐标同时刷新显示， 来减少需要实例化的Cell总数
//  修改： 2016.12.14
//     1: 组件化， 作用规避用户错误使用的情况
//     2: 提供Cell偏移值， 支持Cell的任意中心点， 原因： Cell预设制作的情况多种多样， 中心点并不固定
//     3: 每行或每列的Cell数由设置提供并修改相关地方， 不再根据UIScrollView的可视范围和Cell的大小来决定
// ***************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//
public class GridWrapContent : MonoBehaviour
{
    //cell 相关数据类
    public class CellsConfig
    {
        public int mCount = 0;   //总数据数

        public string mCellName;   //cell 预设的名字
        public int mCellXOffset = 0;        //Cell的X轴偏移， 用于Cell预设制作时存在的各种情况, 如背景图的锚点设置问题， 背景图作为子物体有坐标设置
        public int mCellYOffset = 0;        //Cell的Y轴偏移， 作用： 同上

        public Func<string, GameObject, GameObject> mCellCreateFun;   //创建Cell实例的函数， string：cell的名字， GameObject：父节点， GameObject：创建的Cell实例
        public Action<string, GameObject> mCellDestroyAction;            //删除Cell实例的函数， string：cell的名字， GameObject：Cell实例
        public Action<GameObject, int> mDisplayFun;    //显示函数， GameObject为cell， int 为 数据的索引

        //可选设置参数
        public int mPerInstiateCount = 0;     //每次实例化的数目，  <= 0 为一次性实例化全部
        public int mDeltaFrame = 0;     //间隔的帧数

        //创建Cell实例
        public GameObject CreateCellInstance(GameObject pParent)
        {
			 return mCellCreateFun(mCellName, pParent);  
//            return AssetPipeline.ResourcePoolManager.Instance.SpawnUIGo(mCellName, pParent);
        }

        public void ClearDelegate()
        {
            mCellCreateFun = null;
            mCellDestroyAction = null;
            mDisplayFun = null;
        }
    }


    //必要组件
    public UIScrollView mScrollView;
    public UIPanel mPanel;
    public UIGrid mGrid;

    //可选组件
    public UICenterOnChild mCenterOnChild;
    
    public CellsConfig mCellsConfig;        //要显示的数据想关数据
    public GridArrange.ArrangeType mArrangeType = GridArrange.ArrangeType.Vertical; //排序类型
    public GridArrange mGridArrange;        //格子间如何排序

    public Vector3[] mWorldCorner = new Vector3[4];         //显示范围四个点的位置

    public Vector3 mVisuableCenter;        //显示范围的中心点，世界坐标系
    public Vector2 mVisuableSize;          //UIScrollView 中设置的可见范围的大小


    ////key: Cell Prefab 的名字, Value : Cell 的父物体
    private readonly Dictionary<string, GameObject> mCellParentDic = new Dictionary<string, GameObject>();
    private GameObject mCellParent;

    //key: Cell Prefab 的名字, Value : Cell 的实例， 对复用的支持， 有时可能用户只有Cell Prefab 不一样
    private readonly Dictionary<string, List<GameObject>> mCellInstanceDic = new Dictionary<string, List<GameObject>>(); 
    private List<GameObject> mGoList = new List<GameObject>();       //生成的Cell实例

    private UIWidget mRegionWidget;       //占位i符，根据数据总数和UIGrid 计算得出的显示所需的全部范围
    private Vector3 mPaneOriginLocalPos;       //调用Display函数时， 记录UIPanel此时的位置（本地坐标系）
    private Coroutine mCoroutine;

    public int mCellWidth = 0;            //Cell的宽度
    public int mCellHeight = 0;           //Cell的高度

    public int mColumnLimit = 0;          //每行或每列的限制个数

    public int mDragStepSize = 0;            //拖动时偏移如果不是该值设置的整数倍，将进行补足
    public event Action<int> mDrageAdjustAction;        //拖动限制通知， int 代表 目前的偏移数， 从0，1,2,3....开始
    private Vector3 mDragStartLocalPos = Vector3.zero;       //记录拖动开始时UIScrollview的位置

    void Update()
    {
        if(mGridArrange != null)
            mGridArrange.DrawHleperInfo();
    }
    //初始化UI控件
    public void InitUIControl()
    {
        mScrollView = NGUITools.FindInParents<UIScrollView>(this.gameObject);
        mPanel = mScrollView.GetComponent<UIPanel>();

        mCenterOnChild = mScrollView.GetComponentInChildren<UICenterOnChild>();

        //生成的组件
        mRegionWidget = NGUITools.AddChild<UIWidget>(mScrollView.gameObject);
        RegisEvent();

    }

    //重置UI控件
    public void ResetUIControll()
    {
        mPanel.onClipMove -= UpdateContent;

        mScrollView = null;
        mPanel = null;

        mCenterOnChild = null;
        DestroyImmediate(mRegionWidget.gameObject);
    }

    public void Display(CellsConfig pCellsConfig, GridArrange.ArrangeType pArrangeType)
    {
        if(mScrollView == null)
            InitUIControl();

        if (mCellsConfig != null && pCellsConfig.mCellName != mCellsConfig.mCellName)
            mCellParent.SetActive(false);
  
        mCellsConfig = pCellsConfig;
        mArrangeType = pArrangeType;

        mCellParent = GetOrCreateCellParent(pCellsConfig.mCellName);
        mCellParent.SetActive(true);
        mGoList = GetOrCreateCellList(pCellsConfig.mCellName);

        InitData();

        if (mCoroutine != null)
            StopCoroutine(mCoroutine);

        if (mCellsConfig.mPerInstiateCount > 0)   //分批次实例化 Cell
           mCoroutine = StartCoroutine(CreateAllCellCourtine());
        else             //一次性实例化所有Cell实例
            CreateAllCell();            
    }

    private void InitData()
    {
        CorrectPaneValue();

        mPaneOriginLocalPos = mPanel.transform.localPosition;

        mPanel.worldCorners.CopyTo(mWorldCorner, 0);
        mVisuableCenter = (mWorldCorner[0] + mWorldCorner[2])/2f;
        mVisuableSize = new Vector2(mPanel.baseClipRegion.z, mPanel.baseClipRegion.w);

        transform.position = mWorldCorner[1];
        transform.localPosition += new Vector3(mCellsConfig.mCellXOffset, mCellsConfig.mCellYOffset);

        mGridArrange = GridArrange.Create(mArrangeType, this);
        mGridArrange.InitData();
        mGridArrange.SetOccupiedWidget(mRegionWidget);
    }

    //当可视范围的x或y方向小于Cell对应值时， 进行纠正
    void CorrectPaneValue()
    {
        Vector4 tClipRegion = mPanel.baseClipRegion;
        if (mPanel.baseClipRegion.z <mCellWidth)
        {
            tClipRegion.z =mCellWidth;
        }

        if (mPanel.baseClipRegion.w < mCellHeight)
        {
            tClipRegion.w = mCellHeight;
        }

        mPanel.baseClipRegion = tClipRegion;
    }

    private void RegisEvent()
    {
        mPanel.onClipMove += UpdateContent;
        mScrollView.onDragStarted += ()=> { mDragStartLocalPos = mScrollView.transform.localPosition; };
        mScrollView.onDragFinished += OnDrageFinishCallback;
    }

    private GameObject GetOrCreateCellParent(string pCell)
    {
        GameObject tParentGo;
        if (mCellParentDic.TryGetValue(pCell, out tParentGo) == true)
            return tParentGo;

        tParentGo = NGUITools.AddChild(this.gameObject);
        tParentGo.name = string.Format("{0}_Collect", pCell);
        tParentGo.transform.position = this.transform.position;

        mCellParentDic.Add(pCell, tParentGo);

        return tParentGo;
    }

    private List<GameObject> GetOrCreateCellList(string pCell)
    {
        List<GameObject> tCellList;
        if (mCellInstanceDic.TryGetValue(pCell, out tCellList) == true)
        {
            tCellList.RemoveAll((value) =>
            {
                if (value == null)
                    return true;

                return false;
            });

            return tCellList;
        }

        tCellList = new List<GameObject>();
        mCellInstanceDic.Add(pCell, tCellList);

        return tCellList;
    }

    //创建所有实例
    private void CreateAllCell()
    {
        int i;
        for (i = 0; i < mCellsConfig.mCount && i < mGridArrange.MaxCellInstanceCount; ++i)
        {
            CreateCellInstance(i);
        }

        //对多余的隐藏起来
        for (; i < mGoList.Count; ++i)
        {
            mGoList[i].SetActive(false);
        }
    }

    //以协程的方式创建所有Cell实例
    private IEnumerator CreateAllCellCourtine()
    {
        //先隐藏全部
        int i = 0;
        for (i = 0; i < mGoList.Count; ++i)
        {
            mGoList[i].SetActive(false);
        }

        int tMin = Mathf.Min(mCellsConfig.mCount, mGridArrange.MaxCellInstanceCount);
        i = 0;
        while (i < tMin)
        {
            int j = 0;
            for (j = 0; j < mCellsConfig.mPerInstiateCount; ++j)
            {
                if (i < tMin)
                {
                    CreateCellInstance(i++);
                }
                else
                {
                    break;
                }
            }

            if (i < tMin)
            {
                //yield return 5； Unity也不会停5帧，而是在下一帧执行， 所以用下面的代码实现
                for (j = 0; j < mCellsConfig.mDeltaFrame; ++j)
                    yield return mCellsConfig.mDeltaFrame;
            }
        }
    }

    //创建单个Cell实例
    private void CreateCellInstance(int pIndex)
    {
        GameObject tCell = null;

        if (pIndex < mGoList.Count)
        {
            tCell = mGoList[pIndex];
            tCell.transform.parent = mCellParent.transform;
            tCell.SetActive(true);
        }
        else
        {
            tCell = mCellsConfig.CreateCellInstance(mCellParent);
            mGoList.Add(tCell);
        }

        Vector3 tLocalPos = mGridArrange.GetLoaclPosByIndex(pIndex);
        tCell.transform.localPosition = tLocalPos;

        mCellsConfig.mDisplayFun(tCell, pIndex);
    }

    // 位置变换时平移就好， 数据没有的就隐藏
    private void  UpdateContent(UIPanel pPanel)
    {
        for (int i = 0; i < mGoList.Count; ++i)
        {
            GameObject tCellGo = mGoList[i];

            if (mGridArrange.IsOutOfLimitRegion(tCellGo.transform.position) == false)
            {
                continue;
            }

            //cell 已偏离， 获取需偏离的距离, 以this.tranform为坐标系
            Vector2 tOffset = mGridArrange.GetOffsetDistance(tCellGo.transform.position);
            Vector3 tLocalPos = tCellGo.transform.localPosition + new Vector3(tOffset.x, tOffset.y, 0f);
            int tDataIndex = 0;

            bool tHasData = mGridArrange.GetDataIndexByLocalPos(tLocalPos, ref tDataIndex);

            //有索引的情况下，还得索引不超过设置的数据量的索引
            if (tHasData == true && tDataIndex < mCellsConfig.mCount)
            {
                tCellGo.transform.localPosition = tLocalPos;                
                mCellsConfig.mDisplayFun(tCellGo, tDataIndex);
            }
        }
    }

    //响应拖拽结束事件
    private void OnDrageFinishCallback()
    {
        if (mDragStepSize <= 0)
            return;

        Vector3 tOffest = mScrollView.transform.localPosition - mDragStartLocalPos;

        float tTarValue = mScrollView.movement == UIScrollView.Movement.Horizontal ? tOffest.x : tOffest.y;
        int tSign = tTarValue < 0 ? -1 : 1;
        float tAdd =  (mDragStepSize - Mathf.Abs((tTarValue)%mDragStepSize)) * tSign;

        float tTotalOffset = 0;
        Vector3 tTarLocalPos = mScrollView.transform.localPosition;
        if (mScrollView.movement == UIScrollView.Movement.Horizontal)
        {
            tTarLocalPos.x += tAdd;
            tTotalOffset = tTarLocalPos.x - mPaneOriginLocalPos.x;
        }
        else
        {
            tTarLocalPos.y += tAdd;
            tTotalOffset = tTarLocalPos.y - mPaneOriginLocalPos.y;
        }

        int tStepCount = Mathf.FloorToInt(tTotalOffset * tSign / mDragStepSize);
    //    Debug.LogError("grid wrap 步数 = " + tStepCount);

        FixCellByLocalPos(tTarLocalPos);
    }
    
    //刷新所有Cell 的内容
    public void RefreshAllCellsContent(Action<GameObject, int> pRefreshFunc = null)
    {       
        for (int i = 0; i < mGoList.Count; ++i)
        {
            GameObject tGo = mGoList[i];
            RefreshCellContentByGo(tGo, pRefreshFunc);
        }
    }

    //刷新单个Cell

    public bool RefreshCellContentByGo(GameObject pCell, Action<GameObject, int> pRefreshFunc = null)
    {
        if (pCell == null || mCellsConfig == null)
            return false;

        //若没有指定刷新函数， 则用指定的显示函数
        if (pRefreshFunc == null)
        {
            pRefreshFunc = mCellsConfig.mDisplayFun;
        }

        int tDataIndex = 0;
        if (mGridArrange.GetDataIndexByLocalPos(pCell.transform.localPosition, ref tDataIndex) == false)
        {
#if UNITY_EDITOR
            Debug.LogError("获取数据索引失败， 请检查");
#endif
            return false;
        }

        pRefreshFunc(pCell, tDataIndex);

        return true;
    }

    //刷新单个Cell
    public bool RefreshCellContentByIndex(int pIndex, Action<GameObject, int> pRefreshFunc = null)
    {
        if (mCellsConfig == null)
            return false;

        //若没有指定刷新函数， 则用指定的显示函数
        if (pRefreshFunc == null)
        {
            pRefreshFunc = mCellsConfig.mDisplayFun;
        }

        GameObject tCell = GetCellByIndex(pIndex);
        if (tCell == null)
        {
            return false;
        }

        pRefreshFunc(tCell, pIndex);

        return true;
    }

    //改变数量并刷新显示， 会保留原来的位置信息， 如果数据是减少的会改变位置使之适配
    public void ChangeCount(int mCount)
    {
        if (mCellsConfig == null)
            return;

        Vector3 tOffset = mScrollView.transform.localPosition - mPaneOriginLocalPos;
        int tPerInstiateCount = mCellsConfig.mPerInstiateCount;
        mCellsConfig.mPerInstiateCount = 0;

        mCellsConfig.mCount = mCount;
        Display(mCellsConfig, mArrangeType);

        mCellsConfig.mPerInstiateCount = tPerInstiateCount;

        mScrollView.transform.localPosition += tOffset;
        mPanel.clipOffset -= new Vector2(tOffset.x, tOffset.y);

        mScrollView.Press(false);
    }

    public enum PosTag
    {
        TopLeft,       //左上
        Center,        //中间
        BootomeRight,   //右下
    }

    //通过cell的索引来定位
    public void FixCellByIndex(int pIndex, PosTag pPosTag, Action<int, GameObject> pCallBack = null, float pStrength = 8f)
    {
        if (mCellsConfig == null)
            return;

        if (pIndex < 0 || pIndex >= mCellsConfig.mCount)
            return;

        //数据量少， Cell 已全部显示， 则无需定位
        if ((mScrollView.movement == UIScrollView.Movement.Horizontal && mRegionWidget.width < mPanel.baseClipRegion.z) ||
            (mScrollView.movement == UIScrollView.Movement.Vertical && mRegionWidget.height < mPanel.baseClipRegion.w))
        {
            if (pCallBack != null)
            {
                GameObject tCell = GetCellByIndex(pIndex);
                pCallBack(pIndex, tCell);
            }

            return;
        }

        //计算目标位置的本地坐标
        Vector3 tTargetPos = mGridArrange.GetLoaclPosByIndex(pIndex); //这个值对应 PosTag.TopLeft
        switch (pPosTag)
        {
            case PosTag.Center:
                tTargetPos += new Vector3((-mVisuableSize.x + mCellWidth)/2, (mVisuableSize.y - mCellHeight)/2, 0f);
                break;

            case PosTag.BootomeRight:
                tTargetPos += new Vector3(-mVisuableSize.x + mCellWidth, mVisuableSize.y - mCellHeight, 0f);
                break;
        }

        Vector3 tFinalLocalPos = mPaneOriginLocalPos;
        switch (mScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                tFinalLocalPos.x -= tTargetPos.x;
                break;

            case UIScrollView.Movement.Vertical:
                tFinalLocalPos.y -= tTargetPos.y;
                break;
        }

        FixCellByLocalPos(tFinalLocalPos, () =>
        {
            if (pCallBack != null)
            {
                GameObject tCell = GetCellByIndex(pIndex);
                pCallBack(pIndex, tCell);
            }
        }, pStrength);
    }

    //通过坐标进行定位
    public void FixCellByLocalPos(Vector3 pTargetPos, Action pCallBack = null, float pStrength = 8f)
    {
        //限制位置在可滑动的区域内
        switch (mScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                float tMinX = mPaneOriginLocalPos.x - mRegionWidget.width + mVisuableSize.x;
                if (pTargetPos.x < tMinX)
                    pTargetPos.x = tMinX;

                if (pTargetPos.x > mPaneOriginLocalPos.x)
                    pTargetPos.x = mPaneOriginLocalPos.x;
                break;

            case UIScrollView.Movement.Vertical:

                float tMaxY = mPaneOriginLocalPos.y + mRegionWidget.height - mVisuableSize.y;
                if (pTargetPos.y < mPaneOriginLocalPos.y)
                    pTargetPos.y = mPaneOriginLocalPos.y;

                if (pTargetPos.y > tMaxY)
                    pTargetPos.y = tMaxY;
                break;
        }


        if (mDragStepSize > 0 && mDrageAdjustAction != null)
        {
            Vector3 tOffsetV3 = pTargetPos - mPaneOriginLocalPos;
            float tTotalOffset = mScrollView.movement == UIScrollView.Movement.Horizontal ? tOffsetV3.x : tOffsetV3.y;
            int tStep = Mathf.CeilToInt(tTotalOffset/mDragStepSize);
            mDrageAdjustAction(tStep);

         //   Debug.LogError("步数 = " + tStep);
        }

        SpringPanel.Begin(mScrollView.panel.cachedGameObject, pTargetPos, pStrength)
            .onFinished = () =>
            {
                if (pCallBack != null)
                    pCallBack();
            };
    }

    //通过数据索引来获取, 若返回NUll 则代表 pIndex 对应的Cell 还没有显示
    public GameObject GetCellByIndex(int pIndex)
    {
        int tIndex = -1;
        for (int i = 0; i < mGoList.Count; ++i)
        {
            GameObject tCell = mGoList[i];
            if (mGridArrange.GetDataIndexByLocalPos(tCell.transform.localPosition, ref tIndex))
            {
                if (tIndex == pIndex)
                    return tCell;
            }
        }

        return null;
    }

    #region 项目专属方法
    //缓存所有cell
    public void DespawnAllCell()
    {
        if (mCellsConfig == null)
            return;

        foreach (var tKV in mCellInstanceDic)
        {
            if (tKV.Key == null || tKV.Value == null)
                continue;

            List<GameObject> tGoList = tKV.Value;

            for (int i = 0; i < tGoList.Count; ++i)
            {
                GameObject tGo = tGoList[i];
                if (tGo != null && tGo.transform != null)
                    tGo.transform.parent = null;

                mCellsConfig.mCellDestroyAction(tKV.Key, tGo);
            }
        }

        mCellInstanceDic.Clear();

        //清除缓存
        mCellsConfig.ClearDelegate();
        mDrageAdjustAction = null;
    }

    #endregion

#if UNITY_EDITOR && !USE_JSZ

    public int mCellXOffset = 0;
    public int mCellYOffset = 0;

    [ContextMenu("Cell排序")]
    public void RepositionCellInEditor()
    {
        try
        {
            InitUIControl();

            mCellsConfig = new CellsConfig();
            mCellsConfig.mCellXOffset = mCellXOffset;
            mCellsConfig.mCellYOffset = mCellYOffset;
            InitData();

            for (int i = 0; i < this.transform.childCount; ++i)
            {
                Transform tCell = this.transform.GetChild(i);
                tCell.localPosition = mGridArrange.GetLoaclPosByIndex(i);
            }
        }
        finally
        {
            ResetUIControll();
            mCellsConfig = null;
        }
    }

#endif

}
