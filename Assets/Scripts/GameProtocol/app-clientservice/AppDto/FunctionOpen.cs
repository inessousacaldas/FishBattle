
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 功能开启 */
  public class FunctionOpen 
  {

	public enum FunctionOpenEnum
	{
    	NONE = 0,
    	/** 1 人物属性 */
    	FUN_1 = 1,
    	/** 2 背包 */
    	FUN_2 = 2,
    	/** 3 仓库 */
    	FUN_3 = 3,
    	/** 4 战斗系统：人物指令/自动战斗 */
    	FUN_4 = 4,
    	/** 5 游戏地图 */
    	FUN_5 = 5,
    	/** 6 组队系统 */
    	FUN_6 = 6,
    	/** 7 邮箱功能 */
    	FUN_7 = 7,
    	/** 8 好友系统 */
    	FUN_8 = 8,
    	/** 9 聊天系统 */
    	FUN_9 = 9,
    	/** 10 伙伴系统 */
    	FUN_10 = 10,
    	/** 11 阵法 */
    	FUN_11 = 11,
    	/** 12 背包：扩充 */
    	FUN_12 = 12,
    	/** 13 便捷组队 */
    	FUN_13 = 13,
    	/** 14 角色创建 */
    	FUN_14 = 14,
    	/** 15 帮派师爷 */
    	FUN_15 = 15,
    	/** 16 伙伴进阶 */
    	FUN_16 = 16,
    	/** 17 交易中心：拍卖行 */
    	FUN_17 = 17,
    	/** 18 交易中心：商会/摆摊 */
    	FUN_18 = 18,
    	/** 19 主线 */
    	FUN_19 = 19,
    	/** 20 支线 */
    	FUN_20 = 20,
    	/** 21 日常委托 */
    	FUN_21 = 21,
    	/** 22 地区治安 */
    	FUN_22 = 22,
    	/** 23 噬身之蛇 */
    	FUN_23 = 23,
    	/** 24 远古魔兽 */
    	FUN_24 = 24,
    	/** 25 安全巡查 */
    	FUN_25 = 25,
    	/** 26 特别任务 */
    	FUN_26 = 26,
    	/** 27 游击士 */
    	FUN_27 = 27,
    	/** 28 月卡 */
    	FUN_28 = 28,
    	/** 29 首冲 */
    	FUN_29 = 29,
    	/** 30 指引 */
    	FUN_30 = 30,
    	/** 31 提升 */
    	FUN_31 = 31,
    	/** 32 排行榜 */
    	FUN_32 = 32,
    	/** 33 福利 */
    	FUN_33 = 33,
    	/** 34 公会系统主界面 */
    	FUN_34 = 34,
    	/** 35 阵营 */
    	FUN_35 = 35,
    	/** 36 收集 */
    	FUN_36 = 36,
    	/** 37 成就 */
    	FUN_37 = 37,
    	/** 38 日程 */
    	FUN_38 = 38,
    	/** 39 商城 */
    	FUN_39 = 39,
    	/** 40 技能系统 */
    	FUN_40 = 40,
    	/** 41 装备系统 */
    	FUN_41 = 41,
    	/** 42 生产系统主界面按钮 */
    	FUN_42 = 42,
    	/** 43 导力器系统 */
    	FUN_43 = 43,
    	/** 44 招募系统 */
    	FUN_44 = 44,
    	/** 格兰竞技场 */
    	FUN_45 = 45,
    	/** 宝图任务 */
    	FUN_46 = 46,
    	/** 每日问答 文字以及图片 */
    	FUN_47 = 47,
    	/** 四轮之塔 */
    	FUN_48 = 48,
    	/** 人物属性界面属性分页 */
    	FUN_49 = 49,
    	/** 人物属性界面信息分页 */
    	FUN_50 = 50,
    	/** 背包界面包裹分页 */
    	FUN_51 = 51,
    	/** 组队系统我的队伍分页 */
    	FUN_52 = 52,
    	/** 组队系统推荐队伍分页 */
    	FUN_53 = 53,
    	/** 交易系统摆摊功能 */
    	FUN_54 = 54,
    	/** 交易系统商会功能 */
    	FUN_55 = 55,
    	/** 任务系统 */
    	FUN_56 = 56,
    	/** 活动玩法功能 */
    	FUN_57 = 57,
    	/** 充值系统 */
    	FUN_58 = 58,
    	/** 指引功能 */
    	FUN_59 = 59,
    	/** 技能系统技能分页 */
    	FUN_60 = 60,
    	/** 技能系统潜能分页 */
    	FUN_61 = 61,
    	/** 技能系统天赋分页 */
    	FUN_62 = 62,
    	/** 技能系统专精分页 */
    	FUN_63 = 63,
    	/** 装备系统打造分页 */
    	FUN_64 = 64,
    	/** 装备系统洗炼分页 */
    	FUN_65 = 65,
    	/** 装备系统宝石分页 */
    	FUN_66 = 66,
    	/** 装备系统纹章分页 */
    	FUN_67 = 67,
    	/** 生产系统生活技能 */
    	FUN_68 = 68,
    	/** 生产系统委托任务 */
    	FUN_69 = 69,
    	/** 导力器系统属性 */
    	FUN_70 = 70,
    	/** 导力器系统强化 */
    	FUN_71 = 71,
    	/** 导力器系统制造 */
    	FUN_72 = 72,
    	/** 知识竞答 */
    	FUN_73 = 73,
    	/** 伙伴经验按钮 */
    	FUN_74 = 74,
    	/** 75 送花系统 */
    	FUN_75 = 75,
    	/** 76 武术大会 */
    	FUN_76 = 76,
    	/** 77 副本系统 */
    	FUN_77 = 77,
    	/** 公会福利 */
    	FUN_78 = 78,
    	/** 公会活动 */
    	FUN_79 = 79,
    	/** 公会建设 */
    	FUN_80 = 80,
    	/** 公会管理 */
    	FUN_81 = 81,
    	/** 时间之扉 */
    	FUN_82 = 82,
    	/** 精英·时间之扉 */
    	FUN_83 = 83,
    	/** 公会频道 */
    	FUN_84 = 84,
    	FUN_85 = 85,
    	FUN_86 = 86,
    	/** 公会强盗 */
    	FUN_87 = 87,
	}
	public enum MissionState
	{
    	/** 可接 */
    	Default = 0,
    	/** 进行中 */
    	Doing = 1,
    	/** 可提交 */
    	Complete = 2,
    	/** 已完成 */
    	Completed = 3,
	}


    /** 功能编号 */
    public int id;
    
    /** 功能 */
    public string name;
    
    /** 功能描述 */
    public string description;
    
    /** 是否关闭 */
    public bool close;
    
    /** 人物等级 */
    public int grade;
    
    /** 开启服务器等级条件 */
    public int serverGrade;
    
    /** 新手引导 */
    public bool guide;
    
    /** 任务id */
    public int missionId;
    
    /** 任务状态 */
    public int missionState;
    

  }
}
