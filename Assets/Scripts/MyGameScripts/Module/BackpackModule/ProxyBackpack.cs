using AppDto;
using BackpackViewController = BackpackDataMgr.BackpackViewController;
using TempBackPackViewController = BackpackDataMgr.TempBackPackViewController;
using PropsCompositeViewController = BackpackDataMgr.PropsCompositeViewController;

public class ProxyBackpack
{
    public static void OpenBackpack(BackpackViewTab tab = BackpackViewTab.Backpack)
    {
        BackpackViewController.Open(tab);
    }

    public static void OpenTempBackpack(){
        TempBackPackViewController.Open();
    }

    public static void OpenComposite(CompositeTabType type, BagItemDto item)
    {
        PropsCompositeViewController.Open(type, item);
    }
}