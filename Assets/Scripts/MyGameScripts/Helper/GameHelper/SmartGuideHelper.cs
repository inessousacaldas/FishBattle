using System;
using UnityEngine;
using System.Collections;
using AppDto;

public class SmartGuideHelper
{
    private enum OpenWinType
    {
        OpenChatView = 5,
        OpenEquipView = 8,
        OpenCrewRecruitView = 15,
        OpenShopView = 12,
        OpenFormationView = 14,  //队形
        OpenCrewFavorView = 17,  //伙伴好感度
        OpenCraftsView = 18,     //伙伴技巧
        OpenDevelopView = 19,    //伙伴进阶   
        OpenTechnicView = 20,    //伙伴战技
        OpenQuartzForceView = 21,   //结晶回路打造
        OpenQuartzStrenghtView = 22,    //结晶回路强化
        OpenAssistSkillView = 24,   //生活技能
        OpenTrainingView = 25,   //伙伴研修
        OpenSkillSpecialityView = 26,   //技能专精
        OpenEquipGenView = 27,   //技能原石
        OpenMedallionView = 28,     //装备纹章
        OpenItemQuickBuyView = 29,  //快捷购买
    }

    public static void GuideTo(int id, 
        int propsId = -1, 
        Action callback = null)
    {
        var smartGuide = DataCache.getDtoByCls<SmartGuide>(id);
        if (smartGuide == null)
            return;

        UIModuleManager.Instance.CloseModule(GainWayTipsView.NAME);
        switch ((SmartGuide.SmartGuideType)smartGuide.type)
        {
            case SmartGuide.SmartGuideType.SmartGuideType_1:
                ProxyShop.OpenShopByType(int.Parse(smartGuide.param));
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_2:
                switch ((OpenWinType)int.Parse(smartGuide.param))
                {
                    case OpenWinType.OpenChatView:
                        ProxySociality.OpenChatMainView(ChatPageTab.Friend);
                        break;
                    case OpenWinType.OpenEquipView:
                        ProxyEquipmentMain.Open();
                        break;
                    case OpenWinType.OpenCrewRecruitView:
                        ProxyCrewReCruit.Open();
                        break;
                    case OpenWinType.OpenShopView:
                        var goodslist = DataCache.getArrayByCls<ShopGoods>();
                        var goods = goodslist.Find(d => d.itemId == propsId);
                        if (goods == null)
                        {
                            GameDebuger.LogError("数据异常,请检查");
                            return;
                        }
                        var shop = DataCache.getDtoByCls<Shop>(goods.shopId);
                        var type = GetShopType(shop.shopType);
                        ProxyShop.OpenShop(type, (ShopTypeTab)shop.shopType, goods.id);
                        break;
                    case OpenWinType.OpenItemQuickBuyView:
                        ProxyItemQuickBuy.OpenQuickBuyView(propsId);
                        break;
                }
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_3:
                var npc = DataCache.getDtoByCls<Npc>(StringHelper.ToInt(smartGuide.param));
                //当前是否在四轮之塔，里面做了确定/取消判断，
                if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate
                {
                    WorldManager.Instance.FlyToByNpc(npc);
                })) return;
                WorldManager.Instance.FlyToByNpc(npc);
                if (callback != null)
                    callback();
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_4:
                TipManager.AddTip(smartGuide.param);
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_5:    //打开日程
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_6:    //任务追踪
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_7:    //特殊商店
                break;
            case SmartGuide.SmartGuideType.SmartGuideType_100:  //其他
                break;
        }
    }

    private static ShopTypeTab GetShopType(int shopId)
    {
        switch ((ShopTypeTab)shopId)
        {
            case ShopTypeTab.LimitShopId:
                return ShopTypeTab.LimitShop;
            case ShopTypeTab.BinnianmondShopId:
                return ShopTypeTab.BinnianmondShop;
            case ShopTypeTab.ScroeShopId:
                return ShopTypeTab.ScoreShop;
            case ShopTypeTab.ArenaScroeShopId:
                return ShopTypeTab.ArenaShop;
            default:
                return ShopTypeTab.None;
        }
    }
}
