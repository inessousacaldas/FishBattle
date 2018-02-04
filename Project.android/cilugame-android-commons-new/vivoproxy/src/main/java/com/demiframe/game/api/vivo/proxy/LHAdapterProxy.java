package com.demiframe.game.api.vivo.proxy;

import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.IAdapter;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.connector.IToolBar;
import com.demiframe.game.api.connector.IUserManager;

public class LHAdapterProxy
        implements IAdapter {
    private LHActivityProxy activityProxy;
    private LHExitProxy exitProxy;
    private LHExtendProxy extendProxy;
    private LHPayProxy payProxy;
    private LHToolBarProxy toolbarProxy;
    private LHUserManagerProxy userManagerProxy;

    public IActivity activityProxy() {
        if (this.activityProxy == null)
            this.activityProxy = new LHActivityProxy();
        return this.activityProxy;
    }

    public IExit exitProxy() {
        if (this.exitProxy == null)
            this.exitProxy = new LHExitProxy();
        return this.exitProxy;
    }

    public IExtend extendProxy() {
        if (this.extendProxy == null)
            this.extendProxy = new LHExtendProxy();
        return this.extendProxy;
    }

    public IPay payProxy() {
        if (this.payProxy == null)
            this.payProxy = new LHPayProxy();
        return this.payProxy;
    }

    public IToolBar toolbarProxy() {
        if (this.toolbarProxy == null)
            this.toolbarProxy = new LHToolBarProxy();
        return this.toolbarProxy;
    }

    public IUserManager userManagerProxy() {
        if (this.userManagerProxy == null)
            this.userManagerProxy = new LHUserManagerProxy();
        return this.userManagerProxy;
    }
}