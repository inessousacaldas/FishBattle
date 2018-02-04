
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public class SkillEditorLineViewController: SkillEditorBaseController<SkillEditorLineView>
    {
        private List<SkillEditorInfoNode> _infoNodeList;
        private ContainerController<SkillEditotLineItemController> _nodeContainerController = new ContainerController<SkillEditotLineItemController>();

        public void SetData(List<SkillEditorInfoNode> infoNodeList)
        {
            _infoNodeList = infoNodeList;

            UpdateList();
        }

        protected override void AfterInitView()
        {
            base.AfterInitView();

            _nodeContainerController.SetData(CreateItem);
        }

        protected override void OnDispose()
        {
            _nodeContainerController.Dispose();

            base.OnDispose();
        }

        private SkillEditotLineItemController CreateItem()
        {
            return SkillEditorHelper.CreateAndSetItemFast<SkillEditotLineItemController>(SkillEditotLineItem.NAME,
                _view.Grid_UIGrid.gameObject);
        }

        private void UpdateList()
        {
            var mainIndex = 0;
            var subIndex = 0;
            _nodeContainerController.UpdateItemList(_infoNodeList.Count, (i, controller) =>
            {
                var node = _infoNodeList[i];
                if (node.IsActionInfo())
                {
                    mainIndex++;
                    subIndex = 0;
                    controller.SetData(node, mainIndex, subIndex);
                }
                else
                {
                    subIndex++;
                    controller.SetData(node, mainIndex, subIndex);
                }
            });

            _view.Grid_UIGrid.Reposition();
            _view.ScrollView_UIScrollView.ResetPosition();
        }
    }
}
#endif