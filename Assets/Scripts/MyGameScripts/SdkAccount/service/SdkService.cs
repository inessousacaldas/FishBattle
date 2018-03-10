using System;
using System.Collections;
using System.Collections.Generic;
using SdkAccountDto;

class SdkService
{
    private static Dictionary<string, string> _jsonDics = new Dictionary<string, string>();
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="name">账号名</param>
    /// <param name="password">md5密码</param>
    /// <param name="type">账号类型0自由；1手机；2邮箱；3设备；4QQ登录；5微信登录；目前暂支持1,2,3.</param>
    public static void RequestLogin(string name, string password, 
        AccountDto.AccountType type, 
        Action<AccountDto> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetLoginUrl();

        //设备号登录，用户名仅显示用，密码作为登录账户
        if(type == AccountDto.AccountType.device)
        {
            url = url + string.Format("?gameId={0}&name={1}&type={2}",
                manager.GetGameID(), password, (int)type);
        }
        else
        {
            url = url + string.Format("?gameId={0}&name={1}&password={2}&type={3}", 
                manager.GetGameID(), name, password, (int)type);
        }

        RequestJson(url, "SdkLogin", "登录中", delegate (string json)
        {
            var dto = JsHelper.ToObject<LoginResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                AccountDto dAccount = new AccountDto(dto);
                //dAccount.name = name;
                dAccount.password = password;
                dAccount.type = type;
                downLoadFinishCallBack(dAccount);
            }
        }, true, true);
    }

    /// <summary>
    /// 请求验证码
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="downLoadFinishCallBack"></param>
    public static void RequestPhoneCode(string phone,
        Action<int> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetPhoneCodeUrl();
        url = url + "?phone=" + phone;

        //应该返回错误码,而不是json？
        RequestJson(url, "SdkRegister", "请求验证码中", delegate (string json)
        {
            ResponseDto dto = JsHelper.ToObject<ResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                downLoadFinishCallBack(0);
            }
        }, true, true);
    }

    /// <summary>
    /// 注册、修改密码、找回密码
    /// </summary>
    /// <param name="name">目前只支持手机号</param>
    /// <param name="password"></param>
    /// <param name="type"></param>
    /// <param name="verifyCode"></param>
    /// <param name="optType">操作类型:1注册；2修改密码；3找回密码</param>
    /// <param name="downLoadFinishCallBack"></param>
    public static void RequestRegister(string name, string password, AccountDto.AccountType type,
         string verifyCode, Action<int> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetRegistUrl();
        url = url + string.Format("?gameId={0}&name={1}&password={2}&type={3}&verifyCode={4}",
            manager.GetGameID(), name, password, (int)type, verifyCode);

        RequestJson(url, "SdkRegister", "请求中", delegate (string json)
        {
            ResponseDto dto = JsHelper.ToObject<ResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                downLoadFinishCallBack(0);
            }
        }, true, true);
    }


    public static void RequestFindPassword(string name, string password, AccountDto.AccountType type, 
        string verifyCode, Action<int> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetFindPasswordUrl();

        url = url + string.Format("?gameId={0}&name={1}&password={2}&type={3}&verifyCode={4}",
           manager.GetGameID(), name, password, (int)type, verifyCode);

        RequestJson(url, "SdkFindPassword", "请求中", delegate (string json)
        {
            ResponseDto dto = JsHelper.ToObject<ResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                downLoadFinishCallBack(0);
            }
        }, true, true);
    }

    public static void RequestModifyPassword(string sid, string password, 
        string verifyCode, Action<int> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetModifyPasswordUrl();

        url = url + string.Format("?gameId={0}&sid={1}&password={2}&verifyCode={3}",
           manager.GetGameID(), sid, password, verifyCode);

        RequestJson(url, "SdkModifyPassword", "请求中", delegate (string json)
        {
            ResponseDto dto = JsHelper.ToObject<ResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                downLoadFinishCallBack(0);
            }
        }, true, true);
    }

    /// <summary>
    /// 账号绑定
    /// </summary>
    /// <param name="sid">已登录的会话ID</param>
    /// <param name="name">绑定目标账号名</param>
    /// <param name="password"></param>
    /// <param name="type">绑定目标账号类型</param>
    /// <param name="verifyCode"></param>
    /// <param name="downLoadFinishCallBack"></param>
    public static void RequestBind(string sid, string name, string password,
        AccountDto.AccountType type, string verifyCode, Action<int> downLoadFinishCallBack)
    {
        var manager = SdkLoginMessage.Instance;
        string url = manager.GetBoundUrl();
        url = url + string.Format("?gameId={0}&sid={1}&name={2}&password={3}&type={4}&verifyCode={5}",
            manager.GetGameID(), sid, name, password, (int)type, verifyCode);

        RequestJson(url, "SdkBind", "请求绑定中", delegate (string json)
        {
            ResponseDto dto = JsHelper.ToObject<ResponseDto>(json);
            if (CheckDtoValid(dto))
            {
                downLoadFinishCallBack(0);
            }
        }, true, true);
    }

    public static bool CheckDtoValid(ResponseDto dto)
    {
        if (dto == null)
        {
            SdkProxyModule.ShowTips("请求超时");
            return false;
        }
        if (dto.code > 0)
        {
            SdkProxyModule.ShowTips(dto.msg);
            return false;
        }

        return true;
    }

    public static void RequestJson(string url, string jsonName, string tips, Action<string> downLoadFinishCallBack,
        bool needLock = true, bool refresh = false, Dictionary<string, string> headers = null)
    {
        if (!refresh && _jsonDics.ContainsKey(jsonName))
        {
            string json = _jsonDics[jsonName];
            downLoadFinishCallBack(json);
            return;
        }

        GameDebuger.Log("RequestJson " + url);


        if (needLock)
        {
            SdkLoadingTipController.Show(tips, true, true);
        }

        Hashtable hashHeaders = null;
        if (headers != null)
        {
            hashHeaders = new Hashtable();
            foreach (var header in headers)
            {
                hashHeaders[header.Key] = header.Value;
            }
        }

        HttpController.Instance.DownLoad(url, delegate (ByteArray byteArray)
        {
            if (needLock)
            {
                SdkLoadingTipController.Stop(tips);
            }

            string json = byteArray.ToUTF8String();

            _jsonDics[jsonName] = json;

            GameDebuger.Log("下载成功");
            GameDebuger.Log(json);

            downLoadFinishCallBack(json);
        }, null, delegate (Exception exception)
        {
            if (needLock)
            {
                SdkLoadingTipController.Stop(tips);
            }

            GameDebuger.Log(string.Format("RequestJson url={0} error={1}", url, exception.ToString()));

            downLoadFinishCallBack(null);
        }, false, SimpleWWW.ConnectionType.Short_Connect, hashHeaders);
    }
}
