using UnityEngine;
using System.Collections.Generic;
using Assets.Standard_Assets.NGUI.CustomExtension.Manager;

public class JSTimer : MonoBehaviour
{
    private static JSTimer _instance;

    public static JSTimer Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("JSTimer");
                _instance = go.AddComponent<JSTimer>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public abstract class Task
    {
        public string taskName;
        //onUpdate回调频率，默认为每0.1秒回调一次
        public float updateFrequence = 0.1f;
        //记录当前累计时间
        public float cumulativeTime;
        //是否暂停
        public bool isPause;
        //是否受timeScale影响
        public bool timeScale;
        //是否有效
        public bool isValid;
   }

    public class TimerTask : Task
    {
        public delegate void OnTimerUpdate();

        public OnTimerUpdate onUpdate;

        public TimerTask(string taskName, OnTimerUpdate onUpdate, float updateFrequence, bool timeScale)
        {
            this.taskName = taskName;
            Reset(onUpdate, updateFrequence, timeScale);
        }

        public void Reset(OnTimerUpdate onUpdate, float updateFrequence, bool timeScale)
        {
            this.cumulativeTime = 0f;
            this.updateFrequence = updateFrequence;
            this.onUpdate = onUpdate;
            this.isPause = false;
            this.timeScale = timeScale;
            this.isValid = true;
        }

        public void Cancel()
        {
            this.isValid = false;
        }

        public void DoUpdate()
        {
            if (onUpdate != null)
                onUpdate();
        }

        public void Dispose()
        {
            onUpdate = null;
        }
    }

    public class CdTask : Task
    {
        public delegate void OnCdUpdate(float remainTime);

        public delegate void OnCdFinish();

        public OnCdUpdate onUpdate;
        public OnCdFinish onFinished;

        //倒计时总时间（单位：秒）
        public float totalTime;
        //剩余时间（单位：秒）
        public float remainTime;

        public CdTask(string taskName, float totalTime, OnCdUpdate onUpdate, OnCdFinish onFinished, float updateFrequence, bool timeScale)
        {
            this.taskName = taskName;
            Reset(totalTime, onUpdate, onFinished, updateFrequence, timeScale);
        }

        public void Reset(float totalTime, OnCdUpdate onUpdate, OnCdFinish onFinished, float updateFrequence = 0.1f, bool timeScale = false)
        {
            this.totalTime = totalTime;
            this.remainTime = totalTime;
            this.onUpdate = onUpdate;
            this.onFinished = onFinished;

            this.updateFrequence = updateFrequence;
            this.cumulativeTime = 0f;
            this.isPause = false;
            this.timeScale = timeScale;
            this.isValid = true;

            JSTimer.Instance.AddCdIsNotExist(this); 
        }

        public void DoFinish()
        {
            if (onFinished != null)
                onFinished();
        }

        public void DoUpdate()
        {
            if (onUpdate != null)
                onUpdate(this.remainTime);
        }

        public void Dispose()
        {
            onUpdate = null;
            onFinished = null;
        }
    }

    private List<CdTask> _cdTasks = new List<CdTask>(32);

    public List<CdTask> CdTasks
    {
        get
        {
            return _cdTasks;
        }
    }

    private List<TimerTask> _timerTasks = new List<TimerTask>(32);

    public List<TimerTask> TimerTasks
    {
        get
        {
            return _timerTasks;
        }
    }

    #region CoolDown Func

    /// <summary>
    /// 设置倒计时器.
    /// </summary>
    /// <param name="taskName">Task name.</param>
    /// <param name="totalTime">倒计时总时长</param>
    /// <param name="onUpdate">Update事件处理委托.</param>
    /// <param name="onFinished">Finish事件处理委托.</param>
    /// <param name="updateFrequence">Update频率.</param>
    /// <param name="timeScale">是否受timeScale影响.</param>
    public CdTask SetupCoolDown(
        string taskName
        , float totalTime
        , CdTask.OnCdUpdate onUpdate
        , CdTask.OnCdFinish onFinished
        , float updateFrequence = 0.1f
        , bool timeScale = false)
    {
        if (string.IsNullOrEmpty(taskName))
            return null;

        if (totalTime <= 0)
        {
            if (onFinished != null)
            {
                onFinished();
            }
            return null;
        }

        CdTask cdTask = GetCdTask(taskName);
        if (cdTask != null)
        {
            cdTask.Reset(totalTime, onUpdate, onFinished, updateFrequence, timeScale);
        }
        else
        {
            cdTask = new CdTask(taskName, totalTime, onUpdate, onFinished, updateFrequence, timeScale);
//            _cdTasks.Add(cdTask);//MarsZ：统一在Reset中调用添加到列表。因为Reset外部调用时不会添加到列表。2017-02-21 15:07:08
        }

        return cdTask;
    }

    public CdTask GetCdTask(string taskName)
    {
        return _cdTasks.Find((task) =>
        {
            return task.taskName.Equals(taskName);
        });
    }

    int frameDelayUniqueIndex = 0;

    /// <summary>
    /// 下一帧再执行
    /// </summary>
    /// <param name="onFinish"></param>
    public void SetNextFrame(CdTask.OnCdFinish onFinish)
    {
        if (frameDelayUniqueIndex == int.MaxValue)
            frameDelayUniqueIndex = 0;

        //设置一个远小于一帧的时间~
        SetupCoolDown("frameDelayEvent" + frameDelayUniqueIndex++, 0.000001f, null, onFinish);
    }
    public bool IsCdExist(string taskName)
    {
        return GetCdTask(taskName) != null;
    }

    public bool AddCdIsNotExist(CdTask pCdTask)
    {
        if (null == pCdTask)
            return false;
        if (IsCdExist(pCdTask.taskName))
            return false;
        _cdTasks.Add(pCdTask);
        return true;
    }

    public bool PauseCd(string taskName)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.isPause = true;
            return true;
        }

        return false;
    }

    public bool ResumeCd(string taskName)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.isPause = false;
            return true;
        }

        return false;
    }

    public void CancelCd(string taskName)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.isValid = false;
        }
    }

    public float GetRemainTime(string taskName)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
            return task.remainTime;
        else
            return 0f;
    }

    public void AddCdUpdateHandler(string taskName, CdTask.OnCdUpdate updateHandler)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.onUpdate -= updateHandler;
            task.onUpdate += updateHandler;
        }
    }

    public void RemoveCdUpdateHandler(string taskName, CdTask.OnCdUpdate updateHandler)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.onUpdate -= updateHandler;
        }
    }

    public void AddCdFinishHandler(string taskName, CdTask.OnCdFinish finishHandler)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.onFinished -= finishHandler;
            task.onFinished += finishHandler;
        }
    }

    public void RemoveCdFinishHandler(string taskName, CdTask.OnCdFinish finishHandler)
    {
        CdTask task = GetCdTask(taskName);
        if (task != null)
        {
            task.onFinished -= finishHandler;
        }
    }

    #endregion

    #region Timer Func

    /// <summary>
    /// 设置计时器，注意计时器需要手动移除，不然会一直计时下去，所以没有onFinish回调
    /// </summary>
    /// <param name="taskName">Task name.</param>
    /// <param name="onUpdate">Update事件处理委托.</param>
    /// <param name="updateFrequence">Update频率.</param>
    /// <param name="timeScale">是否受timeScale影响.</param>
    public TimerTask SetupTimer(string taskName, TimerTask.OnTimerUpdate onUpdate, float updateFrequence = 0.1f, bool timeScale = false)
    {
        if (string.IsNullOrEmpty(taskName))
            return null;

        var timerTask = GetTimerTask(taskName);
        if (timerTask != null)
        {
            timerTask.Reset(onUpdate, updateFrequence, timeScale);
        }
        else
        {
            timerTask = new TimerTask(taskName, onUpdate, updateFrequence, timeScale);
            _timerTasks.Add(timerTask);
        }

        return timerTask;
    }

    public TimerTask GetTimerTask(string taskName)
    {
        return _timerTasks.Find((task) =>
        {
            return task.taskName.Equals(taskName);
        });
    }

    public bool PauseTimer(string taskName)
    {
        var task = GetTimerTask(taskName);
        if (task != null)
        {
            task.isPause = true;
            return true;
        }

        return false;
    }

    public bool ResumeTimer(string taskName)
    {
        var task = GetTimerTask(taskName);
        if (task != null)
        {
            task.isPause = false;
            return true;
        }

        return false;
    }

    public void CancelTimer(string taskName)
    {
        TimerTask task = GetTimerTask(taskName);
        if (task != null)
        {
            task.isValid = false;
        }
    }

    public void AddTimerUpdateHandler(string taskName, TimerTask.OnTimerUpdate updateHandler)
    {
        TimerTask task = GetTimerTask(taskName);
        if (task != null)
        {
            task.onUpdate -= updateHandler;
            task.onUpdate += updateHandler;
        }
    }

    public void RemoveTimerUpdateHandler(string taskName, TimerTask.OnTimerUpdate updateHandler)
    {
        TimerTask task = GetTimerTask(taskName);
        if (task != null)
        {
            task.onUpdate -= updateHandler;
        }
    }

    #endregion

    private List<TimerTask> _timerToRemove = new List<TimerTask>();
    private List<CdTask> _coolDownToRemove = new List<CdTask>();

    void Update()
    {
        UILoopManager.FrameLoop();
        //更新计时器任务
        for (int i = 0, imax = _timerTasks.Count; i < imax; ++i)
        {
            TimerTask timerTask = _timerTasks[i];
            if (timerTask.isValid)
            {
                if (timerTask.isPause)
                    continue;

                float deltaTime = timerTask.timeScale ? Time.deltaTime : Time.unscaledDeltaTime;
                timerTask.cumulativeTime += deltaTime;
                if (timerTask.cumulativeTime >= timerTask.updateFrequence)
                {
                    timerTask.cumulativeTime = 0f;
                    timerTask.DoUpdate();
                }
            }
            else
            {
                _timerToRemove.Add(timerTask);
            }
        }

        if (_timerToRemove.Count > 0)
        {
            for (int i = 0; i < _timerToRemove.Count; ++i)
            {
                TimerTask timerTask = _timerToRemove[i];
                timerTask.Dispose();
                _timerTasks.Remove(timerTask);
            }

            _timerToRemove.Clear();
        }

        //更新倒计时任务
        for (int i = 0, imax = _cdTasks.Count; i < imax; ++i)
        {
            CdTask cdTask = _cdTasks[i];
            if (cdTask.isValid)
            {
                if (cdTask.isPause)
                    continue;

                float deltaTime = cdTask.timeScale ? Time.deltaTime : Time.unscaledDeltaTime;
                cdTask.remainTime -= deltaTime;
                if (cdTask.remainTime <= 0f)
                {
                    cdTask.remainTime = 0f;
                    cdTask.isValid = false;
                    cdTask.DoUpdate();
                    cdTask.DoFinish();
                }
                else
                {
                    cdTask.cumulativeTime += deltaTime;
                    if (cdTask.cumulativeTime >= cdTask.updateFrequence)
                    {
                        cdTask.cumulativeTime = 0f;
                        cdTask.DoUpdate();
                    }
                }
            }
            else
            {
                _coolDownToRemove.Add(cdTask);
            }
        }

        if (_coolDownToRemove.Count > 0)
        {
            for (int i = 0; i < _coolDownToRemove.Count; ++i)
            {
                CdTask cdTask = _coolDownToRemove[i];
                cdTask.Dispose();
                _cdTasks.Remove(cdTask);
            }

            _coolDownToRemove.Clear();
        }
    }

    public void Dispose()
    {
        _cdTasks.Clear();
        _timerTasks.Clear();
    }
}
