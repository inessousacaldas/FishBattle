using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;

namespace SceneTrigger
{
    /// <summary>
    /// 编辑器数据序列化后的对象
    /// </summary>
    [System.Serializable]
    public class TriggerConfigItem
    {
        public int triggerID;
        public ConditionType conditionType;
        public string conditionParamJson;
        public TriggerType triggerType;
        public string triggerParamJson;
    }

    /// <summary>
    /// 触发条件
    /// </summary>
    public abstract class ConditionBase
    {
#if UNITY_EDITOR
        // 运行时没必要保留json的String，编辑器下方便Debug查看
        public readonly string paramJson;
#endif
        internal bool initFinish = false;
        // Condition的参数类，类名必须为Param，否则EditorGUI找不到对应类型
        public ConditionBase(string paramJson)
        {
#if UNITY_EDITOR
            this.paramJson = paramJson;
#endif
        }
        public virtual void Init() { initFinish = true; }
        public virtual bool Update() { return false; }
        public abstract void Dispose();
    }
    /// <summary>
    /// 触发行为
    /// </summary>
    public abstract class TriggerBase
    {
        public readonly string paramJson;
        internal bool active = false;
        internal bool initFinish = false;
        // Trigger的参数类，类名必须为Param，否则EditorGUI找不到对应类型
        public TriggerBase(string paramJson) { this.paramJson = paramJson; }
        public virtual void Init() { initFinish = true; }
        public virtual void Update() { }
        public virtual void OnActive() { }
        public virtual void OnDeactive() { }
        public virtual bool IsFinish() { return false; }
        public abstract void Dispose();
    }
    /// <summary>
    /// Condition类型映射表，为了避免反射
    /// </summary>
    public enum ConditionType
    {
        Circular,
        EnterScene,
    }
    /// <summary>
    /// Trigger类型映射表，为了避免反射
    /// </summary>
    public enum TriggerType
    {
        CameraHight,
        LockCamera,
        SceneLayer,
    }

    public static class SceneTriggerHelper
    {
        private delegate ConditionBase ConditionDlg(string paramJson);

        private delegate TriggerBase TriggerDlg(string paramJson);

        public static ConditionBase ConditionFactory(ConditionType conditionType, string paramJson)
        {
            return conditionDlgs[conditionType](paramJson);
        }

        public static TriggerBase TriggerFactory(TriggerType triggerType, string paramJson)
        {
            return triggerDlgs[triggerType](paramJson);
        }


        //新建类型此处添加工厂构造方法

        private static Dictionary<ConditionType, ConditionDlg> conditionDlgs = new Dictionary<ConditionType, ConditionDlg>
        {
            {ConditionType.Circular, paramJson => new CirleRegionCondition(paramJson) },
            { ConditionType.EnterScene, paramJson => new EnterSceneCondition(paramJson) }
        };

        private static Dictionary<TriggerType, TriggerDlg> triggerDlgs = new Dictionary<TriggerType, TriggerDlg>
        {
            {TriggerType.CameraHight, paramJson => new CameraHeightTrigger(paramJson) },
            {TriggerType.LockCamera, paramJson => new CameraLockTrigger(paramJson) },
            {TriggerType.SceneLayer, paramJson => new SceneLayerTrigger(paramJson) }
        };

    }

    /// <summary>
    /// 定义某个字段在Editor显示的字符
    /// </summary>
    public class Rename : System.Attribute
    {
        public readonly string name;

        public Rename(string name)
        {
            this.name = name;
        }
    }

    public class Slider : System.Attribute
    {
        public readonly float min;
        public readonly float max;

        public Slider(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public class HideInEditor : System.Attribute
    {
        public HideInEditor()
        {
            
        }
    }

    public class EnumMask : System.Attribute
    {
        public readonly Type type;
        public EnumMask(Type enumType)
        {
            if(enumType.IsEnum)
                type = enumType;
            else
            {
                throw new SystemException();
            }
        }
    }
}
