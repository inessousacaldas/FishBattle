// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsLabAndSprViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TipsLabAndSprViewController
{
    private List<TipsSpriteItemController> _spriteItemList = new List<TipsSpriteItemController>();

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
        _spriteItemList.Clear();
    }

    public void UpdateView(string text, List<string> list, List<int> qualityList=null)
    {
        int dif = list.Count - _spriteItemList.Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<TipsSpriteItemController, TipsSpriteItem>(View.Tabel_UITable.gameObject, TipsSpriteItem.NAME);
                _spriteItemList.Add(ctrl);
            }
        }
        else if (dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                _spriteItemList[_spriteItemList.Count - i - 1].gameObject.SetActive(false);
            }
        }

        dif = list.Count - _spriteItemList.Count;
        if (dif > 0)
            return;

        var isShowQuality = false;
        if (qualityList!=null && qualityList.Count == list.Count)
            isShowQuality = true;
        list.ForEachI((str, index) =>
        {
            var ctrl = _spriteItemList[index];
            ctrl.gameObject.SetActive(true);
            var quality = isShowQuality ? qualityList[index] : 1;
            ctrl.UpdateView(str,quality);
        });

        View.NameLabel_UILabel.text = text;

        View.Tabel_UITable.Reposition();
        View.Tabel_UITable.transform.localPosition = new Vector3(View.NameLabel_UILabel.transform.localPosition.x + View.NameLabel_UILabel.width, View.Tabel_UITable.transform.localPosition.y);
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
