using AppDto;

public class BaseSubmitDelegate {
    protected MissionDataMgr.MissionData _model;
    protected string _submitName;

    public BaseSubmitDelegate(MissionDataMgr.MissionData model,string submitName) {
        _model = model;
        _submitName = submitName;
    }


    /// <summary>
    /// 返回提交名字
    /// </summary>
    /// <returns></returns>
    public string GameSubmitName() {
        return _submitName;
    }

    public static NpcInfoDto GetNpc(NpcInfoDto npc) {
        if(npc == null || npc.npc == null)
            return null;
        NpcInfoDto npcInfo = null;
        if(WorldManager.Instance.GetModel().GetSceneId() == npc.sceneId)
            npcInfo = npc;

        //	当改NPC是虚拟NPC是转换为具体NPC
        if(npcInfo != null && npcInfo.npc is FactionNpc) {
            npcInfo.npc = MissionHelper.NpcVirturlToEntity(npcInfo.npc);
        }
        return npcInfo;
    }
}
