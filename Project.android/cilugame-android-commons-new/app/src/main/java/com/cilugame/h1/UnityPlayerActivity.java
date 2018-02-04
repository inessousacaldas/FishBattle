package com.cilugame.h1;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Color;
import android.graphics.PixelFormat;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.widget.ImageView;

import com.cilugame.h1.activity.GameLoaderActivity;
import com.demiframe.game.api.GameApi;
import com.unity3d.player.UnityPlayer;
import com.cilugame.h1.util.Logger;
import com.cilugame.h1.view.ViewManager;

import com.tencent.gcloud.voice.GCloudVoiceEngine;

import java.io.File;

public class UnityPlayerActivity extends Activity {
    public static UnityPlayerActivity instance;
    protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code
    private ViewManager viewManager;
    private static final String ANDROID_DLL_VERSION = "android.dll.version";
    private static final String FILES_DLL_DIR = "dlls";
    private static String SPLASH_PATH = "bin/Data/splash.png";

    private Drawable bgDrawable;
    private ImageView splashView;

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
//        final String appPackageName = getApplicationContext().getPackageName();
//        final int rLayoutId = getResources().getIdentifier("cilu_unity_activity", "layout", appPackageName);
//        setContentView(rLayoutId);
//
//        final int rGameLoaderLayoutId = getResources().getIdentifier("cilu_unity_Layout", "id", appPackageName);
//        final View view = findViewById(rGameLoaderLayoutId);
//        if (bgDrawable != null) {
//            if (Build.VERSION.SDK_INT >= 16)
//                view.setBackground(bgDrawable);
//            else
//                view.setBackgroundDrawable(bgDrawable);
//        } else {
//            view.setBackgroundColor(Color.BLACK);
//        }

        splashView =  new ImageView(UnityPlayer.currentActivity);
        if (bgDrawable != null) {
            if (Build.VERSION.SDK_INT >= 16)
                splashView.setBackground(bgDrawable);
            else
                splashView.setBackgroundDrawable(bgDrawable);
        } else {
            splashView.setBackgroundColor(Color.BLACK);
        }

        Resources r = UnityPlayer.currentActivity.getResources();

        mUnityPlayer.addView(splashView, r.getDisplayMetrics().widthPixels, r.getDisplayMetrics().heightPixels);

        setContentView(mUnityPlayer);
        this.getWindow().getDecorView().setBackgroundColor(Color.TRANSPARENT);
        mUnityPlayer.requestFocus();

//        new Handler().postDelayed(new Runnable() {
//            @Override
//            public void run() {
//                mUnityPlayer.removeView(splashView);
//            }
//        }, 3000);

        //getWindow().getDecorView().setBackgroundColor(Color.TRANSPARENT);

        // AndroidPlugin.SetAssetManager(getAssets());
        // this.viewManager = new ViewManager(this);
        CLSDKPlugin.Setup();
    }

    public void UnityInitFinish(){
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(splashView != null){
                    mUnityPlayer.removeView(splashView);
                    splashView = null;
                    bgDrawable = null;
                }
            }
        });
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
        GCloudVoiceEngine.getInstance().init(getApplicationContext(),this);
        Logger.Log("Init GcCloudVoice Success");
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

        Intent intent = getIntent();
        Log.i("UnityActivity", "src intent: " + intent);

        boolean loadUnityPlayer = intent.hasExtra("patchUpdated");
        instance = this;

        try{
            bgDrawable = Drawable.createFromStream(getAssets().open(SPLASH_PATH), null);
        }catch (Exception e){
            Log.d("startUnity", "load splash error");
        }

        checkNewInstallation();

        if (loadUnityPlayer)
        {
            startUnityPlayerActivity();
        }
        else
        {
            //这里不能做SDK的初始化，因为加载GameLaoderActivity后，UnityPlayerActivity会被销毁
            // CLSDKPlugin.Setup();
            startLoaderActivity();
        }
    }

    private void startLoaderActivity() {
        try {
            Intent gameIntent = new Intent(this, GameLoaderActivity.class);
            gameIntent.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
            startActivity(gameIntent);
            this.finish();
        } catch (Exception e) {
            Logger.Log("startLoaderActivity Exception " + e.toString());
            startUnityPlayerActivity();
        }
    }

    // Quit Unity
    @Override protected void onDestroy() {
        // !!!!这里的时序很重要，一定要unityPlayer.quit()再super.onDestroy() 前面才行
        Logger.Log("Activity onDestroy");
        if (mUnityPlayer != null) {
            GameApi.getInstance().onDestroy(this);
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
            GameApi.getInstance().onPause(this);
        }

    }

    // Resume Unity
    @Override protected void onResume() {
        Logger.Log("Activity onResume");
        super.onResume();
        if (mUnityPlayer != null) {
            this.mUnityPlayer.resume();
            GameApi.getInstance().onResume(this);
        }
    }

    @Override
    protected void onNewIntent(Intent intent){
        Logger.Log("Activity onNewIntent");
        super.onNewIntent(intent);
        setIntent(intent);

        if (mUnityPlayer != null) {
            GameApi.getInstance().onNewIntent(this, intent);
        }

    }

    @Override
    protected void onRestart() {
        Logger.Log("Activity onRestart");
        super.onRestart();
        if (mUnityPlayer != null) {
            GameApi.getInstance().onRestart(this);
        }
    }

    @Override
    protected void onStart() {
        Logger.Log("Activity onStart");
        super.onStart();

        if (mUnityPlayer != null) {
            GameApi.getInstance().onStart(this);
        }
    }

    @Override
    protected void onStop() {
        Logger.Log("Activity onStop");
        super.onStop();
        if (mUnityPlayer != null) {
            GameApi.getInstance().onStop(this);
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        Logger.Log("Activity OnActivityResult");
        super.onStop();
        if (mUnityPlayer != null) {
            GameApi.getInstance().onActivityResult(this, requestCode, resultCode, data);
        }
    }

    // This ensures the layout will be correct.
    @Override public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        if (mUnityPlayer != null) {
            this.mUnityPlayer.configurationChanged(newConfig);
            GameApi.getInstance().onConfigurationChanged(this, newConfig);
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