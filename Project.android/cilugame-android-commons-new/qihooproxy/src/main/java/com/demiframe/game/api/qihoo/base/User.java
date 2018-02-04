package com.demiframe.game.api.qihoo.base;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHUser;
/**
 * Created by FQ-PC018 on 2017/2/27.
 */

public class User {
    private static User userInstance;
    private String uid;

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

    public void setUserInfo(LHUser userInfo){
        if(userInfo == null){
            uid = "";
            return;
        }

        this.uid = userInfo.getUid();
    }

    //用户唯一id
    public String getUserId(){
        return uid;
    }

    public boolean CheckLogin(){
        if(uid == null){
            return false;
        }

        return !uid.isEmpty();
    }

    public void Clear(){
        this.uid = "";
        this.lastRoleData = null;
    }
}
