
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 结晶回路DTO */
  public class QuartzExtraDto : GeneralResponse
  {



    /**  */
    public int elementId;
    
    /** 主属性 */
    public List<CharacterPropertyDto> baseProperties;
    
    /** 副属性 */
    public List<CharacterPropertyDto> secondProperties;
    
    /** 魔能属性 */
    public List<QuartzPropertyDto> quartzProperties;
    
    /** 特效被动 */
    public int passiveSkill;
    
    /** 强化等级 */
    public int strengGrade;
    
    /** 突破等级 */
    public int breakGrade;
    

  }
}
