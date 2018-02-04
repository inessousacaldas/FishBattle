
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupBackActionInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(MoveBackActionInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"time", CreateTypeField("时长")},
                });
        }
    }
}

#endif