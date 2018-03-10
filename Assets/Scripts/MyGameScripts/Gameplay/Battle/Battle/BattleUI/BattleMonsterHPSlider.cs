using AssetPipeline;
using UnityEngine;
using System.Collections.Generic;
using AppDto;

public class BattleMonsterHPSlider : MonoBehaviour
{
    private float _hpPrecent;
    private float _lerpBaseTime;
    private float _lerpTime;

    private MonsterController _monster;
    private float _oriFillAmount = -1;
    private UISprite readySprite;
    private UISlider mCDDurationSlider;
    private UISprite mCDDurationSprite;
    private MonsterHPSlider mMonsterHPSlider;
    private GameObject mCntrBuffIconPrefab;
    private UIGrid mBuffGrid;

    private UISlider slider;

    //BUFF效果图标字典
    private Dictionary<int,GameObject> mBuffEffectGODic = null;

    public static BattleMonsterHPSlider CreateNew(MonsterController mc)
    {
        var hpSliderPrefab = ResourcePoolManager.Instance.LoadUI("MonsterHPSlider") as GameObject;
        var hpSliderGO = NGUITools.AddChild(LayerManager.Root.BattleUIHUDPanel.cachedGameObject, hpSliderPrefab);
        var follower = hpSliderGO.AddComponent<UIFollowTarget>();

        follower.gameCamera = LayerManager.Root.BattleCamera_Camera;
        follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;

        var hpSlider = hpSliderGO.GetMissingComponent<BattleMonsterHPSlider>();
        hpSlider.SetupView();
        hpSlider.UpdateFollowTarget(mc);
        hpSlider.Setup(mc);
        return hpSlider;
    }

    private void SetupView()
    {
        mMonsterHPSlider = BaseView.Create<MonsterHPSlider>(gameObject);
        slider = mMonsterHPSlider.HPSlider_UISlider;
        readySprite = mMonsterHPSlider.ReadySprite_UISprite;
        mCDDurationSlider = mMonsterHPSlider.CDSlider_UISlider;
        mCDDurationSprite = mMonsterHPSlider.Front_Sprite_UISprite;
        mBuffGrid = mMonsterHPSlider.BuffGrid_UIGrid;
        mCntrBuffIconPrefab = mMonsterHPSlider.CntrBuffIcon;
    }

    public void UpdateFollowTarget(MonsterController mc)
    {
        var tf = mc.GetMountHUD();

        if (tf == null)
        {
            tf = mc.gameObject.transform;
            UpdateFollowTarget(tf);
        }
        else
        {
            UpdateFollowTarget(tf, new Vector3(0, -0.1f, 0));
        }
    }

    public void UpdateFollowTarget(Transform mc)
    {
        UpdateFollowTarget(mc,new Vector3(0, 2f, 0));
    }

    private void UpdateFollowTarget(Transform mc,Vector3 pOffset)
    {
        var follower = gameObject.GetComponent<UIFollowTarget>();
        if (follower == null)
        {
            return;
        }
        var tf = mc;
        follower.offset = pOffset;
        follower.target = tf;
    }

    private void Setup(MonsterController mc)
    {
        _monster = mc;
        mBuffEffectGODic = new Dictionary<int, GameObject>();
        UpdateUIVisibleBySide(null != _monster && _monster.side == BattlePosition.MonsterSide.Player);
        AddEvents();
        slider.value = _monster.currentHP / (float)_monster.MaxHP;
        ShowReady(false);
        mCDDurationSlider.value = 1f;
    }

    private void Update()
    {
        if (_monster != null)
        {
            ShowReady(_monster.NeedReady);

            _hpPrecent = _monster.CurHp / (float)_monster.MaxHP;

            if (slider.value != _hpPrecent)
            {
                if (_oriFillAmount == -1f)
                {
                    _lerpTime = 0;
                    _oriFillAmount = slider.value;

                    if (_hpPrecent == 0f)
                    {
                        _lerpBaseTime = 4f;
                    }
                    else
                    {
                        float modifyPrecent = Mathf.Abs(_oriFillAmount - _hpPrecent);
                        _lerpBaseTime = 1f / (1f * modifyPrecent);
                    }
                }
                _lerpTime += _lerpBaseTime * Time.deltaTime;
                slider.value = Mathf.Lerp(_oriFillAmount, _hpPrecent, _lerpTime);
            }
            else
            {
                _oriFillAmount = -1f;
                //if (_hpPrecent == 0f)
                //{
                //    //this.gameObject.SetActive(false);
                //}
            }
            if (_monster.IsDead())
            {
                ShowCD(0);
                ShowReady(false);
            }
        }
    }

    private void ShowReady(bool ready)
    {
        float tAlpha = ready ? 1f : 0f;
        if (readySprite.alpha != tAlpha)
            readySprite.alpha = tAlpha;
    }

    public void ShowCD(float pDuration = 0, bool pPlayReverse = false, JSTimer.CdTask.OnCdFinish pOnCdFinish = null)
    {
//        if (_monster.videoSoldier.id == ModelManager.Player.GetPlayerId())
//            GameDebuger.LogError(string.Format("ShowCD pDuration:{0},name:{1}", pDuration, _monster.videoSoldier.name));
        if (pDuration <= 0)
        {
            if (mCDDurationSlider.value != 0)
                mCDDurationSlider.value = 0;
            if (null != pOnCdFinish)
                pOnCdFinish();
        }
        else
        {
            if (_monster.IsDead())
            {
                if (null != pOnCdFinish)
                    pOnCdFinish();
                return;
            }
            string mCDTimerName = "ShowCD" + GetInstanceID().ToString();
            JSTimer.Instance.CancelCd(mCDTimerName);
            JSTimer.CdTask.OnCdUpdate tOnCdUpdate = null;
            if (null != mCDDurationSlider && mCDDurationSlider.enabled)
            {
                mCDDurationSlider.value = pPlayReverse ? 0f : 1f;     

                tOnCdUpdate = (pRemainTime) =>
                {
                    mCDDurationSlider.value = (pPlayReverse ? (pDuration - pRemainTime) : pRemainTime) / pDuration;
                };
            }
            JSTimer.Instance.SetupCoolDown(mCDTimerName, pDuration, tOnCdUpdate, pOnCdFinish, 0.01f);
        }
    }

    //历史潜规则导致通过设置gameobject的active会失效，此处通过控制脚本的enable达到近乎同样的目的
    private void UpdateUIVisibleBySide(bool pVisible)
    {
        mCDDurationSlider.enabled = mCDDurationSprite.enabled = pVisible;
    }

    public void Destroy()
    {
        RemoveEvents();
        ClearBuffEffect();
        mBuffEffectGODic = null;
        _monster = null;
        Destroy(gameObject);
    }

    private void AddEvents()
    {
        GameEventCenter.AddListener(GameEvent.BATTLE_FIGHT_BUFF_STATUS_CHANGED, OnBuffStatusChanged);
    }

    private void RemoveEvents()
    {
        GameEventCenter.RemoveListener(GameEvent.BATTLE_FIGHT_BUFF_STATUS_CHANGED, OnBuffStatusChanged);
    }

    #region BUFF效果列表

    private void AddBuffEffect(SkillBuff pSkillBuff)
    {
        if (null == pSkillBuff || (null != mBuffEffectGODic && mBuffEffectGODic.ContainsKey(pSkillBuff.id)))
            return;
        var tGameObject = GameObjectExt.AddChild(mBuffGrid.gameObject, mCntrBuffIconPrefab);
        mBuffEffectGODic.Add(pSkillBuff.id, tGameObject);
        UpdateBuffIcon(tGameObject, pSkillBuff.uiHeadEffect);
        tGameObject.GetComponent<UIWidget>().alpha = 1f;
        mBuffGrid.RepositionDelay();
    }

    private void RemoveBuffEffect(int pBuffId)
    {
        if (pBuffId <= 0 || null == mBuffEffectGODic || mBuffEffectGODic.Count <= 0)
            return;
        GameObject tGameObject = null;
        if (mBuffEffectGODic.TryGetValue(pBuffId, out tGameObject))
        {
            mBuffEffectGODic.Remove(pBuffId);
            GameObjectExt.DestroyLog(tGameObject);
        }
    }

    private void ClearBuffEffect()
    {
        if (null == mBuffEffectGODic)
            return;
        List<int> tBuffIdList = mBuffEffectGODic.Keys.ToList();
        if (null == tBuffIdList || tBuffIdList.Count <= 0)
            return;
        for (int tCounter = 0; tCounter < tBuffIdList.Count; tCounter++)
        {
            RemoveBuffEffect(tBuffIdList[tCounter]);
        }
        mBuffEffectGODic.Clear();
        mBuffEffectGODic = null;
    }

    private void UpdateBuffIcon(GameObject pBuffIcon, string pBuffEffect)
    {
        var sprite = pBuffIcon.GetComponentInChildren<UISprite>();
        UIHelper.SetSkillIcon(sprite, pBuffEffect);
    }

    private void OnBuffStatusChanged(long pMonsterUID, SkillBuff pBuff, bool pIsBuffAdded)
    {
        if (null == _monster || pMonsterUID <= 0 || null == pBuff || _monster.GetId() != pMonsterUID)
            return;
        if (pIsBuffAdded)
            AddBuffEffect(pBuff);
        else
            RemoveBuffEffect(pBuff.id);
    }

    #endregion

    public void SetActive(bool b)
    {
        mMonsterHPSlider.MonsterHPSlider_UIWidget.alpha = b ? 1f : 0f;
    }
}