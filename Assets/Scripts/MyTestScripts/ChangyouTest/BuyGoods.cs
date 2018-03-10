using UnityEngine;
using System.Collections;
using CySdk;
using LITJson;

public class BuyGoods : MonoBehaviour {

	public Goods goods3  { get;set;  }

	public void doBuy(string userinfo)
	{
		goods3.setPushInfo (userinfo);
		if(goods3.getType() != 3){
			goods3.setPushInfo (userinfo);
			//SPSdkManager.Instance.DoPay(goods3,100,1,(jsonParam) => {
   //             JsonData jsonData = JsonMapper.ToObject(jsonParam);
   //             int state_code = (int)jsonData["state_code"];
   //             string message = (string)jsonData["message"];
   //             if (state_code == ResultCode.PAY_SUCCESS)
   //             {
   //               //  writeLog("支付成功回调\n");
                    
   //             }
   //             else
   //             {
   //               //  writeLog("支付失败或者取消回调\n");
   //             }
   //         }); //或者 PlatformGame.game().pay(goods3.getGoodsId(),userinfo,100,1);
		}else{
			//任意购
			GameHandler.Instance.curGoods = goods3;
			GameHandler.Instance.pricePanel.gameObject.SetActive (true);
			GameHandler.Instance.priceField.text = "";
		}
	}
}
