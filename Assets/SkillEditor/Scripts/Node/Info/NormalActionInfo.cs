
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupNormalActionInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(NormalActionInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"startTime", CreateTypeField("延时")},
                    {"delayTime", CreateTypeField("生命时间")},
                });
        }
    }
}

#endif