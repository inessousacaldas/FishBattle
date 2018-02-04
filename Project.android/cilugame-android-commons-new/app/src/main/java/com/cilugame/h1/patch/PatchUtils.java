package com.cilugame.h1.patch;

import android.app.Activity;
import android.app.AlertDialog;
import android.util.Log;
import android.widget.Toast;

import com.cilugame.android.commons.IOUtils;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;

/**
 * 动态更新DLL工具类
 * <p/>
 * Created by Tony on 1/2/16.
 */
public class PatchUtils {

    /**
     * 操作-修改版本检查下载URL
     */
    public static final int OPT_CHANGE_URL = 1;

    /**
     * 操作-修改整个版本内容
     */
    public static final int OPT_REPLACE_CONTENT = 2;

    /**
     * 操作-恢复
     */
    public static final int OPT_RESTORE = 3;

    /**
     * 操作-弹出版本文件内容
     */
    public static final int OPT_SHOW = 4;

    /**
     * 操作-删除DLLs目录
     */
    public static final int OPT_REMOVE_DLLS = 5;

    public static final String TAG = "PatchUtils";
    public static final String FILES_DLL_DIR = "dlls";
    public static final String DLL_VERSION_FILE_NAME = "version.json";
    public static final String DLL_VERSION_FILE_ENCODING = "UTF-8";

    // error code
    public static final int SUCCESS = 0;
    public static final int ERR_VERSION_DIR_NOT_FOUND = 1;
    public static final int ERR_VERSION_FILE_NOT_FOUND = 2;
    public static final int ERR_VERSION_CHANGE = 2;
    public static final int ERR_PARAMS_EMPTY = 3;

    public static int opt(int opt, String content) {
    	Log.e(TAG, "PatchUtils opt=" + opt + " content=" + content);
    	if (opt == OPT_CHANGE_URL) {
            String[] urls = content.split(";");
            return setLocalUpdateURL(urls[0], urls[1]);
        } else if (opt == OPT_REPLACE_CONTENT) {
            return setLocalVersion(content);
        } else if (opt == OPT_RESTORE) {
            restoreLocalVersion();
        } else if (opt == OPT_SHOW) {
            showLocalVersion();
        } else if (opt == OPT_REMOVE_DLLS) {
            removeDlls();
        }
        return SUCCESS;
    }

    private static void removeDlls() {
        File filesDir = getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        if (dllsDir.exists() && dllsDir.isDirectory()) {
            String[] children = dllsDir.list();
            for (int i = 0; i < children.length; i++) {
                new File(dllsDir, children[i]).delete();
            }
            dllsDir.delete();
        }
        Toast.makeText(UnityPlayer.currentActivity, "删除DLL目录成功", Toast.LENGTH_SHORT).show();
    }

    private static void showLocalVersion() {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                new AlertDialog.Builder(UnityPlayer.currentActivity).setMessage(getLocalVersion()).create().show();
            }
        });
    }

    /**
     * 设置本地动态更新地址,参数为空字符串时恢复原来的版本文件
     *
     * @param versionURL
     * @param downURLPrefix
     * @return
     */
    public static int setLocalUpdateURL(String versionURL, String downURLPrefix) {
        if (versionURL == null || versionURL.isEmpty() || downURLPrefix == null || downURLPrefix.isEmpty()) {
            return ERR_PARAMS_EMPTY;
        }
        File filesDir = getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME);
        File backupVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME.replace(".json", ".orig"));

        if (dllsDir.exists()) {
            if (dllVersionFile.exists()) {
                try {
                    // backup file
                    if (!backupVersionFile.exists()) {
                        IOUtils.copy(new FileInputStream(dllVersionFile), new FileOutputStream(backupVersionFile));
                    }
                    String strJson = IOUtils.readFileToString(dllVersionFile, DLL_VERSION_FILE_ENCODING);
                    JSONObject localVersionInfo = new JSONObject(strJson);
                    localVersionInfo.put("remoteVersion", versionURL);
                    localVersionInfo.put("downloadURLPrefix", downURLPrefix);
                    IOUtils.writeStringToFile(dllVersionFile, localVersionInfo.toString(), DLL_VERSION_FILE_ENCODING);
                    return SUCCESS;
                } catch (Exception e) {
                    Log.e(TAG, "change local version.json error: ", e);
                }
                return ERR_VERSION_CHANGE;
            } else {
                return ERR_VERSION_FILE_NOT_FOUND;
            }
        } else {
            return ERR_VERSION_DIR_NOT_FOUND;
        }
    }

    /**
     * 设备本地版本内容
     *
     * @param jsonContent
     * @return
     */
    public static int setLocalVersion(String jsonContent) {
        if (jsonContent == null || jsonContent.isEmpty()) {
            return ERR_PARAMS_EMPTY;
        }
        File filesDir = getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME);
        File backupVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME.replace(".json", ".orig"));

        if (dllsDir.exists()) {
            if (dllVersionFile.exists()) {
                try {
                    // backup file
                    if (!backupVersionFile.exists()) {
                        IOUtils.copy(new FileInputStream(dllVersionFile), new FileOutputStream(backupVersionFile));
                    }
                    IOUtils.writeStringToFile(dllVersionFile, jsonContent, DLL_VERSION_FILE_ENCODING);
                    return SUCCESS;
                } catch (Exception e) {
                    Log.e(TAG, "change local version.json error: ", e);
                }
                return ERR_VERSION_CHANGE;
            } else {
                return ERR_VERSION_FILE_NOT_FOUND;
            }
        } else {
            return ERR_VERSION_DIR_NOT_FOUND;
        }
    }

    public static void restoreLocalVersion() {
        restoreLocalVersion(UnityPlayer.currentActivity);
    }

    /**
     * 恢复原来的版本文件
     */
    public static void restoreLocalVersion(Activity activity) {
        File filesDir = activity != null ? activity.getFilesDir() : getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME);
        File backupVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME.replace(".json", ".orig"));

        // restore version file
        if (dllsDir.exists()) {
            if (backupVersionFile.exists()) {
                try {
                    IOUtils.copy(new FileInputStream(backupVersionFile), new FileOutputStream(dllVersionFile));
                    backupVersionFile.delete();
                } catch (Exception e) {
                    Log.e(TAG, "restore local version.json error: ", e);
                }
            }
        }
    }

    public static String getLocalVersion() {
        File filesDir = getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME);

        // restore version file
        if (dllsDir.exists()) {
            if (dllVersionFile.exists()) {
                try {
                    return IOUtils.readFileToString(dllVersionFile, DLL_VERSION_FILE_ENCODING);
                } catch (Exception e) {
                    Log.e(TAG, "restore local version.json error: ", e);
                }
            }
        }
        return null;
    }

    private static File getFilesDir() {
        return UnityPlayer.currentActivity.getFilesDir();
    }
}
