using System.Collections.Generic;
using AssetPipeline;
using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public enum ModuleLayer
    {
        Base = 10,
        First = 20,
        Second = 30,
    }

    public class SkillEditorModuleManager: SkillEditorInstance<SkillEditorModuleManager>
    {
        private SkillEditorGUIController _rootController;
        private Dictionary<string, IViewController> _moduleDict = new Dictionary<string, IViewController>();

        public Transform RooTransform
        {
            get { return _rootController.Root; }
        }

        protected override void Init()
        {
            base.Init();

            _rootController = SkillEditorHelper.CreateAndSetItemFast<SkillEditorGUIController>(SkillEditorGUI.NAME);
//            Object.DontDestroyOnLoad(_rootController.BaseView.gameObject);
        }

        public C OpenFunModule<C>(string name, ModuleLayer layer, bool bgClose = true, bool bgMaskClose = true) where C : IViewController, new()
        {
            CloseModule(name);
            var controller = SkillEditorHelper.CreateAndSetItemFast<C>(name, _rootController.Root.gameObject);
            _moduleDict[name] = controller;

            if (bgClose)
            {
                AddBgMask(name, controller.BaseView.gameObject, bgMaskClose);
            }
            controller.BaseView.gameObject.ResetPanelsDepth((int)layer);

            return controller;
        }

        private void AddBgMask(string moduleName, GameObject module, bool bgMaskClose)
        {
            var bgMask = NGUITools.AddChild(module, ResourcePoolManager.Instance.LoadUI("ModuleBgBoxCollider"));

            if (bgMaskClose)
            {
                var button = bgMask.GetMissingComponent<UIEventTrigger>();
                EventDelegate.Set(button.onClick, () =>
                {
                    CloseModule(moduleName);
                });
            }
            var uiWidget = bgMask.GetMissingComponent<UIWidget>();
            uiWidget.depth = -1;
            uiWidget.autoResizeBoxCollider = true;
            uiWidget.SetAnchor(module, -10, -10, 10, 10);
            NGUITools.AddWidgetCollider(bgMask);
        }


        public void CloseModule(string name)
        {
            IViewController controller = null;
            if (_moduleDict.TryGetValue(name, out controller))
            {
                var go = controller.BaseView.gameObject;
                controller.Dispose();
                Object.Destroy(go);
                _moduleDict.Remove(name);
            }
        }
    }
}
#endif