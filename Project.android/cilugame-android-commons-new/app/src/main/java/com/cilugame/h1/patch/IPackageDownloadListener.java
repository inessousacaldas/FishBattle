package com.cilugame.h1.patch;

/**
 * 整包下载事件监听器
 * <p/>
 * Created by Tony on 15/12/28.
 */
public interface IPackageDownloadListener {

    /**
     * 无事件
     */
    public static final int EVENT_TYPE_NONE = 0;

    /**
     * 玩家点开始更新
     */
    public static final int EVENT_TYPE_START = 1;

    /**
     * 更新被暂停了
     */
    public static final int EVENT_TYPE_STOP = 2;

    /**
     * 玩家退出了更新界面
     */
    public static final int EVENT_TYPE_QUIT_DOWNLOAD = 3;

    /**
     * 下载已完成
     */
    public static final int EVENT_TYPE_DOWNLOAD_FINISH = 4;

    /**
     * 开始安装
     */
    public static final int EVENT_TYPE_INSTALL = 5;

    /**
     * 玩家取消了安装
     */
    public static final int EVENT_TYPE_QUIT_INSTALL = 6;

    void onEvent(int type, Object... args);
}
