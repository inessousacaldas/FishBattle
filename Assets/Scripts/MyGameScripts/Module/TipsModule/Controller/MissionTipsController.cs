using AppDto;
using System;
using System.Collections.Generic;

public partial interface IMissionTipsController
{
    void UpdateView(GeneralItem dto, string price, long time);
}

public class MissionTipsController : BaseTipsController, IMissionTipsController
{
    public void UpdateView(GeneralItem dto)
    {
        if (dto == null)
            return;

        SetTitle(dto.icon, 0, dto.name, (int)AppItem.ItemTypeEnum.MissionItem);
        SetLineView(false);
        //描述
        SetLabelView(dto.description);
        //SetLabelView(time.ToString());
    }
}