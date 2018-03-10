using System;
using System.Collections.Generic;



/// <summary>
/// 窗口类型的，NeverDestroy的，窗口永远缓存，下次打开就缓存里面取出
/// Destroy的，就直接删除
/// Default的，默认30s检查一遍，30s内打开的话，直接缓存里面取出
/// </summary>
public static class UIModulePoolDefinition
{
    public enum ModulePoolType
    {
        NeverDestroy = Int32.MaxValue,
        Destroy = -1,

        Default = 30,
    }


    /// <summary>
    /// 使用Dict来增加速度，牺牲空间
    /// </summary>
    public static readonly Dictionary<string, object> SharePrefabDict = new Dictionary<string, object>()
    {
        //{ProxyShopModule.CommonShopWinPrefab, null },
    };


    private static Dictionary<string, int> _moduleDict = new Dictionary<string, int>()
    {
        //不用缓存形式，是因为新版本控制器（主要是monoviewcontroller有自己的生命周期，关闭时会自动执行dispose等）

        // 不释放类型
//        {ProxyChatModule.NAME, (int)ModulePoolType.NeverDestroy },

        // 固定时间释放类型
//        { ProxyShareModule.MainViewPath, (int)ModulePoolType.Default },
//        { ProxyShareModule.SelectPlatformViewPath, (int)ModulePoolType.Default },
//        {ProxyTradePetModule.TradePetWinPath, (int) ModulePoolType.Default},
//        { ProxyPlayerPropertyModule.DYE_VIEW, (int)ModulePoolType.Default },
//        { ProxyArenaModule.NAME, (int)ModulePoolType.Default },
//        {ProxyShopModule.NAME, (int) ModulePoolType.Default},
                                                                              //        {ProxyShopModule.CommonShopWinPrefab, (int) ModulePoolType.Default},
        { ProxyQRCodeModule.QRCodeDownloadView, (int)ModulePoolType.Default },
        { ProxyQRCodeModule.QRCodeScanView, (int)ModulePoolType.Default },
        { ProxyQRCodeModule.QRCodeEnsureView, (int)ModulePoolType.Default },
        { ProxyQRCodeModule.QRCodePayView, (int)ModulePoolType.Default },
        { ProxyQRCodeModule.QRCodeWaitPayView, (int)ModulePoolType.Default },
    };


    public static int GetModulePoolTime(string moduleName)
    {
        if (_moduleDict.ContainsKey(moduleName))
        {
            return _moduleDict[moduleName];
        }

        return (int)ModulePoolType.Destroy;
    }
}
