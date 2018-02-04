using System;
using UnityEngine;

public abstract class SdkLoginMessageBase
{
    /// <summary>
    /// 登录成功
    /// </summary>
    /// <param name="bGuest">是否是设备号登录</param>
    /// <param name="sid"></param>
    public abstract void Sdk2CLogin(bool bGuest, string sid);


    /// <summary>
    /// SDK登出
    /// </summary>
    public abstract void Sdk2CLogout();

    /// <summary>
    /// 切换账号, 游戏内做清除工作并返回到登录场景
    /// </summary>
    public abstract void Sdk2CReLogin();

    /// <summary>
    /// 设置LoginSDK的UI根结点
    /// </summary>
    /// <param name="root"></param>
    public void C2SdkInitRoot(GameObject root)
    {
        SdkModuleMgr.Instance.InitRoot(root);
    }

    /// <summary>
    /// 打开登录界面
    /// </summary>
    public void C2SdkLogin()
    {
        SdkProxyModule.ClearModule();
        SdkProxyModule.OpenLogin();
    }

    /// <summary>
    /// 打开绑定界面
    /// </summary>
    public void C2SdkBind()
    {
        SdkProxyModule.OpenBind();
    }

    /// <summary>
    /// 打开用户中心
    /// </summary>
    public void C2SdkUserCenter()
    {
        SdkProxyModule.OpenGameCenter();
    }

    public void C2SdkLogout()
    {
        SdkAccountModel.Instance.Game2Logout();
    }

    public string GetServerUrl()
    {
        return GameSetting.SDK_SERVER;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    public string GetRegistUrl()
    {
        return GetServerUrl() + "/sdkc/account/register.json";
    }

    public string GetFindPasswordUrl()
    {
        return GetServerUrl() + "/sdkc/account/findPassword.json";
    }

    public string GetModifyPasswordUrl()
    {
        return GetServerUrl() + "/sdkc/account/updatePassword.json"; ;
    }

    public string GetLoginUrl()
    {
        return GetServerUrl() + "/sdkc/account/login.json";
    }

    public string GetBoundUrl()
    {
        return GetServerUrl() + "/sdkc/account/bound.json";
    }

    public string GetCheckSessionUrl()
    {
        return GetServerUrl() + "/sdkc/account/checkSession.json";
    }

    /// <summary>
    /// 获取手机验证码
    /// </summary>
    /// <returns></returns>
    public string GetPhoneCodeUrl()
    {
        return GetServerUrl() + "/sdkc/account/phoneVerifyCode.json";
    }

    /// <summary>
    /// 获取发送验证码url，做账号的手机绑定
    /// </summary>
    /// <returns>The send verify code URL.</returns>
    public string GetSendVerifyCodeUrl()
    {
        return GetServerUrl() + "/sdkc/account/smsCode.json";
    }

    /// <summary>
    /// 获取绑定手机号码
    /// </summary>
    /// <returns>The send verify code URL.</returns>
    public string GetBindMobileNum()
    {
        return GetServerUrl() + "/sdkc/account/showPhone.json";
    }

    public string GetUUID()
    {
        return BaoyugameSdk.getUUID();
    }

    public void GetPay()
    {

    }

    /// <summary>
    /// 获取项目游戏ID
    /// </summary>
    public abstract int GetGameID();
    public abstract int GetLayer();

    public abstract void SetupCoolDown(string taskName, float totalTime,
        Action<float> onUpdate, Action onFinished);
}
