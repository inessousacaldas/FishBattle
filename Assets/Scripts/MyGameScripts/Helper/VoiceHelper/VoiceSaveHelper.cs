using System;
using System.Threading;
using UnityEngine;
using QCloud.CosApi.Api;
using LITJson;
using System.IO;
using AssetPipeline;

public class VoiceSaveHelper : MonoBehaviour
{
    public const string MIME_TYPE = "amr audio/x-amr";

    private const int MaxTranslateTime = 10;
    private byte[] _amrData;
#pragma warning disable

    private int _channels;

    //    private long _currChannelId;
    //    private float _recordTime;
    //private string _voiceKey;

    //private string _bucket;

    private int _errorCode;
    private int _hz;
#pragma warning restore
    private bool _isGetVoiceRun;
    private bool _isSaveComplete;//保存执行完毕
    private bool _isSaveInternet;//是否保存成功

    private bool _isToAmr;
    private Action<int, string> _onError;

    //    private Action<long, string, string, float> _onFinalResults;
    //    private Action<long, string, bool, float> _onSaveVoice;

    private Action<VoiceFinalResult> mOnFinalResultCallBack;
    private Action<VoiceSaveResult> mOnSaveVoiceCallBack;
    private Action<VoiceEndResult> mOnVoiceEndResult;
    private long _playerId;


    private int _samples;

    private float[] _samplesBuf;


    private byte[] _wavBuf;

    private VoiceFinalResult mVoiceFinalResult = new VoiceFinalResult();
    private VoiceSaveResult mVoiceSaveResult = new VoiceSaveResult();

    private Thread thread;
    private void Awake()
    {
        _errorCode = VoiceErrorCode.NONE;
        _isToAmr = false;
        _isSaveInternet = false;

        _isGetVoiceRun = false;

        _isSaveComplete = false;
    }

    private void Update()
    {
        if (_errorCode == VoiceErrorCode.ERROR_TO_AMR)
        {
            // 压缩语言失败,退出
            if (_onError != null)
                _onError(VoiceErrorCode.ERROR_TO_AMR, "语音数据处理失败");
            DestroyMe();
        }
        else if (_errorCode == VoiceErrorCode.ERROR_SEND_FILE)
        {
            // 报错网络文件失败,如果不在翻译状态下,退出
            if (_onError != null)
                _onError(VoiceErrorCode.ERROR_SEND_FILE, "网络文件保存失败");

            _errorCode = VoiceErrorCode.NONE;

            if (mOnSaveVoiceCallBack != null)
            {
                mOnSaveVoiceCallBack(mVoiceSaveResult);
            }

            if (_isGetVoiceRun == false)
            {
                Debug.Log("Error:网络文件保存失败,不在翻译状态下,退出");
                DestroyMe();
            }
        }
        else if (_isToAmr)
        {
            // 压缩数据完成,上传百度翻译
            Debug.Log("压缩数据完成,上传百度翻译");
            _isToAmr = false;
            GetVoiceText();
        }
        // 保存完成
        else if (_isSaveInternet)
        {
            // 报错网络文件成功,如果不在翻译状态下,退出
            _isSaveInternet = false;
            if (mOnSaveVoiceCallBack != null)
            {
                mOnSaveVoiceCallBack(mVoiceSaveResult);
            }

            if (_isGetVoiceRun == false)
            {
                Debug.Log("网络文件保存成功,如果不在翻译状态下,退出");
                DestroyMe();
            }
        }
    }

    private static VoiceSaveHelper CreateInstance()
    {
        var go = new GameObject("VoiceSaveHelper");
        DontDestroyOnLoad(go);
        return go.AddComponent<VoiceSaveHelper>();
    }

    public static void SaveVoice(string voiceKey,
        AudioClip recondClip, 
        long playerId, 
        long channelId, 
        float recordTime,
        int lastPos,
        Action<VoiceFinalResult> onFinalResults, 
        Action<VoiceSaveResult> onSaveVoice,
        Action<VoiceEndResult> onVoiceEndResult,
        Action<int, string> onError)
    {
        var saver = CreateInstance();
        saver.SaveVoiceInternal(voiceKey, recondClip, playerId, channelId, recordTime, lastPos, onFinalResults, onSaveVoice, onVoiceEndResult, onError);
    }

    private void SaveVoiceInternal(string voiceKey, 
        AudioClip recondClip,
        long playerId,
        long channelId,
        float recordTime,
        int lastPos,
        Action<VoiceFinalResult> onFinalResults, 
        Action<VoiceSaveResult> onSaveVoice,
        Action<VoiceEndResult> onVoiceEndResult,
        Action<int, string> onError)
    {
        _playerId = playerId;

        mVoiceFinalResult.ChannelId = channelId;
        mVoiceFinalResult.RecordTime = recordTime;
        mVoiceFinalResult.VoiceKey = voiceKey;

        mVoiceSaveResult.ChannelId = channelId;
        mVoiceSaveResult.RecordTime = recordTime;
        mVoiceSaveResult.VoiceKey = voiceKey;


        mOnFinalResultCallBack = onFinalResults;
        mOnSaveVoiceCallBack = onSaveVoice;
        mOnVoiceEndResult = onVoiceEndResult;
        _onError = onError;

        //        _voiceKey = voiceKey;
        //_samplesBuf = new float[recondClip.samples * recondClip.channels];
        _samplesBuf = new float[lastPos + lastPos / 10];
        recondClip.GetData(_samplesBuf, 0);
        _hz = recondClip.frequency;
        _channels = recondClip.channels;
        _samples = Mathf.Min(recondClip.samples, _samplesBuf.Length);
        //#else
        thread = new Thread(HandleVoice);
        thread.Start();
        thread.IsBackground = true;
        //#endif
    }

    /// <summary>
    /// 多线程~处理转码和语音上传
    /// </summary>
    private void HandleVoice()
    {
        var amrData = HzamrPlugin.Encode(_samplesBuf, _samples);

        //Debug.Log(string.Format("[WavToAmr]:Wav Size = {0},Amr Size = {1}", _wavBuf.Length, amrData.Length));

        _amrData = amrData;

        if (_amrData != null && _amrData.Length > 0)
        {
            _isToAmr = true;
            //转码完成，开始上传~
            SaveVoiceToCos();
        }
        else
        {
            _errorCode = VoiceErrorCode.ERROR_TO_AMR;
        }
    }

    private void GetVoiceText()
    {
        if (_amrData != null && _amrData.Length > 0)
        {
            _isGetVoiceRun = true;

            // 开启语音最终翻译倒计时,如果x秒后不返还,则可以判断引起停止
            CSTimer.Instance.SetupCoolDown("VoiceTranslateTime", MaxTranslateTime, null,
                OnVoiceTranslateTimeFinished);
            BaiduVoiceRecognition.Instance.GetVoiceText(_playerId, _amrData, _amrData.Length, GetVoiceTextResult);
        }
    }

    private void GetVoiceTextResult(string result)
    {
        CSTimer.Instance.CancelCd("VoiceTranslateTime");

        if (!string.IsNullOrEmpty(result) && result[result.Length - 1] == '，')
        {
            result = result.Substring(0, result.Length - 1);
        }

        //        if (_onFinalResults != null)
        //            _onFinalResults(_currChannelId, _voiceKey, result, _recordTime);
        mVoiceFinalResult.Result = result;
        if (mOnFinalResultCallBack != null)
            mOnFinalResultCallBack(mVoiceFinalResult);

        if (_isSaveComplete)
        {
            Debug.Log("语音转文字成功,退出");
            DestroyMe();
        }
        else
        {
            _isGetVoiceRun = false;
        }
    }

    private void DestroyMe()
    {
        thread.Abort();
        VoiceEndResult result = new VoiceEndResult();

        result.ChannelId = mVoiceSaveResult.ChannelId;
        result.fileUrl = mVoiceSaveResult.fileUrl;
        result.SaveState = mVoiceSaveResult.SaveState;
        result.VoiceKey = mVoiceSaveResult.VoiceKey;
        result.RecordTime = mVoiceSaveResult.RecordTime;
        result.TranslateRes = mVoiceFinalResult.Result;
        if(mOnVoiceEndResult != null)
            mOnVoiceEndResult(result);

        mOnFinalResultCallBack = null;
        mOnFinalResultCallBack = null;
        mOnSaveVoiceCallBack = null;
        GameDebuger.Log("OnVoiceSave End , state : " + mVoiceSaveResult.SaveState);
        Destroy(gameObject);
    }

    private void OnVoiceTranslateTimeFinished()
    {
        mVoiceFinalResult.Result = "语音转文字失败";
        if (mOnFinalResultCallBack != null)
            mOnFinalResultCallBack(mVoiceFinalResult);

        if (_isSaveComplete)
        {
            Debug.Log("语音转文字失败,退出");
            DestroyMe();
        }
        else
        {
            _isGetVoiceRun = false;
        }
    }

    //保存到腾讯云
    private void SaveVoiceToCos()
    {
        TencentVoiceHelper.UpLoadVoice(_amrData, mVoiceFinalResult.VoiceKey,
            fileUrl => {
                _isSaveInternet = true;
                mVoiceSaveResult.fileUrl = fileUrl;
                mVoiceSaveResult.SaveState = true;
            },
            () => {
                mVoiceSaveResult.SaveState = false;
                _errorCode = VoiceErrorCode.ERROR_SEND_FILE;
            });

        _isSaveComplete = true;
    }
    private void SaveVoiceToQiniu()
    {
        Debug.LogError("<<<<<<<<<<<音频文件保存到七牛<<<<<<<<<<:_bucket" + GameConfig.QINIU_BUCKET + "mVoiceFinalResult.VoiceKey:" + mVoiceFinalResult.VoiceKey);
        QiNiuFileExt.PutFileBuf(_amrData, _amrData.Length, GameConfig.QINIU_BUCKET, mVoiceFinalResult.VoiceKey, false, MIME_TYPE, (state, key) =>
        {
            if (state)
                _isSaveInternet = true;
            else
                _errorCode = VoiceErrorCode.ERROR_SEND_FILE;

            _isSaveComplete = true;
        });
    }

}

public class VoiceFinalResult
{
    public long ChannelId;
    public string VoiceKey;
    public string Result;
    public float RecordTime;

}

public class VoiceSaveResult
{
    public long ChannelId;
    public string VoiceKey;
    public bool SaveState;
    public float RecordTime;
    public string fileUrl;
}

public class VoiceEndResult
{
    public long ChannelId;//频道
    public string VoiceKey; //文件名
    public bool SaveState; //保存状态
    public string TranslateRes; // 装换文字
    public float RecordTime;
    public string fileUrl; // 下载链接
}