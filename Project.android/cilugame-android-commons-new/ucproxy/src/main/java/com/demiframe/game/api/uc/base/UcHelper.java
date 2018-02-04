package com.demiframe.game.api.uc.base;

import android.text.TextUtils;
import cn.uc.gamesdk.open.OrderInfo;
import com.demiframe.game.api.common.LHRole;

public class UcHelper
{
  public static UcHelper ucHelper;
  private OrderInfo orderInfo;
  private LHRole role;
  private UcUser ucUser;

  public static UcHelper getInstances()
  {
    if (ucHelper == null)
      ucHelper = new UcHelper();
    return ucHelper;
  }

  public boolean checkLogin()
  {
    return ((this.ucUser != null) && (!(TextUtils.isEmpty(this.ucUser.getSid()))));
  }

  public OrderInfo getOrderInfo()
  {
    return this.orderInfo;
  }

  public LHRole getRole()
  {
    return this.role;
  }

  public UcUser getUcUser()
  {
    return this.ucUser;
  }

  public void setOrderInfo(OrderInfo paramOrderInfo)
  {
    this.orderInfo = paramOrderInfo;
  }

  public void setRole(LHRole lhRole)
  {
    this.role = lhRole;
  }

  public void setUcUser(UcUser paramUcUser)
  {
    this.ucUser = paramUcUser;
  }
}