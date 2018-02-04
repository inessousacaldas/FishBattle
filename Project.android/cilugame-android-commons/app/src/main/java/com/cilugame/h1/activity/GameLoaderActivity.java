package com.cilugame.h1.activity;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.DialogInterface.OnKeyListener;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.content.res.AssetManager;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.cilugame.android.commons.HttpUtils;
import com.cilugame.android.commons.IOUtils;
import com.cilugame.h1.patch.DLLDownloadTask;
import com.cilugame.h1.patch.DLLDownloadTip;
import com.cilugame.h1.patch.FileInfo;
import com.cilugame.h1.patch.PatchUtils;

public class GameLoaderActivity extends Activity {

    public static final String TAG = "GameLoaderActivity";

    public static final String SPLASH_PATH = "splash.png";
    public static final String FILES_DLL_DIR = "dlls";
    public static final String ASSETS_DLL_DIR = "bin/Data/Managed";
    public static final String DLL_VERSION_FILE_NAME = "version.json";
    public static final String DLL_VERSION_FILE_ENCODING = "UTF-8";
    private static final String METADATA_GAME_ACTIVITY_CLASS_NAME = "gameActivityClass";

    private TextView txtLoadTip;

    private ProgressBar pbLoader;

    private JSONObject localVersionInfo;

    private JSONObject remoteVersionInfo;

    private File dllsDir;

    private AssetManager assetManager;

    private DLLDownloadTask downloadTask;

    private boolean forceUpdate = true;

    private final Handler handler = new Handler() {
        @Override
        public void handleMessage(Message msg) {
            Log.i(TAG, msg.what + "" + msg.obj);
            if (DLLDownloadTip.MSG_VERSION_DOWNLOAD == msg.what) {
                String strJson = (String) msg.obj;
                if (strJson == null || strJson.isEmpty()) {
                    Log.e(TAG, "remote version not available! ");
                    txtLoadTip.setText("版本文件下载失败!");
                    versionRemoteRetry();
                    return;
                }
                try {
                    remoteVersionInfo = new JSONObject(strJson);
                } catch (JSONException e) {
                    Log.e(TAG, "parse remote version error: " + strJson, e);
                    txtLoadTip.setText("版本信息错误!");
                }
                if(remoteVersionInfo == null) {
                    versionRemoteRetry();
                    return;
                }
                versionsCompare();
            } else if (DLLDownloadTip.MSG_DLL_DOWNLOAD_FILE == msg.what) {
                txtLoadTip.setText((String) msg.obj);
            } else if (DLLDownloadTip.MSG_DLL_DOWNLOAD_FINISH == msg.what) {
                txtLoadTip.setText("程序更新成功!");
                try {
                    saveNewVersion(new File(dllsDir, DLL_VERSION_FILE_NAME), remoteVersionInfo.toString());
                } catch (Exception e) {
                    Log.e(TAG, "save remote version file error:", e);
                }
                startGameActivity(true);
            } else if(DLLDownloadTip.ERR_DLL_DOWNLOAD_FAIL == msg.what) {
                dllDownloadRetry();
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        this.assetManager = getAssets();

        final String appPackageName = getApplicationContext().getPackageName();
        // R.layout.cilu_activity_game_loader
        final int rLayoutId = getResources().getIdentifier("cilu_activity_game_loader", "layout", appPackageName);
        // R.id.cilu_gameLoaderLayout
        final int rGameLoaderLayoutId = getResources().getIdentifier("cilu_gameLoaderLayout", "id", appPackageName);
        // R.id.cilu_game_loader_txtLoadTip
        final int rTxtLoadTip = getResources().getIdentifier("cilu_game_loader_txtLoadTip", "id", appPackageName);
        // R.id.cilu_game_loader_pbLoader
        final int rProgressbarLoader = getResources().getIdentifier("cilu_game_loader_pbLoader", "id", appPackageName);
        setContentView(rLayoutId);

        View view = findViewById(rGameLoaderLayoutId);
        view.setBackgroundColor(Color.BLACK);

        txtLoadTip = (TextView) findViewById(rTxtLoadTip);
        txtLoadTip.setTextColor(Color.WHITE);
        pbLoader = (ProgressBar) findViewById(rProgressbarLoader);
        pbLoader.setVisibility(View.INVISIBLE);

        // 检查网络设置
        networkCheck();
    }

    private void SetSplash()
    {
        final String appPackageName = getApplicationContext().getPackageName();
        // R.id.cilu_gameLoaderLayout
        final int rGameLoaderLayoutId = getResources().getIdentifier("cilu_gameLoaderLayout", "id", appPackageName);

        View view = findViewById(rGameLoaderLayoutId);
        Drawable bgDrawable = null;
        try {
            bgDrawable = Drawable.createFromStream(getAssets().open(SPLASH_PATH), null);
        } catch (IOException e) {
            Log.w(TAG, "set loader background error:", e);
        }
        if (bgDrawable != null) {
            if (Build.VERSION.SDK_INT >= 16)
                view.setBackground(bgDrawable);
            else
                view.setBackgroundDrawable(bgDrawable);
        } else {
            view.setBackgroundColor(Color.BLACK);
        }
        if(txtLoadTip != null)
            txtLoadTip.setTextColor(Color.BLACK);
    }

    private void localFileCheck() {
        File filesDir = getFilesDir();
        dllsDir = new File(filesDir, FILES_DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLL_VERSION_FILE_NAME);
        // JSONObject localVersionInfo = null;
        if (dllsDir.exists()) {
            if (dllVersionFile.exists()) {
                try {
                    String strJson = IOUtils.readFileToString(dllVersionFile, DLL_VERSION_FILE_ENCODING);
                    localVersionInfo = new JSONObject(strJson);
                } catch (Exception e) {
                    Log.e(TAG, "read local version.json error: ", e);
                }
            }
        }
        Log.i(TAG, "local version info step1: " + localVersionInfo);
        // DLL未拷贝过到本地目录, 从assets拷出来; 这里还要考虑覆盖安装的处理
        final String assertDllPath = ASSETS_DLL_DIR + "/";
        final JSONObject assetsVersionInfo = getAssetsVersionInfo(assertDllPath);
        final int localV = localVersionInfo == null ? 0 : localVersionInfo.optInt("id");
        final int assetV = assetsVersionInfo == null ? 0 : assetsVersionInfo.optInt("id");
        Log.i(TAG, "local version vs assets version: " + localV + ", " + assetV);
        if (localVersionInfo == null || localV < assetV) {
            if (!dllsDir.exists()) {
                dllsDir.mkdirs();
            }
            localVersionInfo = assetsVersionInfo;
            int fileCount = 0;
            try {
                JSONObject files = localVersionInfo.getJSONObject("files");
                Iterator<String> itKeys = files.keys();
                while (itKeys.hasNext()) {
                    String fileName = itKeys.next();
                    FileOutputStream outputStream = new FileOutputStream(new File(dllsDir, fileName));
                    IOUtils.copy(assetManager.open(assertDllPath + fileName), outputStream);
                    IOUtils.closeQuietly(outputStream);
                    fileCount++;
                }
                saveNewVersion(new File(dllsDir, DLL_VERSION_FILE_NAME), localVersionInfo.toString());
            } catch (Exception e) {
                Log.e(TAG, "copy assets version.json error: ", e);
            }
            Log.i(TAG, "finish copy dll file from assets, file count: " + fileCount);
        }
        Log.i(TAG, "local version info step2: " + localVersionInfo);
        if (localVersionInfo == null) {
            startGameActivity(false);
            return;
        }
        versionRemoteCheck();
    }

    private void networkCheck() {
        if (!HttpUtils.isNetworkAvailable(this)) {
            final AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.setTitle("网络异常");
            builder.setMessage("当前网络不可用，请检查你的网络设置");
            builder.setCancelable(false);
            builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    dialog.dismiss();
                    networkCheck();
                }
            });
            builder.create().show();
        } else {
            // 检查DLL文件是不是拷贝过了
            localFileCheck();
        }
    }

    private JSONObject getAssetsVersionInfo(String assertDllPath) {
        try {
            String strJson = IOUtils.toString(assetManager.open(assertDllPath + DLL_VERSION_FILE_NAME), DLL_VERSION_FILE_ENCODING);
            return new JSONObject(strJson);
        } catch (Exception e) {
            Log.e(TAG, "get assets version error:", e);
        }
        return null;
    }

    private void saveNewVersion(File newVersionFile, String strJson) throws Exception {
        IOUtils.writeStringToFile(newVersionFile, strJson, DLL_VERSION_FILE_ENCODING);
    }

    private void versionRemoteCheck() {
        final String appPackageName = getApplicationContext().getPackageName();
        txtLoadTip.setText("正在检测游戏版本");
        final String remoteVersionURL = localVersionInfo.optString("remoteVersion");
        Thread remoteThread = new Thread(new Runnable() {
            @Override
            public void run() {
                // 读取网络版本
                String strJson = HttpUtils.get(remoteVersionURL + "?p=" + appPackageName + "&r=" + Math.random(), 10000);
                final Message msg = new Message();
                msg.what = DLLDownloadTip.MSG_VERSION_DOWNLOAD;
                msg.obj = strJson;
                handler.sendMessage(msg);
            }
        });
        remoteThread.start();
    }

    private int versionRetryCount = 0;
    private void versionRemoteRetry() {
        versionRetryCount++;
        boolean showExit = false;
        if(versionRetryCount>10) {
            showExit = true;
            versionRetryCount = 0;
            PatchUtils.restoreLocalVersion(this);
        }
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("版本检查失败");
        builder.setMessage("版本文件下载失败,请检查网络或者重试");
        builder.setCancelable(false);
        builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                versionRemoteCheck();
            }
        });
        if(showExit) {
            final Activity opener = this;
            builder.setNegativeButton("稍后再玩", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    dialog.dismiss();
                    opener.finish();
                }
            });
        }
        builder.create().show();
    }

    private void versionsCompare() {

        // 每次检查本地文件
        final int versionIdDiff = remoteVersionInfo.optInt("id", 0) - localVersionInfo.optInt("id", 0);
        if (versionIdDiff<0) {
            Log.i(TAG, "remote version less than local version!");
            // goto game activity!
            startGameActivity(false);
            return;
        }

        // 找出差异文件
        // final JSONObject localFilesJson = localVersionInfo.optJSONObject("files");
        final JSONObject filesJson = remoteVersionInfo.optJSONObject("files");
        if (filesJson == null || filesJson.length() <= 0) {
            // goto game activity!
            startGameActivity(false);
            return;
        }

        final String downloadURLPrefix = localVersionInfo.optString("downloadURLPrefix");
        final String remoteFileSuffix = remoteVersionInfo.optString("suffix", "dll");
        final boolean zip = remoteVersionInfo.has("suffix");
        final List<FileInfo> diffFileURLs = new ArrayList<FileInfo>(5);
        final Iterator<String> itKeys = filesJson.keys();
        long totalSize = 0;
        while (itKeys.hasNext()) {
            String fileName = itKeys.next();
            // JSONObject localFileInfo = localFilesJson.optJSONObject(fileName);
            File file = new File(dllsDir,fileName);
            JSONObject remoteFileInfo = filesJson.optJSONObject(fileName);
            String localFileVersion = IOUtils.md5Hex(file); // localFileInfo.optString("md5");
            String remoteFileVersion = remoteFileInfo.optString("md5");
            if (!remoteFileVersion.equalsIgnoreCase(localFileVersion)) {
                String remoteFileName = fileName.substring(0, fileName.lastIndexOf('.')) + "." + remoteFileSuffix;
                String strURL = downloadURLPrefix + "/" + remoteFileName + "?v=" + remoteFileVersion;
                FileInfo fileInfo = new FileInfo(fileName,strURL, file, remoteFileVersion);
                diffFileURLs.add(fileInfo);
                int fileSize = remoteFileInfo.optInt("size");
                if(zip)
                    fileSize = remoteFileInfo.optInt("zipSize", fileSize);
                totalSize += fileSize;
            }
        }
        forceUpdate = remoteVersionInfo.optBoolean("forceUpdate", false);
        Log.i(TAG, "need download files: " + diffFileURLs);
        if (diffFileURLs.isEmpty()) {
            // goto game activity!
            startGameActivity(false);
            return;
        }
        SetSplash(); //显示启动页
        if(versionIdDiff == 0) {
            // 如果版本相等,但有文件要更新,表示本地文件不正确,要强制更新
            forceUpdate = true;
        }
        checkFreeSpaces(diffFileURLs, totalSize, zip);
    }

    private void checkFreeSpaces(List<FileInfo> diffFileURLs, long totalSize, boolean zip) {
        // 检查剩余空间大小
        float freeSpaceInBytes = IOUtils.freeBytes(Environment.getDataDirectory());
        if(freeSpaceInBytes < totalSize) {
            // 空间不足
            Log.i(TAG, "剩余空间不足:" + freeSpaceInBytes + ", " + IOUtils.byteCountToDisplaySize(freeSpaceInBytes));
            showFreeSpaceNotEnough(diffFileURLs, totalSize, zip);
        } else {
            updatePatchConfirm(diffFileURLs, dllsDir, totalSize, zip);
        }
    }

    private void showFreeSpaceNotEnough(final List<FileInfo> diffFileURLs, final long totalSize, final boolean zip) {
        final Activity opener = this;
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage("您的设备剩余空间不足, 下载文件大小: " + IOUtils.byteCountToDisplaySize(totalSize));
        builder.setTitle("空间不足");
        builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                checkFreeSpaces(diffFileURLs, totalSize, zip);
            }
        });
        builder.setNegativeButton("稍后再玩", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                opener.finish();
            }
        });
        builder.setOnKeyListener(new OnKeyListener() {
            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // 不能用返回键取消
                return false;
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    protected void updatePatchConfirm(final List<FileInfo> diffFileURLs, final File outputDir, long totalSize, final boolean zip) {
        StringBuilder tip = new StringBuilder("需要更新" + diffFileURLs.size() + "个文件");
        if (totalSize > 0) {
            tip.append(", 大小: " + IOUtils.byteCountToDisplaySize(totalSize));
        }
        tip.append("，建议WIFI环境下载");
        txtLoadTip.setText(tip);
        pbLoader.setMax((int) totalSize);
        pbLoader.setProgress(0);
        pbLoader.setVisibility(View.VISIBLE);

        final Activity opener = this;
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage(tip);
        builder.setTitle("确认下载");
        builder.setPositiveButton("好!", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                downloadTask = new DLLDownloadTask(opener, handler, pbLoader, diffFileURLs, outputDir, zip);
                downloadTask.execute();
            }
        });
        String cancelButtonName = forceUpdate ? "稍后再玩" : "暂不下载";
        builder.setNegativeButton(cancelButtonName, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                if (forceUpdate) {
                    opener.finish();
                } else {
                    // goto game activity!
                    startGameActivity(false);
                }
            }
        });
        builder.setOnKeyListener(new OnKeyListener() {
            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // 不能用返回键取消
                return false;
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    private void dllDownloadRetry() {
        final Activity opener = this;
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage("文件下载失败,请检查网络设置或者重试");
        builder.setTitle("下载失败");
        builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                if(downloadTask != null) {
                    downloadTask = downloadTask.clone();
                }
                downloadTask.execute();
            }
        });
        builder.setNegativeButton("稍后再玩", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                opener.finish();
            }
        });
        builder.setOnKeyListener(new OnKeyListener() {
            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // 不能用返回键取消
                return false;
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    private void startGameActivity(boolean patchUpdated) {
        this.txtLoadTip.setText("正在进入游戏");
        this.pbLoader.setVisibility(View.INVISIBLE);
        String startGameActivityName = null;
        try {
            ActivityInfo activityInfo = getPackageManager().getActivityInfo(this.getComponentName(), PackageManager.GET_ACTIVITIES | PackageManager.GET_META_DATA);
            startGameActivityName = activityInfo.metaData.getString(METADATA_GAME_ACTIVITY_CLASS_NAME);
        } catch (PackageManager.NameNotFoundException e) {
            Log.e(TAG, "get metadata error: ", e);
        }
        if (startGameActivityName != null) {
            try {
                Intent oldIntent = getIntent();
                Log.i(TAG, "src intent: " + oldIntent);
                final Class<?> activityClass = Class.forName(startGameActivityName);
                Intent gameIntent = new Intent(this, activityClass);
                gameIntent.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
                gameIntent.putExtra("patchUpdated", patchUpdated);
                gameIntent.putExtras(oldIntent);
                startActivity(gameIntent);
                this.finish();
            } catch (ClassNotFoundException e) {
                Log.e(TAG, "Class not found: " + startGameActivityName, e);
            }
        } else {
            Toast.makeText(this, "无法进入游戏,请设置游戏页面类", Toast.LENGTH_LONG).show();
            Log.e(TAG, "get null game activity class name!");
        }
    }
}
