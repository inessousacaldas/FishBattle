// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SearchFriendViewController.cs
// Author   : xjd
// Created  : 10/13/2017 6:07:28 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;


public partial interface ISearchFriendViewController
{
    string InputFiledText { get; set; }

    UniRx.IObservable<long> OnSearchStream { get; }
}

public partial class SearchFriendViewController
{
    public string InputFiledText
    {
        get { return View.InputField_UIInput.value; }
        set { View.InputField_UIInput.value = value; }
    }

    private List<SearchFriendItemController> _searchItemList = new List<SearchFriendItemController>();

    readonly UniRx.Subject<long> searchStream = new UniRx.Subject<long>();

    public UniRx.IObservable<long> OnSearchStream { get { return searchStream; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.InputField_UIInput.onChange, OnInputValueChange);
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();

        SearchFriendDataMgr.DataMgr.ClearSearchList();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ISearchFriendData data)
    {
        var dataItemList = data.SearchItemList;
        var itemCount = 0;
        _searchItemList.GetElememtsByRange(itemCount, -1).ForEach(item => item.Hide());
        dataItemList.ForEachI((itemDto, index) =>
        {
            var ctrl = AddFriendItemNoExist(index);
            ctrl.UpdateView(itemDto);
            ctrl.Show();

            _disposable.Add(ctrl.OnClickItemStream.Subscribe(id =>
            {
                searchStream.OnNext(id);
            }));
        });

        View.Grid_UIGrid.Reposition();

        if (data.IsChange)
        {
            View.ScrollView_UIScrollView.ResetPosition();
            data.IsChange = false;
        }  
    }

    private SearchFriendItemController AddFriendItemNoExist(int index)
    {
        SearchFriendItemController ctrl = null;
        _searchItemList.TryGetValue(index, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<SearchFriendItemController, SearchFriendItem>(View.Grid_UIGrid.gameObject, SearchFriendItem.NAME);
            _searchItemList.Add(ctrl);
        }

        return ctrl;
    }

    private void OnInputValueChange()
    {
        
    }

}
