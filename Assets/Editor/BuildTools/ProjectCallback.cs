using System.Collections.Generic;

public static class ProjectCallback
{
    #region PlayerSettingTool 保存后的回调
    public delegate void AfterSave();
    private static Dictionary<int, AfterSave> _afterPlayerSettingToolSave = new Dictionary<int, AfterSave>();

    public enum AfterSaveOrder
    {
        ChannelSetting,
    }

    public static void RegisterAfterPlayerSettingToolSave(AfterSaveOrder order, AfterSave callback)
    {
        _afterPlayerSettingToolSave[(int)order] = callback;
    }

    public static void TriggerAfterPlayerSettingToolSave()
    {
        foreach (var key in _afterPlayerSettingToolSave.Keys.ToList())
        {
            _afterPlayerSettingToolSave[key]();
        }
    }
    #endregion
}
