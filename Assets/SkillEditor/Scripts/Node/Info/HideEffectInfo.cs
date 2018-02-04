
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupHideEffectInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(HideEffectInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"delayTime", CreateTypeField("生命时间")},
                });
        }
    }
}

#endif