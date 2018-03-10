
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 引导玩家进入各个功能或界面 */
  public class SmartGuide 
  {

	public enum SmartGuideType
	{
    	/** 1.普通商店 */
    	SmartGuideType_1 = 1,
    	/** 2.功能窗口 */
    	SmartGuideType_2 = 2,
    	/** 3.npc导航 */
    	SmartGuideType_3 = 3,
    	/** 4.飘字提示 */
    	SmartGuideType_4 = 4,
    	/** 5.打开日程 */
    	SmartGuideType_5 = 5,
    	/** 6.任务追踪 */
    	SmartGuideType_6 = 6,
    	/** 7.特殊商店 */
    	SmartGuideType_7 = 7,
    	/** 100.其他 */
    	SmartGuideType_100 = 100,
	}


    /** 引导编号 */
    public int id;
    
    /** 名称 */
    public string name;
    
    /** 引导类型 */
    public int type;
    
    /** 图标 */
    public string icon;
    
    /** 参数类型 */
    public string param;
    
    /** 选中的物品编号 */
    public int selectItemId;
    
    /** 战败引导图标 */
    public string loseGuideIcon;
    
    /** 描述 */
    public string memo;
    

  }
}
