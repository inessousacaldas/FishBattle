// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PreviewSTLineController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
public partial class PreviewSTLineController
{
    List<UISprite> spritePools = new List<UISprite>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        spritePools.AddRange(View.IconItems_Transform.GetComponentsInChildren<UISprite>());

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        spritePools.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    public void InitDataView(string title,List<string> IconList)
    {
        View.NameLbl_UILabel.text = title;
        spritePools.ForEach(x => x.gameObject.SetActive(false));
        IconList.ForEachI((x, i) => {
            if(i >= spritePools.Count) //最多只能容纳5个~
            {
                GameDebuger.LogError("Sprite不足,请确认最大数量");
                return;
            }
            UIHelper.SetItemIcon(spritePools[i], x);
            spritePools[i].gameObject.SetActive(true);
        });
    }
}
