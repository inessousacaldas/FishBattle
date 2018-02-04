using UnityEngine;
using System;

/// <summary>
/// 带拖拽回调的拖放组件
/// @MarsZ 2017-04-05 19:50:03
/// </summary>
public class UIDragDropItemCallbackable : UIDragDropItem
{
    private Vector3 mDragStartPosition = Vector3.zero;
    public Action<GameObject,Vector3> OnDragDropReleaseHandler;
    public Action OnDragStartHandler;

    protected override void OnDragDropStart()
    {
        mDragStartPosition = transform.position;
        base.OnDragDropStart();
    }

    protected override void OnDragDropRelease(GameObject surface)
    {
        base.OnDragDropRelease(surface);
        if (null != OnDragDropReleaseHandler)
            OnDragDropReleaseHandler(surface,mDragStartPosition);
    }
}