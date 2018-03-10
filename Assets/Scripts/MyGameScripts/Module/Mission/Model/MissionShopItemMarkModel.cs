using UnityEngine;
using System.Collections.Generic;
using AppDto;

public class MissionShopItemMarkModel {

    //=======================================玩家便捷打开商店相关=======================================
    private int _quickShopItemID = -1;
    public int QuickShopItemID {
        get { return _quickShopItemID; }
        set { _quickShopItemID = value; }
    }

    private int _weaPonShopItemID = -1;
    public int WeaPonShopItemID {
        get { return _weaPonShopItemID; }
    }

    #region 便捷商店打开类型枚举
    public enum OpenShopQuickMenu {
        Nothing = 0,
        GroceryStore,
        WeaPonStore
    }

    private OpenShopQuickMenu _openShopQuickMenu = OpenShopQuickMenu .Nothing;

    public OpenShopQuickMenu openShopQuickMenu {
        get { return _openShopQuickMenu; }
    }
    #endregion



    private Dictionary<int,ItemNpcRelation> _itemNpcRelationDic = null;
    public void SetShopItemMark(SubmitDto tSubmitDto) {
        if(tSubmitDto.count < tSubmitDto.needCount) {
            if(MissionHelper.IsCollectionItemOrItemCategory(tSubmitDto)) {
                int tItemID = MissionHelper.IsCollectionItem(tSubmitDto)?(tSubmitDto as CollectionItemSubmitDto).itemId
                    :MissionHelper.IsCollectionItemOrItemCategory(tSubmitDto)?(tSubmitDto as CollectionItemCategorySubmitDto).itemCategory.itemIds[0]:0;
                Dictionary<int, ItemNpcRelation> tItemNpcRelationDic = GetItemNpcRelationDic();
                if(tItemNpcRelationDic.ContainsKey(tItemID)) {
                    ItemNpcRelation tItemNpcRelation = tItemNpcRelationDic[tItemID];
                    //根据Shop表id来区分5杂货，6武器，7.导力
                    switch(tItemNpcRelation.shopCategory) {
                        case 0:
                            GameDebuger.LogError("商品的shopCategory出错:请查阅ItemNpcRelation表");
                            break;
                        case 5:
                            _openShopQuickMenu = OpenShopQuickMenu.GroceryStore;
                            _quickShopItemID = tItemID;
                            break;
                        case 6:
                            _openShopQuickMenu = OpenShopQuickMenu.WeaPonStore;
                            _weaPonShopItemID = tItemID;
                            break;

                    }
                }
            }
        }
    }


    #region 获取itemid为主键的 物品和npc的关系 shopitem(静态表)
    public Dictionary<int,ItemNpcRelation> GetItemNpcRelationDic()
    {
        if(_itemNpcRelationDic == null) {
            _itemNpcRelationDic = DataCache.getDicByCls<ItemNpcRelation>(); 
        }
        return _itemNpcRelationDic;
    }
    #endregion


    public void ClearShopItemMark() {
        if(_openShopQuickMenu != OpenShopQuickMenu.Nothing) {
            if(_openShopQuickMenu == OpenShopQuickMenu.GroceryStore)
            {
                _quickShopItemID = -1;
            }
            else if(_openShopQuickMenu == OpenShopQuickMenu.WeaPonStore)
            {
                _weaPonShopItemID = -1;
            }
            else {
                GameDebuger.LogError("_openShopQuickMenu数据出错："+_openShopQuickMenu);
            }
            _openShopQuickMenu = OpenShopQuickMenu.Nothing;
        }
    }
}
