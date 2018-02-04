package com.demiframe.game.api.yijie.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.GameApi;
import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;

import com.snowfish.cn.ganga.helper.SFOnlineHelper;
import com.snowfish.cn.ganga.helper.SFOnlineInitListener;
import com.snowfish.cn.ganga.helper.SFOnlineLoginListener;
import com.snowfish.cn.ganga.helper.SFOnlineUser;

public class LHActivityProxy
  implements IActivity
{
  public LHActivityProxy()
  {
    LHCheckSupport.setCheckSupport(new ICheckSupport()
    {
      public boolean isAntiAddictionQuery()
      {
        return false;
      }

      public boolean isSupportBBS()
      {
        return false;
      }

      public boolean isSupportLogout()
      {
        return true;
      }

      public boolean isSupportOfficialPlacard()
      {
        return true;
      }

      public boolean isSupportShowOrHideToolbar()
      {
        return false;
      }

      public boolean isSupportUserCenter()
      {
        return false;
      }
    });
  }

  public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent)
  {
  }

  public void onCreate(Activity activity, final IInitCallback initCallback, Object object)
  {
    SFOnlineHelper.onCreate(activity, new SFOnlineInitListener() {
      @Override
      public void onResponse(String tag, String value) {
        if(tag.equalsIgnoreCase("success")){
          //初始化成功的回调
          LHResult lhResult = new LHResult();
          lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
          initCallback.onFinished(lhResult);
        }else if(tag.equalsIgnoreCase("fail")){
          //初始化失败的回调，value：如果SDK返回了失败的原因，会给value赋值
          LHResult lhResult = new LHResult();
          lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
          lhResult.setData(value);
          initCallback.onFinished(lhResult);
        }
      }});
    
    
    //// TODO: 2017/2/24 onLogout 没有失败的情况?
    SFOnlineHelper.setLoginListener(activity, new SFOnlineLoginListener() {
      @Override
      public void onLogout(Object o) {
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, o);
      }

      @Override
      public void onLoginSuccess(SFOnlineUser sfOnlineUser, Object o) {
        //// TODO: 2017/2/28 设置子渠道号，是否可以提前获取？ 
        GameApi.getInstance().getChannelInfo().setSubChannelId(sfOnlineUser.getChannelId());

        LHUser lhUser = new LHUser();
        //TODO:未处理yijie的唯一id
        lhUser.setSid(sfOnlineUser.getToken());
        //渠道的唯一id，并不是yijie的唯一id
        lhUser.setUid(sfOnlineUser.getChannelUserId());

        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
      }

      @Override
      public void onLoginFailed(String s, Object o) {
        LHUser lhUser = new LHUser();
        lhUser.setLoginMsg(s);
        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
      }
    });

  }

  public void afterOnCreate(Activity activity, IInitCallback listener, Object obj) {

  }

  public Boolean needAfterCreate(){
    return false;
  }

  public void onDestroy(Activity activity)
  {
    SFOnlineHelper.onDestroy(activity);
  }

  public void onNewIntent(Activity activity, Intent paramIntent)
  {
  }

  public void onPause(Activity activity)
  {
    SFOnlineHelper.onPause(activity);
  }

  public void onRestart(Activity activity)
  {
    SFOnlineHelper.onRestart(activity);
  }

  public void onResume(Activity activity)
  {
    SFOnlineHelper.onResume(activity);
    //如果刷新有延时，延时调用
//    handel.postDelayed(new Runnable() {
//      public void run() {
//        SFOnlineHelper.onResume(activity);
//      }
//    }, 1000);
  }

  public void onStop(Activity activity)
  {
    SFOnlineHelper.onStop(activity);
  }

  public void onStart(Activity activity)
  {
  }

  public void onConfigurationChanged(Activity activity, Configuration newConfig){

  }
}