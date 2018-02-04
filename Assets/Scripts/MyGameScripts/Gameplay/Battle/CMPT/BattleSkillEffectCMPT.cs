using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 战斗中用到的特效组件
/// @MarsZ 2017年10月12日15:27:10
/// </summary>
public class BattleSkillEffectCMPT : IDisposable
{
    //战斗特效列表 一个monster可以有多个相同特效  
    private List<BattleSkillEffectInfo> _battleEffectList;

    public BattleSkillEffectCMPT()
    {
        if (_battleEffectList == null)
        {
            _battleEffectList = new List<BattleSkillEffectInfo>();
        }
    }

    private void AddToEffectList(BattleSkillEffectInfo pEffectGameObject)
    {
        if (null == _battleEffectList)
            _battleEffectList = new List<BattleSkillEffectInfo>();
        if (_battleEffectList.Contains(pEffectGameObject))
        {
            GameDebuger.LogError(string.Format("AddToEffectList failed, it's exist yet ! , pEffectGameObject:{0}", pEffectGameObject));
            return;
        }
        _battleEffectList.Add(pEffectGameObject);
    }

    private bool RemoveFromEffectList(BattleSkillEffectInfo pEffectGameObject)
    {
        if (null == _battleEffectList || _battleEffectList.Count <= 0)
            return false;
        return _battleEffectList.Remove(pEffectGameObject);
    }

    public void ClearEffect(BattleSkillEffectInfo pEffectTime)
    {
        if (null == pEffectTime)
            return;
        RemoveFromEffectList(pEffectTime);
        pEffectTime.Dispose();
        pEffectTime = null;
    }

    private void CleanBattleEffects()
    {
        if (null == _battleEffectList || _battleEffectList.Count <= 0)
            return;
        List<BattleSkillEffectInfo> tEffectList = _battleEffectList.ToList();
        for (int i = 0; i < tEffectList.Count; i++)
            ClearEffect(tEffectList[i]);
        _battleEffectList.Clear();          
    }

    public void Dispose()
    {
        CleanBattleEffects();
    }

    public BattleSkillEffectInfo CreateBattleSkillEffectInfo(
        GameObject tEffectRoot
        , GameObject pParentContainer
        , string effName
        , Action<GameObject> effectDisposeCallback = null)
    {

        var effectTime = tEffectRoot.GetComponent<EffectTime>();
        if (effectTime == null)
        {
            effectTime = tEffectRoot.AddComponent<EffectTime>();
            effectTime.time = 5;
        }

        BattleSkillEffectInfo tBattleSkillEffectInfo = new BattleSkillEffectInfo();
        tBattleSkillEffectInfo.MainEffectTime = effectTime;
        tBattleSkillEffectInfo.EffectDisposeCallback = (pPoolEffect) =>
        {
            if (null != effectDisposeCallback)
                effectDisposeCallback(pPoolEffect);

            RemoveFromEffectList(tBattleSkillEffectInfo);
        };

        tBattleSkillEffectInfo.EffectFinishCallback = (pEffectTime) =>
        {
            if (null != tBattleSkillEffectInfo)
                ClearEffect(tBattleSkillEffectInfo);
        };
        GameObjectExt.AddPoolChild(pParentContainer, tEffectRoot);
        AddToEffectList(tBattleSkillEffectInfo);
        //fish todo: 其实有一个点未处理：IsMainEffectFromPool = false;
        tBattleSkillEffectInfo.AddPoolEffectFromParent(tEffectRoot.transform);
        tBattleSkillEffectInfo.EffName = effName;
        return tBattleSkillEffectInfo;
    }

    public void RemoveEffectsByName(string effname)
    {
        var set = _battleEffectList.Filter(e =>string.Equals(e.EffName.ToLower(), effname.ToLower()));
        set.ToList().ForEach<BattleSkillEffectInfo>(effectInfo =>
        {
            effectInfo.MainEffectTime.Stop();
            ClearEffect(effectInfo);
        });
    }

    public GameObject GetEffGameObject(string effName)
    {
        var info = _battleEffectList.Find<BattleSkillEffectInfo>(
            e=>!string.IsNullOrEmpty(e.EffName) && string.Equals( e.EffName, effName));
        return info != null ? info.MainEffectTime.gameObject : null;
    }

    public EffectTime GetEffTimeByName(string effName)
    {
        var info = _battleEffectList.Find<BattleSkillEffectInfo>(e=>!string.IsNullOrEmpty(e.EffName) && string.Equals( e.EffName, effName));
        return info == null ? null : info.MainEffectTime;
    }
}

public class BattleSkillEffectInfo : IDisposable
{
    /**特效销毁回调*/
    public Action<GameObject> EffectDisposeCallback;

    private Action<EffectTime> mEffectFinishCallback;

    /**特效播放结束*/
    public Action<EffectTime> EffectFinishCallback
    {
        get{ return mEffectFinishCallback; }
        set
        {
            mEffectFinishCallback = value;
            mMainEffectTime.OnFinish = mEffectFinishCallback;
        }
    }

    /**预设类特效的临时父级，用于一个预设特效需要复制多份一起显示时*/
    private  EffectTime mMainEffectTime;
    private List<GameObject> poolEffectList;
    /**主要MainEffectTime是否来自缓存池*/
    private bool IsMainEffectFromPool = true;

    public void Dispose()
    {
        if (null != MainEffectTime)
        {
            MainEffectTime.RemoveComponent<EffectTime>();
            if (!IsMainEffectFromPool)//如果主GO不是从池中拿的，直接销毁
            {
//                GameDebuger.LogError(string.Format("destroy MainEffectTime:{0},GetHashCode:{1}",MainEffectTime,MainEffectTime.GetHashCode()));
                NGUITools.Destroy(MainEffectTime.gameObject);//这个层级的GO是自己new出来的GO，已经空的，直接销毁即可。2017-01-06 15:35:59
            }
        }
        
        poolEffectList.ForEach<GameObject>((pPoolEffect) =>
            {
                if (null != EffectDisposeCallback)
                    EffectDisposeCallback(pPoolEffect);
                AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(pPoolEffect);
            });
        poolEffectList = null;
    }
 
    private bool AddPoolEffect(GameObject pGameObject)
    {
        if (null == poolEffectList)
            poolEffectList = new List<GameObject>();
        if (poolEffectList.Contains(pGameObject))
            return false;
        poolEffectList.Add(pGameObject);
        return true;
    }

    public bool AddPoolEffectFromParent(Transform pParent)
    {
        if (null == pParent)
            return false;
        IsMainEffectFromPool = false;
        for (int tCounter = 0, tLen = pParent.childCount; tCounter < tLen; tCounter++)
        {
            AddPoolEffect(pParent.GetChild(tCounter).gameObject);
        }
        return true;
    }

    public string EffName
    {
        get { return mMainEffectTime == null ? string.Empty : mMainEffectTime.effName; }
        set { 
            if (mMainEffectTime != null && !string.IsNullOrEmpty(value))
                mMainEffectTime.effName = value.ToLower();
        }
    }

    public  EffectTime MainEffectTime
    {
        get{ return mMainEffectTime; }
        set
        { 
            mMainEffectTime = value;
            if (null != mMainEffectTime)
            {
//                GameDebuger.LogError(string.Format("set mMainEffectTime:{0},GetHashCode:{1}", mMainEffectTime, mMainEffectTime.GetHashCode()));
                mMainEffectTime.OnFinish = EffectFinishCallback;
            }
        }
    }
}

