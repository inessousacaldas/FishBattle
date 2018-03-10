using System;
using UnityEngine;
using AppDto;
using System.Collections.Generic;
using MonsterManager = BattleDataManager.MonsterManager;
using Fish;

namespace SkillEditor
{
    /// <summary>
    /// 技能战斗编辑器，技能演示
    /// @MarsZ 2017年04月26日15:23:53
    /// </summary>
    internal class BattleEditorPreviewController : MonoBehaviour ,IBattleEditorPreviewController
    {
        #region property and field

        public Video PreviewVideo { get; private set; }
        private VideoRoundInterpreter _interpreter;
        #endregion

        #region implement functions


        #endregion

        #region interface

        public void Awake()
        {
            _interpreter = new VideoRoundInterpreter();
        }
        public void OpenPreview(int pEnemyCount,int pEnemyFormation)
        {
            MonsterManager.Instance.ResetData();
            InitBattle(BattleEditorPreviewSimulater.SimulateVideo(0,pEnemyCount,pEnemyFormation));
        }

        public void ReplayPreview(int pSkillId)
        {
            MonsterManager.Instance.ResetMonsterStatus();
            var tSkill = DataCache.getDtoByCls<Skill>(pSkillId);
            UnityEngine.Assertions.Assert.IsNotNull(tSkill, string.Format("技能（id={0}）不在表中！", pSkillId));
            var tVideoRound = BattleEditorPreviewSimulater.SimulateVideoRound(PreviewVideo.ateam.teamSoldiers, PreviewVideo.bteam.teamSoldiers, BattleEditorPreviewSimulater.PlayerDto.id, tSkill);
            //TODO fish: test new battle system
            var play = _interpreter.InterpreteVideoRound(tVideoRound);
            play.AutoDispose();
            try{
                play.Play();
            }
            catch(Exception e){
                GameDebuger.LogError(e);
            }
        }

        public void ClosePreview(bool pCloseBgAlso = true)
        {
            DisposeBattle(pCloseBgAlso);
            Destroy(gameObject);
        }

        #endregion

        #region 战斗管理

        private void InitBattle(Video video)
        {
            UnityEngine.Assertions.Assert.IsNotNull(video, "战斗数据不得为空！");
            PreviewVideo = video;
            InitlializeBattleInfo();
            StartGameVideo();
        }

        //GO销毁
        //        private void OnDestroy()
        //        {
        //            DisposeBattle();
        //        }

        private void DisposeBattle(bool pCloseBgAlso = true)
        {
            BattleStatusEffectManager.Instance.Dispose();
            MonsterManager.Instance.ResetData();
            BattleActionPlayerPoolManager.Instance.Dispose();
        }

        private void InitlializeBattleInfo()
        {
            BattleActionPlayerPoolManager.Instance.Setup(gameObject);
        }

        public void StartGameVideo()
        {
            ShowTeamMonsters(BattlePosition.MonsterSide.Enemy);
            ShowTeamMonsters(BattlePosition.MonsterSide.Player);
        }

        private const float PLAYER_Y_Rotation = 40f;
        private const float ENEMY_Y_Rotation = -80f;

        private void ShowTeamMonsters(BattlePosition.MonsterSide side)
        {
            VideoTeam videoTeam = null;

            if (side == BattlePosition.MonsterSide.Player)
            {
                videoTeam = PreviewVideo.ateam;
            }
            else
            {
                videoTeam = PreviewVideo.bteam;
            }

            CreateMonsters(videoTeam.teamSoldiers, side);
        }

        public bool CreateMonsters(List<VideoSoldier> strikers, BattlePosition.MonsterSide side)
        {
            if (strikers == null || strikers.Count <= 0)
                return false;

            var yRotation = side == BattlePosition.MonsterSide.Player ? 160f : -50f;

            int mcIndex = 1;
            for (int i = 0, len = strikers.Count; i < len; i++)
            {
                VideoSoldier soldier = strikers[i];
                MonsterController mc = CreateMonster(soldier, yRotation,
                                           BattlePositionCalculator.GetMonsterPosition(soldier, side), side);

                mc.gameObject.name = mc.gameObject.name + "_" + mcIndex;

                mcIndex++;
            }

            return true;
        }

        private MonsterController CreateMonster(VideoSoldier monsterData, float yRotation, Vector3 position,
            BattlePosition.MonsterSide side)
        {
            var go = new GameObject();
            SBMonsterController mc = go.AddComponent<SBMonsterController>();

            GameObjectExt.AddPoolChild(LayerManager.Root.BattleActors, go);

            mc.transform.localEulerAngles = new Vector3(0, yRotation, 0);
            mc.transform.localScale = Vector3.one;
            mc.transform.localPosition = position;

            mc.InitMonster(monsterData, side);

            MonsterManager.Instance.AddMonsterController(mc);
            return mc;
        }

        #endregion
    }

}

