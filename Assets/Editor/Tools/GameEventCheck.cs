using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using Object = UnityEngine.Object;
using System.Text;
using Refactor;
namespace EditorTools
{
    public class GameEventCheck : EditorWindow
    {
        private const int MAX_WIDTH = 800;
        private const int FIELD_WIDTH = 100;
        private const int BUTTON_HEIGHT = 50;
        private const string AddListenerMatch = @"GameEventCenter(\s)*?\.(\s)*?AddListener(\s)*?\((\s)*?(?<STR>[\w.]+)(\s)*?,(\s)*?(?<STR2>[\w]+)(\s)*?\)(\s)*?\;";
        private const string RemoveListenerMatch = @"GameEventCenter(\s)*?\.(\s)*?RemoveListener(\s)*?\((\s)*?(?<STR>[\w.]+)(\s)*?(,(\s)*?(?<STR2>[\w]+))?(\s)*?\)(\s)*?\;";
        private const string MyGameScriptsPath = "Assets/Scripts/MyGameScripts";


        private static GameEventCheck gameEventCheck;

        [MenuItem("Tools/GameEventCheck")]
        static void ShowWindow()
        {
            gameEventCheck = GetWindow<GameEventCheck>(false, "GameEventCheck", true);
            gameEventCheck.Show();
        }
        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(MAX_WIDTH));
            GUILayout.BeginHorizontal(GUILayout.Width(MAX_WIDTH));

            if (GUILayout.Button("GameEventCheck", GUILayout.Height(BUTTON_HEIGHT)))
            {
                outPutDic.Clear();
                BeginCheckGameEvent();
                OutFile();
                RefactorUtils.SaveAndRefreshU3D();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        Dictionary<string, OutPut> outPutDic = new Dictionary<string, OutPut>();
        private OutPut curOutPut;
        private void BeginCheckGameEvent()
        {
            List<string> allModulePath = RefactorUtils.GetAllCodeFilesInDirectory(MyGameScriptsPath);
            foreach (string path in allModulePath)
            {
                string allContent = File.ReadAllText(path);
                string fileName = Path.GetFileName(path);
                curOutPut = new OutPut(fileName, path);
                if (CheckFile(allContent))
                {
                    outPutDic.Add(fileName, curOutPut);
                }
                curOutPut = null;
            }
        }

        private bool CheckFile(string allContent)
        {
            MatchAddListener(allContent);
            MatchRemoveListener(allContent);
            return curOutPut.NeedOut();
        }

        private void MatchAddListener(string allContent)
        {
            MatchCollection matches = Regex.Matches(allContent, AddListenerMatch);
            foreach (Match match in matches)
            {
                string eventName = match.Groups["STR"].Value;
                string actionName = match.Groups["STR2"].Value;
                EventItem eventItem = curOutPut.GetEventItem(eventName);
                int count;
                if (eventItem.addCount.TryGetValue(actionName, out count) == false)
                    count = 0;
                count++;
                eventItem.addCount[actionName] = count;
            }
        }

        private void MatchRemoveListener(string allContent)
        {
            MatchCollection matches = Regex.Matches(allContent, RemoveListenerMatch);

            foreach (Match match in matches)
            {
                string eventName = match.Groups["STR"].Value;
                string actionName = match.Groups["STR2"].Value;

                if (eventName == "this")
                {
                    curOutPut.removeThis = true;
                    break;
                }
                else if(string.IsNullOrEmpty(actionName))
                {
                    EventItem eventItem = curOutPut.GetEventItem(eventName);
                    eventItem.removeCount["All"] = 1;
                }
                else
                {
                    EventItem eventItem = curOutPut.GetEventItem(eventName);
                    int count;
                    if (eventItem.removeCount.TryGetValue(actionName, out count) == false)
                        count = 0;
                    count++;
                    eventItem.removeCount[actionName] = count;
                }
            }
        }
        private class OutPut
        {
            public readonly string fileName;
            public readonly string filePath;
            public List<EventItem> eventList;
            public bool removeThis = false;
            public OutPut(string _fileName, string _filePath)
            {
                eventList = new List<EventItem>();
                fileName = _fileName;
                filePath = _filePath;
            }
            public EventItem GetEventItem(string eventName)
            {
                EventItem eventItem = eventList.Find(item => item.eventName == eventName);
                if (eventItem == null)
                {
                    eventItem = new EventItem(eventName, this);
                    eventList.Add(eventItem);
                }

                return eventItem;
            }
            public bool NeedOut()
            {
                if (removeThis)
                    return false;
                foreach (var item in eventList)
                {
                    if (item.NeedOut())
                        return true;
                }
                return false;
            }

            internal void Out(StreamWriter streamWriter)
            {
                foreach (var eventItem in eventList)
                {
                    if(eventItem.NeedOut())
                    {
                        eventItem.Out(streamWriter);
                    }
                }
            }
        }
        private class EventItem
        {
            public readonly OutPut master;
            public readonly string eventName;
            public Dictionary<string, int> addCount;
            public Dictionary<string, int> removeCount;
            public EventItem(string eventName, OutPut master)
            {
                this.eventName = eventName;
                this.master = master;
                addCount = new Dictionary<string, int>();
                removeCount = new Dictionary<string, int>();
            }
            public bool NeedOut()
            {
                if (removeCount.ContainsKey("All"))
                    return false;
                foreach (var action in addCount)
                {
                    string actionName = action.Key;
                    int addNum = action.Value;
                    if (CheckActionNeedOut(actionName, addNum))
                    {
                        return true;
                    }
                }
                return false;
            }
            private bool CheckActionNeedOut(string actionName, int addNum)
            {
                int removeNum = GetRemoveNum(actionName);
                if (removeNum < addNum)
                    return true;
                return false;
            }
            private int GetRemoveNum(string actionName)
            {
                int removeNum = 0;
                if (removeCount.TryGetValue(actionName, out removeNum) == false)
                    removeNum = 0;
                return removeNum;
                
            }
            public void Out(StreamWriter streamWriter)
            {
                foreach (var action in addCount)
                {
                    string actionName = action.Key;
                    int addNum = action.Value;
                    if (CheckActionNeedOut(actionName, addNum))
                    {
                        streamWriter.Write(master.fileName);
                        streamWriter.Write("\t");
                        streamWriter.Write(master.filePath);
                        streamWriter.Write("\t");
                        streamWriter.Write(eventName);
                        streamWriter.Write("\t");
                        streamWriter.Write(actionName);
                        streamWriter.Write("\t");
                        streamWriter.Write(addNum);
                        streamWriter.Write("\t");
                        int removeNum = GetRemoveNum(actionName);
                        streamWriter.Write(removeNum);
                        streamWriter.Write("\t");
                        streamWriter.WriteLine();
                    }
                }
            }
        }

        private void OutFile()
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format("/GameEventChek_{0}_{1}_{2}.xls", now.Day, now.Hour, now.Minute);
            string path = Application.dataPath + fileName;
            using (FileStream file = File.Create(path))
            {
                StreamWriter streamWriter = new StreamWriter(file);
                streamWriter.Write("#fileName\t");
                streamWriter.Write("filePath\t");
                streamWriter.Write("eventName\t");
                streamWriter.Write("actionName\t");
                streamWriter.Write("addCount\t");
                streamWriter.Write("removeCount\t");
                streamWriter.WriteLine();
                foreach (var outPut in outPutDic)
                {
                    outPut.Value.Out(streamWriter);
                }
                streamWriter.Close();
            }

        }
    }
}

