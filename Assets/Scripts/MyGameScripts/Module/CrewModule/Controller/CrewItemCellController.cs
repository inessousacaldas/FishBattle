// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using MyGameScripts.Gameplay.Player;
using System;
using UniRx;
using UnityEngine;

public partial interface ICrewItemCellController
{
    void UpdateDataAndView(ICrewBookData bookData, int idx, bool isShwoName);

    void IsSelect(bool b);
    bool SelectState { get; }

    long GetUid { get; }

    int GetCrewId { get; }
}

public partial class CrewItemCellController
{

    private CompositeDisposable _disposable;

    private Subject<ICrewItemData> _clickEvt = new Subject<ICrewItemData>();
    public UniRx.IObservable<ICrewItemData> GetClickEvt { get { return _clickEvt; } }

    private OrbmentInfoDto _orbmentDto;
    private ICrewItemData _itemData;
    private CrewShortDto _crewShortDto;

    private long _uid;
    public long GetUid { get { return _uid; } }

    private int _idx;
    private int _crewId;
    public int GetCrewId { get { return _crewId; } }

    private bool _isSelect = false;
    public bool SelectState { get { return _isSelect; } }


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(_view.CrewItemCell_UIButton.onClick, OnClickHandler);
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }


    public void UpdateDataAndView(ICrewBookData bookData, int idx, bool isShwoName)
    {
        _idx = idx;
        Crew crew = bookData.GetCrew;
        if (crew == null)
        {
            TipManager.AddTip("========伙伴数据导表有误========");
            return;
        }

        _crewId = crew.id;
        _itemData = CrewItemData.Create(_idx, _uid, _crewId);
        UIHelper.SetPetIcon(_view.CrewIcon_UISprite, crew.icon);

        _view.Lv.SetActive(bookData.GetInfoDto != null);


        if (bookData.GetInfoDto != null) //已拥有
        {
            _uid = bookData.GetInfoDto.id;
            _view.LevelLabel_UILabel.text = string.Format("Lv.{0}", bookData.GetInfoDto.grade);
            SetCrewFaction(bookData.GetInfoDto.slotsElementLimit, bookData.GetCrew.typeIcon);
        }
        else
        {
            var orbment = DataCache.getDtoByCls<Orbment>(bookData.GetCrew.orbmentId);
            SetCrewFaction(orbment.quartzProperty.elementId, bookData.GetCrew.typeIcon);
        }

        SetCrewType(bookData.GetStatue);
        SetRare(bookData.GetCrew.rare);
    }


    private void SetCrewType(CrewStatueType type)
    {
        switch (type)
        {
            case CrewStatueType.Main:
                _view.Type_UISprite.spriteName = "flag_general";
                break;
            case CrewStatueType.Auxiliary:
                _view.Type_UISprite.spriteName = "flag_auxiliary";
                break;
            case CrewStatueType.None:
                _view.Type_UISprite.spriteName = "";
                break;
            default:
                _view.Type_UISprite.spriteName = "";
                break;
        }
    }

    private void SetRare(int rare)
    {
        _view.Rare_UISprite.spriteName = "rare_" + rare;
        _view.Rare_UISprite.MakePixelPerfect();
    }

    private void SetCrewFaction(int slots, string faction)
    {
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(slots);
        _view.FactionSprite_UISprite.spriteName = faction;
    }

    public void IsSelect(bool b)
    {
        _isSelect = b;
        _view.bg_select.gameObject.SetActive(b);
    }

    private void OnClickHandler()
    {
        _clickEvt.OnNext(_itemData); 
    }

}
