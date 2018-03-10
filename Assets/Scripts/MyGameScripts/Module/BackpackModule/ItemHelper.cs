using System;
using AppDto;
using System.Collections.Generic;
using QualityEnum = AppDto.AppItem.QualityEnum;
using Assets.Scripts.MyGameScripts.UI;

public static class ItemHelper
{
    public static GeneralItem GetGeneralItemByItemId(int itemId)
    {
        try
        {
            return DataCache.getDtoByCls<GeneralItem> (itemId);
        }
        catch(Exception e){
            GameDebuger.LogError(string.Format("get GeneralItem failed , e:{0}", e));
            return null;
        }
    }
    public static T GetGeneralItemByItemId<T>(int itemId) where T: GeneralItem
    {
        try
        {
            var data = DataCache.getDtoByCls<GeneralItem>(itemId);
            var res = data as T;
            return res;
        }
        catch (Exception e)
        {
            GameDebuger.LogError(string.Format("get GeneralItem failed , e:{0}", e));
            return null;
        }
    }

    // 按照bagdto的idx排序并用null填满空余位置
    public static IEnumerable<object> TransBagDtoToObjectSet(IEnumerable<BagItemDto> src, int length){
        var set = new List<object>(length);
        for (var i = 0; i < length; i++)
        {
            set.Add(null);
        }
        src.ForEach(dto=>set[dto.index] = dto);

        return set;
    }

    private static Dictionary<QualityEnum, string> ItemNameColor = new Dictionary<QualityEnum, string>(new GenericEnumComparer<QualityEnum>()){
        {QualityEnum.WHITE, "f0f0f0"}
        , {QualityEnum.GREEN, "7ee830"}
        , {QualityEnum.BLUE, "68c4ff"}
        , {QualityEnum.PURPLE, "ce7cfd"}
        , {QualityEnum.ORANGE, "f5904c"}
        , {QualityEnum.RED, "ff5050"}
    };
    /// <summary>
    /// 根据品质获取颜色
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string GetItemNameColorByRank(int quality)
    {
        string col = ItemNameColor[QualityEnum.WHITE];
        ItemNameColor.TryGetValue((QualityEnum) quality, out col);
        return string.IsNullOrEmpty(col) ? ItemNameColor[QualityEnum.WHITE] : col;
    }

    #region --------------------------------------------------------根据id获取信息---------------------------------------------------------

    /// <summary>
    /// 根据物品的Id获取品质~
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public static string GetItemNameColorByID(int itemID)
    {
        var props = DataCache.getDtoByCls<GeneralItem>(itemID) as AppItem;
        var rank = props == null ? 0 : props.quality;
        return GetItemNameColorByRank(rank);
    }
    
    /// <summary>
    /// 根据物品id获取icon
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public static string GetItemIcon(int itemID)
    {
        var props = DataCache.getDtoByCls<GeneralItem>(itemID) as Props;
        return props == null ? string.Empty : props.icon;
    }
    #endregion
    

    #region ------------------------------------根据id 获取物品名字，如有数量，也跟随品质颜色,后面的同学可以补充-------------------------
    /// <summary>
    /// 根据id获取名字。数量也跟随品质颜色，仅用于飘字提示，其余功能慎用
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string GetItemName(int itemID,int count = 0)
    {
        var item = GetGeneralItemByItemId(itemID);
        if (item == null) return "";
        if (item is AppItem)
        {
            var appItem = item as AppItem;
            return GetItemNameAppitem(appItem, count);
        }
        else if(item is AppVirtualItem)
        {
            var virtualItem = item as AppVirtualItem;
            return GetItemNameAppVirtual(virtualItem, count);
        }
        return "";
    }
    private static string GetItemNameAppVirtual(AppVirtualItem item,int count = 0)
    {
        string str = "";
        var rank = item == null ? 0 : item.quality;
        var name = item == null ? "" : item.name;
        str = HtmlUtil.Font2(name, GetItemNameColorByRank(rank));
        string c = count == 0 ? "" : count.ToString(); 
        str = str + c;
        return str;
    }
    private static string GetItemNameAppitem(AppItem item, int count = 0)
    {
        string str = "";
        var rank = item == null ? 0 : item.quality;
        var name = item == null ? "" : item.name;
        str = HtmlUtil.Font2(name, GetItemNameColorByRank(rank));
        string c = count == 0 ? "" : "*"+count;
        str = str + c;
        return str;
    }
    #endregion
}