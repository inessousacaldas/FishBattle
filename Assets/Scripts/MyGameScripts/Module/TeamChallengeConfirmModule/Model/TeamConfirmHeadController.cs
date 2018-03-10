using UnityEngine;
using System.Collections;
using AppDto;
using MyGameScripts.Gameplay.Player;

public class TeamConfirmHeadController:MonolessViewController<TeamConfirmHeadView>
{
    public string GetName() {
        return View.Name_UILabel.text;
    }
    public void Init(TeamMemberDto tTeamMemberDto)
    {
        UIHelper.SetPetIcon(View.Head_UISprite,tTeamMemberDto.playerDressInfo.charactor.texture.ToString());
        View.Name_UILabel.text = tTeamMemberDto.nickname;
        View.OccupationBG_UISprite.spriteName = GlobalAttr.GetMagicIcon(tTeamMemberDto.slotsElementLimit);
        View.OccupationIcon_UISprite.spriteName = "faction_" + tTeamMemberDto.factionId;
        View.LevelLabel_UILabel.text = "Lv." + tTeamMemberDto.grade;
    }

    public void SetStaue() {
        View.LevelLabel_UILabel.color = new Color(255,255,255);
        View.Name_UILabel.color = new Color(255,255,255);
        View.Head_UISprite.isGrey = false;
        View.OccupationBG_UISprite.isGrey = false;
        View.OccupationIcon_UISprite.isGrey = false;
    }

    /// <summary>
    /// 还原数据
    /// </summary>
    public void OnClose() {
        View.Head_UISprite.spriteName = "";
        View.Name_UILabel.text = "";
        View.LevelLabel_UILabel.color = new Color(179,179,179);
        View.Name_UILabel.color = new Color(179,179,179);
        View.Head_UISprite.isGrey = true;
        View.OccupationBG_UISprite.isGrey = true;
        View.OccupationIcon_UISprite.isGrey = true;
    }

}
