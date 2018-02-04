using AppDto;
using UniRx;

public sealed partial class RoleSkillBattleData: IRoleSkillBattleData
{

    public Skill.SkillEnum curTab;
    private IRoleSkillMainData roleSkillData;
    private ICrewInfoData crewData;

    private static RoleSkillBattleData _ins = null;
    private static Subject<int> stream = null;
    private int id;
    private CompositeDisposable _disposable;

    public RoleSkillBattleData BattleDataMgr
    {
        get
        {
            InitDataAndStream();
            return _ins;
        }
    }

    public IObservableExpand<int> Stream
    {
        get
        {
            InitDataAndStream();
            return stream;
        }
    }

    private void InitDataAndStream()
    {
        if(stream ==null)
        {
            stream = new Subject<int>();
        }
        if (_ins == null)
        {
            _ins = new RoleSkillBattleData();
            _ins.Init();
        }
    }

    public void Init()
    {
        _disposable = new CompositeDisposable();
        stream.Hold(_ins.id);
    }

    public void FireData()
    {
        if(stream!=null)
            stream.OnNext(id);
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public IRoleSkillMainData RoleSkillData
    {
        get { return roleSkillData; }
    }
    public ICrewInfoData CrewData
    {
        get { return crewData; }
    }

    public void UpdateMainData(IRoleSkillMainData data)
    {
        roleSkillData = data;
    }

}
