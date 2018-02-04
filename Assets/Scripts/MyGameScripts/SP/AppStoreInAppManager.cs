using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Collections;

public class AppStoreInAppManager : MonoBehaviour
{
    static AppStoreInAppManager _instance = null;
    public static AppStoreInAppManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private bool _hasInit = false;

	private bool _hasWaitRestoredPurchases = false;

    //delegates

    public delegate void OnProductListReceived();
    public OnProductListReceived onProductListReceived;

    public delegate void OnProductListRequestFailed();
    public OnProductListRequestFailed onProductListRequestFailed;

    public delegate void OnBaoyugamePurchaseSuccessed();
    public OnBaoyugamePurchaseSuccessed onBaoyugamePurchaseSuccessed;

    public delegate void OnBaoyugamePurchaseFailed(string error);
    public OnBaoyugamePurchaseFailed onBaoyugamePurchaseFailed;

    public delegate void OnBaoyugamePurchaseCancel(string error);
    public OnBaoyugamePurchaseCancel onBaoyugamePurchaseCancel;
    //

    //properies
    public bool CanMakePayments
    {
        get
        {
            return IOSInAppPurchaseManager.JsbInstance.IsInAppPurchasesEnabled;
        }
    }

    public bool ProductListHasReceived
    {
        get
        {
            return IOSInAppPurchaseManager.JsbInstance.Products != null && IOSInAppPurchaseManager.JsbInstance.Products.Count > 0;
        }
    }

    //create
    public static void Setup()
    {
        GameDebuger.Log("AppStoreInAppManager Setup...");
        if (GameObject.Find("AppStoreInAppManager") == null)
        {
            GameObject obj = new GameObject("AppStoreInAppManager");
            obj.AddComponent<AppStoreInAppManager>();
            DontDestroyOnLoad(obj);
        }
    }

    void Awake()
    {
        GameDebuger.Log("AppStoreInAppManager Awake...");
        _instance = this;
    }

    void OnDestroy()
    {
        GameDebuger.Log("AppStoreInAppManager OnDestroy...");

        Destroy();

        _instance = null;
    }

    public void Init(string[] productIdentifiers)
    {
        if (!_hasInit)
        {
            _hasInit = true;

            for (int i = 0; i < productIdentifiers.Length; i++)
            {
                IOSInAppPurchaseManager.JsbInstance.AddProductId(productIdentifiers[i]);
            }

            IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
            IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;

	        _hasWaitRestoredPurchases = false;
            IOSInAppPurchaseManager.JsbInstance.LoadStore();
        }
    }

    public void Destroy()
    {
        if (_hasInit)
        {
            IOSInAppPurchaseManager.OnStoreKitInitComplete -= OnStoreKitInitComplete;
            IOSInAppPurchaseManager.OnTransactionComplete -= OnTransactionComplete;
        }
    }

    void OnEnable()
    {
        GameDebuger.Log("AppStoreInAppManager OnEnable...");
        // Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
    }

    void OnDisable()
    {
        GameDebuger.Log("AppStoreInAppManager OnDisable...");
        // Remove all the event handlers
    }


    /// <summary>
    /// SDK初始化完毕
    /// </summary>
    /// <param name="result"></param>
    private void OnStoreKitInitComplete(ISN_Result result)
    {
        if (result.IsSucceeded)
        {
            GameDebuger.Log("OnStoreKitInitComplete Success");

            var products = IOSInAppPurchaseManager.JsbInstance.Products;
            for (int i = 0; i < products.Count; i++)
            {
                GameDebuger.Log(products[i].ToString());
            }
//	        _hasWaitRestoredPurchases = false;
        }
        else
        {
            GameDebuger.LogError("Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
        }
    }

    private void OnTransactionComplete(IOSStoreKitResult result)
    {
        GameDebuger.Log("OnTransactionComplete: " + result.ProductIdentifier);
        GameDebuger.Log("OnTransactionComplete: state: " + result.State);
        GameDebuger.Log("OnTransactionComplete: transactionIdentifier " + result.TransactionIdentifier);

        switch (result.State)
        {
            case InAppPurchaseState.Purchased:
                productPurchaseAwaitingConfirmationEvent(result);
                break;
            case InAppPurchaseState.Restored:
                //Our product been succsesly purchased or restored
                //So we need to provide content to our user depends on productIdentifier

                // 对于不合法的重复购买，会被调用到
                productPurchaseAwaitingConfirmationEvent(result);
                //FinishTransaction(result.TransactionIdentifier);
                break;
            case InAppPurchaseState.Deferred:
                //iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
                //You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
                //reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.

                // 不做处理，一直等待刷新状态
                break;
            case InAppPurchaseState.Failed:
                //Our purchase flow is failed.
                //We can unlock intrefase and repor user that the purchase is failed. 
                GameDebuger.Log("Transaction failed with error, code: " + result.Error.Code);
                GameDebuger.Log("Transaction failed with error, description: " + result.Error.Description);

                // 失败也要终结订单
                FinishTransaction(result.TransactionIdentifier);
                RaiseBaoyugamePurchaseFailed(result.Error.Description);
                break;
        }
    }


    private void FinishTransaction(string transactionIdentifier)
    {
        Debug.Log("FinishTransaction, transactionIdentifier: " + transactionIdentifier);
        IOSInAppPurchaseManager.JsbInstance.FinishTransaction(transactionIdentifier);
    }

    public void RestoreCompletedTransactions()
    {
		if (_hasWaitRestoredPurchases)
		{
			_hasWaitRestoredPurchases = false;

			IOSInAppPurchaseManager.JsbInstance.RestorePurchases();
		}
    }

    private Dictionary<string, string> _orderDic = new Dictionary<string, string>();

    public void PurchaseProduct(string productIdentifier, int quantity, string orderId)
    {
        if (_orderDic.ContainsKey(productIdentifier))
        {
            _orderDic[productIdentifier] = orderId;
        }
        else
        {
            _orderDic.Add(productIdentifier, orderId);
        }
        GameDebuger.Log("PurchaseProduct .. productIdentifier: " + productIdentifier + ", quantity: " + quantity + ", orderId: " + orderId);
        //		StoreKitBinding.purchaseProduct( productIdentifier, quantity );
        IOSInAppPurchaseManager.JsbInstance.BuyProduct(productIdentifier);
    }


    private void productPurchaseAwaitingConfirmationEvent(IOSStoreKitResult result)
    {
        if (_orderDic.ContainsKey(result.ProductIdentifier))
        {
            string orderId = _orderDic[result.ProductIdentifier];
            _orderDic.Remove(result.ProductIdentifier);
            StartCoroutine(SendReceiptToServer(result, orderId));
        }
        else
        {
			// 一般跑进这里是购买恢复
			// 恢复购买需要角色Id，如果没有的话不允许恢复，做个标记，登陆之后才允许调用
	        if (ModelManager.Player == null || ModelManager.Player.GetPlayerId() == 0)
	        {
		        _hasWaitRestoredPurchases = true;
				return;
	        }

            ServiceProviderManager.RequestOrderId(GameSetting.Channel, ModelManager.Player.GetPlayerId().ToString(), result.ProductIdentifier, 0, GameSetting.LoginWay, GameSetting.Channel, GameSetting.BundleId,
                                                  delegate (OrderJsonDto dto)
                                                  {
                                                      if (dto.code == 0)
                                                      {
                                                          StartCoroutine(SendReceiptToServer(result, dto.orderId));
                                                      }
                                                      else
                                                      {
                                                          GameDebuger.Log(dto.msg);
                                                          TipManager.AddTip(dto.msg);
                                                          RequestLoadingTip.Reset();
                                                      }
                                                  });
        }
    }


    private IEnumerator SendReceiptToServer(IOSStoreKitResult result, string orderId)
    {
        IOSProductTemplate tpl = IOSInAppPurchaseManager.JsbInstance.GetProductById(result.ProductIdentifier);

        string receipt = result.Receipt;

		string local = tpl==null?"":tpl.CountryCode;
		string price = tpl==null?"":(tpl.CurrencyCode + "_" + tpl.Price);
		
        WWWForm form = new WWWForm();
        form.AddField("receipt", receipt);
        form.AddField("orderId", orderId);
        form.AddField("deviceId", BaoyugameSdk.getUUID());
        if (tpl != null)
        {
            form.AddField("local", local);
            form.AddField("price", price);
        }

        string payUrl = GameSetting.PAY_SERVER + "/gpayc/tcappstore/validate.json";
        GameDebuger.Log(string.Format("payUrl={0} receipt={1} orderId={2}", payUrl, receipt, orderId));
        using (WWW www = new WWW(payUrl, form))
        {
            yield return www;

            GameDebuger.Log("purchased product id = " + result.ProductIdentifier);

            if (string.IsNullOrEmpty(www.error))
            {
                if (!string.IsNullOrEmpty(www.text))
                {
                    string json = www.text;
                    GameDebuger.Log(json);
                    ValidateJsonDto dto = (ValidateJsonDto)JsHelper.ToObject<ValidateJsonDto>(json);
                    if (dto.code == 0)
                    {
                        FinishTransaction(result.TransactionIdentifier);
                        RaiseBaoyugamePurchaseSuccessed();
                        RequestLoadingTip.Reset();
                    }
                    // 该订单之前已经付费，终止订单不做处理
                    else if (dto.code == 8)
                    {
                        FinishTransaction(result.TransactionIdentifier);
                        RequestLoadingTip.Reset();
                    }
                    else
                    {
                        RaiseBaoyugamePurchaseFailed("statusCode=" + dto.msg);
                    }
                }
                else
                {
                    RaiseBaoyugamePurchaseFailed(null);
                }
            }
            else
            {
                RaiseBaoyugamePurchaseFailed(www.error);
            }
        }
    }



    //helper
    void RaiseBaoyugamePurchaseSuccessed()
    {
        if (onBaoyugamePurchaseSuccessed != null)
        {
            onBaoyugamePurchaseSuccessed();
        }
    }

    void RaiseBaoyugamePurchaseFailed(string error)
    {
        RequestLoadingTip.Reset();
        if (onBaoyugamePurchaseFailed != null)
        {
            onBaoyugamePurchaseFailed(error);
        }
    }

    void RaiseBaoyugamePurchaseCancel(string error)
    {
        RequestLoadingTip.Reset();
        if (onBaoyugamePurchaseCancel != null)
        {
            onBaoyugamePurchaseCancel(error);
        }
    }

}