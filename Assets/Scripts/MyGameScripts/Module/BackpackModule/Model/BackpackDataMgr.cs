using System;
using System.Collections.Generic;
using UniRx;
using AppDto;

public sealed partial class BackpackDataMgr
{

    public static Subject<IEnumerable<int>> gainItemStream = null;
    public static UniRx.IObservable<IEnumerable<int>> GainItemStream
    {
        get { return gainItemStream ?? (gainItemStream = new Subject<IEnumerable<int>>()); }
    }

    // 初始化
    private void LateInit()
    {
        if (gainItemStream == null)
            gainItemStream = new Subject<IEnumerable<int>>();
        _data = new BackpackData();
        _data.InitData();
        _disposable = new CompositeDisposable();
        _disposable.Add(NotifyListenerRegister.RegistListener<BagDto>(HandleBagDtoNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<BagItemNotify>(HandleBagItemDtoNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<BagCapabillityNotify>(HandleBagCapabillityNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<CrewChipNotify>(HandleCrewChipNotify));
        // warehouse\
        _disposable.Add(NotifyListenerRegister.RegistListener<WarehouseDto>(HandleWarehouseDtoNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<MissionItemNotify>(HandleMissionDtoDtoNotify));
        
    }

    private void OnDispose(){
        gainItemStream.Dispose();
        gainItemStream = null;
    }

    public void InitBagAndTemp(){
        BackPackNetMsg.ReqItemsInBagAndTemp();
    }

    /// <summary>
    /// 如果满，返回true ,否则返回False
    /// </summary>
    /// <returns></returns>
    public bool IsBagFull()
    {
        var remain_0 = _data.backPackDto.capability - _data.BagItemsCnt;
        return (remain_0 ) <= 0 ;
    }
    #region HandleNetMsg
    private void HandleBagDtoNotify(BagDto dto)
    {
        var success = _data.UpdateBagDto(dto);
        if (success)
            FireData();
    }

    private void HandleBagItemDtoNotify(BagItemNotify noti){

        if (noti == null)
        {
            return;
        }
        GameLog.Log_BAG("BagItemNotify bagid = "+((AppItem.BagEnum)noti.bagId).ToString());

        _data.UpdateAddedItems(noti);

        var _BagDto = _data._dtoDic[(AppItem.BagEnum)noti.bagId];
        if (_BagDto == null)
        {
            GameLog.Log_BAG("Error :bagdto is null, BagItemNotify bagid = "+((AppItem.BagEnum)noti.bagId).ToString());
            return;
        }
        //暂时屏蔽 改由服务器"设法"推送 －－－fish
        //Func<IEnumerable<BagItemDto>, IEnumerable<int>> fun = delegate (IEnumerable<BagItemDto> src)
        //{
        //    var gainSet = new List<int>();
        //    noti.updateItems.ForEach(i =>
        //    {
        //        var temp = src.Find(item =>
        //        {
        //            if (i.uniqueId <= 0)
        //                return i.itemId == item.itemId && i.index == item.index;
        //            return i.uniqueId == item.uniqueId;
        //        });
        //        if (temp == null || temp.count < i.count)
        //            gainSet.Add(i.itemId);
        //    });
        //    return gainSet;
        //};
        //gainItemStream.OnNext(fun(_BagDto.items)); // 推获得新物品数据

        noti.updateItems.ForEach<BagItemDto>(item =>
            _BagDto.items.ReplaceOrAdd(x => x.index == item.index, item)
        );
        noti.deleteItems.ForEach<int>(idx => _BagDto.items.Remove(s=>s.index == idx));

        //消耗物品等，取消当前选中
        var CurSelectBagItemDto = DataMgr._data.GetBagItemByIndex(DataMgr._data.CurBackPackSelectIdx, DataMgr._data.CurBagTab);
        if (CurSelectBagItemDto == null)
        {
            DataMgr._data.CurBackPackSelectIdx = -1;
        }

        var CurSelectWareHouseItemDto = DataMgr._data.GetWarehouseItemsByIndex(DataMgr._data.SelectWarehouseItemIdx);
        if(CurSelectWareHouseItemDto ==null)
        {
            DataMgr._data.SelectWarehouseItemIdx = -1;
        }


        FireData();
        GameEventCenter.SendEvent(GameEvent.BACK_PACK_ITEM_CHANGE);
    }

    private void HandleBagCapabillityNotify(BagCapabillityNotify noti)
    {
        if (noti == null) return;
        var _BagDto = _data._dtoDic[(AppItem.BagEnum)noti.bagId];
        _BagDto.capability = noti.newCapability;
        FireData();
    }

    private void HandleCrewChipNotify(CrewChipNotify notify)
    {
        if (notify == null) return;

        DataMgr._data.UpdateCrewChips(notify.crewChipItems);
    }

    private void HandleWarehouseDtoNotify(WarehouseDto dto)
    {
        GameLog.Log_BAG("HandleWarehouseDtoNotify-----");
        if (dto == null || dto.bagDto == null)
            return;
        var success = _data.UpdateBagDto(dto.bagDto);

        if (!success) return;
        _data.UpdateWarehouseNameSet(dto.warehousePageDtos);
        stream.OnNext(_data);
    }

    private void HandleMissionDtoDtoNotify(MissionItemNotify notify)
    {
        if(notify == null || notify.items == null)
            return;
        DataMgr._data.UpdateMissionItem(notify.items);
    }

    #endregion
    public int GetItemCountByItemID(int itemID){
        var cnt = 0;
        _data.GetAllItemInBagAndTemp().ForEach(dto=>{
            if (dto != null && dto.itemId == itemID)
                cnt += dto.count;
        });

        return cnt;
    }

    //获取背包的物品数量(不包括临时背包)
    public int GetBackpackItemCountByItemID(int itemID)
    {
        var cnt = 0;
        _data.backPackDto.items.ForEach(dto =>
        {
            if (dto != null && dto.itemId == itemID)
                cnt += dto.count;
        });

        return cnt;
    }

    public BagItemDto GetItemByItemID(int itemId)
    {
        var item = _data.GetAllItemInBagAndTemp().Find(d => d.itemId == itemId);
        return item;
    }

    public int CurWareHousePage { get { return _data.CurWareHousePage;} }

    //收到分解所得物品
    public void RecieveDecGainItem(GeneralResponse e)
    {
        var dataList = e as DataList;
        GameDebuger.Log("分解长度====" + dataList.items.Count);
        var gainDtoList = new List<ResolveGainDto>();

        dataList.items.ForEach(dto =>
        {
            gainDtoList.Add(dto as ResolveGainDto);
        });

        _data.GainDtoList = gainDtoList;
    }

    public BackpackViewTab GetBackpackViewTab() { return _data.CurTab; }

    public IEnumerable<BagItemDto> GetBagItems()
    {
        return _data.GetBagItems();
    }

    public IEnumerable<BagItemDto> GetTradeItems()
    {
        return _data.GetTradeItems();
    }

    public IEnumerable<BagItemDto> GetStallDtos()
    {
        return _data.GetStallDtos();
    } 

    public IEnumerable<BagItemDto> GetBagItems(AppItem.ItemTypeEnum type)
    {
        return _data.backPackDto.items.Filter(s => s.item.itemType == (int)type);
    }
    public IEnumerable<BagItemDto> GetQuartzItems()
    {
        return _data.GetQuartzItems();
    }

    public IEnumerable<BagItemDto> GetEquipmentItems()
    {
        return GetBagItems(AppItem.ItemTypeEnum.Equipment);
    }

    public IEnumerable<BagItemDto> GetMedallionItems()
    {
        if (_data.backPackDto == null || _data.backPackDto.items.Count == 0)
            return null;

        var list = _data.backPackDto.items.Filter(s => s != null && s.extra != null && s.item != null && (s.item as RealItem).itemType == (int)AppItem.ItemTypeEnum.Medallion);
        return MedallionSort(list.ToList());
    }

    public IEnumerable<BagItemDto> GetRuneItems()
    {
        if (_data.backPackDto == null || _data.backPackDto.items.Count == 0)
            return null;

        var list = _data.backPackDto.items.Filter(s => s != null && s.item != null && (s.item as RealItem).itemType == (int)AppItem.ItemTypeEnum.Engrave);
        return RuneSort(list.ToList());
    }

    private static Comparison<BagItemDto> _comparison_medallion = null;
    private static List<BagItemDto> MedallionSort(List<BagItemDto> list)
    {
        if (_comparison_medallion == null)
        {
            _comparison_medallion = (a, b) =>
            {
                var tempA = a.extra as MedallionDto;
                var tempB = b.extra as MedallionDto;
                return tempA.engraves.Count == tempB.engraves.Count ? tempA.itemId.CompareTo(tempB.itemId) : tempA.engraves.Count.CompareTo(tempB.engraves.Count);
            };
        }

        list.Sort(_comparison_medallion);
        return list;
    }

    public IEnumerable<BagItemDto> GetTreasureMapItems()
    {
        if (_data.backPackDto == null || _data.backPackDto.items.Count == 0)
            return null;

        return _data.backPackDto.items.Filter(s => s != null && s.item != null && (s.item as RealItem).itemType == (int)AppItem.ItemTypeEnum.TreasureMap
            && (s.extra as PropsExtraDto_17) != null);
    }

    private static Comparison<BagItemDto> _comparison_rune = null;
    private static List<BagItemDto> RuneSort(List<BagItemDto> list)
    {
        if (_comparison_rune == null)
        {
            _comparison_rune = (a, b) =>
            {
                return a.item.quality == b.item.quality ? a.itemId.CompareTo(b.itemId) : a.item.quality.CompareTo(b.item.quality);
            };
        }

        list.Sort(_comparison_rune);
        return list;
    }


    #region 获得战斗中可以使用的物品数据
    public IEnumerable<BagItemDto> GetBattleItems(
        GeneralCharactor.CharactorType charactorType = GeneralCharactor.CharactorType.Unknown
        , int battleType = 0)
    {
        return _data.backPackDto.items.Filter(item => (item.item is Props) && IsItemCanBeUsedInBattle(item.item as Props, charactorType, battleType));
    }
    
    private bool IsItemCanBeUsedInBattle(Props pProps, GeneralCharactor.CharactorType charactorType, int battleType)
    {
        var tIsItemCanBeUsed = false;
        var tScopeId = pProps.scopeId;
        switch (battleType)
        {
//                case PvpVideo.PvpTypeEnum_CSPK:
//                    tIsItemCanBeUsed = tScopeId == Props.ScopeEnum_Cross;
//                    break;
//                case PvpVideo.PvpTypeEnum_Arena:
//                    tIsItemCanBeUsed = tScopeId == Props.ScopeEnum_ArenaBattle;
//                    break;
            default:
                    tIsItemCanBeUsed = tScopeId == (int)Props.ScopeEnum.Battle;
                break;
        }
        return tIsItemCanBeUsed && (pProps.triggerType == 0 || pProps.triggerType == (int)charactorType);
    }
    #endregion
}