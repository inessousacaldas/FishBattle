// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : GameConfig.cs
// Author   : senkay <senkay@126.com>
// Created  : 12/5/2016 
// Porpuse  : 游戏配置文件，记录一些第三方插件的配置信息
// **********************************************************************
//
using System;

public class GameConfig
{
    //应用ID
    public const int APP_ID = 1;

    #region 七牛配置
    //https://portal.qiniu.com/bucket/h5-private
    //七牛访问域名
    public const string QINIU_DOMAIN = "oxr8wz9jf.bkt.clouddn.com";
    //七牛存储空间名
    public const string QINIU_BUCKET = "s3-private";
    //七牛图片类型
    public const string QINIU_MIMETYPE_IMAGE = "image/jpeg";
    //七牛音频类型
    public const string QINIU_MIMETYPE_AUDIO = "amr audio/x-amr";
    #endregion

    #region talkingdata配置
    //talkingdata appid 这个要注意因为调用时机的需要,TalkingDataHelper这里的appid才是这种用到的appid，放到框架层了
    public const string TALKINGDATA_APPID = "75DB7BC2B38B42958687FE14C0907959";
    #endregion

    #region 百度语音转文字配置
    //http://yuyin.baidu.com/app
    public const string BAIDU_VOP_APPID = "8997474";
    public const string BAIDU_VOP_APIKEY = "GdRqsjLgaOTQ7LQfKONXWOBr";
    public const string BAIDU_VOP_SECRETKEY = "76f61181d44f7e5351451458729a1f45";
    #endregion

    #region Testin配置
    //放到框架层代码
    #if UNITY_IPHONE
    public const string TESTIN_APPKEY = "675e18a4db1224c5971df10418e32165"; //TODO need new
    #else
    public const string TESTIN_APPKEY = "675e18a4db1224c5971df10418e32165"; //TODO need new
#endif
    #endregion

    #region TencentCos 腾讯对象存储
    public const string TENCENT_COSAPI_CGI_URL = "http://gz.file.myqcloud.com/files/v2/";
    public const int TENCENT_COS_APP_ID = 1251965894;
    public const string TENCENT_COS_SECRET_ID = "AKIDslhimvJsJbbWWdRVdnAh6aBeSfiamzb2";
    public const string TENCENT_COS_SECRET_KEY = "TmM4dgmw2yzEXg1IbylT4XmZOndPaksx";
    public const string TENCENT_COS_BUCKET = "mjgame";
    public const string TENCENT_COS_VOICE_Folder = "/Test/Chat/Voice/";
    #endregion

    #region TecentGCloud 腾讯游戏语音
    public const string TENCENT_GCLOUD_APPID = "1444219453";
    public const string TENCENT_GCLOUD_APPKEY = "d3368b6139ab6fd73227dd8adc55f93e";
    #endregion
}

