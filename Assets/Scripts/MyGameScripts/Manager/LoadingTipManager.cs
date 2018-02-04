using UnityEngine;
using System.Collections.Generic;

public class LoadingTipManager{

    public class HintMsgConfig
    {
        public List<HintMsg> list = new List<HintMsg>();
    }
	public class HintMsg{
		public int level;
		public int endLevel;
		public string tips;
		public int isLoadingTip;
	}

	private static List<HintMsg> _tipList; 		 
	private static List<HintMsg> _loadingTipList; //只显示在loading界面

	public static void Setup(){
		AssetPipeline.ResourcePoolManager.Instance.LoadConfig ("LoadingTipsData", (asset) => {
			if (asset != null) {
				TextAsset textAsset = asset as TextAsset;
				if (textAsset != null) {
					var hintMsgConfig = JsHelper.ToObject<HintMsgConfig> (textAsset.text);
				    _tipList = hintMsgConfig.list;
					_loadingTipList = new List<HintMsg> ();
					for (int i = 0; i < _tipList.Count; ++i) {
						if (_tipList [i].isLoadingTip == 1)
							_loadingTipList.Add (_tipList [i]);
					}
				}
			}
		});
	}

	public static string GetLoadingTip(){
		if (_loadingTipList != null)
		{
			var tipMsg = _loadingTipList.Random();
			return tipMsg!=null?tipMsg.tips:"";
		}
		else
		{
			return "";
		}
	}

	public static string GetSystemChannelTip(int playerLv){
		if (_tipList != null)
		{
			List<HintMsg> tmpList = new List<HintMsg>();
			for(int i=0;i<_tipList.Count;++i){
				if(playerLv >= _tipList[i].level && playerLv <= _tipList[i].endLevel)
					tmpList.Add(_tipList[i]);
			}
			
			var tipMsg = tmpList.Random();
			return tipMsg!=null?tipMsg.tips:"";
		}
		else
		{
			return "";
		}
	}
}
