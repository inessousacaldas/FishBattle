// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistDelegateMissionBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UniRx;

public partial class AssistDelegateMissionBtnController
{
    private List<UISprite> _startsList = new List<UISprite>();
    private int _id = 0;
    private Dictionary<int, DelegateMissionDuration> _delegateMissionDuration = DataCache.getDicByCls<DelegateMissionDuration>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _startsList.Add(View.LvStar_1_UISprite);
        _startsList.Add(View.LvStar_2_UISprite);
        _startsList.Add(View.LvStar_3_UISprite);
        _startsList.Add(View.LvStar_4_UISprite);
        _startsList.Add(View.LvStar_5_UISprite);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnAssistDelegateMissionBtn_UIButtonClick.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(DelegateMissionDto dto, DelegateMission mission, bool isChose)
    {
        if (dto == null || mission == null) return;

        _id = dto.id;
        View.Label_UILabel.text = mission.name;
        View.Icon_UISprite.spriteName = mission.icon;
        var hourTime = dto.needTime / 1000 / 60 / 60;
        var starsCount = 0;
        foreach (KeyValuePair<int,DelegateMissionDuration> itemData in _delegateMissionDuration)
        {
            if (itemData.Value.hrs == hourTime)
            {
                starsCount = itemData.Value.id;
                break;
            }
        }

        _startsList.ForEachI((itemStar,index) =>
        {
            if (index < starsCount)
                itemStar.enabled = true;
            else
                itemStar.enabled = false;
        });

        if (dto.finishTime <= 0)  //未开始
        {
            View.IngBg_UISprite.gameObject.SetActive(false);
            View.CompleteBg_UISprite.gameObject.SetActive(false);
            View.Sprite_UISprite.gameObject.SetActive(false);
        }
        else if (dto.finishTime < SystemTimeManager.Instance.GetUTCTimeStamp())  //已完成可领取奖励
        {
            View.IngBg_UISprite.gameObject.SetActive(false);
            View.CompleteBg_UISprite.gameObject.SetActive(true);
            View.Sprite_UISprite.gameObject.SetActive(true);
            View.Sprite_UISprite.spriteName = "person_2";
        }
        else  //进行中
        {
            View.IngBg_UISprite.gameObject.SetActive(true);
            View.CompleteBg_UISprite.gameObject.SetActive(false);
            View.Sprite_UISprite.gameObject.SetActive(true);
            View.Sprite_UISprite.spriteName = "person_1";
        }

        if (isChose)
            View.ChoseBg_UISprite.enabled = true;
        else
            View.ChoseBg_UISprite.enabled = false;
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
