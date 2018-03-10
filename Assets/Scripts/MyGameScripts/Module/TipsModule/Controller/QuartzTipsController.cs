using AppDto;
using System;
using System.Collections.Generic;


public partial interface IQuartzTipsController
{
    void UpdateView(BagItemDto itemDto, string price, long time);
}

public class QuartzTipsController : BaseTipsController, IQuartzTipsController
{
    public void UpdateView(BagItemDto itemDto, string price, long time)
    {
        if(itemDto == null || itemDto.extra as QuartzExtraDto==null || itemDto.item==null || itemDto.item as Quartz==null)
        {
            Close();
            return;
        }  

        var quartzExtraDto = itemDto.extra as QuartzExtraDto;
        var appDto = itemDto.item;

        var ctrl = SetTitle((itemDto.item as Quartz).icon, appDto.quality, QuartzHelper.GetItemName(itemDto), (int)AppItem.ItemTypeEnum.Quartz, -appDto.quality);
        int quartzId = quartzExtraDto.baseProperties.Count > 0 ? quartzExtraDto.baseProperties[0].propId : quartzExtraDto.passiveSkill;
        var quartzIcon = DataCache.getDtoByCls<QuartzBaseProperty>(quartzId);
        if(quartzIcon != null)
            ctrl.SetQuartzIcon(quartzIcon.icon);
        SetLineView(false);
        //战斗力
        //SetLabelView(appDto.)
        //SetLineView();
        //主属性
        SetBaseProps(quartzExtraDto);
        //魔能属性
        SetQuartzProp(quartzExtraDto);
        SetLineView();
        //副属性
        SetSeconeProp(quartzExtraDto);
        //强化等级过高无法装备str
        SetIsOverGrade(quartzExtraDto);
        SetLineView();
        //描述
        SetLabelView(appDto.description);
        //SetLabelView(time.ToString());
    }  

    private void SetBaseProps(QuartzExtraDto dto)
    {
        if (dto.baseProperties.IsNullOrEmpty())
        {
            var skillArr = DataCache.getArrayByCls<Skill>();
            if (skillArr.Find(x => x.id == dto.passiveSkill) != null)
                SetLabelView(skillArr.Find(x => x.id == dto.passiveSkill).shortDescription);
        }
        else
            AddProperty(dto.baseProperties);
    }

    private void SetQuartzProp(QuartzExtraDto dto)
    {
        SetQuartzItemView(dto);
    }

    private void SetSeconeProp(QuartzExtraDto dto)
    {
        AddProperty(dto.secondProperties, ColorConstantV3.Color_Blue_Str);
    }

    private void SetIsOverGrade(QuartzExtraDto dto)
    {
        var str = "强化等级过高，无法装配".WrapColor(ColorConstantV3.Color_Red);
        var grade = DataCache.getDtoByCls<QuartzStrengGrade>(dto.strengGrade);
        if (grade == null)
        {
            GameDebuger.LogError(string.Format("QuartzStrengGrade{0},请检查", dto.strengGrade));
            return;
        }
        if (grade.playerGradeLimit > ModelManager.Player.GetPlayerLevel())
            SetLabelView(str);
    }
}