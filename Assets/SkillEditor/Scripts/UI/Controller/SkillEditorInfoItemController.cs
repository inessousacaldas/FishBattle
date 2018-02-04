
using System;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public interface IInfoItemController: IViewController
    {
        object GetValue();
    }

    public class SkillEditorInfoItemController: SkillEditorBaseController<SkillEditorInfoItem>, IInfoItemController
    {
        // 如果是填字的话，这里只存第一个数值
        private object _value;
        private SkillEditorTypeFiled _typeField;

        private bool IsGetValue
        {
            get { return !_typeField.ValueList.IsNullOrEmpty(); }
        }

        public void SetData(object value, SkillEditorTypeFiled typeField)
        {
            _value = value;
            _typeField = typeField;

            _view.Title_UILabel.text = _typeField.Title;
            _view.SkillEditorInfoItem_UIButton.enabled = IsGetValue;
            _view.Value_BoxCollider.enabled = !IsGetValue;
            SetInputValue();
        }

        protected override void AfterInitView()
        {
            base.AfterInitView();

            EventDelegate.Set(_view.SkillEditorInfoItem_UIButton.onClick, OnClick);
        }

        protected override void OnDispose()
        {
            _value = null;
            _typeField = null;

            base.OnDispose();
        }

        private void OnClick()
        {
            SkillEditorInfoTypeNodeConfig.OpenSelectView(_value, _typeField, OnValueChanged);
        }

        private void SetInputValue()
        {
            _view.Value_UIInput.value = _typeField.GetTextFunc(_value);
        }

        private void OnValueChanged(object value)
        {
            _value = value;
            SetInputValue();
        }

        public virtual object GetValue()
        {
            if (IsGetValue)
            {
                return _value;
            }

            return _view.Value_UIInput.text;
        }

    }
}

#endif