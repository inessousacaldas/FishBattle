using SkillEditor;
using UnityEngine;
using System;
using AppDto;

/// <summary>
/// 战斗技能编辑管理器，外部入口文件。外部尽量不要调用 SkillEditor 包内的东西。
/// @MarsZ 2017年04月25日15:43:20
/// </summary>
internal class ProxyBattleSkillEditorPreview
{
    public static void OpenPreview(int pEnemyCount,int pEnemyFormation)
    {
        BattleSkillEditorPreviewManager.Instance.OpenPreview(pEnemyCount,pEnemyFormation);
    }

    public static void ReplayPreview(int pSkillId)
    {
        BattleSkillEditorPreviewManager.Instance.ReplayPreview(pSkillId);
    }

    public static void ClosePreview(bool pCloseBgAlso = true)
    {
        BattleSkillEditorPreviewManager.Instance.ClosePreview(pCloseBgAlso);
    }
}