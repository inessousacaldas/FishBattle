using System;
using UnityEngine;

public class DragDropItem: UIDragDropItem
{
    private Action _onDragStartAct;
    private Action _onDragDropEndAct;
	private bool isNeedAddPanel;

    public Action<GameObject, Vector3> OnDragDropReleaseHandler;
    public Action OnDragStartAct {
        set { _onDragStartAct = value; }
    }

    public Action OnDragDropEndAct
    {
        set { _onDragDropEndAct = value; }
    }

    public delegate void OnDragEnd ();

    protected override void OnDragStart()
    {
        base.OnDragStart();
        if (this.gameObject.GetComponent<UIPanel>() == null)
        {
            isNeedAddPanel = true;
//            this.gameObject.AddComponent<UIPanel>();
        }

        if (_onDragStartAct != null)
            _onDragStartAct ();
    }

    protected override void OnDragDropEnd()
    {
        base.OnDragDropEnd();
        if (!isNeedAddPanel)
        {
//            this.gameObject.RemoveComponent<UIPanel>();
        }

        if (_onDragDropEndAct != null)
            _onDragDropEndAct ();
    }

    protected override void OnDragDropRelease(GameObject go)
    {
        base.OnDragDropRelease(go);
        if (OnDragDropReleaseHandler != null)
            OnDragDropReleaseHandler(go, this.transform.localPosition);
    }
}
