using UnityEngine;
using System.Collections;

public class ProxyBuiltInWebModule  {

    public static void Open(string url)
    {
#if UNITY_IPHONE || UNITY_ANDROID
        GameObject ui = UIModuleManager.Instance.OpenFunModule(BuiltInWebView.NAME, UILayerType.TopLockScreen, true);
        var controller = ui.GetMissingComponent<BuiltInWebViewController>();
        controller.Open(url);
#else
        Application.OpenURL(url);
#endif
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(BuiltInWebView.NAME);
    }
}
