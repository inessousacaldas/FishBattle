using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using AppDto;
using Object = UnityEngine.Object;
namespace AssetPipeline
{
    /// <summary>
    /// 项目资源导入后处理,自动设置游戏中大部分资源BundleName,对于其依赖资源不做处理
    /// 这样做的好处就是不用每次新增资源时都要手动更新一下所有的BundleName,才能在Editor模式下进行加载
    /// 等到要真正打包的时候在做一次全面的检查操作
    /// </summary>
    public class GameResPostprocessor : AssetPostprocessor
    {
        private static readonly StringBuilder sb = new StringBuilder();
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] moveAssets, string[] movedFromAssetPaths)
        {
            sb.Length = 0;
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.IsCommonAsset())
                {
                    var importer = AssetImporter.GetAtPath(assetPath);
                    if (importer != null)
                    {
                        if (importer.UpdateBundleName(importer.GetAssetBundleName(ResGroup.Common)))
                        {
                            sb.AppendLine("Update Common BundleName:" + assetPath);
                        }
                    }
                }
                else if (assetPath.IsPrefabFile())
                {
                    if (assetPath.IsUIPrefab())
                    {
                        var uiImporter = AssetImporter.GetAtPath(assetPath);
                        if (uiImporter.UpdateBundleName(uiImporter.GetAssetBundleName(ResGroup.UIPrefab)))
                        {
                            sb.AppendLine("Update UIPrefab BundleName:" + assetPath);
                        }
                    }
                    else if (assetPath.IsGameModel())
                    {
                        var modelImporter = AssetImporter.GetAtPath(assetPath);
                        string prefabBundleName = modelImporter.GetAssetBundleName(ResGroup.Model);
                        if (modelImporter.UpdateBundleName(prefabBundleName))
                        {
                            sb.AppendLine("Update Model BundleName:" + assetPath);

                            //只有ModelPrefab的BundleName发生变更时,才重新设置其依赖资源BundleName
                            string modelName = assetPath.ExtractResName();
                            //只对pet_或ride_pet_为前缀的模型根据依赖关系打包，其它模型全部资源打成一个包
                            if (modelName.StartsWith("pet_") || modelName.StartsWith("ride_pet_"))
                            {
                                //模型关联材质统一打包
                                string matDir = Path.GetDirectoryName(assetPath).Replace("/Prefabs", "/Materials");
                                if (Directory.Exists(matDir))
                                {
                                    var matGUIDs = AssetDatabase.FindAssets("t:Material", new[] { matDir });
                                    if (matGUIDs.Length > 0)
                                    {
                                        foreach (var matGUID in matGUIDs)
                                        {
                                            string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                                            var matImporter = AssetImporter.GetAtPath(matPath);
                                            if (matImporter.UpdateBundleName(prefabBundleName + "_mat"))
                                            {
                                                sb.AppendLine("Update ModelMat BundleName:" + matPath);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError("当前模型关联材质数量为0,请检查:" + modelName);
                                    }
                                }
                                else
                                {
                                    if (matDir.Contains("RoleCreate") == false)
                                        Debug.LogError("当前模型材质目录命名异常,请检查:" + matDir);
                                }
                            }
                        }
                    }
                    else if (assetPath.IsGameEffect())
                    {
                        var effectImporter = AssetImporter.GetAtPath(assetPath);
                        if (effectImporter.UpdateBundleName(effectImporter.GetAssetBundleName(ResGroup.Effect)))
                        {
                            sb.AppendLine("Update Effect BundleName:" + assetPath);
                        }
                    }
                    else if (assetPath.IsUIAtlas())
                    {
                        if (assetPath.IsCommonAsset()) continue;
                        var uiImporter = AssetImporter.GetAtPath(assetPath);

                        if (uiImporter.UpdateBundleName(uiImporter.GetAssetBundleName(ResGroup.UIAtlas)))
                        {
                            sb.AppendLine("Update Atlas BundleName:" + assetPath);
                        }
                    }
                    else if (assetPath.IsUIFont())
                    {
                        if (assetPath.IsCommonAsset()) continue;
                        var prefabImport = AssetImporter.GetAtPath(assetPath);
                        //MyFont相关资源需要特殊处理,加上表情图集一起打包
                        if (assetPath.StartsWith("Assets/UI/Fonts/MyFont/"))
                        {
                            var dependencies = AssetDatabase.GetDependencies(assetPath);
                            foreach (string refPath in dependencies)
                            {
                                if (refPath.StartsWith("Assets/UI/Fonts/MyFont/"))
                                {
                                    var importer = AssetImporter.GetAtPath(refPath);
                                    if (importer.UpdateBundleName(AssetManager.GetBundleName("MyFont1", ResGroup.UIFont)))
                                    {
                                        sb.AppendLine("Update UIFont BundleName:" + assetPath);

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (prefabImport.UpdateBundleName(prefabImport.GetAssetBundleName(ResGroup.UIFont)))
                            {
                                sb.AppendLine("Update UIFont BundleName:" + assetPath);

                            }
                        }
                    }
                }
                else if (assetPath.IsTextureFile())
                {
                    if (assetPath.IsGameImage())
                    {
                        var imageImporter = AssetImporter.GetAtPath(assetPath);
                        if (imageImporter != null &&
                            imageImporter.UpdateBundleName(imageImporter.GetAssetBundleName(ResGroup.Image)))
                        {
                            sb.AppendLine("Update Image BundleName:" + assetPath);
                        }
                    }
                }
                else if (assetPath.IsAudioFile())
                {
                    if (assetPath.IsGameAudio())
                    {
                        var audioImporter = AssetImporter.GetAtPath(assetPath);
                        if (audioImporter.UpdateBundleName(audioImporter.GetAssetBundleName(ResGroup.Audio)))
                        {
                            sb.AppendLine("Update Audio BundleName:" + assetPath);
                        }
                    }
                }
                else if (assetPath.IsConfigFile())
                {
                    if (assetPath.IsGameConfig() && !assetPath.Contains("ConfigDataNew"))
                    {
                        var importer = AssetImporter.GetAtPath(assetPath);
                        if (importer.UpdateBundleName(importer.GetAssetBundleName(ResGroup.Config)))
                        {
                            sb.AppendLine("Update Config BundleName:" + assetPath);
                        }
                    }
                    /*else if (assetPath.IsGameScript())
                    {
                        var importer = AssetImporter.GetAtPath(assetPath);
#if ENABLE_JSB
                        if (importer.UpdateBundleName(GameResPath.AllScriptBundleName))
                        {
                            sb.AppendLine("Update Script BundleName:" + assetPath);
                        }
#else
                        importer.assetBundleName = "";
#endif
                    }*/
                }
            }

            if (sb.Length > 0)
            {
                Debug.Log(sb);
            }
        }
    }

    public class AssetBundleBuilder : EditorWindow
    {
        public static AssetBundleBuilder Instance;

        [MenuItem("GameResource/AssetBundleBuilder #&e")]
        public static void ShowWindow()
        {
            if (Instance == null)
            {
                var window = GetWindow<AssetBundleBuilder>(false, "AssetBundleBuilder", true);
                window.minSize = new Vector2(872f, 680f);
                window.Show();
            }
            else
            {
                Instance.Close();
            }
        }

        private Dictionary<ResGroup, bool> _foldoutDic;
        private static ResConfig _oldResConfig;
        public static ResConfig _curResConfig;
        private static BuildBundleStrategy _buildBundleStrategy;

        private void OnEnable()
        {
            //var st = Stopwatch.StartNew();
            Instance = this;
            _foldoutDic = new Dictionary<ResGroup, bool>();
            var resGroups = Enum.GetValues(typeof(ResGroup));
            foreach (ResGroup flag in resGroups)
            {
                _foldoutDic.Add(flag, EditorPrefs.GetBool("ABFoldOut_" + flag, false));
            }

            var curBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            switch (curBuildTarget)
            {
                case BuildTarget.Android:
                    _selectedCdnPlatformType = CDNPlatformType.Andoird;
                    break;
                case BuildTarget.iOS:
                    _selectedCdnPlatformType = CDNPlatformType.IOS;
                    _showPrograss = false;
                    break;
                default:
                    _selectedCdnPlatformType = CDNPlatformType.Win;
                    break;
            }

            //加载最近一次加载的版本信息
            if (_curResConfig == null)
            {
                string lastResConfigPathHash = GetLastResConfigPathHash();
                string lastResConfigPath = EditorPrefs.GetString(lastResConfigPathHash, "");
                RefreshResConfigData(LoadResConfig(lastResConfigPath));
            }

            //加载小包资源配置策略
            if (_buildBundleStrategy == null)
            {
                string strategyPath = GetBuildBundleStrategyPath();
                if (File.Exists(strategyPath))
                {
                    _buildBundleStrategy = FileHelper.ReadJsonFile<BuildBundleStrategy>(strategyPath);
                }
                else
                {
                    _buildBundleStrategy = new BuildBundleStrategy();
                }
                RefreshBundleNameData();
            }
            //st.Stop();
            //Debug.LogError(st.ElapsedMilliseconds);
        }

        private void OnDestroy()
        {
            //退出前保存下打包策略配置
            SaveBuildBundleStrategy();

            //保存ResConfigPanel面板操作信息
            if (_foldoutDic != null)
            {
                var resGroups = Enum.GetValues(typeof(ResGroup));
                foreach (ResGroup flag in resGroups)
                {
                    EditorPrefs.SetBool("ABFoldOut_" + flag, _foldoutDic[flag]);
                }
            }

            Instance = null;
        }

        #region Editor UI

        private int _rightTab;
        private bool _showPrograss = true;
        public bool _slientMode = false;
        private Vector2 _leftContentScroll;
        //标识哪些资源分组需要重新更新BundleName
        private UpdateBundleFlag _updateResGroupMask = UpdateBundleFlag.Everything;

        [Flags]
        public enum UpdateBundleFlag
        {
            Everything = -1,
            Nothing = 0,
            UI = 2,
            Model = 4,
            Effect = 8,
            Scene = 16,
            Audio = 32,
            Config = 64,
            Script = 128,
            StreamScene = 0x01 << 8,
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(430f)); //Left Cotent Begin
            _leftContentScroll = EditorGUILayout.BeginScrollView(_leftContentScroll, "PreVerticalScrollbar",
                "PreVerticalScrollbar");
            {
                //版本信息
                EditorGUILayout.BeginVertical("HelpBox");
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("版本信息", "BoldLabel");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button("加载Old ResConfig", "LargeButton", GUILayout.Height(50f)))
                        {
                            _oldResConfig = LoadResConfigFilePanel();
                        }
                        GUILayout.Label(GetResConfigInfo(_oldResConfig));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button("加载Cur ResConfig", "LargeButton", GUILayout.Height(50f)))
                        {
                            RefreshResConfigData(LoadResConfigFilePanel(true));
                            _rightTab = 1;
                        }
                        GUILayout.Label(GetResConfigInfo(_curResConfig));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();

                //打包选项
                EditorGUILayout.BeginVertical("HelpBox");
                {
                    GUILayout.Label("打包", "BoldLabel");
                    _updateResGroupMask = (UpdateBundleFlag)EditorGUILayout.EnumMaskField("ResGroup:", _updateResGroupMask);
                    if (GUILayout.Button("更新所有BundleName", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (_updateResGroupMask == UpdateBundleFlag.Nothing) return;

                        if (EditorUtility.DisplayDialog("确认", "是否重新设置所有资源BundleName?", "继续", "取消"))
                        {
                            EditorApplication.delayCall += UpdateAllBundleName;
                            _rightTab = 0;
                        }
                    }
                    EditorGUILayout.Space();

                    if (GUILayout.Button("清空所有BundleName", "LargeButton", GUILayout.Height(50f)))
                    {
                        int option = EditorUtility.DisplayDialogComplex("确认", "是否清空所有资源BundleName?", "全部清空", "Cancel", "清空未使用的");
                        if (option != 1)
                        {
                            EditorApplication.delayCall += () =>
                            {
                                CleanUpBundleName(option == 0);
                            };
                        }
                    }
                    EditorGUILayout.Space();

                    GUI.color = Color.red;
                    if (GUILayout.Button("一键打包新版本整包资源", "LargeButton", GUILayout.Height(50f)))
                    {
                        long nextVer = 0;
                        if (_curResConfig == null)
                        {
                            string filePath = EditorUtility.OpenFilePanel("加载版本资源配置信息", GetResConfigRoot(), "json");
                            var match = Regex.Match(filePath, @"resConfig_(\d+)");
                            if (match.Success && match.Groups.Count > 0)
                            {
                                nextVer = long.Parse(match.Groups[1].Value) + 1;
                            }
                        }
                        else
                        {
                            nextVer = _curResConfig.Version + 1;
                        }
                        string tip = nextVer == 0
                            ? "当前版本ResConfig为空,资源版本号将归0,请确认?"
                            : "本次打包资源版本号为:" + nextVer;
                        if (EditorUtility.DisplayDialog("确认", tip, "继续", "取消"))
                        {
                            bool generateTotalRes = EditorUtility.DisplayDialog("生成包内资源", "是否生成包内资源？", "生成", "不生成");
                            EditorApplication.delayCall += () =>
                            {
                                BuildAll(nextVer, generateTotalRes);
                            };
                            _rightTab = 1;
                        }
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("生成包内整包资源", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否需要把导出资源迁移到StreamingAssets", "继续", "取消"))
                        {
                            EditorApplication.delayCall += GenerateTotalRes;
                        }
                    }
                    if (GUILayout.Button("生成包内小包资源", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否需要把导出资源迁移到StreamingAssets", "继续", "取消"))
                        {
                            EditorApplication.delayCall += GenerateMiniRes;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (GUILayout.Button("还原当前版本资源到gameres下", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (_curResConfig == null) return;
                        if (EditorUtility.DisplayDialog("确认", "是否还原资源版本号为: " + _curResConfig.Version + " 到gameres目录下,\n这将会覆盖打包目录下的manifest文件,请确认?", "继续", "取消"))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                RevertBackupToGameRes(_curResConfig);
                            };
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Resconfig转换Json->Tz", "LargeButton", GUILayout.Height(50f)))
                        {
                            EditorApplication.delayCall += ResconfigJson2Tz;
                        }
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Resconfig转换Tz->Json", "LargeButton", GUILayout.Height(50f)))
                        {
                            EditorApplication.delayCall += ResconfigTz2Json;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                //生成补丁包选项
                EditorGUILayout.BeginVertical("HelpBox");
                {
                    GUILayout.Label("生成补丁", "BoldLabel");
                    if (GUILayout.Button("加载所有已生成的所有PatchInfo", "LargeButton", GUILayout.Height(50f)))
                    {
                        LoadAllPatchInfo();
                        _rightTab = 2;
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("生成OldVer-->CurVer\nPatchInfo", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (_oldResConfig == null)
                        {
                            _oldResConfig = LoadResConfigFilePanel();
                        }

                        GeneratePatchInfo(_oldResConfig, _curResConfig);
                        _rightTab = 2;
                    }

                    if (GUILayout.Button("生成所有版本-->CurVer\nPatchInfo", "LargeButton", GUILayout.Height(50f)))
                    {
                        EditorApplication.delayCall += GenerateAllPatchInfo;
                        _rightTab = 2;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (GUILayout.Button("生成CurVer所有CDN资源", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否生成当前版本所有CDN资源?", "继续", "取消"))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                GenerateRemoteRes(_curResConfig);
                            };
                        }
                    }
                    EditorGUILayout.Space();

                    if (GUILayout.Button("上传补丁资源和版本号", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否上传补丁资源和版本号?", "继续", "取消"))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                GameResVersionManager.UploadVersionResourcesPatch(_curResConfig);
                            };
                        }
                    }
                    EditorGUILayout.Space();

                    if (GUILayout.Button("检查无用资源", "LargeButton", GUILayout.Height(50f)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否检查无用资源?", "继续", "取消"))
                        {
                            EditorApplication.delayCall += CheckUnusedRes;
                        }
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical(); //Left Cotent End

            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical(); //Right Cotent Begin
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(_rightTab == 0, "BundleNamePanel", "ButtonLeft"))
                    _rightTab = 0;
                if (GUILayout.Toggle(_rightTab == 1, "ResConfigPanel", "ButtonMid"))
                    _rightTab = 1;
                if (GUILayout.Toggle(_rightTab == 2, "PatchInfoList", "ButtonMid"))
                    _rightTab = 2;
                if (GUILayout.Toggle(_rightTab == 3, "MinResList", "ButtonRight"))
                    _rightTab = 3;
                EditorGUILayout.EndHorizontal();

                if (_rightTab == 0)
                {
                    DrawBundleNamePanel();
                }
                else if (_rightTab == 1)
                {
                    DrawResConfigPanel();
                }
                else if (_rightTab == 2)
                {
                    DrawPatchInfoPanel();
                }
                else if (_rightTab == 3)
                {
                    DrawMinResListPanel();
                }
            }
            EditorGUILayout.EndVertical(); //Right Cotent End

            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();
        }

#endregion

#region CheckUnusedRes
        private void CheckUnusedRes()
        {
            CheckBattleSkillRes();
            CheckSoundRes();
        }

        /// <summary>
        /// 检查战斗特效
        /// </summary>
        private void CheckBattleSkillRes()
        {
            GameDebuger.Log("检查无用战斗技能特效");
            if (_buildBundleStrategy != null)
            {
                Dictionary<int, SkillConfigInfo> dics = new Dictionary<int, SkillConfigInfo> ();

                String battleConfigPath = "Assets/GameResources/ConfigFiles/BattleConfig/BattleConfig.bytes";

                JsonBattleConfigInfo battleConfig = FileHelper.ReadJsonFile<JsonBattleConfigInfo>(battleConfigPath);

                if (battleConfig != null) {
                    for (int i=0,len=battleConfig.list.Count; i<len; i++)
                    {
                        JsonSkillConfigInfo info = battleConfig.list[i];
                        if(dics.ContainsKey(info.id))
                            GameDebuger.LogError(string.Format("BattleConfig这个ID已存在，策划赶紧改下。id:{0},name:{1}",info.id,info.name));
                        else
                            dics.Add (info.id, info.ToSkillConfigInfo ());
                    }
                }

                List<string> effectInfos = new List<string>();

                if (dics != null)
                {
                    foreach(int key in dics.Keys)
                    {
                        SkillConfigInfo info = dics[key];

                        foreach(BaseActionInfo action in info.attackerActions)
                        {
                            if (action.effects != null)
                            {
                                foreach(BaseEffectInfo effect in action.effects)
                                {
                                    if (effect is NormalEffectInfo)
                                    {
                                        string effConfig = info.id+";" +(effect as NormalEffectInfo).name;
                                        if (!effectInfos.Contains(effConfig))
                                        {
                                            effectInfos.Add(effConfig);
                                        }
                                    }
                                }                        
                            }
                        }

                        foreach(BaseActionInfo action in info.injurerActions)
                        {
                            if (action.effects != null)
                            {
                                foreach(BaseEffectInfo effect in action.effects)
                                {
                                    if (effect is NormalEffectInfo)
                                    {
                                        string effConfig = info.id+";" +(effect as NormalEffectInfo).name;
                                        if (!effectInfos.Contains(effConfig))
                                        {
                                            effectInfos.Add(effConfig);
                                        }
                                    }
                                }                        
                            }
                        }
                    }
                }

                List<string> skillPrefabList = new List<string>();

                foreach(string info in effectInfos)
                {
                    string id = info.Split(';')[0];
                    string name = info.Split(';')[1];

                    if (name.Contains("_") == false)
                    {
                        string skillPrefabName = string.Format("skill_eff_{0}_{1}", id, name);
                        skillPrefabList.Add(skillPrefabName);
                    }
                    else
                    {
                        skillPrefabList.Add(name);
                    }
                }

                List<string> notFindList = new List<string>();

                int totalCount = 0;
                Dictionary<string, string> confis = _buildBundleStrategy.replaceResConfig;
                foreach(string key in confis.Keys)
                {
                    if (key.StartsWith("effect/skill_eff"))
                    {
                        var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(key);
                        if (assetPaths.Length > 0)
                        {
                            if (!assetPaths[0].Contains("TempRes"))
                            {
                                totalCount++;
                                string assetName = key.Replace("effect/", "");
                                //Debug.Log("assetName = " + assetName);
                                if (!skillPrefabList.Contains(assetName))
                                {
                                    //GameDebuger.LogError(assetName + " Not Find It");
                                    notFindList.Add(assetName);
                                }                                
                            }
                        }
                    }
                }

                GameDebuger.LogError(string.Format("无用战斗技能特效 {0}/{1} :\n{2}", notFindList.Count, totalCount, string.Join("\n", notFindList.ToArray())));
            }
        }

   

        //检查无用音效资源
        private void CheckSoundRes()
        {
            GameDebuger.Log("检查无用音效资源");
            if (_buildBundleStrategy != null)
            {
                List<string> checkList = new List<string>();

                //宠物战斗音效
                List<Model> modelList = DataCache.getArrayByCls<Model>();
                foreach(Model model in modelList)
                {
                    checkList.Add(model.phySkillSound);
                    checkList.Add(model.magicSkillSound);
                }

                List<Npc> npcList = DataCache.getArrayByCls<Npc>();

                foreach(Npc npc in npcList)
                {
                    if (npc is NpcGeneral)
                    {
                        checkList.Add( "sound_npc_" + (npc as NpcGeneral).soundId);    
                    }
                }

                //战斗技能音效
                Dictionary<int, SkillConfigInfo> dics = new Dictionary<int, SkillConfigInfo> ();

                String battleConfigPath = "Assets/GameResources/ConfigFiles/BattleConfig/BattleConfig.bytes";

                JsonBattleConfigInfo battleConfig = FileHelper.ReadJsonFile<JsonBattleConfigInfo>(battleConfigPath);

                if (battleConfig != null) {
                    for (int i=0,len=battleConfig.list.Count; i<len; i++)
                    {
                        JsonSkillConfigInfo info = battleConfig.list[i];
                        if(dics.ContainsKey(info.id))
                            GameDebuger.LogError(string.Format("BattleConfig这个ID已存在，策划赶紧改下。id:{0},name:{1}",info.id,info.name));
                        else
                            dics.Add (info.id, info.ToSkillConfigInfo ());
                    }
                }

                List<string> effectInfos = new List<string>();

                if (dics != null)
                {
                    foreach(int key in dics.Keys)
                    {
                        SkillConfigInfo info = dics[key];

                        foreach(BaseActionInfo action in info.attackerActions)
                        {
                            if (action.effects != null)
                            {
                                foreach(BaseEffectInfo effect in action.effects)
                                {
                                    if (effect is SoundEffectInfo)
                                    {
                                        string soundName = (effect as SoundEffectInfo).name;
                                        checkList.Add(soundName);
                                    }
                                }                        
                            }
                        }

                        foreach(BaseActionInfo action in info.injurerActions)
                        {
                            if (action.effects != null)
                            {
                                foreach(BaseEffectInfo effect in action.effects)
                                {
                                    if (effect is SoundEffectInfo)
                                    {
                                        string soundName = (effect as SoundEffectInfo).name;
                                        checkList.Add(soundName);
                                    }
                                }                        
                            }
                        }
                    }
                }


                List<string> notFindList = new List<string>();
                int totalCount = 0;
                Dictionary<string, string> confis = _buildBundleStrategy.replaceResConfig;
                foreach(string key in confis.Keys)
                {
                    if (key.StartsWith("audio/pet_") || key.StartsWith("audio/sound_"))
                    {
                        var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(key);
                        if (assetPaths.Length > 0)
                        {
                            if (!assetPaths[0].Contains("TempRes"))
                            {
                                totalCount++;
                                string assetName = key.Replace("audio/", "");
                                //Debug.Log("assetName = " + assetName);
                                if (!checkList.Contains(assetName))
                                {
                                    //GameDebuger.LogError(assetName + " Not Find It");
                                    notFindList.Add(assetName);
                                }                                
                            }
                        }
                    }
                }

                GameDebuger.LogError(string.Format("无用音效资源 {0}/{1} :\n{2}", notFindList.Count, totalCount, string.Join("\n", notFindList.ToArray())));
            }

        }
#endregion

#region ResConfigPanel

        private string _manifestSearchFilter = "";
        private string _selectedManifestKey;
        private Vector2 _resConfigPanelScroll;
        private Vector2 _resConfigPanelDetailScroll;
        private readonly StringBuilder _bundleManifestInfo = new StringBuilder();
        private static Dictionary<ResGroup, List<string>> _manifestBundleNameGroups; //当前版本资源配置BundleName分组信息

        private static void RefreshResConfigData(ResConfig resConfig)
        {
            _curResConfig = resConfig;
            if (_manifestBundleNameGroups == null)
            {
                _manifestBundleNameGroups = new Dictionary<ResGroup, List<string>>();
                var resGroupEnums = Enum.GetValues(typeof(ResGroup));
                foreach (ResGroup resGroup in resGroupEnums)
                {
                    _manifestBundleNameGroups.Add(resGroup, new List<string>());
                }
            }
            else
            {
                foreach (var resGroupList in _manifestBundleNameGroups.Values)
                {
                    resGroupList.Clear();
                }
            }

            if (_curResConfig != null)
            {
                foreach (var pair in _curResConfig.Manifest)
                {
                    var resGroup = ResConfig.GetResGroupFromBundleName(pair.Key);
                    _manifestBundleNameGroups[resGroup].Add(pair.Key);
                }
            }
        }

        private void DrawResConfigPanel()
        {

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("打开ResConfig目录", "LargeButton", GUILayout.Height(20f)))
            {
                OpenDirectory(GetResConfigRoot());
            }
            if (GUILayout.Button("打开版本备份目录", "LargeButton", GUILayout.Height(20f)))
            {
                OpenDirectory(GetBackupRoot());
            }
            EditorGUILayout.EndHorizontal();
            // Search field
            GUILayout.BeginHorizontal();
            {
                var after = EditorGUILayout.TextField("", _manifestSearchFilter, "SearchTextField");

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = "";
                    GUIUtility.keyboardControl = 0;
                }

                if (_manifestSearchFilter != null && _manifestSearchFilter != after)
                {
                    _manifestSearchFilter = after;
                }
            }
            GUILayout.EndHorizontal();

            //BundleName列表
            if (_manifestBundleNameGroups != null && _manifestBundleNameGroups.Count > 0)
            {
                var resultDic = new Dictionary<ResGroup, int>(_manifestBundleNameGroups.Count);
                EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(300f));
                {
                    EditorGUILayout.Space();
                    _resConfigPanelScroll = EditorGUILayout.BeginScrollView(_resConfigPanelScroll);
                    foreach (var pair in _manifestBundleNameGroups)
                    {
                        var resGroup = pair.Key;
                        var buildResList = pair.Value;
                        GUILayout.BeginHorizontal();
                        _foldoutDic[resGroup] = EditorGUILayout.Foldout(_foldoutDic[resGroup], resGroup + " Count: " + buildResList.Count);
                        bool essentialRes = IsEssentialResType(resGroup);
                        if (!essentialRes && _buildBundleStrategy != null)
                        {
                            if (GUILayout.Button("全选", GUILayout.Width(40f)))
                            {
                                if (EditorUtility.DisplayDialog("提示", "将该分组资源全部设置为小包包内资源,请确认?", "确定", "取消"))
                                {
                                    foreach (string resKey in buildResList)
                                    {
                                        _buildBundleStrategy.AddMinResKey(resKey);
                                    }
                                }
                            }
                            if (GUILayout.Button("取消", GUILayout.Width(40f)))
                            {
                                if (EditorUtility.DisplayDialog("提示", "将该分组资源从小包包内资源中移除,请确认?", "确定", "取消"))
                                {
                                    foreach (string resKey in buildResList)
                                    {
                                        _buildBundleStrategy.RemoveMinResKey(resKey);
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                        var hitCount = 0;
                        if (_foldoutDic[resGroup])
                        {
                            for (var i = 0; i < buildResList.Count; ++i)
                            {
                                var bundleName = buildResList[i];
                                if (!string.IsNullOrEmpty(_manifestSearchFilter) &&
                                    bundleName.IndexOf(_manifestSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                                    continue;
                                hitCount++;
                                GUILayout.Space(-1f);
                                GUI.backgroundColor = _selectedManifestKey == bundleName
                                    ? Color.white
                                    : new Color(0.8f, 0.8f, 0.8f);
                                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                                GUI.backgroundColor = Color.white;

                                //编号
                                GUILayout.Label(i.ToString(), GUILayout.Width(40f));

                                if (GUILayout.Button(bundleName, "OL TextField", GUILayout.Height(20f)))
                                {
                                    if (_selectedManifestKey != bundleName)
                                    {
                                        _selectedManifestKey = bundleName;
                                        _bundleManifestInfo.Length = 0;
                                    }
                                }

                                if (!essentialRes && _buildBundleStrategy != null)
                                {
                                    var resInfo = _curResConfig.GetResInfo(bundleName);
                                    if (_buildBundleStrategy.minResConfig.ContainsKey(bundleName))
                                    {
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button("X", GUILayout.Width(22f)))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "将该资源以及其依赖资源从小包包内资源中移除,请确认?", "确定",
                                                "取消"))
                                            {
                                                RemoveMinResKeySetRecursively(bundleName, resInfo);
                                            }
                                        }
                                        GUI.backgroundColor = Color.white;
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(" ", GUILayout.Width(22f)))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "将该资源以及其依赖资源添加到小包包内资源中,请确认?", "确定",
                                                "取消"))
                                            {
                                                AddMinResKeySetRecursively(bundleName, resInfo);
                                            }
                                        }
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }
                        }
                        resultDic[resGroup] = hitCount;
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("HelpBox");
                if (resultDic.Count > 0)
                {
                    var sb = new StringBuilder("Search Result:\n");
                    var index = 0;
                    foreach (var pair in resultDic)
                    {
                        sb.Append(pair.Key + ": " + pair.Value + "/" + _manifestBundleNameGroups[pair.Key].Count + "  ");
                        if (index++ > 3)
                        {
                            index = 0;
                            sb.AppendLine();
                        }
                    }
                    GUILayout.Label(sb.ToString(), "LargeLabel");
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("ResInfoList is null");
            }

            //绘制选中BundleName详细信息
            _resConfigPanelDetailScroll = DrawResInfoDetailPanel(_selectedManifestKey, _resConfigPanelDetailScroll);
        }

        private Vector2 DrawResInfoDetailPanel(string bundleName, Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "HelpBox");
            if (_curResConfig != null && !string.IsNullOrEmpty(bundleName))
            {
                var resInfo = _curResConfig.GetResInfo(bundleName);
                if (resInfo != null)
                {
                    string replaceResKey = "";
                    if (_buildBundleStrategy != null)
                        _buildBundleStrategy.replaceResConfig.TryGetValue(bundleName, out replaceResKey);

                    GUILayout.Label(
                        String.Format(
                            "BundleName:{0}\nCRC:{1}\nHash:{2}\nCompressType:{3}\nMD5:{4}\nSize:{5}\nreplaceResKey:{6}\npreload:{7}\n",
                            resInfo.bundleName, resInfo.CRC, resInfo.Hash, resInfo.remoteZipType, resInfo.MD5,
                            EditorUtility.FormatBytes(resInfo.size), replaceResKey, resInfo.preload));

                    GUILayout.Label("=====================================");
                    GUILayout.Label("Dependencies:" + resInfo.Dependencies.Count);
                    foreach (string dependency in resInfo.Dependencies)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(dependency);
                        if (GUILayout.Button("选中", GUILayout.Width(50f)))
                        {
                            _selectedManifestKey = dependency;
                            _bundleManifestInfo.Length = 0;

                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Label("=====================================");

                    if (_bundleManifestInfo.Length > 0)
                    {
                        GUILayout.TextArea(_bundleManifestInfo.ToString());
                    }
                    else
                    {
                        if (GUILayout.Button("查看BundleManifest文件", GUILayout.Height(40f)))
                        {
                            string backupDir = GetBackupDir(_curResConfig);
                            string bundleBackupDir = resInfo.remoteZipType == CompressType.UnityLZMA
                                ? backupDir + "/lzma"
                                : backupDir + "/lz4";
                            var bundleManifestPath = resInfo.GetManifestPath(bundleBackupDir);
                            if (File.Exists(bundleManifestPath))
                            {
                                _bundleManifestInfo.Append(File.ReadAllText(bundleManifestPath));
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            return scrollPos;
        }

#endregion

#region BundleNamePanel

        private string _projectSearchFilter = "";
        private string _selectedProjectBundleName = "";
        private Vector2 _bundleNamePanelScroll;
        private Vector2 _bundleNamePanelDetailScroll;
        private static Dictionary<ResGroup, List<string>> _projectBundleNameGroups; //当前项目里BundleName分组信息
        private static int _projectBundleNameTotalCount; //当前项目里BundleName总数
        private static HashSet<string> _unusedBundleNameSet; //当前项目里未使用的BundleName集合

        private static void RefreshBundleNameData(bool updateMinRes = false)
        {
            if (_projectBundleNameGroups == null)
            {
                _projectBundleNameGroups = new Dictionary<ResGroup, List<string>>();
                var resGroupEnums = Enum.GetValues(typeof(ResGroup));
                foreach (ResGroup resGroup in resGroupEnums)
                {
                    _projectBundleNameGroups.Add(resGroup, new List<string>());
                }
            }
            else
            {
                foreach (var resGroupList in _projectBundleNameGroups.Values)
                {
                    resGroupList.Clear();
                }
            }

            var unusedBundleNames = AssetDatabase.GetUnusedAssetBundleNames();
            _unusedBundleNameSet = new HashSet<string>(unusedBundleNames);

            var bundleNames = AssetDatabase.GetAllAssetBundleNames();
            _projectBundleNameTotalCount = bundleNames.Length;
            foreach (var bundleName in bundleNames)
            {
                var resGroup = ResConfig.GetResGroupFromBundleName(bundleName);
                _projectBundleNameGroups[resGroup].Add(bundleName);

                //更新MiniResStrategy
                if (updateMinRes)
                {
                    if (_unusedBundleNameSet.Contains(bundleName))
                    {
                        //不在使用的BundleName,从小包资源配置策略中移除
                        _buildBundleStrategy.RemoveMinResKey(bundleName);
                    }
                    else
                    {
                        //新增的BundleName默认替代资源为空
                        if (!_buildBundleStrategy.replaceResConfig.ContainsKey(bundleName))
                        {
                            _buildBundleStrategy.replaceResConfig.Add(bundleName, "");
                        }

                        if (IsEssentialResType(resGroup))
                        {
                            _buildBundleStrategy.AddMinResKey(bundleName);
                        }
                    }
                }
            }

            //更新完毕,保存一下MiniResStrategy
            if (updateMinRes)
                SaveBuildBundleStrategy();
        }

        private void DrawBundleNamePanel()
        {
            // Search field
            GUILayout.BeginHorizontal();
            {
                var after = EditorGUILayout.TextField("", _projectSearchFilter, "SearchTextField");

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = "";
                    GUIUtility.keyboardControl = 0;
                }

                if (_projectSearchFilter != null && _projectSearchFilter != after)
                {
                    _projectSearchFilter = after;
                }
            }
            GUILayout.EndHorizontal();

            //BundleName列表
            if (_projectBundleNameGroups != null && _projectBundleNameGroups.Count > 0)
            {
                var resultDic = new Dictionary<ResGroup, int>(_projectBundleNameGroups.Count);
                EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(300f));
                {
                    GUILayout.Label(
                        "Total BundleName:" + _projectBundleNameTotalCount + " Unused BundleName:" +
                        _unusedBundleNameSet.Count, "LargeLabel");
                    EditorGUILayout.Space();
                    _bundleNamePanelScroll = EditorGUILayout.BeginScrollView(_bundleNamePanelScroll);
                    foreach (var pair in _projectBundleNameGroups)
                    {
                        var resGroup = pair.Key;
                        var buildResList = pair.Value;
                        GUILayout.BeginHorizontal();
                        _foldoutDic[resGroup] = EditorGUILayout.Foldout(_foldoutDic[resGroup], resGroup + " Count: " + buildResList.Count);
                        GUILayout.EndHorizontal();
                        var hitCount = 0;
                        if (_foldoutDic[resGroup])
                        {
                            for (var i = 0; i < buildResList.Count; ++i)
                            {
                                var bundleName = buildResList[i];
                                if (!string.IsNullOrEmpty(_projectSearchFilter) &&
                                    bundleName.IndexOf(_projectSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                                    continue;
                                hitCount++;
                                GUILayout.Space(-1f);
                                GUI.backgroundColor = _selectedProjectBundleName == bundleName
                                    ? Color.white
                                    : new Color(0.8f, 0.8f, 0.8f);
                                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                                GUI.backgroundColor = Color.white;

                                //编号
                                GUILayout.Label(i.ToString(), GUILayout.Width(40f));

                                GUI.color = _unusedBundleNameSet.Contains(bundleName) ? Color.yellow : Color.white;
                                if (GUILayout.Button(bundleName, "OL TextField", GUILayout.Height(20f)))
                                {
                                    _selectedProjectBundleName = bundleName;
                                }
                                GUI.color = Color.white;

                                if (_buildBundleStrategy != null &&
                                    !bundleName.StartsWith(ResGroup.Common.ToString().ToLower()))
                                {
                                    if (_buildBundleStrategy.preloadConfig.ContainsKey(bundleName))
                                    {
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button("X", GUILayout.Width(22f)))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "将该资源从预加载列表中移除,请确认?", "确定", "取消"))
                                            {
                                                _buildBundleStrategy.preloadConfig.Remove(bundleName);
                                            }
                                        }
                                        GUI.backgroundColor = Color.white;
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(" ", GUILayout.Width(22f)))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "将该资源加入到预加载列表中,请确认?", "确定",
                                                "取消"))
                                            {
                                                _buildBundleStrategy.preloadConfig.Add(bundleName, true);
                                            }
                                        }
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }
                        }
                        resultDic[resGroup] = hitCount;
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("HelpBox");
                if (resultDic.Count > 0)
                {
                    var sb = new StringBuilder("Search Result:\n");
                    var index = 0;
                    foreach (var pair in resultDic)
                    {
                        sb.Append(pair.Key + ": " + pair.Value + "/" + _projectBundleNameGroups[pair.Key].Count + "  ");
                        if (index++ > 3)
                        {
                            index = 0;
                            sb.AppendLine();
                        }
                    }
                    GUILayout.Label(sb.ToString(), "LargeLabel");
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("ResInfoList is null");
            }

            //绘制选中BundleName详细信息
            _bundleNamePanelDetailScroll = DrawBundleNameDetailPanel(_selectedProjectBundleName,
                _bundleNamePanelDetailScroll);
        }

        private Vector2 DrawBundleNameDetailPanel(string bundleName, Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "HelpBox");
            if (!string.IsNullOrEmpty(bundleName))
            {
                if (_unusedBundleNameSet != null && _unusedBundleNameSet.Contains(bundleName))
                {
                    GUI.color = Color.yellow;
                    GUILayout.Label("该BundleName未在项目中使用");
                    GUI.color = Color.white;
                }
                GUILayout.Label("=====================================");
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                GUILayout.Label("Include Asset Path:" + assetPaths.Length);
                for (var i = 0; i < assetPaths.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("选中", GUILayout.Width(50f)))
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(assetPaths[i]);
                    }
                    GUILayout.Label(assetPaths[i]);
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Label("=====================================");
                if (_buildBundleStrategy != null)
                {
                    if (!_buildBundleStrategy.minResConfig.ContainsKey(bundleName) && _buildBundleStrategy.replaceResConfig.ContainsKey(bundleName))
                    {
                        _buildBundleStrategy.replaceResConfig[bundleName] = EditorGUILayout.TextField("replaceResKey:",
                            _buildBundleStrategy.replaceResConfig[bundleName]);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            return scrollPos;
        }

#endregion

#region PatchInfo Panel

        private ResPatchInfo _selectedPatchInfo;
        private List<ResPatchInfo> _patchInfoList = new List<ResPatchInfo>();
        private Vector2 _patchInfoListScrollPos;
        private Vector2 _patchInfoPanelScrollPos;
        private string _cdnRoot = "";
        private CDNPlatformType _selectedCdnPlatformType;

        public enum CDNPlatformType
        {
            Andoird,
            IOS,
            RootIOS,
            Win
        }

        private void DrawPatchInfoPanel()
        {
            GUILayout.BeginVertical(GUILayout.MinHeight(300f));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("打开PatchInfo目录", "LargeButton", GUILayout.Height(20f)))
            {
                OpenDirectory(GetPatchInfoRoot());
            }
            if (GUILayout.Button("清空PatchInfo目录", "LargeButton", GUILayout.Height(20f)))
            {
                if (EditorUtility.DisplayDialog("提示", "清空PatchInfo目录,请确认?", "确定", "取消"))
                {
                    FileHelper.DeleteDirectory(GetPatchInfoRoot(), true);
                }
            }
            EditorGUILayout.EndHorizontal();
            _selectedCdnPlatformType =
                (CDNPlatformType)EditorGUILayout.EnumPopup("CDNRegion:", _selectedCdnPlatformType);
            _cdnRoot = EditorGUILayout.TextField("CDNRoot:", _cdnRoot);

            if (_patchInfoList != null && _patchInfoList.Count > 0)
            {
                _patchInfoListScrollPos = EditorGUILayout.BeginScrollView(_patchInfoListScrollPos);
                for (int i = 0; i < _patchInfoList.Count; ++i)
                {
                    ResPatchInfo patchInfo = _patchInfoList[i];
                    if (patchInfo != null)
                    {
                        GUILayout.Space(-1f);
                        GUI.backgroundColor = _selectedPatchInfo == patchInfo
                            ? Color.white
                            : new Color(0.8f, 0.8f, 0.8f);

                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

                        GUI.backgroundColor = Color.white;
                        GUILayout.Label(i.ToString(), GUILayout.Width(40f));

                        string content = patchInfo.ToFileName() +
                            (patchInfo.CurVer == patchInfo.EndVer ? "(当前版本PatchInfo)" : "");
                        if (GUILayout.Button(content, "OL TextField", GUILayout.Height(20f)))
                        {
                            _selectedPatchInfo = patchInfo;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Box("PatchInfoList is null");
            }
            GUILayout.EndVertical();

            if (_selectedPatchInfo != null)
            {
                _patchInfoPanelScrollPos = EditorGUILayout.BeginScrollView(_patchInfoPanelScrollPos);
                GUILayout.Label(
                    string.Format("CurVer:{0}\nEndVer:{1}\nCurLz4CRC:{2}\nCurLzmaCRC:{3}\nCurTexCRC:{4}\nEndLz4CRC:{5}\nEndLzmaCRC:{6}\nEndTexCRC:{7}\nTotalFileSize:{8}",
                        _selectedPatchInfo.CurVer,
                        _selectedPatchInfo.EndVer,
                        _selectedPatchInfo.CurLz4CRC,
                        _selectedPatchInfo.CurLzmaCRC,
                        _selectedPatchInfo.CurTexCRC,
                        _selectedPatchInfo.EndLz4CRC,
                        _selectedPatchInfo.EndLzmaCRC,
                        _selectedPatchInfo.EndTexCRC,
                        EditorUtility.FormatBytes(_selectedPatchInfo.TotalFileSize))
                    );
                GUILayout.Label(string.Format("更新列表:{0}", _selectedPatchInfo.updateList.Count), "WhiteLargeLabel");
                for (int i = 0; i < _selectedPatchInfo.updateList.Count; i++)
                {
                    string info = string.Format("{0} {1}", i, _selectedPatchInfo.updateList[i].bundleName);
                    GUILayout.Label(info);
                }

                GUILayout.Label(string.Format("删除列表:{0}", _selectedPatchInfo.removeList.Count), "WhiteLargeLabel");
                for (int i = 0; i < _selectedPatchInfo.removeList.Count; i++)
                {
                    string info = string.Format("{0} {1}", i, _selectedPatchInfo.removeList[i]);
                    GUILayout.Label(info);
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("生成当前Patch的Url清单", GUILayout.Height(50f)))
                {
                    GeneratePatchInfoUrlFile(_selectedPatchInfo, _cdnRoot, _selectedCdnPlatformType.ToString().ToLower());
                }
                if (GUILayout.Button("生成当前Patch需要的资源", GUILayout.Height(50f)))
                {
                    if (EditorUtility.DisplayDialog("提示", "导出版本更新资源到patch_resources目录", "确定", "取消"))
                    {
                        GeneratePatchRes(_selectedPatchInfo);
                    }
                }
            }
        }

#endregion

#region MinResList Panel

        private Vector2 _minResPanelScrollPos;
        private Vector2 _minResPanelDetailScrollPos;
        private bool _minResFoldOut;
        private bool _replaceResFoldOut;
        private string _selectedMiniResBundleName;
        private string _minResSearchFilter = "";

        private void DrawMinResListPanel()
        {
            if (_buildBundleStrategy == null) return;
            // Search field
            GUILayout.BeginHorizontal();
            {
                var after = EditorGUILayout.TextField("", _minResSearchFilter, "SearchTextField");

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = "";
                    GUIUtility.keyboardControl = 0;
                }

                if (_minResSearchFilter != null && _minResSearchFilter != after)
                {
                    _minResSearchFilter = after;
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(300f));
            {
                _minResPanelScrollPos = EditorGUILayout.BeginScrollView(_minResPanelScrollPos);

                _minResFoldOut = EditorGUILayout.Foldout(_minResFoldOut,
                    "小包必备资源列表: " + _buildBundleStrategy.minResConfig.Count);
                if (_minResFoldOut)
                {
                    int index = 0;
                    foreach (string bundleName in _buildBundleStrategy.minResConfig.Keys)
                    {
                        if (!string.IsNullOrEmpty(_minResSearchFilter) && bundleName.IndexOf(_minResSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        GUILayout.Space(-1f);
                        GUI.backgroundColor = _selectedMiniResBundleName == bundleName
                            ? Color.white
                            : new Color(0.8f, 0.8f, 0.8f);

                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

                        GUI.backgroundColor = Color.white;
                        GUILayout.Label(index++.ToString(), GUILayout.Width(40f));

                        if (GUILayout.Button(bundleName, "OL TextField", GUILayout.Height(20f)))
                        {
                            _selectedMiniResBundleName = bundleName;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                _replaceResFoldOut = EditorGUILayout.Foldout(_replaceResFoldOut,
                    "小包替代资源信息:" + _buildBundleStrategy.replaceResConfig.Count);
                if (_replaceResFoldOut)
                {
                    int index = 0;
                    foreach (var pair in _buildBundleStrategy.replaceResConfig)
                    {
                        string bundleName = pair.Key;
                        if (!string.IsNullOrEmpty(_minResSearchFilter) && bundleName.IndexOf(_minResSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        GUILayout.Space(-1f);
                        GUI.backgroundColor = _selectedMiniResBundleName == bundleName
                            ? Color.white
                            : new Color(0.8f, 0.8f, 0.8f);

                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

                        GUI.backgroundColor = Color.white;
                        GUILayout.Label(index++.ToString(), GUILayout.Width(40f));

                        if (GUILayout.Button(bundleName + "  ==>  " + pair.Value, "OL TextField", GUILayout.Height(20f)))
                        {
                            _selectedMiniResBundleName = bundleName;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清空MinRes资源列表", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("确认", "是否清空MinRes资源列表？", "清空", "取消"))
                {
                    _buildBundleStrategy.minResConfig.Clear();
                    SaveBuildBundleStrategy();
                }
            }
            if (GUILayout.Button("刷新小包资源配置", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("确认", "是否刷新小包资源配置？", "刷新", "取消"))
                {
                    RefreshBundleNameData(true);
                }
            }
            if (GUILayout.Button("保存小包资源配置", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("确认", "是否保存小包资源配置？", "保存", "取消"))
                {
                    SaveBuildBundleStrategy();
                }
            }
            EditorGUILayout.EndHorizontal();

            //绘制选中ResInfo信息
            _minResPanelDetailScrollPos = DrawBundleNameDetailPanel(_selectedMiniResBundleName,
                _minResPanelDetailScrollPos);
        }

#endregion

#region 标记项目资源AssetBundle名

        private void CleanUpBundleName(bool cleanAll)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            if (cleanAll)
            {
                var allBundleNames = AssetDatabase.GetAllAssetBundleNames();
                for (int i = 0; i < allBundleNames.Length; i++)
                {
                    var bundleName = allBundleNames[i];
                    AssetDatabase.RemoveAssetBundleName(bundleName, true);
                    if (_showPrograss)
                        EditorUtility.DisplayProgressBar("移除所有资源BundleName中", string.Format(" {0} / {1} ", i, allBundleNames.Length),
                            i / (float)allBundleNames.Length);
                }
                if (_showPrograss)
                    EditorUtility.ClearProgressBar();
            }

            RefreshBundleNameData();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("确认", cleanAll ? "清空所有资源BundleName成功" : "清空未使用的BundleName成功", "Yes");
        }

        private readonly StringBuilder _bundleNameLogger = new StringBuilder();
        public void UpdateAllBundleName()
        {
            CustomAssetTool.ClearBundleNameByConfig();  //更新前先清理不需要使用的
            _bundleNameLogger.Length = 0;
            var stopwatch = Stopwatch.StartNew();
            AssetDatabase.RemoveUnusedAssetBundleNames();
            using (var a = new EnableBundleNameException())
            {
                var uiCount = (_updateResGroupMask & UpdateBundleFlag.UI) != UpdateBundleFlag.UI ? -1 : UpdateUI();
                var modelCount = (_updateResGroupMask & UpdateBundleFlag.Model) != UpdateBundleFlag.Model ? -1 : UpdateModel();
                var effCount = (_updateResGroupMask & UpdateBundleFlag.Effect) != UpdateBundleFlag.Effect ? -1 : UpdateEffect();
                var sceneCount = (_updateResGroupMask & UpdateBundleFlag.Scene) != UpdateBundleFlag.Scene ? -1 : UpdateScene();
                var audioCount = (_updateResGroupMask & UpdateBundleFlag.Audio) != UpdateBundleFlag.Audio ? -1 : UpdateAudio();
                var configCount = (_updateResGroupMask & UpdateBundleFlag.Config) != UpdateBundleFlag.Config ? -1 : UpdateConfig();
                var scriptCount = (_updateResGroupMask & UpdateBundleFlag.Script) != UpdateBundleFlag.Script ? -1 : UpdateScript();
                var streamSceneCount = (_updateResGroupMask & UpdateBundleFlag.StreamScene) != UpdateBundleFlag.StreamScene ? -1 : UpdateStreamScene();

                //最后才更新公共资源的BundleName,防止被前面流程覆盖掉
                var commonCount = UpdateCommon();

                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                var tips = string.Format(
                        "资源BundleName变更数量\n-1表示跳过该组资源检查\nCommon：{0}\nUI：{1}\nModel：{2}\nEffect：{3}\nScene：{4}\nAudio:{5}\nConfig:{6}\nScript:{7}\nStreamScene:{8}",
                        commonCount, uiCount, modelCount, effCount, sceneCount, audioCount, configCount, scriptCount, streamSceneCount);
                string desc = string.Format("更新项目资源的BundleName总耗时:{0:00}:{1:00}:{2:00}:{3:00}\n", elapsed.Hours, elapsed.Minutes,
                       elapsed.Seconds, elapsed.Milliseconds / 10) + tips;
                if (!_slientMode)
                {
                    EditorUtility.DisplayDialog("提示", desc, "OK");
                }
                else
                {
                    Debug.Log(desc);
                }
            }

            if (_bundleNameLogger.Length > 0)
            {
                Debug.Log(_bundleNameLogger);
            }
            AssetDatabase.Refresh();
            RefreshBundleNameData(true);
        }

        /// <summary>
        ///     Common类型资源,开始游戏前全部加载进游戏中
        /// </summary>
        /// <returns></returns>
        private int UpdateCommon()
        {
            int changeCount = 0;
            //更新公共资源BundleName
            foreach (string filePath in BuildBundlePath.CommonFilePath)
            {
                var importer = AssetImporter.GetAtPath(filePath);
                if (importer != null)
                {
                    if (importer.UpdateBundleName(importer.GetAssetBundleName(ResGroup.Common)))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update Common BundleName:" + filePath);
                    }
                }
            }

            //更新Shader的BundleName
            foreach (string filePath in BuildBundlePath.ShaderFilePath)
            {
                var importer = AssetImporter.GetAtPath(filePath);
                if (importer != null)
                {
                    if (importer.UpdateBundleName(GameResPath.AllShaderBundleName))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update CustomShader BundleName:" + filePath);
                    }
                }
            }

            var GUIDs = AssetDatabase.FindAssets("t:shader", BuildBundlePath.ShaderFolder);
            for (int i = 0; i < GUIDs.Length; i++)
            {
                var shaderPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                var shaderImporter = AssetImporter.GetAtPath(shaderPath);
                if (shaderImporter.UpdateBundleName(GameResPath.AllShaderBundleName))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update CustomShader BundleName:" + shaderPath);
                }
            }
            return changeCount;
        }

        /// <summary>
        /// 分析所有UI资源信息,并设置BundleName
        /// UI资源目录结构规范示例
        /// UI
        ///     Atlas/
        ///         CommonUIAtlas.prefab
        ///     Fonts/
        ///         CommonFont.prefab
        ///     Prefabs/
        ///         BaseDialogue.prefab
        ///     Images/
        ///         dialogue_bg.png
        /// </summary>
        /// <returns></returns>
        private int UpdateUI()
        {
            var changeCount = 0;
            var GUIDs = AssetDatabase.FindAssets("t:Prefab", BuildBundlePath.UIAtlasFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                string resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                if (resPath.IsCommonAsset()) continue;

                var prefabImporter = AssetImporter.GetAtPath(resPath);
                if (prefabImporter.UpdateBundleName(prefabImporter.GetAssetBundleName(ResGroup.UIAtlas)))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update UIAtlas BundleName:" + resPath);
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理UIAtlas中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }

            GUIDs = AssetDatabase.FindAssets("t:Prefab", BuildBundlePath.UIFontFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                string resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                if (resPath.IsCommonAsset()) continue;

                var prefabImporter = AssetImporter.GetAtPath(resPath);
                //MyFont相关资源需要特殊处理,加上表情图集一起打包
                if (resPath.StartsWith("Assets/UI/Fonts/MyFont/"))
                {
                    var dependencies = AssetDatabase.GetDependencies(resPath);
                    foreach (string refPath in dependencies)
                    {
                        if (refPath.StartsWith("Assets/UI/Fonts/MyFont/"))
                        {
                            var importer = AssetImporter.GetAtPath(refPath);
                            if (importer.UpdateBundleName(AssetManager.GetBundleName("MyFont1", ResGroup.UIFont)))
                            {
                                changeCount++;
                                _bundleNameLogger.AppendLine("Update UIFont BundleName:" + refPath);
                            }
                        }
                    }
                }
                else
                {
                    if (prefabImporter.UpdateBundleName(prefabImporter.GetAssetBundleName(ResGroup.UIFont)))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update UIFont BundleName:" + resPath);
                    }
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理UIFont中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }

            GUIDs = AssetDatabase.FindAssets("t:Prefab", BuildBundlePath.UIPrefabFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                string resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                if (resPath.IsCommonAsset()) continue;

                var prefabImporter = AssetImporter.GetAtPath(resPath);
                if (prefabImporter.UpdateBundleName(prefabImporter.GetAssetBundleName(ResGroup.UIPrefab)))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update UIPrefab BundleName:" + resPath);
                }

                //处理UIPrefab依赖关系
                var dependencies = AssetDatabase.GetDependencies(resPath, false);
                for (var j = 0; j < dependencies.Length; j++)
                {
                    var refPath = dependencies[j];
                    if (refPath.IsCommonAsset()) continue;

                    var refImporter = AssetImporter.GetAtPath(refPath);
                    if (refPath.IsTextureFile())
                    {
                        //UIPrefab中引用到的贴图都要统一放在CommonTextures目录下
                        if (refPath.IsGameUITexture())
                        {
                            if (refImporter.UpdateBundleName(refImporter.GetAssetBundleName(ResGroup.UITexture)))
                            {
                                changeCount++;
                                _bundleNameLogger.AppendLine("Update UITexture BundleName:" + refPath);
                            }
                        }
                        else
                        {
                            Debug.LogError(string.Format("<{0}> UIPrefab引用到的图片需要放到BuildBundlePath.UITextureFolder目录下，请检查:{1}", resPath, refPath));
                        }
                    }
                    else if (refPath.IsAudioFile())
                    {
                        Debug.LogError(string.Format("<{0}> 中包含了音频资源，UIPrefab中不应包含除Common资源外的其他音频资源，请检查",
                            refPath));
                    }
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理UIPrefab中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }

            int index = 0;
            //代码动态加载的图片列表
            foreach (string imagePath in BuildBundlePath.ImageFilePath)
            {
                var importer = AssetImporter.GetAtPath(imagePath);
                if (importer != null)
                {
                    if (importer.UpdateBundleName(importer.GetAssetBundleName(ResGroup.Image)))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update Image BundleName:" + imagePath);
                    }
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理Image资源中",
                        string.Format(" {0} / {1} ", index, BuildBundlePath.ImageFilePath.Count),
                        index / (float)BuildBundlePath.ImageFilePath.Count);

                index++;
            }

            //代码动态加载图片目录
            GUIDs = AssetDatabase.FindAssets("t:Texture", BuildBundlePath.ImageFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                var resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                var importer = AssetImporter.GetAtPath(resPath);
                if (importer != null)
                {
                    if (importer.UpdateBundleName(importer.GetAssetBundleName(ResGroup.Image)))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update Image BundleName:" + resPath);
                    }
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理Images目录中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        /// <summary>
        /// 分析模型相关资源,并设置BundleName
        /// 模型资源目录规范示例
        /// pet_11
        ///     Materials/
        ///         pet_11.mat
        ///         pet_11_mutate.mat
        ///     Meshes/
        ///         pet_11.fbx
        ///     Prefabs/
        ///         pet_11.prefab
        ///     Textures/
        ///         pet_11.png
        ///         pet_11_mask.png
        ///         pet_11_mutate.png
        ///     Animation/
        ///         anim_11.overrideController
        ///         anim_11@attack1.fbx
        ///         anim_11@attack2.fbx
        ///         anim_11@idle.fbx
        /// </summary>
        /// <returns></returns>
        private int UpdateModel()
        {
            var changeCount = 0;
            //处理模型资源
            var GUIDs = AssetDatabase.FindAssets("t:Prefab", BuildBundlePath.ModelPrefabFolder);
            for (var modelIndex = 0; modelIndex < GUIDs.Length; modelIndex++)
            {
                var modelPath = AssetDatabase.GUIDToAssetPath(GUIDs[modelIndex]);
                var prefabImporter = AssetImporter.GetAtPath(modelPath);
                string prefabBundleName = prefabImporter.GetAssetBundleName(ResGroup.Model);
                if (prefabImporter.UpdateBundleName(prefabBundleName))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update Model BundleName:" + modelPath);
                }

                var modelName = modelPath.ExtractResName();
                //只对pet_或ride_pet_为前缀的模型根据依赖关系打包，其它模型全部资源打成一个包
                if (modelName.StartsWith("pet_") || modelName.StartsWith("ride_pet_"))
                {
                    //模型关联材质统一打包
                    string matDir = Path.GetDirectoryName(modelPath).Replace("/Prefabs", "/Materials");
                    if (Directory.Exists(matDir))
                    {
                        var matGUIDs = AssetDatabase.FindAssets("t:Material", new[] { matDir });
                        if (matGUIDs.Length > 0)
                        {
                            foreach (var matGUID in matGUIDs)
                            {
                                string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                                var matImporter = AssetImporter.GetAtPath(matPath);
                                if (matImporter.UpdateBundleName(prefabBundleName + "_mat"))
                                {
                                    changeCount++;
                                    _bundleNameLogger.AppendLine("Update ModelMat BundleName:" + matPath);
                                }

                                //设置模型材质关联Shader的BundleName
                                var matDependencies = AssetDatabase.GetDependencies(matPath, false);
                                foreach (string refPath in matDependencies)
                                {
                                    if (refPath.IsShaderFile())
                                    {
                                        var shaderImporter = AssetImporter.GetAtPath(refPath);
                                        if (shaderImporter.UpdateBundleName(GameResPath.AllShaderBundleName))
                                        {
                                            changeCount++;
                                            _bundleNameLogger.AppendLine("Update ModelShader BundleName:" + refPath);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("当前模型关联材质数量为0,请检查:" + modelName);
                        }
                    }
                    else
                    {
                        if(matDir.Contains("RoleCreate") == false)
                            Debug.LogError("当前模型材质目录命名异常,请检查:" + matDir);
                    }

                    //设置模型关联动画资源的BundleName
                    var modelDependencies = AssetDatabase.GetDependencies(modelPath, false);
                    foreach (string refPath in modelDependencies)
                    {
                        if (refPath.EndsWith(".overrideController"))
                        {
                            //筛选出pet模型动画文件单独打包
                            var animImporter = AssetImporter.GetAtPath(refPath);
                            if (animImporter.UpdateBundleName(animImporter.GetAssetBundleName(ResGroup.Model)))
                            {
                                changeCount++;
                                _bundleNameLogger.AppendLine("Update ModelAnim BundleName:" + refPath);
                            }
                        }
                    }
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理模型资源中", string.Format(" {0} / {1} ", modelIndex, GUIDs.Length),
                        modelIndex / (float)GUIDs.Length);
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        private int UpdateEffect()
        {
            var changeCount = 0;
            var GUIDs = AssetDatabase.FindAssets("t:Prefab", BuildBundlePath.EffectPrefabFolder);
            for (var effIndex = 0; effIndex < GUIDs.Length; effIndex++)
            {
                var effPath = AssetDatabase.GUIDToAssetPath(GUIDs[effIndex]);
                var prefabImporter = AssetImporter.GetAtPath(effPath);
                if (prefabImporter.UpdateBundleName(prefabImporter.GetAssetBundleName(ResGroup.Effect)))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update Effect BundleName:" + effPath);
                }

                //处理特效资源材质依赖关系
                var dependencies = AssetDatabase.GetDependencies(effPath);
                for (var effMatIndex = 0; effMatIndex < dependencies.Length; effMatIndex++)
                {
                    var refPath = dependencies[effMatIndex];
                    if (refPath.IsShaderFile())
                    {
                        var shaderImporter = AssetImporter.GetAtPath(refPath);
                        if (shaderImporter.UpdateBundleName(GameResPath.AllShaderBundleName))
                        {
                            changeCount++;
                            _bundleNameLogger.AppendLine("Update EffectShader BundleName:" + refPath);
                        }
                    }
                }
                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理特效资源中", string.Format(" {0} / {1} ", effIndex, GUIDs.Length),
                        effIndex / (float)GUIDs.Length);
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        private int UpdateScene()
        {
            var changeCount = 0;
            //处理场景资源
            var buildSceneList = EditorBuildSettings.scenes;
            for (var sceneIndex = 0; sceneIndex < buildSceneList.Length; sceneIndex++)
            {
                var buildSceneInfo = buildSceneList[sceneIndex];
                if (!buildSceneInfo.path.EndsWith("GameStartScene.unity"))
                {
                    var scenePath = buildSceneInfo.path;
                    var sceneImporter = AssetImporter.GetAtPath(scenePath);
                    if (sceneImporter != null)
                        GameDebuger.LogError("scenePath ----" + scenePath);
                    else
                        GameDebuger.LogError("sceneImporter != null scenePath ----" + scenePath);
                    if (sceneImporter.UpdateBundleName(sceneImporter.GetAssetBundleName(ResGroup.Scene)))
                    {
                        changeCount++;
                        _bundleNameLogger.AppendLine("Update Scene BundleName:" + scenePath);
                    }

                    //处理场景引用到Shader资源信息
                    var dependencies = AssetDatabase.GetDependencies(scenePath);
                    for (var refIndex = 0; refIndex < dependencies.Length; ++refIndex)
                    {
                        var refPath = dependencies[refIndex];
                        if (refPath.IsShaderFile())
                        {
                            var shaderImporter = AssetImporter.GetAtPath(refPath);
                            if (shaderImporter.UpdateBundleName(GameResPath.AllShaderBundleName))
                            {
                                changeCount++;
                                _bundleNameLogger.AppendLine("Update SceneShader BundleName:" + refPath);
                            }
                        }
                    }
                    if (_showPrograss)
                        EditorUtility.DisplayProgressBar("处理场景资源中",
                            string.Format(" {0} / {1} ", sceneIndex, buildSceneList.Length),
                            sceneIndex / (float)buildSceneList.Length);
                }
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        private int UpdateStreamScene()
        {
            var changeCount = 0;
            var streamSceenPrefabs = AssetDatabase.FindAssets("t:prefab", BuildBundlePath.StreamScenes)
                .Select(item => AssetDatabase.GUIDToAssetPath(item))
                .Where(item => item.Contains("/Build/Prefabs/"))
                .ToArray();
            for (int index = 0; index < streamSceenPrefabs.Length; index++)
            {
                var prefabPath = streamSceenPrefabs[index];
                UpdateStreamSceneBySingle(prefabPath, ref changeCount, _bundleNameLogger);
                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理流加载场景资源中", string.Format(" {0} / {1} ", index, streamSceenPrefabs.Length), (float)index / streamSceenPrefabs.Length);
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        public static string UpdateStreamSceneBySingle(string prefabPath, ref int changeCount, StringBuilder logger = null)
        {
            var prefabImporter = AssetImporter.GetAtPath(prefabPath);
            string prefabBundleName = prefabImporter.GetAssetBundleName(ResGroup.StreamScene);
            if (prefabImporter.UpdateBundleName(prefabBundleName))
            {
                changeCount++;
                if(logger != null)
                    logger.AppendLine("Update StreamScenePrefab BundleName:" + prefabImporter);
            }
            //处理依赖
            var dependencies =
                EditorUtility.CollectDependencies(new Object[]
                {AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath)})
                    .Select(item => AssetDatabase.GetAssetPath(item))
                    .ToArray();
            for (var refIndex = 0; refIndex < dependencies.Length; ++refIndex)
            {
                var refPath = dependencies[refIndex];
                if (refPath.IsShaderFile())
                {
                    var shaderImporter = AssetImporter.GetAtPath(refPath);
                    if (shaderImporter.UpdateBundleName(GameResPath.AllShaderBundleName))
                    {
                        changeCount++;
                        if (logger != null)
                            logger.AppendLine("Update StreamSceneShader BundleName:" + refPath);
                    }
                }
                else if (refPath.IsMaterial() || refPath.IsTextureFile())
                {
                    var refAssetImproter = AssetImporter.GetAtPath(refPath);
                    if (refAssetImproter.UpdateBundleName(refAssetImproter.GetAssetBundleName(ResGroup.StreamScene)))
                    {
                        changeCount++;
                        if (logger != null)
                            logger.AppendLine("Update StreamSceneRef BundleName:" + refPath);
                    }
                }
            }

            return prefabBundleName;
        }

        private int UpdateAudio()
        {
            var changeCount = 0;
            //处理音频资源
            var GUIDs = AssetDatabase.FindAssets("t:AudioClip", BuildBundlePath.AudioFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                var resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                if (resPath.IsCommonAsset()) continue;
                var audioImporter = AssetImporter.GetAtPath(resPath);
                if (audioImporter.UpdateBundleName(audioImporter.GetAssetBundleName(ResGroup.Audio)))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update Audio BundleName:" + resPath);
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理音频资源中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        private int UpdateConfig()
        {
            var changeCount = 0;
            //处理配置文件资源
            var GUIDs = AssetDatabase.FindAssets("t:TextAsset t:ScriptableObject", BuildBundlePath.ConfigFolder);
            for (var i = 0; i < GUIDs.Length; i++)
            {
                var resPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                var configImporter = AssetImporter.GetAtPath(resPath);
                if (configImporter.UpdateBundleName(configImporter.GetAssetBundleName(ResGroup.Config)))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update Config BundleName:" + resPath);
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理配置文件中", string.Format(" {0} / {1} ", i, GUIDs.Length),
                        i / (float)GUIDs.Length);
            }

            //处理静态数据文件
#if ENABLE_JSB
            var staticDataFiles = Directory.GetFiles("Assets/GameResources/StaticData", "*.jsz.bytes");
#else
            var staticDataFiles = Directory.GetFiles("Assets/GameResources/StaticData", "*.pbz.bytes");
#endif
            for (int i = 0; i < staticDataFiles.Length; i++)
            {
                string file = staticDataFiles[i];
                var importer = AssetImporter.GetAtPath(file);
                if (importer != null && importer.UpdateBundleName("config/allstaticdata"))
                {
                    changeCount++;
                    _bundleNameLogger.AppendLine("Update Config BundleName:" + importer.assetPath);
                }

                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("处理静态数据中", string.Format(" {0} / {1} ", i, staticDataFiles.Length),
                        i / (float)staticDataFiles.Length);
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            return changeCount;
        }

        private int UpdateScript()
        {
            var changeCount = 0;
#if UNITY_IPHONE
            string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#elif UNITY_ANDROID
            var symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#else
            string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#endif
            if (symbolsDefines.Contains("ENABLE_JSB"))
            {
                var minifyGUIDs = AssetDatabase.FindAssets(".min t:TextAsset", BuildBundlePath.ScriptFolder);
                var minifyFileSet = new HashSet<string>();
                for (var i = 0; i < minifyGUIDs.Length; i++)
                {
                    var guid = minifyGUIDs[i];
                    var fileName = AssetDatabase.GUIDToAssetPath(guid).ExtractResName();
                    if (fileName != null)
                        minifyFileSet.Add(fileName.Replace(".min", ""));
                }

                var allGUIDs = AssetDatabase.FindAssets("t:TextAsset", BuildBundlePath.ScriptFolder);
                for (var i = 0; i < allGUIDs.Length; i++)
                {
                    var filePath = AssetDatabase.GUIDToAssetPath(allGUIDs[i]);
                    var fileName = filePath.ExtractResName();
                    var importer = AssetImporter.GetAtPath(filePath);
                    if (fileName.Contains(".min"))
                    {
                        if (importer.UpdateBundleName(GameResPath.AllScriptBundleName))
                        {
                            changeCount++;
                            _bundleNameLogger.AppendLine("Update Script BundleName:" + filePath);
                        }
                    }
                    else if (!minifyFileSet.Contains(fileName))
                    {
                        if (importer.UpdateBundleName(GameResPath.AllScriptBundleName))
                        {
                            changeCount++;
                            _bundleNameLogger.AppendLine("Update Script BundleName:" + filePath);
                        }
                    }
                    else
                    {
                        importer.assetBundleName = "";
                    }
                }
            }
            else
            {
                //非JSB模式下清空所有脚本资源BundleName,防止打包冗余资源
                AssetDatabase.RemoveAssetBundleName(GameResPath.AllScriptBundleName, true);
                //var allGUIDs = AssetDatabase.FindAssets("t:TextAsset", BuildBundlePath.ScriptFolder);
                //for (var i = 0; i < allGUIDs.Length; i++)
                //{
                //    var filePath = AssetDatabase.GUIDToAssetPath(allGUIDs[i]);
                //    var importer = AssetImporter.GetAtPath(filePath);
                //    importer.assetBundleName = "";
                //}
            }

            return changeCount;
        }

#endregion

#region Build AssetBundle

        public void BuildAll(long nextVer, bool generateTotalRes = false)
        {
            var stopwatch = Stopwatch.StartNew();
            if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)    //安卓下进行alpha分离
                MaterialTextureForETC1.StartDepart();
            var exportDir = GetExportBundlePath();

            //根据已设置好的BundleName信息生成AssetBundleBuild列表
            List<AssetBundleBuild> lz4ResList;
            List<AssetBundleBuild> lzmaResList;
#if BUNDLE_APPEND_HASH
            var lz4Options = BuildAssetBundleOptions.AppendHashToAssetBundleName |
                             BuildAssetBundleOptions.ChunkBasedCompression;
            var lzmaOptions = BuildAssetBundleOptions.AppendHashToAssetBundleName;
#else
            var lz4Options = BuildAssetBundleOptions.ChunkBasedCompression;
            var lzmaOptions = BuildAssetBundleOptions.None;
#endif

            GenerateAssetBundleBuildList(out lz4ResList, out lzmaResList);
            //先打包UI资源,使用LZ4压缩方式打包
            BuildBundles(exportDir + "/lz4", lz4ResList, lz4Options);
            //打包其他资源,使用LZMA压缩方式打包
            BuildBundles(exportDir + "/lzma", lzmaResList, lzmaOptions);
            //打包TileMap
            uint allTexCRC32 = JPGTexTool.BuildTexture(exportDir); 
            //生成该版本ResConfig成功后才备份该版本资源
            bool skip;
            var newResConfig = GenerateResConfig(exportDir, nextVer, allTexCRC32, out skip);
            if (newResConfig != null)
            {
                BackupAssetBundle(newResConfig, exportDir);
            }
            else if (_slientMode && !skip)
            {
                throw new SystemException("生成 newResConfig失败");
            }
            if (generateTotalRes)
                GenerateTotalRes();
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            if (!_slientMode)
            {
                EditorUtility.DisplayDialog("提示",
                    string.Format("打包项目资源总耗时:{0:00}:{1:00}:{2:00}:{3:00}\n", elapsed.Hours, elapsed.Minutes,
                        elapsed.Seconds, elapsed.Milliseconds / 10), "OK");
            }
            else
            {
                Debug.Log(string.Format("打包项目资源总耗时:{0:00}:{1:00}:{2:00}:{3:00}\n", elapsed.Hours, elapsed.Minutes,
                    elapsed.Seconds, elapsed.Milliseconds / 10));
            }
        }

        private void GenerateAssetBundleBuildList(out List<AssetBundleBuild> lz4ResList, out List<AssetBundleBuild> lzmaResList)
        {
            lz4ResList = new List<AssetBundleBuild>();
            lzmaResList = new List<AssetBundleBuild>();
            foreach (var pair in _projectBundleNameGroups)
            {
                var resGroup = pair.Key;
                if(resGroup == ResGroup.TileMap)    //只用于编辑器下加载
                    continue;
                if (resGroup == ResGroup.Common)
                {
                    var bundleNames = pair.Value;
                    foreach (string bundleName in bundleNames)
                    {
                        var abb = new AssetBundleBuild
                        {
                            assetBundleName = bundleName,
                            assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                        };
                        lz4ResList.Add(abb);
                        lzmaResList.Add(abb);
                    }
                }
                else if (resGroup == ResGroup.UIPrefab
                         || resGroup == ResGroup.UIAtlas
                         || resGroup == ResGroup.UIFont
                         || resGroup == ResGroup.UITexture
                         || resGroup == ResGroup.StreamScene)
                {
                    var bundleNames = pair.Value;
                    foreach (string bundleName in bundleNames)
                    {
                        var abb = new AssetBundleBuild
                        {
                            assetBundleName = bundleName,
                            assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                        };
                        lz4ResList.Add(abb);
                    }
                }
                else
                {
                    var bundleNames = pair.Value;
                    foreach (string bundleName in bundleNames)
                    {
                        var abb = new AssetBundleBuild
                        {
                            assetBundleName = bundleName,
                            assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                        };
                        lzmaResList.Add(abb);
                    }
                }
            }
        }

        private void BuildBundles(string exportDir, List<AssetBundleBuild> abbList, BuildAssetBundleOptions buildOptions)
        {
            CreateDirectory(exportDir);
            BuildPipeline.BuildAssetBundles(exportDir, abbList.ToArray(), buildOptions, EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        /// 根据Unity打包生成的manifest信息生成自定义的ResConfig信息
        /// </summary>
        private ResConfig GenerateResConfig(string exportDir, long nextVer, uint allTexCRC32, out bool skip)
        {
            skip = false;
            string lz4Root = exportDir + "/lz4";
            string lzmaRoot = exportDir + "/lzma";
            string customRoot = Path.Combine(exportDir, "custom/tilemap");
            string lz4ManifestPath = lz4Root + "/lz4.manifest";
            var lz4Manifest = LoadYAMLObj<RawAssetManifest>(lz4ManifestPath);
            if (lz4Manifest == null)
            {
                Debug.LogError("解析Manifest文件失败:" + lz4ManifestPath);
                return null;
            }

            string lzmaManifestPath = lzmaRoot + "/lzma.manifest";
            var lzmaManifest = LoadYAMLObj<RawAssetManifest>(lzmaManifestPath);
            if (lzmaManifest == null)
            {
                Debug.LogError("解析Manifest文件失败:" + lzmaManifestPath);
                return null;
            }

            //此次打包的资源ManifestCRC与上个版本一致时,询问用户是否跳过
            skip = _curResConfig != null && _curResConfig.lzmaCRC == lzmaManifest.CRC && _curResConfig.lz4CRC == lz4Manifest.CRC && _curResConfig.tileTexCRC == allTexCRC32;
            if (skip && (_slientMode || EditorUtility.DisplayDialog("提示", "本次打包资源ManifestCRC与上次一致,是否跳过？", "跳过", "备份")))
            {
                return null;
            }

            var newResConfig = new ResConfig
            {
                Version = nextVer,
                lz4CRC = lz4Manifest.CRC,
                lzmaCRC = lzmaManifest.CRC,
                tileTexCRC = allTexCRC32,
                BuildTime = DateTimeToUnixTimestamp(DateTime.Now),
            };

            //生成UI资源与Common资源ResInfo信息
            foreach (var bundleInfo in lz4Manifest.Manifest.AssetBundleInfos.Values)
            {
                string bundleName = StripHashSuffix(bundleInfo.Name);
                var bundleManifest = LoadYAMLObj<RawBundleManifest>(lz4Root + "/" + bundleName + ".manifest");
                if (bundleManifest != null)
                {
                    var resInfo = new ResInfo
                    {
                        bundleName = bundleName,
                        CRC = bundleManifest.CRC,
                        Hash = bundleManifest.Hashes["AssetFileHash"].Hash,
                    };
                    UpdatePreloadFlag(resInfo);
                    UpdateRemoteZipType(resInfo);

                    foreach (string dependency in bundleInfo.Dependencies.Values)
                    {
                        //无需记录Common类的资源依赖,因为这部分资源加载了就不释放了
                        if (dependency.StartsWith("common/")) continue;
                        resInfo.Dependencies.Add(StripHashSuffix(dependency));
                    }

                    newResConfig.Manifest.Add(bundleName, resInfo);
                }
                else
                {
                    Debug.LogError("解析BundleManifest文件失败:" + bundleInfo.Name);
                    return null;
                }
            }

            //生成其他资源ResInfo信息
            foreach (var bundleInfo in lzmaManifest.Manifest.AssetBundleInfos.Values)
            {
                string bundleName = StripHashSuffix(bundleInfo.Name);
                if (bundleName.StartsWith("common/")) continue;
                var bundleManifest = LoadYAMLObj<RawBundleManifest>(lzmaRoot + "/" + bundleName + ".manifest");
                if (bundleManifest != null)
                {
                    var resInfo = new ResInfo
                    {
                        bundleName = bundleName,
                        CRC = bundleManifest.CRC,
                        Hash = bundleManifest.Hashes["AssetFileHash"].Hash,
                    };
                    UpdatePreloadFlag(resInfo);
                    UpdateRemoteZipType(resInfo);

                    foreach (string dependency in bundleInfo.Dependencies.Values)
                    {
                        //无需记录Common类的资源依赖,因为这部分资源加载了就不释放了
                        if (dependency.StartsWith("common/")) continue;
                        resInfo.Dependencies.Add(StripHashSuffix(dependency));
                    }

                    newResConfig.Manifest.Add(bundleName, resInfo);
                }
                else
                {
                    Debug.LogError("解析BundleManifest文件失败:" + bundleInfo.Name);
                    return null;
                }
            }
            return newResConfig;
        }

        /// <summary>
        /// 根据资源类型更新preload标记
        /// </summary>
        /// <param name="resInfo"></param>
        private void UpdatePreloadFlag(ResInfo resInfo)
        {
            var resGroup = ResConfig.GetResGroupFromBundleName(resInfo.bundleName);
            if (resGroup == ResGroup.Common)
            {
                resInfo.preload = true;
            }
            else if (_buildBundleStrategy.preloadConfig.ContainsKey(resInfo.bundleName))
            {
                resInfo.preload = true;
            }
        }

        /// <summary>
        /// 默认为CustomZip,实际测试发现部分资源压缩后变化不大,所以不采用Zip压缩处理
        /// </summary>
        /// <param name="resInfo"></param>
        private void UpdateRemoteZipType(ResInfo resInfo)
        {
            var resGroup = ResConfig.GetResGroupFromBundleName(resInfo.bundleName);
            if (resGroup == ResGroup.UIPrefab || resGroup == ResGroup.UIAtlas || resGroup == ResGroup.UIFont ||
                resGroup == ResGroup.UITexture || resGroup == ResGroup.Common || resGroup == ResGroup.StreamScene)
            {
                //不做压缩处理了,统一以Unity原生打包压缩格式来处理
                resInfo.remoteZipType = CompressType.UnityLZ4;
                ////除了UIPrefab,其他UI资源和Common资源都需要用Zip再压缩一遍后再上传CDN,尽量减少资源更新的下载量
                //resInfo.remoteZipType = resGroup == ResGroup.UIPrefab ? CompressType.UnityLZ4 : CompressType.CustomZip;
            }
            else if (resGroup == ResGroup.TileMap)
            {
                resInfo.remoteZipType = CompressType.CustomTex;
            }
            else
            {
                resInfo.remoteZipType = CompressType.UnityLZMA;
            }
        }

        private void BackupAssetBundle(ResConfig newResConfig, string exportDir)
        {
            //打包资源完毕,备份当前版本资源到gameres_{CRC}目录
            try
            {
                string lz4ExportRoot = exportDir + "/lz4";
                string lzmaExportRoot = exportDir + "/lzma";
                string tileMapExportRoot = exportDir + "/custom";

                if (newResConfig != null)
                {
                    var backupDir = GetBackupDir(newResConfig);
                    //先删除之前已存在的资源目录
                    if (FileUtil.DeleteFileOrDirectory(backupDir))
                    {
                        Debug.Log("旧版本资源目录已存在,将清空后重新备份:" + backupDir);
                    }
                    string lz4BackupRoot = backupDir + "/lz4";
                    string lzmaBackupRoot = backupDir + "/lzma";
                    string tileMapBackupRoot = backupDir + "/custom";
                    Directory.CreateDirectory(lz4BackupRoot);
                    Directory.CreateDirectory(lzmaBackupRoot);

                    //先备份AssetBundleManifest信息
                    FileUtil.CopyFileOrDirectory(lz4ExportRoot + "/lz4", lz4BackupRoot + "/lz4");
                    FileUtil.CopyFileOrDirectory(lz4ExportRoot + "/lz4.manifest", lz4BackupRoot + "/lz4.manifest");

                    FileUtil.CopyFileOrDirectory(lzmaExportRoot + "/lzma", lzmaBackupRoot + "/lzma");
                    FileUtil.CopyFileOrDirectory(lzmaExportRoot + "/lzma.manifest", lzmaBackupRoot + "/lzma.manifest");
                    //FileUtil.CopyFileOrDirectory(tileMapExportRoot, tileMapBackupRoot);
                    int finishedCount = 0;
                    //备份该版本资源的Bundle及其manifest文件到备份目录
                    foreach (var resInfo in newResConfig.Manifest.Values)
                    {
                        string bundleExportDir = GetBundleBackupDir(resInfo, exportDir);
                        string bundleBackupDir = GetBundleBackupDir(resInfo, backupDir);

                        var bundleFileInfo = new FileInfo(resInfo.GetExportPath(bundleExportDir));
                        var bundleManifest = resInfo.GetManifestPath(bundleExportDir);
                        if (bundleFileInfo.Exists && File.Exists(bundleManifest))
                        {
                            var backupBundlePath = resInfo.GetABPath(bundleBackupDir);
                            var backupBundleManifest = resInfo.GetManifestPath(bundleBackupDir);
                            Directory.CreateDirectory(Path.GetDirectoryName(backupBundlePath));

                            FileUtil.CopyFileOrDirectory(bundleFileInfo.FullName, backupBundlePath);
                            FileUtil.CopyFileOrDirectory(bundleManifest, backupBundleManifest);

                            //对于压缩类型为CompressType.CustomZip进行压缩备份
                            if (resInfo.remoteZipType == CompressType.CustomZip)
                            {
                                var exportZipPath = resInfo.GetRemotePath(bundleExportDir);
                                if (!File.Exists(exportZipPath))
                                    ZipManager.CompressFile(bundleFileInfo.FullName, exportZipPath);

                                var zipFileInfo = new FileInfo(exportZipPath);
                                if (zipFileInfo.Exists)
                                {
                                    var backupZipPath = resInfo.GetRemotePath(bundleBackupDir);
                                    FileUtil.CopyFileOrDirectory(exportZipPath, backupZipPath);
                                    resInfo.MD5 = MD5Hashing.HashFile(backupZipPath);
                                    resInfo.size = zipFileInfo.Length;
                                }
                                else
                                {
                                    throw new Exception("压缩Bundle异常,请检查:" + bundleFileInfo.FullName);
                                }
                            }
                            else
                            {
                                resInfo.MD5 = MD5Hashing.HashFile(backupBundlePath);
                                resInfo.size = bundleFileInfo.Length;
                            }

                            //统计压缩后该版本资源的总大小
                            newResConfig.TotalFileSize += resInfo.size;
                        }
                        else
                        {
                            throw new Exception(string.Format("打包异常,在打包目录找不到该文件或其Manifest文件: {0} \n ManifestPath:{1} \n bundlePath:{2}", resInfo.bundleName, bundleManifest, bundleFileInfo.FullName));
                        }
                        finishedCount += 1;
                        if (_showPrograss)
                            EditorUtility.DisplayProgressBar("备份AssetBundle中",
                                string.Format(" {0} / {1} ", finishedCount, newResConfig.Manifest.Count),
                                finishedCount / (float)newResConfig.Manifest.Count);
                    }

                    //备份完该版本Bundle资源,保存newResConfig信息
                    string jsonPath = GetResConfigRoot() + "/" + newResConfig.ToFileName();
                    FileHelper.SaveJsonObj(newResConfig, jsonPath, false, true);
                    string jzPath = GetResConfigRoot() + "/" + newResConfig.ToRemoteName();
                    newResConfig.SaveFile(jzPath, true);
                    newResConfig.CheckSelfDependencies();
                    RefreshResConfigData(newResConfig);
                    EditorPrefs.SetString(GetLastResConfigPathHash(), jsonPath);
                }
                else
                {
                    throw new Exception("curResConfig is null");
                }
            }
            catch (Exception e)
            {
                if (_slientMode)
                    throw;
                Debug.LogException(e);
                EditorUtility.DisplayDialog("提示", "备份当前版本资源失败,详情查看Log!!!", "OK");
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();
        }

#endregion

#region 小包资源配置策略

        private static void RemoveMinResKeySetRecursively(string resKey, ResInfo resInfo)
        {
            _buildBundleStrategy.RemoveMinResKey(resKey);
            if (resInfo.Dependencies.Count > 0)
            {
                for (int i = 0; i < resInfo.Dependencies.Count; i++)
                {
                    var refResInfo = _curResConfig.GetResInfo(resInfo.Dependencies[i]);
                    if (refResInfo != null)
                    {
                        RemoveMinResKeySetRecursively(resInfo.Dependencies[i], refResInfo);
                    }
                }
            }
        }

        private static void AddMinResKeySetRecursively(string resKey, ResInfo resInfo)
        {
            _buildBundleStrategy.AddMinResKey(resKey);
            if (resInfo.Dependencies.Count > 0)
            {
                for (int i = 0; i < resInfo.Dependencies.Count; i++)
                {
                    var refResInfo = _curResConfig.GetResInfo(resInfo.Dependencies[i]);
                    if (refResInfo != null)
                    {
                        AddMinResKeySetRecursively(resInfo.Dependencies[i], refResInfo);
                    }
                }
            }
        }

        private static bool IsEssentialResType(ResGroup resGroup)
        {
            return resGroup == ResGroup.Common ||
                   resGroup == ResGroup.UIPrefab || resGroup == ResGroup.UIAtlas || resGroup == ResGroup.UIFont ||
                   resGroup == ResGroup.Image || resGroup == ResGroup.UITexture ||
                   resGroup == ResGroup.Config ||
                   resGroup == ResGroup.Script;
        }

        private static bool IsEssentialResType(ResInfo resInfo)
        {
            var resGroup = ResConfig.GetResGroupFromBundleName(resInfo.bundleName);
            return IsEssentialResType(resGroup);
        }

        private static void SaveBuildBundleStrategy()
        {
            if (_buildBundleStrategy == null) return;
            FileHelper.SaveJsonObj(_buildBundleStrategy, GetBuildBundleStrategyPath(), false, true);
        }

#endregion

#region 生成StreamingAssets资源

        private void GenerateTotalRes()
        {
            GeneratePackageBundle(false);
        }

        private void GenerateMiniRes()
        {
            GeneratePackageBundle(true);
        }

        /// <summary>
        /// 迁移Bundle到StreamingAssets下
        /// </summary>
        private void GeneratePackageBundle(bool isMiniRes)
        {
            if (_curResConfig == null)
            {
                EditorUtility.DisplayDialog("确认", "当前版本信息为空,请重新确认?", "Yes");
                return;
            }

            //先清空StreamingAsset资源目录
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            string packageDir = Application.streamingAssetsPath + "/" + GameResPath.BUNDLE_ROOT;
            Directory.CreateDirectory(packageDir);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var backupDir = GetBackupDir(_curResConfig);
            var finishedCount = 0;
            var bundleCount = _curResConfig.Manifest.Count;
            long totalFileLength = 0L;
            try
            {
                var miniResConfig = isMiniRes ? new MiniResConfig() : null;
                foreach (var resInfo in _curResConfig.Manifest.Values)
                {
                    //小包模式下,只拷贝必需资源到包内
                    resInfo.isPackageRes = true;
                    if (isMiniRes && !_buildBundleStrategy.minResConfig.ContainsKey(resInfo.bundleName))
                    {
                        resInfo.isPackageRes = false;
                        string replaceKey = "";
                        if (!_buildBundleStrategy.replaceResConfig.TryGetValue(resInfo.bundleName, out replaceKey))
                        {
                            Debug.LogError("该BundleName未设置replaceKey,请检查:" + resInfo.bundleName);
                        }
                        miniResConfig.replaceResConfig.Add(resInfo.bundleName, replaceKey);
                    }

                    if (resInfo.isPackageRes)
                    {
                        string bundleBackupDir = GetBundleBackupDir(resInfo, backupDir);
                        var inputFile = resInfo.GetABPath(bundleBackupDir);
                        if (File.Exists(inputFile))
                        {
                            var outputFile = resInfo.GetABPath(packageDir);
                            var dir = Path.GetDirectoryName(outputFile);
                            totalFileLength += resInfo.size;
                            Directory.CreateDirectory(dir);
                            FileUtil.CopyFileOrDirectory(inputFile, outputFile);
                        }
                        else
                        {
                            throw new Exception("生成包内资源异常,不存在该Bundle文件:" + inputFile);
                        }
                    }

                    finishedCount += 1;
                    if (_showPrograss)
                        EditorUtility.DisplayProgressBar("拷贝AssetBundle中",
                            string.Format(" {0} / {1} ", finishedCount, bundleCount), (float)finishedCount / bundleCount);
                }

                if (isMiniRes)
                {
                    //小包模式下,需要生成MiniResConfig到包内
                    FileHelper.SaveJsonObj(miniResConfig,
                        Application.streamingAssetsPath + "/" + GameResPath.MINIRESCONFIG_FILE, true);
                }

                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
                {
                    // 生成一个假信息，如果打包后没有对dll进行处理，则打包出来的不会进行dll更新
                    var dllVersion = new DllVersion()
                    {
                        Version = DllHelper.Max_Versoin,
                    };
                    FileHelper.SaveJsonObj(dllVersion, Application.streamingAssetsPath + "/" + GameResPath.DLLVERSION_FILE);
                }

                string packageResConfigPath = Path.Combine(Application.streamingAssetsPath, GameResPath.RESCONFIG_FILE);
                _curResConfig.isMiniRes = isMiniRes;
                _curResConfig.SaveFile(packageResConfigPath, true);
                ChannelSetting.ChangeTextures();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                if (_showPrograss)
                    EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                return;
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            string hint = string.Format("迁移资源到StreamingAssets耗时:{0:00}:{1:00}:{2:00}:{3:00}\n包内资源大小为:{4}", elapsed.Hours,
                elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds / 10, EditorUtility.FormatBytes(totalFileLength));
            if (!_showPrograss && !_slientMode)
            {
                EditorUtility.DisplayDialog("提示", hint, "Yes");
            }

            Debug.Log(hint);
        }

#endregion

#region 还原备份资源到打包目录
        /// <summary>
        /// 还原指定资源版本号资源到gameres目录中,如果gameres被清空,可从backup中拷贝过来,减少重新打包资源的时间
        /// </summary>
        /// <param name="resConfig"></param>
        private void RevertBackupToGameRes(ResConfig resConfig)
        {
            if (_curResConfig == null)
            {
                EditorUtility.DisplayDialog("确认", "当前版本信息为空,请重新确认?", "Yes");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var exportDir = GetExportBundlePath();
                string lz4ExportRoot = exportDir + "/lz4";
                string lzmaExportRoot = exportDir + "/lzma";
                Directory.CreateDirectory(lz4ExportRoot);
                Directory.CreateDirectory(lzmaExportRoot);

                var backupDir = GetBackupDir(_curResConfig);
                string lz4BackupRoot = backupDir + "/lz4";
                string lzmaBackupRoot = backupDir + "/lzma";

                //先还原AssetBundleManifest信息
                FileUtil.ReplaceFile(lz4BackupRoot + "/lz4", lz4ExportRoot + "/lz4");
                FileUtil.ReplaceFile(lz4BackupRoot + "/lz4.manifest", lz4ExportRoot + "/lz4.manifest");

                FileUtil.ReplaceFile(lzmaBackupRoot + "/lzma", lzmaExportRoot + "/lzma");
                FileUtil.ReplaceFile(lzmaBackupRoot + "/lzma.manifest", lzmaExportRoot + "/lzma.manifest");

                int finishedCount = 0;
                //还原该版本资源的Bundle及其manifest文件到打包目录
                foreach (var resInfo in resConfig.Manifest.Values)
                {
                    string bundleExportDir = GetBundleBackupDir(resInfo, exportDir); 
                    string bundleBackupDir = GetBundleBackupDir(resInfo, backupDir);

                    var bundleFileInfo = new FileInfo(resInfo.GetABPath(bundleBackupDir));
                    var bundleManifest = resInfo.GetManifestPath(bundleBackupDir);
                    if (bundleFileInfo.Exists && File.Exists(bundleManifest))
                    {
                        var exportBundlePath = resInfo.GetExportPath(bundleExportDir);
                        var exportBundleManifest = resInfo.GetManifestPath(bundleExportDir);
                        Directory.CreateDirectory(Path.GetDirectoryName(exportBundlePath));

                        FileUtil.ReplaceFile(bundleFileInfo.FullName, exportBundlePath);
                        FileUtil.ReplaceFile(bundleManifest, exportBundleManifest);

                        //对于压缩类型为CompressType.CustomZip,压缩包还原
                        if (resInfo.remoteZipType == CompressType.CustomZip)
                        {
                            var backupZipPath = resInfo.GetRemotePath(bundleBackupDir);
                            if (File.Exists(backupZipPath))
                            {
                                FileUtil.ReplaceFile(backupZipPath, resInfo.GetRemotePath(bundleExportDir));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("还原异常,在备份目录找不到该文件或其Manifest文件:" + resInfo.bundleName);
                    }
                    finishedCount += 1;
                    if (_showPrograss)
                        EditorUtility.DisplayProgressBar("还原AssetBundle中",
                            string.Format(" {0} / {1} ", finishedCount, resConfig.Manifest.Count),
                            finishedCount / (float)resConfig.Manifest.Count);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                EditorUtility.DisplayDialog("提示", "还原当前版本资源失败,详情查看Log!!!", "OK");
            }

            if (_showPrograss)
                EditorUtility.ClearProgressBar();

            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            if (!_slientMode)
            {
                EditorUtility.DisplayDialog("提示",
                    string.Format("还原Bundle资源总耗时:{0:00}:{1:00}:{2:00}:{3:00}\n", elapsed.Hours, elapsed.Minutes,
                        elapsed.Seconds, elapsed.Milliseconds / 10), "OK");
            }
            else
            {
                Debug.Log(string.Format("还原Bundle资源总耗时:{0:00}:{1:00}:{2:00}:{3:00}\n", elapsed.Hours, elapsed.Minutes,
                    elapsed.Seconds, elapsed.Milliseconds / 10));
            }
        }
#endregion

#region 生成PatchInfo

        /// <summary>
        /// 加载patchinfo目录下所有版本更新信息
        /// </summary>
        private void LoadAllPatchInfo()
        {
            var patchInfoFiles = Directory.GetFiles(GetPatchInfoRoot());
            _patchInfoList = new List<ResPatchInfo>(patchInfoFiles.Length);
            foreach (string filePath in patchInfoFiles)
            {
                string fileName = Path.GetFileName(filePath);
                //Mac下目录中会带有隐藏的.DS_Store文件
                if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith("patch_"))
                    continue;

                try
                {
                    var patchInfo = FileHelper.ReadJsonFile<ResPatchInfo>(filePath);
                    _patchInfoList.Add(patchInfo);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            _patchInfoList.Sort(SortByCurVer);
        }

        /// <summary>
        /// 生成历史各版本升级到最新版本的PatchInfo信息
        /// </summary>
        private void GenerateAllPatchInfo()
        {
            if (_curResConfig == null)
                return;

            string resConfigRoot = GetResConfigRoot();
            var configFiles = Directory.GetFiles(resConfigRoot, "*.json");
            _patchInfoList = new List<ResPatchInfo>(configFiles.Length);
            for (int i = 0; i < configFiles.Length; ++i)
            {
                string fileName = Path.GetFileName(configFiles[i]);
                //Mac下目录中会带有隐藏的.DS_Store文件
                if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith("resConfig_"))
                    continue;

                try
                {
                    ResConfig oldResConfig = LoadResConfig(configFiles[i]);
                    GeneratePatchInfo(oldResConfig, _curResConfig);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("生成PatchInfo中", String.Format(" {0} / {1} ", i, configFiles.Length),
                        (float)i / configFiles.Length);
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }

        private void GeneratePatchInfo(ResConfig oldResConfig, ResConfig newResConfig)
        {
            if (oldResConfig == null || newResConfig == null)
            {
                ShowNotification(new GUIContent("ResConfig 为空，请检查"));
                return;
            }

            //无需生成当前版本PatchInfo
            if (oldResConfig.Version == newResConfig.Version)
            {
                return;
            }

            ResPatchInfo patchInfo = new ResPatchInfo
            {
                CurVer = oldResConfig.Version,
                CurLz4CRC = oldResConfig.lz4CRC,
                CurLzmaCRC = oldResConfig.lzmaCRC,
                CurTexCRC = oldResConfig.tileTexCRC,
                EndVer = newResConfig.Version,
                EndLz4CRC = newResConfig.lz4CRC,
                EndLzmaCRC = newResConfig.lzmaCRC,
                EndTexCRC = newResConfig.tileTexCRC
            };

            //生成更新列表
            //CRC不为0，且CRC值发生变更的，加入更新列表
            //oldResConfig不存在的，直接加入更新列表
            foreach (var newRes in newResConfig.Manifest)
            {
                if (oldResConfig.Manifest.ContainsKey(newRes.Key))
                {
                    if (oldResConfig.Manifest[newRes.Key].CRC != newRes.Value.CRC)
                    {
                        patchInfo.updateList.Add(newRes.Value);
                        patchInfo.TotalFileSize += newRes.Value.size;
                    }
                }
                else
                {
                    patchInfo.updateList.Add(newRes.Value);
                    patchInfo.TotalFileSize += newRes.Value.size;
                }
            }

            //生成删除列表
            //oldResConfig的key在newResConfig中找不到对应key的，证明该资源已被删除
            foreach (var oldRes in oldResConfig.Manifest)
            {
                if (!newResConfig.Manifest.ContainsKey(oldRes.Key))
                {
                    patchInfo.removeList.Add(oldRes.Key);
                }
            }

            //导出patch配置信息
            string path = Path.Combine(GetPatchInfoRoot(), patchInfo.ToFileName());
            FileHelper.SaveJsonObj(patchInfo, path, false, true);
            int index = _patchInfoList.FindIndex(info =>
            {
                if (info.ToFileName() == patchInfo.ToFileName())
                    return true;

                return false;
            });
            if (index != -1)
            {
                _patchInfoList[index] = patchInfo;
            }
            else
            {
                _patchInfoList.Add(patchInfo);
                _patchInfoList.Sort(SortByCurVer);
            }
        }

        public static int SortByCurVer(ResPatchInfo a, ResPatchInfo b)
        {
            return -a.CurVer.CompareTo(b.CurVer);
        }

        /// <summary>
        /// 根据ResPatchInfo信息生成需要更新的文件列表
        /// </summary>
        private void GeneratePatchInfoUrlFile(ResPatchInfo patchInfo, string cdnRoot, string cdnRegion)
        {
            if (patchInfo == null)
            {
                this.ShowNotification(new GUIContent("ResPatchInfo 为空，请检查"));
                return;
            }

            if (string.IsNullOrEmpty(cdnRoot))
            {
                this.ShowNotification(new GUIContent("cdnRoot为空，请检查"));
                return;
            }

            if (patchInfo.CurVer != patchInfo.EndVer && patchInfo.updateList.Count == 0)
            {
                this.ShowNotification(new GUIContent("资源未发生变更，无需导出PatchInfo Url信息"));
                return;
            }

            var sb = new StringBuilder();
            string bundleRoot = string.Format("{0}/{1}/staticRes/{2}", cdnRoot, cdnRegion, GameResPath.REMOTE_BUNDLE_ROOT);
            //版本号相同导出当前版本所有资源
            if (patchInfo.CurVer == patchInfo.EndVer)
            {
                foreach (var resInfo in _curResConfig.Manifest.Values)
                {
                    sb.AppendLine(resInfo.GetRemotePath(bundleRoot));
                }
            }
            else
            {
                foreach (var resInfo in patchInfo.updateList)
                {
                    sb.AppendLine(resInfo.GetRemotePath(bundleRoot));
                }
            }

            if (sb.Length > 0)
            {
                string filePath = GetPatchInfoUrlConfigFileName(patchInfo, cdnRegion);
                string dir = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, sb.ToString());

                OpenDirectory(dir);
            }
        }

        public string GetPatchInfoUrlConfigFileName(ResPatchInfo patchInfo, string cdnRegion)
        {
            string exportRoot = GetExportPlatformPath();
            return String.Format("{0}/patch_urlconfig/patch_url_{1}_{2}_{3}.txt", exportRoot, patchInfo.CurVer,
                patchInfo.EndVer, cdnRegion);
        }

        /// <summary>
        /// 根据oldResConfig与curResConfig生成资源更新清单
        /// </summary>
        private void GeneratePatchRes(ResPatchInfo patchInfo)
        {
            if (patchInfo == null)
            {
                this.ShowNotification(new GUIContent("ResPatchInfo 为空，请检查"));
                return;
            }

            if (patchInfo.CurLz4CRC == patchInfo.EndLz4CRC &&
                patchInfo.CurLzmaCRC == patchInfo.EndLzmaCRC &&
                patchInfo.CurTexCRC == patchInfo.EndTexCRC &&
                patchInfo.updateList.Count == 0 && patchInfo.removeList.Count == 0)
            {
                this.ShowNotification(new GUIContent("资源未发生变更，无需导出Patch资源"));
                return;
            }

            string backupDir = GetBackupDir(_curResConfig);
            //如果当前目录已存在,询问用户是否需要重新生成
            string patchResDir = GetPatchResRoot(patchInfo);
            if (Directory.Exists(patchResDir))
            {
                if (EditorUtility.DisplayDialog("确认", "当前PatchInfo的补丁资源已生成,请问是否跳过?", "跳过", "重新生成"))
                    return;
            }

            Debug.Log(string.Format("Remove PatchRes Folder:{0}", FileUtil.DeleteFileOrDirectory(patchResDir)));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int finishedCount = 0;
            //拷贝AssetBundle到PatchRes目录下
            if (patchInfo.updateList.Count > 0)
            {
                foreach (var resInfo in patchInfo.updateList)
                {
                    string bundleBackupDir = GetBundleBackupDir(resInfo, backupDir);
                    string inputFile = resInfo.GetRemotePath(bundleBackupDir);

                    try
                    {
                        if (File.Exists(inputFile))
                        {
                            string outputFile = resInfo.GetRemotePath(patchResDir);
                            string dir = Path.GetDirectoryName(outputFile);
                            Directory.CreateDirectory(dir);
                            FileUtil.CopyFileOrDirectory(inputFile, outputFile);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        if (_showPrograss)
                            EditorUtility.ClearProgressBar();
                        AssetDatabase.Refresh();
                        return;
                    }
                    finishedCount += 1;
                    if (_showPrograss)
                        EditorUtility.DisplayProgressBar("拷贝AssetBundle中",
                            string.Format(" {0} / {1} ", finishedCount, patchInfo.updateList.Count),
                            finishedCount / (float)patchInfo.updateList.Count);
                }
                if (_showPrograss)
                    EditorUtility.ClearProgressBar();
            }
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            if (!_showPrograss && !_slientMode)
            {
                EditorUtility.DisplayDialog("提示",
                    string.Format("生成资源更新清单耗时:{0:00}:{1:00}:{2:00}:{3:00}", elapsed.Hours, elapsed.Minutes,
                        elapsed.Seconds,
                        elapsed.Milliseconds / 10), "Yes");
            }
            else
            {
                Debug.Log(string.Format("生成资源更新清单耗时:{0:00}:{1:00}:{2:00}:{3:00}", elapsed.Hours, elapsed.Minutes,
                    elapsed.Seconds, elapsed.Milliseconds / 10));
            }

            AssetDatabase.Refresh();
            OpenDirectory(patchResDir);
        }

        /// <summary>
        /// 生成指定资源版本的所有CDN上的资源,根据ResInfo.remoteZipType字段对原始打包数据进行压缩
        /// </summary>
        /// <param name="resConfig"></param>
        private void GenerateRemoteRes(ResConfig resConfig)
        {
            if (resConfig == null)
            {
                this.ShowNotification(new GUIContent("ResConfig 为空，请检查"));
                return;
            }

            string backupDir = GetBackupDir(resConfig);
            //如果当前目录已存在,询问用户是否需要重新生成
            string remoteResDir = GetRemoteResRoot(resConfig);
            if (Directory.Exists(remoteResDir))
            {
                if (EditorUtility.DisplayDialog("确认", "当前版本的CDN资源已生成,请问是否跳过?", "跳过", "重新生成"))
                    return;
            }

            Debug.Log(string.Format("Remove RemoteRes Folder:{0}", FileUtil.DeleteFileOrDirectory(remoteResDir)));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int finishedCount = 0;

            foreach (var resInfo in resConfig.Manifest.Values)
            {
                string bundleBackupDir = GetBundleBackupDir(resInfo, backupDir);
                string inputFile = resInfo.GetRemotePath(bundleBackupDir);

                try
                {
                    if (File.Exists(inputFile))
                    {
                        string outputFile = resInfo.GetRemotePath(remoteResDir);
                        string dir = Path.GetDirectoryName(outputFile);
                        Directory.CreateDirectory(dir);
                        FileUtil.CopyFileOrDirectory(inputFile, outputFile);
                    }
                    else
                    {
                        throw new Exception("不存在该文件:" + inputFile);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    if (_showPrograss)
                        EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                    return;
                }
                finishedCount += 1;
                if (_showPrograss)
                    EditorUtility.DisplayProgressBar("拷贝AssetBundle中",
                        string.Format(" {0} / {1} ", finishedCount, resConfig.Manifest.Count),
                        finishedCount / (float)resConfig.Manifest.Count);
            }
            if (_showPrograss)
                EditorUtility.ClearProgressBar();
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            if (!_showPrograss && !_slientMode)
            {
                EditorUtility.DisplayDialog("提示",
                    string.Format("生成指定资源版本的所有CDN上的资源耗时:{0:00}:{1:00}:{2:00}:{3:00}", elapsed.Hours, elapsed.Minutes,
                        elapsed.Seconds, elapsed.Milliseconds / 10), "Yes");
            }
            else
            {
                Debug.Log(string.Format("生成指定资源版本的所有CDN上的资源耗时:{0:00}:{1:00}:{2:00}:{3:00}", elapsed.Hours,
                    elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds / 10));
            }

            AssetDatabase.Refresh();
            OpenDirectory(remoteResDir);
        }

        #endregion
#region Resconfig Json转换到Tab

        private void ResconfigJson2Tz()
        {
            string selectFolder = EditorUtility.OpenFolderPanel("选择要转换的Resconfig目录", GetExportPlatformPath(), GameResPath.RESCONFIG_ROOT);
            if (string.IsNullOrEmpty(selectFolder))
                return;

            string[] files = Directory.GetFiles(selectFolder, "*.json", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "未选中有效目录", "OK");
                return;
            }
            int i = 0;
            string converOutPath = selectFolder.Replace(Path.GetFileName(selectFolder), GameResPath.RESCONFIG_ROOT + "Tab");
            if (Directory.Exists(converOutPath))
            {
                bool isDele = EditorUtility.DisplayDialog("提示", string.Format("已存在目录：\n{0}\n 是否删除", converOutPath), "删除并覆盖", "Cancel");
                if (isDele)
                    FileUtil.DeleteFileOrDirectory(converOutPath);
                else
                    return;
            }
            Directory.CreateDirectory(converOutPath);
            files.ForEach(filePath =>
            {
                ResConfig resConfig = FileHelper.ReadJsonFile<ResConfig>(filePath, false);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                resConfig.SaveFile(Path.Combine(converOutPath, fileName + ".tz"), true);
                i++;
                EditorUtility.DisplayProgressBar("进度", string.Format("正在转换{0}/{1}", i, files.Length), (float)i / files.Length);
            });
            EditorUtility.ClearProgressBar();
            Process.Start(converOutPath);

        }

        private void ResconfigTz2Json()
        {
            string selectFolder = EditorUtility.OpenFolderPanel("选择要转换的Resconfig目录", GetExportPlatformPath(), GameResPath.RESCONFIG_ROOT);
            if (string.IsNullOrEmpty(selectFolder))
                return;
            string[] files = Directory.GetFiles(selectFolder, "*.tz", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "未选中有效目录", "OK");
                return;
            }
            int i = 0;
            string converOutPath = selectFolder.Replace(Path.GetFileName(selectFolder), GameResPath.RESCONFIG_ROOT + "Json");
            if (Directory.Exists(converOutPath))
            {
                bool isDele = EditorUtility.DisplayDialog("提示", string.Format("已存在目录：\n{0}\n 是否删除", converOutPath), "删除并覆盖", "Cancel");
                if (isDele)
                    FileUtil.DeleteFileOrDirectory(converOutPath);
                else
                    return;
            }
            files.ForEach(filePath =>
            {
                byte[] rawBytes = FileHelper.ReadAllBytes(filePath);
                ResConfig resConfig = ResConfig.ReadFile(ZipLibUtils.Uncompress(rawBytes), false);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                FileHelper.SaveJsonObj(resConfig, Path.Combine(converOutPath, fileName + ".json"), false, true);
                i++;
                EditorUtility.DisplayProgressBar("进度", string.Format("正在转换{0}/{1}", i, files.Length), (float)i / files.Length);
            });
            EditorUtility.ClearProgressBar();
            Process.Start(converOutPath);
        }
#endregion

        #region Helper Func

        //获取resConfig配置信息目录
        public static string GetResConfigRoot()
        {
            return GetExportPlatformPath() + "/" + GameResPath.RESCONFIG_ROOT;
        }

        public static string GetExportBundlePath()
        {
            return GetExportPlatformPath() + "/" + GameResPath.BUNDLE_ROOT;
        }

        public static string GetExportBundlePathByBuildTarget(BuildTarget buildTarget)
        {
            return GetExportPlatformPathByBuildTarget(buildTarget) + "/" + GameResPath.BUNDLE_ROOT;
        }
        public static string GetBackupRoot()
        {
            return GetExportPlatformPath() + "/backup";
        }

        //根据resConfig版本号，获取版本资源导出目录
        public static string GetBackupDir(ResConfig resConfig)
        {
            if (resConfig == null)
                return null;

            return GetBackupRoot() + "/" + GameResPath.BUNDLE_ROOT + "_" + resConfig.Version;
        }

        public static string GetPatchResRoot(ResPatchInfo patchInfo)
        {
            string exportRoot = GetExportPlatformPath();
            return string.Format("{0}/patch_resources/patch_{1}_{2}", exportRoot, patchInfo.CurVer, patchInfo.EndVer);
        }

        public static string GetRemoteResRoot(ResConfig resConfig)
        {
            string exportRoot = GetExportPlatformPath();
            return string.Format("{0}/{1}/remoteres_{2}", exportRoot, GameResPath.REMOTE_BUNDLE_ROOT, resConfig.Version);
        }

        //获取patch信息目录
        public static string GetPatchInfoRoot()
        {
            return GetExportPlatformPath() + "/patchinfo";
        }

        public static string GetBuildBundleStrategyPath()
        {
            return GameResPath.EXPORT_FOLDER + "/buildBundleStrategy.json";
        }

        private static string GetExportPlatformPath()
        {
            return GetExportPlatformPathByBuildTarget(EditorUserBuildSettings.activeBuildTarget);
        }

        private static string GetExportPlatformPathByBuildTarget(BuildTarget buildTarget)
        {
            string platformRoot;
            if (buildTarget == BuildTarget.Android)
            {
                platformRoot = GameResPath.EXPORT_FOLDER + "/Android";
            }
            else if (buildTarget == BuildTarget.iOS)
            {
                platformRoot = GameResPath.EXPORT_FOLDER + "/IOS";
            }
            else
            {
                platformRoot = GameResPath.EXPORT_FOLDER + "/PC";
            }
            return platformRoot;
        }
        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void OpenDirectory(string path)
        {
            if (EditorUtility.DisplayDialog("确认", "是否打开导出资源目录？", "打开", "取消"))
            {
                var dir = Path.GetFullPath(path);
                if (Directory.Exists(dir))
                    Process.Start(dir);
                else
                {
                    EditorUtility.DisplayDialog("提示", "不存在:" + path + " 目录", "OK");
                }
            }
        }

        public static T LoadYAMLObj<T>(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var yamlParser = new YamlDotNet.Serialization.DeserializerBuilder();
                return yamlParser.Build().Deserialize<T>(sr);
            }
        }

        public static string StripHashSuffix(string bundleName)
        {
#if BUNDLE_APPEND_HASH
            int index = bundleName.LastIndexOf('_');
            if (index > 0)
            {
                return bundleName.Substring(0, index);
            }
            return bundleName;
#else
            return bundleName;
#endif
        }

        private static ResConfig LoadResConfigFilePanel(bool saveHistory = false)
        {
            string dir = GetResConfigRoot();
            Directory.CreateDirectory(dir);
            string filePath = EditorUtility.OpenFilePanel("加载版本资源配置信息", dir, "json");
            var resConfig = LoadResConfig(filePath);
            if (saveHistory)
            {
                EditorPrefs.SetString(GetLastResConfigPathHash(), filePath);
            }

            return resConfig;
        }

        private static ResConfig LoadResConfig(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                var resConfig = FileHelper.ReadJsonFile<ResConfig>(filePath);
                return resConfig;
            }
            return null;
        }

        /// <summary>
        ///     unixTimestamp单位为毫秒
        /// </summary>
        /// <param name="unixTimestamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(long unixTimestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddTicks(unixTimestamp * 10000).ToLocalTime();
        }

        /// <summary>
        ///     返回的unixTimestamp单位为毫秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).Ticks / 10000;
        }

        public static string GetResConfigInfo(ResConfig resConfig)
        {
            return resConfig == null
                ? "ResConfig is null"
                : string.Format(
                    "Version:{0}\nlz4CRC:{1}\nlzmaCRC:{2}\ntileTexCRC:{3}\nBuildTime:{4}\nCompressType:{5}\nTotalFileSize:{6}\nCount:{7}",
                    resConfig.Version,
                    resConfig.lz4CRC,
                    resConfig.lzmaCRC,
                    resConfig.tileTexCRC,
                    UnixTimeStampToDateTime(resConfig.BuildTime).ToString("yyyy-M-d HH:mm:ss"),
                    resConfig.compressType,
                    EditorUtility.FormatBytes(resConfig.TotalFileSize),
                    resConfig.Manifest.Count);
        }

        public static string GetBundleBackupDir(ResInfo resInfo, string backupDir)
        {
            if(resInfo.remoteZipType == CompressType.UnityLZMA)
            {
                return backupDir + "/lzma";
            }
            else if (resInfo.remoteZipType == CompressType.CustomTex)
            {
                return backupDir + "/custom";
            }
            else
            {
                return backupDir + "/lz4";
            }
        }
        private static string GetLastResConfigPathHash()
        {
            int hash = Application.dataPath.GetHashCode();
            string lastResConfigPathHash = string.Format("LastResConfigPath{0}", hash);
            return lastResConfigPathHash;
        }
        #endregion
    }
}
