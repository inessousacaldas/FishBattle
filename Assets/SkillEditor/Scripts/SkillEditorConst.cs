
using System.Collections.Generic;
using AppDto;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static class SkillEditorConst
    {
        public static readonly string UIPrefabPath = "Assets/SkillEditor/Prefab";

        #region 场景配置
        public static readonly List<int> BattleScenes = new List<int>()
        {
            BattleDemoConfigModel.DEFAULT_SCENE_ID,
        };

        public static readonly int BattleSideMaxNum = 10;

        public static readonly int DefaultMemberNum = 5;
        public static readonly float TeamAYRotation = -160f;
        public static readonly float TeamBYRotation = 20f;

        #endregion

        #region 人物配置
        public static readonly int MainCharactorType = (int)GeneralCharactor.CharactorType.MainCharactor;
        public static readonly int MaxHp = 100;
        public static readonly int DefaultHp = 80;
        public static readonly int ChangeMinHp = 10;
        public static readonly int ChangeMaxHp = 30;
        #endregion

        #region 技能配置
        public static readonly string BattleConfigPath =
            "Assets/GameResources/ConfigFiles/BattleConfig/BattleConfig.bytes";
        public static readonly string DefaultTargetNum = "1";
        public static readonly bool DefaultAtOnce = true;
        public static readonly string DefaultMultiPart = "1";
        public static readonly int MoveActionPlayTime = 3000;
        public static readonly int WithoudMoveActionPlayTime = 1000;
        public static readonly int NearClientAttackSkillType = 1;
        public static readonly int FarClientAttackSkillType = 2;
        #endregion

        #region UI
        public enum BackFrames
        {
            backWithFrame0,
            backWithFrame1,
            backWithFrame2,
            backWithFrame3,
            backWithFrame4,
            backWithFrame5,
            backWithFrame6,
        };
        #endregion
    }
}

#endif