using AppDto;
using System;
using System.Collections.Generic;

public partial interface ISkillTipsController
{
    void UpdateView(int id);
}

public class SkillTipsController : BaseTipsController, ISkillTipsController
{
    public void UpdateView(int id)
    {
        var skillDto = DataCache.getDtoByCls<Skill>(id);

        var ctrl = SetTitle(skillDto.icon, 0, skillDto.name, -1);
        ctrl.SetSkillIcon(skillDto.icon);
        SetLineView(false);
        //描述
        SetLabelView(skillDto.shortDescription);
    }
}