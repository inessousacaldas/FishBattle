using System;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public abstract class MonoTabItemController<V> : SkillEditorBaseController<V>, ITabItemController where V: BaseView, new()
    {
        private int _index;
        private Action<int> _onClickAction;

        public abstract void SetSelected(bool selected);

        public void SetOnClickAction(int index, Action<int> onClickAction)
        {
            _index = index;
            _onClickAction = onClickAction;
        }

        protected void OnClick()
        {
            if (_onClickAction != null)
            {
                _onClickAction(_index);
            }
        }
    }
}
#endif