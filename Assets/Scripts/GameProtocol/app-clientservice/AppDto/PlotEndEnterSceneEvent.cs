
//  G e n e r a t e d   f i l e .   D o   n o t   e d i t . 
// Generated By Gecko,if you need to edit it,extends it! 

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppDto
{  
  /**  */
  public class PlotEndEnterSceneEvent : PlotEndEvent
  {



    /** 场景编号 */
    public int sceneId;
    
    /** 场景x坐标 */
    public float x;
    
    /** 场景z坐标 */
    public float z;
    
    /** 场景y坐标 */
    public float y;
    

    private SceneMap _scene;
    public  SceneMap scene{
    	get
    	{
    		if (_scene !=null){
				return _scene;
			}else{
				_scene = DataCache.getDtoByCls<SceneMap>(sceneId);
				return _scene;
			}
    	}
    	set
    	{
    		_scene = value;
    	}
    }
  }
}
