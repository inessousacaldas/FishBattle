using System;
using AppDto;
using BattleInstController = BattleDataManager.BattleInstController;

namespace MyGameScripts.Gameplay.Battle.Demo.Helper
{
    public static class BattleDtoExt
    {

        public static Comparison<VideoRound> RoundAssendSorter = (VideoRound x, VideoRound y) => x.round - y.round; 

        public static bool IsBattleValid(this VideoRound tVideoRound)
        {
            return null != tVideoRound && tVideoRound.battleId != BattleInstController.SYMBOL_INVALID_VIDEO_ROUND;
        }
        
        public static BattlePlayerInfoDto GetMainRole_BattlePlayerInfoDto(this Video video)
        {
            return video == null
                   ? null
                : video.playerInfos.Find<BattlePlayerInfoDto>(player =>
                player.playerId == ModelManager.Player.GetPlayerId());
        }

        #region VideoSoldier

        public static string PlayerHeadTex(this VideoSoldier soldier)
        {
            if (soldier == null || soldier.charactor == null)
            {
                GameDebuger.Log("soldier为空,请检查");
                return "";
            }

            if (soldier.charactor is MainCharactor) //代表主角
                return string.Format("head_{0}", (soldier.charactor as MainCharactor).texture);
            if (soldier.charactor is Crew) //代表伙伴
                return (soldier.charactor as Crew).icon;

            return "";
        }
        
        public static int GetNormallAtkSkillID(this VideoSoldier soldier)
        {
            if (soldier == null) return 0;

            // 玩家和宠物判断
            switch ((GeneralCharactor.CharactorType)soldier.charactorType)
            {
                case GeneralCharactor.CharactorType.MainCharactor:
                    return soldier.faction == null ? 0 : soldier.faction.simpleSkillId;
                case GeneralCharactor.CharactorType.Crew:
                    var crew = DataCache.getDtoByCls<GeneralCharactor>(soldier.charactorId) as Crew;
                    return crew == null ? 0 : crew.simpleSkillId;
                default:return 0;
            }
        }

        public static Skill GetNormallAtkSkill(this VideoSoldier soldier)
        {
            var skillID = GetNormallAtkSkillID(soldier);
            return DataCache.getDtoByCls<Skill>(skillID);
        }

        public static bool CheckCPEnoughForSCraft(this VideoSoldier soldier)
        {
            var sid = soldier.defaultSCraftsId;
            if (sid > 0)
            {
                var skill = DataCache.getDtoByCls<Skill>(sid);
                return (skill != null && (skill as Crafts).consume <= soldier.cp);
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}