// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BackpackViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using LITJson;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public sealed partial class BackpackDataMgr
{
    public partial class BackpackViewController    {
        public new BackpackView View { get { return _view; } }
        public static void Open(
            BackpackViewTab tab = BackpackViewTab.Backpack
            , ItemTypeTab itemTab = ItemTypeTab.Item)
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_2, true)) return;

            DataMgr._data.curTab = tab;
            DataMgr._data.CurBagTab = itemTab;
            UIModuleManager.Instance.OpenFunModule<BackpackViewController>(
                BackpackView.NAME
                , UILayerType.DefaultModule
                , true
                , false);    
        }

        private TabbtnManager tabMgr = null;

        private ItemsPageContainerController _backpack = null;
        private ItemsPageContainerController _wareHouse = null;
        private ModelDisplayController _modelDisplay = null;
        //private SpineDisplayController _spineDisplay = null;
        private Dictionary<Equipment.PartType, ItemCellController> _equipDic = new Dictionary<Equipment.PartType, ItemCellController>();

        private S3PopupListController wareHousePopupListCtrl;
        S3PopUpItemData OnChoiceItemData = new S3PopUpItemData() { bgSprite = "dropbox_btn_name1", fontSize = 18, rect = new Vector2(130,35) };
        S3PopUpItemData OnUnChoiceItemData = new S3PopUpItemData() { bgSprite = "dropbox_btn_name2", fontSize = 16, rect = new Vector2(124,31) };
	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
            //UpdateTabInfo();
            CreateTabItem();
            InitBackPackPageContainer();
            InitPlayerInfo();
            if(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_3))
                InitWareHouse();
        }

        protected override void InitData()
        {
            _disposable = new CompositeDisposable();
        }

        // 客户端自定义代码
	    protected override void RegistCustomEvent ()
        {
            var d = PlayerModel.Stream.SubscribeAndFire(View.UpdatePlayerInfo);
            _disposable.Add(d);
            _disposable.Add(EquipmentMainDataMgr.DataMgr.OnEquipmentWear.Subscribe(_ => {
                OnEquipmentCellChange();
            }));
            _disposable.Add(EquipmentMainDataMgr.DataMgr.OnEquipmentTakeOff.Subscribe(_ => {
                OnEquipmentCellChange();
            }));
        }

        protected override void OnDispose()
        {
            _backpack = null;
            tabMgr.Dispose();
            _modelDisplay.CleanUpModel();
            _disposable.Dispose();
        }

        private void InitPlayerInfo()
        {
            InitEquipment();
            IntiPlayerProperty();
            InitPlayerModel();
        }

        private void InitPlayerModel()
        {
            _modelDisplay = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
                            View.ModelAnchorGO
                        , ModelDisplayUIComponent.NAME);
            _modelDisplay.Init(330, 330);
            _modelDisplay.SetBoxCollider(260, 350);


            //_spineDisplay = AddChild<SpineDisplayController, SpineDisplayUIComponent>(
            //    View.ModelAnchorGO
            //, SpineDisplayUIComponent.NAME);
            //_spineDisplay.Init(330, 330);
            //_spineDisplay.SetBoxCollider(260, 350);

        }

        private void IntiPlayerProperty()
        {
            View.SetPlayerRanking(999);
        }

        private void InitEquipment()
        {
            AddEquipmentCell(Equipment.PartType.Glove, View.LeftEquipGrid.gameObject);
            AddEquipmentCell(Equipment.PartType.Clothes, View.LeftEquipGrid.gameObject);
            AddEquipmentCell(Equipment.PartType.Weapon, View.LeftEquipGrid.gameObject);
            
            View.LeftEquipGrid.Reposition();
            AddEquipmentCell(Equipment.PartType.AccOne, View.RightEquipGrid.gameObject);
            AddEquipmentCell(Equipment.PartType.AccTwo, View.RightEquipGrid.gameObject);
            AddEquipmentCell(Equipment.PartType.Shoe, View.RightEquipGrid.gameObject);
            View.RightEquipGrid.Reposition();

            OnEquipmentCellChange();
        }
        
        private void AddEquipmentCell(Equipment.PartType partType, GameObject parent)
        {
            var cell = AddChild<ItemCellController, ItemCell>(
                parent
            , ItemCell.Prefab_BagItemCell
            , "Equip_" + partType.ToString());

            cell.SetShowTips(true);
            cell.SetTipsPosition(new Vector3(-315, 168, 0.0f));
            //暂时注释 装备tips完成后可删 todo xjd
            //cell.UpdateEquipView(null);
            //cell.OnCellClick.Subscribe(_=> {
            //    OnClickEquipmentCell(partType);
            //});
            _equipDic[partType] = cell;
        }
        private void OnEquipmentCellChange()
        {
            var equipDatas = DataMgr._data.equipmentCellsData;
            _equipDic.ForEach(x => {
                var equip = equipDatas.Find(g => g.partType == (int)x.Key);
                x.Value.UpdateEquipView(equip);
            });
        }
        private void OnClickEquipmentCell(Equipment.PartType part)
        {
            var equipmentdto = DataMgr._data.equipmentCellsData.Find(x => x.partType == (int)part);
            if (equipmentdto != null)
            {
                Vector3 tipsShowPos = new Vector3(-315, 168, 0.0f);
                //var curEquipmentDto = EquipmentMainDataMgr.DataMgr.GetSameEquipmentByPart(equipmentdto);
                var _tipsCtrl = ProxyTips.OpenEquipmentTips_FromBag(equipmentdto,isShowCompare:false);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
            }
            
        }
        private void InitBackPackPageContainer()
        {
            _backpack = AddChild<ItemsPageContainerController, ItemsPageContainer>(
                View.ItemsPosAnchor
                , ItemsPageContainer.NAME
                , "backpack");

            //_backpack.UpdateTabBtns(BackpackData.ItemTabNameSet,  DataMgr._data.ItemTabIndex);
            //点击背包上面的~按钮
            _disposable.Add(_backpack.OnTabbtnClick.Subscribe(i=>{
                if ((ItemTypeTab)BackpackData.ItemTabNameSet[i].EnumValue != DataMgr._data.CurBagTab)
                {
                    DataMgr._data.CurBagTab = (ItemTypeTab)BackpackData.ItemTabNameSet[i].EnumValue;
                    DataMgr._data.CurBackPackSelectIdx = -1;
                    UpdateBackPackItems(DataMgr._data);
                }
            }));

            _disposable.Add(
                _backpack.PageMoveAsObserver.Subscribe(
                    page=>DataMgr._data.CurPageNum = page));

            _disposable.Add(_backpack.OnItemClick.Subscribe(OnBackpackItemClickHandler));
            _disposable.Add(_backpack.OnItemDoubleClick.Subscribe(
                idx =>
                {
                    if (DataMgr._data.curTab == BackpackViewTab.Backpack)
                        return;

                    SaveItem(idx);
                }
            ));
        }


        private void OnBackpackItemClickHandler(int index)
        {
           
            
            var _BagDto = DataMgr._data.backPackDto;
            if (DataMgr._data.CurBagTab == ItemTypeTab.Item
                && _BagDto != null
                && index >= _BagDto.capability)
            {
                // 开锁
                BackPackNetMsg.ExpandBag();
            }
            else
            {
                var dto = DataMgr._data.GetBagItemByIndex(index,DataMgr._data.CurBagTab);
                if(dto != null)
                {
                    DataMgr._data.CurBackPackSelectIdx = index;
                    DataMgr._data.SelectWarehouseItemIdx = -1;
                    _backpack.SetSelect(DataMgr._data.CurBackPackSelectIdx);
                    if(_wareHouse != null)
                        _wareHouse.SetSelect(DataMgr._data.SelectWarehouseItemIdx);
                }
            }

        }

        private void InitWareHouse()
        {
            _wareHouse = AddChild<ItemsPageContainerController, ItemsPageContainer>(
                View.WareHousePosAnchor
                , ItemsPageContainer.NAME
                , "wareHouse");

            _wareHouse.ShowPageGroup(true);

            _disposable.Add(
                _wareHouse.PageMoveAsObserver.Subscribe(
                    page =>
                    {
                        DataMgr._data.CurWareHousePage = page;
                        FireData();
                    }));

            _disposable.Add(
                _wareHouse.OnItemClick.Subscribe(index =>
                    {
                        if (index >= DataMgr._data.wareHousePackDto.capability)
                        {
                            BackPackNetMsg.ReqExpandWarehouseCap();
                        }
                        else
                        {
                            var dto = DataMgr._data.GetWarehouseItemsByIndex(index);
                            if (dto != null)
                            {
                                DataMgr._data.SelectWarehouseItemIdx = index;
                                DataMgr._data.CurBackPackSelectIdx = -1;
                                _wareHouse.SetSelect(DataMgr._data.SelectWarehouseItemIdx);
                                _backpack.SetSelect(DataMgr._data.CurBackPackSelectIdx);
                            }
                        }
                    })
            );

            _disposable.Add(_wareHouse.OnItemDoubleClick.Subscribe(
                idx =>
                {
                    GetItem(idx);
                }
            ));

            //仓库选择栏
            wareHousePopupListCtrl = AddChild<S3PopupListController, S3PopupList>(View.WareHousePopupAnchor, S3PopupList.NAME);
            wareHousePopupListCtrl.InitData(S3PopupItem.PREFAB_WAREHOUSEIITEM, null, OnChoiceItemData, OnUnChoiceItemData);
            wareHousePopupListCtrl.OnChoiceIndexStream.Subscribe(idx => {
                if(idx.EnumValue != DataMgr._data.CurWareHousePage)
                {
                    DataMgr._data.CurWareHousePage = idx.EnumValue;
                    
                    FireData();
                }
            });
            
            
            BackPackNetMsg.ReqWarehouseDto(delegate
                {
                    UpdateWarehouseView(DataMgr._data.WareHouseViewData);
                });
        }
        private void UpdateTabInfo(){
            //if (FunctionOpenHelper.isFuncOpen((int) FunctionOpen.FunctionOpenEnum.FUN_3))
            //{
            //    BagTabSet.Add(TabInfoData.Create((int)BackpackViewTab.Warehouse, "仓库"));
            //}
            
        }
        private void CreateTabItem()
        {
            var BagTabSet = new List<ITabInfo>(2) { TabInfoData.Create((int)BackpackViewTab.Backpack, "包裹") };
            if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_3))
                BagTabSet.Insert((int)BackpackViewTab.Warehouse, TabInfoData.Create((int)BackpackViewTab.Warehouse, "仓库"));
            Func<int, ITabBtnController> func = i=>AddChild<TabBtnWidgetController, TabBtnWidget>(
                View.TabGridAnchor
                , TabbtnPrefabPath.TabBtnWidget.ToString()
                , "Tabbtn_" + i);

            tabMgr = TabbtnManager.Create(BagTabSet, func);

            _disposable.Add(tabMgr.Stream.Subscribe(e =>
            {                   
                DataMgr._data.curTab = (BackpackViewTab)e;
                DataMgr._data.CurBagTab = ItemTypeTab.Item;
                FireData();
            }));
        }

        private void UpdateView(IBackpackData _data)
        {
            if (_data == null) return;

            var data = _data.BackpackViewData;
            View.UpdateView(_data);
            _backpack.UpdateTabBtns(_data.BackpackViewData.CurBagTabInfos, DataMgr._data.CurBagTabIndex);
            switch (data.CurTab)
            {
                case BackpackViewTab.Backpack:
                {
                    UpdatePlayerInfo(data);
                    UpdatePlayerModel();
                }
                    break;
                case BackpackViewTab.Warehouse:
                {

                    UpdateWarehouseView(_data.WareHouseViewData);
                }
                    break;
            }
            UpdateBackPackItems(_data.BackpackViewData);
        }

        private void UpdateBackPackItems(IBackpackViewData data)
        {
            var isShowLock = false;
            IEnumerable<object> set = null;
            var len = 0;
            if (data.CurTab == BackpackViewTab.Warehouse)
            {
                set = ItemHelper.TransBagDtoToObjectSet(
                    data.GetBagItems()
                    , data.ItemBagCapability);
                len = data.ItemBagCapability;
                isShowLock = true;
            }
            else
            {
                switch (data.CurBagTab)
                {
                    case ItemTypeTab.Item:
                        set = ItemHelper.TransBagDtoToObjectSet(
                            data.GetBagItems()
                            , data.ItemBagCapability);
                        len = data.ItemBagCapability;
                        isShowLock = true;
                        break;    
                    case ItemTypeTab.Task:
                        set = data.GetMissionItem().Map<ItemDto, object>(s=> s as object);
                        len = 0;
                        break;
                    case ItemTypeTab.Pet:
                        set = data.GetCrewChips().Map(s => s as object); ;
                        len = 0;
                        break;    
                }
            }
            View.Composite_UIButton.gameObject.SetActive(data.isShowComposit);
            View.DecomposeBtn_UIButton.gameObject.SetActive(data.isShowDeComposit);
            View.ArrangeBtn_UIButton.gameObject.SetActive(data.isShowSortBtn);
            View.SaveItemBtn_UIButton.gameObject.SetActive(data.CurTab == BackpackViewTab.Warehouse);
            _backpack.UpdateView(
                data.CurPageNum
                , data.PageNum
                , len
                , set
                , isShowLock
                , DataMgr._data.CurBackPackSelectIdx
                , DataMgr._data.CurBagTabIndex
                );
            View.WealthGroup_UITable.Reposition();
            _backpack.SetSelect(data.CurBackPackSelectIdx);
        }

        private void UpdatePlayerInfo(IBackpackViewData data)
        {

        }

        private void UpdatePlayerModel()
        {
            _modelDisplay.SetupMainRoleModel();//, _mode == BackpackMode.FashionMode);//ModelManager.Player.TransformModelId
            _modelDisplay.CleanUpCustomAnimations ();

            //_spineDisplay.SetSpineID(312);
        }

        private void UpdateWarehouseView(IWareHouseViewData data)
        {   
            var set = ItemHelper.TransBagDtoToObjectSet(
                data.GetWarehouseItems()
                , data.WareHouseBagCapability);
            
            _wareHouse.UpdateView(
                data.CurWareHousePage
                , data.WareHousePageNum
                , data.WareHouseBagCapability
                , set
                , true
                , DataMgr._data.SelectWarehouseItemIdx
                );
            //设置当前选中的物品
            _wareHouse.SetSelect(data.SelectWarehouseItemIdx);
            var names = data.wareHouseNameStrSet.ToList();
            wareHousePopupListCtrl.UpdateView(names, 45,data.CurWareHousePage);
            wareHousePopupListCtrl.SetBgLabel(names[DataMgr._data.CurWareHousePage]);
        }

        private void CloseBtn_UIButtonClickHandler()
        {
            DataMgr._data.CurWareHousePage = 0;
            DataMgr._data.CurPageNum = 0;
            DataMgr._data.CurBackPackSelectIdx = -1;
            DataMgr._data.SelectWarehouseItemIdx = -1;
            DataMgr._data.AddedTemBagItems.Clear();
            UIModuleManager.Instance.CloseModule(BackpackView.NAME);
        }

        private void ArrangeBtn_UIButtonClickHandler()
        {
            DataMgr._data.CurBackPackSelectIdx = -1;
            int SortSpan = 5;
            var curTime = DateTime.Now;
            int remainSeconds = 0;

            if (DataMgr._data.lastSortBagTime != default(DateTime))
                remainSeconds = SortSpan - (int)(curTime - DataMgr._data.lastSortBagTime).TotalSeconds;
            //规定5秒间隔整理
            if (remainSeconds > 0)
            {
                TipManager.AddTopTip(string.Format("操作太过频繁，还剩{0}秒", remainSeconds));
                return;
            }
            BackPackNetMsg.SortBagItems();
        }
        private void DecomposeBtn_UIButtonClickHandler()
        {
            //TipManager.AddTip("敬请期待!");
            //return;
            var dto = DataMgr._data.backPackDto.items.Find(s => s.index == DataMgr._data.CurBackPackSelectIdx);
            ProxyBackpack.OpenComposite(CompositeTabType.DeComposite, dto);
        }

        private void Composite_UIButtonClickHandler()
        {
            //TipManager.AddTip("敬请期待!");
            //return;
            var dto = DataMgr._data.backPackDto.items.Find(s=>s.index == DataMgr._data.CurBackPackSelectIdx);
            ProxyBackpack.OpenComposite(CompositeTabType.Composite, dto);
        }

        private void CopperAddBtnSprite_UIButtonClickHandler()
        {
        }
        private void SiliverAddBtnSprite_UIButtonClickHandler()
        {
        }

        //取出按钮
        private void GetItemBtn_UIButtonClickHandler()
        {
            var idx = DataMgr._data.SelectWarehouseItemIdx;
            GetItem(idx);
        }
        private void GetItem(int idx)
        {
            var dto = DataMgr._data.GetWarehouseItemsByIndex(idx);
            if (dto == null)
                return;
            BackPackNetMsg.ReqMoveItemToBackPack(idx);
        }

        private void WareHouseSortBtn_UIButtonClickHandler()
        {
            int SortSpan = 5;
            var curTime = DateTime.Now;
            int remainSeconds = 0;

            if (DataMgr._data.lastSortWareHouseTime != default(DateTime))
                remainSeconds = SortSpan - (int)(curTime - DataMgr._data.lastSortWareHouseTime).TotalSeconds;
            //规定5秒间隔整理
            if (remainSeconds > 0)
            {
                TipManager.AddTopTip(string.Format("操作太过频繁，还剩{0}秒", remainSeconds));
                return;
            }
            if(!DataMgr._data.CheckWareHousePageIsOpen( DataMgr._data.CurWareHousePage))
            {
                TipManager.AddTopTip("该背包还未开放");
                return;
            }
            BackPackNetMsg.ReqSortWareHouse(DataMgr._data.CurWareHousePage);
        }

        private void EdittBtn_UIButtonClickHandler()
        {
            if(!DataMgr._data.CheckWareHousePageIsOpen(DataMgr._data.CurWareHousePage))
            {
                TipManager.AddTopTip("必须解锁该仓库，才能进行改名");
                return;
            }

            var inputValue = DataMgr._data._wareHouseNameStrSet.TryGetValue(DataMgr._data.CurWareHousePage);
            ProxyWindowModule.OpenInputWindow(
                3
                , 8
                , "仓库改名"
                , "请输入你要修改的仓库名字"
                , "请输入你要修改的仓库名字"
                , inputValue
                , str =>
                {
                    if(str.IndexOf(" ") >= 0)
                        TipManager.AddTip("输入的仓库名字中不能有空格");
                    else if(str == inputValue)
                        ProxyWindowModule.closeInputWin();
                    else if (!string.IsNullOrEmpty(str))
                        BackPackNetMsg.ReqModifyWarehouseName(str);
                    else
                        TipManager.AddTopTip("输入的仓库名字不能为空");
                }
                , null,type: WindowInputPrefabController.CHANGE_WARE_NAME);
        }

        private void SaveItemBtn_UIButtonClickHandler()
        {
            var idx = DataMgr._data.CurBackPackSelectIdx;
            SaveItem(idx);
        }
        private void SaveItem(int idx)
        {
            if (!DataMgr._data.CheckWareHousePageIsOpen(DataMgr._data.CurWareHousePage))
            {
                TipManager.AddTopTip("该背包还未开放");
                return;
            }
            var dto = DataMgr._data.GetBagItemByIndex(idx);
            if (dto == null)
                return;
            BackPackNetMsg.ReqMoveItemToWareHouse(
                idx
                , DataMgr._data.CurWareHousePage);
        }
    }
}
