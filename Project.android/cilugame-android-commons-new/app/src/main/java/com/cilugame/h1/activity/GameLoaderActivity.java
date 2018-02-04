package com.cilugame.h1.activity;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
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
import android.view.Window;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.cilugame.android.commons.DeviceUtils;
import com.cilugame.android.commons.HttpUtils;
import com.cilugame.h1.patch.DLLDownloadTask;
import com.cilugame.h1.patch.DLLDownloadTip;
import com.cilugame.android.commons.IOUtils;
import com.cilugame.h1.patch.FileInfo;
import com.cilugame.h1.patch.PatchUtils;
import com.demiframe.game.api.util.LogUtil;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;


public class GameLoaderActivity extends Activity {
    public static final String TAG = "GameLoaderActivity";
    public static final String SPLASH_PATH = "bin/Data/splash.png";

    public static final String CDN_VERSION_FILE = "versionConfig_0.json";
    public static final String DLL_DIR = "dlls";
    public static final String DLLVERSION_FILENAME = "dllVersion.json";
    public static final String DLL_VERSION_FILE_ENCODING = "UTF-8";
    public static final String ASSETS_DLL_DIR = "bin/Data/Managed";
    public static final String DLL_VERSION_ROOT = "dllversion";
    public static final String DLL_FILE_ROOT = "dll";
    public static final String CDN_SERVER_CONFIG = "cdnServerConfig.json";
    private File dllsDir;
    private JSONObject localDllVersionInfo;

    private JSONObject remoteDllVersion;

    //cdn服务器列表
    private JSONObject assetsCdnServerInfo;
    private JSONArray assetsCdnServerList;

    private boolean forceUpdate = true;

    private TextView loadingTips;

    private ProgressBar progressBar;
    private static Activity instance;

    private final Handler handler = new Handler(){
        @Override
        public void handleMessage(Message msg){
            Log.i(TAG, msg.what + "" + msg.obj);
            if(DLLDownloadTip.MSG_CDNVERSION_DOWNLOAD == msg.what){
                OnDownLoadCdnVersionFinish((String)msg.obj);
            }
            else if(DLLDownloadTip.MSG_DLLVERSION_DOWNLOAD == msg.what){
                OnDownLoadCdnDllVersionFinish((String)msg.obj);
            }else if (DLLDownloadTip.MSG_DLL_DOWNLOAD_FILE == msg.what) {
                loadingTips.setText((String) msg.obj);
            }
            else if(DLLDownloadTip.ERR_DLL_DOWNLOAD_FAIL == msg.what) {
                dllDownloadRetry();
            }
            //更新完成
            else if(DLLDownloadTip.MSG_DLL_DOWNLOAD_FINISH == msg.what){
                loadingTips.setText("程序更新成功!");
                try {
                    saveNewVersion(new File(dllsDir, DLLVERSION_FILENAME), remoteDllVersion.toString());
                } catch (Exception e) {
                    Log.e(TAG, "save remote version file error:", e);
                }
                startGameActivity(true);
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState){
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);
        instance = this;

        final String appPackageName = getApplicationContext().getPackageName();
        final int rLayoutId = getResources().getIdentifier("cilu_activity_game_loader", "layout", appPackageName);
        final int rGameLoaderLayoutId = getResources().getIdentifier("cilu_gameLoaderLayout", "id", appPackageName);
        final int rTxtLoadTip = getResources().getIdentifier("cilu_game_loader_txtLoadTip", "id", appPackageName);
        final int rProgressbarLoader = getResources().getIdentifier("cilu_game_loader_pbLoader", "id", appPackageName);
        setContentView(rLayoutId);

        View view = findViewById(rGameLoaderLayoutId);
        view.setBackgroundColor(Color.BLACK);

        loadingTips = (TextView) findViewById(rTxtLoadTip);
        loadingTips.setTextColor(Color.WHITE);

        progressBar = (ProgressBar) findViewById(rProgressbarLoader);
        progressBar.setVisibility(View.INVISIBLE);

        isAlwaysShowSplash = IsAlwaysShowSplash();

        //显示闪屏
        if(isAlwaysShowSplash){
            ShowSplash();
        }
        //初始化本地版本文件
        if(CheckLocalFile()){
            // 检查网络设置,检查更新
            CheckUpdate();
        }
        else{
            startGameActivity(false);
        }
    }

    //特殊处理，是否处理平滑过渡更新闪屏->游戏闪屏。（手盟有某些判断，不能做此优化）
    private boolean isAlwaysShowSplash = true;
    private boolean IsAlwaysShowSplash(){
        String value = "";
        try {
            ApplicationInfo appInfo = getPackageManager().getApplicationInfo(getPackageName(), PackageManager.GET_META_DATA);
            value = appInfo.metaData.get("DemiFrameChannelId").toString();
            return value.equals("demi");
        } catch (Exception e) {
            Log.d(TAG, "DemiFrameChannelId read error");
        }
        return false;
    }

    private void ShowSplash(){
        final String appPackageName = getApplicationContext().getPackageName();
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

        if(loadingTips != null)
            loadingTips.setTextColor(Color.BLACK);
    }

    private void CheckUpdate() {
        if (!HttpUtils.isNetworkAvailable(this)) {
            final AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.setTitle("网络异常");
            builder.setMessage("当前网络不可用，请检查你的网络设置");
            builder.setCancelable(false);
            builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    dialog.dismiss();
                    CheckUpdate();
                }
            });
            builder.create().show();
        } else {
            cdnVersionCheck();
        }
    }

    //当前使用cdn的index
    private int dllVersionIndex = 0;
    private String curCdnUrl = "";

    private String getRealCdnUrl(String cdnUrl){
        return cdnUrl + "/" + assetsCdnServerInfo.optString("domainType") + "/android/staticRes";
    }

    private boolean IsReleaseType(){
        if(assetsCdnServerInfo == null)
            return false;

        try{
            return assetsCdnServerInfo.optString("domainType").equals("release");
        }catch (Exception e){
            Log.d(TAG, "domainType parse error");
        }
        return false;
    }

    private void cdnVersionCheck(){
        if(assetsCdnServerList == null){
            Log.e(TAG, "cdnVersionCheck errors, serverList is null");
            return;
        }

        if(assetsCdnServerList.length()<= dllVersionIndex){
            cdnVersionRetry();
            return;
        }
        String cdnUrl;
        try{
            cdnUrl = assetsCdnServerList.getString(dllVersionIndex);
        }catch(Exception e){
            loadingTips.setText("版本文件解析错误");
            Log.e(TAG, "dllVersionCheck errors, serverList index null");
            return;
        }
        cdnUrl = getRealCdnUrl(cdnUrl);

        Log.i(TAG, "use cdnServer index= " + dllVersionIndex);

        dllVersionIndex++;
        curCdnUrl = cdnUrl;
        final String versionUrl = cdnUrl + "/" + CDN_VERSION_FILE + "?ver=" + System.currentTimeMillis();
        Thread remoteThread = new Thread(new Runnable() {
            @Override
            public void run() {
                // 读取cdn网络版本
                String strJson = HttpUtils.get(versionUrl, 10000);
                final Message msg = new Message();
                msg.what = DLLDownloadTip.MSG_CDNVERSION_DOWNLOAD;
                msg.obj = strJson;
                handler.sendMessage(msg);
            }
        });
        remoteThread.start();
    }

    private void OnDownLoadCdnVersionFinish(String jsonStr){
        if(jsonStr == null || jsonStr.isEmpty()){
            //下载失败，换一个cdn服务器继续
            cdnVersionCheck();
            return;
        }

        JSONObject remoteCdnVersionInfo = null;
        try {
            remoteCdnVersionInfo = new JSONObject(jsonStr);
        } catch (JSONException e) {
            Log.e(TAG, "parse remoteCdn version error: " + jsonStr, e);
            loadingTips.setText("版本信息错误!");
        }
        //解析失败
        if(remoteCdnVersionInfo == null) {
            cdnVersionCheck();
            return;
        }

        JSONArray remoteCdnServerList = null;
        try
        {
            remoteCdnServerList = remoteCdnVersionInfo.getJSONArray("cdnServerList");
        }catch (Exception e){
            //允许远程服务器列表为空的情况
            Log.i(TAG, "remoteCdnServerInfo is null: ");
        }
        Log.i(TAG, "remoteCdnServerInfo="+remoteCdnServerList);

        if(remoteCdnServerList != null && remoteCdnServerList.toString() != assetsCdnServerList.toString()){
            Log.i(TAG, "remoteCdnServerList update " + remoteCdnServerList);
            try{
                saveNewVersion(new File(dllsDir, CDN_SERVER_CONFIG), assetsCdnServerInfo.toString());
            }catch (Exception e){
                Log.e(TAG, "remoteCdnServerList update save errors" + e);
            }
        }

        final long localVersion = localDllVersionInfo.optLong("Version");
        final long remoteVersion = remoteCdnVersionInfo.optLong("dllVersion");
        //大于服务器上的版本号都跳过，等于原则上应该不跳过的，但是服务器上的dll记录的md5是压缩后的，这样需要多进行很多消耗判断
        //变更了游戏服类型后,可能需要回退版本,这个未处理。
        if(localVersion >= remoteVersion){
            Log.i(TAG, "remote version less than local version!");
            // goto game activity!
            startGameActivity(false);
            return;
        }

        forceUpdate = remoteCdnVersionInfo.optBoolean("forceUpdate");
//        if(localVersion == remoteVersion){
//            // 如果版本相等,但有文件要更新,表示本地文件不正确,要强制更新
//            forceUpdate = true;
//        }

        //显示闪屏
        if(!isAlwaysShowSplash){
            ShowSplash();
        }

        //下载dll版本文件
        final String versionUrl = curCdnUrl + "/" + DLL_VERSION_ROOT + "/" + "dllVersion_" + remoteVersion + ".json";
        Thread remoteThread = new Thread(new Runnable() {
            @Override
            public void run() {
                // 读取cdn网络版本
                String strJson = HttpUtils.get(versionUrl, 10000);
                final Message msg = new Message();
                msg.what = DLLDownloadTip.MSG_DLLVERSION_DOWNLOAD;
                msg.obj = strJson;
                handler.sendMessage(msg);
            }
        });
        remoteThread.start();
    }

    //dll版本文件下载失败
    private void OnDownLoadCdnDllVersionFinish(String jsonStr){
        if(jsonStr == null || jsonStr.isEmpty()){
            //直接提示重新来过
            cdnVersionRetry();
            return;
        }

        remoteDllVersion = null;
        try{
            remoteDllVersion = new JSONObject(jsonStr);
        }catch (Exception e){
            Log.e(TAG, "remoteDllVersion parse error: ", e);
            loadingTips.setText("版本信息错误!");
        }

        if(remoteDllVersion == null){
            //直接提示重新来过
            cdnVersionRetry();
            return;
        }

        final JSONObject filesJson = remoteDllVersion.optJSONObject("Manifest");
        if(filesJson == null || filesJson.length() <= 0){
            // goto game activity!
            Log.i(TAG, "update dll list is empty");
            startGameActivity(false);
            return;
        }

        final List<FileInfo> diffFileURLs = new ArrayList<FileInfo>();
        final Iterator<String> itKeys = filesJson.keys();
        long totalSize = 0;
        while (itKeys.hasNext()) {
            String sKey = itKeys.next();
            // JSONObject localFileInfo = localFilesJson.optJSONObject(fileName);
            JSONObject remoteFileInfo = filesJson.optJSONObject(sKey);
            String fileName = remoteFileInfo.optString("dllName");
            File file = new File(dllsDir, fileName+".dll");
            String localMD5 = IOUtils.md5Hex(file); // localFileInfo.optString("md5");
            String remoteMD5 = remoteFileInfo.optString("MD5");
            if (!remoteMD5.equalsIgnoreCase(localMD5)) {
                String strURL = curCdnUrl + "/" + DLL_FILE_ROOT  + "/" + fileName + "_" + remoteMD5 + ".dll";
                FileInfo fileInfo = new FileInfo(fileName,strURL, file, remoteMD5);
                diffFileURLs.add(fileInfo);
                int fileSize = remoteFileInfo.optInt("size");
                totalSize += fileSize;
            }
        }

        Log.i(TAG, "need download files: " + diffFileURLs);
        if (diffFileURLs.isEmpty()) {
            // goto game activity!
            startGameActivity(false);
            return;
        }

        checkFreeSpaces(diffFileURLs, totalSize, true);
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
        builder.setOnKeyListener(new DialogInterface.OnKeyListener() {
            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // 不能用返回键取消
                return false;
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    private int versionRetryCount = 0;
    //cdn版本文件出错
    private void cdnVersionRetry(){
        curCdnUrl = "";
        dllVersionIndex = 0;

        versionRetryCount++;
        boolean showExit = false;
        if(versionRetryCount>5) {
            showExit = true;
            versionRetryCount = 0;
            //成功后才写入，不需要备份
            //PatchUtils.restoreLocalVersion(this);
        }

        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("版本检查失败");
        builder.setMessage("版本文件下载失败,请检查网络或者重试");
        builder.setCancelable(false);
        builder.setPositiveButton("重试", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                cdnVersionCheck();
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

    private DLLDownloadTask downloadTask;
    protected void updatePatchConfirm(final List<FileInfo> diffFileURLs, final File outputDir, long totalSize, final boolean zip) {
        StringBuilder tip = new StringBuilder("需要更新" + diffFileURLs.size() + "个文件");
        if (totalSize > 0) {
            tip.append(", 大小: " + IOUtils.byteCountToDisplaySize(totalSize));
        }
        progressBar.setMax((int) totalSize);
        progressBar.setProgress(0);
        progressBar.setVisibility(View.VISIBLE);

        //wifi环境下直接更新
        if(DeviceUtils.getNetworkType(this) == DeviceUtils.NETWORK_TYPE_WIFI){
            loadingTips.setText(tip);
            downloadTask = new DLLDownloadTask(this, handler, progressBar, diffFileURLs, outputDir, zip);
            downloadTask.execute();
            return;
        }

        tip.append("，建议WIFI环境下载");
        loadingTips.setText(tip);

        final Activity opener = this;
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage(tip);
        builder.setTitle("确认下载");
        builder.setPositiveButton("好!", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                downloadTask = new DLLDownloadTask(opener, handler, progressBar, diffFileURLs, outputDir, zip);
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
        builder.setOnKeyListener(new DialogInterface.OnKeyListener() {
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
        builder.setOnKeyListener(new DialogInterface.OnKeyListener() {
            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // 不能用返回键取消
                return false;
            }
        });
        builder.setCancelable(false);
        builder.create().show();
    }

    //初始化版本信息，以及检测包内资源拷贝到包外，需要考虑覆盖安装的问题
    //保证把包内资源拷贝到包外再执行返回
    private boolean CheckLocalFile(){
        File filesDir = getFilesDir();
        dllsDir = new File(filesDir, DLL_DIR);
        File dllVersionFile = new File(dllsDir, DLLVERSION_FILENAME);
        if(dllsDir.exists()){
            if(dllVersionFile.exists()){
                try{
                    String jsonStr = IOUtils.readFileToString(dllVersionFile, DLL_VERSION_FILE_ENCODING);
                    localDllVersionInfo = new JSONObject(jsonStr);
                }catch (Exception e){
                    Log.e(TAG, "read local dllVersion.json error: ", e);
                }
            }
        }
        Log.i(TAG, "local version info step1: " + localDllVersionInfo);

        // DLL未拷贝过到本地目录, 从assets拷出来; 这里还要考虑覆盖安装的处理
        final String assertDllPath = ASSETS_DLL_DIR + "/";
        final JSONObject assetsDllVersionInfo = getAssetJsonObject(DLLVERSION_FILENAME);
        final long localVersion = localDllVersionInfo == null ? 0 : localDllVersionInfo.optLong("Version");
        final long assetsVersion = assetsDllVersionInfo == null ? 0 : assetsDllVersionInfo.optLong("Version");

        assetsCdnServerInfo = getAssetJsonObject(CDN_SERVER_CONFIG);
        if(assetsCdnServerInfo == null){
            Toast.makeText(this, "包内无dll下载版本文件，此包不能当正式包发布",Toast.LENGTH_SHORT).show();
            Log.i(TAG, "包内cdnServerInfo为空,不执行更新");
        }

        if(localDllVersionInfo == null || localVersion < assetsVersion){
            if (!dllsDir.exists()) {
                dllsDir.mkdirs();
            }

            localDllVersionInfo = assetsDllVersionInfo;

            int fileCount = 0;
            try {
                JSONObject files = localDllVersionInfo.getJSONObject("Manifest");
                Iterator<String> itKeys = files.keys();
                while (itKeys.hasNext()) {
                    String fileName = itKeys.next();
                    fileName = fileName + ".dll";
                    FileOutputStream outputStream = new FileOutputStream(new File(dllsDir, fileName));
                    IOUtils.copy(getAssets().open(assertDllPath + fileName), outputStream);
                    IOUtils.closeQuietly(outputStream);
                    fileCount++;
                }

                saveNewVersion(new File(dllsDir, DLLVERSION_FILENAME), localDllVersionInfo.toString());
                //拷贝cdn版本到包外
                if(assetsCdnServerInfo != null){
                    saveNewVersion(new File(dllsDir, CDN_SERVER_CONFIG), assetsCdnServerInfo.toString());
                }

            } catch (Exception e) {
                Log.e(TAG, "copy assets version.json error: ", e);
            }
            Log.i(TAG, "finish copy dll file from assets, file count: " + fileCount);
        }

        if(assetsCdnServerInfo == null){
            return false;
        }

        //不是release版本,不支持更新
        if(!IsReleaseType()){
            Log.d(TAG, "is not release version, no support update dll: ");
            return false;
        }

        //读cdn列表
        JSONObject cdnServerInfo = getFileJsonObject(new File(dllsDir, CDN_SERVER_CONFIG));
        //可能是同版本覆盖安装，出现外部没有cdnServerConfig.json的情况，这里直接使用包内版本
        if(cdnServerInfo == null){
            Log.i(TAG, "assetsCdnServerInfo is null or empty, use assets version");
            cdnServerInfo = assetsCdnServerInfo;
        }
        assetsCdnServerInfo = cdnServerInfo;

        try{
            assetsCdnServerList = assetsCdnServerInfo.getJSONArray("cdnServerList");
        }catch (Exception e){
            Log.i(TAG, "assetsCdnServerList is null or empty");
            return false;
        }

        Log.d(TAG, "check localfile finish localDllVersionInfo=" + localDllVersionInfo);
        return localDllVersionInfo != null;
    }

    private JSONObject getFileJsonObject(File file){
        if(file.exists()){
            try{
                String jsonStr = IOUtils.readFileToString(file, DLL_VERSION_FILE_ENCODING);
                return new JSONObject(jsonStr);
            }catch (Exception e){
                Log.e(TAG, "getFileJsonObject error: ", e);
            }
        }

        return null;
    }

    private void saveNewVersion(File newVersionFile, String strJson) throws Exception {
        IOUtils.writeStringToFile(newVersionFile, strJson, DLL_VERSION_FILE_ENCODING);
    }

    private JSONObject getAssetJsonObject(String path) {
        try {
            String strJson = IOUtils.toString(getAssets().open(path), DLL_VERSION_FILE_ENCODING);
            return new JSONObject(strJson);
        } catch (Exception e) {
            Log.e(TAG, "get assets version error:", e);
        }
        return null;
    }

    private JSONArray getAssetJsonArray(String path) {
        try {
            String strJson = IOUtils.toString(getAssets().open(path), DLL_VERSION_FILE_ENCODING);
            return new JSONArray(strJson);
        } catch (Exception e) {
            Log.e(TAG, "get assets version error:", e);
        }
        return null;
    }

    private static final String METADATA_GAME_ACTIVITY_CLASS_NAME = "gameActivityClass";
    private void startGameActivity(boolean patchUpdated) {
        if(patchUpdated)
        {
            this.loadingTips.setText("正在进入游戏");
            this.progressBar.setVisibility(View.INVISIBLE);
        }

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

                if(!isAlwaysShowSplash){
                    GameLoaderActivity.FinishActivity();
                }
                startActivity(gameIntent);
                overridePendingTransition(0, 0);

                if(isAlwaysShowSplash){
                    GameLoaderActivity.FinishActivity();
                }
            } catch (ClassNotFoundException e) {
                Log.e(TAG, "Class not found: " + startGameActivityName, e);
            }
        } else {
            Toast.makeText(this, "无法进入游戏,请设置游戏页面类", Toast.LENGTH_LONG).show();
            Log.e(TAG, "get null game activity class name!");
        }
    }

    @Override
    protected void onPause(){
        super.onPause();
    }

    public static void FinishActivity(){
        if(instance != null){
            instance.finish();
        }
    }
}
