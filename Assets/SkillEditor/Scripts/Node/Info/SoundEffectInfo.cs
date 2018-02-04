
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupSoundEffectInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(SoundEffectInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"name", CreateTypeField("音效")},
                });
        }
    }
}

#endif