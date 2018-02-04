using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaseClassNS;
using UnityEditor;

public class NGUIDepthEditorWindow : BaseEditorWindow
{
	public const int RightButtonWidth = 300;
	public const int BottomMessageHeight = 300;

	public const int CommonLabelDepth = 10;

	private enum ShowType
	{
		// 什么都不显示
		Empty,
		// 全部显示出来
		All,
		// UISprite没有UIAtlas
		EmptySprite,
		// UILabel没有UIFont
		EmptyLabel,
		// UILabel depth != 10
		WrongLabelDepth,
		// 相同depth但是不同mat
		SameDepthCombineDrawCallFail,
	}

	private ShowType _showType = ShowType.Empty;
	private ShowType _curShowType = ShowType.All;

	private NGUIDepthConfig _config;

	public NGUIDepthConfig Config
	{
		get
		{
			if (_config == null)
			{
				_config = NGUIDepthConfig.CreateConfig(SearchPathList);
			}

			return _config;
		}
	}

	public static readonly string[] SearchPathList =
	{
		"Assets/UI/Prefabs",
//		"Assets/UI/Prefabs/Module/ShopModule",
	};


	private Vector2 _leftPrefabScrollPos;
	private Vector2 _leftWidgetScrollPos;
	private string _searchString;

	private List<NGUIPrefabInfo> _prefabInfoList = new List<NGUIPrefabInfo>();
	private NGUIPrefabInfo _selectPrefabInfo;
	private Object _objectAdded;

	[MenuItem("Custom/NGUIDepth/Window")]
	private static NGUIDepthEditorWindow Open()
	{
		var win = Open<NGUIDepthEditorWindow>();

		return win;
	}


	protected override void CustomOnGUI()
	{
		Space();
		BeginVertical();
		{
			BeginHorizontal();
			{

				BeginVertical();
				{
					LeftTopPanel();
				}
				EndVertical();

				Space();

				BeginVertical(GUILayout.Width(RightButtonWidth));
				{
					RightTopPanel();
				}
				EndVertical();
			}
			EndHorizontal();

			Space();
			BeginHorizontal(GUILayout.Height(BottomMessageHeight));
			{
				BeginVertical();
				{
					LeftBottomPanel();
				}
				EndVertical();

				Space();

				BeginVertical(GUILayout.Width(RightButtonWidth));
				{
					RightBottomPanel();
				}
				EndVertical();
			}
			EndHorizontal();
		}
		EndVertical();
	}


	private void LeftTopPanel()
	{
		_searchString = TextField("", _searchString, SearchTextField);

		Space();
		_leftPrefabScrollPos = BeginScrollView(_leftPrefabScrollPos);
		{
			var _lowerSearch = _searchString == null ? null : _searchString.ToLower();
			foreach (var prefabInfo in _prefabInfoList)
			{
				if (string.IsNullOrEmpty(_searchString) || prefabInfo.Prefab.name.ToLower().Contains(_lowerSearch))
				{
					var info = prefabInfo;
					Button(AssetDatabase.GetAssetPath(prefabInfo.Prefab), () =>
					{
						_objectAdded = info.Prefab;
						_curShowType = _showType;
					}, LeftButtonStyle);
				}
			}
		}
		EndScrollView();
	}

	private void RightTopPanel()
	{
		EnumPopup<ShowType>("当前显示（不可修改）", _showType);

		Space();
		Button("刷新", () => RefreshConfig());

		Space();
		Button("显示全部", () => SwitchShowType(ShowType.All));

		Space();
		Button("空Sprite", () => SwitchShowType(ShowType.EmptySprite));

		Space();
		Button("空Label", () => SwitchShowType(ShowType.EmptyLabel));

		Space();
		Button(string.Format("错误Label Depth（{0}）", CommonLabelDepth), () => SwitchShowType(ShowType.WrongLabelDepth));

		Space();
		Button("同Depth不能合并DrawCall", () => SwitchShowType(ShowType.SameDepthCombineDrawCallFail));

		FlexibleSpace();

		Space();
		_objectAdded = ObjectField("场景物体：", _objectAdded, typeof (GameObject), true);
		if (_objectAdded != null)
		{
			var prefabType = PrefabUtility.GetPrefabType(_objectAdded);
			if (prefabType != PrefabType.Prefab && prefabType != PrefabType.PrefabInstance)
			{
				ShowNotification(new GUIContent(prefabType.ToString()));
				_objectAdded = null;
			}
		}
		if (_objectAdded != null && (_selectPrefabInfo == null || _selectPrefabInfo.Prefab != _objectAdded))
		{
			_selectPrefabInfo = new NGUIPrefabInfo(_objectAdded as GameObject);
		}
	}

	private void LeftBottomPanel()
	{
		_leftWidgetScrollPos = BeginScrollView(_leftWidgetScrollPos);
		{
			if (_objectAdded != null && _selectPrefabInfo != null)
			{
				var widgetList = GetWidgetInfoListByShowType(_selectPrefabInfo, _curShowType);
				widgetList.Sort((info1, info2) => info1.RaycastDepth.CompareTo(info2.RaycastDepth));
				var sb = new StringBuilder();
				foreach (var widgetInfo in widgetList)
				{
					sb.AppendLine(widgetInfo.ToString());
				}
				LabelField(sb.ToString(), EditorStyles.wordWrappedLabel);
			}
		}
		EndScrollView();
	}

	private void RightBottomPanel()
	{
		if (_objectAdded != null && _selectPrefabInfo != null)
		{
			Space();
			_curShowType = EnumPopup<ShowType>("当前显示（可修改）", _curShowType);

			Button("选中", () => EditorHelper.SelectObject(_selectPrefabInfo.Prefab));
			Space();
			Button(string.Format("修复Label Depth（{0}）", CommonLabelDepth), () => FixedLabelDepth(_selectPrefabInfo));

			Space();
			Button("压缩Depth", () => CompressWidgetDepth(_selectPrefabInfo));

			Space();
			Button("优化Depth（谨慎使用）", () => OptimizeWidgetsDepth(_selectPrefabInfo));
		}
	}

	private NGUIDepthConfig RefreshConfig()
	{
		_config = null;
		var config = Config;
		SwitchShowType(_showType);

		return config;
	}

	private void SwitchShowType(ShowType type)
	{
		_showType = type;
		_prefabInfoList.Clear();
		_prefabInfoList = GetPrefabInfoListByShowType(Config.InfoList, type);
	}

	private void ClearSelected()
	{
		_objectAdded = null;
		_selectPrefabInfo = null;
	}

	private List<NGUIPrefabInfo> GetPrefabInfoListByShowType(List<NGUIPrefabInfo> infoList, ShowType type)
	{
		var list = new List<NGUIPrefabInfo>();
		foreach (var info in infoList)
		{
			if (GetWidgetInfoListByShowType(info, type).Count > 0)
			{
				list.Add(info);
			}
		}
		return list;
	}

	private List<NGUIWidgetInfo> GetWidgetInfoListByShowType(NGUIPrefabInfo info, ShowType type)
	{
		var list = new List<NGUIWidgetInfo>();

		switch (type)
		{
			case ShowType.Empty:
				{
					break;
				}
			case ShowType.All:
				{
					list.AddRange(info.WidgetList);
					break;
				}
			case ShowType.EmptySprite:
				{
					list.AddRange(info.GetUISpriteEmptyList());
					break;
				}
			case ShowType.EmptyLabel:
				{
					list.AddRange(info.GetUILabelEmptyList());
					break;
				}
			case ShowType.WrongLabelDepth:
				{
					list.AddRange(info.GetUILabelNotSatisfiedDepthList(CommonLabelDepth));
					break;
				}
			case ShowType.SameDepthCombineDrawCallFail:
				{
					list.AddRange(info.GetSameDepthCannotCombineDrawCall());
					break;
				}
		}

		return list;
	}

	private void FixedLabelDepth(NGUIPrefabInfo info)
	{
		info.FixedLabelDepth(CommonLabelDepth);
	}

	private void CompressWidgetDepth(NGUIPrefabInfo info)
	{
		info.CompressWidgetDepth(CommonLabelDepth);
	}

	private void OptimizeWidgetsDepth(NGUIPrefabInfo info)
	{
		info.OptimizeWidgetsDepth(CommonLabelDepth);
	}
}
