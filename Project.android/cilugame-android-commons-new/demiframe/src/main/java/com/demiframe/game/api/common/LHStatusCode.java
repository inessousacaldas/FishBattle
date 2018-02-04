package com.demiframe.game.api.common;

/**
 * Created by CL-PC007 on 2017/2/7.
 */

public class LHStatusCode
{

    private LHStatusCode()
    {
    }

    public static final int LH_SDK_NOSUPPORT = -1;
    public static final int LH_INIT_SUCCESS = 0;
    public static final int LH_INIT_FAIL = 1;
    public static final int LH_LOGIN_SUCCESS = 2;
    public static final int LH_LOGIN_FAIL = 3;
    public static final int LH_LOGIN_CANCEL = 4;
    public static final int LH_LOGIN_SUCCESS_NON_PLATFORM = 5;
    public static final int LH_HNDLE_FISHED = 6;
    //未认证,继续游戏
    public static final int LH_ADDICTED_NO_USER = 7;
    //未认证，不能继续游戏
    public static final int LH_ADDICTED_NO_USER_QUIT = 23;
    //已认证，未成年
    public static final int LH_ADDICTED_MINORITY = 8;
    //已认证，成年
    public static final int LH_ADDICTED_ADULTS = 9;
    //认证出错
    public static final int LH_ADDICTED_EXCEPTION = 10;

    public static final int LH_LOGOUT_SUCCESS = 11;
    public static final int LH_LOGOUT_FAIL = 12;
    public static final int LH_LOGOUT_CANCEL = 13;
    public static final int LH_GUEST_LOGIN_SUCCESS = 15;
    public static final int LH_PAY_SUCCESS = 17;
    public static final int LH_PAY_FAIL = 18;
    public static final int LH_PAY_CANCEL = 19;
    public static final int LH_PAY_SUBMITED = 20;
    public static final int LH_GETCHANNEL_SUCCESS = 21;
    public static final int LH_GETCHANNEL_FAIL = 22;
}
