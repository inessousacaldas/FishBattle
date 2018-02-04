using System;
using AppDto;
using UnityEngine;

public partial interface ITeamBeInviteView
{
//在这里添加自定义接口
    void UpdateView(ITeamData data);

    GameObject TeamInfoGO { get; }
    void SetCancelLabel(int time);
}

public sealed partial class TeamBeInviteView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ITeamData teamData)
    {
        var data = teamData.TeamBeInviteData;
        var tNotify = data.GetCurrentInvitation();// mTeamBeInviteModel.Notify;

        //	做判断如果组队有目标的话

//        bool tShowTarget = tNotify.teamActionTargetId != 0;
            var tShowTarget = false;
//        if (tShowTarget)
//        {
//            TeamActionTarget target = ModelManager.Team.teamActionTargetDic[tNotify.teamActionTargetId];
//            InfoLabel2_UILabel.text = string.Format("目标:{0}", target.name);
//        }

        int tYPos = tShowTarget ? 33 : 13;
//        ProfessionIcon_UISprite.transform.localPosition = new Vector3(ProfessionIcon_UISprite.transform.localPosition.x,
//            tYPos,
//            ProfessionIcon_UISprite.transform.localPosition.z);
//        targetLabel_UILabel.transform.localPosition = new Vector3(targetLabel_UILabel.transform.localPosition.x,
//            tYPos,
//            targetLabel_UILabel.transform.localPosition.z);
//        UIHelper.SetOtherIcon(ProfessionIcon_UISprite, "small_faction_" + leaderInfo.factionId, true);

        var leaderInfo = tNotify.inviteTeamMembers.Find(m => m.id == tNotify.inviterPlayerId);
        string lv = string.Format("({0}级)", leaderInfo.grade.ToString());
        InfoLabel2_UILabel.text = string.Format("{0}{1}邀请你加入他的队伍",
            leaderInfo.nickname.WrapColor(ColorConstantV3.Color_MissionBlue_Str),
            lv.WrapColor(ColorConstantV3.Color_MissionBlue_Str));
    }

    public void SetCancelLabel(int time)
    {
        string cancelLabelStr = "拒绝";

        if (time > 0)
        {
            CancelLabel_UILabel.text = cancelLabelStr + "(" + time + ")";
        }
    }

    public GameObject TeamInfoGO {
        get { return TeamInvitationItem; }
    }
}
