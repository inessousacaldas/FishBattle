using System;

/// <summary>
/// 所有与游戏层的交互都通过此类(SdkLoginMessageBase, SdkLoginMessage)进行
/// </summary>
class SdkLoginMessage : SdkLoginMessageBase
{
    public static readonly SdkLoginMessage Instance = new SdkLoginMessage();
    public override void Sdk2CLogin(bool bGuest, string sid)
    {
        SPSdkManager.Instance.CallbackLoginSuccess(false, sid);
    }

    public override void Sdk2CLogout()
    {
        //游戏层销毁成功后需回发成功给sdk端
        ExitGameScript.Instance.DoReloginAccount();
    }

    public override void Sdk2CReLogin()
    {
        ExitGameScript.Instance.DoReloginAccount();
    }

    public override int GetGameID()
    {
        //H5游戏ID
        return GameConfig.APP_ID;
    }

    /// <summary>
    /// 控制Sdk UI的层次为最高
    /// 若游戏的最高的Layer高于此，请修改
    /// </summary>
    /// <returns></returns>
    public override int GetLayer()
    {
        return 500;
    }

    public override void SetupCoolDown(string taskName, float totalTime,
        Action<float> onUpdate, Action onFinished)
    {
        CSTimer.Instance.SetupCoolDown(taskName, totalTime,
            (remainTime) => { onUpdate(remainTime); },
            () => { onFinished(); },
            1.0f);
    }
}


