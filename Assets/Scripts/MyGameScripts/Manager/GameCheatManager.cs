using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AppDto;

public class GameCheatManager
{
    private static GameCheatManager _instance;

    public static GameCheatManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameCheatManager();
            }
            return _instance;
        }
    }

    public const float CheatingCheckFrequence = 5f;
    public const string CheatingCheckTick = "CheatingCheckTick";

    private long _startTime;
    private static int MAX_SPEEDOVERCOUNT = 2;
    private int _speedOverCount = 0;
    //加速判断次数

    #if UNITY_EDITOR
    //加速器检测开关
    public static bool CheckSpeedUp = false;
    #else
    //加速器检测开关
    public static bool CheckSpeedUp = true;
    #endif

    private List<string> _passedProcess = new List<string>();

    private List<string> _banAppStrList;

    public List<string> BanAppStrList
    {
        get
        {
            GameDebuger.TODO(@"if (_banAppStrList == null || _banAppStrList.Count == 0)
			{
				_banAppStrList = _banAppStrList ?? new List<string>();
				var appList = DataCache.getArrayByCls<BanApp>();

				for (int i = 0; i < appList.Count; i++)
				{
					_banAppStrList.Add(appList[i].name);
				}
				_banAppStrList.Sort();
			}
                ");
            return _banAppStrList;
        }
    }


    public void Setup()
    {
        StartCheck();
    }


    public void Dispose()
    {
        StopCheck();

        if (_banAppStrList != null)
        {
            _banAppStrList.Clear();
        }

        if (_passedProcess != null)
        {
            _passedProcess.Clear();
        }
    }


    private void StartCheck()
    {
        JSTimer.Instance.SetupTimer(CheatingCheckTick, Tick, CheatingCheckFrequence);

        _speedOverCount = 0;
        _startTime = DateTime.UtcNow.Ticks / 10000;
    }

    private void PauseCheck()
    {
        JSTimer.Instance.PauseTimer(CheatingCheckTick);
    }

    private void ResumeCheck()
    {
        JSTimer.Instance.ResumeTimer(CheatingCheckTick);
    }

    private void StopCheck()
    {
        JSTimer.Instance.CancelTimer(CheatingCheckTick);
    }

    private void Tick()
    {
        if (HasCheating())
        {
            CheatingHandle();
        }
    }

    private bool HasCheating()
    {
        return HasCheatingProcess() || IsSpeedUp();
    }


    private bool IsSpeedUp()
    {
        if (!CheckSpeedUp)
        {
            return false;
        }

        long nowTime = DateTime.UtcNow.Ticks / 10000;
        long passTime = nowTime - _startTime;
        //Debug.LogError("OnCheckSpeedUpTimer " + passTime);
        // 误差计算
        if (passTime < (CheatingCheckFrequence - 1) * 1000)
        {
            _speedOverCount++;
            GameDebuger.LogError("@检测加速时间=" + passTime);

            if (_speedOverCount >= MAX_SPEEDOVERCOUNT)
            {
                return true;
            }
        }
        else
        {
            _speedOverCount = 0;
        }

        _startTime = nowTime;

        return false;
    }


    private bool HasCheatingProcess()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WindowsEditor)
        {
            return false;
        }

        GameDebuger.Log("开始应用检测！");

        var runProcess = SystemProcess.GetRunningProcess();
        if (runProcess == null)
        {
            return false;
        }

        if (null == BanAppStrList || BanAppStrList.Count <= 0)
            return false;

        for (int i = 0; i < runProcess.Length; i++)
        {
            var process = runProcess[i];

            if (_passedProcess.Contains(process))
            {
                continue;
            }

            for (int j = 0; j < BanAppStrList.Count; j++)
            {
                var result = process.CompareTo(BanAppStrList[j]);

                if (result == 0)
                {
                    return true;
                }
                else if (result < 0)
                {
                    break;
                }
            }
            _passedProcess.Add(process);
        }

        return false;
    }

    private void CheatingHandle()
    {
        StopCheck();

        GameDebuger.TODO("ModelManager.Player.StopAutoRun();");
        SocketManager.Instance.Close(false);
        ExitGameScript.NeedReturnToLogin = true;
        ExitGameScript.OpenExitTipWindow("系统检测到您的账号异常，请重新登陆");
    }
}