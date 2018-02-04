using System;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public class SkillEditorSaveAsViewController: SkillEditorBaseController<SkillEditorSaveAsView>
    {
        private Action<int, string> _closeAction;

        public void SetData(Action<int, string> close)
        {
            _closeAction = close;
        }

        protected override void AfterInitView()
        {
            base.AfterInitView();

            EventDelegate.Set(_view.ConfirmBtn_UIButton.onClick, ConfirmBtnOnClick);
        }

        protected override void OnDispose()
        {
            _closeAction = null;

            base.OnDispose();
        }

        private void ConfirmBtnOnClick()
        {
            var id = _view.Id_UIInput.value;
            var name = _view.Name_UIInput.value;
            var closeAction = _closeAction;
            _closeAction = null;
            ProxySkillEditorModule.CloseSaveAsView();

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {
                closeAction(int.Parse(id), name);
            }
        }
    }
}
#endif