using System;

#if ENABLE_SKILLEDITOR 
namespace SkillEditor
{
    public interface ITabItemController
    {
        void SetSelected(bool selected);
        void SetOnClickAction(int index, Action<int> onClickAction);
    }
}
#endif