// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleCrewItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;

public partial interface IBattleCrewItemController
{
    UniRx.IObservable<Unit> OnClickHandler { get; }
}

public interface IBattleCrewData
{
    CrewInfoDto GetCrewInfoDto { get; }
    BattleCrewData.CrewState GetState { get; }
}

public class BattleCrewData: IBattleCrewData
{
    //伙伴状态
    public enum CrewState
    {
        None = 0,
        Fight = 1,
        Die = 2,
    }
    private CrewInfoDto _dto;
    private CrewState _state;

    public CrewInfoDto GetCrewInfoDto { get { return _dto; } }
    public CrewState GetState { get { return _state; } }

    public static IBattleCrewData Create(CrewInfoDto dto, CrewState state)
    {
        var data = new BattleCrewData();
        data._dto = dto;
        data._state = state;
        return data;
    }
}

public partial class BattleCrewItemController
{
    private Subject<Unit> _onclickEvt;
    public UniRx.IObservable<Unit> OnClickHandler { get { return _onclickEvt; } } 
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _onclickEvt = new Subject<Unit>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.ItemBtn_UIButton.onClick, () => { _onclickEvt.OnNext(new Unit());});
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetBattleCrewInfo(IBattleCrewData dto)
    {
        if (dto == null || dto.GetCrewInfoDto == null)
        {
            GameDebuger.Log("IBattleCrewData数据有问题,请检查");
            return;
        }

        var crewInfoDto = dto.GetCrewInfoDto;
        var crew = DataCache.getDtoByCls<GeneralCharactor>(crewInfoDto.crewId) as Crew;
        if (crew == null)
        {
            GameDebuger.LogError(string.Format("Crew表找不到{0}", crewInfoDto.crewId));
            return;
        }

        var active = dto.GetState == BattleCrewData.CrewState.None;
        _view.AttriSpr_UISprite.spriteName = GlobalAttr.GetMagicIcon(crewInfoDto.slotsElementLimit);
        _view.QuartzSpr_UISprite.spriteName = crew.typeIcon;
        _view.NameLb_UILabel.text = crew.name;
        _view.PowerLb_UILabel.text = string.Format("战力{0}", ((int)crewInfoDto.power));
        _view.LvLb_UILabel.text = crewInfoDto.grade.ToString();
        UIHelper.SetPetIcon(_view.Icon_UISprite, crew.icon);
        _view.Icon_UISprite.isGrey = active;
        _view.IconBG_UISprite.isGrey = active;
        _view.StateSprite_UISprite.alpha = active ? 1f : 0f;
    }

    public void IsSelect(bool b)
    {
        _view.SelectSpr_UISprite.gameObject.SetActive(b);
    }

}
