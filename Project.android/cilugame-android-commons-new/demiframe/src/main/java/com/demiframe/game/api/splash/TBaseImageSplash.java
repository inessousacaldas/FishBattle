package com.demiframe.game.api.splash;

import android.app.Activity;
import android.util.Log;
import android.view.View;
import android.view.animation.AccelerateInterpolator;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.AnimationSet;
import android.view.animation.DecelerateInterpolator;
import android.widget.ImageView;

public abstract class TBaseImageSplash implements TSplash {
	private View layout;
	private ImageView imageView;

	public TBaseImageSplash(View layout, ImageView imageView) {
		this.layout = layout;
		this.imageView = imageView;
	}

	abstract void loadImage(Activity paramActivity, ImageView paramImageView,
			LoadSplashCallback paramLoadSplashCallback);

	@Override
	public void play(final Activity context, final TSplashListener listener) {
		loadImage(context, this.imageView, new LoadSplashCallback() {
			@Override
			public void onLoadSuccess() {
				TBaseImageSplash.this.playSplash(context, listener);
			}

			@Override
			public void onLoadFailed() {
				listener.onFinish();
			}
		});
	}

	public void playSplash(Activity context, final TSplashListener listener) {
		Animation animation = getAnimation();
		animation.setAnimationListener(new Animation.AnimationListener() {
			@Override
			public void onAnimationStart(Animation paramAnimation) {
				Log.d("t", "animation start");
			}

			@Override
			public void onAnimationRepeat(Animation paramAnimation) {
				Log.d("t", "animation repeat");
			}

			@Override
			public void onAnimationEnd(Animation paramAnimation) {
				Log.d("t", "animation end");
				TBaseImageSplash.this.layout.setVisibility(View.INVISIBLE);
				listener.onFinish();
			}
		});
		Log.d("t", "start animat ion");
		this.layout.startAnimation(animation);
		this.layout.setVisibility(View.VISIBLE);
	}

	private Animation getAnimation() {
		Animation fadeIn = new AlphaAnimation(0.0F, 1.0F);
		fadeIn.setInterpolator(new DecelerateInterpolator());
		fadeIn.setDuration(500L);

		Animation fadeOut = new AlphaAnimation(1.0F, 0.0F);
		fadeOut.setInterpolator(new AccelerateInterpolator());
		fadeOut.setStartOffset(1500L);
		fadeOut.setDuration(500L);

		AnimationSet animation = new AnimationSet(false);
		animation.addAnimation(fadeIn);
		animation.addAnimation(fadeOut);

		return animation;
	}

	public static abstract interface LoadSplashCallback {
		public abstract void onLoadSuccess();

		public abstract void onLoadFailed();
	}
}