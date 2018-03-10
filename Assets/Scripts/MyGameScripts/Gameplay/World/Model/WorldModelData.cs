using System.Collections.Generic;
using System.Text;
using AppDto;
using UniRx;

public interface IWorldViewData
{
    IEnumerable<long > LatestPlayerChageTeamStatusSet { get; }
}

public interface IWorldModelData
{
    IWorldViewData WorldViewData { get; }
}

public sealed partial class WorldModel
{

    public sealed partial class WorldModelData:IWorldModelData, IWorldViewData
    {
        public Dictionary<long, ScenePlayerDto> _playersDic = new Dictionary<long, ScenePlayerDto>(10);
        public List<long> latestPlayerChageTeamStatusSet = new List<long>(10);

        public void Dispose()
        {
            _playersDic.Clear();
        }

        public IWorldViewData WorldViewData {
            get { return this; }
        }
        public IEnumerable<long > LatestPlayerChageTeamStatusSet {
            get { return latestPlayerChageTeamStatusSet; }
        }
        
        public void UpdateWithSceneNotify(TeamSceneNotify noti)
        {
            if (noti == null || noti.teamId <= 0) return;

            latestPlayerChageTeamStatusSet.Clear();
            _playersDic.ForEach(o =>
            {
                var idx = noti.inTeamPlayerIds.FindElementIdx(id =>o.Key == id);
                var p = o.Value;
                if (idx >= 0)
                {
                    p.teamStatus = p.id == noti.leaderPlayerId
                        ? (int)TeamMemberDto.TeamMemberStatus.Leader
                        : (int)TeamMemberDto.TeamMemberStatus.Member;
                    p.teamId = noti.teamId;
                    p.teamIndex = idx;
                    latestPlayerChageTeamStatusSet.Add(p.id);
                }
                else if (p.teamId == noti.teamId)
                {
                    p.teamStatus = (int) TeamMemberDto.TeamMemberStatus.Away;
                    p.teamIndex = -1;
                    latestPlayerChageTeamStatusSet.Add(p.id);
                }
            });

            GameEventCenter.SendEvent(GameEvent.SCENE_TEAM_NOTIFY, latestPlayerChageTeamStatusSet);
        }

        public void UpdateWithLeaveTeamNotify(LeaveTeamNotify noti)
        {
            ScenePlayerDto p;
            _playersDic.TryGetValue(noti.playerId, out p);
            //p=null证明玩家已经跑出视野范围之外,那么就不处理
            //会存在一种情况,服务器推送了SceneObjectRemoveNotify删除_playersDic里面的玩家之后
            //马上又推送了LeaveTeamNotify,就会造成p=null
            if (p == null)
                return;
            p.teamStatus = (int) TeamMemberDto.TeamMemberStatus.NoTeam;
            p.teamId = noti.teamId;
            p.teamIndex = -1;
            GameEventCenter.SendEvent(GameEvent.SCENE_TEAM_NOTIFY, new List<long>(1){noti.playerId});
        }

        
    }
}
