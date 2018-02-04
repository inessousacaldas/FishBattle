
using System;
using System.Collections.Generic;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static class ProxySkillEditorModule
    {
        public static void OpenMainView()
        {
            SkillEditorModuleManager.Instance.OpenFunModule<SkillEditorMainViewController>(SkillEditorMainView.NAME,
                ModuleLayer.Base, false);
        }

        public static void OpenSelectView<T>(List<T> infoList, Action<T> closeAction, Func<T, string> getNameFunc, T info = default(T))
        {
            var controller = SkillEditorModuleManager.Instance.OpenFunModule<SkillEditorSelectViewController<T>>(SkillEditorSelectView.NAME,
                ModuleLayer.Second);

            controller.SetData(infoList, closeAction, getNameFunc, info);
        }

        public static void CloseSelectView()
        {
            SkillEditorModuleManager.Instance.CloseModule(SkillEditorSelectView.NAME);
        }

        public static void OpenSaveAsView(Action<int, string> closeAction)
        {
            var controller = SkillEditorModuleManager.Instance.OpenFunModule<SkillEditorSaveAsViewController>(SkillEditorSaveAsView.NAME,
                ModuleLayer.First);

            controller.SetData(closeAction);
        }

        public static void CloseSaveAsView()
        {
            SkillEditorModuleManager.Instance.CloseModule(SkillEditorSaveAsView.NAME);
        }

        public static void OpenSkillEditorInfoTypeView(SkillEditorInfoNode info, Action<SkillEditorInfoNode> closeAction)
        {
            var controller = SkillEditorModuleManager.Instance.OpenFunModule<SkillEditorInfoTypeViewController>(SkillEditorInfoTypeView.NAME,
                ModuleLayer.First);

            controller.SetData(info, closeAction);
        }


        public static void CloseSkillEditorInfoTypeView()
        {
            SkillEditorModuleManager.Instance.CloseModule(SkillEditorInfoTypeView.NAME);
        }

        public static void OpenSkillEditorLineView(List<SkillEditorInfoNode> infoNodeList)
        {
            var controller = SkillEditorModuleManager.Instance.OpenFunModule<SkillEditorLineViewController>(SkillEditorLineView.NAME,
                ModuleLayer.First);

            controller.SetData(infoNodeList);
        }
    }
}

#endif