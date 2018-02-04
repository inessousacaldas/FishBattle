using System;
using System.Reflection;
using SceneTrigger;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneTriggerEditor
{
    internal static class EditorHelper
    {
        public static void MarkSceneDirty()
        {
            //改成Prefab后不需要Dirty场景
            //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        public static void DrawDefaultInspector(object obj, float guiSpace = 0)
        {
            Type type = obj.GetType();
            if(CheckTypeIsHide(type))
                return;
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                Property property = new Property(GetPropertyType(fieldInfo.FieldType), obj, fieldInfo);
                if(CheckFieldIsHide(property))
                    continue;
                DefaultPropertyField(property, GetFieldLabel(property), guiSpace);
            }
        }

        public static void DrawDefaultField(object obj, string field, float guiSpace = 0)
        {
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(field);
            GetPropertyType(fieldInfo.FieldType);
            Property property = new Property(GetPropertyType(fieldInfo.FieldType), obj, fieldInfo);
            DefaultPropertyField(property, GetFieldLabel(property), guiSpace);
        }

        private static void DrawArrayElementDefaultInspector(object obj, float guiSpace, int element)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(guiSpace);
            EditorGUILayout.LabelField("Element " + element);
            GUILayout.EndHorizontal();

            DrawDefaultInspector(obj, guiSpace + 15);
        }
        private static void DefaultPropertyField(Property property, string label, float space)
        {
            PropertyType propertyType = property.propertyType;

            switch (propertyType)
            {
                case PropertyType.Integer:
                    {
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        int longValue = EditorGUILayout.DelayedIntField(label, property.intValue);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.intValue = longValue;
                        }
                        break;
                    }
                case PropertyType.Boolean:
                    {
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        bool boolValue = EditorGUILayout.Toggle(label, property.boolValue);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.boolValue = boolValue;
                        }
                        break;
                    }
                case PropertyType.Float:
                    {
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        float floatValue;
                        var attributes = property.fieldInfo.GetCustomAttributes(typeof(Slider), false);
                        if (attributes.Length != 0)
                        {
                            Slider slider = (Slider)attributes[0];
                            floatValue = EditorGUILayout.Slider(label, property.floatValue, slider.min, slider.max);
                        }
                        else
                        {
                            floatValue = EditorGUILayout.DelayedFloatField(label, property.floatValue);
                        }
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.floatValue = floatValue;
                        }
                        break;
                    }
                case PropertyType.String:
                    {
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        string stringValue = EditorGUILayout.DelayedTextField(label, property.stringValue);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.stringValue = stringValue;
                        }
                        break;
                    }
                case PropertyType.Color:
                    {
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        Color colorValue = EditorGUILayout.ColorField(label, property.colorValue);
                        GUILayout.EndHorizontal();

                        if (EditorGUI.EndChangeCheck())
                        {
                            property.colorValue = colorValue;
                        }
                        break;
                    }
                case PropertyType.Enum:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Enum enumValue = EditorGUILayout.EnumPopup(label, property.enumValue);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.enumValue = enumValue;
                    }
                    break;
                case PropertyType.Vector2:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Vector2 vector2 = EditorGUILayout.Vector2Field(label, property.vector2);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector2 = vector2;
                    }
                    break;
                case PropertyType.Vector3:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Vector3 vector3 = EditorGUILayout.Vector3Field(label, property.vector3);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector3 = vector3;
                    }
                    break;
                case PropertyType.Vector4:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Vector4 vector4 = EditorGUILayout.Vector4Field(label, property.vector4);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector4 = vector4;
                    }
                    break;
                case PropertyType.Rect:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Rect rect = EditorGUILayout.RectField(label, property.rect);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.rect = rect;
                    }
                    break;
                case PropertyType.Bounds:
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    Bounds bounds = EditorGUILayout.BoundsField(label, property.bounds);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.bounds = bounds;
                    }
                    break;
                case PropertyType.ClassOrStruct:
                    if (property.objFiled == null)
                    {
                        property.objFiled = property.fieldInfo.FieldType.Assembly.CreateInstance(property.fieldInfo.FieldType.FullName);
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    EditorGUILayout.LabelField(label);
                    GUILayout.EndHorizontal();
                    DrawDefaultInspector(property.objFiled, space + 15);
                    break;
                case PropertyType.Array:
                    if (property.array == null)
                    {
                        property.array = Array.CreateInstance(property.fieldInfo.FieldType.GetElementType(), 0);
                    }
                    Array array = property.array;
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    int lenght = EditorGUILayout.DelayedIntField(label, array.Length);
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array.GetValue(i);
                        if (item == null)
                        {
                            item = property.fieldInfo.FieldType.Assembly.CreateInstance(property.fieldInfo.FieldType.GetElementType().FullName);
                            array.SetValue(item, i);
                        }
                        DrawArrayElementDefaultInspector(item, space + 15, i);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Array newArray = Array.CreateInstance(array.GetType().GetElementType(), lenght);
                        Array.Copy(array, newArray, Mathf.Min(array.Length, newArray.Length));
                        property.array = newArray;
                    }
                    break;
            }

        }
        private static string GetFieldLabel(Property property)
        {
            string label;
            var attributes = property.fieldInfo.GetCustomAttributes(typeof(Rename), false);
            if (attributes.Length != 0)
            {
                Rename rename = (Rename)attributes[0];
                label = rename.name + ":";
            }
            else
            {
                label = property.fieldInfo.Name + ":";
            }

            return label;
        }

        private static bool CheckTypeIsHide(Type type)
        {
            return type.GetCustomAttributes(typeof (HideInEditor), false).Length > 0;
        }
        private static bool CheckFieldIsHide(Property property)
        {
            return property.fieldInfo.GetCustomAttributes(typeof (HideInEditor), false).Length > 0;
        }
        private struct Property
        {
            internal readonly PropertyType propertyType;
            internal readonly object obj;
            internal readonly FieldInfo fieldInfo;

            internal Property(PropertyType propertyType, object obj, FieldInfo fieldInfo)
            {
                this.propertyType = propertyType;
                this.obj = obj;
                this.fieldInfo = fieldInfo;
            }

            public int intValue { get { return (int)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public bool boolValue { get { return (bool)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public float floatValue { get { return (float)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public string stringValue { get { return (string)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Color colorValue { get { return (Color)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Enum enumValue { get { return (Enum)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Vector2 vector2 { get { return (Vector2)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Vector3 vector3 { get { return (Vector3)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Vector4 vector4 { get { return (Vector4)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Rect rect { get { return (Rect)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Bounds bounds { get { return (Bounds)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public object objFiled { get { return fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }
            public Array array { get { return (Array)fieldInfo.GetValue(obj); } set { fieldInfo.SetValue(obj, value); } }


        }

        private static PropertyType GetPropertyType(Type type)
        {
            if (type.IsArray)
                return PropertyType.Array;
            if (type.IsEnum)
                return PropertyType.Enum;
            if (typeof(int) == type)
                return PropertyType.Integer;
            if (typeof(bool) == type)
                return PropertyType.Boolean;
            if (typeof(float) == type)
                return PropertyType.Float;
            if (typeof(string) == type)
                return PropertyType.String;
            if (typeof(Color) == type)
                return PropertyType.Color;
            if (typeof(Vector2) == type)
                return PropertyType.Vector2;
            if (typeof(Vector3) == type)
                return PropertyType.Vector3;
            if (typeof(Vector4) == type)
                return PropertyType.Vector4;
            if (typeof(Rect) == type)
                return PropertyType.Rect;
            if (typeof(Bounds) == type)
                return PropertyType.Bounds;
            if (type.IsClass)
                return PropertyType.ClassOrStruct;
            if (type.IsValueType)
                return PropertyType.ClassOrStruct;
            return PropertyType.None;
        }

        private enum PropertyType
        {
            None,
            Integer,
            Boolean,
            Float,
            String,
            Color,
            Enum,
            Vector2,
            Vector3,
            Vector4,
            Rect,
            Bounds,
            ClassOrStruct,
            Array
        }

    }
}
