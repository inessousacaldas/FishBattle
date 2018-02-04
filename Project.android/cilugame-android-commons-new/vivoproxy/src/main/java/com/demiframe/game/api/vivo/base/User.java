package com.demiframe.game.api.vivo.base;

import com.demiframe.game.api.common.LHRole;

/**
 * Created by DM-PC092 on 2017/3/27.
 */

public class User
{
    private static User userInstance;
    private String openid;
    private String authtoken;
    private String name;

    private LHRole lastRoleData;

    public static User getInstances()
    {
        if (userInstance == null)
            userInstance = new User();
        return userInstance;
    }

    public void Clear()
    {
        this.openid = "";
        this.authtoken = "";
        this.name = "";
        this.lastRoleData = null;
    }

    public LHRole getLastRoleData() {
        return lastRoleData;
    }

    public void setLastRoleData(LHRole lastRoleData) {
        this.lastRoleData = lastRoleData;
    }

    public String getOpenId()
    {
        return openid;
    }

    public void setOpenId(String id)
    {
        openid = id;
    }

    public String getName()
    {
        return name;
    }

    public void setName(String _name)
    {
        name = _name;
    }

    public String getAuthtoken()
    {
        return authtoken;
    }

    public void setAuthtoken(String _authtoken)
    {
        authtoken = _authtoken;
    }

}
