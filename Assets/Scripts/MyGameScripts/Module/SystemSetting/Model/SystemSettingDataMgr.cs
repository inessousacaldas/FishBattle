using UniRx;

public sealed partial class SystemSettingDataMgr
{
    public static SystemSettingDataMgr Instance {
        get
        {
            if (stream == null)
		        stream = new Subject<ISystemSettingData>();

            if (_ins == null){
                _ins = new SystemSettingDataMgr();
                _ins.Init();
            }
            
		    stream.Hold(_ins._data);
            return _ins;
        }
    }

    private static SystemSettingDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private SystemSettingData _data = null;
    public static IObservableExpand<ISystemSettingData> Stream{
        get{
            if (stream == null)
                stream = new Subject<ISystemSettingData>();
            return stream;
        }
    }

    private static Subject<ISystemSettingData> stream = null;

    private static void FireData()
    {
        stream.OnNext(Instance._data);
    }

    private SystemSettingDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new SystemSettingData();
        _data.InitData();
    }

    public void Dispose(){
        _data.Dispose();
	    _data = null;
	    stream = stream.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }

    public bool ReceiveTeam
    {
        get
        {
            if (PlayerPrefsExt.HasPlayerKey("ReceiveTeam"))
            {
                return PlayerPrefsExt.GetPlayerInt("ReceiveTeam") == 1;
            }
            PlayerPrefsExt.SetPlayerInt("ReceiveTeam", 1);
            return true;
        }
        set { PlayerPrefsExt.SetPlayerInt("ReceiveTeam", value ? 1 : 0); }
    }

    public bool AutoPlayTeamVoice
    {
        get { return _data.teamToggle; }
        set
        {
            _data.teamToggle = value;
            PlayerPrefsExt.SetBool(SystemSettingData.key_systemContingentToggle, value);
        }
    }
    private bool _isIdle;

    public bool IsIdle
    {
        get { return _isIdle; }
        set { _isIdle = value; }
    }

}
