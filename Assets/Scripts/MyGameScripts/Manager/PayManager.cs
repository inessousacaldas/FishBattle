// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : PayManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 01/03/2016 
// Porpuse  : 
// **********************************************************************
//


using System.Collections.Generic;

public class PayManager
{
    private static readonly PayManager _instance = new PayManager();

    public static PayManager Instance
    {
        get { return _instance; }
    }

    private Dictionary<int, OrderItemJsonDto> _payItemDic;

    private Dictionary<string, CySdk.Goods> _payCyItemDic;


    // 购买成功返回
    private System.Action<bool> _onPayCallBack;

    //购买项数据加载成功返回
    public event System.Action OnPayItemsLoadSuccess;

    public void Setup()
    {
		// PC端比较特殊，需要刷新
	    if (GameSetting.IsOriginWinPlatform)
	    {
		    _payItemDic = null;
	    }

        if (_payItemDic == null)
        {

            ServiceProviderManager.RequestOrderItems(GameSetting.Channel, GameSetting.BundleId,
            delegate (OrderItemsJsonDto dto)
            {
                _payItemDic = new Dictionary<int, OrderItemJsonDto>();
                if (dto != null && dto.items != null)
                {
                    for (int i = 0; i < dto.items.Count; i++)
                    {
                        OrderItemJsonDto item = dto.items[i];
                        _payItemDic.Add(item.gameShopItemId, item);

                    }
                }

                if (GameSetting.Channel == AgencyPlatform.Channel_cyou)
                {
                    SPSdkManager.Instance.CYGoodsData()
                        .CYCallback(CySdk.ResultCode.GOODS_SUCCESS, onCYGoodsDataSuccess)
                        .CYCallback(CySdk.ResultCode.GOODS_FAILED, onCYGoodsDataFailed);
                    return;
                }

                if (GameSetting.Channel == AgencyPlatform.Channel_Appstore)
                {
                    AppStoreInAppManager.Setup();
                    List<string> ids = new List<string>();
                    foreach (OrderItemJsonDto itemJsonDto in GetPayItemDic().Values)
                    {
                        ids.Add(itemJsonDto.id);
                    }
                    AppStoreInAppManager.Instance.Init(ids.ToArray());
                }

                if (OnPayItemsLoadSuccess != null)
                {
                    OnPayItemsLoadSuccess();
                }
            });
        }
        else if (GameSetting.Channel == AgencyPlatform.Channel_cyou && _payCyItemDic == null)
        {
            SPSdkManager.Instance.CYGoodsData()
                      .CYCallback(CySdk.ResultCode.GOODS_SUCCESS, onCYGoodsDataSuccess)
                      .CYCallback(CySdk.ResultCode.GOODS_FAILED, onCYGoodsDataFailed);
            return;
        }
    }

    public bool IsPayItemsLoaded()
    {
        return _payItemDic != null;
    }

    public Dictionary<int, OrderItemJsonDto> GetPayItemDic()
    {
        return _payItemDic;
    }

    private OrderItemJsonDto GetOrderItem(int itemId)
    {
        OrderItemJsonDto item = null;
        _payItemDic.TryGetValue(itemId, out item);
        return item;
    }

    private CySdk.Goods GetCyOrderItem(string itemId)
    {
        CySdk.Goods item = null;
        _payCyItemDic.TryGetValue(itemId, out item);
        return item;
    }


    private void onCYGoodsDataFailed(string jsondata)
    {
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.GOODS_FAILED, onCYGoodsDataFailed);

        CySdk.Result result = JsHelper.ToObject<CySdk.Result>(jsondata);

        GameDebuger.LogError("onCYGoodsDataFailed  Message：" + result.message);
      
    }
    private void onCYGoodsDataSuccess(string jsondata)
    {
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.GOODS_SUCCESS, onCYGoodsDataSuccess);

        CySdk.Result result = JsHelper.ToObject<CySdk.Result>(jsondata);

        //TODO:把OrderItemJsonDto 改为兼容CySdk.Goods 的格式 在商场里面展示

        //List<CySdk.Goods> goods = new List<CySdk.Goods>();

        _payCyItemDic = new Dictionary<string, CySdk.Goods>();

        LITJson.JsonData json = LITJson.JsonMapper.ToObject(result.data.ToJson());
        if (json.IsArray)
        {
            for (int i = 0; i < json.Count; i++)
            {
                LITJson.JsonData jd = json[i];
                CySdk.Goods gd = new CySdk.Goods((string)jd["goods_name"], (int)jd["goods_number"], jd["goods_id"].ToString(), (string)jd["goods_register_id"], (int)jd["goods_price"]);
                gd.setGoodsIcon((string)jd["goods_describe"]);
                gd.setGoodsIcon((string)jd["goods_icon"]);
                gd.setType((int)jd["type"]);

                _payCyItemDic.Add(gd.getGoodsRegisterId(),gd);
                //OrderItemJsonDto item = goods[i];
                //_payItemDic.Add(item.gameShopItemId, item);
            }
        }



        //_payItemDic = new Dictionary<int, OrderItemJsonDto>();
        //if (goods != null )
        //{
        //    for (int i = 0; i < goods.Count; i++)
        //    {
        //        //TODO:把OrderItemJsonDto 改为兼容CySdk.Goods 的格式
        //        OrderItemJsonDto item = goods[i];
        //        _payItemDic.Add(item.gameShopItemId, item);
        //    }
        //}

        if (SupportInAppPurchase())
        {
            AppStoreInAppManager.Setup();
            List<string> ids = new List<string>();
            foreach (OrderItemJsonDto itemJsonDto in GetPayItemDic().Values)
            {
                ids.Add(itemJsonDto.id);
            }
            AppStoreInAppManager.Instance.Init(ids.ToArray());
        }

        if (OnPayItemsLoadSuccess != null)
        {
            OnPayItemsLoadSuccess();
        }
    }

    /// <summary>
    /// onPayCallBack 一般用于:
    ///  活动期间只能一次充值 
    ///  活动期间每天只能充值一次
    ///  的锁死/解锁操作
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="onPayCallBack">true 充值成功,false 充值失败</param>
    /// <param name="delay">因为某些sdk没有返回,所以正常正常情况下3秒,需要 onPayCallBack 请设置为30秒</param>
    public void CyCharge(int itemId, System.Action<bool> onPayCallBack = null, float delay = 3, bool needLock = false)
    {
        _onPayCallBack = onPayCallBack;

        GameDebuger.TODO(@"
        if (!ModelManager.RechargeReward.ChannelOpen)
        {
            TipManager.AddTip('充值失败，请联系客服人员');
            return;
        }
            ");

        if (GameSetting.Channel == AgencyPlatform.Channel_cyou)
        {
            OrderItemJsonDto orderItem = GetOrderItem(itemId);

            if (orderItem == null)
            {
                TipManager.AddTip("无此充值项");
                return;
            }
#if UNITY_EDITOR
            if (!GameSetting.IsOriginWinPlatform)
            {
                TipManager.AddTip("编辑器运行无法充值");
                return;
            }
#endif
            RequestLoadingTip.Show("请求充值，请稍候", true, true, 3f);

            CySdk.Goods good = GetCyOrderItem(orderItem.id);

            if (good == null)
            {
                TipManager.AddTip("渠道无此充值项");
                return;
            }

            RequestLoadingTip.Show("请求充值，请稍候", true, true, delay);

            //畅游渠道sid为畅游登录成功后回传的json字符串，格式为：
            //{ "validateInfo":"7be82a5a1d9720cf0fccd47638dfe4f8cfdbfb2b658d7b46a10377cb6f2f137eb36595f852e075fb732bfcf6780d6107ec8797e5bea6e2a88c48307029093dc4b1a811d5c3e5585b00e1764379bb6facb6858a58bd3cd67089d2f974c967d40c4343eb9f0dcfc1f05a92aac09a65c52bd53d7ea0d59ae6324fa4b2aa5f875fd5e3343e51c7852a1573372e31a28e0dcae5c1a596ee0dab402dce3e695a26c53517640e3376d898f6c313a11fc65012f59135c172a887915408301fb47fa24fb0f902be545bf8ed0e27ce7a1fdc3d18086a8634d60a1aa7893f48d0b94d5eaaf7308105c989f48c9c03bb2af3a06f577a410625f7841b9efc2621455ec3bf4d1e7e697024013a07be5f3d42261eef9070a5772207d1645359514f121c506db86930f1700feb028e0e","channel_id":"4001","opcode":"10001"}
            LITJson.JsonData jsonData = LITJson.JsonMapper.ToObject(ServerManager.Instance.sid);
           

            ServiceProviderManager.RequestOrderId(GameSetting.Channel, ModelManager.Player.GetPlayerId().ToString(),
    orderItem.id, orderItem.cent, GameSetting.LoginWay, SPSDK.channelId(), GameSetting.AppId,
    SPSDK.deviceId(), ModelManager.Player.GetPlayerLevel(), (string)jsonData["validateInfo"], 0, ModelManager.Player.GetPlayer().faction.name,
    delegate (OrderJsonDto dto)
    {
        ChargeCyByOrderJsonDto(good, 1, dto);
    }, needLock);

        }
    }

    public void Charge(int itemId)
    {
        GameDebuger.TODO(@"
        if (!ModelManager.RechargeReward.ChannelOpen)
        {
            TipManager.AddTip('充值失败，请联系客服人员');
            return;
        }
            ");

        OrderItemJsonDto orderItem = GetOrderItem(itemId);
        if (orderItem == null)
        {
            TipManager.AddTip("无此充值项");
            return;
        }

#if UNITY_EDITOR
        if (!GameSetting.IsOriginWinPlatform)
        {
            TipManager.AddTip("编辑器运行无法充值");
            return;
        }
#endif

        RequestLoadingTip.Show("请求充值，请稍候", true, true, 3f);

        ServiceProviderManager.RequestOrderId(GameSetting.Channel, ModelManager.Player.GetPlayerId().ToString(), orderItem.id, 0, GameSetting.LoginWay, GameSetting.Channel, GameSetting.BundleId,
                                            delegate (OrderJsonDto dto)
		{
			ChargeByOrderJsonDto(orderItem, 1, dto);
		});
    }

    public void ChargeCyByOrderJsonDto(CySdk.Goods itemDto, int quantity, OrderJsonDto orderDto)
    {
        if (orderDto != null && orderDto.code == 0)
        {
            if (GameSetting.Channel == AgencyPlatform.Channel_Appstore)
            {
                ChargeByIOSInAppPurchase(itemDto.getGoodsId(), quantity, orderDto.orderId);
            }
            else
            {
                if (GameSetting.IsOriginWinPlatform)
                {
                   // ProxyQRCodeModule.OpenQRCodePayView(itemDto, quantity, orderDto);
                }
                else
                {
                    string pushInfo = string.Format("appOrderId={0}&platform={1}", orderDto.orderId, GameSetting.PlatformTypeId);

                    byte[] bs = System.Text.Encoding.UTF8.GetBytes(pushInfo);
                    pushInfo = System.Convert.ToBase64String(bs);;
                    //玩家剩余元宝数
                    int money = ModelManager.Player.GetPlayerWealth(AppDto.AppVirtualItem.VirtualItemEnum.DIAMOND)>int.MaxValue?int.MaxValue:(int)ModelManager.Player.GetPlayerWealth(AppDto.AppVirtualItem.VirtualItemEnum.DIAMOND);
                    //vip等级
                    int vipLevel = 0;
                    
                    
                    //清空上一次唤起支付的操作
                    SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_SUCCESS, CYPaySuccessCallback);
                    SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_FAILED, CYPayFailCallback);
                    SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_CANCEL, CYPayCancelCallback);

                    SPSdkManager.Instance.CYPay(itemDto.getGoodsId(), pushInfo, money, vipLevel)
                        .CYCallback(CySdk.ResultCode.PAY_SUCCESS, CYPaySuccessCallback)
                        .CYCallback(CySdk.ResultCode.PAY_FAILED, CYPayFailCallback)
                        .CYCallback(CySdk.ResultCode.PAY_CANCEL, CYPayCancelCallback);
                }
            }
        }
        else
        {
            if (orderDto != null)
            {
                GameDebuger.Log(orderDto.msg);
                TipManager.AddTip(orderDto.msg);
            }
            RequestLoadingTip.Reset();
        }

    }

    private void CYPaySuccessCallback(string json)
    {
        TipManager.AddTip("支付完成，如充值成功请等待到账");
        RequestLoadingTip.Reset();

        //支付成功
        if (_onPayCallBack != null)
        {
            _onPayCallBack(true);
            _onPayCallBack = null;
        }

        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_SUCCESS, CYPaySuccessCallback);
    }

    private void CYPayFailCallback(string json)
    {
        //支付失败
        if (_onPayCallBack != null)
        {
            _onPayCallBack(false);
            _onPayCallBack = null;
        }
     SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_FAILED, CYPayFailCallback);
    }


    private void CYPayCancelCallback(string json)
    {
        //支付取消
        if (_onPayCallBack != null)
        {
            _onPayCallBack(false);
            _onPayCallBack = null;
        }
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.PAY_CANCEL, CYPayCancelCallback);
    }


    public void ChargeByOrderJsonDto(OrderItemJsonDto itemDto, int quantity, OrderJsonDto orderDto)
	{
		if (orderDto != null && orderDto.code == 0)
		{
            if (GameSetting.Channel == AgencyPlatform.Channel_Appstore)
			{
				ChargeByIOSInAppPurchase(itemDto.id, quantity, orderDto.orderId);
			}
			else
			{
				if (GameSetting.IsOriginWinPlatform)
				{
					ProxyQRCodeModule.OpenQRCodePayView(itemDto, quantity, orderDto);
				}
				else
				{
					SPSdkManager.Instance.DoPay(orderDto.orderId,
														  itemDto.id,
														  itemDto.gold + "元宝",
														  (itemDto.cent / 100).ToString(),
														  quantity.ToString(),
														  ServerManager.Instance.GetServerInfo()
															  .serverId.ToString(),
														  orderDto.extra != null ? orderDto.extra.tsiPayCburl : "",
														  orderDto.extra != null ? orderDto.extra.vivoAccessKey : "",
														  orderDto.extra != null ? orderDto.extra.vivoOrderNumber : "",
														  delegate (bool success)
														  {
															  if (success)
															  {
																  TipManager.AddTip("支付完成，如充值成功请等待到账");
															  }
															  RequestLoadingTip.Reset();
														  });

				}
			}
		}
		else
		{
			if (orderDto != null)
			{
				GameDebuger.Log(orderDto.msg);
				TipManager.AddTip(orderDto.msg);
			}
			RequestLoadingTip.Reset();
		}

	}


    #region IOS支付


    public static bool SupportInAppPurchase()
    {
      

        if (GameSetting.Platform == GameSetting.PlatformType.IOS && GameSetting.Channel == AgencyPlatform.Channel_cyou
            && GameSetting.SubChannel == AgencyPlatform.Channel_cyou)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void ChargeByIOSInAppPurchase(string productIdentifier, int quantity, string orderId)
    {
        if (GameSetting.Channel == AgencyPlatform.Channel_Appstore)
        {
            AppStoreInAppManager.Instance.onBaoyugamePurchaseSuccessed -= onBaoyugamePurchaseSuccessed;
            AppStoreInAppManager.Instance.onBaoyugamePurchaseSuccessed += onBaoyugamePurchaseSuccessed;

            AppStoreInAppManager.Instance.onBaoyugamePurchaseFailed -= onBaoyugamePurchaseFailed;
            AppStoreInAppManager.Instance.onBaoyugamePurchaseFailed += onBaoyugamePurchaseFailed;

            AppStoreInAppManager.Instance.onBaoyugamePurchaseCancel -= onBaoyugamePurchaseFailed;
            AppStoreInAppManager.Instance.onBaoyugamePurchaseCancel += onBaoyugamePurchaseFailed;

            AppStoreInAppManager.Instance.PurchaseProduct(productIdentifier, quantity, orderId);
        }
    }

	public void RestoreCompletedTransactions()
	{
        if (GameSetting.Channel == AgencyPlatform.Channel_Appstore)
		{
			AppStoreInAppManager.Instance.RestoreCompletedTransactions();
		}
	}


	private void onBaoyugamePurchaseSuccessed()
    {
        AppStoreInAppManager.Instance.onBaoyugamePurchaseSuccessed -= onBaoyugamePurchaseSuccessed;
        AppStoreInAppManager.Instance.onBaoyugamePurchaseFailed -= onBaoyugamePurchaseFailed;
        AppStoreInAppManager.Instance.onBaoyugamePurchaseCancel -= onBaoyugamePurchaseFailed;

        TipManager.AddTip("支付完成，如充值成功请等待到账");
        RequestLoadingTip.Reset();
    }

    private void onBaoyugamePurchaseFailed(string error)
    {
        AppStoreInAppManager.Instance.onBaoyugamePurchaseSuccessed -= onBaoyugamePurchaseSuccessed;
        AppStoreInAppManager.Instance.onBaoyugamePurchaseFailed -= onBaoyugamePurchaseFailed;
        AppStoreInAppManager.Instance.onBaoyugamePurchaseCancel -= onBaoyugamePurchaseFailed;

        GameDebuger.Log(error);
        TipManager.AddTip(error);
        RequestLoadingTip.Reset();
    }
#endregion
}