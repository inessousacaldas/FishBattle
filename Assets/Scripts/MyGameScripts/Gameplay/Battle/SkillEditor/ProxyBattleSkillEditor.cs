using SkillEditor;
using UnityEngine;
using System;
using AppDto;

/// <summary>
/// 战斗技能编辑管理器，外部入口文件。外部尽量不要调用 SkillEditor 包内的东西。
/// @MarsZ 2017年04月25日15:43:20
/// </summary>
internal class ProxyBattleSkillEditor
{
    #region 一些回调

    //技能预览 初始化，如 SkillEditorHelper.SimulateEditorPreview(5364);
    //技能预览 打开，用于数据准备就绪，可以打开战斗界面时，如 BattleDataManager.OpenBattleView
    public static Action<int,int> PreviewOpenHandler;
    //技能预览 重播，播放指定技能配置（客户端模拟回合数据自行播放）
    public static Action<int> PreviewReplayHandler;
    //技能预览 关闭。
    public static Action<bool> PreviewCloseHandler;

    #endregion

    public static void Open(Action<int,int> pPreviewOpenHandler = null,Action<int> pPreviewReplayHandler = null,Action<bool> pPreviewCloseHandler = null)
    {
        BattleSkillEditorManager.Instance.Open();
		SkillEditor.CameraManager.Instance.ShowBattleUI ();

        PreviewOpenHandler = pPreviewOpenHandler;
        PreviewReplayHandler = pPreviewReplayHandler;
        PreviewCloseHandler = pPreviewCloseHandler;
    }

    public static void Close()
    {
        BattleSkillEditorManager.Instance.Close();
		SkillEditor.CameraManager.Instance.ShowGameUI ();

        ClosePreview();
    }

    public static void UpdateSkillConfigInfo(SkillConfigInfo pSkillConfigInfo)
    {
        BattleConfigManager.Instance.UpdateSkillConfigInfo(pSkillConfigInfo);
    }

    public static void OpenPreview(int pEnemyCount,int pEnemyFormation)
    {
        if (null != PreviewOpenHandler)
        {
            GamePlayer.CameraManager.Instance.BattleCameraController.SetPreViewCamera();
            PreviewOpenHandler(pEnemyCount,pEnemyFormation);
        }
    }

    public static void ReplayPreview(int pSkillId)
    {
        if (null != PreviewReplayHandler)
            PreviewReplayHandler(pSkillId);
    }

    private static void ClosePreview(bool pCloseBgAlso = true)
    {
        //            BattleEditorPreviewController.Close();
        GamePlayer.CameraManager.Instance.BattleCameraController.ResetCamera();
        if (null != PreviewCloseHandler)
            PreviewCloseHandler(pCloseBgAlso);
    }
}