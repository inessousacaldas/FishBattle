using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using UniRx;

public sealed partial class RoleSkillDataMgr
{
    
    public RoleSkillPotentialData PotentialData
    {
        get { return _data.potentialData; }
    }

    public RoleSkillTalentData TalentData
    {
        get { return _data.talentData; }
    }

    public RoleSkillSpecialityData SpecData
    {
        get { return _data.specData; }
    }

    public RoleSkillMainData MainData
    {
        get { return _data.mainData; }
    }

    // 初始化
    private void LateInit()
    {
        //_disposable.Add(NotifyListenerRegister.RegistListener<PotentialDto>(HandlePotentialDtoNotify));
        RoleSkillNetMsg.InitListener();
    }

    public void OnDispose(){
            
    }
}
