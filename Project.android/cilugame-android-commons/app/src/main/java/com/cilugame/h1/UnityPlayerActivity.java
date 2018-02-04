package com.cilugame.h1;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;
import com.cilugame.h1.activity.GameLoaderActivity;
import com.cilugame.h1.util.Logger;
import com.cilugame.h1.view.ViewManager;

import java.io.File;

public class UnityPlayerActivity extends Activity {
    public static UnityPlayerActivity instance;
    protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code
    private ViewManager viewManager;
    private static final String ANDROID_DLL_VERSION = "android.dll.version";
    private static final String FILES_DLL_DIR = "dlls";

    public ViewManager GetViewManager() {
        return this.viewManager;
    }
    
    
    public static void quit()
    {
        if (instance != null)
        {
            instance.mUnityPlayer.quit();
        }
    }
    

    private void startUnityPlayerActivity()
    {
        Logger.Log("startUnityPlayerActivity");

        mUnityPlayer = new UnityPlayer(this);
        setContentView(this.mUnityPlayer);
        this.mUnityPlayer.requestFocus();
        
        // AndroidPlugin.SetAssetManager(getAssets());
        // this.viewManager = new ViewManager(this);

          CLSDKPlugin.Setup();
    }

    private void checkNewInstallation()
    {
        String curVersion = AndroidPlugin.GetAppMetaData(ANDROID_DLL_VERSION);
        Logger.Log("checkNewInstallation:" + curVersion);

        SharedPreferences sharedPref = instance.getPreferences(Context.MODE_PRIVATE);
        if (!sharedPref.contains(ANDROID_DLL_VERSION) || !sharedPref.getString(ANDROID_DLL_VERSION, null).equals(curVersion))
        {
            removeDlls();
            try
            {
                sharedPref.edit().putString(ANDROID_DLL_VERSION, curVersion).commit();
            }
            catch (Exception e)
            {
                Log.e("checkNewInstallation", "write local info error:" + ANDROID_DLL_VERSION + ", " + curVersion);
            }
        }
    }

    private static void removeDlls() {
        Logger.Log("removeDlls");

        File filesDir = instance.getFilesDir();
        File dllsDir = new File(filesDir, FILES_DLL_DIR);
        if (dllsDir.exists() && dllsDir.isDirectory()) {
            String[] children = dllsDir.list();
            for (int i = 0; i < children.length; i++) {
                new File(dllsDir, children[i]).delete();
            }
            dllsDir.delete();
        }
//        Toast.makeText(instance, "删除DLL目录成功", Toast.LENGTH_SHORT).show();
    }

    // Setup activity layout
    @Override protected void onCreate(Bundle savedInstanceState) {
        Logger.Log("Activity ver=20160711");
        Logger.Log("Activity onCreate");

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

        Intent intent = getIntent();
        Log.i("UnityActivity", "src intent: " + intent);

        instance = this;

        checkNewInstallation();
        startUnityPlayerActivity();
    }

    // Quit Unity
    @Override protected void onDestroy() {
        // !!!!这里的时序很重要，一定要unityPlayer.quit()再super.onDestroy() 前面才行
        Logger.Log("Activity onDestroy");
        if (mUnityPlayer != null) {
            this.mUnityPlayer.quit();
        }
        super.onDestroy();
    }

    // Pause Unity
    @Override protected void onPause() {
        Logger.Log("Activity onPause");
        super.onPause();
        if (mUnityPlayer != null) {
            this.mUnityPlayer.pause();
        }
    }

    // Resume Unity
    @Override protected void onResume() {
        Logger.Log("Activity onResume");
        super.onResume();
        if (mUnityPlayer != null) {
            this.mUnityPlayer.resume();
        }
    }

    // This ensures the layout will be correct.
    @Override public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        if (mUnityPlayer != null) {
            this.mUnityPlayer.configurationChanged(newConfig);
        }
    }

    // Notify Unity of the focus change.
    @Override public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (mUnityPlayer != null) {
            this.mUnityPlayer.windowFocusChanged(hasFocus);
        }
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    @Override public boolean dispatchKeyEvent(KeyEvent event) {
        if (event.getAction() == KeyEvent.ACTION_MULTIPLE) {
            if (mUnityPlayer != null) {
                return mUnityPlayer.injectEvent(event);
            }
        }
        return super.dispatchKeyEvent(event);
    }

    @Override public boolean onKeyUp(int paramInt, KeyEvent paramKeyEvent) {
        if (mUnityPlayer != null) {
            return this.mUnityPlayer.injectEvent(paramKeyEvent);
        } else {
            return false;
        }
    }

    @Override public boolean onKeyDown(int paramInt, KeyEvent paramKeyEvent) {
        if (mUnityPlayer != null) {
            return this.mUnityPlayer.injectEvent(paramKeyEvent);
        } else {
            return false;
        }
    }

    @Override public boolean onTouchEvent(MotionEvent paramMotionEvent) {
        if (mUnityPlayer != null) {
            return this.mUnityPlayer.injectEvent(paramMotionEvent);
        } else {
            return false;
        }
    }

    @Override public boolean onGenericMotionEvent(MotionEvent paramMotionEvent) {
        if (mUnityPlayer != null) {
            return this.mUnityPlayer.injectEvent(paramMotionEvent);
        } else {
            return false;
        }
    }
}