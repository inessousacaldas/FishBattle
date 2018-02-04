
using System;
using System.Collections.Generic;
using AppDto;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        private enum SkillEffectType
        {
            hit,
            hit1,
            hit2,
            att,
            fly,
            full,
            follow,
        }

        private enum ActTargetType
        {
            //特效目标  0默认， 1，场景中心 2，我方中心   3， 敌军中心
            defaultVal,
            scene,
            player,
            enemy,
        }

        private enum MountType
        {
            Mount_Hit,
            Mount_HUD,
            Mount_Shadow,
        }


        [SetupBaseInfoMethod]
        private static void SetupNormalEffectInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(NormalEffectInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"name", CreateTypeField("特效类型")},
                    {"delayTime", CreateTypeField("生命时间")},
                    {"hitEff", CreateTypeField("是否受击", BoolList)},
                    {"target", CreateTypeField("作用目标", GetCacheEnumList<ActTargetType>())},
                    {"mount", CreateTypeField("作用锚点", GetCacheEnumList<MountType>())},
                    {"fixRotation", CreateTypeField("固定旋转", BoolList)},
                    {"faceToPrevious", CreateTypeField("指向", BoolList)},
                    {"faceToTarget", CreateTypeField("朝向目标", BoolList)},
                    {"IsEffectHasCamera", CreateTypeField("特效带镜头", BoolList)},
                    {"fly", CreateTypeField("是否飞行", BoolList)},
                    {"flyTarget", CreateTypeField("飞向目标")},
                    {"flyTime", CreateTypeField("飞行停顿时间", BoolList)},
                    {"follow", CreateTypeField("是否跟随", BoolList)},
                    {"offX", CreateTypeField("起点偏移X")},
                    {"offY", CreateTypeField("起点偏移Y")},
                    {"offZ", CreateTypeField("起点偏移Z")},
                });
        }
    }
}

#endif