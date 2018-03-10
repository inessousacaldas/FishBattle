using System;
using UnityEngine;
using AppDto;

namespace SkillEditor
{
    internal class BattleSkillEditorPreviewManager
    {
        public bool IsInBattle{ get; private set; }

        private static BattleSkillEditorPreviewManager mInstance = new BattleSkillEditorPreviewManager();

        public static BattleSkillEditorPreviewManager Instance{ get { return mInstance; } }

        public void OpenPreview(int pEnemyCount,int pEnemyFormation)
        {
            IsInBattle = true;
            BattleEditorPreviewController.OpenPreview(pEnemyCount,pEnemyFormation);
        }

        public void ReplayPreview(int pSkillId)
        {
            BattleEditorPreviewController.ReplayPreview(pSkillId);
        }

        public void ClosePreview(bool pCloseBgAlso = true)
        {
            if (!IsInBattle)
                return;
            BattleEditorPreviewController.ClosePreview(pCloseBgAlso);
            mBattleEditorPreviewController = null;
            IsInBattle = false;
        }

        #region 技能战斗演示

        private IBattleEditorPreviewController mBattleEditorPreviewController = null;

        private IBattleEditorPreviewController BattleEditorPreviewController
        {
            get
            {
                if (null == mBattleEditorPreviewController)
                {
                    GameObject tViewRoot = new GameObject("BattleEditorPreviewControllerRoot");
                    mBattleEditorPreviewController = tViewRoot.AddComponent<BattleEditorPreviewController>();
                }
                return mBattleEditorPreviewController;
            }
        }

        #endregion
    }
}

