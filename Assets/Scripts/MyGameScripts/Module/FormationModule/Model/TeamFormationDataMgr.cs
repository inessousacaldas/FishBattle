using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

namespace StaticInit
{
    using StaticDispose;
    public partial class StaticInit
    {
        private StaticDelegateRunner initTeamFormationDataMgr = new StaticDelegateRunner(
            () => { var mgr = TeamDataMgr.DataMgr; });
    }
}

public sealed partial class TeamFormationDataMgr
{
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamDto>(HandleTeamDtoNotify));
    }

    private void OnDispose()
    {

    }

    private void HandleTeamDtoNotify(TeamDto notify)
    {
        if (notify == null)
            return;

        _data.teamDto = notify;
        _data.SetCurFormationID(notify.formation.formationId);
        FireData();
    }

    public IEnumerable<Formation> GetAllFormationInfo()
    {
        return _data.GetAllFormationInfo();
    }

    public Formation GetUpGradeFormation()
    {
        return _data.CurUpFradeFormation;
    }

    public int GetFormationLevel(int id)
    {
        return _data.GetFormationLevel(id);
    }

    public CrewInfoDto GetMainCrew
    {
        get
        {
            var crewInfo = _data.GetSelfCrew().Find(d => d.id == _data.GetMainCrewId);
            return crewInfo;
        }
    }

    public void SetFormationId(int id)
    {
        _data.SetCurFormationID(id);
    }

    public void SetUseCaseId(int id)
    {
        _data.SetUseCaseId(id);
    }
}
