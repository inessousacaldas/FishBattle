// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  UseLeftViewCell.cs
// Author   : willson
// Created  : 2015/3/16 
// Porpuse  : 
// **********************************************************************
using UnityEngine;
using System.Collections.Generic;
using AppDto;

public class UseLeftViewCellController : MonoViewController<IdentifyItemUseView>
{
    protected BagItemDto _dto;
    protected BagItemDto _useDto;
    protected List<BagItemDto> _dtoList = new List<BagItemDto>();


    virtual public void SetUseDto (BagItemDto dto)
    {
    }

    virtual public void SetData (BagItemDto dto)
    {
    }

    virtual public void SetTips (string tips)
    {
    }

    public BagItemDto GetData ()
    {
        return _dto;
    }


    public List<BagItemDto> GetDataList()
    {
        return _dtoList;
    }
}