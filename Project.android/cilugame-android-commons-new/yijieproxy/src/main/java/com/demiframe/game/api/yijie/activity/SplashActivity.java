package com.demiframe.game.api.yijie.activity;

import android.content.Intent;
import android.graphics.Color;

import com.snowfish.cn.ganga.helper.SFOnlineSplashActivity;

/**
 * Created by FQ-PC018 on 2017/2/27.
 */

public class SplashActivity extends SFOnlineSplashActivity {
    @Override
    public void onSplashStop(){
        try{
            //启动主activity
            Class mainCls = Class.forName("{MAIN_GAME_ACTIVITY}");
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
