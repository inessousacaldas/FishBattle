using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using Object = UnityEngine.Object;
using System.Text;

namespace Refactor
{
    public class GameEventRefactor : EditorWindow
    {
        private const int MAX_WIDTH = 800;
        private const int FIELD_WIDTH = 100;
        private const int BUTTON_HEIGHT = 50;


        private const string ModuleCodePath = "Assets/Scripts/MyGameScripts/Module";
        private const string MyGameScriptsPath = "Assets/Scripts/MyGameScripts";
        private const string GameEventPath = "/Scripts/MyGameScripts/GameEvent/GameEvent.cs";
        private const string EventMatch = @"public(\s)+((event)(\s)+)((System)(\s)*?\.)?(\s)*Action(\s)*?(\<(?<STR>[\w\s,\.]+)\>)?(\s)*?(?<STR2>[_a-zA-Z0-9]+)(\s)*?(.)*?;";
        private const string EventReplace = "\tpublic static readonly Event<{1}> {0} = new Event<{1}>(\"{0}\");";
        private const string EventReplace2 = "\tpublic static readonly Event {0} = new Event(\"{0}\");";

        //private const string AddListenerMatch = @"ModelManager(\s)*?\.(\s)*?(?<STR>[_a-zA-Z0-9]+)(\s)*?\.(\s)*?(?<STR2>[_a-zA-Z0-9]+)(\s)*?\+=(\s)*?(?<STR3>[_a-zA-Z0-9]+)(\s)*?;";
        private const string AddListenerMatch = @"(?<STR>[\w]+)((Model)|(Manager))(\s)*?\.(\s)*?Instance(\s)*\.(\s)*(?<STR2>[\w]+)(\s)*?\+=(\s)*?(?<STR3>[\w]+)(\s)*?;";
        private const string AddListenerReplace = @"GameEventCenter.AddListener(GameEvent.${STR}_${STR2}, ${STR3});";

        //private const string RemoveListenerMatch = @"ModelManager(\s)*?\.(\s)*?(?<STR>[_a-zA-Z0-9]+)(\s)*?\.(\s)*?(?<STR2>[_a-zA-Z0-9]+)(\s)*?\-=(\s)*?(?<STR3>[_a-zA-Z0-9]+)(\s)*?;";
        private const string RemoveListenerMatch = @"(?<STR>[\w]+)((Model)|(Manager))(\s)*?\.(\s)*?Instance(\s)*\.(\s)*(?<STR2>[\w]+)(\s)*?\-=(\s)*?(?<STR3>[\w]+)(\s)*?;";
        private const string RemoveListenerReplace = @"GameEventCenter.RemoveListener(GameEvent.${STR}_${STR2}, ${STR3});";

        private const string GameEventReplace = @"(#region 工具迁移事件)(?<STR>(.|\s)*?)(#endregion 工具迁移事件)";
        private const string SendEventMatch = @"if(\s)*?\((\s)*?(?<STR1>[_a-zA-Z0-9]+)(\s)*?!=(\s)*?null(\s)*?\)(\s)*(((\{){1}(\s)*(?<STR1>[_a-zA-Z0-9]+)(\s)*\((\s)*(?<STR2>.*?)\)(\s)*;(\s)*?(\}){1})|(\s)*(?<STR1>[_a-zA-Z0-9]+)(\s)*\((\s)*(?<STR2>.*?)\)(\s)*;(\s)*?)";
        private const string SendEventMatch2 = @"if(\s)*?\((\s)*?(\s)*?null(\s)*?!=(\s)*?(?<STR1>[_a-zA-Z0-9]+)\)(\s)*(((\{){1}(\s)*(?<STR1>[_a-zA-Z0-9]+)(\s)*\((\s)*(?<STR2>.*?)\)(\s)*;(\s)*?(\}){1})|(\s)*(?<STR1>[_a-zA-Z0-9]+)(\s)*\((\s)*(?<STR2>.*?)\)(\s)*;(\s)*?)";
        private const string SendEventReplace = @"GameEventCenter.SendEvent(GameEvent.{0}_${STR1}, ${STR2});";
        private const string SendEventReplace2 = @"GameEventCenter.SendEvent(GameEvent.{0}_${STR1});";

        private const string ModuleRemoveListenerMatch = @"(?<STR>[\w]+)(\s)*?=(\s)*?(null)\;";
        private const string ModuleRemoveListenerReplace = @"GameEventCenter.RemoveListener(GameEvent.{0}_${STR});";
        private static GameEventRefactor gameEventRefactor;
        //[MenuItem("Refactor/GameEvent/GameEventRefactor")]
        static void ShowWindow()
        {
			gameEventRefactor = GetWindow<GameEventRefactor>(true, "GameEventRefactor", true);
            gameEventRefactor.Show();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(MAX_WIDTH));
            GUILayout.BeginHorizontal(GUILayout.Width(MAX_WIDTH));

            if (GUILayout.Button("GameEventRefactor", GUILayout.Height(BUTTON_HEIGHT)))
            {
                BeginGameEventRefactor();
                RefactorUtils.SaveAndRefreshU3D();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        Dictionary<string, List<RefactorDelegate>> refactorDic = new Dictionary<string, List<RefactorDelegate>>();
        private void BeginGameEventRefactor()
        {
            refactorDic.Clear();
            RefactorModule();
            ReplaceGameEvent();
            GenerateGameEvent();
        }

        private void RefactorModule()
        {
            List<string> allModulePath = RefactorUtils.GetAllCodeFilesInDirectory(MyGameScriptsPath);
            allModulePath.Add(@"D:\AllProject\xclient_trunk\Assets\Scripts\MyGameScripts\Module\BackpackModule\Model\FashionModel.cs");
            foreach (string path in allModulePath)
            {
                string allContent = File.ReadAllText(path);
                string moduleName = RefactorUtils.GetH1ModuleName(allContent);
                if (string.IsNullOrEmpty(moduleName) == false)
                {
                    Debug.LogWarning(path);
                    string newAllContent = RefactorThisModule(allContent, moduleName);
                    if (newAllContent != allContent)
                    {
                        File.WriteAllText(path, newAllContent);
                    }
                }
            }
        }

        private string RefactorThisModule(string allContent, string moduleName)
        {
            List<RefactorDelegate> delegateList;
            if (refactorDic.TryGetValue(moduleName, out delegateList) == false)
            {
                delegateList = new List<RefactorDelegate>();
                refactorDic[moduleName] = delegateList;
            }
            StringBuilder newAllContent = new StringBuilder(allContent);
            RemoveModelEvent(moduleName, allContent, delegateList, newAllContent);
            ReplaceEventInvoke(allContent, delegateList, newAllContent, moduleName);
            ReplaceClearEvent(allContent, delegateList, newAllContent, moduleName);
            return newAllContent.ToString();
        }
        private static void RemoveModelEvent(string moduleName, string allContent, List<RefactorDelegate> delegateList, StringBuilder newAllContent)
        {
            MatchCollection matches = Regex.Matches(allContent, EventMatch);
            foreach (Match match in matches)
            {
                string matchValue = match.Value;
                newAllContent.Replace(matchValue, "");
                RefactorDelegate refactorDelegate = new RefactorDelegate(moduleName);
                string eventName = match.Groups["STR2"].Value;
                refactorDelegate.eventName = eventName;
                refactorDelegate.generateEventName = moduleName + '_' + eventName;
                string param = match.Groups["STR"].Value;
                refactorDelegate.param = param;

                delegateList.Add(refactorDelegate);
            }
        }

        private static void ReplaceEventInvoke(string allContent, List<RefactorDelegate> delegateList, StringBuilder newAllContent, string moduleName)
        {
            MatchCollection matches = Regex.Matches(allContent, SendEventMatch);
            ReplaceEventInvokeByMatches(delegateList, newAllContent, moduleName, matches);
            matches = Regex.Matches(allContent, SendEventMatch2);
            ReplaceEventInvokeByMatches(delegateList, newAllContent, moduleName, matches);
        }

        private static void ReplaceEventInvokeByMatches(List<RefactorDelegate> delegateList, StringBuilder newAllContent, string moduleName, MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                string matchValue = match.Value;
                string eventName = match.Groups["STR1"].Value;
                string param = match.Groups["STR2"].Value;
                RefactorDelegate delegateItem = delegateList.Find(item => item.eventName == eventName);
                if (delegateItem != null)
                {
                    string replace = null;
                    if (string.IsNullOrEmpty(param))
                    {
                        replace = Regex.Replace(matchValue, SendEventMatch, SendEventReplace2);
                        replace = string.Format(replace, moduleName);
                    }
                    else
                    {
                        replace = Regex.Replace(matchValue, SendEventMatch, SendEventReplace);
                        replace = string.Format(replace, moduleName);
                    }
                    newAllContent.Replace(matchValue, replace);
                }
            }
        }

        private void ReplaceClearEvent(string allContent, List<RefactorDelegate> delegateList, StringBuilder newAllContent, string moduleName)
        {
            MatchCollection matches = Regex.Matches(allContent, ModuleRemoveListenerMatch);
            foreach (Match match in matches)
            {
                string matchValue = match.Value;
                string eventName = match.Groups["STR"].Value;
                RefactorDelegate delegateItem = delegateList.Find(item => item.eventName == eventName);
                if(delegateItem != null)
                {
                    string replace = Regex.Replace(matchValue, ModuleRemoveListenerMatch, ModuleRemoveListenerReplace);
                    replace = string.Format(replace, moduleName);
                    newAllContent.Replace(matchValue, replace);
                }
            }
        }

        private void ReplaceGameEvent()
        {
            List<string> allControllerPath = RefactorUtils.GetAllCodeFilesInDirectory(MyGameScriptsPath);
            foreach (string path in allControllerPath)
            {
                string allContent = File.ReadAllText(path);
                string newAllContent = ReplaceGameEventThisFile(allContent);
                if (allContent != newAllContent)
                    File.WriteAllText(path, newAllContent);
            }
        }

        private string ReplaceGameEventThisFile(string allContent)
        {
            StringBuilder newAllContent = new StringBuilder(allContent);
            MatchCollection AddListenerMatches = Regex.Matches(allContent, AddListenerMatch);
            foreach (Match match in AddListenerMatches)
            {
                string matchValue = match.Value;
                string moduleName = match.Groups["STR"].Value;
                string eventName = match.Groups["STR2"].Value;
                List<RefactorDelegate> list;
                if (refactorDic.TryGetValue(moduleName, out list))
                {
                    RefactorDelegate delegateItem = list.Find(item => item.eventName == eventName);
                    if (delegateItem != null)
                    {
                        string addLissten = Regex.Replace(matchValue, AddListenerMatch, AddListenerReplace);
                        newAllContent.Replace(matchValue, addLissten);
                    }
                }
            }

            MatchCollection RemoveListenerMatches = Regex.Matches(allContent, RemoveListenerMatch);
            foreach (Match match in RemoveListenerMatches)
            {
                string matchValue = match.Value;
                string moduleName = match.Groups["STR"].Value;
                string eventName = match.Groups["STR2"].Value;
                List<RefactorDelegate> list;
                if( refactorDic.TryGetValue(moduleName, out list))
                {
                    RefactorDelegate delegateItem = list.Find(item => item.eventName == eventName);
                    if(delegateItem != null)
                    {
                        string RemoveListener = Regex.Replace(matchValue, RemoveListenerMatch, RemoveListenerReplace);
                        newAllContent.Replace(matchValue, RemoveListener);
                    }
                }
                
            }
            return newAllContent.ToString();
        }
        private void GenerateGameEvent()
        {
            string filePath = Application.dataPath + GameEventPath;
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);

                Match match = Regex.Match(content, GameEventReplace);
                string matchValue = match.Value;
                string oldGenerateEvent = match.Groups["STR"].Value;
                StringBuilder newGenerateEvent = new StringBuilder("#region 工具迁移事件");
                newGenerateEvent.AppendLine(oldGenerateEvent);

                GenerateGameEventModule(newGenerateEvent);

                newGenerateEvent.Append("\t#endregion 工具迁移事件");
                content = content.Replace(matchValue, newGenerateEvent.ToString());
                File.WriteAllText(filePath, content);
            }
            else
            {
                GameDebuger.LogError("CanNotFindPath: " + filePath);
            }
        }

        private void GenerateGameEventModule(StringBuilder newGenerateEvent)
        {
            foreach (var item in refactorDic)
            {
                if (item.Value.Count == 0)
                    continue;
                string beginRegion = string.Format("\t#region {0}", item.Key);
                newGenerateEvent.AppendLine(beginRegion);

                List<RefactorDelegate> delegateList = item.Value;
                GenerateGameEventItem(delegateList, newGenerateEvent);

                string endRegion = string.Format("\t#endregion {0}", item.Key);
                newGenerateEvent.AppendLine(endRegion);
            }
        }

        private void GenerateGameEventItem(List<RefactorDelegate> delegateList, StringBuilder newGenerateEvent)
        {
            foreach (var item in delegateList)
            {
                string eventItem = null;
                if (string.IsNullOrEmpty(item.param))
                {
                    eventItem = string.Format(EventReplace2, item.generateEventName);
                }
                else
                {
                    eventItem = string.Format(EventReplace, item.generateEventName, item.param);
                }
                newGenerateEvent.AppendLine(eventItem);
            }
        }

        private class RefactorDelegate
        {
            public readonly string moduleName;
            public string param;
            public string eventName;
            internal string generateEventName;

            public RefactorDelegate(string moduleName)
            {
                this.moduleName = moduleName;
            }
        }
    }
}
