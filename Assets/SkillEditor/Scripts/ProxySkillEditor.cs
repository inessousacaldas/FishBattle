
#if ENABLE_SKILLEDITOR

using System.Linq;
using System.Reflection;


namespace SkillEditor
{
    public static class ProxySkillEditor
    {
        // 做一些进入战斗场景前的准备
        public static void EnterSkillEditor()
        {
            ServerManager.Instance.SetServerInfo(GameServerInfoManager.GetRecommendServerList(false).First());
            DataManager.Instance.UpdateStaticData(null, () =>
            {
                JoystickModule.Instance.Setup();
                LayerManager.Instance.SwitchLayerMode(UIMode.BATTLE);
                BattleActionPlayerPoolManager.Instance.Setup(SkillEditorModuleManager.Instance.RooTransform.gameObject);

                SkillEditorController.Instance.EnterBattle();
            }, null, null);
        }
    }
}

#endif