using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using UnityEngine;

public sealed class BattleStatusEffectManager
{
    private static BattleStatusEffectManager _instance;

    private Dictionary<long, Queue<EffectCtrlData>> _skillEfectDic = new Dictionary<long, Queue<EffectCtrlData>>();

    private static BattleStatusEffectManager Create()
    {
        var _ins = new BattleStatusEffectManager();
        return _ins;
    }

    private BattleStatusEffectManager()
    {
    }

    public static BattleStatusEffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Create();
            }
            return _instance;
        }
    }

    public void Dispose()
    {
        _skillEfectDic.Values.ForEach(q =>
        {
            q.Clear();
        });
        _skillEfectDic.Clear();
        
    }

    public void AddEffect(Transform hudTransform, string effectName, long id)
    {
        if (hudTransform == null
            || string.IsNullOrEmpty(effectName)) return;

        var com = CreateUIBattleStatusEffect(id);
        if (com != null)
        {
            com.ShowStatusEffect(hudTransform, effectName);
        }
    }

    public void PlayDamage(
        string msg
        , Transform target
        , float duration
        , int fontType
        , long id
        , float scale = 1f
        , string effName = "")
    {
        var data = new EffectCtrlData
        {
            target = target,
            msg = msg,
            fontIndex = fontType,
            duration = duration,
            effectName = effName,
            _tweenType = EffectCtrlData.TweenType.Damege
        };

        AddData(id, data);
        
        Play(id);
    }

    private void AddData(long id, EffectCtrlData data)
    {
        Queue<EffectCtrlData> q = null;
        _skillEfectDic.TryGetValue(id, out q);
        if (q == null)
        {
            q = new Queue<EffectCtrlData>();
            _skillEfectDic.Add(id, q);
        }
        
        q.Enqueue(data);
    }

    public void PlaySkillName(Transform hudTransform, string name)
    {
        var com = CreateUIBattleStatusEffect();
        if (com != null)
        {
            com.ShowSkillName(name, hudTransform);
        }
    }

    private void Play(long id)
    {
        var taskName = "timer_" + id;
        var cdTask = JSTimer.Instance.GetCdTask(taskName);

        if (cdTask != null && !cdTask.isPause)
        {
            return;
        }

        Queue<EffectCtrlData> q = null;
        _skillEfectDic.TryGetValue(id, out q);

        if (q == null || q.Count <= 0)
        {
            cdTask.Dispose();
            JSTimer.Instance.PauseCd(taskName);
            return;
        }

        var data = q.Dequeue();
        var com = CreateUIBattleStatusEffect();
        if (com != null)
        {
            com.ShowEffect(data);
        }
         
        JSTimer.Instance.SetupCoolDown(taskName, 0.5f, null, () =>
        {
            JSTimer.Instance.PauseCd(taskName);
            Play(id);
        });
    }

    private UIBattleStatusEffectController CreateUIBattleStatusEffect(long id = 0)
	{
		var fontGo = ResourcePoolManager.Instance.SpawnUIGo(BattleHUDText.NAME, LayerManager.Root.BattleUIHUDPanel.cachedGameObject);
        var com = fontGo.GetMissingComponent<UIBattleStatusEffectController>();
		return com;
	}
}