
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /**  */
  public class FriendInfoDto : GeneralResponse
  {



    /** 好友ID */
    public long friendId;
    
    /** 好友名称 */
    public string name;
    
    /** 好友等级 */
    public int grade;
    
    /** 角色编号 */
    public int charactorId;
    
    /** 职业 */
    public int factionId;
    
    /** 国家 */
    public int countryId;
    
    /** 好友度 */
    public int degree;
    
    /** 建立时间 */
    public long createTime;
    
    /** 是否在线 */
    public bool online;
    
    /** 离线时间 */
    public long offlineTime;
    

    private GeneralCharactor _charactor;
    public  GeneralCharactor charactor{
    	get
    	{
    		if (_charactor !=null){
				return _charactor;
			}else{
				_charactor = DataCache.getDtoByCls<GeneralCharactor>(charactorId);
				return _charactor;
			}
    	}
    	set
    	{
    		_charactor = value;
    	}
    }
    private Faction _faction;
    public  Faction faction{
    	get
    	{
    		if (_faction !=null){
				return _faction;
			}else{
				_faction = DataCache.getDtoByCls<Faction>(factionId);
				return _faction;
			}
    	}
    	set
    	{
    		_faction = value;
    	}
    }
  }
}
