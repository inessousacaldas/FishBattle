// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SkillItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class SkillItemController
{
    private Subject<Unit> _itemOnClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> ItemClickHandler { get { return _itemOnClickEvt; } }

    private Subject<Unit> _onPress = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetPressHandler { get { return _onPress; } }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(_view.SkillItem_UIEventTrigger.onPress, () =>
        {
            JSTimer.Instance.SetupCoolDown("SkillItemTrigger", 1f, null, () => { _onPress.OnNext(new Unit()); });
        });
        EventDelegate.Set(_view.SkillItem_UIEventTrigger.onRelease,
            () => { JSTimer.Instance.CancelCd("SkillItemTrigger"); });

        EventDelegate.Add(_view.SkillItem_UIButton.onClick, () => { _itemOnClickEvt.OnNext(new Unit()); });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(int arg1DefaultSkillId)
    {
        var skill = DataCache.getDtoByCls<Skill>(arg1DefaultSkillId);
        if (skill == null)
        {
            GameDebuger.LogError(string.Format("Skill表不存在{0},请检查", arg1DefaultSkillId));
            return;
        }
        UIHelper.SetUITexture(_view.spIcon_UISprite, skill.icon);
        _view.lblName_UILabel.text = skill.name;
    }

    public void UpdateView(Skill skill)
    {
        UIHelper.SetUITexture(_view.spIcon_UISprite, skill.icon);
        _view.lblName_UILabel.text = skill.name;
    }

    public void SetBgSprite(string sprite, int height, int width)
    {
        _view.spIconBg_UISprite.spriteName = sprite;
        _view.spIconBg_UISprite.height = height;
        _view.spIconBg_UISprite.width = width;
    }

    public void HideNameLb()
    {
        _view.lblName_UILabel.gameObject.SetActive(false);
    }
}
