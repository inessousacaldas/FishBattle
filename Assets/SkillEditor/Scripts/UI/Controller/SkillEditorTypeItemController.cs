#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorTypeItemController: MonoTabItemController<SkillEditorTypeItem>
    {
        public void SetData(string text)
        {
            _view.Info_UILabel.text = text;
        }


        protected override void AfterInitView()
        {
            base.AfterInitView();

            EventDelegate.Set(_view.SkillEditorTypeItem_UIButton.onClick, OnClick);
        }

        public override void SetSelected(bool selected)
        {
            _view.Sprite_UISprite.gameObject.SetActive(selected);
        }
    }
}

#endif