// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BaseItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
public partial class BaseItemController
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

    #region 获取途径

    public void GainWayItemInfo(int id)
    {
        var guide = DataCache.getDtoByCls<GuideGainWay>(id);
        if (guide == null)
        {
            GameDebuger.LogError(string.Format("GuideGainWay表找不到{0}", id));
            return;
        }
        var smart = DataCache.getDtoByCls<SmartGuide>(guide.smartGuideId);
        if (smart == null)
        {
            GameDebuger.LogError(string.Format("SmartGuide表找不到{0}", guide.smartGuideId));
            return;
        }

        _view.NameLb_UILabel.text = smart.name;
        _view.Icon_UISprite.spriteName = smart.icon;
        _view.Arrow_UISprite.gameObject.SetActive(true);
        _view.MarkSprite_UISprite.gameObject.SetActive(false);
    }

    public bool ShowMarkSprite
    {
        set { _view.MarkSprite_UISprite.gameObject.SetActive(value); }
    }
    #endregion

}
