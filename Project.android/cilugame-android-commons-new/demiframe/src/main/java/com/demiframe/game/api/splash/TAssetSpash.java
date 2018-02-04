package com.demiframe.game.api.splash;

import java.io.IOException;
import java.io.InputStream;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;

public class TAssetSpash extends TBaseImageSplash {
	private String assetPath;

	public TAssetSpash(View layout, ImageView imageView, String assetPath) {
		super(layout, imageView);
		this.assetPath = assetPath;
	}

	@Override
	void loadImage(final Activity context, final ImageView imageView,
			final TBaseImageSplash.LoadSplashCallback callback) {
		Bitmap bitmap = null;
		try {
			InputStream stream = context.getAssets().open(
					TAssetSpash.this.assetPath);
			bitmap = BitmapFactory.decodeStream(stream);
		} catch (IOException e) {
			Log.e("T", "load asset splash failed : "
					+ TAssetSpash.this.assetPath, e);
		}
		if (bitmap == null) {
			callback.onLoadFailed();
		} else {
			imageView.setImageBitmap(bitmap);
			callback.onLoadSuccess();
		}
	}
}
