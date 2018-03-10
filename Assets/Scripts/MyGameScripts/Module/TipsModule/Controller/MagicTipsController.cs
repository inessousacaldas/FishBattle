using AppDto;
using System;
using System.Collections.Generic;

public partial interface IMagicTipsController
{
    void UpdateView(int id);
}

public class MagicTipsController : BaseTipsController, IMagicTipsController
{
    public void UpdateView(int id)
    {
        var magicDto = DataCache.getDtoByCls<Magic>(id);

        var ctrl = SetTitle(magicDto.icon, 0, magicDto.name, -1, magicDto.grade);
        ctrl.SetMagicIcon(magicDto.icon);
        SetLineView(false);
        //描述
        SetLabelView(magicDto.shortDescription);
    }
}