
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupBaseEffectInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(BaseEffectInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"playTime", CreateTypeField("播放时间")},
                });
        }
    }
}

#endif