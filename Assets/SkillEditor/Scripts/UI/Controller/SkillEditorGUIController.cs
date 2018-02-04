using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorGUIController: SkillEditorBaseController<SkillEditorGUI>
    {
        public Transform Root
        {
            get { return _view.Root_Transform; }
        }
    }
}

#endif