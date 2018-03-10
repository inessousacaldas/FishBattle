
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 装备信息 */
  public class EquipmentInfoDto : GeneralResponse
  {



    /** 当前方案编号(最后一次选择的方案编码) */
    public int activeId;
    
    /** 当前身上的装备 key:Equipment.PartType */
    public List<EquipmentDto> current;
    
    /** 玩家自定义方案集 */
    public List<EquipmentCaseDto> cases;
    
    /** 神器值 */
    public List<AtrifactDto> atrifacts;
    
    /** 今天已打造次数 */
    public int curSmithCount;
    
    /** 今天已洗练次数 */
    public int curResetCount;
    

  }
}
