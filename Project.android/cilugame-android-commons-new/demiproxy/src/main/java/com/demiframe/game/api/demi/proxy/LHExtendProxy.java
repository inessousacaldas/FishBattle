package com.demiframe.game.api.demi.proxy;

import android.app.Activity;
import android.text.TextUtils;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;

public class LHExtendProxy
  implements IExtend
{
  public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback)
  {
  }

  public void enterBBS(final Activity activity)
  {
  }

  public void enterUserCenter(final Activity activity)
  {
  }

  public void realNameRegister(final Activity activity, IHandleCallback handleCallback)
  {
  }

  public void submitRoleData(final Activity activity, LHRole lhRole)
  {
    activity.runOnUiThread(new Runnable()
    {
      public void run()
      {
        LogUtil.d("扩展数据提交成功");
      }
    });
  }

  //提前获取渠道号
  public String getSubChannelId(Activity activity){
    String subChannelId = IOUtil.getMetaDataByName(activity, "DemiFrameSubChannelId");
    if(TextUtils.isEmpty(subChannelId)){
      LogUtil.e("subChannalId is null");
    }
    return subChannelId;
  }

  public void GainGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====GainGameCoin=====");
    LogUtil.d(jsonStr);
  }

  public void ConsumeGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====ConsumeGameCoin=====");
    LogUtil.d(jsonStr);
  }
}