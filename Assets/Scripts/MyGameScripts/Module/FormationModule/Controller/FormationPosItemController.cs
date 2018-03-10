// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FormationPosItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public partial class FormationPosItemController:MonolessViewController<FormationPosItem>
{
    private IDisposable _disposable;
    
    private int _posIdx;  //阵法站位编号
    public int posIdx {
        get { return _posIdx; }
    }
    
    private Subject<int> stream;
    public UniRx.IObservable<int> OnClickEvt { get{return stream;}}

    private Subject<Unit> dragStartStream;
    public Subject<Unit> DragStartEvt {
        get { return dragStartStream;}
    }

    private ModelDisplayController _modelController;
    public Action<FormationPosItemController,GameObject,Vector3> OnDragDropReleaseHandler;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        SetSelect(false);

        _modelController = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
                View.ModelAnchor
                , ModelDisplayUIComponent.NAME
        );
        _modelController.Init(80, 130, 135f, _act: DragAction.Move);

        var mUIDragDropItemCallbackable = _modelController.gameObject.GetMissingComponent<UIDragDropItemCallbackable>();
        mUIDragDropItemCallbackable.restriction = UIDragDropItemCallbackable.Restriction.Vertical;
        mUIDragDropItemCallbackable.OnDragDropReleaseHandler = OnDragDropReleaseCallBack;
        mUIDragDropItemCallbackable.OnDragStartHandler = delegate { dragStartStream.OnNext(new Unit()); };

        _disposable.CombineRelease(View.gameObject.OnClickAsObservable().Subscribe(_ => stream.OnNext(posIdx)));
        _disposable.CombineRelease(_modelController.ClickEvt.Subscribe(_ => stream.OnNext(posIdx)));
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        stream = new Subject<int>();
        dragStartStream = new Subject<Unit>();
    }

    protected override void OnDispose()
    {
        stream = stream.CloseOnceNull();
        dragStartStream = dragStartStream.CloseOnceNull();
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void OnDragDropReleaseCallBack(GameObject pSurface,Vector3 pDragStartPosition)
    {
        if (null != OnDragDropReleaseHandler)
            OnDragDropReleaseHandler(this,pSurface,pDragStartPosition);
    }

    public void SetSelect(bool selected)
    {
        View.selected_UISprite.cachedGameObject.SetActive(selected);
        View.bottomSprite.color = selected ? Color.gray : Color.white;
    }

     
     public void UpdateView(int posKey, ModelStyleInfo info = null, bool isSelect = false)
     {
        _posIdx = posKey;
        View.posLbl_UILabel.text = _posIdx.ToString();
        UpdateModelPos(Vector3.zero);
        _modelController.SetupModel(info);
        _modelController.SetModelScale(1f);
        _modelController.SetModelOffset(-0.15f);
     }

    public void UpdateView(int posKey, GeneralCharactor charactor = null, bool isSelect = false)
    {
        _posIdx = posKey;
        View.posLbl_UILabel.text = _posIdx.ToString();
        UpdateModelPos(Vector3.zero);
        _modelController.SetupModel(charactor);
        _modelController.SetModelScale(1f);
        _modelController.SetModelOffset(-0.15f);
    }

    public void UpdateModelPos(Vector3 vec)
    {
        _modelController.SetPosition(vec);
    }
}
