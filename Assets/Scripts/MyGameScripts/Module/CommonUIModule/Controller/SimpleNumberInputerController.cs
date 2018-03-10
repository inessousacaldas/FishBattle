// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SimpleNumberInputerController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using Debug = GameDebuger;
public partial class SimpleNumberInputerController
{
    private string curRawNumber = "0";
    private int min = 0;
    private int max = 1;
    private bool first = true;
    private UIButton[] tempButtons;
    CompositeDisposable _disposable;
    public static SimpleNumberInputerController Show<T>(
           string moduleName
           , UILayerType layerType
           , bool addBgMask
           , bool bgMaskClose = true)
           where T : MonoController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as SimpleNumberInputerController;

        return controller;
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        tempButtons = _view.GetComponentsInChildren<UIButton>();
        //tempButtons.ForEach(x =>
        //{
        //    var sprite =x.transform.Find("Sprite").GetComponent<UISprite>();
        //    if (sprite != null && !x.gameObject.name.Equals("sure") && !x.gameObject.name.Equals("back"))
        //        sprite.spriteName = "num"+x.gameObject.name;
        //}
        //);
    }
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        tempButtons.ForEach(x => 
        {
            _disposable.Add(x.AsObservable().Subscribe(_ => 
            {
                string name = x.gameObject.name;
                OnClickMethod(name);
            }));
        });
        UIEventListener.Get(_view.CloseMask_UIEventListener.gameObject).onClick += OnCloseClick;
    }
    protected override void RemoveCustomEvent()
    {
        _view.CloseMask_UIEventListener.onClick = null;
        _disposable = _disposable.CloseOnceNull();
        valueChangeStream = valueChangeStream.CloseOnceNull();
    }
    private void OnClickMethod(string buttonName)
    {
        int number = 0;
        if (buttonName == "back")
        {
            if (curRawNumber.Length <= 1)
            {
                curRawNumber = min.ToString();
                first = true;
            }
            else
                curRawNumber = curRawNumber.Substring(0, curRawNumber.Length - 1);

            if (int.TryParse(curRawNumber, out number))
                valueChangeStream.OnNext(number);
        }
        else if(int.TryParse(buttonName, out number))
        {
            if (curRawNumber.Length <= 1 && first)
                curRawNumber = buttonName;
            else
                curRawNumber += buttonName;

            if (int.TryParse(curRawNumber, out number))
            {
                if(number > max)
                {
                    number = max;
                    curRawNumber = number.ToString();
                }

                if(number < min)
                {
                    number = min;
                    curRawNumber = number.ToString();
                }
                valueChangeStream.OnNext(number);
            }
            first = false;
        }
        
        if(buttonName == "sure")
        {
            if (int.TryParse(curRawNumber, out number))
                UIModuleManager.Instance.CloseModule(SimpleNumberInputer.NAME);
        }
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }
    //protected override void OnHide()
    //{
    //    base.OnHide();
    //    curRawNumber = "0";
    //}
    public void InitData(int min,int max,GameObject anchor, UniRx.IObservable<int> parentStream =null)
    {
        this.min = min;
        this.max = max;
        View.Content_UIAnchor.container = anchor;
        View.Content_UIAnchor.Update();
        curRawNumber = min.ToString();
       // if(parentStream != null)
           // _disposable.Add(parentStream.Subscribe(i => { if (IsActive()) curRawNumber = i.ToString(); }));
    }

    public void SetPos(Vector3 pos)
    {
        _view.Content_UIAnchor.transform.localPosition = pos;
    }
    
    private void OnCloseClick(GameObject go)
    {
        // Hide();
        UIModuleManager.Instance.CloseModule(SimpleNumberInputer.NAME);
    }
    
    private Subject<int> valueChangeStream = new Subject<int>();
    public UniRx.IObservable<int> OnValueChangeStream
    {
        get { return valueChangeStream; }
    }
}
