using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EffectTime : MonoBehaviour
{
    public float delayTime = 0; //特效播放延时
    public int loopCount = 1; //0 一直循环   1 一次循环   >1 循环次数
    public string effName;
    public Action<EffectTime> OnFinish;
    public float time = 5; //特效时长

    //private bool _isPlayering;
    //private bool _isStart;
    private int _playCount; //已播放次数
    private ParticleSystem[] psList;
    private Animator[] animatorList;

    void Awake()
    {
        psList = GetComponentsInChildren<ParticleSystem>();
        animatorList = GetComponentsInChildren<Animator>();
    }

	//在OnEnable中开始计算延迟，因为特效有可能从资源池中重用，所以不能放到Start里
	void OnEnable()
	{
		CancelInvoke("Play");
		Invoke("Play", delayTime);
	}

//	void OnDisable()
//	{
//		if (loopCount != 0)
//		{
//			if (IsInvoking("OnPlayFinish"))
//			{
//				CancelInvoke("Play");
//				CancelInvoke("OnPlayFinish");
//				Stop();
//			}
//		}
//	}

    public void Play()
    {
        //Debug.LogError("PlayEffect: "+Time.time);
        ResetEffect();
        if (psList != null)
        {
            for (int i = 0; i < psList.Length; i++)
            {
                var ps = psList[i];
                if (ps != null)
                {
                    ps.Play();
                }
            }
        }
        if (animatorList != null)
        {
            for (int i = 0; i < animatorList.Length; i++)
            {
                var at = animatorList[i];
                if (at != null)
                {
                    //at.enabled = true;
                    at.Play(0);
                }
            }
        }

		CancelInvoke("OnPlayFinish");	
        Invoke("OnPlayFinish", time);
    }

    void ResetEffect()
    {
        if (psList != null)
        {
            for (int i = 0; i < psList.Length; i++)
            {
                ParticleSystem ps = psList[i];
                if (ps != null)
                {
                    ps.Clear();
                    ps.Stop();
                }
            }
        }
        //if (animatorList != null)
        //{
        //    for (int i = 0; i < animatorList.Length; i++)
        //    {
        //        var at = animatorList[i];
        //        if (at != null)
        //        {
        //            at.enabled = false;
        //        }
        //    }
        //}
    }

    void OnPlayFinish()
    {
        //Debug.LogError("OnPlayFinish: " + Time.time);
        if (loopCount == 0)
        {
            Play();
            return;
        }

        _playCount++;
        if (_playCount >= loopCount)
        {
            Stop();
        }
        else
        {
            Play();
        }
    }

    public void Stop()
    {
        //Debug.LogError("StopEffect: " + Time.time);
        ResetEffect();
        if (OnFinish != null)
        {
            OnFinish(this);
			OnFinish = null;
        }
    }
}