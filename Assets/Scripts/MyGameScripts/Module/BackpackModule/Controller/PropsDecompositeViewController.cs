using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
public sealed partial class BackpackDataMgr
{
    public partial class PropsCompositeViewController
    {

        public static int MaxDecAcquireType = 5;    //分解物品后所得物品的最大值

        

        private IDeCompositeItem curDecMaterialItem;
        //private IEnumerable<IDeCompositeItem> acquireItems;
        private ItemCellController decMaterialCellCtl;
        private ItemCellController accquireItemCtrl;
        private List<ItemCellController> accquireItemCtrlList; //分解所得预览
        private TabbtnManager tabMgr = null;
        private PageTurnViewController decomposePageTurn;
        private DecomposeGainPropsViewController decGainCtrl;

        //初始化物品筛选按钮
        private void InitDecPageTabBtns()
        {
            //var idx = DecItemTabName.FindElementIdx(
            //    s => (DecompositeTabType)s.EnumValue == DataMgr._data.CurDecomposeTab);
            //var tabBtnMgr = TabbtnManager.Create(
            //    DecItemTabName
            //    , delegate(int i)
            //    {
            //        return AddChild<TabBtnWidgetController, TabBtnWidget>(
            //            _view.DecTabbtnPos
            //            , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
            //            , "DecTabbtn" + i);
            //    }
            //    ,idx);

            //_disposable.Add(tabBtnMgr.Stream.Subscribe(
            //    pageIdx =>
            //    {
            //        DataMgr._data.CurDecomposeTab = (DecompositeTabType)DecItemTabName[ pageIdx].EnumValue;
            //        FireData();
            //    }
            //    ));
            
        }

        //初始化分解栏item与获得栏item
        private void InitDecomposeItemCell()
        {
            decMaterialCellCtl = AddController<ItemCellController, ItemCell>(
                _view.DecomposeItem
                );

            //accquireItemCtrlList = new List<ItemCellController>();
            //for (int i = 0; i <MaxDecAcquireType; i++)
            //{
            //    var ctrl = AddChild<ItemCellController, ItemCell>(
            //        _view.AcquisitionPos, ItemCell.Prefab_BagItemCell
            //        );
            //    accquireItemCtrlList.Add(ctrl);
            //    ctrl.Hide();
            //}

            accquireItemCtrl = AddController<ItemCellController, ItemCell>(
                _view.AcquisitionItem
                );
        }

        //初始化操作次数控制栏
        private void InitDecomposePageTurn()
        {
            decomposePageTurn = AddController<PageTurnViewController, PageTurnView>(
                _view.DecomposePageTurn
                );
            decomposePageTurn.InitData_NumberInputer(0, 0, pos: PageTurnViewController.InputerShowPos.Down);

            //decomposePageTurn.InitData(
            //    showType: ShowType.singleNum_Zero
            //    ,enableInput:true);

            _disposable.Add(decomposePageTurn.Stream.Subscribe(
                pageIdx =>
                {
                    //更新分解次数及消耗
                    var max = GetDecomposeMaxTime();
                    DataMgr._data.DecomposeTimes = Math.Min(pageIdx, max);
                    //decomposePageTurn.UpdateView(DataMgr._data.DecomposeTimes, max +1);
                    decomposePageTurn.SetPageInfo(DataMgr._data.DecomposeTimes, max);
                    UpdateDecomposePrice(DataMgr._data);
                }
                ));
        }

        //初始化分解成功后弹出的物品获得栏
        private void InitPopupPanel()
        {
            decGainCtrl = AddChild<DecomposeGainPropsViewController, DecomposeGainPropsView>(
                _view.DecompositeContent
                ,DecomposeGainPropsView.NAME
                );
            var parentDepth = decGainCtrl.gameObject.ParentPanelDepth();
            decGainCtrl.gameObject.ResetPanelsDepth(parentDepth + 2);
            _disposable.Add(decGainCtrl.OnBgBoxCollider_UIButtonClick.Subscribe(_ => decGainCtrl.Hide()));
            decGainCtrl.Hide();
        }

        //更新分解栏显示
        private void UpdateDecPage(ICompositViewData data)
        {
            if (curDecMaterialItem == null
                || curDecMaterialItem.Item == null)
            {
                decMaterialCellCtl.UpdateViewWithNull();
                decMaterialCellCtl.SetCountTxt(0, string.Empty);
                decMaterialCellCtl.SetNameLabel(string.Empty);
            }
            else
            {
                decMaterialCellCtl.UpdateView(curDecMaterialItem.Item);
                decMaterialCellCtl.SetCountTxt(curDecMaterialItem.Cnt, "拥有数量{0}");
                var colStr = ItemHelper.GetItemNameColorByID(curDecMaterialItem.Item.id);
                decMaterialCellCtl.SetNameLabel(curDecMaterialItem.Item.name, colStr);
            }
            UpdateAcquireItems(data);
            UpdateDecomposePrice(data);

            var max = GetDecomposeMaxTime();
            //decomposePageTurn.UpdateView(DataMgr._data.DecomposeTimes, max + 1);
            decomposePageTurn.SetPageInfo(DataMgr._data.DecomposeTimes, max);
        }


        //将分解后所得显示
        private void UpdateAcquireItems(ICompositViewData data)
        {
            int idx = 0;

            if (data.DecAcquireItems.IsNullOrEmpty())
            {
                accquireItemCtrl.UpdateViewWithNull();
                accquireItemCtrl.SetCountTxt(0, string.Empty);
                accquireItemCtrl.SetNameLabel(string.Empty);
                return;
            }
            data.DecAcquireItems.ForEach(x => {
                idx++;
                if(idx <= 1)
                {
                    accquireItemCtrl.UpdateView(x);
                    accquireItemCtrl.SetCountTxt(0, string.Empty);
                    var colStr = ItemHelper.GetItemNameColorByID(x.id);
                    accquireItemCtrl.SetNameLabel(x.name, colStr);
                }
            });
           
            //更新分解获得物品
            //acquireItems.ForEach(dto =>
            //{
            //    if (dto.Item != null)
            //    {
            //        idx++;
            //        ctrl = itemCtrlList[idx];
            //        ctrl.UpdateView(dto.Item);
            //        ctrl.SetCountTxt(dto.Cnt, "可得：{0}");
            //        //var colStr = ItemHelper.GetItemNameColorByID(dto.Item.id);
            //        ctrl.Show();
            //    }
            //});

            //if (idx != -1)
            //{
            //    for (int i = idx + 1; i < accquireItemCtrlList.Count; i++)
            //    {
            //        accquireItemCtrlList[i].Hide();
            //    }
            //}
        }

        //更新分解消耗显示
        private void UpdateDecomposePrice(ICompositViewData data)
        {
            var ty = AppVirtualItem.VirtualItemEnum.NONE;
            var cnt = 0;
            if (curDecMaterialItem != null
                && curDecMaterialItem.Item != null)
            {
                var props = DataCache.getDtoByCls<ResolveProps>(curDecMaterialItem.Item.id);
                if (props != null)
                {
                    cnt = data.DecomposeTimes * props.resolvePrice;
                    ty = (AppVirtualItem.VirtualItemEnum) props.virtualItemId;
                }
            }
            //_view.DecConsume_UILabel.text = cnt.ToString();
            _view.DecConsumeContent_UILabel.SetAppVirtualItemIconAndNum(
                ty
                , cnt
                , "分解消耗：");

        }

        //这是一个显示需求： 最小显示一个，即使未选择分解物品
        private int GetDecomposeMaxTime()
        {
            if (curDecMaterialItem == null)
                return 0;
            var props = DataCache.getDtoByCls<ResolveProps>(curDecMaterialItem.Item.id);
            if (props == null)
                return 1;
            return curDecMaterialItem.Cnt == 0 ? 1 : curDecMaterialItem.Cnt;
            //return acquireItems == null ? 1 : curDecMaterialItem.Cnt;
        }

        /// <summary>
        /// 分解按钮
        /// </summary>
        private void DecomposeBtn_UIButtonClickHandler()
        {
            if (curDecMaterialItem == null)
            {
                TipManager.AddTip("请添加分解材料");
                return;
            }

            var props = DataCache.getDtoByCls<ResolveProps>(curDecMaterialItem.Item.id);
            var ty = (AppVirtualItem.VirtualItemEnum) props.virtualItemId;
            var cnt = DataMgr._data.DecomposeTimes * props.resolvePrice;
            //if (ModelManager.Player.EnoughVirtualCurrency(ty, cnt))
            //{
            //    TipManager.AddTip("所需货币不足,弹出充值界面");
            //    return;
            //}

            //数字5要支持静态导表
            //if (DataMgr._data.GetLeftSlotsInBagAndTemp() <= 5)
            //{
            //    TipManager.AddTip("请先清理临时包裹,以防道具丢失");
            //    return;
            //}                

            //OnBgBoxCollider_UIButtonClickHandler();
            BackPackNetMsg.ReqDecomposeItem(
                curDecMaterialItem.Item.id
                , DataMgr._data.DecomposeTimes
                , () => DecomposeSuccess());
        }

        //分解成功后
        private void DecomposeSuccess()
        {
            //从GainDtoList取得服务器传回的数据
            decGainCtrl.UpdateView(DataMgr._data);
            //弹出获取材料界面
            decGainCtrl.Show();
        }


        private void AnnotationBtn_UIButtonClickHandler()
        {
            ProxyTips.OpenTextTips(10, new UnityEngine.Vector3(135, 156));
        }
    }
}