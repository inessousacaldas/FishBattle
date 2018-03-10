// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BuffShowItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;

public partial interface IBuffShowItemController
{
    UniRx.IObservable<Unit> OnBuffShowItem_UIButtonClick { get; }
    void UpdateView();
}

public partial class BuffShowItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView()
    {
        _view.NameLb_UILabel.text = "飞龙在天";
        _view.DescLb_UILabel.text = "获得200%经验加成,持续6天,一刀999级!";
        UIHelper.SetSkillIcon(_view.Icon_UISprite, "1111");
    }

}
