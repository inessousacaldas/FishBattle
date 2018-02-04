using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Refactor
{
	/// <summary>
	/// H1老旧代码重构专用辅助工具类
	/// @MarsZ 2016-11-17 17:31:25
	/// </summary>
	public class RefactorUtils
	{

		public static List<string> GetAllCodeFilesInDirectory(string pDirectory, string pInvalidPathKeyWord = "")
		{
			return GetAllFilesInDirectory(pDirectory, ".cs", pInvalidPathKeyWord);
		}


		public static List<string> GetAllPrefabFilesInDirectory(string pDirectory, string pInvalidPathKeyWord = "")
		{
			return GetAllFilesInDirectory(pDirectory, ".prefab", pInvalidPathKeyWord);
		}

		public static List<string> GetAllFilesInDirectory(string pDirectory, string pSuffix, string pInvalidPathKeyWordPattern = "", string pFileNamePattern = "", bool pIncludeSubDir = true)
		{
			if (string.IsNullOrEmpty(pDirectory))
				return null;
			List<string> tResult = null;
			List<string> tFilePaths = GetFiles(pDirectory, pIncludeSubDir);
			if (null != tFilePaths)
			{
				tResult = new List<string>();
				string tFilePath;
				for (int tCounter = 0, tLen = tFilePaths.Count; tCounter < tLen; tCounter++)
				{
					tFilePath = tFilePaths[tCounter];
					if (string.IsNullOrEmpty(tFilePath))
						continue;
					if (Regex.IsMatch(tFilePath, ".meta"))
						continue;
//					if (tFilePath == "/Users/cilu/Workspace/H5/S1ed_2/Assets/Scripts/MyGameScripts/Module/PetPropertyModule/SkillCellController.cs")
//						GameDebuger.Log(">>>");
					if (!string.IsNullOrEmpty(pInvalidPathKeyWordPattern))
					{
						if (Regex.IsMatch(tFilePath, pInvalidPathKeyWordPattern))
							continue;
					}
					if (tFilePath.LastIndexOf(pSuffix) != tFilePath.Length - pSuffix.Length)
						continue;
					if (!string.IsNullOrEmpty(pFileNamePattern))
					{
						if (!Regex.IsMatch(tFilePath, pFileNamePattern))
							continue;
					}
					tResult.Add(tFilePath);
				}
			}
			return tResult;
		}


		public static bool CreateFileDirectory(string pFilePath, bool pCreateFile = true)
		{
			if (!File.Exists(pFilePath))
			{
				string tDirectory = Path.GetDirectoryName(pFilePath);
				if (!Directory.Exists(tDirectory))
					Directory.CreateDirectory(tDirectory);
				if (pCreateFile)
					File.Create(pFilePath).Close();
				return true;
			}
			return false;//已存在的不替换
		}

		public static List<string> GetFiles(string pDir, bool pIncludeSubDir = true)
		{
			List<string> tFileInfoList = new List<string>();
			if (Directory.Exists(pDir))
			{
				DirectoryInfo tDirectoryInfo = new DirectoryInfo(pDir);
				if (null != tDirectoryInfo)
				{
					FileInfo[] tFileInfos = tDirectoryInfo.GetFiles();
					if (null != tFileInfos)
					{
						FileInfo tFileInfo;
						for (int tCounter = 0, tLen = tFileInfos.Length; tCounter < tLen; tCounter++)
						{
							tFileInfo = tFileInfos[tCounter];
							if (null != tFileInfo)
								tFileInfoList.Add(tFileInfo.FullName);
						}
					}
					if (pIncludeSubDir)
					{
						DirectoryInfo[] tDirectoryInfos = tDirectoryInfo.GetDirectories();
						if (null != tDirectoryInfos)
						{
							List<string> tFileInfoList2 = new List<string>();
							DirectoryInfo tFileInfo;
							for (int tCounter = 0, tLen = tDirectoryInfos.Length; tCounter < tLen; tCounter++)
							{
								tFileInfo = tDirectoryInfos[tCounter];
								if (null != tFileInfo)
								{
									tFileInfoList2 = GetFiles(tFileInfo.FullName);
									if (null != tFileInfoList2 && tFileInfoList2.Count > 0)
									{
										tFileInfoList.AddRange(tFileInfoList2);
									}
								}
							}
						}
					}
				}
			}
			return tFileInfoList;
		}

		public static bool IsViewFile(string pFileContent)
		{
			if (!string.IsNullOrEmpty(pFileContent))
			{
				return Regex.IsMatch(pFileContent, RefactorConst.KEYWORD_VIEW);
			}
			return false;
		}

		//是否H1形式的Controller
		public static bool IsIViewController(string pFileContent)
		{
			if (!string.IsNullOrEmpty(pFileContent))
			{
				return Regex.IsMatch(pFileContent, RefactorConst.KEYWORD_CONTROLLER) || Regex.IsMatch(pFileContent, RefactorConst.KEYWORD_CONTROLLER2);
			}
			return false;
		}

		public static bool IsModuleFile(string pFileContent)
		{
			if (!string.IsNullOrEmpty(pFileContent))
			{
				return Regex.IsMatch(pFileContent, RefactorConst.KEYWORD_MODEL);
			}
			return false;
		}

		public static bool IsMonolessViewController(string pFileContent)
		{
			if (string.IsNullOrEmpty(pFileContent))
				return false;
			if (RefactorConst.RegexIsMonolessViewController.IsMatch(pFileContent))
				return true;
			return false;
		}

		public static bool IsMonoViewController(string pFileContent)
		{
			if (string.IsNullOrEmpty(pFileContent))
				return false;
			if (RefactorConst.RegexIsMonoViewController.IsMatch(pFileContent))
				return true;
			return false;
		}
        public static string GetH1ModuleName(string pFileContent)
        {
            Match match = Regex.Match(pFileContent, RefactorConst.KEYWORD_H1MODEL);
            string matchValue = match.Value;
            string moduleName = null;
            if (string.IsNullOrEmpty(matchValue) == false)
            {
                moduleName = Regex.Replace(matchValue, RefactorConst.KEYWORD_H1MODEL, RefactorConst.KEYWORD_MODELNAME);
            }
            return moduleName;
        }
        public static string GetModuleName(string pFileContent)
		{
			Match match = Regex.Match(pFileContent, RefactorConst.KEYWORD_MODEL);
			string matchValue = match.Value;
			string moduleName = null;
			if (string.IsNullOrEmpty(matchValue) == false)
			{
				moduleName = Regex.Replace(matchValue, RefactorConst.KEYWORD_MODEL, RefactorConst.KEYWORD_MODELNAME);
			}
			return moduleName;
		}

		public static bool UpdateProxyFileFromStaticToInstance(string pProxyFileFullPath)
		{
			if (File.Exists(pProxyFileFullPath))
			{
				string tFileContent = File.ReadAllText(pProxyFileFullPath);
				if (!string.IsNullOrEmpty(tFileContent))
				{
					tFileContent = tFileContent.Replace("public static ", "public ");
					if (!string.IsNullOrEmpty(tFileContent))
					{
						File.WriteAllText(pProxyFileFullPath, tFileContent);
						GameDebuger.Log(string.Format(" Proxy 内容已更新。pProxyFileFullPath：{0}", pProxyFileFullPath));
						return true;
					}
				}
			}
			return false;
		}

		public static void SaveAndRefreshU3D()
		{
			EditorApplication.SaveAssets();
			AssetDatabase.Refresh();
			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
		}

		public static bool DeleteEmptyDir(string pDirectory, bool pIncludeSubDir = true)
		{
			if (!Directory.Exists(pDirectory))
				return true;
			DirectoryInfo tDirectoryInfo = new DirectoryInfo(pDirectory);
			if (null == tDirectoryInfo)
			{
				return true;
			}
			List<string> tFileInfos = GetAllFilesInDirectory(pDirectory, ".cs", ".meta", "", false);// tDirectoryInfo.GetFiles ();
			DirectoryInfo[] tDirectoryInfos = tDirectoryInfo.GetDirectories();
			bool tHasFile = null != tFileInfos && tFileInfos.Count > 0;
			bool tHasSubDir = null != tDirectoryInfos && tDirectoryInfos.Length > 0;

			if (tHasSubDir)
			{
				if (DeleteEmptyDir(tDirectoryInfos, pIncludeSubDir))
				{
					tHasSubDir = false;
				}
			}

			if (tHasFile)
			{
				return false;
			}
			else
			{
				if (tHasSubDir)
					return false;
				else
				{
					Directory.Delete(pDirectory, true);
					return true;
				}
			}
		}

		public static bool DeleteEmptyDir(DirectoryInfo[] pDirectorys, bool pIncludeSubDir = true)
		{
			if (null == pDirectorys || pDirectorys.Length <= 0)
				return true;
			bool tIsAllDirEmpty = true;
			for (int tCounter = 0, tLen = pDirectorys.Length; tCounter < tLen; tCounter++)
			{
				tIsAllDirEmpty = DeleteEmptyDir(pDirectorys[tCounter].FullName, pIncludeSubDir) && tIsAllDirEmpty;
			}
			return tIsAllDirEmpty;
		}

		public static void ShowLabelTip(string pPathNoProject, bool pIncludedProjectPath = false, bool pIsFilePath = false)
		{
			pPathNoProject = pIncludedProjectPath ? pPathNoProject : (RefactorConst.PROJECT_PATH + pPathNoProject);
			if (pIsFilePath)
			{
				if (!File.Exists(pPathNoProject))
					GUILayout.Label("[* 文件不存在]", GUILayout.Width(RefactorConst.FIELD_TIP_WIDTH));
			}
			else
			{
				if (!Directory.Exists(pPathNoProject))
					GUILayout.Label("[* 目录不存在]", GUILayout.Width(RefactorConst.FIELD_TIP_WIDTH));		
			}
		}

		public static bool ShowButton(string pButtonLabel)
		{
			return GUILayout.Button(pButtonLabel, GUILayout.ExpandWidth(true), GUILayout.Height(RefactorConst.BUTTON_HEIGHT));
		}

		public static string GetUniqueFilePathByAssetDatabase(string pFileNameWithOutExtension, string pFilePathDir = null, string pTypeName = "script")
		{
			if (string.IsNullOrEmpty(pFileNameWithOutExtension))
				return null;
//			if (pFileNameWithOutExtension == "SkillCellController")
//				GameDebuger.LogError("来了！");
			List<string> tFiles = GetAllFilesInDirectory(pFilePathDir, ".cs", @"\.meta", @"/" + pFileNameWithOutExtension + @".cs");
			if (null == tFiles || tFiles.Count != 1)
			{
				GameDebuger.LogError(string.Format("文件查找失败，pFileNameWithOutExtension：{0},pFilePathDir:{1} ！", pFileNameWithOutExtension, pFilePathDir));
				return null;
			}
			return tFiles[0];
//			string[] tGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:script {0}", pFileNameWithOutExtension), pFilePathDir);
//			if (null == tGUIDs || tGUIDs.Length <= 0)
//			{
//				GameDebuger.LogError(string.Format("通过 AssetDatabase 查找唯一资源 {0} 失败！", pFileNameWithOutExtension));
//				return null;
//			}
//			int tLen = tGUIDs.Length;
//			string matPath = null;
//			for (int tCounter = 0; tCounter < tLen; tCounter++)
//			{
//				matPath = RefactorConst.PROJECT_PATH + AssetDatabase.GUIDToAssetPath(tGUIDs[tCounter]);
//				if (Path.GetFileNameWithoutExtension(matPath) != pFileNameWithOutExtension)
//					continue;
//				return matPath;
//			}
//			return null;
		}

		public static string GetProjectDirByFileFullPath(string pFileFullPath)
		{
			if (string.IsNullOrEmpty(pFileFullPath))
				return null;
			pFileFullPath = GetRegexSTRValue(RefactorConst.RegexToGetProjectDirByFileFullPath, pFileFullPath);
			return pFileFullPath;
		}

		/// <summary>
		/// 获取正则匹配到的目标替换字符串的值
		/// </summary>
		/// <returns>The regex string value.</returns>
		/// <param name="pRegex">P regex.</param>
		/// <param name="pInputContent">P input content.</param>
		/// <param name="pRelaceSTRStr">正则匹配中的替换字符串.</param>
		public static string GetRegexSTRValue(Regex pRegex, string pInputContent, string pRelaceSTRStr = "STR")
		{
			string tSTRValue = string.Empty;
			Match tMatch = pRegex.Match(pInputContent);
			if (null == tMatch || tMatch.Groups.Count <= 0)
				return string.Empty;
			tSTRValue = tMatch.Groups[pRelaceSTRStr].Value;
			if (string.IsNullOrEmpty(tSTRValue))
			{
//				GameDebuger.LogError (string.Format ("GetRegexSTRValue failed！pRegex：{0}，pRelaceSTRStr：{1}", pRegex, pRelaceSTRStr));
				return string.Empty;
			}
			return tSTRValue;
		}

		public static void LogFile(string pSaveFilePath, string pFileContent)
		{
			RefactorUtils.CreateFileDirectory(pSaveFilePath, true);
			File.AppendAllText(pSaveFilePath, pFileContent + "\n");
		}

		public static string ExcuteRegexReplace(Regex pRegexToMatch, string pRegexToReplaceTo, string pFileContent, string pTip = "")
		{
			if (!pRegexToMatch.IsMatch(pFileContent))
			{
				if (string.IsNullOrEmpty(pTip))
					return pFileContent;
				GameDebuger.LogError(pTip);
				return pFileContent;
			}
			pFileContent = pRegexToMatch.Replace(pFileContent, pRegexToReplaceTo);
			return pFileContent;
		}

		#region 对指定目录下所有文件执行特定正则替换

		public static void ReplaceWithRegexForTargetDir(string pDir, Regex pRegexToMatch, string pRegexToReplaceTo, string pTip = "代码操作", Regex pInvalidContentRegex = null)
		{
			List<string> tAllCodesFileList = GetAllCodesFileList(pDir);
			ReplaceWithRegexForTargetPathFileList(tAllCodesFileList, pRegexToMatch, pRegexToReplaceTo, pTip, pInvalidContentRegex);
			Refactor.RefactorUtils.SaveAndRefreshU3D();
		}

		private static List<string> GetAllCodesFileList(string pDir)
		{
			EditorUtility.DisplayCancelableProgressBar("Refactor Controller", "收集所有文件...", 0.5f);
			List<string> tAllCodesFileList = Refactor.RefactorUtils.GetAllCodeFilesInDirectory(pDir, "(Scripts/MyGameScripts/Module/CommonUIModule)|(Scripts/MyGameScripts/Module/_ModuleName_Module)");
			EditorUtility.ClearProgressBar();
			return tAllCodesFileList;
		}

		private static bool ReplaceWithRegexForTargetPathFileList(List<string> pAllCodesFileList, Regex pRegexToMatch, string pRegexToReplaceTo, string pTip = "代码操作", Regex pInvalidContentRegex = null)
		{
			if (null == pAllCodesFileList || pAllCodesFileList.Count <= 0)
			{
				GameDebuger.LogError("检测没有继承MonolessViewController或MonoViewController的Controller失败，其他文件列表为空！");
				return false;
			}
			string tFileFullPath = string.Empty;
			for (int tCounter = 0, tLen = pAllCodesFileList.Count; tCounter < tLen; tCounter++)
			{
				tFileFullPath = pAllCodesFileList[tCounter];
				UnityEditor.EditorUtility.DisplayCancelableProgressBar(pTip,
					string.Format("{0}:{1}", pTip, tFileFullPath), (float)tCounter / (float)tLen);
				try
				{
					ReplaceWithRegexForTargetPathFile(tFileFullPath, pRegexToMatch, pRegexToReplaceTo, pInvalidContentRegex);
				}
				catch (Exception e)
				{
					GameDebuger.LogError(string.Format("{0} 异常，tFileFullPath：{1}，错误码：{2}", pTip, tFileFullPath, e.Message));
				}

			}
			UnityEditor.EditorUtility.ClearProgressBar();
			return true;
		}

		private static bool ReplaceWithRegexForTargetPathFile(string pFileFullPath, Regex pRegexToMatch, string pRegexToReplaceTo, Regex pInvalidContentRegex = null)
		{
			if (string.IsNullOrEmpty(pFileFullPath) || !File.Exists(pFileFullPath))
				return false;
			string pFileContent = File.ReadAllText(pFileFullPath);
			if (string.IsNullOrEmpty(pFileContent))
				return false;
			if (!pRegexToMatch.IsMatch(pFileContent))
				return false;
			if (null != pInvalidContentRegex)
			{
				if (pInvalidContentRegex.IsMatch(pFileContent))
					return false;
			}
			pFileContent = pRegexToMatch.Replace(pFileContent, pRegexToReplaceTo);
			File.Delete(pFileFullPath);
			File.WriteAllText(pFileFullPath, pFileContent);
			return true;
		}

		#endregion
	}
}

