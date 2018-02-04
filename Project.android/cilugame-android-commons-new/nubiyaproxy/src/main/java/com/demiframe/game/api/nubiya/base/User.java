package com.demiframe.game.api.nubiya.base;

/**
 * Created by DM-PC092 on 2017/5/3.
 */

public class User
{
    private static User userInstance;
    private String UId;
    private String SessionId;
    //用于映射游戏账户的ID
    private String gameID;

    public static User getInstances()
    {
        if (userInstance == null)
            userInstance = new User();
        return userInstance;
    }

    public void Clear()
    {
        this.UId = "";
        this.SessionId = "";
        this.gameID = "";
    }

    public String getUId()
    {
        return UId;
    }

    public void setUId(String id)
    {
        UId = id;
    }

    public String getSessionId()
    {
        return SessionId;
    }

    public void setSessionId(String _sid)
    {
        SessionId = _sid;
    }

    public void setGameID(String gameID){
        this.gameID = gameID;
    }

    public String getGameID(){
        return this.gameID;
    }
}
