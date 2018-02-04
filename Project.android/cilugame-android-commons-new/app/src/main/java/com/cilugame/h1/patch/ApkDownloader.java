package com.cilugame.h1.patch;

import java.io.File;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.os.AsyncTask;
import android.os.Environment;
import android.os.StatFs;
import android.util.Log;

import com.cilugame.android.commons.HttpUtils;
import com.cilugame.android.commons.IOUtils;
import com.cilugame.h1.UnityCallbackWrapper;
import com.cilugame.h1.activity.CLGamePacketFragment;
import com.unity3d.player.UnityPlayer;

/**
 * Apk文件下载器
 * <p/>
 * Created by Tony on 15/12/19.
 */
public class ApkDownloader {

    public static final String TAG = "ApkDownloader";

    private static Activity currentActivity;

    private static String packageName;

    private static ApkDownloader downloader;

    private static IPackageDownloadListener eventListener;

    private static ProgressDialog prepareProgress;

    private APKDownloadTask downloadTask;

    private final String strApkURL;

    private File outputFile;

    private final long deviceFreeBytes;

    private long contentLength;

    private String lastModified;

    private String localLastModified;

    private final String updateTitle;

    private final String updateMessage;

    private CLGamePacketFragment dialog;

    private boolean exitGame;

    public ApkDownloader(String title, String conetnt, String strApkURL, File outputFile, long freeBytes) {
        this.updateTitle = title;
        this.updateMessage = conetnt;
        this.strApkURL = strApkURL;
        this.outputFile = outputFile;
        this.deviceFreeBytes = freeBytes;
    }

    public static void init(Activity context) {
        init(context, null);
    }

    public static void init(Activity context, IPackageDownloadListener listener) {
        currentActivity = context;
        packageName = currentActivity.getApplicationContext().getPackageName();
        eventListener = listener;
        if(eventListener == null) {
            eventListener = new IPackageDownloadListener() {
                @Override
                public void onEvent(int type, Object... args) {
                    Log.d("cilu_packet_event", " event :" + type + ", " + Arrays.toString(args));
                }
            };
        }
    }

    public static ApkDownloader getCurrent() {
        return downloader;
    }
    
    /**
     * 初始化设置当前上下文(UnityPlayer.currentActivity)
     */
    public static void init() {
    	currentActivity = UnityPlayer.currentActivity;
        init(UnityPlayer.currentActivity, new IPackageDownloadListener() {
			@Override
			public void onEvent(int type, Object... args) {
				// evtType 参考: IPackageDownloadListener.EVENT_TYPE_*
				final int evtType = type;
				Log.i("cilu_packet_event", " event :" + evtType + ", " + Arrays.toString(args));
				UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
					@Override
					public void run() {
						UnityCallbackWrapper.SendToUnity("OnFullPacketUpdate", ""+evtType);
					}
				});
			}
		});
    }

    /**
     * 打开整包更新界面 (玩家退出界面时退出游戏)
     * 
     * @param title 更新标题(可以在后面带上版本号)
     * @param message 更新内容
     * @param strApkURL 要下载的包链接
    public static void download(final String title, final String message, final String strApkURL) {
        download(title, message, strApkURL, true);
    }
    */

    /**
     * 打开整包更新界面
     * 
     * @param title 更新标题(可以在后面带上版本号)
     * @param message 更新内容
     * @param strApkURL 要下载的包链接
     * @param exitGame true:玩家退出界面时退出游戏, false:玩家退出更新界面时返回游戏
     */
    public static void download(final String title, final String message, final String strApkURL, boolean exitGame) {
        if (currentActivity == null) {
           // Log.e(TAG, "current activity not set!");
           // return;
        	init();
        }
        if (downloader != null) {
            downloader.cancel();
            downloader = null;
        }

        if(prepareProgress != null) {
            prepareProgress.dismiss();
        }
        prepareProgress = new ProgressDialog(currentActivity);
        prepareProgress.setMessage("正在加载文件信息,请稍后");
        prepareProgress.setIndeterminate(true);
        prepareProgress.show();

        File downToDir = null;
        boolean externalStorage = false;
        if (Environment.MEDIA_MOUNTED.equalsIgnoreCase(Environment.getExternalStorageState())) {
            downToDir = Environment.getExternalStorageDirectory();
            externalStorage = true;
        } else {
            downToDir = Environment.getDataDirectory();
        }
        long freeBytes = (long) (megabytesAvailable(downToDir) * IOUtils.ONE_MB);
        Log.d(TAG, "check free space: " + downToDir + ", size: " + IOUtils.byteCountToDisplaySize(freeBytes) + ", can write:" + downToDir.canWrite());

        String apkFileName = getFileName(strApkURL);
        if (apkFileName == null || apkFileName.length()<=0 || !apkFileName.endsWith(".apk")) {
            apkFileName = packageName + ".apk";
        }
        File outputDir = new File(downToDir, externalStorage ? packageName + "/apk" : "/apk");
        if (!outputDir.exists()) {
            boolean createDirSuccess = outputDir.mkdirs();
            if (!createDirSuccess) {
                Log.e(TAG, "create output dir fail: " + outputDir);
            }
        }
        File apkFile = new File(outputDir, apkFileName);
        Log.d(TAG, "will download file to:" + apkFile);

        downloader = new ApkDownloader(title, message, strApkURL, apkFile, freeBytes);
        downloader.setExitGame(exitGame);
        downloader.localInfoCheck();
        downloader.remoteFileCheck();
    }

    public void showDownload() {
        dialog = new CLGamePacketFragment();
        dialog.setDownloader(this);
        dialog.show(currentActivity.getFragmentManager(), "cl_packet_dialog");
    }

    public void start() {
        // 剩余空间检查
        if (contentLength > deviceFreeBytes) {
            AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
            builder.setTitle("空间不足");
            builder.setMessage("您的手机空间不足, 至少需要(" + IOUtils.byteCountToDisplaySize(contentLength) + "), 当前空间(" + IOUtils.byteCountToDisplaySize(deviceFreeBytes) + ")");
            builder.create().show();
        } else {
            downloadTask = new APKDownloadTask(currentActivity, dialog, strApkURL, outputFile, downloader.getContentLength());
            downloadTask.execute();
        }
    }

    public boolean isTaskRunning() {
        return downloadTask != null && downloadTask.getStatus() == AsyncTask.Status.RUNNING;
    }

    public void pause() {
        if (downloadTask != null) {
            downloadTask.cancel();
        }
    }

    private void cancel() {
        if (downloadTask != null) {
            downloadTask.cancel();
        }
        if (dialog != null) {
            dialog.dismiss();
        }
    }

    public File getOutputFile() {
        return outputFile;
    }

    public long getContentLength() {
        return contentLength;
    }

    public long getDeviceFreeBytes() {
        return deviceFreeBytes;
    }

    public String getUpdateTitle() {
        return updateTitle;
    }

    public String getUpdateMessage() {
        return updateMessage;
    }

    public CLGamePacketFragment getDialog() {
        return dialog;
    }

    public void setDialog(CLGamePacketFragment dialog) {
        this.dialog = dialog;
        if(downloadTask != null) {
            downloadTask.setProgressDialog(dialog);
        }
    }

    public boolean isExitGame() {
        return exitGame;
    }

    public void setExitGame(boolean exitGame) {
        this.exitGame = exitGame;
    }

    private void localInfoCheck() {
        String key = HttpUtils.strURLWithoutParams(strApkURL);
        SharedPreferences sharedPref = currentActivity.getPreferences(Context.MODE_PRIVATE);
        if (sharedPref.contains(key)) {
            localLastModified = sharedPref.getString(key, null);
        }
    }

    private void writeLocalInfo() {
        try {
            String key = HttpUtils.strURLWithoutParams(strApkURL);
            SharedPreferences sharedPref = currentActivity.getPreferences(Context.MODE_PRIVATE);
            SharedPreferences.Editor editor = sharedPref.edit();
            editor.putString(key, lastModified);
            editor.commit();
        } catch (Exception e) {
            Log.e(TAG, "write local info error:" + strApkURL + ", " + lastModified);
        }
    }

    private void remoteFileCheck() {
        new Thread(new Runnable() {
            @Override
            public void run() {
                Map<String, List<String>> headers = HttpUtils.head(strApkURL);
                boolean success = false;
                if (headers != null) {
                    try {
                        contentLength = Long.parseLong(headers.get("Content-Length").get(0));
                        lastModified = headers.get("Last-Modified").get(0);
                        success = true;
                    } catch (Exception e) {
                        Log.e(TAG, "parse header err:" + headers, e);
                    }
                }
                if(prepareProgress != null) {
                    prepareProgress.dismiss();
                }
                onRemoteFileCheck(success);
            }
        }).start();
    }

    public void onRemoteFileCheck(final boolean success) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (success) {
                    if (outputFile.exists() && !lastModified.equalsIgnoreCase(localLastModified)) {
                        outputFile.delete();
                        outputFile = new File(outputFile.getPath());
                    }
                    writeLocalInfo();
                    showDownload();
                } else {
                    showCheckErrorTip();
                }
            }
        });
    }

    private void showCheckErrorTip() {
        AlertDialog.Builder builder = new AlertDialog.Builder(currentActivity);
        builder.setTitle("提示");
        builder.setMessage("获取远程文件信息失败, 请稍后再试");
        builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                remoteFileCheck();
            }
        });
        builder.setNegativeButton("取消", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    public static String getFileName(String strURL) {
        String fileName = strURL.substring(strURL.lastIndexOf('/') + 1);
        if (fileName.indexOf("?") >= 0) {
            fileName = fileName.substring(0, fileName.indexOf("?"));
        }
        return fileName;
    }

    public static float megabytesAvailable(File f) {
        StatFs stat = new StatFs(f.getPath());
        long bytesAvailable = 0;
        //if(Build.VERSION.SDK_INT >= 18) {
        //    bytesAvailable = stat.getBlockSizeLong() * stat.getAvailableBlocksLong();
        //} else {
            bytesAvailable = stat.getBlockSize() * stat.getAvailableBlocks();
        //}
        return bytesAvailable / (1024.f * 1024.f);
    }

    public void publishEvent(int type, Object... args) {
        if(eventListener != null) {
            eventListener.onEvent(type, args);
        }
    }
}
