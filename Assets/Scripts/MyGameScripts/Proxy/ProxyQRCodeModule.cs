using System;
using UnityEngine;
using System.Collections;

public static class ProxyQRCodeModule
{
    #region 客户端下载提示
    public const string QRCodeDownloadView = "QRCodeDownloadView";

    public static void OpenQRCodeDownloadView(UILayerType depth = UILayerType.Dialogue)
    {
        var controller = UIModuleManager.Instance.OpenFunModule<QRCodeDownloadViewController>(QRCodeDownloadView, depth, true);
        controller.SetData();
    }

    public static void CloseQRCodeDownloadView()
    {
        UIModuleManager.Instance.CloseModule(QRCodeDownloadView);
    }
    #endregion


    #region 扫描
    public const string QRCodeScanView = "QRCodeScanView";

    public static void OpenQRCodeScanView(Action closeCallback, UILayerType depth = UILayerType.QRCodeScan)
    {
        var ui = UIModuleManager.Instance.OpenFunModule(QRCodeScanView, depth, false);
        var controller = ui.GetMissingComponent<QRCodeScanViewController>();
        controller.SetData(closeCallback);
    }

    public static void CloseQRCodeScanView()
    {
        UIModuleManager.Instance.CloseModule(QRCodeScanView);
    }
    #endregion

    #region 扫描登陆
    public const string QRCodeEnsureView = "QRCodeEnsureView";

    public static void OpenQRCodeEnsureView(Action closeCallback, string sid, UILayerType depth = UILayerType.QRCodeScan)
    {
        var ui = UIModuleManager.Instance.OpenFunModule(QRCodeEnsureView, depth, false);
        var controller = ui.GetMissingComponent<QRCodeEnsureViewController>();
        controller.SetData(closeCallback, sid);
    }

    public static void CloseQRCodeEnsureView()
    {
        UIModuleManager.Instance.CloseModule(QRCodeEnsureView);
    }
    #endregion


    #region 支付二维码
    public const string QRCodePayView = "QRCodePayView";

    public static void OpenQRCodePayView(OrderItemJsonDto itemDto, int quantity, OrderJsonDto orderDto, UILayerType depth = UILayerType.QRCodeScan)
    {
//        TipManager.AddTip("请到移动端进行充值");
//        return;

        CloseQRCodeWaitPayView();

        var ui = UIModuleManager.Instance.OpenFunModule(QRCodePayView, depth, true);
        var controller = ui.GetMissingComponent<QRCodePayViewController>();
        controller.SetData(itemDto, quantity, orderDto);
    }

    public static void CloseQRCodePayView()
    {
        UIModuleManager.Instance.CloseModule(QRCodePayView);
    }
    #endregion


    #region 等待支付
    public const string QRCodeWaitPayView = "QRCodeWaitPayView";

    public static void OpenQRCodeWaitPayView(UILayerType depth = UILayerType.QRCodeScan)
    {
        CloseQRCodePayView();

        var ui = UIModuleManager.Instance.OpenFunModule(QRCodeWaitPayView, depth, true);
        var controller = ui.GetMissingComponent<QRCodeWaitPayViewController>();
        controller.SetData();
    }

    public static void CloseQRCodeWaitPayView()
    {
        UIModuleManager.Instance.CloseModule(QRCodeWaitPayView);
    }
    #endregion

}
