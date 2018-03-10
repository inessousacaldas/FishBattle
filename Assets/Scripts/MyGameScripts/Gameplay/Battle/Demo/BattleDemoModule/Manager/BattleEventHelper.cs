public static class BattleEventHelper
{
    // 放一些会同时发送的事件，方便以后修改-- fish 2017.7.26
    public static void SendEvent_BATTLE_UI_ACTION_REQUEST_SUCCESS()
    {
        GameEventCenter.SendEvent(GameEvent.BATTLE_UI_ACTION_REQUEST_SUCCESS);
        BattleDataManager.DataMgr.SetState(BattleSceneStat.FINISH_COMMAND);
        DemoSimulateHelper.SimulateRoundStart();
    }
}