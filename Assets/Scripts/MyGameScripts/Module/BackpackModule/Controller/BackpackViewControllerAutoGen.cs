// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BackpackViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public sealed partial class BackpackDataMgr
{
    public partial class BackpackViewController:FRPBaseController_V1<BackpackView, IBackpackView>
    {	
	    private CompositeDisposable _disposable = null;
	    protected override void InitViewWithStream()
        {
            stream.OnNext(DataMgr._data);
        }

	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
            _disposable.Add(stream.Subscribe(data=>UpdateView(data)));
            _disposable.Add(View.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnArrangeBtn_UIButtonClick.Subscribe(_ => ArrangeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnDecomposeBtn_UIButtonClick.Subscribe(_ => DecomposeBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnComposite_UIButtonClick.Subscribe(_ => Composite_UIButtonClickHandler()));
            _disposable.Add(View.OnCopperAddBtnSprite_UIButtonClick.Subscribe(_ => CopperAddBtnSprite_UIButtonClickHandler()));
            _disposable.Add(View.OnSiliverAddBtnSprite_UIButtonClick.Subscribe(_ => SiliverAddBtnSprite_UIButtonClickHandler()));
            _disposable.Add(View.OnGetItemBtn_UIButtonClick.Subscribe(_ => GetItemBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnWareHouseSortBtn_UIButtonClick.Subscribe(_ => WareHouseSortBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnEdittBtn_UIButtonClick.Subscribe(_ => EdittBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnSaveItemBtn_UIButtonClick.Subscribe(_=>SaveItemBtn_UIButtonClickHandler()));

        }
    }
}
