// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  UseItemCellController.cs
// Author   : willson
// Created  : 2015/3/14 
// Porpuse  : 
// **********************************************************************
using UnityEngine;
using AppDto;

public class UseItemCellController : MonolessViewController<UseItemCell>
{
    private ItemCellController _cell;

    private int _currCount;

    private BagItemDto _itemDto;
    private bool _isMultiple;

    private System.Action<UseItemCellController> _onClickCallBack;
    private System.Action<UseItemCellController> _onRemoveCallBack;


    protected override void AfterInitView()
    {
        _isMultiple = false;

        _cell = AddCachedChild<ItemCellController,ItemCell>(_view.gameObject, ItemCell.NAME);
        _cell.CanDisplayCount(false);
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(_view.RomeveBtn.onClick, OnRomeveBtn);
    }

    public void SetData(BagItemDto itemDto, System.Action<UseItemCellController> onClickCallBack, 
                        bool isMultiple = false, System.Action<UseItemCellController> onRemoveCallBack = null)
    {
        _isMultiple = isMultiple;
        _onClickCallBack = onClickCallBack;
        _onRemoveCallBack = onRemoveCallBack;

        SetData(itemDto);
    }

    public void SetData(BagItemDto itemDto)
    {
        _currCount = 0;
        _cell.isSelect = false;
		
        _itemDto = itemDto;
        if(itemDto != null) {
            _cell.UpdateView(_itemDto,OnItemClick);    //itemcell接口改了,先屏蔽处理,防止报错
        }
        if (_itemDto == null)
        {
            _view.RomeveBtn.gameObject.SetActive(false);
            _view.CountLabel.text = "";
        }
        else
        {
            if (_isMultiple)
            {
                SetMultipleState();
            }
            else
            {
                if (_itemDto.count > 1)
                {
                    _view.CountLabel.text = _itemDto.count.ToString();
                }
                else
                {
                    _view.CountLabel.text = "";
                }
                _view.RomeveBtn.gameObject.SetActive(false);
            }
        }
    }

    private void SetMultipleState()
    {
        if (_itemDto.item.maxOverlay > 1)
        {
            _view.RomeveBtn.gameObject.SetActive(_currCount > 0);
            _view.CountLabel.text = string.Format("{0}/{1}", _currCount, _itemDto.count);
            _cell.isSelect = _currCount > 0;
        }
        else
        {
            _view.RomeveBtn.gameObject.SetActive(false);
            _view.CountLabel.text = "";
        }
    }

    public BagItemDto GetData()
    {
        return _itemDto;
    }

    public int GetCount()
    {
        return _currCount;
    }

    public void SelectSingle(bool b)
    {
        _cell.isSelect = b;
        _currCount = b ? 1 : 0;
    }

    public bool IsSelect
    {
        get
        {
            return _cell.isSelect;
        }
    }

    public void SelectMultiple()
    {
        if (_itemDto.item.maxOverlay == 1)
        {
            _cell.isSelect = !_cell.isSelect;
            if (_cell.isSelect)
            {
                _currCount = 1;
            }
            else
            {
                _currCount = 0;
            }
        }
        else
        {
            _cell.isSelect = true;

            if (_currCount < _itemDto.count)
                _currCount++;
        }

        if (_isMultiple)
        {
            SetMultipleState();
        }
    }

    private void OnItemClick(ItemCellController itemCellController)
    {
        if (_onClickCallBack != null)
            _onClickCallBack(this);
    }

    private void OnRomeveBtn()
    {
        if (_currCount > 0)
            _currCount--;

        if (_isMultiple)
        {
            SetMultipleState();
        }

        if (_onRemoveCallBack != null)
            _onRemoveCallBack(this);
    }
}