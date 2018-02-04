using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Refactor
{
	/// <summary>
	/// H1老旧代码的重构专用常量
	/// @MarsZ 2016-11-17 17:31:02
	/// </summary>
	public class RefactorConst
	{
		public const string DEFAULT_PROXY_FILE_SEARCH_DIR_UNDER_PROJECT = "Assets/Scripts/MyGameScripts";
		public const string DIR_MYGAMESCRIPTS = "Assets/Scripts/MyGameScripts";
		public static readonly string[] DIR_MYGAMESCRIPTS_S = new string[]{ "Assets/Scripts/MyGameScripts" };

        public const string KEYWORD_H1MODEL = @"(public)*?(\s)*?class(\s)+?(?<STR>[_a-zA-Z0-9]+)Model(\s)*?";

        public const string PATTERN_FILE_MODEL = @"(?<str>[^\s]+?)Model\.cs";
		public const string KEYWORD_VIEW = @"public[\s]+class[\s]+[^\s]+[\s]+[:\s]+BaseView";
		public const string KEYWORD_CONTROLLER = @"public[\s]+?class[\s]+?[^\s]+?[\s]*?:[\s]*?MonoBehaviour[\s]*?\,[\s]*?IViewController";
		public const string KEYWORD_CONTROLLER2 = @"public[\s]+?class[\s]+?[^\s]+?Controller[\s]*?\:";
		public const string KEYWORD_MODEL = @"(public)*?(\s)*?class(\s)+?(?<STR>[_a-zA-Z0-9]+)Model(\s)*?:(\s)*?IModuleModel";
		public const string KEYWORD_MODELNAME = @"${STR}";

		public const int MAX_WIDTH = 800;
		public const int INPUT_VALUE_WIDTH = 600;
		public const int FIELD_WIDTH = 100;
		public const int BUTTON_HEIGHT = 50;
		public const int FIELD_TIP_WIDTH = 100;

		public static Regex RegexIsMonolessViewController = new Regex(@"public(\s)*?class(\s)*?[^:{,]+?(\s)*?\:(\s)*?MonolessViewController");
		public static Regex RegexIsMonoViewController = new Regex(@"public(\s)*?class(\s)*?[^:{,]+?(\s)*?\:(\s)*?MonoViewController");

		public static Regex RegexToGetProjectDirByFileFullPath = new Regex(@"(?<STR>[^. ]+?)Assets/(?<STR2>[^ ]+?)");
		public static string RegexToGetProjectDirByFileFullPathTo = @"${STR}";
		//@"${STR} ${STR2}";

		private static string mPROJECT_PATH = "";

		public static string PROJECT_PATH
		{
			get
			{
				if (string.IsNullOrEmpty(mPROJECT_PATH))
					mPROJECT_PATH = Application.dataPath.Replace("Assets", string.Empty);
//				mPROJECT_PATH = "/Users/cilu/Workspace/H5/S1ed_2/";
//				GameDebuger.LogError(string.Format("H5重构临时设定项目地址:{0}", mPROJECT_PATH));
				return  mPROJECT_PATH;
			}
		}
	}
}

