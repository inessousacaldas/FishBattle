using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleShakeEffectHelper : MonoBehaviour
{
    // 战斗震动效果
    // 1. Battle2dBg 战斗底图
    // 2. Scene2dCamera 2D场景摄像机
    // 3. BattleCamea 战斗摄像机

    private Transform _BattleCamea;

    public Vector3 shakeIntensity = new Vector3(0.3f, 0.3f, 0.3f);
    public float shakeDuration = 4.0f;

    private Vector3 shake_intensity; // = 0.3f;

    public bool isRunning;

    public bool BattleShakeEffectRunning
    {
        get { return isRunning; }
    }

    public delegate void FinishShakingDelegate();

    public FinishShakingDelegate FinishShaking;

    private Vector3 lastEulerAngles;
    private Vector3 lastPosition;

    private bool updateOnce = true;

    public void Setup()
    {
        _BattleCamea = transform;
    }


    public void Launch(float duration, Vector3 intensity = default (Vector3))
    {
        if(isRunning)
            return;
        if (intensity != default(Vector3))
        shakeIntensity = intensity;
        
        shakeDuration = duration;
        shake_intensity = shakeIntensity;

        
        if (updateOnce == false)
        {
            updateOnce = true;

            lastPosition = lastPosition * -1;
            lastEulerAngles = lastEulerAngles * -1;
            _BattleCamea.localPosition = _BattleCamea.localPosition + lastPosition;
            _BattleCamea.localEulerAngles = _BattleCamea.localEulerAngles + lastEulerAngles;
        }

        isRunning = true;

        //StartCoroutine(RunShake());
        
    }

    IEnumerator RunShake()
    {
        yield return null;
        isRunning = true;
    }

    public void UpdatePostionAndRotation()
    {
    }

    public void Stop()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;

            if (shake_intensity == default(Vector3))
                shake_intensity = shakeIntensity;
        }
        else
        {
            isRunning = false;

            if (FinishShaking != null)
                FinishShaking();

            if (updateOnce == false)
            {
                updateOnce = true;
                lastPosition = lastPosition * -1;
                lastEulerAngles = lastEulerAngles * -1;

                _BattleCamea.localPosition = _BattleCamea.localPosition + lastPosition;
                _BattleCamea.localEulerAngles = _BattleCamea.localEulerAngles + lastEulerAngles;

            }
            return;
        }

        if (shake_intensity != default(Vector3))
        {
            if (updateOnce)
            {
                Func<Vector3, Vector3> aaa = (a) =>
                {
                    var temp = Random.insideUnitSphere;
                    temp = new Vector3( temp.x * a.x, temp.y * a.y, temp.z * a.z);      
                    return temp;
                };

                lastPosition = aaa(shake_intensity);
                lastEulerAngles = aaa(shake_intensity);
            }
            else
            {
                lastPosition = lastPosition * -1;
                lastEulerAngles = lastEulerAngles * -1;
            }

            updateOnce = !updateOnce;

            _BattleCamea.localPosition = _BattleCamea.localPosition + lastPosition;
            _BattleCamea.localEulerAngles = _BattleCamea.localEulerAngles + lastEulerAngles;

        }
    }
}
