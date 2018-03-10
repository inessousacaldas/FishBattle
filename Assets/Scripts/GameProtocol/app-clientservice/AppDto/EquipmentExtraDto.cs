
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 装备额外信息 */
  public class EquipmentExtraDto : GeneralResponse
  {



    /** 装备战力 */
    public int power;
    
    /** 品质 */
    public int quality;
    
    /** 当前属性 */
    public EquipmentPropertyDto currentProperty;
    
    /** 洗炼属性 */
    public EquipmentPropertyDto resetProperty;
    
    /** 纹章 */
    public MedallionDto medallion;
    
    /** 套装 */
    public int groupId;
    
    /** 特效 */
    public int effectId;
    
    /** 今天已洗练次数 */
    public int curResetCount;
    

  }
}
