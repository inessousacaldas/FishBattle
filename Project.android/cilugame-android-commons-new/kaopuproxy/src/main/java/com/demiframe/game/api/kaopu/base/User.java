package com.demiframe.game.api.kaopu.base;

import com.demiframe.game.api.common.LHRole;


public class User {
    private static User userInstance;

    public LHRole getLastRoleData() {
        return lastRoleData;
    }

    public void setLastRoleData(LHRole lastRoleData) {
        this.lastRoleData = lastRoleData;
    }

    private LHRole lastRoleData;

    public static User getInstances()
    {
        if (userInstance == null)
            userInstance = new User();
        return userInstance;
    }

    public void Clear(){
        this.lastRoleData = null;
    }
}
