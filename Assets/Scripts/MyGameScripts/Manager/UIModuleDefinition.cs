using System.Collections.Generic;


/// <summary>
/// 控制大小窗，符合条件为 DefaultModule 到 FiveModule 之间的
/// 当打开第二个大窗，前面的大窗和小窗被隐藏，当关闭第二个大窗的时候，前面的窗口恢复
/// </summary>
public static class UIModuleDefinition
{
    public enum ModuleType
    {
        None,
        MainModule,
        SubModule,
    }

    /// <summary>
    /// 静态表
    /// </summary>
    private static readonly Dictionary<string, ModuleType> _moduleDict = new Dictionary<string, ModuleType>()
    {
        // 小窗
        {ProxyLoginModule.NAME_TESTSDK,ModuleType.SubModule },
        {ProxyMainUI.PLAYERINFO_VIEW, ModuleType.SubModule },
        {ProxyMainUI.BATTLE_ORDER_EDITOR_VIEW ,ModuleType.SubModule},
        {ProxyMainUI.BATTLE_BUFFTIPS_VIEW,  ModuleType.SubModule},
        {ProxyMainUI.BATTLE_ORDER_LIST_VIEW,  ModuleType.SubModule},
        {ProxyWindowModule.INPUT_NAME_WINDOWPREFAB,ModuleType.SubModule },
    };

    /// <summary>
    /// 如果不在静态表里面的
    /// 根据层次进行判断，一般层次的算大窗，其它属小窗
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public static ModuleType GetUIModuleType(string moduleName, UILayerType layerType)
    {
        if (_moduleDict.ContainsKey(moduleName))
        {
            return _moduleDict[moduleName];
        }
        else
        {
            if (layerType >= UILayerType.DefaultModule 
                && layerType < UILayerType.SubModule)
            {
                return ModuleType.MainModule;
            }
            else
            {
                return ModuleType.SubModule;
            }
        }
    }

    public static bool IsMainModule(string moduleName){
        if (_moduleDict.ContainsKey(moduleName))
        {
            return _moduleDict[moduleName] == ModuleType.MainModule;
        }
        else
        {
            return false;
        }
    }
}
