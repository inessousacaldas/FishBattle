package com.demiframe.game.api.common;

import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IExitCallback;

/**
 * Created by CL-PC007 on 2017/2/9.
 */

public class LHCallbackListener {
    private static LHCallbackListener instance;
    public static LHCallbackListener getInstance(){
        if(instance == null){
            instance = new LHCallbackListener();
        }
        return instance;
    }

    private IInitCallback initListener;
    private IUserListener userListener;
    private IExitCallback exitListener;
    private IHandleCallback payListener;
    private IHandleCallback channelInitListener;//获取渠道参数成功

    public void setInitListener(IInitCallback initListener){
        this.initListener = initListener;
    }

    public IInitCallback getInitCallback(){
        return initListener;
    }


    public void setUserListener(IUserListener userListener){
        this.userListener = userListener;
    }

    public IUserListener getUserListener(){
        return userListener;
    }

    public void setExitListener(IExitCallback exitCallback){
        this.exitListener = exitCallback;
    }

    public IExitCallback getExitListener(){
        return this.exitListener;
    }

    public void setPayListener(IHandleCallback payListener){
        this.payListener = payListener;
    }

    public IHandleCallback getPayListener(){
        return this.payListener;
    }

    public void setChannelInitListener(IHandleCallback channelInitListener){
        this.channelInitListener = channelInitListener;
    }

    public IHandleCallback getChannelInitListener(){
        return this.channelInitListener;
    }
}
