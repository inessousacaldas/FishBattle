
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 阵型 */
  public class Formation 
  {

	public enum FormationType
	{
    	/** 普通阵 */
    	Regular = 1,
    	/** 雁行阵 */
    	WildGoodFly = 2,
    	/** 锋矢阵 */
    	FrontArrow = 3,
    	/** 长蛇阵 */
    	Hydra = 4,
    	/** 鱼鳞阵 */
    	Scale = 5,
    	/** 方圆阵 */
    	Cirumference = 6,
    	/** 鹤翼阵 */
    	CraneWing = 7,
    	/** 云龙阵 */
    	ClopudDragon = 8,
    	/** 偃月阵 */
    	CeaseMonth = 9,
	}
	public enum PosistionRow
	{
    	/** 前排 下标0-3 */
    	FrontRow = 0,
    	/** 中排 下标4-7 */
    	RockRow = 1,
    	/** 后排 下标8-11 */
    	BackRow = 2,
	}
	public enum PosistionCol
	{
    	/** 第A行 下标0-3 */
    	One = 0,
    	/** 第B行 下标4-7 */
    	Two = 1,
    	/** 第C行 下标8-11 */
    	Three = 2,
    	/** 第D行 下标12-15 */
    	Four = 3,
	}


    /** 阵型编号 */
    public int id;
    
    /** 名称 */
    public string name;
    
    /** 描述 */
    public string description;
    
    /** 阵法等级上限 */
    public int gradeLimit;
    
    /** 克阵 */
    public List<int> debuffTargetIds;
    
    /** 被克制 */
    public List<int> targetIds;
    
    /** 材料 */
    public List<int> materialIds;
    
    /** 描述内容最大值 */
    public string descMaxNum;
    

  }
}
