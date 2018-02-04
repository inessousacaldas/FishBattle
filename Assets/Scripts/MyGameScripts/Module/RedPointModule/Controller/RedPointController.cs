// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPointController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class RedPointController
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

    public void InitView(int type, int depth)
    {
        var pointData = RedPointDataMgr.DataMgr.GetRedPointData(type);
        if (pointData == null)
        {
            View.Sprite_UISprite.enabled = false;
            View.Num_UILabel.text = "";
            return;
        }

        View.Sprite_UISprite.enabled = pointData.isShow;
        View.Sprite_UISprite.depth = depth;
        View.Num_UILabel.text = pointData.num > 0 ? pointData.num.ToString() : "";
    }

    public void SetShow(bool isShow, int count = 0)
    {
        View.Sprite_UISprite.enabled = isShow;
        View.Num_UILabel.text = count > 0 ? count.ToString() : "";
    }
}
