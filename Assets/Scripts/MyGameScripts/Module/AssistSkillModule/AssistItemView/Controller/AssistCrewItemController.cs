// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistCrewItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using System.Collections.Generic;

public partial class AssistCrewItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Bg_UIButtonEvt.Subscribe(_ =>
        {
            if(_isIng && !_isChose)
            {
                TipManager.AddTip("伙伴正在执行其他委托任务");
                return;
            }
            _isChose = !_isChose;
            View.ChoseBg_UISprite.enabled = _isChose;
            var data = new CrewChoseClickEvent();
            data.id = _id;
            data.crewId = _crewId;
            data.isSelect = _isChose;
            clickChoseStream.OnNext(data);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private long _id = 0;
    private int _crewId = 0;
    private bool _isChose = false;
    private bool _isIng = false;
    private Dictionary<int, DelegateMission> _delegateMissionDic = DataCache.getDicByCls<DelegateMission>();
    public void UpdateView(CrewShortDto dto, Crew msg, bool isChose, int curMissionId, bool isIng)
    {
        _id = dto.id;
        _crewId = dto.crewId;
        _isChose = isChose;
        _isIng = isIng;
        UIHelper.SetPetIcon(_view.Icon_UISprite, msg.icon);
        View.ChoseBg_UISprite.enabled = isChose;
        View.IngSprite_UISprite.enabled = isIng;
        if (!isIng && _delegateMissionDic.ContainsKey(curMissionId) && msg.delegateTypeIds.Contains(_delegateMissionDic[curMissionId].type))
            View.SignSprite_UISprite.enabled = true;
        else
            View.SignSprite_UISprite.enabled = false;

        View.Rare_UISprite.spriteName = string.Format("rare_{0}", msg.rare);
        View.Rare_UISprite.MakePixelPerfect();
        View.Lv_UILabel.text = dto.grade.ToString();
    }

    public void SetIsChose(bool b)
    {
        View.ChoseBg_UISprite.enabled = b;
    }

    public class CrewChoseClickEvent
    {
        public long id;
        public int crewId;
        public bool isSelect;
    }

    readonly UniRx.Subject<CrewChoseClickEvent> clickChoseStream = new UniRx.Subject<CrewChoseClickEvent>();
    public UniRx.IObservable<CrewChoseClickEvent> OnClickChoseStream
    {
        get { return clickChoseStream; }
    }
}
