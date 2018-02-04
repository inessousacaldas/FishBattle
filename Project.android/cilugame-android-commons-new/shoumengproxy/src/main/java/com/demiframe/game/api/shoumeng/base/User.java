package com.demiframe.game.api.shoumeng.base;

import mobi.shoumeng.integrate.game.UserInfo;

/**
 * Created by FQ-PC018 on 2017/2/27.
 */

public class User {
    private static User userInstance;
    private UserInfo userInfo;
    public static User getInstances()
    {
        if (userInstance == null)
            userInstance = new User();
        return userInstance;
    }

    public void setUserInfo(UserInfo userInfo){
        this.userInfo = userInfo;
    }

    //用户唯一id
    public String getUserId(){
        if(this.userInfo == null){
            return "";
        }
        return this.userInfo.getLoginAccount();
    }
}
