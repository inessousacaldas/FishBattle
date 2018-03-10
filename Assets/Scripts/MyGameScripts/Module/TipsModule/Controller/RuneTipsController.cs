using AppDto;
using System;
using System.Collections.Generic;

public partial interface IRuneTipsController
{
    void UpdateView(GeneralItem dto, string price, long time);
}

public class RuneTipsController : BaseTipsController, IRuneTipsController
{
    public override void UpdateView(GeneralItem dto, string price, long time)
    {
        if (dto == null || dto as AppItem == null)
        {
            Close();
            return;
        }   

        var appitem = dto as AppItem;
        int lv = appitem as Props == null ? 0 : (appitem as Props).minGrade;
        SetTitle(appitem.icon, appitem.quality, appitem.name, (int)AppItem.ItemTypeEnum.Engrave,lv);
        SetLineView(false);
        //属性/圣能消耗
        SetLabelView(GetAttrStr(dto).WrapColor(ColorConstantV3.Color_Blue));
        SetLabelView(GetCapStr(dto));
        SetLineView();
        //描述
        SetLabelView(appitem.description);
        //SetLabelView(time.ToString());
    }

    private string GetAttrStr(GeneralItem dto)
    {
        var attrStr = string.Empty;
        if (dto as Props == null || ((dto as Props).propsParam) as PropsParam_3 == null)
            return "";

        var propsParam = ((dto as Props).propsParam) as PropsParam_3;
        if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY)
            attrStr = string.Format("{0}:{1}~{2}", DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name, propsParam.emin, propsParam.emax);
        else if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
            attrStr = string.Format("强化属性:{0}~{1}", propsParam.emin, propsParam.emax);

        return attrStr;
    }

    private string GetCapStr(GeneralItem dto)
    {
        var capStr = string.Empty;
        if (dto as Props == null || ((dto as Props).propsParam) as PropsParam_3 == null)
            return "";
        var propsParam = ((dto as Props).propsParam) as PropsParam_3;
        capStr = string.Format("消耗圣能:{0}~{1}", propsParam.omin, propsParam.omax);

        return capStr;
    }
}