package com.cilugame.h1.view;

import android.app.Activity;
import com.cilugame.h1.util.Logger;

public class ViewManager
{
  private Activity context;
  public UnityEditTextDialog editDialog;

  public ViewManager(Activity paramActivity)
  {
    this.context = paramActivity;
  }

  public void HideEditDialog()
  {
    if ((this.editDialog == null) || (!this.editDialog.isShowing()))
      return;
    this.context.runOnUiThread(new Runnable()
    {
      public void run()
      {
        ViewManager.this.editDialog.dismiss();
      }
    });
  }

  public void SetEditText(final String paramString)
  {
    if (this.editDialog == null)
      return;

    this.context.runOnUiThread(new Runnable()
    {
      public void run()
      {
        if (ViewManager.this.editDialog == null)
          return;
        ViewManager.this.editDialog.SetText(paramString);
      }
    });
  }

  public void ShowEditDialog(final String paramString, final UnityEditTextStyle paramUnityEditTextStyle)
  {
    Logger.Log("Call ShowEditDialog");
    Logger.Log("left " + paramUnityEditTextStyle.left);
    Logger.Log("top " + paramUnityEditTextStyle.top);
    Logger.Log("width " + paramUnityEditTextStyle.width);
    Logger.Log("height " + paramUnityEditTextStyle.height);
    Logger.Log("maxLength " + paramUnityEditTextStyle.maxLength);
    Logger.Log("textSize " + paramUnityEditTextStyle.textSize);
    Logger.Log("textColor a:" + paramUnityEditTextStyle.textColorA + " r:" + paramUnityEditTextStyle.textColorR + " g:" + paramUnityEditTextStyle.textColorG + " b:" + paramUnityEditTextStyle.textColorB);
    Logger.Log("inputMode " + paramUnityEditTextStyle.inputMode);
    Logger.Log("inputFlag " + paramUnityEditTextStyle.inputFlag);
    Logger.Log("inputReturn " + paramUnityEditTextStyle.inputReturn);
    if ((this.editDialog != null) && (this.editDialog.isShowing()))
      return;
    this.context.runOnUiThread(new Runnable()
    {
      public void run()
      {
        if (ViewManager.this.editDialog == null)
        {
        	ViewManager.this.editDialog = new UnityEditTextDialog(ViewManager.this.context, paramString, paramUnityEditTextStyle);
        }
        else
        {
            ViewManager.this.editDialog.show();
            ViewManager.this.editDialog.SetText(paramString);
            ViewManager.this.editDialog.SetEditStyle(paramUnityEditTextStyle);
        }
      }
    });
  }
}