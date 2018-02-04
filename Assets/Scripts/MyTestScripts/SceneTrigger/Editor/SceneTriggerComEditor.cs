using System;
using LITJson;
using SceneTrigger;
using UnityEngine;
using UnityEditor;

namespace SceneTriggerEditor
{
    [CanEditMultipleObjects, CustomEditor(typeof(SceneTriggerCom))]
    public class SceneTriggerComInspector : Editor
    {
        public new SceneTriggerCom target { get { return base.target as SceneTriggerCom; } }

        public override void OnInspectorGUI()
        {
            if (target.configItem == null)
                target.configItem = new TriggerConfigItem();

            DrawConditionParam();
            DrawTriggerParam();
        }

        private void DrawConditionParam()
        {
            target.configItem.conditionType = (ConditionType)EditorGUILayout.EnumPopup("触发条件", target.configItem.conditionType);

            EditorGUI.BeginChangeCheck();
            var param = GetConditionParam();
            if (param != null)
                EditorHelper.DrawDefaultInspector(param);
            if (EditorGUI.EndChangeCheck())
            {
                target.configItem.conditionParamJson = JsonMapper.ToJson(param);
                EditorHelper.MarkSceneDirty();
            }
        }
        private void DrawTriggerParam()
        {
            target.configItem.triggerType = (TriggerType)EditorGUILayout.EnumPopup("触发行为", target.configItem.triggerType);

            EditorGUI.BeginChangeCheck();
            var param = GetTriggerParam();
            if (param != null)
                EditorHelper.DrawDefaultInspector(param);
            if (EditorGUI.EndChangeCheck())
            {
                target.configItem.triggerParamJson = JsonMapper.ToJson(param);
                EditorHelper.MarkSceneDirty();
            }
        }

        void OnSceneGUI()
        {
            if (target.configItem == null)
                target.configItem = new TriggerConfigItem();

            DrawCustomSceneGUI();
        }

        private void DrawCustomSceneGUI()
        {
            if (target.configItem.conditionType == ConditionType.Circular)
            {
                var param = GetConditionParam<CirleRegionCondition.Param>();

                //找出3个值中被改变的那个
                float scale;
                if (target.transform.localScale.x == target.transform.localScale.y)
                    scale = target.transform.localScale.z;
                else if (target.transform.localScale.x == target.transform.localScale.z)
                    scale = target.transform.localScale.y;
                else
                    scale = target.transform.localScale.x;

                target.transform.localScale = Vector3.one * scale;

                param.center = target.transform.position;
                param.radius = scale;
                string json = JsonMapper.ToJson(param);
                if (json != target.configItem.conditionParamJson)
                {
                    target.configItem.conditionParamJson = json;
                    EditorHelper.MarkSceneDirty();
                }
                Handles.DrawWireDisc(param.center, target.transform.up, param.radius);
            }
        }

        private T GetConditionParam<T>()
           where T : class, new()
        {
            T param;
            if (string.IsNullOrEmpty(target.configItem.conditionParamJson))
                param = new T();
            else
                param = JsonMapper.ToObject<T>(target.configItem.conditionParamJson);
            return param;
        }
        private T GetTriggerParam<T>()
           where T : class, new()
        {
            T param;
            if (string.IsNullOrEmpty(target.configItem.triggerParamJson))
                param = new T();
            else
                param = JsonMapper.ToObject<T>(target.configItem.triggerParamJson);
            return param;
        }

        private object GetConditionParam()
        {
            var type = SceneTriggerHelper.ConditionFactory(target.configItem.conditionType, string.Empty).GetType();
            return ParseParamByJson(target.configItem.conditionParamJson, type);
        }
        private object GetTriggerParam()
        {
            var type = SceneTriggerHelper.TriggerFactory(target.configItem.triggerType, string.Empty).GetType();
            return ParseParamByJson(target.configItem.triggerParamJson, type);
        }

        private static object ParseParamByJson(string json, Type type)
        {
            var paramType = type.GetNestedType("Param");
            if (paramType == null)
                return null;
            if (string.IsNullOrEmpty(json) || json == "null")
                return paramType.Assembly.CreateInstance(paramType.FullName);
            object obj = null;
            try
            {
                obj = JsonMapper.ToObject(json, paramType);

            }
            catch (Exception ex)
            {
                Debug.LogError("Json 反序列化失败");
                Debug.LogException(ex);
            }
            return obj;
        }
    }

}