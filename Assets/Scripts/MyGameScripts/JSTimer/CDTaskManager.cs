using CDTask = JSTimer.CdTask;
using OnCdUpdate = JSTimer.CdTask.OnCdUpdate;
using OnCdFinish = JSTimer.CdTask.OnCdFinish;
using System.Collections.Generic;
using System;


/// <summary>
/// CDTask 统一管理的容器，组件化思想，需要时添加即可，记得销毁。
/// 主要还是方便把一个类中所有的CDTask进行集中的、统一的管理。
/// @MarsZ 2017-03-22 15:32:19
/// </summary>
public class CDTaskManager:IDisposable
{
    private Dictionary<string,CDTask> mCDTaskDic;

    public CDTaskManager()
    {
        mCDTaskDic = new Dictionary<string, CDTask>();
    }

    #region interface

    public CDTask AddOrResetCDTask(string taskName, 
        float totalTime, 
        OnCdUpdate onUpdate = null, 
        OnCdFinish onFinished = null, 
        float updateFrequence = 0.1f, 
        bool timeScale = false)
    {
        CDTask tCDTask = FindCDTaskFromDic(taskName);
        if (null == tCDTask)
        {
            tCDTask = JSTimer.Instance.SetupCoolDown(taskName, totalTime, onUpdate, onFinished, updateFrequence, timeScale);
            mCDTaskDic.Add(taskName, tCDTask);
        }
        else
            tCDTask.Reset(totalTime, onUpdate, onFinished, updateFrequence, timeScale);
        return tCDTask;
    }

    public void RemoveCDTask(string taskName)
    {
        CDTask tCDTask = FindCDTaskFromDic(taskName);
        if (null == tCDTask)
            return;
        DisposeCDTask(tCDTask);
        mCDTaskDic.Remove(taskName);
    }

    public void Dispose()
    {
        if (null == mCDTaskDic || mCDTaskDic.Count <= 0)
            return;
        foreach (KeyValuePair<string,CDTask> tKeyValuePair in mCDTaskDic)
        {
            DisposeCDTask(tKeyValuePair.Value);
        }
        mCDTaskDic.Clear();
        mCDTaskDic = null;
    }

    #endregion

    private void DisposeCDTask(CDTask pCDTask)
    {
        if (null == pCDTask)
            return;
        JSTimer.Instance.CancelCd(pCDTask.taskName);
        pCDTask.Dispose();
        pCDTask = null;
    }

    private CDTask FindCDTaskFromDic(string taskName)
    {
        if (null == mCDTaskDic || mCDTaskDic.Count <= 0)
            return null;
        CDTask tCDTask = null;
        if (mCDTaskDic.TryGetValue(taskName, out tCDTask))
            return tCDTask;
        return null;
    }
}