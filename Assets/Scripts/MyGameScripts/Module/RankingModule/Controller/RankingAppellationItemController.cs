// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingAppellationItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UnityEngine;

public partial class RankingAppellationItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
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

    public void SetItemInfo(RankItemDto dto, int index)
    {
        SetNullItem();
        _view.PlayerIcon_UISprite.gameObject.SetActive(dto != null);
        if (dto == null)
            return;

        var type = dto.GetType();
        switch (type.ToString())
        {
            case "AppDto.RankCrewDto":
                SetRankCrewItem(dto as RankCrewDto, index);
                break;
            case "AppDto.RankFlowersDto":
                SetFlowerItem(dto as RankFlowersDto, index);
                break;
            case "AppDto.RankTowerDto":
                SetRankTowerItem(dto as RankTowerDto, index);
                break;
        }
    }

    private void SetRankCrewItem(RankCrewDto rankData, int index)
    {
        _view.NameLabel_UILabel.text = rankData.playerName;
        _view.SorceLabel_UILabel.text = string.Format("{0}\r\n{1}战力",rankData.name, rankData.crewPower).WrapColor(ColorConstantV3.Color_Yellow_Str);
        _view.RankingMark_UISprite.spriteName = string.Format("Ranking_{0}", index);
        transform.localScale = index == 1 ?
            new Vector3(1f, 1f, 1f)
            : new Vector3(0.8f, 0.8f, 0.8f);
        var charactor = DataCache.getDtoByCls<GeneralCharactor>(rankData.charactorId);
        if (charactor is MainCharactor)
            UIHelper.SetPetIcon(_view.PlayerIcon_UISprite, (charactor as MainCharactor).texture.ToString());
    }

    private void SetFlowerItem(RankFlowersDto dto, int index)
    {
        _view.NameLabel_UILabel.text = dto.name;
        _view.SorceLabel_UILabel.text = string.Format("鲜花:{0}", dto.flowers).WrapColor(ColorConstantV3.Color_Yellow_Str);
        _view.RankingMark_UISprite.spriteName = string.Format("Ranking_{0}", index);
        var charactor = DataCache.getDtoByCls<GeneralCharactor>(dto.charactorId);
        if(charactor is MainCharactor)
            UIHelper.SetPetIcon(_view.PlayerIcon_UISprite, (charactor as MainCharactor).texture.ToString());
    }

    private void SetRankTowerItem(RankTowerDto dto, int index)
    {
        _view.NameLabel_UILabel.text = dto.name;
        _view.SorceLabel_UILabel.text = string.Format("通关层数:{0}", dto.towerId).WrapColor(ColorConstantV3.Color_Yellow_Str);
        _view.RankingMark_UISprite.spriteName = string.Format("Ranking_{0}", index);
        var charactor = DataCache.getDtoByCls<GeneralCharactor>(dto.charactorId);
        if (charactor is MainCharactor)
            UIHelper.SetPetIcon(_view.PlayerIcon_UISprite, (charactor as MainCharactor).texture.ToString());
    }

    private void SetNullItem()
    {
        _view.PlayerIcon_UISprite.gameObject.SetActive(false);
        _view.NameLabel_UILabel.text = "";
        _view.SorceLabel_UILabel.text = "";
        _view.RankingMark_UISprite.spriteName = "";
        _view.PlayerGrade_UILabel.text = "";
    }
}
