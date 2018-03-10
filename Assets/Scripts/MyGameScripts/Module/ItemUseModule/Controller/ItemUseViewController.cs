// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ItemUseViewController.cs
// Author   : willson
// Created  : 2015/3/14 
// Porpuse  : 
// **********************************************************************
using AppDto;
using System.Collections.Generic;
using System;

//物品使用
public class ItemUseViewController : MonoViewController<ItemUseView>
{
    protected const string UseItemCellName = "UseItemCell";
    protected UseLeftViewCellController _leftView;

    protected BagItemDto _useDto;

    protected bool _isMultiple;
    protected bool _isBefore;
    protected bool _isCanReClick;

    protected List<BagItemDto> _items;
    protected List<UseItemCellController> _itemCellList;

    protected const int PAGE_COUNT = 12;
    protected Summary _summary;

    protected override void AfterInitView()
    {
        _itemCellList = new List<UseItemCellController>();

        InitLeftGroup();
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(View.CloseBtn.onClick, OnClose);
        EventDelegate.Add(View.OptBtn.onClick, OnOptBtn);
    }

    virtual protected void InitLeftGroup()
    {
    }

    virtual public void SetData(BagItemDto useDto, List<BagItemDto> items, bool isMultiple = false, bool isBefore = false, bool isCanReClick = true)
    {
        _useDto = useDto;
        _items = items;

        _isMultiple = isMultiple;
        _isBefore = isBefore;
        _isCanReClick = isCanReClick;

        int iTotalPage = (int)Math.Ceiling((double)_items.Count / PAGE_COUNT);
        _summary = Summary.create(_items.Count, iTotalPage, 1, PAGE_COUNT);
        iTotalPage = iTotalPage == 0 ? 1 : iTotalPage;

        AddItem(iTotalPage);

        View.LGroup.SetActive(_items.Count > 0);

        _leftView.SetUseDto(useDto);
    }

    public void AddItem(int page)
    {
        for (int index = 0, totalCount = PAGE_COUNT * page; index < totalCount; index++)
        {
            UseItemCellController cell = AddCachedChild<UseItemCellController,UseItemCell>(View.itemGrid.gameObject, UseItemCell.NAME);
            if (index < _items.Count)
            {
                cell.SetData(_items[index], OnItemClick, _isMultiple);
            }
            else
            {
                cell.SetData(null, OnItemClick, _isMultiple);
            }
            _itemCellList.Add(cell);
        }
    }

    virtual protected void OnItemClick(UseItemCellController cell)
    {
        if (cell.GetData() != null)
        {
            if (_isMultiple)
            {
                cell.SelectMultiple();
            }
            else
            {
                for (int index = 0; index < _itemCellList.Count; index++)
                {
                    _itemCellList[index].SelectSingle(cell == _itemCellList[index]);
                }
            }
            _leftView.SetData(cell.GetData());
        }
    }

    virtual protected void OnOptBtn()
    {
    }

    private void OnClose()
    {
        ProxyItemUseModule.Close();
    }
}