package com.cilugame.android.commons;

import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.content.ClipData;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Build;

public class ZPHTemp {
	public static boolean isClientInstalled(String packageName) {
		PackageInfo packageInfo;
		try {
			packageInfo = getActivity().getPackageManager().getPackageInfo(packageName, 0);
		} catch (NameNotFoundException e) {
			packageInfo = null;
			e.printStackTrace();
		}
		if (packageInfo == null) {
			// System.out.println("没有安装");
			return false;
		} else {
			// System.out.println("已经安装");
			return true;
		}
	}

	public static void setTextToClipboard(final String str) {
		getActivity().runOnUiThread(new Runnable() {
			public void run() {
				if (Build.VERSION.SDK_INT >= 11) {
					android.content.ClipboardManager c = (android.content.ClipboardManager) getActivity()
							.getSystemService(Context.CLIPBOARD_SERVICE);
					c.setPrimaryClip(ClipData.newPlainText("CopyText", str));

				} else {
					android.text.ClipboardManager c = (android.text.ClipboardManager) getActivity()
							.getSystemService(Context.CLIPBOARD_SERVICE);
					c.setText(str);
				}
			}
		});
	}

	private static Activity getActivity() {
		return UnityPlayer.currentActivity;
	}
}
