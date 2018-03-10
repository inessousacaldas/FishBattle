using AppDto;

public class GameServerTimeDtoListener : BaseDtoListener<GameServerTimeDto>
{
    protected override void HandleNotify(GameServerTimeDto notify)
    {
        SystemTimeManager.Instance.SyncServerTime(notify.time);
    }
}