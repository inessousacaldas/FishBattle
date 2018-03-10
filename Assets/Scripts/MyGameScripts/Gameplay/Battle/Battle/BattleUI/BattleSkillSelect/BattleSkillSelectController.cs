// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleSkillSelectView.cs
// Author   : SK
// Created  : 2014/11/25
// Purpose  : 
// **********************************************************************
using UnityEngine;
using System.Collections;
using AppDto;
using System.Collections.Generic;
using System;

public class BattleSkillSelectController : MonoViewController<BattleSkillSelectView>
{
    #region 主要用于PC端快捷键判断是哪种窗口

    public enum SkillType
    {
        None,
        Stunt,
        Guide,
        Skill,
    }

    public SkillType SkillTypeMode
    {
        get { return _skillTypeMode; }
    }

    private SkillType _skillTypeMode = SkillType.None;

    #endregion

    public static string GuideSkillSelectViewOpen = "GuideSkillSelectViewOpen";


    private const string SKILL_CD_COOL_DOWN = "SKILL_CD_COOL_DOWN_ID_{0}";

    public event Action<Skill> OnSkillSelect;

    private List<SkillCellPrefab> mSkillButtonCellList = new List<SkillCellPrefab>(20);

    /// <summary>
    /// 从DataModel中取得相关数据对界面进行初始化
    /// </summary>

    /// <summary>
    /// Registers the event.
    /// DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnCloseButtonClick);
        GameEventCenter.AddListener(GameEvent.BATTLE_UI_SKILL_COMMON_CD, OnSkillCDUpdate);
    }

    /// <summary>
    /// 关闭界面时清空操作放在这
    /// </summary>
    protected override void OnDispose()
    {
        if (OnSkillSelect != null)
        {
            OnSkillSelect(null);
            OnSkillSelect = null;
        }

        mSkillButtonCellList.Clear();

        //        RemoveCDTask(string.Format(SKILL_CD_COOL_DOWN, _mc.videoSoldier.id));//legacy，cdtask在本类基类中统一管理了，2017-03-22 17:01:22

        _firstSkillWidget = null;
        _skillTypeMode = SkillType.None;

        GameEventCenter.RemoveListener(GameEvent.BATTLE_UI_SKILL_COMMON_CD, OnSkillCDUpdate);
    }

    public void OpenStunt(MonsterController mc, List<int> skills, Action<Skill> OnStuntSelectDelegate)
    {
        _mc = mc;

        OnSkillSelect = OnStuntSelectDelegate;

        UpdateSkillList(skills);

        _skillTypeMode = SkillType.Stunt;
        BattleDataManager.DataMgr.SetState(BattleSceneStat.ON_COMMAND_ENTER);
    }

    public void OpenGuide(MonsterController mc, Action<Skill> OnSkillSelectDelegate)
    {
        _mc = mc;

        OnSkillSelect = OnSkillSelectDelegate;

        var skills = new List<int>();
        skills.Add(1111);
        UpdateSkillList(skills);

        Invoke("DelaySendOpenEvent", 0.1f);

        _skillTypeMode = SkillType.Guide;
    }

    public void Open(MonsterController mc, Action<Skill> OnSkillSelectDelegate)
    {
        _mc = mc;

        OnSkillSelect = OnSkillSelectDelegate;

        if (mc.IsPet())
        {
            ShowPetSkill(mc);
        }
        else
        {
            ShowHeroSkill(mc);
        }

        _skillTypeMode = SkillType.Skill;
    }

    private MonsterController _mc;


    #region event handler

    private void OnCloseButtonClick()
    {
        if (OnSkillSelect != null)
        {
            OnSkillSelect(null);
            OnSkillSelect = null;
        }

        ProxyBattleDemoModule.HideSkillSelect();
    }

    private void OnSkillPointUpdate(long pPlayerUID, int pSkillPoint)
    {
        if (!SuitableToUpdateView(pPlayerUID))
            return;
        UpdateSkillItemList();
    }

    private void OnSkillCDUpdate(long pPlayerUID, float pCDRemain, float pTotalCD)
    {
        if (!SuitableToUpdateView(pPlayerUID))
            return;
        UpdateSkillItemListCD(pCDRemain, pTotalCD);
    }

    #endregion

    private int _skillCount = 0;

    private void ShowHeroSkill(MonsterController mc)
    {
        var skillIds = mc.GetAllSkillIds();

        UpdateSkillList(skillIds, mc.IsCoupleAtBattle(), mc.GetFriendDegree());

        Invoke("DelaySendOpenEvent", 0.1f);
    }

    private void DelaySendOpenEvent()
    {
        UIModuleManager.Instance.SendOpenEvent(GuideSkillSelectViewOpen, this);
    }

    private SkillCellPrefab AddSkillCell(Skill skill)
    {
        if (skill == null)
        {
            return null;
        }

        if (skill.skillType == (int)Skill.SkillType.Passive)
        {
            return null;
        }

        _skillCount++;



        var go = AddCachedChild(View.Table.gameObject, SkillCellPrefab.NAME);

        var skillId = skill.id;
        GameDebuger.TODO(@"if (skillId < 10)
        {
            skillId += 3000;
        }");

        go.name = skillId.ToString();
        var skillCellPrefab = BaseView.Create<SkillCellPrefab>(go);

        UpdateItemUI(skillCellPrefab);

        return skillCellPrefab;
    }

    private void UpdateItemUI(SkillCellPrefab pSkillCellPrefab, Skill pSkill = null)
    {
        if (null == pSkillCellPrefab || null == pSkillCellPrefab.gameObject)
        {
            GameDebuger.LogError(string.Format("[错误]UpdateItemUI  failed , pSkillCellPrefab:{0} or pSkillCellPrefab.gameObject is null !", pSkillCellPrefab));
            return;
        }
        var tSkill = pSkill;
        var tSkillId = 0;
        if (null == tSkill)
        {
            if (!int.TryParse(pSkillCellPrefab.gameObject.name, out tSkillId))
            {
                GameDebuger.LogError(string.Format("[错误]UpdateItemUI failed, pSkillCellPrefab.gameObject.name:{0} is invalid ! ", pSkillCellPrefab.gameObject.name));
                return;
            }
            tSkill = DataCache.getDtoByCls<Skill>(tSkillId);
        }
        tSkillId = tSkillId < 10 ? (tSkillId + 3000) : tSkillId;
        var tTempString = string.Empty;
        var limitTip = string.Empty;
        tTempString = DemoSkillHelper.GetSkillActionPointLimitTip(_mc.GetId(), tSkillId);
        if (!string.IsNullOrEmpty(tTempString))
            limitTip = tTempString;

        ////-trigger.maxHp()*0.05
        GameDebuger.TODO(@"if (!string.IsNullOrEmpty(tSkill.applyHpLimitFormula))
        {
            int value = ExpressionManager.DotSkillFormula('applyHpLimitFormula' + tSkill.id, tSkill.applyHpLimitFormula, _mc.videoSoldier);
            if (_mc.currentHP < Math.Abs(value))
            {
                limitTip = '气血不符';
            }
        }

        if (!string.IsNullOrEmpty(tSkill.applyHpMaxLimitFormula))
        {
            int value = ExpressionManager.DotSkillFormula('applyHpMaxLimitFormula' + tSkill.id, tSkill.applyHpMaxLimitFormula, _mc.videoSoldier);
            if (_mc.currentHP > Math.Abs(value))
            {
                limitTip = '气血不符';
            }
        }

        if (!string.IsNullOrEmpty(tSkill.spendHpFormula))
        {
            int value = ExpressionManager.DoSkillFormula('spendHpFormula' + tSkill.id, tSkill.spendHpFormula, _mc.videoSoldier);
            if (_mc.currentHP < Math.Abs(value))
            {
                limitTip = '气血不足';
            }
        }

        if (!string.IsNullOrEmpty(tSkill.spendMpFormula))
        {
            int value = ExpressionManager.DoSkillFormula('spendMpFormula' + tSkill.id, tSkill.spendMpFormula, _mc.videoSoldier);
            if (_mc.currentEp < Math.Abs(value))
            {
                limitTip = '魔法不足';
            }
        }");

        tTempString = DemoSkillHelper.GetSkillSPLimitTip(_mc, tSkill);
        if (!string.IsNullOrEmpty(tTempString))
            limitTip = tTempString;


        //1813、1814、1815、1816这4个技能优先判断自身当前是否有变身buff（buffid：120）
        if (tSkill.id == 1813 || tSkill.id == 1814 || tSkill.id == 1815 || tSkill.id == 1816)//历史遗留的蛋疼代码，不是我写的，2017-03-06 17:38:19
        {
            if (_mc.ContainsBuff(120) == false)
            {
                limitTip = "需要变身";
            }
        }

        if (IsCoupleSkill(tSkill))
        {
            GameDebuger.TODO(@"if (isCoupleAtBattle == false)
            {
                pSkillCellPrefab.SkillCellPrefab_UISprite.isGrey = true;
                limitTip = '需要配偶上阵';
            }
            else
            {
                if (friendDegree >= tSkill.friendly)
                {
                    pSkillCellPrefab.SkillCellPrefab_UISprite.isGrey = false;
                }
                else
                {
                    pSkillCellPrefab.SkillCellPrefab_UISprite.isGrey = true;
                    limitTip = '好友度不足';
                }
            }");
        }
        else
        {
            pSkillCellPrefab.SkillCellPrefab_UISprite.isGrey = false;
        }

        pSkillCellPrefab.NameLabel_UILabel.text = tSkill.name;
        pSkillCellPrefab.TypeLabel_UILabel.text = limitTip == ""
            ? tSkill.shortDescription
            : limitTip;
        pSkillCellPrefab.TypeLabel_UILabel.color = limitTip == ""
            ? ColorConstant.Color_Battle_SkillCanUseTip
            : ColorConstant.Color_Battle_SkillCanNotUseTip;

        pSkillCellPrefab.SpriteMask_UISprite.fillAmount = 0f;
        UIHelper.SetSkillIcon(pSkillCellPrefab.IconSprite_UISprite, tSkill.icon);
        UIEventListener.BoolDelegate tOnToolTip = (pGameObject, pShow) =>
        {
            GameEventCenter.SendEvent(GameEvent.BATTLE_UI_SHOW_SKILL_TIP, pSkill, pShow);
        };
        if (pSkillCellPrefab.SkillCellPrefab_UISprite.isGrey) return;
        var button = pSkillCellPrefab.SkillCellPrefab_UIButton;
        EventDelegate.Set(button.onClick, delegate
        {
            if (OnSkillSelect != null)
            {
                OnSkillSelect(tSkill);
                OnSkillSelect = null;
            }
            ProxyBattleDemoModule.HideSkillSelect();
        });
        UIEventListener.Get(pSkillCellPrefab.IconSprite_UISprite.gameObject).onTooltip = tOnToolTip;

        if (_firstSkillWidget == null)
        {
            _firstSkillWidget = button.sprite;
        }
    }

    private bool IsCoupleSkill(Skill skill)
    {
        GameDebuger.TODO(@"return skill.relationType == Skill.RelationTypeEnum_Couple;");
        return false;
    }

    private UIWidget _firstSkillWidget = null;

    public UIWidget firstSkillWidget
    {
        get
        {
            return _firstSkillWidget;
        }
    }

    private void ShowPetSkill(MonsterController mc)
    {
        GameDebuger.TODO(@"PetPropertyInfo petPropertyInfo = ModelManager.Pet.GetPetInfoByUID(mc.GetId());

        List<int> skills = petPropertyInfo.GetBattleSkillList();        
");
        var skills = mc.GetAllSkillIds();//DEMO临时修改，正式时用上方注释了的代码，2017-02-28 17:01:40

        UpdateSkillList(skills);
    }

    private void UpdateSkillList(List<int> list, bool isCoupleAtBattle = false, int friendDegree = 0)
    {
        this.gameObject.SetActive(true);
        View.Table.gameObject.RemoveChildren();
        mSkillButtonCellList.Clear();

        _skillCount = 0;

        var checkList = new List<int>();

        var sortList = ChangeSort(list);

        SkillCellPrefab tSkillButtonCell = null;
        for (int i = 0, len = sortList.Count; i < len; i++)
        {
            var skillId = sortList[i];
            if (checkList.Contains(skillId)) continue;
            checkList.Add(skillId);
            var skill = DataCache.getDtoByCls<Skill>(skillId);
            tSkillButtonCell = AddSkillCell(skill);
            if (null != tSkillButtonCell)
                mSkillButtonCellList.Add(tSkillButtonCell);
        }

        //      var trans = View.BGSprite_UISprite.transform;
        //      var pos = trans.localPosition;
        //        trans.OverlayPosition(BattleController.Instance.View.SkillButton.transform);
        //        trans.localPosition = new Vector3(pos.x, trans.localPosition.y, 0);
        //
        if (_skillCount > 6)
        {
            var n = 0;
            if (_skillCount % 2 > 0)
            {
                n = (int)(_skillCount / 2) + 1;
            }
            else
            {
                n = _skillCount / 2;
            }

            View.BGSprite_UISprite.height = 87 * n + 25;
            View.BGSprite_UISprite.width = 510;
            View.Table.columns = 2;
//            View.AnchorContainer.leftAnchor.absolute = -476;
        }
        else
        {
            View.BGSprite_UISprite.height = 87 * _skillCount + 25;
            View.BGSprite_UISprite.width = 265;
            View.Table.columns = 1;
//            View.AnchorContainer.leftAnchor.absolute = -229;
        }


        // 计算多一遍，保证正确
//        JSTimer.Instance.StartCoroutine(UpdateView());

        View.Table.repositionNow = true;

        UpdateSkillItemListCD(BattleDataManager.DataMgr.BattleDemo.CurrentCommonCDLeft, BattleDataManager.DataMgr.BattleDemo.CommonCD);
    }

    private List<int> ChangeSort(List<int> list)
    {
        var result = new List<int>();
        if (list.Count > 6)
        {
            int rowNum = (int)(list.Count / 2);
            if (list.Count % 2 != 0)
            {
                rowNum = (int)(list.Count / 2) + 1;
            }

            for (int i = 0; i < rowNum; i++)
            {
                int a = i;
                int b = rowNum + i;
                if (list.Count > a)
                {
                    result.Add(list[a]);
                }
                if (list.Count > b)
                {
                    result.Add(list[b]);
                }
            }
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i]);
            }
        }


        return result;
    }

    private IEnumerator UpdatePosition()
    {
        yield return null;

        var trans = View.BGSprite_UISprite.transform;
        var pos = trans.localPosition;
        GameEventCenter.SendEvent(GameEvent.BATTLE_UI_UPDATE_POSITION, trans);
        /**trans.OverlayPosition(BattleController.Instance.View.SkillButton.transform);*/
        trans.localPosition = new Vector3(pos.x, trans.localPosition.y, 0);
    }

    private bool SuitableToUpdateView(long pPlayerUID)
    {
        if (BaseView.IsViewDestroy(View) || pPlayerUID <= 0 || null == _mc || null == _mc.videoSoldier || pPlayerUID != _mc.videoSoldier.id)
            return false;
        return true;
    }

    private void UpdateSkillItemList()
    {
        if (null == mSkillButtonCellList || mSkillButtonCellList.Count <= 0)
            return;
        SkillCellPrefab tSkillButtonCell = null;
        for (int tCounter = 0; tCounter < mSkillButtonCellList.Count; tCounter++)
        {
            tSkillButtonCell = mSkillButtonCellList[tCounter];
            UpdateItemUI(tSkillButtonCell);
        }
    }

    //pCurrentRemain:当前剩余倒计时，pTotalDuration总倒计时
    private void UpdateSkillItemListCD(float pCurrentRemain, float pTotalDuration)
    {
        if (null == mSkillButtonCellList || mSkillButtonCellList.Count <= 0)
            return;
        long tPlayerUID = _mc.videoSoldier.id;
        string tCDName = string.Format(SKILL_CD_COOL_DOWN, tPlayerUID);
        JSTimer.CdTask.OnCdUpdate tOnCdUpdate = (pRemain) =>
        {
            if (!SuitableToUpdateView(tPlayerUID))
                RemoveCDTask(tCDName);
            else
                UpdateSkillCDForItemUIList(pRemain / pTotalDuration);
        };
        JSTimer.CdTask.OnCdFinish tOnCdFinish = () =>
        {
            RemoveCDTask(tCDName);
        };
        JSTimer.CdTask tCdTask = AddOrResetCDTask(tCDName, pTotalDuration, tOnCdUpdate, tOnCdFinish, 0.01f);
        tCdTask.remainTime = pCurrentRemain;
    }

    private void UpdateSkillCDForItemUIList(float pFillAmount)
    {
        if (null == mSkillButtonCellList || mSkillButtonCellList.Count <= 0)
            return;
        SkillCellPrefab tSkillButtonCell = null;
        for (int tCounter = 0; tCounter < mSkillButtonCellList.Count; tCounter++)
        {
            tSkillButtonCell = mSkillButtonCellList[tCounter];
            if (null == tSkillButtonCell)
                continue;
            tSkillButtonCell.SpriteMask_UISprite.fillAmount = pFillAmount;
        }
    }
}

