// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  AudioManager.cs
// Author   : wenlin
// Created  : 2013/4/16 14:48:34
// Purpose  : 
// **********************************************************************

using UnityEngine;
using AssetPipeline;
public class AudioManager
{
    private static AudioManager _instance;

    private string _curMusicName = "";

    private float _dubbingVolume;

    private GameObject _managerGO;
    private AudioSource _musicAudioSource;

    private float _musicVolume = 0.5f;
    private float _soundVolume = 0.75f;

    private bool _toggleDubbing;
    private bool _toggleMusic = false;
    private bool _toggleSound = true;

    private AudioManager()
    {
    }

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AudioManager();
            }
            return _instance;
        }
    }

    public bool ToggleMusic
    {
        get { return _toggleMusic; }
        set
        {
            _toggleMusic = value;
            if (_toggleMusic)
            {
                if (_musicAudioSource == null)
                {
                    PlayMusic(_curMusicName);
                }
                else
                {
                    if (null == _musicAudioSource.clip)
                    {
                        GameDebuger.LogError(string.Format("声音控制失败，声音片段（_musicAudioSource.clip）为空，请检查对应的声音文件是否已正确导入!（U3D编辑器下重新导入一下目标声音（{0}）即可）。", _curMusicName));
                        return;
                    }
                    else if (!_curMusicName.Contains(_musicAudioSource.clip.name))
                    {
                        PlayMusic(_curMusicName, false);
                    }
                }
            }
        }
    }

    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            if (_musicAudioSource != null)
            {
                _musicAudioSource.volume = _musicVolume;
            }
        }
    }

    public bool ToggleSound
    {
        get { return _toggleSound; }
        set
        {
            _toggleSound = value;
            NGUITools.ToggleSound = value;
        }
    }

    public float SoundVolume
    {
        get { return _soundVolume; }
        set
        {
            _soundVolume = value;
            NGUITools.soundVolume = value;
        }
    }

    public bool ToggleDubbing
    {
        get { return _toggleDubbing; }
        set { _toggleDubbing = value; }
    }

    public float DubbingVolume
    {
        get { return _dubbingVolume; }
        set { _dubbingVolume = value; }
    }

    /// <summary>
    ///     Setup AudioManager
    /// </summary>
    public void Setup()
    {
        _managerGO = GameObject.Find("AudioManager");
    }

    public void PlayMusic(string musicName, bool checkSame = true)
    {
        if (string.IsNullOrEmpty(musicName))
        {
            GameDebuger.Log("Music name is null");
            return;
        }

        if (checkSame && _curMusicName == musicName)
        {
            GameDebuger.Log("Music : " + musicName + "is Playing ");
            return;
        }

        _curMusicName = musicName;

        if (!_toggleMusic)
        {
            return;
        }

        GameDebuger.Log("Play music : " + musicName);

        ResourcePoolManager.Instance.LoadAudioClip(musicName, asset =>
        {
            if (asset != null)
            {
                AudioClip audioClip = asset as AudioClip;
                PlayMusicAudioClip(audioClip);
            }
            else
            {
                GameDebuger.Log("Can not find the Music of " + musicName);
            }
        });
    }

    private void PlayMusicAudioClip(AudioClip audioClip)
    {
        if (_musicAudioSource == null)
        {
            GameObject go = new GameObject();
            
            go.transform.parent = _managerGO.transform;

            _musicAudioSource = go.AddComponent<AudioSource>();
        }

        _musicAudioSource.gameObject.name = "AudioMusic:" + audioClip.name;
        AudioClip oldAudioClip = _musicAudioSource.clip;
        _musicAudioSource.clip = audioClip;
        _musicAudioSource.loop = true;
        _musicAudioSource.volume = MusicVolume;

        _musicAudioSource.Play();
        if(oldAudioClip != null)
            Resources.UnloadAsset(oldAudioClip);
    }

    /// <summary>
    ///     Play Sound
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="option"></param>
    public void PlaySound(string soundName)
    {
        if (!ToggleSound)
        {
            return;
        }

        if (string.IsNullOrEmpty(soundName))
        {
            GameDebuger.Log("Sound name is null");
            return;
        }

        ResourcePoolManager.Instance.LoadAudioClip(soundName, asset =>
        {
            if (asset != null)
            {
                AudioClip audioClip = asset as AudioClip;
                NGUITools.PlaySound(audioClip, SoundVolume);
            }
            else
            {
                GameDebuger.Log("Can not find the sound of " + soundName);
            }
        });
    }

    /// <summary>
    ///     Play Dubbing
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="option"></param>
    public void PlayDubbing(string dubbingName)
    {
        if (!ToggleDubbing)
        {
            return;
        }

        if (string.IsNullOrEmpty(dubbingName))
        {
            GameDebuger.Log("Dubbing name is null");
            return;
        }

        ResourcePoolManager.Instance.LoadAudioClip(dubbingName, asset =>
        {
            if (asset != null)
            {
                AudioClip audioClip = asset as AudioClip;
                NGUITools.PlaySound(audioClip, DubbingVolume);
            }
            else
            {
                GameDebuger.Log("Can not find the Dubbing of " + dubbingName);
            }
        });
    }

    public void StopMusic()
    {
        if (_musicAudioSource != null)
        {
            _musicAudioSource.Stop();
        }

        _curMusicName = "";
    }

    /*
	 * 当玩家在使用语音的时候，将当前场景音量放最小
	 */

    public void StopVolumeWhenRecordVoice()
    {
        ToggleMusic = false;
        MusicVolume = 0;

        ToggleSound = false;
        SoundVolume = 0;
    }

    /*
	 * 停止当前正在播放语音
	 */

    public void PlayVoiceWhenFinishRecord()
    {
        GameDebuger.TODO(@"

        if (ModelManager.SystemData.IsIdleMode())
        {
            return;
        }

        ToggleMusic = ModelManager.SystemData.musicToggle;
        MusicVolume = ToggleMusic ? ModelManager.SystemData.musicValue/100.0f : 0f;

        ToggleSound = ModelManager.SystemData.soundToggle;
        SoundVolume = ToggleSound ? ModelManager.SystemData.soundValue/100.0f : 0f;

        ToggleDubbing = ModelManager.SystemData.DubbingToggle;
        DubbingVolume = ToggleDubbing ? ModelManager.SystemData.DubbingValue/100.0f : 0f;
            ");
    }
}