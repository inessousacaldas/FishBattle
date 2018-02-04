// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TempBackPackViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public sealed partial class BackpackDataMgr
{
    public partial class TempBackPackViewController:FRPBaseController_V1<TempBackPackView, ITempBackPackView>
    {	
        private CompositeDisposable _disposable = null;

        protected override void InitViewWithStream()
        {
            stream.OnNext(DataMgr._data);
        }

	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
            _disposable.Add(stream.Subscribe(data=>{
                UpdateDataAndView(data);
                View.UpdateView(data);}));
            _disposable.Add(stream.Subscribe(data=>View.UpdateView(data)));
            _disposable.Add(View.OnTransBtn_UIButtonClick.Subscribe(_ => TransBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnTransAllBtn_UIButtonClick.Subscribe(_ => TransAllBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnArrangeBtn_UIButtonClick.Subscribe(_ => ArrangeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnDecomposeBtn_UIButtonClick.Subscribe(_ => DecomposeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnComposite_UIButtonClick.Subscribe(_ => Composite_UIButtonClickHandler()));

        }
    }
}
