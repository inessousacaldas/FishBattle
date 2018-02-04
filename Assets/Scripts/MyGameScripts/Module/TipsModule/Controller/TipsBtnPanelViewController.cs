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

        if (leftClick != null)
        {
            _onLeftClick = leftClick;
            _disposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ => _onLeftClick()));
        }

        if (rightClick != null)
        {
            _onRightClick = rightClick;
            _disposable.Add(OnRightBtn_UIButtonClick.Subscribe(_ => _onRightClick()));
            View.RightBtn_UIButton.gameObject.SetActive(true);
        }
        else
        {
            View.LeftBtn_UIButton.transform.localPosition = View.MiddleTrans.localPosition;
            View.RightBtn_UIButton.gameObject.SetActive(false);
        }
    }

    public void OnLeftClick()
    {
        View.LeftExpandBg_UISprite.gameObject.SetActive(!View.LeftExpandBg_UISprite.gameObject.activeSelf);
        View.LeftExpandTable_UITable.gameObject.SetActive(!View.LeftExpandTable_UITable.gameObject.activeSelf);
    }

    public void OnRightClick()
    {
        View.RightExpandBg_UISprite.gameObject.SetActive(!View.RightExpandBg_UISprite.gameObject.activeSelf);
        View.RightExpandTable_UITable.gameObject.SetActive(!View.RightExpandTable_UITable.gameObject.activeSelf);
    }

    public void UpdateView(Dictionary<string, Action> leftDic, string right, Action rightClick, int depth)
    {
        var itemHeight = 0;
        leftDic.ForEachI((KVpair, idx) =>
        {
            if (idx == 0)
            {
                View.LeftLabel_UILabel.text = KVpair.Key;
                //if(KVpair.Value == null)
                    _disposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ =>
                    {
                        View.LeftExpandBg_UISprite.gameObject.SetActive(!View.LeftExpandBg_UISprite.gameObject.activeSelf);
                        View.LeftExpandTable_UITable.gameObject.SetActive(!View.LeftExpandTable_UITable.gameObject.activeSelf);
                    }));
            }
            else
            {
                var ctrl = AddChild<TipsBtnItemController, TipsBtnItem>(View.LeftExpandTable_UITable.gameObject, TipsBtnItem.NAME);
                ctrl.UpdateView(KVpair.Key, KVpair.Value);
                itemHeight = ctrl.GetHeight();
                _disposable.Add(ctrl.OnBtn_UIButtonClick.Subscribe(_ =>
                {
                    View.LeftExpandBg_UISprite.gameObject.SetActive(false);
                    View.LeftExpandTable_UITable.gameObject.SetActive(false);
                    if(KVpair.Value != null)
                        KVpair.Value();
                }));
            }
        });

        //rightDic.ForEachI((KVpair, idx) =>
        //{
        //    if (idx == 0)
        //    {
        //        View.RightLabel_UILabel.text = KVpair.Key;
        //        if (KVpair.Value == null)
        //            _disposable.Add(OnRightBtn_UIButtonClick.Subscribe(_ =>
        //            {
        //                View.RightExpandBg_UISprite.gameObject.SetActive(!View.RightExpandBg_UISprite.gameObject.activeSelf);
        //                View.RightExpandTable_UITable.gameObject.SetActive(!View.RightExpandTable_UITable.gameObject.activeSelf);
        //            }));
        //    }
        //    else
        //    {
        //        var ctrl = AddChild<TipsBtnItemController, TipsBtnItem>(View.RightExpandTable_UITable.gameObject, TipsBtnItem.NAME);
        //        ctrl.UpdateView(KVpair.Key, KVpair.Value);
        //        _disposable.Add(ctrl.OnBtn_UIButtonClick.Subscribe(_ =>
        //        {
        //            View.RightExpandBg_UISprite.gameObject.SetActive(false);
        //            View.RightExpandTable_UITable.gameObject.SetActive(false);
        //            if (KVpair.Value != null)
        //                KVpair.Value();
        //        }));
        //    }
        //});

        this.gameObject.GetComponent<UIPanel>().depth = depth;
        View.LeftExpandTable_UITable.Reposition();
        //View.RightExpandTable_UITable.Reposition();

        Bounds bLeft = NGUIMath.CalculateRelativeWidgetBounds(View.LeftExpandTable_UITable.transform);
        //Bounds bRight = NGUIMath.CalculateRelativeWidgetBounds(View.RightExpandTable_UITable.transform);
        View.LeftExpandBg_UISprite.SetDimensions(View.LeftExpandBg_UISprite.width, (leftDic.Count-1)* itemHeight + _exBgHeigher);
        var oldPos = View.LeftExpandTable_UITable.transform.localPosition;
        View.LeftExpandTable_UITable.transform.localPosition = new Vector3(oldPos.x, oldPos.y + (leftDic.Count - 1) * itemHeight);
        //View.RightExpandBg_UISprite.SetDimensions((int)bRight.size.x, (int)bRight.size.y);

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
