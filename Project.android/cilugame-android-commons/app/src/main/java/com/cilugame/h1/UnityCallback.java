package com.cilugame.h1;

import java.util.ArrayList;

public class UnityCallback
{
  public static native String InputValidate(String paramString);

  public static native void OnBatteryChange(int paramInt);

  public static native void OnCheckAndUpdateVersion(int paramInt);

  public static native void OnDeleteTagResult(int paramInt, String paramString);

  public static native void OnEditDialogHide();

  public static native void OnEditDialogShow();

  public static native void OnEnterBackground();

  public static native void OnEnterForeground();

  public static native void OnGetClipboardText(String paramString);

  public static native void OnInputReturn();

  public static native void OnInputTextChanged(String paramString);

  public static native void OnKeyUp(int paramInt);

  public static native void OnNotifactionClickedResult(String paramString1, String paramString2, String paramString3);

  public static native void OnNotifactionShowedResult(String paramString1, String paramString2, String paramString3);

  public static native void OnPay(int paramInt, String paramString);

  public static native void OnQihooQueryAntiAddiction(int paramInt);

  public static native void OnQihooRegRealName(String paramString);

  public static native void OnQihooShare(int paramInt);

  public static native void OnRegisterResult(int paramInt, String paramString);

  public static native void OnSelectImage(String[] paramArrayOfString);

  public static native void OnSetTagResult(int paramInt, String paramString);

  public static native void OnShowExitDialog(int paramInt);

  public static native void OnShowUCVipPage(int paramInt);

  public static native void OnTerminate();

  public static native void OnTextMessage(String paramString1, String paramString2, String paramString3);

  public static native void OnUnregisterResult(int paramInt);

  public static native void OnYybShare(int paramInt);
}