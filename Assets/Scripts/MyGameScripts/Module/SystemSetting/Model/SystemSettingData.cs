public interface ISystemSettingData
{

}

public sealed partial class SystemSettingDataMgr
{
    public sealed partial class SystemSettingData:ISystemSettingData
    {
        public const string key_systemFactionToggle = "_SystemFactionToggle";
        public const string key_systemWorldToggle = "_SystemWorldToggle";
        public const string key_systemContingentToggle = "_SystemContingentToggle";
        public const string key_systemFriendsToggle = "_SystemFriendsToggle";
        public const string key_systemStrangerToggle = "_SystemStrangerToggle";

        public SystemSettingData()
        {

        }

        //自动播放语音开关
        public bool guildToggle = true;
        public bool worldToggle = true;
        public bool teamToggle = true;
        public bool friendsToggle = true;
        public bool strangerToggle = false;
        public bool voiceToggle = true;

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
    }
}
