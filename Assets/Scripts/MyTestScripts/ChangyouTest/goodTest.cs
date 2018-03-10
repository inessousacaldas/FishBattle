using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class goodTest : MonoBehaviour {


    public static goodTest Instance;

    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start () {

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            PayManager.Instance.OnPayItemsLoadSuccess += GetGoods;
            SPSdkManager.Instance.OnCYCallback("{\"state_code\":800,\"message\":\"获取商品列表成功\",\"data\":[{\"goods_describe\":\"测试商品\",\"goods_icon\":\"icon01\",\"goods_id\":\"6615\",\"goods_name\":\"测试商品\",\"goods_number\":0,\"goods_price\":10,\"goods_register_id\":\"1\",\"type\":0},{\"goods_describe\":\"测试商品2\",\"goods_icon\":\"icon2\",\"goods_id\":\"6617\",\"goods_name\":\"测试商品2\",\"goods_number\":100,\"goods_price\":2,\"goods_register_id\":\"2\",\"type\":1},{\"goods_describe\":\"测试商品3\",\"goods_icon\":\"icon3\",\"goods_id\":\"6618\",\"goods_name\":\"测试商品3\",\"goods_number\":99999999,\"goods_price\":3,\"goods_register_id\":\"3\",\"type\":2},{\"goods_describe\":\"测试商品4\",\"goods_icon\":\"icon4\",\"goods_id\":\"6619\",\"goods_name\":\"测试商品4\",\"goods_number\":99999999,\"goods_price\":4,\"goods_register_id\":\"4\",\"type\":3}]}");
        }
        else
        {
            PayManager.Instance.OnPayItemsLoadSuccess += GetGoods;
        }
        

    }

    private Dictionary<int, OrderItemJsonDto> dic = new Dictionary<int, OrderItemJsonDto>();

    public void GetGoods()
    {
        dic= PayManager.Instance.GetPayItemDic();
        PayManager.Instance.OnPayItemsLoadSuccess -= GetGoods;
    }

    private void OnGUI()
    {
        if (dic != null && dic.Count > 0)
        {
            int index = 0;
            foreach (var good in dic)
            {
                if (GUI.Button(new Rect(Screen.width / 2, index * 50, 200, 40), good.Value.id))
                {  
                    PayManager.Instance.CyCharge(good.Value.gameShopItemId);
                }
                index++;
            }


        }
    }


    [ContextMenu("Logout")]
    void Logout()
    {
        SPSdkManager.Instance.OnCYCallback("{\"state_code\":200,\"message\":\"注销\",\"data\":{}}");
    }

    [ContextMenu("LoginSuccess")]
    void Login()
    {
        SPSdkManager.Instance.OnCYCallback("{\"state_code\":300,\"message\":\"登录成功\",\"data\":{\"validateInfo\":\"7be82a5a1d9720cf0fccd47638dfe4f89794c88e26185797e6544bbf263ee1472d5e127981ec5bdf9f7508cdce5c8a8b56f7213fdd4cb36eaf095530e98f79dc4391fff834788e1c5938e0e6d3e5aeef9a90ece58906eb6764eb279d69f04cf41fc63a1392a585791d82745eb1218faf9e91d93d1e21c18c4eca128a94db58bf257a9cab548ffba7e4075aa153279bfe0a36ed6d83b215c844bf8c2cbb5dc114ebd92b4e8c56fd315c01c214d8868da28f4d6b7503a1ceb4c010cd1b55e9e63aefecfff615c1c10b8e7e4cc591545881732ce8d20b755a9bbe1ba5d5d968c5a172ab2b1b0e48bbb58e6de6cdccf9519f98eac2759101289ee88d4cdce855667cd236be3168216eacc852cac6a4d056269fa5c3ce21b7b45c49d25ea135c2ba0c78cf483d785c69f0\",\"channel_id\":\"4001\",\"opcode\":\"10001\"}}");
    }



}

