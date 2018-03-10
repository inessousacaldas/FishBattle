using UnityEngine;
using System;

/// <summary>
/// 挂机设置界面专用英雄半身像ITEM，可拖拽
/// @MarsZ 2017-04-01 11:09:45
/// </summary>
public class BattleAutoSummonHeroController:HeroCardItemController
{
    public PositionType Positon = PositionType.Undefined;
    public Action<BattleAutoSummonHeroController,GameObject,Vector3> OnDragDropReleaseHandler;


    protected override void InitUI()
    {
        base.InitUI();
        DragDropEnable = false;
    }

    public override void UpdateData(long pHeroUID)
    {
        base.UpdateData(pHeroUID);
        GameDebuger.LogError("[TEMP]设置品质，pHeroUID：" + pHeroUID.ToString());
        Quality = (int)pHeroUID % 4;
    }

    public int Quality{ get; private set; }


    #region 拖放

    private bool mDragDropEnable = false;

    public bool DragDropEnable
    {
        get
        { 
            return mDragDropEnable;
        }
        set
        {
            if (mDragDropEnable != value)
            {
                mDragDropEnable = value;
                UIDragDropItemCallbackable.enabled = mDragDropEnable;
            }
        }
    }

    private UIDragDropItemCallbackable mUIDragDropItemCallbackable;

    public UIDragDropItemCallbackable UIDragDropItemCallbackable
    {
        get
        {
            if (null == mUIDragDropItemCallbackable)
            {
                mUIDragDropItemCallbackable = View.gameObject.GetMissingComponent<UIDragDropItemCallbackable>();
                mUIDragDropItemCallbackable.restriction = UIDragDropItemCallbackable.Restriction.Vertical;
                mUIDragDropItemCallbackable.OnDragDropReleaseHandler = OnDragDropReleaseCallBack;
            }
            return mUIDragDropItemCallbackable;
        }
    }

    private void OnDragDropReleaseCallBack(GameObject pSurface,Vector3 pDragStartPosition)
    {
        if (null != OnDragDropReleaseHandler)
            OnDragDropReleaseHandler(this,pSurface,pDragStartPosition);
    }
    #endregion

    //位置类型
    public enum PositionType
    {
        Undefined = 0,
        CurrentUsed = 1,
        CurrentSelected = 2,
        ToBeSelected = 3,
    }
}