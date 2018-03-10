using System;
using UnityEngine;
using AppDto;

namespace SkillEditor
{
    internal class BattleSkillEditorManager
    {
        private static BattleSkillEditorManager mInstance = new BattleSkillEditorManager();

        public static BattleSkillEditorManager Instance{ get { return mInstance; } }

        public void Open()
        {
            BattleSkillEditorController.Show();
        }

        public void Close()
        {
            BattleSkillEditorController.Close();
            mBattleSkillEditorController = null;
        }

        #region 技能编辑

        private BattleSkillEditorController mBattleSkillEditorController = null;

        private BattleSkillEditorController BattleSkillEditorController
        {
            get
            {
                if (null == mBattleSkillEditorController)
                {
                    GameObject tViewRoot = new GameObject("BattleSkillEditorRoot");
                    //此处用当前项目具体业务的编辑器控制器类
                    mBattleSkillEditorController = tViewRoot.AddComponent<SBBattleSkillEditorController>();
                }
                return mBattleSkillEditorController;
            }
        }

        #endregion
    }
}

