using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;


[InitializeOnLoad]
public static class GameDebugerConsole
{
    static GameDebugerConsole()
    {
        _showCustomLog = EditorPrefs.GetBool(ShowCustomLogKey, false);
        _handler = new GameDebugerLogHandler();
        _handler.SetIsCustomLogHandler(_showCustomLog);
        _assetErrorPause = EditorPrefs.GetBool(AssetErrorPause, false);
        assetPauseString = EditorPrefs.GetString(AssetErrorPauseString, string.Empty);
    }

    #region LogHandler
    private static GameDebugerLogHandler _handler;

    private class GameDebugerLogHandler : ILogHandler
    {
        private ILogHandler _defaultHandler = Debug.unityLogger.logHandler;

//        public GameDebugerLogHandler()
//        {
//            Debug.logger.logHandler = this;
//        }

        public void SetIsCustomLogHandler(bool customLog)
        {
            if (customLog)
            {
                Debug.unityLogger.logHandler = this;
            }
            else
            {
                Debug.unityLogger.logHandler = _defaultHandler;
            }
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            format = string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss:ffff"), format);

            _defaultHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            _defaultHandler.LogException(exception, context);
        }
    }
    #endregion

    private static int _recordScriptId;

    [OnOpenAsset(0)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        if (instanceID == _recordScriptId)
        {
            return false;
        }

        var stackTrace = GetConsoleStackTrace();
        if (!string.IsNullOrEmpty(stackTrace) && Regex.IsMatch(stackTrace, @"GameDebuger.cs:|GameDebugerConsole.cs:"))
        {
            var matches = Regex.Match(stackTrace, @"\(at (.+)\)");
            while (matches.Success)
            {
                var pathLine = matches.Groups[1].Value;
                if (!pathLine.Contains("GameDebuger"))
                {
                    //                    Debug.LogError(pathLine);

                    var msgs = pathLine.Split(':');
                    var script = AssetDatabase.LoadMainAssetAtPath(msgs[0]);
                    _recordScriptId = script.GetInstanceID();
                    return AssetDatabase.OpenAsset(_recordScriptId, Convert.ToInt32(msgs[1]));
                }
                matches = matches.NextMatch();
            }
        }

        return false;
    }


    private static string GetConsoleStackTrace()
    {
        var consoleType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        var consoleInstance = consoleType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

        if (consoleInstance != null && EditorWindow.focusedWindow == consoleInstance)
        {
            var textField = consoleType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            return textField.GetValue(consoleInstance).ToString();
        }

        return null;
    }

    #region preference
    private const string ShowCustomLogKey = "ShowCustomLogKey";
    private static bool _showCustomLog;
    private const string AssetErrorPause = "AssetErrorPause";
    private const string AssetErrorPauseString = "AssetErrorPauseString";
    private static bool _assetErrorPause;
    private static string assetPauseString;

    [PreferenceItem("Debuger")]
    private static void PreferenceOnGUI()
    {
        EditorGUILayout.Space();
        var curState = _showCustomLog;
        curState = EditorGUILayout.Toggle("使用自定义Log", curState);
        if (curState != _showCustomLog)
        {
            _showCustomLog = curState;
            EditorPrefs.SetBool(ShowCustomLogKey, _showCustomLog);
        }
        curState = _assetErrorPause;
        curState = EditorGUILayout.Toggle("发生资源加载错误时暂停", curState);
        if (curState != _assetErrorPause)
        {
            _assetErrorPause = curState;
            EditorPrefs.SetBool(AssetErrorPause, _assetErrorPause);
        }
        var curString = assetPauseString;
        if (_assetErrorPause)
        {
            curString = EditorGUILayout.TextArea(curString);
            if (curString != assetPauseString)
            {
                assetPauseString = curString;
                EditorPrefs.SetString(AssetErrorPauseString, assetPauseString);
            }
        }
    }
    #endregion
}
