package com.demiframe.game.api.splash;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;

public class TSplashSequence {

	private List<TSplash> list = new ArrayList<TSplash>();

	public void addSplash(TSplash splash) {
		this.list.add(splash);
	}

	public void play(Activity context, TSplashListener listener) {
		play(context, listener, 0);
	}

	private void play(final Activity context, final TSplashListener listener,
			final int index) {
		if (index >= this.list.size()) {
			listener.onFinish();
		} else {
			((TSplash) this.list.get(index)).play(context, new TSplashListener() {
						public void onFinish() {
							TSplashSequence.this.play(context, listener, index + 1);
						}
					});
		}
	}

}
