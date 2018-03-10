using System;

/// <summary>
/// 怪物操作状态管理器
/// 主要是怪物的能否使用技能状态及通知
/// @MarsZ
/// </summary>
public class MonsterOptionStateManager : IDisposable
{
    private MonsterController mMonsterController = null;
    private long mMonsterUID = 0L;

    public MonsterOptionStateManager(MonsterController pMonsterController)
    {
        mMonsterController = pMonsterController;
        mMonsterUID = mMonsterController.GetId();
    }

    private MonsterOptionState mOptionState;

    public MonsterOptionState OptionState
    {
        get
        { 
            return mOptionState;
        }
        set
        {
            if (mOptionState != value)
            {
                if (value == MonsterOptionState.Enable)
                {
                    if (!OptionEnableEnable)
                        return;
                }
                mOptionState = value;
                GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_MONSTER_OPTION_STATE_CHANGED, mMonsterUID, mOptionState);
            }
        }
    }

    //是否可以设置为操作可用。
    private bool OptionEnableEnable
    {
        get
        {
            return null != mMonsterController && !mMonsterController.dead;
        }
    }

    public void Dispose()
    {
        OptionState = MonsterOptionState.Disable; 
        mMonsterController = null;
        mMonsterUID = 0L;
    }

    //暂定2个状态，以后可扩展
    public enum MonsterOptionState
    {
        Disable = 0,
        Enable = 1
    }
}