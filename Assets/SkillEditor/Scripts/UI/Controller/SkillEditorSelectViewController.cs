
using System;
using System.Collections.Generic;
using System.Linq;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorSelectViewController<T>: SkillEditorBaseController<SkillEditorSelectView>
    {
        private TabContainerController<SkillEditorSelectItemController> _containerController = new TabContainerController<SkillEditorSelectItemController>();

        private Action<T> _closeAction;
        private Func<T, string> _getNameFunc;

        private List<T> _infoList;
        private List<T> _tempInfoList;
        private string _lastSearch;

        protected override void AfterInitView()
        {
            base.AfterInitView();

            _containerController.SetData(CreateItem, null, null, true, true);

            EventDelegate.Set(_view.ConfirmBtn_UIButton.onClick, ConfirmBtnOnClick);
            EventDelegate.Set(_view.Search_UIInput.onSubmit, OnSubmit);
        }

        protected override void OnDispose()
        {
            _containerController.Dispose();
            _infoList = null;

            base.OnDispose();
        }

        public void SetData(List<T> infoList, Action<T> closeAction, Func<T, string> getNameFunc, T info = default(T))
        {
            _infoList = infoList;
            _closeAction = closeAction;
            _getNameFunc = getNameFunc;

            RefreshList(info);
        }

        private SkillEditorSelectItemController CreateItem()
        {
            return SkillEditorHelper.CreateAndSetItemFast<SkillEditorSelectItemController>(
                SkillEditorSelectItem.NAME, _view.Grid_UIGrid.gameObject);
        }


        private void RefreshList(T info = default(T))
        {
            _lastSearch = _view.Search_UIInput.value;
            _tempInfoList = string.IsNullOrEmpty(_lastSearch)
                ? _infoList
                : _infoList.Where(infoItem => _getNameFunc(infoItem).Contains(_lastSearch)).ToList();
            _containerController.UpdateItemList(_tempInfoList.Count, (i, controller) => controller.SetData(_getNameFunc(_tempInfoList[i])));

            _view.Grid_UIGrid.Reposition();
            _view.ScrollView_UIScrollView.ResetPosition();

            if (info != null)
            {
                var index = _tempInfoList.IndexOf(info);
                if (index >= 0)
                {
                    JSTimer.Instance.SetupCoolDown("Drag", 0.1f, null, () =>
                    {
                        if (_view != null)
                        {
                            _containerController.SetSelected(index);
                            var delta = index / (_containerController.UseCount - 10f);
                            _view.ScrollView_UIScrollView.SetDragAmount(0, delta, false);
                        }
                    });
                }
            }
            else
            {
                _containerController.SetSelected(TabConst.NoneSelected);
            }
        }

        private void ConfirmBtnOnClick()
        {
            var closeAction = _closeAction;
            var curIndex = _containerController.CurrentSelectedIndex;
            var info = curIndex >= 0 ? _tempInfoList[curIndex] : default(T);
            _closeAction = null;

            ProxySkillEditorModule.CloseSelectView();

            closeAction(info);
        }

        private void OnSubmit()
        {
            if (_lastSearch != _view.Search_UIInput.value)
            {
                RefreshList();
            }
        }

    }
}

#endif