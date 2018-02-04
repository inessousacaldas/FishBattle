using System;
using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorNodeItemController : MonoTabItemController<SkillEditorNodeItem>
    {
        public void SetData(SkillEditorInfoNode node, int mainIndex, int subIndex)
        {
            _view.Info_UILabel.text = SkillEditorInfoCollection.GetNodeItemName(node, mainIndex, subIndex);
            var backFrame = SkillEditorConst.BackFrames.backWithFrame3;
            if (node.IsActionInfo())
            {
                if (node.IsAttack)
                {
                    backFrame = SkillEditorConst.BackFrames.backWithFrame1;
                }
                else
                {
                    backFrame = SkillEditorConst.BackFrames.backWithFrame2;
                }
            }
            _view.SkillEditorNodeItem_UIButton.normalSprite = backFrame.ToString();
        }


        protected override void AfterInitView()
        {
            base.AfterInitView();

            EventDelegate.Set(_view.SkillEditorNodeItem_UIButton.onClick, OnClick);
        }

        public override void SetSelected(bool selected)
        {
            _view.Sprite_UISprite.gameObject.SetActive(selected);
        }
    }
}

#endif