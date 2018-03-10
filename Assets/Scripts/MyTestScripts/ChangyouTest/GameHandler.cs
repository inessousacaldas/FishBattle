using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using CySdk;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LITJson;

public class GameHandler : MonoBehaviour
{

    public Button scanTestButton;

    public static GameHandler Instance;

    private bool isProtrait = false;

    public RectTransform loginPanel;
    public Button loginBtn;
    public Button authentionBtn;
    public Button deviceInfoBtn;

    public RectTransform serverPanel;
    public Button createRoleBtn;
    public Button choooseServerBtn;
    public Button startBtn;
    public Button scanBtn;

    public RectTransform gamePanel;
    public Button getHostBtn;
    public Button goodsListBtn;
    public Button roleUpgradeBtn;
    public Button exitBtn;
    public Button logoutBtn;
    public Button switchBtn;
    public Button userCenterBtn;
    public Button serviceCenterBtn;
    public Button payHistoryBtn;
    public Button authViewBtn;
    public Button feedbackBtn;

    public RectTransform goodsPanel;
    public RectTransform goodsContent;
    public Button goodsCloseBtn;
    public Button replaceOrderBtn;

    public RectTransform logContent;
    public Text logText;
    public Button cleanLogBtn;
    public Text fpsText;

    public GameObject exitDialog;
    public Button quitBtn;
    public Button cancelBtn;

    public RectTransform pricePanel;
    public InputField priceField;
    public Button closePricePanel;

    public Text appInfo;
    public Goods curGoods;

    void Awake()
    {
        BuglyAgent.InitWithAppId("c180163ca5");
        writeLog(PlayerPrefs.GetString("initresult"));

    }

    void Start()
    {

        Instance = this;

        string[] e = { "正式", "测试", "预发布" };

        appInfo.text = SPSDK.appName() + "(sdk版本:" + SPSDK.sdkVersionName() +
        " 渠道版本:" + SPSDK.channelVersion() +
        " 渠道号:" + SPSDK.channelId() +
        " 环境:" + e[SPSDK.mode()] +
        ")";





        if (!SPSDK.isLandscape())
            isProtrait = true;

        if (isProtrait)
        {
            CanvasScaler scaler = this.gameObject.GetComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(480, 800);
            scaler.matchWidthOrHeight = 0;
        }

#if UNITY_IOS
			exitBtn.gameObject.SetActive (false);
#endif


        showLoginView();

        deviceInfoBtn.onClick.AddListener(delegate ()
        {

            writeLog("device id=" + SPSDK.deviceId() + "\n" +
                "IP=" + SPSDK.ip() + "\n" +
                "AppKey=" + SPSDK.appKey() + "\n"
            );

            // 模拟打印一个cmbi日志（id = 0001），研发参考该调用方式
            SPSDK.gameEvent("0001");
        });

        loginBtn.onClick.AddListener(delegate ()
        {

            // SPSDK.Login();
            //模拟打印一个cmbi日志，研发参考该调用方式
            // SPSDK.gameEvent("0002");
            SPSdkManager.Instance.CYLogin().CYCallback(CySdk.ResultCode.LOGIN_SUCCESS, (json) =>
            {
                //登录成功callback
                writeLog("登录成功callback\n");
            })
            .CYCallback(CySdk.ResultCode.LOGIN_FAILED, (json) =>
            {
                //登录失败callback
                writeLog("登录失败callback\n");
            })
            .CYCallback(CySdk.ResultCode.LOGIN_CANCEL, (json) =>
            {
                //登录取消callback
                writeLog("登录取消callback\n");
            });
            SPSDK.gameEvent("0002");
        });

        authentionBtn.onClick.AddListener(delegate ()
        {
            SPSDK.isAuthention();
            // 模拟打印一个cmbi日志，研发参考该调用方式
            SPSDK.gameEvent("0003");
        });
        scanTestButton.onClick.AddListener(delegate ()
        {
            SPSDK.scan();
        }

            );
        scanBtn.onClick.AddListener(delegate ()
        {
            SPSDK.scan();
            SPSDK.gameEvent("0004");
        });

        createRoleBtn.onClick.AddListener(delegate ()
        {
            int rid = UnityEngine.Random.Range(100000, 999999);

            SPSDK.roleCreate("" + rid, "roleName" + rid, 1, 1470899572);

            writeLog("已调用创角接口");
            // 模拟打印一个cmbi日志，研发参考该调用方式
            SPSDK.gameEvent("0005");
        });

        choooseServerBtn.onClick.AddListener(delegate ()
        {

            SPSDK.enterServer(110, "serverName");

            // 模拟打印一个cmbi日志，研发参考该调用方式
            SPSDK.gameEvent("0006");
            startBtn.gameObject.SetActive(true);
        });

        startBtn.onClick.AddListener(delegate ()
        {

            SPSDK.gameStarted("100000", "roleName", 1, "party", 100, 1, 1234567890);

            // 模拟打印一个cmbi日志，研发参考该调用方式
            SPSDK.gameEvent("0007");
            showGameView();
        });

        getHostBtn.onClick.AddListener(delegate ()
        {

            // SPSDK.getHost();
            SPSdkManager.Instance.CYGetHost().CYCallback(CySdk.ResultCode.HOST_SUCCESS, (json) =>
            {
                CySdk.Result result= JsHelper.ToObject<CySdk.Result>(json);
                //网关callback
                writeLog(string.Format("网关获取成功callback\n state_code:{0}\n message:{1}\n data:{2}",result.state_code,result.message,result.data));
            })
            .CYCallback(CySdk.ResultCode.HOST_FAILED,(json)=> {
                CySdk.Result result = JsHelper.ToObject<CySdk.Result>(json);
                //网关callback
                writeLog(string.Format("网关获取失败callback\n state_code:{0}\n message:{1}\n data:{2}", result.state_code, result.message, result.data));
            });

        });

        goodsListBtn.onClick.AddListener(delegate ()
        {

            // SPSDK.goodsData();
            SPSdkManager.Instance.CYGoodsData().CYCallback(CySdk.ResultCode.GOODS_SUCCESS, (json) =>
            {
                CySdk.Result result = JsHelper.ToObject<CySdk.Result>(json);
                //网关callback
                writeLog(string.Format("商品获取成功callback\n state_code:{0}\n message:{1}\n data:{2}", result.state_code, result.message, result.data));
            })
            .CYCallback(CySdk.ResultCode.GOODS_FAILED, (json) => {
                 CySdk.Result result = JsHelper.ToObject<CySdk.Result>(json);
                //网关callback
                writeLog(string.Format("商品获取失败callback\n state_code:{0}\n message:{1}\n data:{2}", result.state_code, result.message, result.data));
 });

        });

        exitBtn.onClick.AddListener(delegate ()
        {
           // SPSDK.exit();
            SPSdkManager.Instance.CYExit().CYCallback(CySdk.ResultCode.EXIT_GAME, doExit).CYCallback(CySdk.ResultCode.EXIT_GAME_DIALOG,doExit);
           

        });

        logoutBtn.onClick.AddListener(delegate ()
        {
            //SPSDK.logout();

            if (SPSDK.apiAvailable(CySdk.ApiCode.LOGOUT))
            {
                SPSdkManager.Instance.CYLogout().CYCallback(CySdk.ResultCode.LOGOUT, (json) =>
                {
                    showLoginView();
                });

            }

        });

        switchBtn.onClick.AddListener(delegate ()
        {

            //SPSDK.switchUser();
            if (SPSDK.apiAvailable(CySdk.ApiCode.SWITCH_USER))
            {
                SPSdkManager.Instance.CYLogout().CYCallback(CySdk.ResultCode.SWITCH_USER_SUCCESS, (json) =>
                {
                    showLoginView();
                })
                .CYCallback(CySdk.ResultCode.SWITCH_USER_FAILED,(json)=> {

                    CySdk.Result result = JsHelper.ToObject<CySdk.Result>(json);
                    writeLog(string.Format("切换账号失败callback\n state_code:{0}\n message:{1}\n data:{2}", result.state_code, result.message, result.data));

                });

            }

        });

        userCenterBtn.onClick.AddListener(delegate ()
        {
            if (SPSDK.apiAvailable(CySdk.ApiCode.USER_CENTER))
            {
                SPSDK.userCenter();
            }
        });

        serviceCenterBtn.onClick.AddListener(delegate ()
        {
            if (SPSDK.apiAvailable(CySdk.ApiCode.SERVICE_CENTER))
            {
                SPSDK.serviceCenter();
            }

        });

        payHistoryBtn.onClick.AddListener(delegate ()
        {

            SPSDK.payHistoryView();

        });

        authViewBtn.onClick.AddListener(delegate ()
        {
            SPSDK.showBindingView();
        });

        feedbackBtn.onClick.AddListener(delegate ()
        {
            JsonData jd = new JsonData();
            jd["appkey"] = SPSDK.appKey();
            jd["appsecret"] = SPSDK.appSecret();
            jd["version"] = SPSDK.appVersionName();
            jd["channelname"] = SPSDK.channelId();
            jd["serverid"] = 110;
            jd["servername"] = "servername";
            jd["playerid"] = "999999";
            jd["playername"] = "roleName";
            jd["roleid"] = "100000";
            jd["extend"] = "";
            SPSDK.currencyReqMet("FeedBackPlugin", "SubmitFeedback", jd.ToJson());
        });

        roleUpgradeBtn.onClick.AddListener(delegate ()
        {

            SPSDK.roleUpgrade(1);

            writeLog("已调用角色升级接口");
        });

        goodsCloseBtn.onClick.AddListener(delegate ()
        {
            goodsPanel.gameObject.SetActive(false);
        });

        replaceOrderBtn.onClick.AddListener(delegate ()
        {
            SPSDK.replaceOrder();
        });

        cleanLogBtn.onClick.AddListener(delegate ()
        {
            cleanLog();
        });

        quitBtn.onClick.AddListener(delegate ()
        {

            SPSDK.killGame();

        });

        cancelBtn.onClick.AddListener(delegate ()
        {
            exitDialog.SetActive(false);
        });

        closePricePanel.onClick.AddListener(delegate ()
        {
            pricePanel.gameObject.SetActive(false);

            double price = System.Convert.ToDouble(priceField.text);
            if (price > 0 && curGoods != null)
            {
                SPSDK.payWilful(curGoods.getGoodsId(), price, curGoods.getPushInfo(), 100, 1);
            }
        });
    }

    public void showLoginView()
    {
        loginPanel.gameObject.SetActive(true);
        serverPanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(false);
    }

    private void showServerView()
    {
        loginPanel.gameObject.SetActive(false);
        serverPanel.gameObject.SetActive(true);
        gamePanel.gameObject.SetActive(false);

        startBtn.gameObject.SetActive(false);
    }

    private void showGameView()
    {
        loginPanel.gameObject.SetActive(false);
        serverPanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(true);


        logoutBtn.gameObject.SetActive(SPSDK.apiAvailable(ApiCode.LOGOUT));
        switchBtn.gameObject.SetActive(SPSDK.apiAvailable(ApiCode.SWITCH_USER));
        userCenterBtn.gameObject.SetActive(SPSDK.apiAvailable(ApiCode.USER_CENTER));
        serviceCenterBtn.gameObject.SetActive(SPSDK.apiAvailable(ApiCode.SERVICE_CENTER));



        GridLayoutGroup group = gamePanel.GetComponent<GridLayoutGroup>();
        if (isProtrait)
        {
            group.startAxis = GridLayoutGroup.Axis.Horizontal;
        }
        else
        {
            group.startAxis = GridLayoutGroup.Axis.Vertical;
        }
    }

    public void doGotHost(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        if (state_code == ResultCode.HOST_SUCCESS)
        {
            writeLog("获取网关回调成功\n");
        }
        else
        {
            writeLog(message);
        }
    }

    public void doAuth(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        if (state_code == ResultCode.AUTH_SUCCESS)
        {
            writeLog("已认证\n");
        }
        else
        {
            writeLog(message);
        }
    }

    public void showGoods(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];


        if (state_code == ResultCode.GOODS_FAILED)
            return;


        goodsPanel.gameObject.SetActive(true);

        string prefabName = "goods";
        if (isProtrait)
        {
            goodsPanel.Find("GoodsList").GetComponent<RectTransform>().sizeDelta = new Vector2(400, 600);
            goodsContent.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(400, 100);
            prefabName = "goodsprotrait";
        }

        clearGoods();

        try
        {
            JsonData json = JsonMapper.ToObject(jsonData["data"].ToJson());
            if (json.IsArray)
            {

                goodsContent.sizeDelta = new Vector2(0, json.Count * 100);

                for (int i = 0; i < json.Count; i++)
                {
                    JsonData jd = json[i];
                    UnityEngine.Object prefab = Resources.Load(prefabName);
                    GameObject goods = (GameObject)Instantiate(prefab, new Vector3(200, -50 - 100 * i), Quaternion.identity);
                    goods.transform.SetParent(goodsContent.transform);
                    goods.transform.localScale = goodsContent.transform.localScale;
                    double price = double.MaxValue;
                    // if ((jd["goods_price"]).IsArray)
                    price = (int)jd["goods_price"];
                    //if ((jd["goods_price"]).IsFloat())
                    //  price = (double)jd["goods_price"];
                    // if ((jd["goods_price"]).IsInt())
                    //  price = (long)jd["goods_price"];
                    Goods gd = new Goods((string)jd["goods_name"], (int)jd["goods_number"], jd["goods_id"].ToString(), (string)jd["goods_register_id"], price);
                    gd.setGoodsIcon((string)jd["goods_icon"]);
                    gd.setType((int)jd["type"]);
                    goods.transform.Find("name").GetComponent<Text>().text = gd.getGoodsName();
                    goods.transform.Find("desc").GetComponent<Text>().text = gd.getGoodsDescribe();
                    goods.transform.Find("price").GetComponent<Text>().text = gd.getGoodsPrice() + " rmb";
                    goods.transform.Find("icon").GetComponent<DownloadGoodsIcon>().loadPic(gd.getGoodsIcon());
                    goods.transform.Find("buyBtn").GetComponent<BuyGoods>().goods3 = gd;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }

    }

    private void clearGoods()
    {
        for (int i = 0; i < goodsContent.transform.childCount; i++)
        {
            GameObject go = goodsContent.transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }

    public void verifyToken(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];

        //如果为切换账号，游戏需退出当前账号并将游戏界面切换到登陆
        if (state_code == ResultCode.SWITCH_USER_SUCCESS)
        {
            showLoginView();
        }

        if (state_code == ResultCode.LOGIN_SUCCESS || state_code == ResultCode.SWITCH_USER_SUCCESS)
        {
            string[] e1 = { "正式", "测试", "预发布" };

            appInfo.text = SPSDK.appName() + "(sdk版本:" + SPSDK.sdkVersionName() +
            " 渠道版本:" + SPSDK.channelVersion() +
            " 渠道号:" + SPSDK.channelId() +
            " 环境:" + e1[SPSDK.mode()] +
            ")";
            //此处为模拟接口，正式接入时游戏应将数据发给游戏服务器，由游戏服务器发起token验证请求
            try
            {
                JsonData json = JsonMapper.ToObject(jsonData["data"].ToJson());
                string vi = (string)json["validateInfo"];
                JsonData jd = new JsonData();
                jd["validateInfo"] = vi;
                string opcode = (string)json["opcode"];
                string channelid = (string)json["channel_id"];
                StartCoroutine(sendReq(jd.ToJson(), opcode, channelid));

            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }
        else
        {
            writeLog(message);
            SPSDK.showToast(message);
        }
    }
    IEnumerator sendReq(string info, string opcode, string channelid)
    {
        string appkey = "";
        string appsecret = "";

        string url = "http://gateway.changyou.com/gameserver/verifytoken";
        int debugMode = SPSDK.mode();
        if (ResultCode.MODE_RELEASE == debugMode)
        {
            url = "http://gateway.changyou.com/gameserver/verifytoken";
        }
        else if (ResultCode.MODE_PRERELEAS == debugMode)
        {
            url = "http://ygateway.changyou.com/gameserver/verifytoken";
        }
        else if (ResultCode.MODE_DEBUG == debugMode)
        {
            url = "http://tgateway.changyou.com/gameserver/verifytoken";
        }

        appkey = SPSDK.appKey();
        appsecret = SPSDK.appSecret();

        string tag = createTag();
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("appkey", appkey);
        header.Add("sign", createSign(appkey, info, appsecret, opcode, channelid, tag));
        header.Add("tag", tag);
        header.Add("opcode", opcode);
        header.Add("channelId", channelid);
        byte[] postBytes = Encoding.UTF8.GetBytes("data=" + info);
        WWW www = new WWW(url, postBytes, header);
        yield return www;
        if (www.error == null)
        {//服务器正常返回
            string response = www.text;
            writeLog(response);
            JsonData json = JsonMapper.ToObject(response);
            if ((int)json["state"] == 200)
            {
                //解析服务器返回的数据
                JsonData retData = JsonMapper.ToObject(json["data"].ToJson());
                string status = (string)retData["status"];
                string gameUid = (string)retData["userid"];
                string channelOid = "";
                string accessToken = "";
                if (((IDictionary)retData).Contains("oid"))
                    channelOid = (string)retData["oid"];
                if (((IDictionary)retData).Contains("access_token"))
                    accessToken = (string)retData["access_token"];
                bool pass = (status == "1");
                if (((IDictionary)retData).Contains("extension"))
                {
                    string extStr = (string)retData["extension"];
                    SPSDK.otherVerify(extStr);
                }

                SPSDK.tokenVerify(pass, gameUid, channelOid, accessToken);

                if (pass)
                    showServerView();
                else
                {
                    SPSDK.showToast("token验证失败");
                }
            }
        }
        else
        {
            writeLog(www.error);
            SPSDK.showToast(www.error);
        }
    }

    public void doLogout(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        if (state_code == ResultCode.LOGOUT)
        {
            showLoginView();
        }
        else
        {
            writeLog(message);
        }
    }

    public void doExit(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        if (state_code == ResultCode.EXIT_GAME)
        {
            //渠道有退出框，并且用户点击了退出，回调该方法
            //游戏需要回收游戏相关资源 
            //退出游戏
            writeLog("渠道有退出框,并且用户点击了退出\n");
            SPSDK.killGame();
        }
        else if (state_code == ResultCode.EXIT_GAME_DIALOG)
        {
            //渠道没有退出框，需要使用游戏自己实现的退出窗口并且完成退出相关的逻辑
            writeLog("使用游戏自己实现的退出窗口\n");
            exitDialog.SetActive(true);
        }
    }

    public void doPayment(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        if (state_code == ResultCode.PAY_SUCCESS)
        {
            writeLog("支付成功回调\n");
        }
        else
        {
            writeLog("支付失败或者取消回调\n");
        }
    }

    public void doPlugin(string jsonParam)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonParam);
        int state_code = (int)jsonData["state_code"];
        string message = (string)jsonData["message"];
        JsonData jsonResult = JsonMapper.ToObject((string)jsonData["data"]);
        string result = jsonResult["result"].ToString();
        switch (result)
        {
            case PluginCode.BIND_SUCESS:
            case PluginCode.CLOSE:
            case PluginCode.COMPLAINT_SUCESS:
            case PluginCode.FEEDBACK_SUCESS:
            case PluginCode.GET_PLAYER_INFO:
            case PluginCode.RECEIVE_MESSAGES:
            case PluginCode.SHOW_ICON:
            case PluginCode.TIMEOUT:
            case PluginCode.UNBIND_SUCESS:
                writeLog(jsonParam);
                break;
            default:
                writeLog("参数错误" + jsonParam);
                break;
        }
    }



    private string createTag()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(100000, 9999999).ToString();
    }

    private string createSign(string appkey, string data, string appsecret, string opcode, string channelId, string tag)
    {
        if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(opcode))
            return "";
        return Md5_16(opcode + data + appkey + appsecret + tag + channelId);
    }

    public static string Md5_16(string convertString)
    {
        if (convertString == null || convertString.Length == 0)
            return "";
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string result = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(convertString)), 4, 8);
        return result.Replace("-", "").ToLower();
    }

    public void writeLog(string text)
    {
        //		if (logText == null)
        //			return;
        string newText = logText.text + text + "\n";
        logText.text = newText;
        logContent.sizeDelta = new Vector2(0, logText.preferredHeight + 20);
    }

    private void cleanLog()
    {
        logText.text = "";
        logContent.sizeDelta = new Vector2(0, 20);
    }

    void Update()
    {
        UpdateTick();
        fpsText.text = "FPS:" + mLastFps;
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            if (goodsPanel.gameObject.activeInHierarchy)
            {
                goodsPanel.gameObject.SetActive(false);
            }
        }
    }
    private long mFrameCount = 0;
    private long mLastFrameTime = 0;
    public static long mLastFps = 0;
    private float mRefreshTime = 1000;
    private void UpdateTick()
    {
        if (true)
        {
            mFrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (mLastFrameTime == 0)
            {
                mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - mLastFrameTime) >= mRefreshTime)
            {
                long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

                mLastFps = fps;

                mFrameCount = 0;

                mLastFrameTime = nCurTime;
            }
        }
    }
    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }
}
