
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 装备纹章铭刻符模板 */
  public class PropsParam_3 : PropsParam
  {

	public enum EngraveType
	{
    	UNKNOWN = 0,
    	/** 普通类 */
    	ORDINARY = 1,
    	/** 强化类 */
    	STRENGTHEN = 2,
	}


    /** 类型 */
    public int type;
    
    /** CharacterPropertyTypeId */
    public int cpId;
    
    /** 消耗圣能区间 */
    public int omin;
    
    /**  */
    public int omax;
    
    /** 效果区间 */
    public float emin;
    
    /**  */
    public float emax;
    

  }
}
