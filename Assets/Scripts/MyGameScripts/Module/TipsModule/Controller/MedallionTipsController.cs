using AppDto;
using System.Text;
using System.Collections.Generic;

public partial interface IMedallionTipsController
{
    void UpdateView(BagItemDto dto, string price, long time);
}

public class MedallionTipsController : BaseTipsController, IMedallionTipsController
{
    private string _capStr = "圣能：";
    private List<string> _runeList = new List<string>();
    private List<int> _runeQuaList = new List<int>();

    public void UpdateView(BagItemDto dto, string price, long time)
    {
        if (dto == null || dto.item==null || dto.item as MedallionProps==null)
        {
            Close();
            return;
        }  

        var appDto = dto.item;
        MedallionProps appItem = dto.item as MedallionProps;
        int lv = appItem.minGrade;
        SetTitle(appDto.icon, appDto.quality, appDto.name, (int)AppItem.ItemTypeEnum.Medallion, lv);
        SetLineView(false);
        //属性
        SetLabelView(GetAttrStr(dto).WrapColor(ColorConstantV3.Color_Blue));
        SetLineView();
        //圣能
        SetLabelView(_capStr);
        //铭刻符
        SetLabAndSprView("印记：", _runeList, _runeQuaList);
        SetLineView();
        //描述
        SetLabelView(appDto.description);
        //SetLabelView(time.ToString());
    }

    private string GetAttrStr(BagItemDto dto)
    {
        var attrStr = new StringBuilder();

        MedallionDto itemDto = dto.extra as MedallionDto;
        MedallionProps appItem = dto.item as MedallionProps;
        if (itemDto == null || appItem == null)
            return "";

        int usedCap = 0;
        float propsTimes = 1.0f;
        Dictionary<int, float> idToEffect = new Dictionary<int, float>();

        itemDto.engraves.ForEach(ItemDto =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(ItemDto.itemId);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            //强化铭刻符提升属性总倍数
            if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
            {
                propsTimes *= ItemDto.effect;
            }
            else if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY) //相同属性叠加
            {
                if (idToEffect.ContainsKey(ItemDto.itemId))
                    idToEffect[ItemDto.itemId] = idToEffect[ItemDto.itemId] + ItemDto.effect;
                else
                    idToEffect.Add(ItemDto.itemId, ItemDto.effect);
            }

            //已使用圣能
            usedCap += ItemDto.occupation;

            //铭刻符信息
            _runeList.Add(localData.icon);
            if (localData as AppItem != null)
                _runeQuaList.Add((localData as AppItem).quality);
        });

        idToEffect.ForEachI((item, index) =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(item.Key);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            attrStr.Append(DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name);
            attrStr.Append("+");
            
            if(index == idToEffect.Count - 1)
                attrStr.Append(((int)(item.Value * propsTimes)).ToString());
            else
                attrStr.AppendLine(((int)(item.Value * propsTimes)).ToString());
        });

        _capStr += string.Format("{0}/{1}", usedCap, appItem.capacity);

        if (itemDto.engraves.IsNullOrEmpty())
            attrStr.Append("空纹章");

        return attrStr.ToString();
    }
}