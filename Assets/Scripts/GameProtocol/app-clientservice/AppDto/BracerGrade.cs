
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 游击士等级相关配置
<p>
TODO: 挪到游击士系统目录下 */
  public class BracerGrade 
  {



    /** 等级值(Rank) */
    public int id;
    
    /** 等级名称 */
    public string name;
    
    /** 图标 */
    public string icon;
    
    /** 到达该级所需经验值 */
    public long exp;
    
    /** 是否为新手 */
    public bool newbie;
    
    /** 属性加成id */
    public List<int> attrId;
    
    /** 属性加成值 */
    public List<float> attrAdd;
    
    /** 开放导力器孔数量 */
    public int slotsCount;
    
    /** 打造等级 */
    public int quartzSmithGrade;
    
    /** 天赋点 */
    public int talentPoint;
    

  }
}
