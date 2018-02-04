using AppDto;
using System;
using System.Collections.Generic;


public partial interface IVirtualItemTipsController
{
    void UpdateView(AppVirtualItem itemDto, string price, long time);
}

public class VirtualItemTipsController : BaseTipsController, IVirtualItemTipsController
{
    public void UpdateView(AppVirtualItem itemDto, string price, long time)
    {
        if (itemDto == null || itemDto as GeneralItem==null)
            return;

        var generalDto = itemDto as GeneralItem;
        SetTitle(generalDto.icon, itemDto.quality, generalDto.name, itemDto.itemType);
        SetLineView(false);
        //描述
        SetLabelView(generalDto.description);
        //SetLabelView(time.ToString());
    }         
}