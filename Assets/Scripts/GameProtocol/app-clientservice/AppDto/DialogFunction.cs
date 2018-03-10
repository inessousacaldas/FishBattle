
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /**  */
  public class DialogFunction 
  {

	public enum DialogTypeEnum
	{
    	/** 未知 */
    	UnKnow = 0,
    	/** 商店 */
    	DialogType_1 = 1,
    	/** 日常委托 */
    	DialogType_2 = 2,
    	/** 开战 */
    	DialogType_3 = 3,
    	/** 打开组队平台界面 */
    	DialogType_4 = 4,
    	/** 关闭对话 */
    	DialogType_5 = 5,
    	/** 观战 */
    	DialogType_6 = 6,
    	/** 领取安全巡查任务 */
    	DialogType_7 = 7,
    	/** 查看巡查剩余次数 */
    	DialogType_8 = 8,
    	/** 领取宝图任务 */
    	DialogType_9 = 9,
    	/** 进入 */
    	DialogType_10 = 10,
    	/** 四轮之塔重置剩余进度 */
    	DialogType_11 = 11,
    	/** 四轮之塔排行榜 */
    	DialogType_12 = 12,
    	/** 格兰竞技场 */
    	DialogType_13 = 13,
    	/** 回到洛连特 */
    	DialogType_14 = 14,
    	/** 四轮之塔前往下一层 */
    	DialogType_15 = 15,
    	/** 打开副本界面 */
    	DialogType_16 = 16,
    	/** 参加武术大会 */
    	DialogType_17 = 17,
    	/** 离开武术大会场景 */
    	DialogType_18 = 18,
    	/** 了解公会 */
    	DialogType_19 = 19,
    	/** 了解公会竞赛 */
    	DialogType_20 = 20,
    	/** 公会任务 */
    	DialogType_21 = 21,
    	DialogType_22 = 22,
    	DialogType_23 = 23,
	}


    /** 窗口功能编号 */
    public int id;
    
    /** 窗口功能名称 */
    public string name;
    
    /** 窗口功能参数 */
    public int param;
    
    /** 类型 */
    public int type;
    
    /** 功能开关id */
    public int functionOpenId;
    
    /**  */
    public string dialog;
    
    /** 对应具体导表id */
    public int logicId;
    

    private FunctionOpen _functionOpen;
    public  FunctionOpen functionOpen{
    	get
    	{
    		if (_functionOpen !=null){
				return _functionOpen;
			}else{
				_functionOpen = DataCache.getDtoByCls<FunctionOpen>(functionOpenId);
				return _functionOpen;
			}
    	}
    	set
    	{
    		_functionOpen = value;
    	}
    }
  }
}
