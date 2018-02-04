// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemExpantViewController.cs
// Author   : xush
// Created  : 7/2/2017 4:22:27 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IItemExpantViewController
{
    void InitCellList(IEnumerable<BagItemDto> dataList);
    void ShowOrHide(bool b);
    string SetBtnName { set; }

    UIPanel GetItemPanel { get; }
    UniRx.IObservable<Unit> GetClickHandler { get; }
    UniRx.IObservable<int> GetItemClickHandler { get; } 
}
public partial class ItemExpantViewController
{
    private CompositeDisposable _disposable;
    private List<ItemCellController> _itemList = new List<ItemCellController>();
    private delegate void _func(ItemCellController con, int idx);

    private const int ItemCellNum = 20; //默认20个

    #region Subject
    private Subject<Unit> _btnClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetClickHandler { get { return _btnClickEvt; } }

    private Subject<int> _itemClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetItemClickHandler { get { return _itemClickEvt; } }
    
    #endregion
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
    }

    private void InitItem(int num)
    {
        _func f = delegate (ItemCellController con, int idx)
        {
            _disposable.Add(con.OnCellClick.Subscribe(_ =>
            {
                _itemList.ForEachI((item, i) => { item.isSelect = i == idx; });
                _itemClickEvt.OnNext(idx);
            }));
        };

        var n = num > ItemCellNum ? num : ItemCellNum;
        for (int i = _view.Grid_UIGrid.transform.childCount; i < n; i++)
        {
            var controller = AddChild<ItemCellController, ItemCell>(_view.Grid_UIGrid.gameObject, ItemCell.NAME);

            _itemList.Add(controller);
            f(controller, i);
        }
        _view.Grid_UIGrid.Reposition();
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(_view.Button_UIButton.AsObservable().Subscribe(_ =>
        {
            _btnClickEvt.OnNext(new Unit());
            Close();
        }));
    }

    protected override void RemoveCustomEvent ()
    {
    }
    
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    public void InitCellList(IEnumerable<BagItemDto> dataList)
    {
        if (dataList == null) return;
        //每次添加一排(4个)格子,而不是每次增加1个格子
        var num = Mathf.Ceil((float) dataList.Count()/4f)*4;
        InitItem((int)num);
        _itemList.ForEachI((item, idx) =>
        {
            item.isSelect = idx == 0;   //默认选中第一个
            if (idx < dataList.Count())
                item.ExpantOrbmentItem(dataList.TryGetValue(idx));
            else
                item.UpdateView();
        });
    }

    public void ShowOrHide(bool b)
    {
        gameObject.SetActive(b);
    }

    public string SetBtnName{set { _view.BtnName_UILabel.text = value; } }

    private void Close()
    {
        UIModuleManager.Instance.CloseModule(ItemExpantView.NAME);
    }

    public UIPanel GetItemPanel { get { return _view.ScrollView_UIScrollView.panel; } }
}
