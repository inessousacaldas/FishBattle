#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorSelectItemController: MonoTabItemController<SkillEditorSelectItem>
    {
        public void SetData(string text)
        {
            _view.Name_UILabel.text = text;
        }

        protected override void AfterInitView()
        {
            base.AfterInitView();

            EventDelegate.Set(_view.SkillEditorSkillItem_UIButton.onClick, OnClick);
        }

        public override void SetSelected(bool selected)
        {
            _view.Sprite_UISprite.gameObject.SetActive(selected);
        }
    }
}

#endif