using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorInfoTypeViewController : SkillEditorBaseController<SkillEditorInfoTypeView>
    {
        private Action<SkillEditorInfoNode> _closeAction;
        private SkillEditorInfoNode _curInfo;
        private List<Type> _cacheTypeList;
        private TabContainerController<SkillEditorTypeItemController> _typeContainerController = new TabContainerController<SkillEditorTypeItemController>();

        private Dictionary<string, IInfoItemController> _infoFieldDitc = new Dictionary<string, IInfoItemController>();

        public void SetData(SkillEditorInfoNode info, Action<SkillEditorInfoNode> closeAction)
        {
            _closeAction = closeAction;
            _curInfo = info != null ? CreateInfoNode(info) : null;

            UpdateTypeList();
        }

        protected override void AfterInitView()
        {
            base.AfterInitView();

            _cacheTypeList = SkillEditorInfoTypeNodeConfig.ShowTypeNodeDict.Keys.ToList();
            _typeContainerController.SetData(CreateTypeItem, null, TypeItemOnSelected);

            EventDelegate.Set(_view.ConfirmBtn_UIButton.onClick, ConfirmBtnOnClick);
            EventDelegate.Set(_view.AttackToggle_UIToggle.onChange, TargetOnChange);
        }

        protected override void OnDispose()
        {
            _closeAction = null;
            _typeContainerController.Dispose();

            base.OnDispose();
        }

        private void ConfirmBtnOnClick()
        {
            SaveCurInfo();
            ClearInfoDict();

            var action = _closeAction;
            var info = _curInfo;

            ProxySkillEditorModule.CloseSkillEditorInfoTypeView();

            action(info);
        }

        private void TargetOnChange()
        {
            _curInfo.IsAttack = _view.AttackToggle_UIToggle.value;
        }

        #region 左边列表
        private SkillEditorTypeItemController CreateTypeItem()
        {
            return SkillEditorHelper.CreateAndSetItemFast<SkillEditorTypeItemController>(
                SkillEditorTypeItem.NAME, _view.TypeGrid_UIGrid.gameObject);
        }

        private void UpdateTypeList()
        {
            _typeContainerController.UpdateItemList(_cacheTypeList.Count, (i, controller) =>
            {
                controller.SetData(SkillEditorInfoTypeNodeConfig.ShowTypeNodeDict[_cacheTypeList[i]].Name);
            });

            if (_cacheTypeList.Count > 0)
            {
                var infoType = GetCurInfoType();
                if (infoType != null)
                {
                    var index = _cacheTypeList.FindIndex(type => type == infoType);
                    _typeContainerController.SetSelected(index);
                }
                else
                {
                    _typeContainerController.SetSelected(TabConst.DefaultSelected);
                }

                _view.TypeGrid_UIGrid.Reposition();
                _view.TypeScrollView_UIScrollView.ResetPosition();
            }
        }

        private void TypeItemOnSelected(int index, int lastIndex)
        {
            var curType = _cacheTypeList[index];
            if (lastIndex >= 0 || _curInfo == null)
            {
                _view.AttackToggle_UIToggle.value = true;
                _curInfo = CreateInfoNode(curType, _view.AttackToggle_UIToggle.value);
            }

            UpdateInfoView();
        }
        #endregion

        #region 右边列表
        private void UpdateInfoView()
        {
            // 公用部分
            _view.AttackToggle_UIToggle.value = _curInfo.IsAttack;

            ClearInfoDict();
            CreateFieldItem(GetCurInfoType(), _infoFieldDitc);

            JSTimer.Instance.SetupCoolDown("Reposition", 0.01f, null, () =>
            {
                if (_view != null)
                {
                    _view.InfoGrid_UITable.Reposition();
                    _view.InfoScrollView_UIScrollView.ResetPosition();
                }
            });
        }

        private void ClearInfoDict()
        {
            // 清掉列表
            foreach (var keyValue in _infoFieldDitc)
            {
                var go = keyValue.Value.BaseView.gameObject;
                keyValue.Value.Dispose();
                UnityEngine.Object.Destroy(go);
            }
            _infoFieldDitc.Clear();
        }


        private void CreateFieldItem(Type type, Dictionary<string, IInfoItemController> fieldDict)
        {
            var baseType = type.BaseType;
            if (baseType != typeof(object))
            {
                CreateFieldItem(baseType, fieldDict);
            }

            if (SkillEditorInfoTypeNodeConfig.TypeNodeDict.ContainsKey(type))
            {
                var skFieldDict = SkillEditorInfoTypeNodeConfig.TypeNodeDict[type];
                foreach (var keyValue in skFieldDict)
                {
                    var name = keyValue.Key;
                    var typeField = keyValue.Value;
                    var fieldInfo = type.GetField(name);

                    if (fieldInfo != null)
                    {
                        var item = SkillEditorHelper.CreateAndSetItemFast<SkillEditorInfoItemController>(
                            SkillEditorInfoItem.NAME, _view.InfoGrid_UITable.gameObject);
                        fieldDict[name] = item;

                        item.SetData(fieldInfo.GetValue(GetCurInfo()), typeField);
                    }
                    else
                    {
                        Debug.LogError(string.Format("类型 {0} 不存在字段 {1}", type, name));
                    }
                }
            }
        }

        private void SaveCurInfo()
        {
            var info = GetCurInfo();
            var type = GetCurInfoType();

            foreach (var keyValue in _infoFieldDitc)
            {
                var name = keyValue.Key;
                var item = keyValue.Value;
                var field = type.GetField(name);
                field.SetValue(info, Convert.ChangeType(item.GetValue(), field.FieldType));
            }
        }
        #endregion

        #region 辅助
        private Type GetCurInfoType()
        {
            if (_curInfo == null)
            {
                return null;
            }

            return _curInfo.IsActionInfo() ? _curInfo.ActionInfo.GetType() : _curInfo.EffectInfo.GetType();
        }

        private object GetCurInfo()
        {
            if (_curInfo.IsActionInfo())
            {
                return _curInfo.ActionInfo;
            }
            else
            {
                return _curInfo.EffectInfo;
            }
        }

        private SkillEditorInfoNode CreateInfoNode(Type type, bool isAttack)
        {
            SkillEditorInfoNode node;
            var ins = Activator.CreateInstance(type);
            if (ins is BaseActionInfo)
            {
                node = new SkillEditorInfoNode(ins as BaseActionInfo, null, isAttack);
            }
            else
            {
                node = new SkillEditorInfoNode(null, ins as BaseEffectInfo, isAttack);
            }
            return node;
        }

        private SkillEditorInfoNode CreateInfoNode(SkillEditorInfoNode infoNode)
        {
            return infoNode.DeepCopy();
        }
        #endregion
    }
}

#endif