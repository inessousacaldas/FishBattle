package com.cilugame.h1.view;

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnDismissListener;
import android.content.DialogInterface.OnShowListener;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Typeface;
import android.os.Bundle;
import android.os.Handler;
import android.os.Handler.Callback;
import android.os.Message;
import android.text.Editable;
import android.text.InputFilter;
import android.text.TextWatcher;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.TextView;
import android.widget.TextView.OnEditorActionListener;
import com.cilugame.h1.UnityCallbackWrapper;
import com.cilugame.h1.util.Logger;

public class UnityEditTextDialog extends Dialog
{
  public static final int INPUT_FLAG_DEFAULT = 0;
  public static final int INPUT_FLAG_PASSWORD = 1;
  public static final int INPUT_FLAG_SENSITIVE = 2;
  public static final int INPUT_MODE_ANY = 0;
  public static final int INPUT_MODE_DECIMAL = 5;
  public static final int INPUT_MODE_EMAIL = 1;
  public static final int INPUT_MODE_NUM = 2;
  public static final int INPUT_MODE_PHONE = 3;
  public static final int INPUT_MODE_URL = 4;
  public static final int RETURN_TYPE_DEFAULT = 0;
  public static final int RETURN_TYPE_DONE = 1;
  public static final int RETURN_TYPE_GO = 4;
  public static final int RETURN_TYPE_SEARCH = 3;
  public static final int RETURN_TYPE_SEND = 2;
  public static UnityEditTextDialog editBoxDialog;
  private EditText editText;
  private UnityEditTextStyle editTextStyle;
  private Handler handler;
  private InputFilter inputFilter;
  private int mInputFlagConstraints;
  private int mInputModeContraints;
  private LinearLayout parentView;
  private String text;

  public UnityEditTextDialog(Context paramContext, String paramString, UnityEditTextStyle paramUnityEditTextStyle)
  {
    super(paramContext, 16973841);
    this.text = paramString;
    this.editTextStyle = paramUnityEditTextStyle;
  }

  private void Close()
  {
    //UnityCallbackWrapper.OnInputTextChanged(this.editText.getText().toString());
    CloseKeyboard();
    dismiss();
  }

  private void CloseKeyboard()
  {
    ((InputMethodManager)getContext().getSystemService("input_method")).hideSoftInputFromWindow(this.editText.getWindowToken(), 0);
  }

  private void OpenKeyboard()
  {
    ((InputMethodManager)getContext().getSystemService("input_method")).showSoftInput(this.editText, 0);
  }

  public void SetEditStyle(UnityEditTextStyle paramUnityEditTextStyle)
  {
	  /*
    if (paramUnityEditTextStyle == null)
      return;
    this.editTextStyle = paramUnityEditTextStyle;
    LinearLayout.LayoutParams localLayoutParams = new LinearLayout.LayoutParams(-1, -1);
    localLayoutParams.leftMargin = paramUnityEditTextStyle.left;
    localLayoutParams.topMargin = paramUnityEditTextStyle.top;
    localLayoutParams.width = paramUnityEditTextStyle.width;
    localLayoutParams.height = paramUnityEditTextStyle.height;
    this.editText.setLayoutParams(localLayoutParams);
    this.editText.setPadding(0, 0, 0, 0);
    this.editText.setTypeface(Typeface.DEFAULT_BOLD);
    this.editText.setTextSize(0, paramUnityEditTextStyle.textSize);
    this.editText.setGravity(paramUnityEditTextStyle.alignment);
    this.editText.setTextColor(Color.argb(paramUnityEditTextStyle.textColorA, paramUnityEditTextStyle.textColorR, paramUnityEditTextStyle.textColorG, paramUnityEditTextStyle.textColorB));
    this.editText.setBackgroundColor(0);
    int i = this.editText.getImeOptions();
    this.editText.setImeOptions(0x10000000 | i);
    i = this.editText.getImeOptions();
    switch (paramUnityEditTextStyle.inputMode)
    {
    default:
      label208: this.editText.setInputType(this.mInputModeContraints | this.mInputFlagConstraints);
      switch (paramUnityEditTextStyle.inputFlag)
      {
      default:
      case 0:
      case 1:
      case 2:
      }
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
    case 5:
    }
    while (true)
    {
      this.editText.setInputType(this.mInputFlagConstraints | this.mInputModeContraints);
      switch (paramUnityEditTextStyle.inputReturn)
      {
      default:
        this.editText.setImeOptions(i | 0x1);
        return;
        this.mInputModeContraints = 1;
        break label208:
        this.mInputModeContraints = 33;
        break label208:
        this.mInputModeContraints = 4098;
        break label208:
        this.mInputModeContraints = 3;
        break label208:
        this.mInputModeContraints = 17;
        break label208:
        this.mInputModeContraints = 12290;
        break label208:
        this.mInputFlagConstraints = 1;
        continue;
        this.mInputFlagConstraints = 129;
        continue;
        this.mInputFlagConstraints = 524288;
      case 0:
      case 1:
      case 2:
      case 3:
      case 4:
      }
    }
    this.editText.setImeOptions(i | 0x1);
    return;
    this.editText.setImeOptions(i | 0x6);
    return;
    this.editText.setImeOptions(i | 0x4);
    return;
    this.editText.setImeOptions(i | 0x3);
    return;
    this.editText.setImeOptions(i | 0x2);
    */
  }

  public void SetText(String paramString)
  {
    this.editText.setText(paramString);
  }

  protected void onCreate(Bundle paramBundle)
  {
    super.onCreate(paramBundle);
    this.parentView = new LinearLayout(getContext());
    LayoutParams layoutParams = new LinearLayout.LayoutParams(-1, -1);
    this.parentView.setOrientation(LinearLayout.VERTICAL);
    this.editText = new EditText(getContext());
    this.parentView.addView(this.editText);
    this.setContentView(this.parentView, layoutParams);  
    getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);
    SetText(this.text);
    SetEditStyle(this.editTextStyle);
    this.handler = new Handler(new Handler.Callback()
    {
      public boolean handleMessage(Message paramMessage)
      {
        UnityEditTextDialog.this.editText.requestFocus();
        UnityEditTextDialog.this.editText.setSelection(UnityEditTextDialog.this.editText.length());
        UnityEditTextDialog.this.OpenKeyboard();
        return false;
      }
    });
    this.parentView.setOnClickListener(new View.OnClickListener()
    {
      public void onClick(View paramView)
      {
        UnityEditTextDialog.this.Close();
      }
    });
    this.editText.addTextChangedListener(new TextWatcher()
    {
      public void afterTextChanged(Editable paramEditable)
      {
      }

      public void beforeTextChanged(CharSequence paramCharSequence, int paramInt1, int paramInt2, int paramInt3)
      {
      }

      public void onTextChanged(CharSequence paramCharSequence, int paramInt1, int paramInt2, int paramInt3)
      {
       // UnityCallbackWrapper.OnInputTextChanged(paramCharSequence.toString());
      }
    });
    this.editText.setOnEditorActionListener(new TextView.OnEditorActionListener()
    {
      public boolean onEditorAction(TextView paramTextView, int paramInt, KeyEvent paramKeyEvent)
      {
        if ((paramInt != 0) || ((paramInt == 0) && (paramKeyEvent != null) && (paramKeyEvent.getAction() == 0)))
        {
          UnityEditTextDialog.this.Close();
          //UnityCallbackWrapper.OnInputReturn();
          return true;
        }
        return false;
      }
    });
    setOnShowListener(new DialogInterface.OnShowListener()
    {
      public void onShow(DialogInterface paramDialogInterface)
      {
        Logger.Log("Show UnityEditTextDialog");
        //UnityCallbackWrapper.OnEditDialogShow();
        UnityEditTextDialog.this.handler.sendEmptyMessageDelayed(1, 50L);
      }
    });
    setOnDismissListener(new DialogInterface.OnDismissListener()
    {
      public void onDismiss(DialogInterface paramDialogInterface)
      {
        Logger.Log("Close UnityEditTextDialog");
        //UnityCallbackWrapper.OnEditDialogHide();
      }
    });
  }
}

/* Location:           C:\dex2jar-2.0\bbxy-1.7.19-b-n-112-dex2jar.jar
 * Qualified Name:     com.yuelang.xiyou.view.UnityEditTextDialog
 * JD-Core Version:    0.5.4
 */