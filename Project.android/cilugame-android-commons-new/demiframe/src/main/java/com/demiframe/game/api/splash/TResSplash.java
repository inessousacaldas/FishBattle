package com.demiframe.game.api.splash;

import android.app.Activity;
import android.view.View;
import android.widget.ImageView;

public class TResSplash extends TBaseImageSplash {
	private int resourceId;

	public TResSplash(View layout, ImageView imageView, int id) {
		super(layout, imageView);
		this.resourceId = id;
	}

	@Override
	void loadImage(Activity context, ImageView imageView,
			TBaseImageSplash.LoadSplashCallback callback) {
		imageView.setImageResource(this.resourceId);
		callback.onLoadSuccess();
	}
}
