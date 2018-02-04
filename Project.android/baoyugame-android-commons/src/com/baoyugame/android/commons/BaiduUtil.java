package com.baoyugame.android.commons;

import java.util.ArrayList;
import java.util.Arrays;

import com.baidu.speech.VoiceRecognitionService;
import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognitionListener;
import android.speech.SpeechRecognizer;
import android.util.Base64;
import android.util.Log;


public final class BaiduUtil implements RecognitionListener
{
	private static final int EVENT_ERROR = 11;
	
	private static Activity activity = null;
	
	private static SpeechRecognizer speechRecognizer;

	private static BaiduUtil instance;
	
	private static Intent intent;
	
	public static void RegisterBaiduASR()
	{
		if(speechRecognizer == null)
		{
			UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","before 注册BaiduASR");
			//Log.d("BaiduUtil", ">>>>> RegisterBaiduASR -> before UnityPlayer.currentActivity.runOnUiThread");  
			UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
				@Override
				public void run() {
					UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> RegisterBaiduASR");
			        speechRecognizer = SpeechRecognizer.createSpeechRecognizer(getActivity(), new ComponentName(getActivity(), VoiceRecognitionService.class));
			        if(instance == null)
			        	instance = new BaiduUtil();
			        speechRecognizer.setRecognitionListener(instance);
			        
			        intent = new Intent();
			        //intent.putExtra(Constant.EXTRA_KEY, "dbAZnpWMVWuzVSmVt1CZms3x");
			        //intent.putExtra(Constant.EXTRA_SECRET, "BYTOGHEUIvPBkVZ0t0NLMrpPlcWj7iek");
			        
			        intent.putExtra(Constant.EXTRA_NLU, "enable");
			        intent.putExtra(Constant.EXTRA_SAMPLE, Constant.SAMPLE_8K);
			        intent.putExtra(Constant.EXTRA_VAD, Constant.VAD_INPUT);
			        UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> End RegisterBaiduASR");
				}
			});
		}
	}
	
	public static void StartListening()
	{
		UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","开始说话");
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> StartListening");
				speechRecognizer.startListening(intent);
				UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> End StartListening");
			}
		});
	}
	
	public static void StopListening()
	{
		UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","说完了");
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> StopListening");
				speechRecognizer.stopListening();
				UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog",">>>>> End StopListening");
			}
		});		
	}
	
	public static void Cancel()
	{
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				speechRecognizer.cancel();
			}
		});		
	}
	
	public static void Destroy()
	{
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				speechRecognizer.destroy();
				speechRecognizer = null;
			}
		});			
	}
	
	@Override
	public void onBeginningOfSpeech() {
		// TODO 当用户开始说话后，将会回调此方法
		 UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdk_BeginningOfSpeech","");
	}

	@Override
	public void onBufferReceived(byte[] buffer) {
		// TODO 此方法会被回调多次，buffer是当前帧对应的PCM语音数据，拼接后可得到完整的录音数据
		//UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","此方法会被回调多次，buffer是当前帧对应的PCM语音数据，拼接后可得到完整的录音数据");
		//String bufferStr = Base64.encodeToString(buffer, Base64.DEFAULT);
		//UnityPlayer.UnitySendMessage("GameStart","OnBufferReceived",bufferStr);
	}

	@Override
	public void onEndOfSpeech() {
		// TODO 当用户停止说话后，将会回调此方法
		UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdk_EndOfSpeech","");
	}

	@Override
	public void onError(int error) {
		// TODO 错误码详见错误码一节（注意：识别出错将不再回调onResults方法）
		UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","错误码详见错误码一节,error: " + error);
	}

	@Override
	public void onEvent(int eventType, Bundle params) {
        switch (eventType) {
        case EVENT_ERROR:
            String reason = params.get("reason") + "";
            UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","EVENT_ERROR, " + reason);
            break;
        case VoiceRecognitionService.EVENT_ENGINE_SWITCH:
            int type = params.getInt("engine_type");
        	UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","*引擎切换至" + (type == 0 ? "在线" : "离线"));
            break;
    	}
	}

	@Override
	public void onPartialResults(Bundle partialResults) {
		// TODO 在最终识别结果返回前，可能会收到多个临时识别结果
        ArrayList<String> nbest = partialResults.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        if (nbest.size() > 0) {
        	UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","~临时识别结果：" + Arrays.toString(nbest.toArray(new String[0])));
            //txtResult.setText(nbest.get(0));
        }
	}

	@Override
	public void onReadyForSpeech(Bundle params) {
		// TODO 只有当此方法回调之后才能开始说话，否则会影响识别效果
        //print("准备就绪，可以开始说话");
		UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdk_ReadyForSpeech","");
	}

	@Override
	public void onResults(Bundle results) {
		// TODO 识别结果中包含：识别结果、原始结果等，具体见结果解析一节
        //long end2finish = System.currentTimeMillis() - speechEndTime;
        ArrayList<String> nbest = results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdk_Result",Arrays.toString(nbest.toArray(new String[nbest.size()])));
	}

	@Override
	public void onRmsChanged(float rmsdB) {
		// TODO 引擎将对每一帧语音进行回调一次改方法
		//UnityPlayer.UnitySendMessage("GameStart","OnBaiduASRSdkLog","引擎将对每一帧语音进行回调一次改方法");
	}

	public static Activity getActivity() {
		if (null == activity) {
			setActivity(UnityPlayer.currentActivity);
		}
		return activity;
	}
	
	public static void setActivity(Activity activity) {
		BaiduUtil.activity = activity;
	}
}