package com.demiframe.game.api.splash;

import java.io.IOException;
import java.util.Arrays;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.RelativeLayout;

public abstract class TSplashActivity extends Activity {

	private RelativeLayout layout;
	private ImageView imageView;
	private TSplashSequence sequence = new TSplashSequence();
	
    @Override
	protected void onCreate(Bundle savedInstance) {
		super.onCreate(savedInstance);

		requestWindowFeature(Window.FEATURE_NO_TITLE);
		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

		this.layout = new RelativeLayout(this);
		this.layout.setBackgroundColor(getBackgroundColor());
		this.layout.setVisibility(View.INVISIBLE);
		RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
		this.imageView = new ImageView(this);
		this.imageView.setScaleType(ImageView.ScaleType.FIT_CENTER);
		RelativeLayout.LayoutParams imageViewParams = new RelativeLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);

		this.layout.addView(this.imageView, imageViewParams);

		String assetDir = "demiframesplash";
		String[] assetPaths = new String[0];
		try {
			assetPaths = getAssets().list(assetDir);
		} catch (IOException e) {
			e.printStackTrace();
		}
		Log.d("t", "assetsPaths size " + assetPaths.length);
		int count = 0;
		Arrays.sort(assetPaths);
		for (String str : assetPaths) {
			Log.d("t", "assets splash " + str);
		}

		String resourcePrefix = "demiframesplash_";
		count = 0;
		while (true) {
			if (count < assetPaths.length) {
				this.sequence.addSplash(new TAssetSpash(this.layout,
						this.imageView, assetDir + "/" + assetPaths[count]));
			} else {
				int id = getResources().getIdentifier(resourcePrefix + count,
						"drawable", getPackageName());
				if (id == 0) {
					break;
				}
				this.sequence.addSplash(new TResSplash(this.layout,
						this.imageView, id));
			}
			count++;
		}

		setContentView(this.layout, params);
	}

    @Override
	protected void onResume() {
		super.onResume();
		Log.d("t", "onresume");
		this.sequence.play(this, new TSplashListener() {
			public void onFinish() {
				TSplashActivity.this.onSplashStop();
			}
		});
	}

	public abstract int getBackgroundColor();

	public abstract void onSplashStop();

}
