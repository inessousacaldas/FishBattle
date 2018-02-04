package com.demiframe.game.api.shunwang.base;

import com.demiframe.game.api.common.LHRole;
/**
 * Created by DM-PC092 on 2017/5/2.
 */

public class User
{
    private static User userInstance;
    private String guidId;
    private String token;

    public static User getInstances()
    {
        if (userInstance == null)
            userInstance = new User();
        return userInstance;
    }

    public void Clear()
    {
        this.guidId = "";
        this.token = "";
    }

    public String getGuidId()
    {
        return guidId;
    }

    public void setGuidId(String id)
    {
        guidId = id;
    }

    public String getToken()
    {
        return token;
    }

    public void setAccessToken(String _token)
    {
        token = _token;
    }

}
