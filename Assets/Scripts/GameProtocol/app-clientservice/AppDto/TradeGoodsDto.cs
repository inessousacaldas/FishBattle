
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /** 用于通知其它服务器交易物品变动 */
  public class TradeGoodsDto : GeneralResponse
  {



    /** 物品编号 */
    public int itemId;
    
    /** 物品剩余库存，每天0点回复 */
    public int amount;
    
    /** 物品当前价格 */
    public float price;
    
    /** 原始价格（0点时刷新的） */
    public float originalPrice;
    

    private TradeGoods _item;
    public  TradeGoods item{
    	get
    	{
    		if (_item !=null){
				return _item;
			}else{
				_item = DataCache.getDtoByCls<TradeGoods>(itemId);
				return _item;
			}
    	}
    	set
    	{
    		_item = value;
    	}
    }
  }
}
