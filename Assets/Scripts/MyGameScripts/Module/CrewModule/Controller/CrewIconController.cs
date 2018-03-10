using System;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
using UniRx;

public class CrewIconController : MonolessViewController<CrewIcon>
{
    private IDisposable _disposable;
    
    private Subject<ICrewItemData> _clickEvt = new Subject<ICrewItemData>();
    public UniRx.IObservable<ICrewItemData> GetClickEvt {get { return _clickEvt; }}

    private Subject<OrbmentInfoDto> _orbmentClickEvt = new Subject<OrbmentInfoDto>();
    public UniRx.IObservable<OrbmentInfoDto> GetOrbmentEvt { get { return _orbmentClickEvt; } }  

    private OrbmentInfoDto _orbmentDto;
    private ICrewItemData _itemData;
    private CrewShortDto _crewShortDto;

    private long _uid;
    private int _idx;
    private int _crewId;

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
        UIHelper.SetPetIcon(_view.Icon_UISprite, crew.icon);
        _view.BackGround_UISprite.spriteName = string.Format("crew_ib_" + crew.quality);
        _view.BackGround_UIButton.normalSprite = string.Format("crew_ib_" + crew.quality);

        _view.Lv_UILabel.gameObject.SetActive(bookData.GetInfoDto != null);
        _view.Name_UILabel.gameObject.SetActive(isShwoName);

        if (bookData.GetInfoDto != null) //已拥有
        {
            _uid = bookData.GetInfoDto.id;
            _view.Lv_UILabel.text = bookData.GetInfoDto.grade.ToString();
        }

        if (isShwoName)
            _view.Name_UILabel.text = crew.name;

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
        _view.Rare_UISprite.spriteName = "rare_" +  rare;
        _view.Rare_UISprite.MakePixelPerfect();
    }

    public void UpdateDataAndView(OrbmentInfoDto orbmentDto)
    {
        _orbmentDto = orbmentDto;
        _view.Lv_UILabel.gameObject.SetActive(true);
        _view.Name_UILabel.gameObject.SetActive(false);
        _view.BackGround_UISprite.spriteName = string.Format("crew_ib_{0}", 3);
        _view.Lv_UILabel.text = orbmentDto.grade.ToString();

        var crew = DataCache.getDtoByCls<GeneralCharactor>(orbmentDto.crewId) as Crew;
        _view.Rare_UISprite.gameObject.SetActive(crew != null);
        if (crew != null)
        {
            _view.BackGround_UISprite.spriteName = string.Format("crew_ib_" + crew.quality);
            _view.BackGround_UIButton.normalSprite = string.Format("crew_ib_" + crew.quality);
            UIHelper.SetPetIcon(_view.Icon_UISprite, crew.icon);
            SetCrewType((CrewStatueType)orbmentDto.battleCrewType);
            SetRare(crew.rare);
        }
        else
        {
            var playerDto = ModelManager.Player.GetPlayer().charactor as MainCharactor;
            UIHelper.SetPetIcon(_view.Icon_UISprite, playerDto == null ? "" : string.Format("head_{0}", playerDto.texture));
            _view.BackGround_UIButton.normalSprite = "crew_ib_3";
            _view.Type_UISprite.spriteName = "";
        }
    }

    public void UpdateView(CrewShortDto dto, Crew crewInfo)
    {
        _crewShortDto = dto;
        UIHelper.SetPetIcon(_view.Icon_UISprite, crewInfo.icon);
        _view.BackGround_UISprite.spriteName = string.Format("crew_ib_" + crewInfo.quality);
        _view.BackGround_UIButton.normalSprite = string.Format("crew_ib_" + crewInfo.quality);
        _view.Name_UILabel.text = crewInfo.name;
        _view.Lv_UILabel.gameObject.SetActive(true);
        _view.Lv_UILabel.text = dto.grade.ToString();

        RegistDelegateClickEvent();
    }

    private void RegistDelegateClickEvent()
    {
        EventDelegate.Add(_view.BackGround_UIButton.onClick, OnDeleGateClickHandler);
    }

    private void OnDeleGateClickHandler()
    {
        if (_crewShortDto == null)
            return;
    }


    protected override void RegistCustomEvent()
    {
        if(_view.BackGround_UIButton!=null)
        {
            EventDelegate.Add(_view.BackGround_UIButton.onClick,OnClickHandler);
        }
    }

    protected override void OnDispose()
    {
        _disposable = null;
    }

    public void OnClickHandler()
    {
        if (_orbmentDto != null)
            _orbmentClickEvt.OnNext(_orbmentDto);
        else
        {
            _clickEvt.OnNext(_itemData);
        }
    }

    public void IsSelect(bool b)
    {
        _view.Select_UISprite.gameObject.SetActive(b);
    }

    public int GetCrewId { get { return _crewId; } }

    public long GetUid { get { return _uid; } }

    public int GetIdx { get { return _idx; } }

    public ICrewItemData GetItemData { get { return _itemData; } }

    public void IsShowLv(bool b)
    {
        _view.Lv_UILabel.gameObject.SetActive(b);
    }
}
