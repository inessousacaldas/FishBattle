// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 8/29/2017 4:01:34 PM
// **********************************************************************

public static class ProxyEquipmentMain
{
    public static void Open(EquipmentViewTab tab = EquipmentViewTab.Smith)
    {
        EquipmentMainDataMgr.EquipmentMainViewLogic.Open(tab);
    }

    /// <summary>
    /// 打开特效界面
    /// </summary>
    public static void OpenSpecialEffect()
    {
        EquipmentEffectsDataMgr.EquipmentEffectsLogic.Open();
    }
    /// <summary>
    /// 打开纹章
    /// </summary>
    public static void OpenEngraveView()
    {
        EngraveDataMgr.EngraveViewLogic.Open();
    }

    public static void CloseEngraveView()
    {
        UIModuleManager.Instance.CloseModule(EngraveView.NAME);
    }

    public static void CloseMainView()
    {
        UIModuleManager.Instance.CloseModule(EquipmentMainView.NAME);
    }
}

