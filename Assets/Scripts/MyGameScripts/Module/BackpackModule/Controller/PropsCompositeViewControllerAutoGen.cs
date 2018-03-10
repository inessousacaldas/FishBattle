// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PropsCompositeViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public sealed partial class BackpackDataMgr
{
    public partial class PropsCompositeViewController:FRPBaseController_V1<PropsCompositeView, IPropsCompositeView>
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
            _disposable.Add(View.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnComposeBtn_UIButtonClick.Subscribe(_ => ComposeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnComposeTipBtn_UIButtonClick.Subscribe(_ => ComposeTipBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnDecomposeBtn_UIButtonClick.Subscribe(_ => DecomposeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnAnnotationBtn_UIButtonClick.Subscribe(_ => AnnotationBtn_UIButtonClickHandler()));

        }
    }
}
