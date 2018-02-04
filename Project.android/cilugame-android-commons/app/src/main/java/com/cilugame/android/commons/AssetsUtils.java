package com.cilugame.android.commons;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.zip.Inflater;
import java.util.zip.InflaterInputStream;

import android.app.Activity;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.res.AssetManager;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

/**
 * Asset要资源类
 * @author Tony
 *
 */
public class AssetsUtils {
	
	private static final String TAG = "AssetsUtils";
	
	private static final int BUFF_SIZE = 4096;
	
	private static Activity activity = null;
	
	private static AssetManager assetManager = null;
	
	public static Activity getActivity() {
		if (null == activity) {
			setActivity(UnityPlayer.currentActivity);
			AssetsUtils.assetManager = activity.getResources().getAssets();
		}
		return activity;
	}

	public static void setActivity(Activity activity) {
		AssetsUtils.activity = activity;
		AssetsUtils.assetManager = AssetsUtils.activity.getResources().getAssets();
	}

	/**
	 * 读取assets资源到byte数组
	 * 
	 * @param fileName
	 * @return byte数组
	 */
	public static byte[] readBytes(String fileName) {
		AssetManager assetManager = getActivity().getResources().getAssets();
		InputStream input = null;
		ByteArrayOutputStream output = null;
		byte [] data = new byte[0];
		try {
			input = assetManager.open(fileName);
			output = new ByteArrayOutputStream();
			byte[] buffer = new byte[BUFF_SIZE];
	        int n = 0;
	        while (-1 != (n = input.read(buffer))) {
	            output.write(buffer, 0, n);
	        }
	        data = output.toByteArray();
	        buffer = null;
	        Log.d(TAG, "readBytes size :" + data.length + ", from " + fileName );
		} catch (Exception e) {
			Log.e(TAG, "readBytes error at file :" + fileName, e);
		} finally {
			if ( input != null ) {
				try {
					input.close();
				} catch (IOException e) {
				}
				input = null;
			}
			if ( output != null ) {
				try {
					output.close();
				} catch (IOException e) {
				}
				output = null;
			}
		}
		return data;
	}
	
	/**
	 * 把asset复制成另一个目录文件
	 * @param assetName 资源名字
	 * @param toFilePath 文件全路径
	 * @param decompresses 是否解压(true解压,false正常读取)
	 * @return
	 */
	public static boolean copyAssetAs(String assetName,String toFilePath,boolean decompresses) {
		InputStream input = null;
		FileOutputStream output = null;
		boolean result = true;
		try {
			File toFile = new File(toFilePath);
			if ( !toFile.getParentFile().exists() ) {
				toFile.getParentFile().mkdirs();
			}
			
			input = assetManager.open(assetName);
			if ( decompresses ) {
				input = new InflaterInputStream(input,new Inflater(),BUFF_SIZE);
			}
			byte[] buffer = new byte[BUFF_SIZE];
	        int n = 0;
	        output = new FileOutputStream(toFile);
	        while (-1 != (n = input.read(buffer))) {
	            output.write(buffer, 0, n);
	        }
	        buffer = null;
	        Log.d(TAG, "copy asset:" + assetName + " as: " + toFilePath );
		} catch (Exception e) {
			Log.e(TAG, "copy assetName error at :" + assetName + " as: " + toFilePath, e);
			result = false;
		} finally {
			if ( input != null ) {
				try {
					input.close();
				} catch (IOException e) {
				}
				input = null;
			}
			if ( output != null ) {
				try {
					output.close();
				} catch (IOException e) {
				}
				output = null;
			}
		}
		return result;
	}
	
	/**
	 * 复制资源的批量接口
	 * assetName|toFilePath|decompresses,...
	 * @param assets
	 * @return
	 */
	public static void copyAssetAs(String assets) {
		String []strAssets = assets.split(",");
		for ( String strAsset : strAssets ) {
			String[] assetParams = strAsset.split("\\|");
			copyAssetAs(assetParams[0], assetParams[1], "1".equals(assetParams[2]));
		}
	}
	
	/**
	 * 读取metadata
	 * @param name
	 * @return
	 */
	public static String getMetaData(String name) {
		Log.e(TAG, "getMetaData name="+name);
		Activity cur = getActivity();
		String value = "";
		try {
			ApplicationInfo appInfo = cur.getPackageManager().getApplicationInfo(cur.getPackageName(), PackageManager.GET_META_DATA);
			value = appInfo.metaData.get(name).toString();
			Log.e(TAG, "getMetaData value="+value);
		} catch (Exception e) {
			Log.e(TAG, "getMetaData Exception", e);
		}
		return value;
	}
	
}
