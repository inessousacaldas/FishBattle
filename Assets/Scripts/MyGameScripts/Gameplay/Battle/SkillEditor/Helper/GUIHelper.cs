using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Debug = UnityEngine.Debug;
using System.Text;

namespace SkillEditor
{
    /// <summary>
    /// U3D 原生GUI 扩展
    /// @MarsZ 2017年04月27日15:30:25
    /// </summary>
    public static class GUIHelper
    {
        public const int DEFAULT_FONT_SIZE = 14;
        public const int DEFAULT_SCROLLVIEW_BTN_HEIGHT = 22;
        public const int DEFAULT_POPUP_BTN_HEIGHT = 26;
        public const int DEFAULT_FUNCTION_BTN_HEIGHT = 30;

        #region GUILayout Helper

        public static void DrawBox(string pBoxContent, int pBoxWidth = 0, int pHeight = DEFAULT_SCROLLVIEW_BTN_HEIGHT)
        {
            pBoxWidth = pBoxWidth <= 0 ? AppStringHelper.GetGBLength(pBoxContent) * 8 : pBoxWidth;
            GUILayout.Box(pBoxContent, GUILayout.Width(pBoxWidth), GUILayout.Height(pHeight));
        }

        static public bool DrawHeader(string text)
        {
            return DrawHeader(text, text, false, false);
        }

        static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = PlayerPrefsExt.GetBool(key, true);

            GUILayout.Space(3f);
            if (!forceOn && !state)
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            text = "<b>" + text + "</b>";
            if (state)
                text = "\u25BC " + text;
            else
                text = "\u25BA " + text;
            if (DrawButton(text, GUILayout.Width(200f), GUILayout.Height(DEFAULT_SCROLLVIEW_BTN_HEIGHT)))
                //        if (!GUILayout.Toggle(true, text,  GUILayout.Width(200f), DEFAULT_SCROLLVIEW_BTN_HEIGHT))
                state = !state;

            if (GUI.changed)
                PlayerPrefsExt.SetBool(key, state);

            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state)
                GUILayout.Space(3f);
            return state;
        }

        static public string DrawTextField(string title, string value, int width = 50, bool needLayout = true, int pTitleWidth = 0)
        {
            value = string.IsNullOrEmpty(value) ? string.Empty : value;

            if (needLayout)
                GUILayout.BeginHorizontal();

            DrawBox(title, pTitleWidth);
            //GUILayout.Label (title, GUILayout.MaxWidth(50));
            string newValue = GUILayout.TextField(value, GUILayout.Width(width), GUILayout.Height(DEFAULT_SCROLLVIEW_BTN_HEIGHT));

            if (needLayout)
                GUILayout.EndHorizontal();
            return newValue;
        }

        static public int DrawIntField(string title, int value, int width = 30, bool needLayout = true, int pTitleWidth = 0)
        {
            if (needLayout)
                GUILayout.BeginHorizontal();
    
            DrawBox(title, pTitleWidth);
            int newValue = GUILayout.TextField(value.ToString(), GUILayout.Width(width), GUILayout.Height(DEFAULT_SCROLLVIEW_BTN_HEIGHT)).ToIntInSkillEditor();
    
            if (needLayout)
                GUILayout.EndHorizontal();
            return newValue;
        }

        static public float DrawFloatField(string title, float value, int width = 30, bool needLayout = true, int pTitleWidth = 0)
        {
            if (needLayout)
                GUILayout.BeginHorizontal();

            DrawBox(title, pTitleWidth);
            float newValue = GUILayout.TextField(value.ToString(), GUILayout.Width(width), GUILayout.Height(DEFAULT_SCROLLVIEW_BTN_HEIGHT)).ToFloatInSkillEditor();

            if (needLayout)
                GUILayout.EndHorizontal();

            return newValue;
        }

        static public bool DrawToggle(string title, bool value, int width = 30, bool needLayout = true)
        {
            if (needLayout)
                GUILayout.BeginHorizontal();

            DrawBox(title);
            bool newValue = GUILayout.Toggle(value, string.Empty, GUILayout.MaxWidth(width), GUILayout.Height(DEFAULT_SCROLLVIEW_BTN_HEIGHT));

            if (needLayout)
                GUILayout.EndHorizontal();
            return newValue;
        }

        public static int EnumPopup(string pGUIDPrefix, string label, Enum selected, params GUILayoutOption[] options)
        {
            Type tEnumType = selected.GetType();
            string[] tEnumNames = Enum.GetNames(tEnumType);
            int tPreSelectedIndex = tEnumNames.FindElementIdx((pName) =>
                {
                    return pName == selected.ToString();
                });
            int mSelectetGridIndex = ShowButtonList(pGUIDPrefix, tPreSelectedIndex, tEnumNames, 125, "TextField");
            return mSelectetGridIndex;
        }

        public static bool DisplayDialog(string title, string message, string ok)
        {
            return DisplayDialog(title, message, ok, string.Empty);
        }

        public static bool DisplayDialog(string title, string message, string ok, string cancel)
        {
            #if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
            #else
            UnityEngine.Debug.LogError("DisplayDialog failed , run this in editor !");
            return false;
            #endif
        }

        #endregion

        public static void Space(int num)
        {
            for (int i = 0; i < num; i++)
            {
                GUILayout.Space(1);
            }
        }

        public static void LabelAndButton(string label, string button, Action buttonAction = null, GUIStyle style = null)
        {
            GUILayout.BeginHorizontal();
            {
                if (style != null)
                {
                    GUILayout.Label(label, style, GUILayout.Width(400));
                }
                else
                {
                    GUILayout.Label(label, GUILayout.Width(400));
                }

                if (DrawButton(button, GUILayout.Width(200)))
                {
                    if (buttonAction != null)
                        buttonAction();
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示数组
        /// </summary>
        /// <param name="labelArray"></param>
        public static void LabelArray(string[] labelArray, Color bColor, Color eColor)
        {
            if (labelArray == null)
                return;

            GUI.color = bColor;
            foreach (string str in labelArray)
            {
                GUILayout.Label(str, GUILayout.Width(400));
            }
            Space(1);
            GUI.color = eColor;
        }

        public static void LabelArray(string[] labelArray, Color bColor, Color eColor, GUIStyle style)
        {
            if (labelArray == null)
                return;

            GUI.color = bColor;
            foreach (string str in labelArray)
            {
                GUILayout.Label(str, style, GUILayout.Width(400));
            }
            Space(1);
            GUI.color = eColor;
        }

        public static int ShowButtonList(string pGUIDPrefix, int pPreSelectedIndex, string[] pNames, int pWidth, GUIStyle pScrollViewGUIStyle)
        {
            string tUID = pGUIDPrefix + GetButtonScrollViewUID(pNames);
            int tHeight = GetButtonScrollViewHeight(tUID);
            GUILayout.BeginScrollView(Vector2.zero, pScrollViewGUIStyle, GUILayout.Width(pWidth), GUILayout.Height(tHeight));
            {
                if (pNames != null)
                {
                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    if (tHeight > 30)//高度高，画所有按钮
                    {
                        for (int tCounter = 0, tLen = pNames.Length; tCounter < tLen; tCounter++)
                        {
                            if (DrawListButton(pNames[tCounter], pWidth, "TextField"))
                            {
                                SetButtonScrollViewHeight(tUID, DEFAULT_POPUP_BTN_HEIGHT);
                                return tCounter;
                            }
                        }
                    }
                    else//高度低，只画一个选中的按钮
                    {
                        if (DrawListButton(pNames[pPreSelectedIndex], pWidth, "TextField"))
                        {
                            SetButtonScrollViewHeight(tUID, DEFAULT_POPUP_BTN_HEIGHT * pNames.Length);
                            return pPreSelectedIndex;
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
            Space(1);
            return pPreSelectedIndex;
        }

        private static bool DrawListButton(string pText, int pWidth, GUIStyle pGUIStyle = null)
        {
            if (null == pGUIStyle)
                return DrawButton(pText, GUILayout.Width((int)(pWidth * 0.8)));
            else
                return DrawButton(pText, pGUIStyle, GUILayout.Width((int)(pWidth * 0.8)));
        }

        public static bool DrawButton(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, options);
        }

        public static bool DrawButton(string text, GUIStyle pGUIStyle, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, pGUIStyle, options);
        }

        private static Dictionary<string,int> mButtonScrollViewUIDHeightDic = new Dictionary<string, int>();

        private static string GetButtonScrollViewUID(string[] pNames)
        {
            if (null == pNames || pNames.Length <= 0)
                return string.Empty;
            StringBuilder tStringBuilder = new StringBuilder();
            for (int tCounter = 0, tLen = pNames.Length; tCounter < tLen; tCounter++)
            {
                tStringBuilder.Append(pNames[tCounter]);
            }
            return tStringBuilder.ToString();
        }

        private static int GetButtonScrollViewHeight(string pUID)
        {
            if (string.IsNullOrEmpty(pUID))
                return 0;
            int tHeight;
            if (mButtonScrollViewUIDHeightDic.TryGetValue(pUID, out tHeight))
                return tHeight;
            return DEFAULT_POPUP_BTN_HEIGHT;
        }

        private static bool SetButtonScrollViewHeight(string pUID, int pHeight)
        {
            if (string.IsNullOrEmpty(pUID))
                return false;
            if (mButtonScrollViewUIDHeightDic.ContainsKey(pUID))
                mButtonScrollViewUIDHeightDic[pUID] = pHeight;
            else
                mButtonScrollViewUIDHeightDic.Add(pUID, pHeight);
            return true;
        }
    }
}

