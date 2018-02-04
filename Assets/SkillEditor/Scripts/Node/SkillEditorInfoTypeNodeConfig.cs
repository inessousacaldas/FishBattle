using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static partial class SkillEditorInfoTypeNodeConfig
    {
        #region 通用的列表
        private static readonly List<object> BoolList = new List<object>()
        {
            true,
            false,
        };

        // 记录一些 enum 的列表
        private static Dictionary<Type, List<object>> _cacheListDict = new Dictionary<Type, List<object>>();
        private static List<object> GetCacheEnumList<T>()
        {
            var type = typeof (T);
            List<object> list = null;
            if (!_cacheListDict.TryGetValue(type, out list))
            {
                list = new List<object>();
                var enums = Enum.GetValues(type);
                foreach (var e in enums)
                {
                    list.Add(e);
                }
                _cacheListDict[type] = list;
            }
            return list;
        }
        #endregion

        #region 支持的 Info
        public static readonly Dictionary<Type, SkillEditorInfoTypeNode> ShowTypeNodeDict = new Dictionary<Type, SkillEditorInfoTypeNode>()
        {
            {
                typeof(MoveActionInfo), new SkillEditorInfoTypeNode("攻击前移")
            },
            {
                typeof(NormalActionInfo), new SkillEditorInfoTypeNode("一般动作")
            },
            {
                typeof(MoveBackActionInfo), new SkillEditorInfoTypeNode("攻击后移")
            },
            {
                typeof(TakeDamageEffectInfo), new SkillEditorInfoTypeNode("显示受击")
            },
            {
                typeof(ShowInjureEffectInfo), new SkillEditorInfoTypeNode("显示伤害")
            },
            {
                typeof(NormalEffectInfo), new SkillEditorInfoTypeNode("一般特效")
            },
            {
                typeof(SoundEffectInfo), new SkillEditorInfoTypeNode("音效")
            },
            {
                typeof(ShakeEffectInfo), new SkillEditorInfoTypeNode("震动")
            },
            {
                typeof(HideEffectInfo), new SkillEditorInfoTypeNode("隐藏特效")
            },
        };
        #endregion


        #region 动态配置
        [AttributeUsage(AttributeTargets.Method)]
        private class SetupBaseInfoMethodAttribute : Attribute
        {

        };


        private static Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> _typeNodeDict;

        public static Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>> TypeNodeDict
        {
            get
            {
                if (_typeNodeDict == null)
                {
                    _typeNodeDict = new Dictionary<Type, Dictionary<string, SkillEditorTypeFiled>>();
                    var methods =
                        typeof(SkillEditorInfoTypeNodeConfig).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                            .Where(
                                info =>
                                    info.GetCustomAttributes(typeof(SetupBaseInfoMethodAttribute), false).Length > 0);
                    var objs = new object[] { _typeNodeDict };
                    foreach (var methodInfo in methods)
                    {
                        methodInfo.Invoke(null, objs);
                    }
                }
                return _typeNodeDict;
            }
        }
        #endregion


        #region 辅助
        private static SkillEditorTypeFiled CreateTypeField(string title)
        {
            return CreateTypeField(title, null);
        }

        private static SkillEditorTypeFiled CreateTypeField(string title, List<object> valueList)
        {
            return CreateTypeField(title, valueList, null);
        }


        private static SkillEditorTypeFiled CreateTypeField(string title, List<object> valueList, Func<object, string> getTextFunc)
        {
            getTextFunc = getTextFunc ?? DefaultGetName;

            return new SkillEditorTypeFiled()
            {
                Title = title,
                GetTextFunc = getTextFunc,
                ValueList = valueList,
            };
        }

        public static void OpenSelectView(object value, SkillEditorTypeFiled typeField, Action<object> closeAction)
        {
            ProxySkillEditorModule.OpenSelectView(typeField.ValueList, closeAction, typeField.GetTextFunc, value);
        }

        private static string DefaultGetName(object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            return string.Empty;
        }

        #endregion
    }
}

#endif