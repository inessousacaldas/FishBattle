// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  Eq_GoodsChoiceContentController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UniRx;
public partial class Eq_GoodsChoiceContentController
{
    // 界面初始化完成之后的一些后续初始化工作
    List<Eq_GoodItemController> eq_goodCtrls = new List<Eq_GoodItemController>();

    Subject<GeneralItem> _onClickGoodsStream = new Subject<GeneralItem>();
    public UniRx.IObservable<GeneralItem> OnClickGoodsStream { get { return _onClickGoodsStream; } }
    CompositeDisposable _disposable;
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
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

    public void UpdateView(IEnumerable<Eq_GoodItemVo> bagItem)
    {
        _disposable.Clear();
        eq_goodCtrls.ForEach(x => x.Hide());
        bagItem.ForEachI((x, i) => {
            if(eq_goodCtrls.Count <= i)
            {
                var ctrl = AddChild<Eq_GoodItemController, Eq_GoodItem>(View.RightContent_UIGrid.gameObject, Eq_GoodItem.NAME);
                eq_goodCtrls.Add(ctrl);
            }
            _disposable.Add(eq_goodCtrls[i].OnEmbedItem_UIButtonClick.Subscribe(_=> {
                var tempIem = x.item;
                _onClickGoodsStream.OnNext(tempIem);
            }));
            eq_goodCtrls[i].UpdateDataView(x);
            eq_goodCtrls[i].Show();
        });

        View.RightContent_UIGrid.Reposition();
        //View.ScrollView_UIScrollView.ResetPosition();
    }
}
