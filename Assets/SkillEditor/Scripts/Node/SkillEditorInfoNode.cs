#if ENABLE_SKILLEDITOR
namespace SkillEditor
{ 
    public class SkillEditorInfoNode
    {
        public BaseActionInfo ActionInfo;
        public BaseEffectInfo EffectInfo;
        // 这玩意信息会丢失
        public bool IsAttack;

        public SkillEditorInfoNode(BaseActionInfo actionInfo, BaseEffectInfo effectInfo, bool isAttack)
        {
            ActionInfo = actionInfo;
            EffectInfo = effectInfo;
            IsAttack = isAttack;
        }

        public bool IsActionInfo()
        {
            return EffectInfo == null;
        }

        public SkillEditorInfoNode DeepCopy()
        {
            var actionInfo = JsHelper.ToObject<JsonActionInfo>(JsHelper.ToJson(ActionInfo)).ToBaseActionInfo();
            var effectInfo = EffectInfo != null ? JsHelper.ToObject<JsonEffectInfo>(JsHelper.ToJson(EffectInfo)).ToBaseEffectInfo() : null;
            var node = new SkillEditorInfoNode(actionInfo, effectInfo, IsAttack);

            return node;
        }
    }
}

#endif