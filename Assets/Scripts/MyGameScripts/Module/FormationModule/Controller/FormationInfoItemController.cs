// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FormationInfoItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;
using UnityEngine;

public enum FormationState
{
    None,
    UnEnable, // 未掌握
    Enable, // 可学习
    Learned// 已经学习
}

public partial interface IFormationInfoItemController
{
    UniRx.IObservable<Unit> GetOpenHandler { get; }
    UniRx.IObservable<Unit> GetCloseHandler { get; }
    void SetOpenBtnState(bool b);
    void SetCloseBtnState(bool b);
}

public partial class FormationInfoItemController
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    private Subject<Unit> _openEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetOpenHandler { get { return _openEvt; } }

    private Subject<Unit> _closeEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetCloseHandler { get { return _closeEvt; } } 
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OpenBtn_UIButtonEvt.Subscribe(_=> {_openEvt.OnNext(new Unit());}));
        _disposable.Add(CloseBtn_UIButtonEvt.Subscribe(_ => { _closeEvt.OnNext(new Unit());}));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(Formation formation, FormationState state, int level, bool isSelect, bool isActive)
    {
        if (formation.id == (int) Formation.FormationType.Regular)
        {
            View.nameLbl_UILabel.text = formation.name;
        }
        else
        {
            switch (state){
                case FormationState.UnEnable:
                        View.nameLbl_UILabel.text = formation.name; 
                        View.stateLb_UILabel.text = "未学习";
                    break;
                case FormationState.Learned:
                        View.nameLbl_UILabel.text = formation.name;
                        View.stateLb_UILabel.text = string.Format("Lv.{0}", level);
                    break;
            }
        }

        SetSelect(isSelect);
        SetFlagActive(isActive);
        _view.iconSprite_UISprite.spriteName = string.Format("formationicon_{0}", formation.id);
    }

    public void SetFlagActive(bool active){
        View.flagSprite_UISprite.cachedGameObject.SetActive(active);
    }

    public void SetSelect (bool selected)
    {
        View.Select.SetActive(selected);
    }

    public void SetOpenBtnState(bool b) { _view.OpenBtn_UIButton.gameObject.SetActive(b); }

    public void SetCloseBtnState(bool b) { _view.CloseBtn_UIButton.gameObject.SetActive(b);}
}