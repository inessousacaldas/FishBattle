package com.demiframe.game.api.common;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;

import com.demiframe.game.api.connector.IExitCallback;

/**
 * Created by CL-PC007 on 2017/5/2.
 */

public class LHInnerExitDialog {
    private static AlertDialog exitDialog;
    public static void exit(final Activity activity, final IInnerExitCallback callback){
        if(exitDialog != null){
            exitDialog = null;
            callback.onCancel();
            return;
        }

        exitDialog = (new AlertDialog.Builder(activity)).setTitle("退出").setMessage("确定要退出游戏吗？").setPositiveButton("是", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int which) {
                callback.onSuccess();
                exitDialog = null;
            }
        }).setNegativeButton("否", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int which) {
                callback.onCancel();
                exitDialog = null;
            }
        }).setOnCancelListener(new DialogInterface.OnCancelListener() {
            public void onCancel(DialogInterface dialog) {
                callback.onCancel();
                exitDialog = null;
            }
        }).show();
    }
}
