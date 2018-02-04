using System;
using System.Collections.Generic;
using AppDto;

public static class TeamDataHelper
{
    private static Dictionary<TeamMemberDto.TeamMemberStatus, string> StatusFlag =
        new Dictionary<TeamMemberDto.TeamMemberStatus, string>(){
        {TeamMemberDto.TeamMemberStatus.Leader, "teamPos_leader"}
        , {TeamMemberDto.TeamMemberStatus.Away, "flag_away"}
        , {TeamMemberDto.TeamMemberStatus.Offline, "flag_offline"}
        , {TeamMemberDto.TeamMemberStatus.Member, "teamPos_member"}
        , {TeamMemberDto.TeamMemberStatus.NoTeam, ""}
    };

    public static string GetFlagByStatus(int status){// todo fish
//        //先判断是否指挥再判断状态 ：teamPos_order
        TeamMemberDto.TeamMemberStatus s = (TeamMemberDto.TeamMemberStatus)status;
        var flagName = "";
        TeamDataHelper.StatusFlag.TryGetValue(s, out flagName);
        return flagName;
    }

}

public interface IMemberInfo{
    
    string TeamPosTxt{get;}
    bool IsShowArraySprite{ get;}   // 阵眼
    string EffectDescLblTxt{ get;}
    IEnumerable<Tuple<string,int>> EffectInfo{ get;}
    string nameStr{ get;}
    int lev{ get;}
    Faction FactionInfo{ get;}

//    model
}