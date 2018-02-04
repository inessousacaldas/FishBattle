// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  SingleMicrophoneCapture.cs
// Author   : willson
// Created  : 2014/12/8 
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetPipeline;
using Qiniu.Auth.digest;
using Qiniu.Conf;
using Qiniu.RS;
using UnityEngine;

public class VoiceRecognitionManager : MonoBehaviour
{

    public delegate void OnEndOfSpeech(long _currChannelId,string voiceKey,float _recordTime);

    private const int MaxRecordTime = 15;
    private const float ShortVoiceTime = 1f;
    public static string LocalFilePath = GameResPath.persistentDataPath + "/talk";
    private static VoiceRecognitionManager _instance;

    private long _currChannelId;
    private string mParam;
    private bool _isRecord;
    private int _recordFreq;

    // 是否有麦克风
    private bool _micConnected;

    private bool _needCheckVoiceRecordData;
    private AudioSource _playAudioSource;

    private AudioSource _recordAudioSource;
    private float _recordTime;
    private Dictionary<string, AudioClip> _selfVoiceDic;
    private Queue<string> _selfVoiceKeyQueue;
    // 缓存自身的语音数据,队列控制顺序
    public static VoiceRecognitionManager Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("_VoiceRecognitionManager");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<VoiceRecognitionManager>();
            _instance.Init();
        }
    }

    public event Action<int, string> OnError;

   
    // 当用户停止说话后，将会回调此方法
    public event OnEndOfSpeech OnEndOfSpeechEvt;
    // 录音时间过长
    public event Action OnRecordTimeOut;

    //模块独立处理回调，返回所有数据
    public event Action<long, float, AudioClip, int> OnEndOfSpeechToOther;

    public event Action<VoiceSaveResult> OnSaveVoiceEndEvt;
    public event Action<VoiceFinalResult> OnTranslateVoiceEndEvt;
    public event Action<VoiceEndResult> OnVoiceEndResultEvt; //保存和翻译转换同时完成
    public void Setup()
    {

		BaiduVoiceRecognition.Instance.getToken(GameConfig.BAIDU_VOP_APIKEY, GameConfig.BAIDU_VOP_SECRETKEY);
        CheckTalkDirectory();
    }

    private void CheckTalkDirectory()
    {
        try
        {
            FileHelper.CreateDirectory(LocalFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void Init()
    {
        _selfVoiceKeyQueue = new Queue<string>();
        _selfVoiceDic = new Dictionary<string, AudioClip>();

        var devices = Microphone.devices;
        if (devices.Length <= 0)
        {
            GameDebuger.Log(">>>>>>>>>>  Microphone Init:Microphone not connected!");
            _micConnected = false;
        }
        else
        {
            _micConnected = true;
            int min;
            int max;
            //返回值为0时,代表该录音设备支持任何频率
            Microphone.GetDeviceCaps(null, out min, out max);
            // 百度只能处理 8000 的音频信息
            if (max > 0)
            {
                _recordFreq = max > 8000 ? 8000 : max;
            }
            else
            {
                _recordFreq = 8000;
            }

            _recordAudioSource = gameObject.AddComponent<AudioSource>();
            _playAudioSource = gameObject.AddComponent<AudioSource>();

            string deviceNames = "";
            for (int i = 0; i < devices.Length; i++)
            {
                deviceNames += devices[i] + "\n";
            }
            GameDebuger.Log(string.Format(">>>>>>>>>>  Microphone Init:Microphone connected!\n{0}\nMaxRecordFreq:{1}\nCurRecordFreq:{2}", deviceNames, max, _recordFreq));
        }
    }

    public bool IsMicConnected()
    {
        return _micConnected;
    }

    public void Record(long channelId, string param = "")
    {
        if (_isRecord || Microphone.IsRecording(null))
        {
            TipManager.AddTip("正在录音中...");
            return;
        }

        if (_micConnected && _recordAudioSource != null)
        {
            _currChannelId = channelId;
            mParam = param;
            // 注意这里在外部先静音了
            //AudioManager.Instance.StopVolumeWhenRecordVoice();

            _isRecord = true;
            _recordAudioSource.Stop();
            _recordAudioSource.mute = true;
            _recordAudioSource.clip = Microphone.Start(null, true, MaxRecordTime, _recordFreq);

            _recordAudioSource.Play();
            _recordTime = 0;
            _needCheckVoiceRecordData = true;
            JSTimer.Instance.SetupCoolDown("voiceRecordTime", MaxRecordTime, OnUpdateRecordTime,
                OnRecordTimeFinished);
            GameDebuger.Log("========StartRecord========");
        }
    }

    public void Stop(Action OnStop = null)
    {
        if (_micConnected && _recordAudioSource != null)
        {
            GameDebuger.Log("录音结束...");
            int lastPos = Microphone.GetPosition(null);
            Microphone.End(null);
            _recordAudioSource.Stop();

            //AudioManager.Instance.PlayVoiceWhenFinishRecord();
            //TipManager.AddTopTip("录音结束 播放测试");
            JSTimer.Instance.CancelCd("voiceRecordTime");

            if (!IsEnoughLength())
            {
                _isRecord = false;
                if (OnError != null)
                    OnError(VoiceErrorCode.ERROR_SHORT_VOICE, "录音数据太短");
                return;
            }

            //ProxyChatModule.HideSpeechFlag();
            if (OnStop != null)
                OnStop();

            if (_currChannelId < 0)
            {
                //_currChannelId 是<0表示模块独立处理，不走聊天流程
                AudioClip clip = VoiceHelper.CopyToNewClip(_recordAudioSource, _recordFreq);
                if (OnEndOfSpeechToOther != null)
                    OnEndOfSpeechToOther(_currChannelId, _recordTime, clip, lastPos);
            }
            else
            {
                string voiceKey = QiNiuFileExt.NewKey;
                GameDebuger.Log("录音结束...生成key: " + voiceKey);
                SavaSelfVoiceCache(voiceKey);

                VoiceSaveHelper.SaveVoice(
                    voiceKey, 
                    _recordAudioSource.clip, 
                    ModelManager.Player.GetPlayerId(),
                    _currChannelId,
                    _recordTime,
                    lastPos,
                    OnTranslateVoiceEnd, 
                    OnSaveVoiceEnd,
                    OnVoiceEnd,
                    OnError);

                if (OnEndOfSpeechEvt != null)
                {
                    OnEndOfSpeechEvt(_currChannelId, voiceKey, _recordTime);
                }
            }

            GameDebuger.Log("Record语音长度,clipLength:" + Mathf.CeilToInt(_recordAudioSource.clip.length)
                + "  ,计时器时间:" + _recordTime
                + "  ,lastPos:" + lastPos);

            _isRecord = false;
        }
    }
    /// <summary>
    /// 同时结束
    /// </summary>
    /// <param name="obj"></param>
    private void OnVoiceEnd(VoiceEndResult obj)
    {
        if (OnVoiceEndResultEvt != null)
            OnVoiceEndResultEvt(obj);
    }

    /// <summary>
    /// 语音保存结束
    /// </summary>
    /// <param name="obj"></param>
    private void OnSaveVoiceEnd(VoiceSaveResult obj)
    {
        if (OnSaveVoiceEndEvt != null)
            OnSaveVoiceEndEvt(obj);
    }

    /// <summary>
    /// 语音转换结束
    /// </summary>
    /// <param name="obj"></param>
    private void OnTranslateVoiceEnd(VoiceFinalResult obj)
    {
        if (OnTranslateVoiceEndEvt != null)
            OnTranslateVoiceEndEvt(obj);
    }

    public void Cancel()
    {
        if (_micConnected && _recordAudioSource != null)
        {
            _isRecord = false;
            Microphone.End(null);
            _recordAudioSource.Stop();

            AudioManager.Instance.PlayVoiceWhenFinishRecord();
            JSTimer.Instance.CancelCd("voiceRecordTime");
        }
    }


    public bool IsRecord()
    {
        return _isRecord;
    }

    private bool ValidateVoiceData()
    {
        return VoiceHelper.ValidateVoiceData(_recordAudioSource);
    }

    private void SavaSelfVoiceCache(string voiceKey)
    {
        var clip = VoiceHelper.CopyToNewClip(_recordAudioSource, _recordFreq);
        GameDebuger.Log("录音结束...提取录音数据: " + voiceKey);
        if (clip != null)
        {
            _selfVoiceKeyQueue.Enqueue(voiceKey);
            _selfVoiceDic[voiceKey] = clip;
        }
    }

    public void SavaSelfVoiceCache(string voiceKey, AudioClip clip)
    {
        _selfVoiceDic[voiceKey] = clip;
    }


    /*
	 * 计算录制的时长
	 */

    private void OnUpdateRecordTime(float time)
    {
        if (_needCheckVoiceRecordData && _recordTime >= 1f)
        {
            _needCheckVoiceRecordData = false;
            if (!ValidateVoiceData())
            {
                Cancel();
                // 录音1秒后没有数据,判断录音失败
                if (OnError != null)
                    OnError(VoiceErrorCode.ERROR_NO_DATA, "录音失败,请检查麦克风权限");
            }
        }
        _recordTime = MaxRecordTime - time;
    }

    /*
	 * 最长录音计时结束
	 */

    private void OnRecordTimeFinished()
    {
        if (_recordTime > MaxRecordTime)
        {
            _recordTime = MaxRecordTime;
        }

        Stop();

        if (OnRecordTimeOut != null)
        {
            TipManager.AddTip(string.Format("最长只能录制{0}秒的音频", MaxRecordTime));
            OnRecordTimeOut();
        }
    }


    /*
	 * 判断当前录制的语音时间是否够长，不够钱则不发送
	 */

    public bool IsEnoughLength()
    {
        if (_recordAudioSource != null)
        {
            if (_recordTime > ShortVoiceTime)
            {
                return true;
            }
            TipManager.AddTip("语音录制太短");
            return false;
        }
        return false;
    }

    public void PlayRecord()
    {
        if (_recordAudioSource != null)
            _recordAudioSource.Play(); //Playback the recorded audio
    }

    /// <summary>
    /// 删除语音的缓存
    /// </summary>
    public void CleanupVoiceCache()
    {
        try
        {
            FileHelper.DeleteDirectory(LocalFilePath, true);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #region 语音加载

    private string _currPlayVoiceKey = string.Empty;
    public event Action<string> OnPlayVoice;
    public event Action<string, bool> OnStopVoice;

    public string GetCurrPlayVoiceKey()
    {
        return _currPlayVoiceKey;
    }

    public bool IsPlayingVoice()
    {
        if (_playAudioSource != null)
            return _playAudioSource.isPlaying;
        return true;
    }

    public const string PlayVoiceStop = "PlayVoiceStop";

    private void PlaySoundByClip(AudioClip clip, string fileKey, float recordTime)
    {
        if (_playAudioSource == null)
        {
            return;
        }

        if (clip.length > 0)
        {
            AudioManager.Instance.StopVolumeWhenRecordVoice();
            //TipManager.AddWhiteTip( "音频剪辑准备好了，开始播放！" );
            GameDebuger.Log("音频剪辑准备好了，开始播放！");
            _playAudioSource.clip = clip;
            //Todo:播放的音量
            //_playAudioSource.volume = ModelManager.SystemData.voiceToggle == true ? ModelManager.SystemData.voiceValue / 100.0f : 0;
            _playAudioSource.loop = false;
            _playAudioSource.Play();

            //先将声音设置成正常的音量//
            GameDebuger.Log("语音长度: " + recordTime);
            if (recordTime == 0)
                recordTime = clip.length / 8f;
            recordTime += 0.5f;

            _currPlayVoiceKey = fileKey;

            JSTimer.Instance.SetupCoolDown(PlayVoiceStop, recordTime, null, OnVoiceStop);
            if (OnPlayVoice != null)
            {
                OnPlayVoice(_currPlayVoiceKey);

            }
        }
    }

    private void OnVoiceStop()
    {
        if (_playAudioSource != null)
        {
            _playAudioSource.Stop();
        }

        GameDebuger.Log("语音停止播放了....");
        if (OnStopVoice != null)
        {
            OnStopVoice(_currPlayVoiceKey, true);
        }
        _currPlayVoiceKey = string.Empty;
        AudioManager.Instance.PlayVoiceWhenFinishRecord();
    }

    public void StopCurrVoice(bool needContinue)
    {
        if (_playAudioSource != null && _playAudioSource.isPlaying)
        {
            _playAudioSource.Stop();
            if (string.IsNullOrEmpty(_currPlayVoiceKey) && OnStopVoice != null)
            {
                OnStopVoice(_currPlayVoiceKey, needContinue);
            }
            _currPlayVoiceKey = string.Empty;
        }
    }

    private bool IsPlayVoiceTheSameAsCurPlaying(string fileKey)
    {
        if (IsPlayingVoice() && _currPlayVoiceKey == fileKey)
        {
            if (_playAudioSource != null && _playAudioSource.isPlaying)
            {
                JSTimer.Instance.CancelCd(PlayVoiceStop);
                OnVoiceStop();
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 自己录的语音存在_selfVoiceDic
    /// 其他人的语音 从7牛上下载到本地，如果已经下载了，则直接播放，否则先下载
    /// </summary>
    /// <param name="fileKey"></param>
    /// <param name="recordTime"></param>
    public void PlayVoice(string fileKey, float recordTime,string voiceUrl = "")
    {
        if (_isRecord)
        {
            StopCurrVoice(false);
            return;
        }
        if (IsPlayVoiceTheSameAsCurPlaying(fileKey))
        {

            return;
        }
        StopCurrVoice(false);

        // 播放自己的音频
        if (_selfVoiceDic.ContainsKey(fileKey))
        {
            PlaySoundByClip(_selfVoiceDic[fileKey], fileKey, recordTime);
            return;
        }

        string wavPath = LocalFilePath + "/" + fileKey;
        if (File.Exists(wavPath))
        {
            // 在本地文件缓存中
            GameDebuger.Log("在本地文件缓存中");
            LoadLocalVoice(wavPath, fileKey, recordTime);

        }
        else
        {
            // 网络存储
            if(!string.IsNullOrEmpty(voiceUrl))
            {
                GameDebuger.Log("从网络存储获得数据");
                LoadInternetVoice(voiceUrl, fileKey, recordTime);
            }
            //var actual = QiNiuFileExt.GetFileUrl(GameConfig.QINIU_DOMAIN, fileKey);
            //LoadInternetVoice(actual, fileKey, recordTime);

        }

    }
    /// <summary>
    /// 加载本地录音
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileKey"></param>
    /// <param name="recordTime"></param>
    private void LoadLocalVoice(string filePath, string fileKey, float recordTime)
    {
        VoiceLoadHelper.LoadLocalVoice(filePath, clip =>
        {
            if (clip != null)
            {
                PlaySoundByClip(clip, fileKey, recordTime);


            }
            else
                TipManager.AddTip("获取不到音频" + fileKey);
        });
    }
    /// <summary>
    /// 加载网络录音
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="fileKey"></param>
    /// <param name="recordTime"></param>
    private void LoadInternetVoice(string actual, string fileKey, float recordTime)
    {
        VoiceLoadHelper.LoadInternetVoice(actual, fileKey, isSuccess =>
        {
            if (isSuccess)
            {
                PlayVoice(fileKey, recordTime);
            }
        }, OnError);
    }

    #endregion

    #region 说话时的能量值计算
    public float GetVoiceVolume()
    {
        return VoiceHelper.GetVoiceVolume(_recordAudioSource);
    }
    #endregion
}


