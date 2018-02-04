package com.demiframe.game.api.connector;

import  com.demiframe.game.api.common.LHUser;

public abstract interface IUserListener
{
    public abstract void onLogin(int code, LHUser lhUser);

    public abstract void onLogout(int code, Object data);
}
