// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  LuaManager.cs
// Author   : wenlin
// Created  : 2014/5/13 
// Purpose  : 
// **********************************************************************

//#define ENABLE_JSB

using System;
using AppDto;
using UnityEngine;
#if ENABLE_JSB
using SharpKit.JavaScript;
#else
using LuaInterface;
#endif

public static class ExpressionManager
{
    public static void Setup()
    {
        Dispose();
#if ENABLE_JSB
        _jsFuncCaches = new JsObject();
#else
        _luaState = new LuaState();
#endif
    }

    public static void Dispose()
    {
#if ENABLE_JSB
        _jsFuncCaches = null;
#else
        if (_luaState != null)
        {
            _luaState.Close();
            _luaState = null;
        }
#endif
    }
    private static bool IsFuncExist(string funcName)
    {
#if ENABLE_JSB
        return _jsFuncCaches.hasOwnProperty(funcName);
#else
        if (_luaState == null)
            return false;

        return _luaState[funcName] != null;
#endif
    }


    public static double EvalExpression(string funcName, string formula, string argNames, params object[] args)
    {
        if (IsFuncExist(funcName))
        {
            return CallFunction(funcName, args);
        }
        //编译公式表达式,并缓存下来
#if !ENABLE_JSB
        formula = formula.Replace("@", ",");
        formula = formula.Replace("#", ":");
        formula = formula.ToLower();

        string funcScript = string.Format(@"function {0}({1}) return {2}; end",
            funcName,
            argNames.ToLower(),
            formula);

        try
        {
            CompileLuaFunc(funcName, funcScript);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return 0.0;
        }
#else
        //所有参数统一按照小写处理
        formula = formula.ToLower();

        string funcScript = string.Format("(function {0}({1}){{ return {2}; }})",
            funcName,
            argNames.ToLower(),
            formula);

        try
        {
            CompileJsFunc(funcName, funcScript);
        }
        catch (JsError e)
        {
            Debug.LogError("EvalExpression Error:" + e.message + "\n" + funcScript);
            return 0.0;
        }
#endif
        return CallFunction(funcName, args);
    }

    private static double CallFunction(string funcName, params object[] args)
    {
#if ENABLE_JSB
        if (_jsFuncCaches == null)
            return 0.0;

        if (_jsFuncCaches.hasOwnProperty(funcName))
        {
            JsFunction jsFunc = _jsFuncCaches[funcName].As<JsFunction>();
            object ret = jsFunc.apply(null, args);
            return (double) ret;
        }
        return 0.0;
#else
        if (_luaState == null)
        {
            return 0.0;
        }

        if (_luaState[funcName] != null)
        {
            // Get the function object
            LuaFunction f = _luaState.GetFunction(funcName);

            // Call it, takes a variable number of object parameters and attempts to interpet them appropriately
            object[] r = f.Call(args);

            return (double) r[0];
        }
        return 0.0;
#endif
    }

    #region JS API

#if ENABLE_JSB
    private static JsObject _jsFuncCaches;

    private static void CompileJsFunc(string funcName, string script)
    {
        if (_jsFuncCaches == null)
            return;

        if (!_jsFuncCaches.hasOwnProperty(funcName))
        {
            _jsFuncCaches[funcName] = JsContext.eval(script);
        }
    }
#endif

    #endregion

    #region Lua API

#if !ENABLE_JSB
    private static LuaState _luaState;

    private static void CompileLuaFunc(string funcName, string script)
    {
        // Get the function object
        if (_luaState[funcName] == null)
        {
            _luaState.DoString(script, funcName, null);
        }
    }
#endif

    #endregion

    #region 默认分类

    /// <summary>
    ///     宝图计算 -- Name:相同数据保持唯一 lua:计算数据	Dos the treasure map copper formula.
    /// </summary>
    /// <returns>The treasure map copper formula.</returns>
    /// <param name="funcName">Func name.</param>
    /// <param name="formula">Formula.</param>
    public static int DoTreasureMapCopperFormula(string funcName, string formula)
    {
        int gameServerGrade = ModelManager.Player.ServerGrade;
        int value = (int) EvalExpression(funcName, formula, "slv", gameServerGrade);
        return value;
    }

    /// <summary>
    ///     伙伴属性值
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="crewlv"></param>
    /// <returns></returns>
    public static int DoCrewAttValueFormula(string funcName, string formula, int crewlv, double stareffect= 0.0f)
    {
        int value = (int) EvalExpression(funcName, formula, "LV, stareffect", crewlv, stareffect);
        return value;
    }

    /// <summary>
    ///     连理枝领取登记奖励需要恩爱值
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static float DoNeedLoveValue(string funcName, string formula, int lv)
    {
        float value = (float) EvalExpression(funcName, formula, "lv", lv);
        return value;
    }

    /// <summary>
    /// 世界频道说话需要活力计算 Math.min(50,Math.max(10,70-level*1))
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static float DoWorldChatNeedVigour(string funcName, string formula, int level)
    {
        float value = (float)EvalExpression(funcName, formula, "level", level);
        return value;
    }

    /// <summary>
    /// 世界频道发言等级限制 Math.max(openGrade,Math.min((SLV-20),40))
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="openGrade"></param>
    /// <param name="serverGrade"></param>
    /// <returns></returns>
    public static float DoWorldChatLvLimit(string funcName, string formula,int openGrade,int serverGrade)
    {
        float value = (float)EvalExpression(funcName, formula, "openGrade,SLV", openGrade, serverGrade);
        return value;
    }

    #endregion

    #region 技能相关

    public static int DoSkillFormula(string funcName, string formula, VideoSoldier videoSoldier)
    {
        formula = formula.Replace("()", "");
        int value = (int) EvalExpression(funcName, formula, "trigger", videoSoldier);
        return value;
    }

    public static int DoScenarioSkillLevelFormula(string funcName, string formula, int lv)
    {
        int value = (int) EvalExpression(funcName, formula, "lv", lv);
        return value;
    }

//    public static int DoAssistSkillCopperFormula(AssistSkillDto dto, int lv = 0)
//    {
//        if (lv == 0)
//        {
//            lv = dto.level + 1;
//        }
//
//        AssistSkill skill = dto.assistSkill;
//        int index = 0;
//        int beginLv = 0;
//        int endLv = 0;
//        for (; index < skill.levelRangeStr.Count; index++)
//        {
//            string lvRange = skill.levelRangeStr[index];
//            string[] lvRanges = lvRange.Split('-');
//            beginLv = StringHelper.ToInt(lvRanges[0]);
//            endLv = StringHelper.ToInt(lvRanges[1]);
//            if (beginLv <= lv && lv <= endLv)
//            {
//                break;
//            }
//        }
//
//        int value = 0;
//
//        if (index < skill.copperFormula.Count)
//        {
//            string funcName = string.Format("AssistSkillCopper_{0}_{1}_{2}", skill.id, beginLv, endLv);
//            string formula = skill.copperFormula[index];
//
//            value = (int) Math.Ceiling(EvalExpression(funcName, formula, "lv", lv));
//        }
//        return value;
//    }
//
//    public static int DoAssistSkillContributeFormula(AssistSkillDto dto, int lv, out bool isLowCost)
//    {
//        isLowCost = false;
//        if (lv == 0)
//        {
//            lv = dto.level + 1;
//        }
//
//        //小于限制等级时， 不消耗帮贡
//        if (lv < DataCache.GetStaticConfigValue(AppStaticConfigs.ASSIST_SKILL_LEVEL_FLAG))
//        {
//            return 0;
//        }
//
//        AssistSkill skill = dto.assistSkill;
//
//        int value = 0;
//        string funcName = string.Format("AssistSkillContribute_{0}", skill.id);
//        string formula = skill.contributeFormula;
//        value = (int) EvalExpression(funcName, formula, "lv", lv);
//
//        if (lv <= ModelManager.Player.ServerGrade - 20 && !string.IsNullOrEmpty(skill.contributeDiscountFormula))
//        {
//            isLowCost = true;
//            return (int) Math.Ceiling(float.Parse(skill.contributeDiscountFormula)*value);
//        }
//        return value;
//    }
//
//    public static int DoVigourConsumeFormula(AssistSkillDto dto, int level)
//    {
//        AssistSkill skill = dto.assistSkill;
//
//        int value = 0;
//        string funcName = string.Format("VigourConsume_{0}", dto.id);
//        string formula = skill.vigourConsumeFormula[0];
//
//        value = (int) EvalExpression(funcName, formula, "productLevel, skillLevel", level, level);
//
//        if (skill.vigourConsumeDiscount > 0 && level <= ModelManager.Player.ServerGrade - 20)
//        {
//            value = (int) Math.Ceiling(value*skill.vigourConsumeDiscount);
//        }
//
//        return value;
//    }

    public static int DoVigourConsumeFormula(string funcName, string formula, int lv)
    {
        int value = (int) EvalExpression(funcName, formula, "productLevel, skillLevel", lv, lv);
        return value;
    }

    #endregion

    #region 道具相关

    public static int DoStallGoodsBasePriceFormula(string funcName, int priceFactor, string formula)
    {
        int gameServerGrade = ModelManager.Player.ServerGrade;
        int value =
            Mathf.FloorToInt(
                (float) EvalExpression(funcName, formula, "gameservergrade, pricefactor", gameServerGrade, priceFactor));
        return value;
    }

    public static int DoPropsParam21Formula(string funcName, string formula, int rarity)
    {
        if (formula.Contains("LV"))
            return 0;

        int playerLv = ModelManager.Player.GetPlayerLevel();
        int value = (int) EvalExpression(funcName, formula, "rarity,targetLevel", rarity, playerLv);
        return value;
    }

    #endregion

    #region 装备相关

    /// <summary>
    ///     装备伤害计算 -- Dos the equipment hurt formula.
    ///     funcName：不同装备（不同ID：等级需要名称不一致）
    ///     formula：计算公式
    ///     ilv：等级
    ///     varyRate：率
    /// </summary>
    /// <returns>The equipment hurt formula.</returns>
    /// <param name="funcName">Func name.</param>
    /// <param name="formula">Formula.</param>
    /// <param name="ilv">Ilv.</param>
    /// <param name="varyRate">Vary rate.</param>
    public static int DoEquipmentHurtFormula(string funcName, string formula, int ilv, float varyRate)
    {
        int value = (int) EvalExpression(funcName, formula, "ilv, varyRate", ilv, varyRate);
        return value;
    }


    /// <summary>
    ///     计算装备属性品质
    ///     属性品质 = （属性评分-基值）/幅度
    ///     中的
    ///     基值 or 幅度
    /// </summary>
    /// <returns></returns>
    public static float DoEquipmentPropertyQuality(string funcName, string formula, int ilv)
    {
        float value = (float) EvalExpression(funcName, formula, "ilv", ilv);
        return value;
    }

    #endregion

    #region 商品相关

//    /// <summary>
//    ///     商品价格
//    /// </summary>
//    /// <param name="item"></param>
//    /// <param name="buyNum">已经购买数量</param>
//    /// <returns></returns>
//    public static long DoShopItemVirtualCount(ShopItem item, int buyNum)
//    {
//        string funcName = string.Format("DoShopItemVirtualCount_{0}", item.id);
//        string formula = item.virtualCount;
//        long value = (long) EvalExpression(funcName, formula, "total", buyNum);
//        return value;
//    }
//
//    /// <summary>
//    ///     商品限购数量
//    /// </summary>
//    /// <param name="item"></param>
//    /// <returns></returns>
//    public static int DoShopItemRestrictCount(ShopItem item)
//    {
//        int playerLv = ModelManager.Player.GetPlayerLevel();
//        string funcName = string.Format("DoShopItemRestrictCount_{0}", item.id);
//        string formula = item.restrictCount;
//        int value = (int) EvalExpression(funcName, formula, "lv", playerLv);
//        return value;
//    }
	#endregion

    #region 拍卖相关
    /// <summary>
    /// 拍卖上架价格
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <returns></returns>
    public static float DoAuctionSellCostFormulas(string funcName, string formula)
    {   
        int gameServerGrade = ModelManager.Player.ServerGrade;
        float value = (float)EvalExpression(funcName, formula, "SLV", gameServerGrade);
        return value;
 
    }

	#endregion
	
	#region 天宫招募价格
	/// <summary>
	/// 天宫招募价格  Dos the camp war recruit formulas.
	/// </summary>
	/// <returns>The camp war recruit formulas.</returns>
	/// <param name="funcName">Func name.</param>
	/// <param name="formula">Formula.</param>
	public static int DoCampWarRecruitFormulas(string funcName, string formula, int iCount)
	{   
		int value = (int)EvalExpression(funcName, formula, "playerCount", iCount);

		return value;
	}
		
	#endregion

	#region 决斗手续费用
	/// <summary>
	/// 决斗手续费用  Dos the duel gauntlet free.
	/// </summary>
	/// <returns>The duel gauntlet free.</returns>
	/// <param name="funcName">Func name.</param>
	/// <param name="formula">Formula.</param>
	/// <param name="iCount">I count.</param>
	public static int DoDuelGauntletFree(string funcName, string formula, int iCount) {
		int playerLv = ModelManager.Player.GetPlayerLevel();
		int value = (int)EvalExpression(funcName, formula, "lv, N", playerLv, iCount);

		return value;
	}
    #endregion
	
    #region 法宝公式
    /// <summary>
    /// 法宝评分 50+quality*60+Math.pow(grade+4，2)
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <returns></returns>
//    public static float DoMagicScoreFormulas(string funcName, string formula,MagicEquipmentExtraDto extra)
//    {     
//        float value = (float)EvalExpression(funcName, formula, "quality,grade", extra.quality, extra.grade);
//        return value;
//
//    }

    /// <summary>
    /// 法宝分解根据品质获得的碎片
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="extra"></param>
    /// <returns></returns>
    public static float DoMagicResolveChipFormulas(string funcName, string formula, int Quality)
    {
        float value = (float)EvalExpression(funcName, formula, "quality", Quality);
        return value;
    }

    /// <summary>
    /// 法宝描述效果公式
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="magicLevel"></param>
    /// <returns></returns>
    public static float DoMagicEquipmentMainSkillDescFormulas(string funcName, string formula, int magicLevel)
    {
        float value = (float)EvalExpression(funcName, formula, "skillLevel", magicLevel);
        return value;
    }

	#endregion

    #region 大唐无双
    public static int TangBuffComputer(string funcName,string formula)
    {
        int playerLv = ModelManager.Player.GetPlayerLevel();
        int value = (int)EvalExpression(funcName, formula, "Lv", playerLv);
        return value;
    }
    #endregion

    #region 选秀大赛
    public static int TalentConputer(string funcName,string formula)
    {
        int playerLv = ModelManager.Player.GetPlayerLevel();
        int value = (int)EvalExpression(funcName, formula, "Lv", playerLv);
        return value;
    }
    #endregion

    #region 背包解锁消耗计算
    public static int UnlockBagSlots( int cap )
    {
        var funcName = "BACKPACK_EXPAND_FORMULA";
        var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.BACKPACK_EXPAND_FORMULA, "");
        var size = DataCache.GetStaticConfigValue(AppStaticConfigs.PACK_EXPAND_SIZE, 0);
        var init = DataCache.GetStaticConfigValue(AppStaticConfigs.BACKPACK_INIT_CAPACITY, 0);

        return (int)EvalExpression(funcName, formula, "CAP, INIT, SIZE", cap, init, size);

    }
    /// <summary>
    /// 获取已仓库解锁次数~
    /// </summary>
    /// <param name="cap"></param>
    /// <returns></returns>
    public static int UnlockWarehouseSlots(int cap)
    {
        var funcName = "WAREHOUSE_EXPAND_TIMES_FORMULA";
        var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.WAREHOUSE_EXPAND_TIMES_FORMULA, "");
        var size = DataCache.GetStaticConfigValue(AppStaticConfigs.PACK_EXPAND_SIZE, 0);
        var init = DataCache.GetStaticConfigValue(AppStaticConfigs.WAREHOUSE_INIT_CAPACITY, 0);

        return (int)EvalExpression(funcName, formula, "CAP, INIT, SIZE", cap, init, size);
    }
    #endregion

    #region 技能相关
    /// <summary>
    /// 计算技能描述属性值
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="formula"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static int SkillPropertyDes(string formula,float lv,int idx)
    {
        int num = (int)EvalExpression("SkillPropertyDes"+idx, formula, "lv", lv);
        return num;
    }

    //计算升级技能铜币
    public static int UpgradeSkillCostGold(string funcName, string formula, int curLevel)
    {
        int num = (int)EvalExpression(funcName, formula, "level", curLevel);
        return num;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="potential">潜能配置</param>
    /// <param name="prop">需要计算的属性</param>
    /// <param name="formula">公式</param>
    /// <param name="curLevel">需要计算的等级对应的属性</param>
    /// <returns></returns>
    public static int UpgradeSkillPotentialEffect(Potential potential,int prop, string formula,int curLevel)
    {
        int num = (int)EvalExpression("UpgradeSkillPotentialEffect_" + potential.id + "_" + prop, formula, "lv", curLevel);
        return num;
    }

    /// <summary>
    /// 技能-天赋，升级天赋需要根据等级来开放
    /// </summary>
    /// <param name="potential"></param>
    /// <param name="prop"></param>
    /// <param name="formula"></param>
    /// <param name="curLevel"></param>
    /// <returns></returns>
    public static int UpgradeSkillTalentLevelOpen(string formula,int curLevel)
    {
        int num = (int)EvalExpression("UpgradeSkillTalentLevelOpen", formula, "lv", curLevel);
        return num;
    }

    /// <summary>
    /// 技能-战技，升级战技所需要的人物等级
    /// </summary>
    /// <param name="formula"></param>
    /// <param name="curLevel"></param>
    /// <returns></returns>
    public static int UpgradeSkillCraftsRoleLevel(string formula,int curLevel,int limit)
    {
        int num = (int)EvalExpression("UpgradeSkillCraftsRoleLevel", formula, "lv,limit", curLevel,limit);
        return num;
    }

    /// <summary>
    /// 伙伴技能—伙伴技巧解锁格子数量
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static int UnLockCrewSkillPassive(int quality)
    {
        var funcName = "CREW_PASSIVE_UNLOCK_FORMULA";
        var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.CREW_PASSIVE_UNLOCK_FORMULA, "");
        return (int)EvalExpression(funcName, formula, "QUALITY", quality);
    }

    /// <summary>
    /// 装别信息预览
    /// </summary>
    /// <param name="equipmentTypeId">装备类型Id</param>
    /// <param name="attrId">装备属性Id</param>
    /// <param name="formula">公式</param>
    /// <param name="lv">装备等级</param>
    /// <param name="r">波动系数</param>
    /// <returns></returns>
    public static int PreviewEquipmentSmithAttr(int equipmentTypeId,int attrId,string formula,int lv,float r)
    {
        var funcName = string.Format("PreviewEquipmentSmithAttr_{0}_{1}",equipmentTypeId,attrId);
        return (int)EvalExpression(funcName, formula, "LV,R", lv, r);
    }

    /// <summary>
    /// 获取装备附加属性的范围
    /// </summary>
    /// <param name="attrId"></param>
    /// <param name="formula"></param>
    /// <param name="lv"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static float PreviewEquipmentExtraAttr(int attrId, string formula, int lv, float r)
    {
        var funcName = string.Format("PreviewEquipmentSmithExtraAttr_{0}", attrId);
        return (float)EvalExpression(funcName, formula, "LV,R", lv, r);
    }
    #endregion

    /// <summary>
    /// 摆摊物品基础价格
    /// </summary>
    /// <returns></returns>

    #region 答题
    public static int QuestionAddExp(string funcName, string formula)
    {
        int lv = ModelManager.Player.GetPlayerLevel();
        int value = (int)EvalExpression(funcName, formula, "grade", lv);
        return value;
    }
    #endregion

    #region 摆摊
    public static int StallGoodsBasePrice(string funcName, string formula)
    {
        int serverLv = ModelManager.Player.ServerGrade;
        int value = (int) EvalExpression(funcName, formula, "gameServerGrade", serverLv);
        return value;
    }
    #endregion

    #region 料理
    public static double AssistSkillProductProps(string funcName, string formula, string quality)
    {
        double value = EvalExpression(funcName, formula, "美味度", quality);
        return value;
    }
    #endregion

    #region 生活委托
    public static double DelegateMissionProps(string funcName, string formula)
    {
        double value = EvalExpression(funcName, formula, "");
        return value;
    }
    #endregion
}