using System;
using System.Collections;
using System.IO;
using System.Threading;
using AssetPipeline;
using UnityEngine;

public class VoiceLoadHelper : MonoBehaviour
{
    private byte[] _amrBuf;
    private string _filePath;

    private bool _isSuccess;
    private Action<int, string> _onError;

    private Action<bool> _OnLoadInternetComplete;

    //    private void Update()
    //    {
    //        if (_isComplete)
    //        {
    //            if (_OnLoadInternetComplete != null)
    //            {
    //                _OnLoadInternetComplete(_isSuccess);
    //            }
    //            Destroy(gameObject);
    //        }
    //    }

    private static VoiceLoadHelper CreateInstance()
    {
        var go = new GameObject("VoiceLoadHelper");
        DontDestroyOnLoad(go);
        return go.AddComponent<VoiceLoadHelper>();
    }

    #region LocalVoice
    public static void LoadLocalVoice(string filePath, Action<AudioClip> OnLoadFinish)
    {
        var loader = CreateInstance();
        loader._LoadLocalVoice(filePath, OnLoadFinish);
    }

    private void _LoadLocalVoice(string filePath, Action<AudioClip> OnLoadFinish)
    {
        FileHelper.ReadFileAsync(filePath, file =>
        {
            Debug.Log("加载本地数据: " + file.FilePath);
            var samples = HzamrPlugin.Decode(file.Bytes, 0);
            var clip = AudioClip.Create("audio", samples.Length, HzamrPlugin.Channels, HzamrPlugin.Frequency, false);
            clip.SetData(samples, 0);

            if (OnLoadFinish != null)
                OnLoadFinish(clip);
            Destroy(gameObject);
        }, (path, error) =>
        {
            Debug.Log("本地数据问题: " + path + "\n" + error);
            if (OnLoadFinish != null)
                OnLoadFinish(null);
            Destroy(gameObject);
        });
    }
    #endregion

    #region InternetVoice

    public static void LoadInternetVoice(string actual, string fileKey, Action<bool> OnLoadInternetComplete, Action<int, string> onError)
    {
        var loader = CreateInstance();
        loader._LoadInternetVoice(actual, fileKey, OnLoadInternetComplete, onError);
    }

    private void _LoadInternetVoice(string actual, string fileKey,
        Action<bool> OnLoadInternetComplete, Action<int, string> onError)
    {
        _onError = onError;
        _OnLoadInternetComplete = OnLoadInternetComplete;
        StartCoroutine(LoadInternetVoiceCoroutine(actual, fileKey));
    }

    private IEnumerator LoadInternetVoiceCoroutine(string path, string fileKey)
    {
        var amrWWW = new WWW(path);
        yield return amrWWW;

        if (amrWWW.isDone && amrWWW.error == null)
        {
            string filePath = GameResPath.persistentDataPath + "/talk/";
            _filePath = filePath + fileKey;

            string checkPath = _filePath.Substring(0, _filePath.LastIndexOf('/'));
            Debug.Log("save audio path = " + checkPath);
            if (!Directory.Exists(checkPath))
            {
                Directory.CreateDirectory(checkPath); //创建新路径
            }

            _amrBuf = amrWWW.bytes;
            FileHelper.WriteAllBytes(_filePath, _amrBuf);

            if (_OnLoadInternetComplete != null)
            {
                _OnLoadInternetComplete(true);
            }
            Destroy(gameObject);
        }
        else
        {
            if (_onError != null)
                _onError(VoiceErrorCode.ERROR_LOAD_FILE, "该音频无法加载");

            Debug.Log(path + ",该音频无法加载: " + amrWWW.error + "  , " + fileKey);
            if (_OnLoadInternetComplete != null)
            {
                _OnLoadInternetComplete(false);
            }
            Destroy(gameObject);
        }
    }

    #endregion
}
