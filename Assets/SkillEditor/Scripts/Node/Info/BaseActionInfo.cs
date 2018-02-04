
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupBaseActionInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(BaseActionInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"name", CreateTypeField("动作", GetCacheEnumList<ModelHelper.AnimType>()) },
                });
        }
    }
}

#endif