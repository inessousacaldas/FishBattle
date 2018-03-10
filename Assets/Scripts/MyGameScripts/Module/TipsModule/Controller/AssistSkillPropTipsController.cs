using AppDto;
using System;
using System.Collections.Generic;

public partial interface IAssistSkillPropTipsController
{
    void UpdateView(GeneralItem dto, string price, long time);
    void UpdateView(BagItemDto dto, string price, long time);
}

public class AssistSkillPropTipsController : BaseTipsController, IAssistSkillPropTipsController
{
    public override void UpdateView(GeneralItem dto, string price, long time)
    {
        if (dto == null || dto as AppItem == null || dto as Props == null)
        {
            Close();
            return;
        }

        SetAllView(dto);
    }

    public void UpdateView(BagItemDto dto, string price, long time)
    {
        if (dto == null || dto.item == null || dto.item as AppItem == null || dto.item as Props == null || dto.extra as PropsExtraDto == null)
        {
            Close();
            return;
        }

        SetAllView(dto.item, (dto.extra as PropsExtraDto).rarity);
    }

    private void SetAllView(GeneralItem dto, int quality = -1)
    {
        var appitem = dto as AppItem;
        quality = quality == -1 ? appitem.quality : quality;
        SetTitle(appitem.icon, quality, appitem.name, (appitem as Props).itemType, (appitem as Props).minGrade);
        SetLineView(false);
        //作用
        SetLabelView((appitem as Props).introduction.WrapColor(ColorConstantV3.Color_Blue));
        SetLineView();
        //具体作用
        SetLabelView(GetDetailStr(appitem, quality).WrapColor(ColorConstantV3.Color_Blue));
        SetLineView();

        //描述
        SetLabelView(appitem.description);
        //SetLabelView(time.ToString());
    }

    private string GetDetailStr(GeneralItem dto, int quality)
    {
        var appitem = dto as AppItem;
        var propsDto = appitem as Props;
        string introductionStr = propsDto.introduction;
        var detailStr = string.Empty;

        detailStr += string.Format("美味度{0}，", quality);

        introductionStr = introductionStr.Substring(introductionStr.IndexOf("："));
        //获取物品作用字符串
        var effStrs = introductionStr.Split('；');
        effStrs.ForEachI((itemStr,index) =>
        {
            if (index > 0)
                detailStr += "；";
            var subIntroductionStr = string.Empty;
            if (itemStr.IndexOf("=") <= 0)
            {
                subIntroductionStr = itemStr.Replace("：", "");
                detailStr += " " + subIntroductionStr;
                return;
            }
            else
            {
                subIntroductionStr = itemStr.Substring(0, itemStr.IndexOf("="));
                subIntroductionStr = subIntroductionStr.Replace("：", "");
                detailStr += " " + subIntroductionStr;
            }
            
            var expressionStr = itemStr.Substring(itemStr.IndexOf("=") + 1);
            if (string.IsNullOrEmpty(expressionStr))
                return;
            var tempStr = string.Empty;//表达式后面的描述
            var isPercent = false;//是否已百分数显示
            if (expressionStr.IndexOf("，") > 0)
            {
                tempStr = expressionStr.Substring(expressionStr.IndexOf("，"));
                expressionStr = expressionStr.Substring(0, expressionStr.IndexOf("，"));
            }
            else if(expressionStr.IndexOf("（") > 0)
            {
                tempStr = expressionStr.Substring(expressionStr.IndexOf("（"));
                expressionStr = expressionStr.Substring(0, expressionStr.IndexOf("（"));
            }
            if (expressionStr.IndexOf("%") > 0)
                isPercent = true;
            expressionStr = expressionStr.Replace("%", "/100");
            var expression = ExpressionManager.AssistSkillProductProps("assistskill" + dto.id + index, expressionStr, quality.ToString());
            detailStr += isPercent ? string.Format("{0}%", expression * 100) : expression.ToString();
            detailStr += tempStr;
        });

        return detailStr;
    }
}