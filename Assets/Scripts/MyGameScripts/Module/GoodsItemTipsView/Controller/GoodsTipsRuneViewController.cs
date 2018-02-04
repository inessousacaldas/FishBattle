// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GoodsTipsRuneViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
public partial class GoodsTipsRuneViewController
{
    private List<GoodsTipsSpriteItemController> _spriteItemList = new List<GoodsTipsSpriteItemController>();

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

    public void UpdateView(string text, List<string> list,int maxCount = 5)
    {
        int dif = maxCount - _spriteItemList.Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<GoodsTipsSpriteItemController, GoodsTipsSpriteItem>(View.Tabel_UITable.gameObject, GoodsTipsSpriteItem.NAME);
                _spriteItemList.Add(ctrl);
            }
        }
        else if (dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                _spriteItemList[_spriteItemList.Count - i - 1].Hide();
            }
        }

        dif = list.Count - _spriteItemList.Count;
        if (dif > 0)
            return;
        int len = 0;
        list.ForEachI((str, index) =>
        {
            len++;
            var ctrl = _spriteItemList[index];
            ctrl.Show();
            ctrl.UpdateView(str);
        });
        for(int i=len;i<maxCount;i++)
        {
            var ctrl = _spriteItemList[i];
            ctrl.Show();
            ctrl.UpdateView("");
        }

        View.NameLabel_UILabel.text = text;

        View.Tabel_UITable.Reposition();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
