// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemContainerController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial class ItemContainerController:MonolessViewController<ItemContainer>
{
    private UIScrollView _containerScrollView;
    private CompositeDisposable _disposable;
    public UIScrollView ContainerScrollView
    {
        set
        {
            View.ItemContainer_UIDragScrollView.scrollView = _containerScrollView;
            _containerScrollView = value;
        }
        get { return _containerScrollView; }
    }

    private List<ItemCellController> _cells = new List<ItemCellController>(ItemsContainerConst.PageCapability);

//    private GameObject _showTipsPos;
//    public GameObject ShowTipsPos
//    {
//        get
//        {
//            return _showTipsPos;
//        }
//        set
//        {
//            _showTipsPos = value;
//            for (int index = 0; index < _cells.Count; index++)
//            {
////                _cells[index].ShowTipsPos = value;
//            }
//        }
//    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        for (int i = 0; i < ItemsContainerConst.PageCapability; i++)
        {
            AddIfNotExitItemCell(i);
        }
    }

    protected override void OnDispose()
    {
        itemClickEvt = itemClickEvt.CloseOnceNull();
        itemDoubleClickEvt = itemDoubleClickEvt.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }

    public void SetSelect(int idx)
    {
        _cells.ForEachI(delegate(ItemCellController cell, int i)
        {
            if (cell != null)
                cell.isSelect = i == idx;
        });
    }
    public ItemCellController GetCell(int idx)
    {
        if(_cells.Count <= idx)
        {
            return null;
        }
        return _cells[idx];
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IEnumerable<object> cellDataSet, int offset, int length)
    {
        if (!this.gameObject.activeSelf  ) return;
        var i = 0;
        cellDataSet.ForEachI(delegate(object dto, int arg2) {
            var cell = _cells[i];
            cell.SetShowTips(true);
            cell.SetTipsPosition(new Vector3(-315.0f, 168.0f, 0.0f));

            if(dto == null)
                cell.UpdateViewWithNull(i >= length);
            else if(dto.GetType() == typeof(tempdto))
                cell.UpdateView(dto as tempdto,i >= length);
            else if (dto.GetType() == typeof (BagItemDto))
            {
                cell.UpdateView(dto as BagItemDto,i >= length);
            }
            else if(dto.GetType() == typeof(CrewChipDto))
                cell.UpdateView(dto as CrewChipDto,i >= length);
            else if(dto.GetType() == typeof(ItemDto)) {
                cell.UpdateMissonItemView(dto as ItemDto);
            }
            i++;
        });

        for (; i < ItemsContainerConst.PageCapability; i++)
        {
            var cell = _cells[i];
            cell.UpdateViewWithNull(i >= length);
        }
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IEnumerable<BagItemDto> data, int length)
    {
        if (!this.gameObject.activeSelf) return;
        for (var i = 0; i < ItemsContainerConst.PageCapability; i++)
        {
            var dto = data.Find(d=> d.index % ItemsContainerConst.PageCapability == i);
            AddIfNotExitItemCell(i);
            var cell = _cells[i];
            if (dto != null)
            {
                cell.UpdateView(dto, i >= length);
            }
            else
                cell.UpdateView();
        }
    }

    public void UpdateView(Func<int, BagItemDto> getItem, int length)
    {
        if (!this.gameObject.activeSelf  ) return;
        for (var i = 0; i < ItemsContainerConst.PageCapability; i++)
        {
            var dto = getItem(i);
            var cell = _cells[i];
            cell.UpdateView(dto, i >= length);
        }
    }

    private void AddIfNotExitItemCell(int index)
    {
        ItemCellController cell = null;
        _cells.TryGetValue(index, out cell);
        if (cell == null)
        {
            cell = AddCachedChild<ItemCellController, ItemCell>(
                View.GridGroup_UIGrid.gameObject
                , ItemCell.Prefab_BagItemCell
                , "itemcell_" + index.ToString());

            cell.ContainerScrollView = ContainerScrollView;
            _disposable.Add(cell.OnCellClick.Subscribe(_ => 
            {
                itemClickEvt.OnNext(index);
             }));
            
            _disposable.Add(cell.OnCellDoubleClick.Subscribe(
                    _=>itemDoubleClickEvt.OnNext(index)
                )
            );

            _cells.Add(cell);
        }
    }

    private Subject<int> itemClickEvt = new Subject<int>();
    public UniRx.IObservable<int> OnItemClick {
        get { return itemClickEvt; }
    }

    private Subject<int> itemDoubleClickEvt = new Subject<int>();
    public UniRx.IObservable<int> OnItemDoubleClick {
        get { return itemDoubleClickEvt; }
    }
    
    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void SetMultipleSelect(IEnumerable<int> indexSet)
    {
        _cells.ForEach(cell =>
        {
            if (cell != null)
                cell.isSelect = false;
        });
        
        indexSet.ForEach(idx=>
        {
            var cell = _cells.TryGetValue(idx);
            
            if (cell != null)
                cell.isSelect = true;
        });
    }
}
