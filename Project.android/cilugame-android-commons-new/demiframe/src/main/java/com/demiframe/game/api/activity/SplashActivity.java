package com.demiframe.game.api.activity;

/**
 * Created by xianjian on 2017/2/28.
 */


import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.view.animation.AccelerateInterpolator;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.AnimationSet;
import android.view.animation.DecelerateInterpolator;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.ImageView.ScaleType;
import android.widget.RelativeLayout.LayoutParams;

import com.demiframe.game.api.splash.TSplashActivity;

import java.util.ArrayList;
import java.util.List;

public class SplashActivity extends TSplashActivity {

    @Override
    public void onSplashStop(){
        try{
            //启动主activity,打包工具替换名字
            Class mainCls = Class.forName("{DEMI_DEFINE_MAIN_ACTIVITY}");
            Intent intent = new Intent(this, mainCls);
            startActivity(intent);
            finish();
        }catch (Exception e){
            e.printStackTrace();
        }
    }

    @Override
    public int getBackgroundColor(){
        return Color.WHITE;
    }
}

