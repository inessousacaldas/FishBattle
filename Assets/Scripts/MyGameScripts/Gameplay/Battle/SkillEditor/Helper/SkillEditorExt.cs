using System;

namespace SkillEditor
{
    /// <summary>
    /// Skill editor ext.
    /// 部分方法加后缀以免冲突
    /// </summary>
    public static class SkillEditorExt
    {
        public static int ToIntInSkillEditor(this string pValue)
        {
            if (string.IsNullOrEmpty(pValue))
                return 0;
            int tIntValue = 0;
            int.TryParse(pValue, out tIntValue);
            return tIntValue;
        }

        public static float ToFloatInSkillEditor(this string pValue)
        {
            if (string.IsNullOrEmpty(pValue))
                return 0f;
            float tFloatValue = 0;
            float.TryParse(pValue, out tFloatValue);
            return tFloatValue;
        }
    }
}

