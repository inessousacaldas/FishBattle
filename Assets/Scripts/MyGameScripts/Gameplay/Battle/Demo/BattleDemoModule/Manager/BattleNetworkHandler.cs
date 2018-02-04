using AppDto;
using UniRx;
using BattleNetworkManager = BattleDataManager.BattleNetworkManager;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeBattleNetworkHandler = new StaticDelegateRunner(
            BattleNetworkHandler.ExecuteDispose);
    }
}

public static class BattleNetworkHandler
{
    private static CompositeDisposable _disposable;
    private static CompositeDisposable _fix_disposable;

    public static void InitNotifyListener(){
        if (_fix_disposable == null)
        {
            _fix_disposable = new CompositeDisposable();
        }
        _fix_disposable.Add(NotifyListenerRegister.RegistListener<Video>(BattleNetworkManager.OnEnterBattleSuccess));
        _fix_disposable.Add(NotifyListenerRegister.RegistListener<DemoVideo>(BattleNetworkManager.OnEnterBattleSuccess));
    }

    public static void Setup()
    {
        if (_disposable != null)
        {
            return;
        }

        _disposable = new CompositeDisposable();
        _disposable.Add(NotifyListenerRegister.RegistListener<FighterReadyNotifyDto>(BattleNetworkManager.HandlerSoldierReadyNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<VideoRound>(BattleNetworkManager.HanderVideoRound));
        _disposable.Add(NotifyListenerRegister.RegistListener<ActionReadyNotify>(BattleDataManager.DataMgr.HandelActionQueueDto));
        _disposable.Add(NotifyListenerRegister.RegistListener<BattleAutoNotify>(BattleDataManager.DataMgr.HandleBattleAutoNotify));
    }

    public static void StopNotifyListener()
    {
        GameDebuger.LogError("StopNotifyListener-------------");
        _disposable = _disposable.CloseOnceNull();
    }

    public static void ExecuteDispose(){
        StopNotifyListener();
        _fix_disposable = _fix_disposable.CloseOnceNull();
    }

}