// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GenericCheckboxController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;

public interface IGenericCheckBoxData
{
    string Name { get; }
    bool IsSelect { get; }
}

public class GenericCheckBoxData : IGenericCheckBoxData
{
    private string name;

    public string Name
    {
        get { return name; }
    }

    public bool IsSelect
    {
        get { return isSelect; }
    }

    private bool isSelect;

    public static GenericCheckBoxData Create(string name, bool isSelect)
    {
        var data = new GenericCheckBoxData();
        data.name = name;
        data.isSelect = isSelect;
        return data;
    }
}

public interface IGenericCheckBox
{
    IObservable<bool> ClickStateHandler { get; }
    void UpdateView(IGenericCheckBoxData data);
    bool IsSelect {get;set;}
}

public partial class GenericCheckBoxController : IGenericCheckBox
{
    private Subject<Unit> clickEvt;

    private Subject<bool> clickState;
    public IObservable<bool> ClickStateHandler {
        get { return clickState; }
    }

    // 界面初始化完成之后的一些后续初始化工作

    protected override void AfterInitView ()
    {
        clickEvt = _view.gameObject.OnClickAsObservable();
        clickState = new Subject<bool>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        clickEvt.Subscribe(_ =>
        {
            IsSelect = !IsSelect;
            clickState.OnNext(IsSelect);
        });
        clickState.Hold(IsSelect);
    }

    protected override void OnDispose()
    {
        clickEvt = clickEvt.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView( IGenericCheckBoxData data)
    {
        _view.Label_UILabel.text = data.Name;
        IsSelect = data.IsSelect;
        clickState.Hold(IsSelect);
    }

    public bool IsSelect {
        get { return View.Checkmark_UISprite.enabled; }
        set { View.Checkmark_UISprite.enabled = value;}
    }
}
