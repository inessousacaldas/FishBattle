using AppDto;
public class ProxyWorldMapModule
{
    public static void CloseAllModule()
    {
        GameDebuger.TODO(@"ProxyDialogueModule.CloseNpcDialogue();");
        CloseWorldMap();
        CloseMiniMap();
        CloseMiniWorldMap();
        GameDebuger.TODO(@"ProxyPromoteModule.Close();");
    }

    #region WorldMapController

    public static void OpenWorldMap()
    {
        GameDebuger.TODO(@"if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_GameMap, true))
        {
            return;
        }");

        UIModuleManager.Instance.OpenFunModule<WorldMapController>(WorldMapView.NAME, UILayerType.ThreeModule, true);
    }

    public static void CloseWorldMap()
    {
        UIModuleManager.Instance.CloseModule(WorldMapView.NAME);
    }

    #endregion

    #region MiniMapController

    public static void OpenMiniMap()
    {
        WorldMapDataMgr.MiniWorldMapViewLogic.Open();
//        var controller = UIModuleManager.Instance.OpenFunModule<MiniMapController>(MiniMapView.NAME, UILayerType.ThreeModule, true);
//        controller.Open();
    }

    public static void CloseMiniMap()
    {
        UIModuleManager.Instance.CloseModule(MiniWorldMapView.NAME);
//        UIModuleManager.Instance.CloseModule(MiniMapView.NAME);
    }

    #endregion

    #region MiniWorldMap

    public static void OpenMiniWorldMap()
    {
        if (!WorldManager.Instance.CanFlyable())
        {
            TipManager.AddTip("此地插翅难飞");
            return;
        }

        if (WorldManager.Instance.GetModel().GetSceneDto() == null)
        {
            TipManager.AddTip("还没有进入任何地图，不能打开此功能");
            return;
        }
        WorldMapDataMgr.MiniWorldMapViewLogic.Open();
    }

    public static void CloseMiniWorldMap()
    {
        UIModuleManager.Instance.CloseModule(MiniWorldMapView.NAME);
    }

    #endregion
}