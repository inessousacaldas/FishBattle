using UnityEngine;
using System;
using AppDto;

public class ProxyWindowModule
{

    public const string NAME_WindowPrefab = "WindowPrefab";
    public const string SIMPLE_NAME_WindowPrefab = "SimpleWindowPrefab";

    public const string NAME_WindowPrefabForTop = "WindowPrefabTop";
    public const string SIMPLE_NAME_WindowPrefabForTop = "SimpleWindowPrefabTop";

    public const string INPUT_NAME_WINDOWPREFAB = "WindowInputPrefab";
    public const string NAME_WindowOptSavePrefab = "WindowOptSavePrefab";

    public const string QueueWindowPrefabPath = "QueueWindowPrefab";
    public const string MarryWindowPrefabPath = "MarryWindowPrefab";
    public const string DivorceWindowPrefabPath = "DivorceWindowPrefab";
    public const string PhoneConfirmWindowPrefabPath = "PhoneConfirmWindowPrefab";
    public const string TeamSettingWindowPrefabPath = "TeamSettingWinPrefab";

    public const string SpeakViewPrefabPath = "SpeakView";      //一键喊话界面

    public const string WindowOnlyMsgView = "WindowOnlyMsgView";

    #region 带边框确认框
    //    public static void OpenConfirmWindow(string msg,
    //        string title = "",
    //        Action onHandler = null,
    //        Action cancelHandler = null,
    //        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
    //        string okLabelStr = null,
    //        string cancelLabelStr = null,
    //        int closeWinTime = 0 /*秒*/,
    //        bool isCloseCallCancelHandler = true,
    //        bool clearColor=false)
    public static void OpenConfirmWindow(string msg,
        string title = "",
        Action onHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null,
        string cancelLabelStr = null,
        int closeWinTime = 0 /*秒*/,
        bool isCloseCallCancelHandler = true,
        bool clearColor=false)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(NAME_WindowPrefab, UILayerType.Dialogue, true, false);

        if (string.IsNullOrEmpty(title))
        {
            title = "提示";
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        if (string.IsNullOrEmpty(cancelLabelStr))
        {
            cancelLabelStr = "取消";
        }

        if (ui == null)
        {
            BuiltInDialogueViewController.OpenView(msg,
                onHandler, cancelHandler, UIWidget.Pivot.Center, okLabelStr, cancelLabelStr);
            return;
        }

        var controller = ui.GetMissingComponent<WindowPrefabController>();
        controller.OpenConfirmWindow(msg, title, onHandler, cancelHandler, pivot, okLabelStr, cancelLabelStr,
            closeWinTime, isCloseCallCancelHandler,clearColor);
    }

    #endregion

    #region 无边框确认框

    public static void OpenSimpleConfirmWindow(string msg,
        Action onHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null, string cancelLabelStr = null, int closeWinTime = 0 /*秒*/,
        UILayerType layer = UILayerType.Dialogue, bool closeWinTimeForOk = false)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(SIMPLE_NAME_WindowPrefab, layer, true, false);

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        if (string.IsNullOrEmpty(cancelLabelStr))
        {
            cancelLabelStr = "取消";
        }

        var controller = ui.GetMissingComponent<SimpleWindowPrefabController>();
        controller.OpenConfirmWindow(msg, onHandler, cancelHandler, pivot, okLabelStr, cancelLabelStr, closeWinTime, closeWinTimeForOk);
    }

    #endregion

    #region 单个按钮,带边框提示框

    public static void OpenMessageWindow(string msg,
        string title = "",
        Action onHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null,
        UILayerType layer = UILayerType.Dialogue, bool justClose = false)
    {
        string prefabName = NAME_WindowPrefab;
        if (layer == UILayerType.TopDialogue)
        {
            prefabName = NAME_WindowPrefabForTop;
        }

        GameObject ui = UIModuleManager.Instance.OpenFunModule(prefabName, layer, true, false);

        if (string.IsNullOrEmpty(title))
        {
            title = "提示";
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        var controller = ui.GetMissingComponent<WindowPrefabController>();
        controller.OpenMessageWindow(msg, title, onHandler, pivot, okLabelStr, justClose,
            layer == UILayerType.TopDialogue);
    }

    #endregion

    #region 无边框提示框

    public static void OpenSimpleMessageWindow(string msg,
        Action onHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null,
        UILayerType layer = UILayerType.Dialogue)
    {
        string prefabName = SIMPLE_NAME_WindowPrefab;
        if (layer == UILayerType.TopDialogue)
        {
            prefabName = SIMPLE_NAME_WindowPrefabForTop;
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        GameObject ui = UIModuleManager.Instance.OpenFunModule(prefabName, layer, true, false);

        if (ui == null)
        {
            BuiltInDialogueViewController.OpenView(msg,
                onHandler, null, UIWidget.Pivot.Center, okLabelStr);
            return;
        }

        var controller = ui.GetMissingComponent<SimpleWindowPrefabController>();
        controller.OpenMessageWindow(msg, onHandler, pivot, okLabelStr, layer == UILayerType.TopDialogue);
    }

    #region 输入框

    public static void OpenInputWindow(
        int minChar = 0,
        int maxChar = 0,
        string title = "",
        string desContent = "",
        string inputTips = "",
        string inputVlaue = "",
        Action<string> onHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = "确定", string cancelLabelStr = "取消", int closeWinTime = 0, UILayerType layer = UILayerType.Dialogue,
        int type = 0)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(INPUT_NAME_WINDOWPREFAB, layer, true, false);

        if (string.IsNullOrEmpty(title))
        {
            title = "系统";
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        if (string.IsNullOrEmpty(cancelLabelStr))
        {
            cancelLabelStr = "取消";
        }

        var controller = ui.GetMissingComponent<WindowInputPrefabController>();
        controller.OpenInputWindow(minChar, maxChar, title, desContent, inputTips, inputVlaue, onHandler, cancelHandler,
            pivot, okLabelStr, cancelLabelStr, closeWinTime, type);

    }

    #endregion

    #region 带不再提示框的确认框

    public static void OpenOptSaveWindow(string msg,
        string title = "",
        Action<bool> onHandler = null,
        Action<bool> cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null, string cancelLabelStr = null, string toggleStr = null,
        int closeWinTime = 0 /*秒*/, UILayerType layer = UILayerType.Dialogue, bool isCloseCallCancelHandler = true)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(NAME_WindowOptSavePrefab, layer, true, false);

        if (string.IsNullOrEmpty(title))
        {
            title = "提示";
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        if (string.IsNullOrEmpty(cancelLabelStr))
        {
            cancelLabelStr = "取消";
        }

        if (string.IsNullOrEmpty(toggleStr)) {
            toggleStr = "不再提示";
        }

        var controller = ui.GetMissingComponent<WindowOptSavePrefabController>();
        controller.OpenOptSaveWindow(msg, title, onHandler, cancelHandler, pivot, okLabelStr, cancelLabelStr, toggleStr,
            closeWinTime, isCloseCallCancelHandler);
    }

    #endregion


    #region 排队窗口

    public static QueueWindowPrefabController OpenQueueWindow(string serverName, int queuePosition, int waitTime,
        UILayerType layer = UILayerType.Dialogue, System.Action onHandler = null)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(QueueWindowPrefabPath, layer, true, false);
        var controller = ui.GetMissingComponent<QueueWindowPrefabController>();
        controller.Open(serverName, queuePosition, waitTime);
        return controller;
    }

    #endregion

//    #region 求婚窗口
//
//    public static void OpenMarryWindow(MarryDto dto, UILayerType layer = UILayerType.ThreeModule, System.Action onHandler = null)
//    {
//        GameObject ui = UIModuleManager.Instance.OpenFunModule(MarryWindowPrefabPath, layer, true, false);
//        var controller = ui.GetMissingComponent<MarryWindowPrefabController>();
//        controller.Open(dto);
//        //        return controller;
//    }
//
//    #endregion


    /*
    #region 手机号码

    public static void OpenPhoneConfirmWindow(string msg,
        string title = "",
        Action onHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Left,
        string okLabelStr = null, string cancelLabelStr = null, int closeWinTime = 0, //秒
        int layer = UILayerType.Dialogue, bool isCloseCallCancelHandler = true)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(PhoneConfirmWindowPrefabPath, layer, true, false);
        if (string.IsNullOrEmpty(title))
        {
            title = "提示";
        }

        if (string.IsNullOrEmpty(okLabelStr))
        {
            okLabelStr = "确定";
        }

        if (string.IsNullOrEmpty(cancelLabelStr))
        {
            cancelLabelStr = "取消";
        }

        if (ui == null)
        {
            BuiltInDialogueViewController.OpenView(msg,
                onHandler, cancelHandler, UIWidget.Pivot.Center, okLabelStr, cancelLabelStr);
            return;
        }

        var controller = ui.GetMissingComponent<PhoneConfirmWindowPrefabController>();
        controller.OpenConfirmWindow(msg, title, onHandler, cancelHandler, pivot, okLabelStr, cancelLabelStr,
            closeWinTime, isCloseCallCancelHandler);
    }

    #endregion
    */

    public static bool IsOpen ()
    {
        return UIModuleManager.Instance.IsModuleCacheContainsModule (NAME_WindowPrefab) || UIModuleManager.Instance.IsModuleCacheContainsModule (SIMPLE_NAME_WindowPrefab)
            || UIModuleManager.Instance.IsModuleCacheContainsModule(NAME_WindowOptSavePrefab ) || UIModuleManager.Instance.IsModuleCacheContainsModule(MarryWindowPrefabPath);
    }
    #endregion

    #region 组队设置

    public static void OpenTeamSettingWindow()
    {
        string title = "组队设置";
        GameObject ui = UIModuleManager.Instance.OpenFunModule(TeamSettingWindowPrefabPath, UILayerType.SubModule, true, false);
        var controller = ui.GetMissingComponent<TeamSettingWinController>();
        controller.OpenTeamSettingWin(title);
    }

    public static void CloseTeamSettingWindow()
    {
        UIModuleManager.Instance.CloseModule(TeamSettingWindowPrefabPath);
    }

    #endregion

    #region 只有文本描述的提示框
    public static void OpenOnlyMsgView(string msg)
    {
        WindowOnlyMsgViewController.Show(msg);
    }
    public static void CloseWindowOnlyMsgPanel()
    {
        UIModuleManager.Instance.CloseModule(WindowOnlyMsgView);
    }
    #endregion

    public static void Close ()
    {
        UIModuleManager.Instance.CloseModule (NAME_WindowPrefab);
    }

    public static void CloseForTop ()
    {
        UIModuleManager.Instance.CloseModule (NAME_WindowPrefabForTop);
    }

    public static void closeInputWin()
    {
        UIModuleManager.Instance.CloseModule (INPUT_NAME_WINDOWPREFAB);
    }

    public static void closeSimpleWin()
    {
        UIModuleManager.Instance.CloseModule (SIMPLE_NAME_WindowPrefab);
    }

    public static void closeSimpleWinForTop()
    {
        UIModuleManager.Instance.CloseModule (SIMPLE_NAME_WindowPrefabForTop);
    }

    public static void closeOptWin()
    {
        UIModuleManager.Instance.CloseModule(NAME_WindowOptSavePrefab);
    }

    public static void CloseQueueWindow()
    {
        UIModuleManager.Instance.CloseModule(QueueWindowPrefabPath);
    }

    public static void CloseMarryWindow()
    {
        UIModuleManager.Instance.CloseModule(MarryWindowPrefabPath);
    }

    public static void ClosePhoneConfirmWindow()
    {
        UIModuleManager.Instance.CloseModule(PhoneConfirmWindowPrefabPath);
    }
}