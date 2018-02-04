using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public class SkillEditorInfoTypeNode
    {
        public string Name;

        public SkillEditorInfoTypeNode(string name)
        {
            Name = name;
        }
    }


    public class SkillEditorTypeFiled
    {
        public string Title;
        public Func<object, string> GetTextFunc;
        public List<object> ValueList;
    }
}
#endif