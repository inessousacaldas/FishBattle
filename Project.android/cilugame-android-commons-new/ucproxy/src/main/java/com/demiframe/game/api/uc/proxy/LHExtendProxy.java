package com.demiframe.game.api.uc.proxy;

import android.app.Activity;
import android.text.TextUtils;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.uc.base.UcHelper;
import com.demiframe.game.api.uc.ucutils.LHUCGameSdk;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;

public class LHExtendProxy
  implements IExtend
{

  public void enterBBS(Activity activity)
  {
    LogUtil.d("渠道不支持的操作<enterBBS>");
  }

  public void enterUserCenter(Activity activity)
  {
    LogUtil.d("渠道不支持的操作<enterUserCenter>");
  }

  public void realNameRegister(Activity activity, IHandleCallback handleCallback)
  {
    LogUtil.d("渠道不支持的操作<realNameRegister>");
    if (handleCallback != null)
    {
      LHResult localLHResult = new LHResult();
      localLHResult.setCode(-1);
      handleCallback.onFinished(localLHResult);
    }
  }

  public void submitRoleData(Activity activity, LHRole lhRole)
  {
    UcHelper.getInstances().setRole(lhRole);
    //改由服务端提交角色数据
    //LHUCGameSdk.submitRoleData(activity, lhRole.getZoneId(), lhRole.getZoneName(), lhRole.getRoledId(), lhRole.getRoleName(),
    //    Long.parseLong(lhRole.getRoleLevel()), Long.parseLong(lhRole.getRoleCTime())/1000);
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
  }

  public void ConsumeGameCoin(Activity activity, String jsonStr){
  }
}