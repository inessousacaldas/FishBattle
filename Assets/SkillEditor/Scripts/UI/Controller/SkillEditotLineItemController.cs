using UnityEngine;
using System.Collections;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
public class SkillEditotLineItemController: SkillEditorBaseController<SkillEditotLineItem>
{
    private SkillEditorInfoNode _infoNode;
    private int _mainIndex;
    private int _subIndex;

    public void SetData(SkillEditorInfoNode infoNode, int mainIndex, int subIndex)
    {
        _infoNode = infoNode;
        _mainIndex = mainIndex;
        _subIndex = subIndex;

        UpdateUI();
    }

    protected override void AfterInitView()
    {
        base.AfterInitView();

        _view.Line_UIRangeBar.OnLowValueChanged += UpdateUI;
        _view.Line_UIRangeBar.OnHightValueChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        _view.Msg_UILabel.text = SkillEditorInfoCollection.GetNodeItemName(_infoNode, _mainIndex, _subIndex);
    }
}
}
#endif