package tsjj.baoyugame.com.activity;

import android.content.Intent;
import android.util.Log;

public class BaoyuActivityFix {
	public static BaoyuActivityCallback callback = null;
	
	public static void setActivityCallback(BaoyuActivityCallback _callback){
		callback = _callback;
	}
	
	public static void initActivityCallback(){
		if(callback != null) {
			callback = new BaoyuActivityCallback();
		}
	}
	
	public static class BaoyuActivityCallback{
		public BaoyuActivityCallback(){}
		
		public void onNewIntent(Intent intent){}
		
		public void onActivityResult(int requestCode, int resultCode, Intent data){
			Log.d("BaoyuActivityFix", ">>>>>>>>>>>> requestCode:" + requestCode + ",  resultCode:" + resultCode + ",  data" + data);
		}
	}
}
