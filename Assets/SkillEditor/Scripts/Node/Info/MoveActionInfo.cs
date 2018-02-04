
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        [SetupBaseInfoMethod]
        private static void SetupMoveActionInfo(Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> typeNodeDict)
        {
            typeNodeDict.Add(typeof(MoveActionInfo), new Dictionary<string, SkillEditorTypeFiled>()
                {
                    {"time", CreateTypeField("时长")},
                    {"distance", CreateTypeField("距离")},
                    {"center", CreateTypeField("是否中心", BoolList)},
                });
        }
    }
}

#endif