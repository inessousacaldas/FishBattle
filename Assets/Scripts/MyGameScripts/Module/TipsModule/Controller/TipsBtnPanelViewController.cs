// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsBtnPanelViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

public partial class TipsBtnPanelViewController
{
    private static CompositeDisposable _disposable;
    private Action _onLeftClick;
    private Action _onRightClick;
    private const int _exBgHeigher = 10;  //额外按钮的背景 有遮挡或超出按钮高度部分 

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
    }

    protected override void OnDispose()
    {

    }

    public void UpdateView(string left, string right, Action leftClick, Action rightClick)
    {
        View.LeftLabel_UILabel.text = left;
        View.RightLabel_UILabel.text = right;

        View.RightBtn_UIButton.gameObject.SetActive(rightClick != null);
        View.LeftBtn_UIButton.gameObject.SetActive(leftClick != null);

        if (rightClick != null)
        {
            _onRightClick = rightClick;
            _disposable.Add(OnRightBtn_UIButtonClick.Subscribe(_ => _onRightClick()));
        }

        if (leftClick != null)
        {
            _onLeftClick = leftClick;
            _disposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ => _onLeftClick()));
        }
        View.RightBtn_UIButton.transform.localPosition = leftClick == null
            ? View.MiddleTrans.localPosition
            : new Vector3(60, 0, 0);
        View.LeftBtn_UIButton.transform.localPosition = rightClick == null
            ? View.MiddleTrans.localPosition
            : new Vector3(-60, 0, 0);
    }

    public void OnLeftClick()
    {
        //View.LeftExpandBg_UISprite.gameObject.SetActive(!View.LeftExpandBg_UISprite.gameObject.activeSelf);
        View.LeftExpandTable_UITable.gameObject.SetActive(!View.LeftExpandTable_UITable.gameObject.activeSelf);
    }

    public void UpdateView(Dictionary<string, Action> leftDic, string right, Action rightClick, int depth)
    {
        var itemHeight = 0;
        if (leftDic.Count == 1)
        {
            var key = leftDic.Keys.ToList()[0];
            View.LeftLabel_UILabel.text = key;
            _disposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ =>
            {
                var action = leftDic.Values.ToList()[0];
                if (action != null)
                    action();
            }));
        }
        else
        {
            View.LeftLabel_UILabel.text = "更多";
            leftDic.ForEachI((KVpair, idx) =>
            {
                var ctrl = AddChild<TipsBtnItemController, TipsBtnItem>(View.LeftExpandTable_UITable.gameObject, TipsBtnItem.NAME);
                ctrl.UpdateView(KVpair.Key, KVpair.Value);
                itemHeight = ctrl.GetHeight();
                _disposable.Add(ctrl.OnBtn_UIButtonClick.Subscribe(_ =>
                {
                    if (KVpair.Value != null)
                        KVpair.Value();
                }));
            });
            _disposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ =>
            {
                OnLeftClick();
            }));
        }

        this.gameObject.GetComponent<UIPanel>().depth = depth;
        View.LeftExpandTable_UITable.Reposition();

        View.LeftExpandBg_UISprite.SetDimensions(View.LeftExpandBg_UISprite.width, (leftDic.Count-1)* itemHeight + _exBgHeigher);

        View.RightLabel_UILabel.text = right;
        if (rightClick != null)
        {
            _onRightClick = rightClick;
            _disposable.Add(OnRightBtn_UIButtonClick.Subscribe(_ => _onRightClick()));
        }
    }
    
    public int GetHeight()
    {
        return this.gameObject.GetComponent<UIWidget>().height;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
