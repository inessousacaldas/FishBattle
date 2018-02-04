package com.cilugame.mobilephotoreader;

public class PhotoReaderLogUtil {
	private static String TAG = "PhotoReader";
	
	public static void Log(String msg)
	{
		android.util.Log.d(TAG, msg);
	}

	public static void Log(String msg, Exception e)
	{
		android.util.Log.e(TAG, msg, e);
	}	
}
