using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class BackpackDataMgr
{
    // 包裹初始化
    public static class BackPackNetMsg
    {
        public static void ReqItemsInBagAndTemp(Action onComplete = null, Action onError = null)
        {
            var b = DataMgr;
//            long version = GameDataManager.DataMgr.GetDataVersion(GameDataManager.Data_Self_PackDto_Backpack);
            ServiceRequestAction.requestServer(Services.Backpack_Check()
                , ""
                , delegate(GeneralResponse response) {
                    GameUtil.SafeRun(onComplete);
                  }
                , delegate(ErrorResponse response)
                {
                    GameUtil.SafeRun(onError);
                });
            ServiceRequestAction.requestServer(Services.Temppack_Check()
                , ""
                , delegate(GeneralResponse response) {
                    GameUtil.SafeRun(onComplete);
                }
                , delegate(ErrorResponse response)
                {
                    GameUtil.SafeRun(onError);
                });
        }
        //    分解
// 合成
// 整理
        public static void SortBagItems(Action onSuccess = null, Action onFail = null)
        {
            GameUtil.GeneralReq(Services.Backpack_Sort(), resp => {
                DataMgr._data.lastSortBagTime = DateTime.Now;
                BackpackDataMgr.DataMgr.HandleBagDtoNotify(resp as BagDto);
                GameUtil.SafeRun(onSuccess);
            },null, error => { GameUtil.SafeRun(onFail); });
        }

        public static void ExpandBag(Action onSuccess = null, Action onFail = null)
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_12, true)) return;

            var itemid = DataCache.GetStaticConfigValue(AppStaticConfigs.BACKPACK_EXPAND_CONSUME_ITEM_ID, 0);
            
            var item = ItemHelper.GetGeneralItemByItemId(itemid);

            //            var itemNum = DataMgr.GetItemCountByItemID(itemid);
            //            if (itemNum < DataMgr._data.expandConsumeNum)
            //            {
            //                TipManager.AddTip(item.name + "数量不足");
            //                return;
            //            }

            int hadCount =  DataMgr.GetItemCountByItemID(itemid);
            var set = new List<ItemUseTipsCellVo>(1){ new ItemUseTipsCellVo { item = item, hadCount = hadCount,needCount = DataMgr._data.expandConsumeNum } };
            ItemUseTipsViewController.Open(
                set
                , "包裹解锁", string.Format("[383838]你是否确定使用[174181]{1}[-] [0c8130] x {0}[-]来解锁包裹空间？", DataMgr._data.expandConsumeNum,item.name)
                , delegate
                {
                    GameUtil.GeneralReq(Services.Backpack_Expand(), resp => {
                        DataMgr.HandleBagDtoNotify(resp as BagDto);
                        GameUtil.SafeRun(onSuccess);
                        UIModuleManager.Instance.CloseModule(ItemUseTipsView.NAME);
                    },null, error => { GameUtil.SafeRun(onFail); });  
                });
        }

        //  临时背包移动物品到背包
        public static void TransItemFromTempBagToBack(
            int idx
            , Action onSuccess = null
            , Action onFail = null){
            if (idx < 0)
            {

            }
            else if (!DataMgr._data.CheckTempItemNull(idx))
            {
                return;
            }
            else
            {
                GameUtil.GeneralReq(
                    Services.Temppack_ToBackpack(idx)
                    , resp =>
                    {
                        GameUtil.SafeRun(onSuccess);
                    }
                    , null,error => {GameUtil.SafeRun(onFail); });
            }
        }

        public static void TransAllItemFromTempBagToBack(
            Action onSuccess = null
            , Action onFail = null)
        {
            GameUtil.GeneralReq(Services.Temppack_AllToBackpack(),
                resp => {
                    GameUtil.SafeRun(onSuccess);
                }, null,
                error => 
                { GameUtil.SafeRun(onFail); });
        }

        public static void ReqWarehouseDto(
            Action onSuccess = null
            , Action<ErrorResponse> onFail = null)
        {
            GameUtil.GeneralReq(
                Services.Warehouse_Check()
            , delegate(GeneralResponse resp)
                {
                    DataMgr.HandleWarehouseDtoNotify(resp as WarehouseDto);
                }
            , onSuccess
            , onFail);
        }

        /// <summary>
        /// 更改背包名字
        /// </summary>
        /// <param name="newName"></param>
        public static void ReqModifyWarehouseName(string newName)
        {
            

            GameUtil.GeneralReq(
                Services.Warehouse_EditPageName(DataMgr._data.CurWareHousePage, newName)
                , delegate(GeneralResponse resp)
                {
                    DataMgr._data._wareHouseNameStrSet[DataMgr._data.CurWareHousePage] = newName;
                    FireData();
                }
                , () =>
                {
                    TipManager.AddTip("仓库改名成功");
                    ProxyWindowModule.closeInputWin();
                });
        }

        public static void ReqExpandWarehouseCap()
        {
            //var itemid = DataCache.GetStaticConfigValue(AppStaticConfigs.BACKPACK_EXPAND_CONSUME_ITEM_ID, 0);
            //当前的解锁次数
            int curUnlockTimes = ExpressionManager.UnlockWarehouseSlots(DataMgr._data.wareHousePackDto.capability);
            
            var wareHouseExpandConfig =  DataCache.getDtoByCls<WareHouseExpand>(curUnlockTimes + 1);
            var itemid = wareHouseExpandConfig.virtualId;
            var item = ItemHelper.GetGeneralItemByItemId(itemid);

            //            var itemNum = DataMgr.GetItemCountByItemID(itemid);
            //            if (itemNum < DataMgr._data.expandConsumeNum)
            //            {
            //                TipManager.AddTip(item.name + "数量不足");
            //                return;
            //            }

            // 样式不对
            var hadCount = ModelManager.IPlayer.GetPlayerWealth((AppVirtualItem.VirtualItemEnum)itemid);
            var needCount = wareHouseExpandConfig.count;
            var set = new List<ItemUseTipsCellVo>(1){ new ItemUseTipsCellVo() { item = item, hadCount= hadCount,needCount = needCount } };

            string otherTips = "\n(绑钻不足时自动使用钻石补足)";
            
            string showText = string.Format("[383838]你确定使用[174181] {0} [-] [0c8130]x{1}[-] 来解锁仓库空间？", item.name, wareHouseExpandConfig.count);
            if (item.id == (int)AppVirtualItem.VirtualItemEnum.BINDDIAMOND)
                showText += otherTips;

            ItemUseTipsViewController.Open(
                set
                , "仓库解锁",
                showText
                ,
                () =>
                {
                    ExChangeHelper.CheckIsNeedExchange((AppVirtualItem.VirtualItemEnum)item.id, wareHouseExpandConfig.count, () =>
                    {
                        GameUtil.GeneralReq(Services.Warehouse_Expand(), resp =>
                        {
                            UIModuleManager.Instance.CloseModule(ItemUseTipsView.NAME);
                        }, null, error =>
                        {

                        });
                    });
                });
        }

        public static void ReqSortWareHouse(int page)
        {
            if (page < 0)
            {
                return;
            }
            

            GameUtil.GeneralReq(
                Services.Warehouse_Sort(page)
                , resp =>
                {
                    var _bagDto = resp as BagDto;
                    DataMgr._data.SelectWarehouseItemIdx = -1;
                    DataMgr._data.lastSortWareHouseTime = DateTime.Now;
                    DataMgr.HandleBagDtoNotify(_bagDto);
                }
            );
        }

        public static void ReqMoveItemToBackPack(int idx)
        {
            if ( idx < 0)
            {
                TipManager.AddTip("请选择一个物品");
                return;
            }

            GameUtil.GeneralReq(
                Services.Warehouse_ToBackpack(idx)
                , resp=>{
                DataMgr._data.SelectWarehouseItemIdx = -1;
                FireData();
            }
            );
        }

        public static void ReqMoveItemToWareHouse(int itemIdx, int warehouseIdx)
        {
            if (itemIdx < 0)
            {
                TipManager.AddTip("请选择一个物品");
                return;
            }
            if (warehouseIdx < 0)
            {
                TipManager.AddTip("请选择一个仓库");
                return;
            }

            GameUtil.GeneralReq(
                Services.Backpack_ToWarehouse( itemIdx, warehouseIdx)
                , resp=>{
                DataMgr._data.CurBackPackSelectIdx = -1;
                FireData();
            });
        }

        public static void ReqComposeItem(int propsId,int count,int circulationType,bool quick)
        {
            if (propsId <= 0)
            {
                TipManager.AddTip("请添加合成材料");
                return;
            }
            else if (count <= 0)
            {
                TipManager.AddTip("合成材料不足");
                return;
            }
            else if ( DataMgr._data.GetLeftSlotsInBagAndTemp() <=5
                && DataMgr._data.IsShowComposeTips)
            {
                //ProxyTipsModule.ShowConsumerTips(ConsumerTipsViewData.Create(
                //    contentStr:"包裹剩余格子不多，合成后可能丢失大部分道具， 确定继续合成？"
                //    , isShowCheckBox:true
                //    , checkBoxLabel:"本次登录不再提示"
                //    , checkBoxState:false)
                //    , isShow =>
                //{
                //    DataMgr._data.IsShowComposeTips = !isShow;
                //    GameUtil.GeneralReq(Services.Items_CompositeProps( propsId, count, circulationType, quick),resp=> {
                //        TipManager.AddTopTip("合成成功");
                //    });
                //}
                //, null);

                var controller = ProxyBaseWinModule.Open();
                BaseTipData tipData = BaseTipData.Create("提示", "包裹剩余格子不多，合成后可能丢失大部分道具， 确定继续合成？", 0, () =>
                {
                    GameUtil.GeneralReq(Services.Items_CompositeProps(propsId, count, circulationType, quick), resp =>
                    {
                        TipManager.AddTopTip("合成成功");
                    });
                }, null);

                controller.InitView(tipData);

                return;
            }

            GameUtil.GeneralReq(Services.Items_CompositeProps(propsId, count, circulationType, quick), resp =>
            {
                TipManager.AddTopTip("合成成功");
            });
        }

        public static void ReqDecomposeItem(int propsId, int count, Action onSuccess = null)
        {
            if (propsId <0)
            {
                TipManager.AddTip("请添加分解道具");
                return;
            }else if (DataMgr._data.GetLeftSlotsInBagAndTemp() <= 5)
            {
                TipManager.AddTip("请先清理临时包裹，以防道具丢失");
                return;
            }
            if(count <= 0)
            {
                TipManager.AddTip("请添加分解道具");
                return;
            }
            GameUtil.GeneralReq(Services.Items_ResolveProps(propsId, count)
                , delegate(GeneralResponse e)
                {
                    DataMgr.RecieveDecGainItem(e);
                    TipManager.AddTopTip("分解成功");
                }
                ,onSuccess);
        }

        public static void BackpackApply(int index, int count, string opt = "", Action callback = null)
        {
            GameUtil.GeneralReq(Services.Backpack_Apply(index, count, opt), e =>
            {
                TipManager.AddTip("物品使用成功");
                if (callback != null)
                    callback();
                FireData();
            });
        }

        //分解结晶回路
        public static void ResolveQuartz(string uids)
        {
            GameUtil.GeneralReq(Services.Items_ResolveQuartz(uids), resp =>
            {
                TipManager.AddTopTip("分解成功");
            });
        }

        //分解装备
        public static void ResolveEquipment(string uids)
        {
            GameUtil.GeneralReq(Services.Items_ResolveEquipment(uids), resp =>
            {
                TipManager.AddTopTip("分解成功");
            });
        }
    }

}
