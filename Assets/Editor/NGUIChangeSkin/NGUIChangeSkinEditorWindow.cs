using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BaseClassNS;
using AssetPipeline;
using UnityEditor;

public class NGUIChangeSkinEditorWindow : BaseEditorWindow
{
	public const int RightButtonWidth = 300;

	private Object _selectedPrefab;
	private NGUIPrefabDependence _selectedDependence;

	private Dictionary<Object, Object> _dependenceDict = new Dictionary<Object, Object>();
	private bool _needUpdate;

	private NGUIPrefabDependence.WidgetType _showWidgetType = NGUIPrefabDependence.WidgetType.All;


    private Object _curRes;
    private Object _newRes;


	[MenuItem("Custom/NGUIChangeSkin/Window")]
	private static NGUIChangeSkinEditorWindow Open()
	{
		var win = Open<NGUIChangeSkinEditorWindow>();

		return win;
	}

	protected override void CustomOnGUI()
	{
		Space();

		BeginHorizontal();
		{
			BeginVertical();
			{
				LeftTopPanel();
			}
			EndVertical();

			BeginVertical(GUILayout.Width(RightButtonWidth));
			{
				RightTopPanel();
			}
			EndVertical();
		}
		EndHorizontal();

        Space();
	    BeginHorizontal();
	    {
	        _curRes = ObjectField("旧资源：", _curRes, typeof (UIAtlas));
	        _newRes = ObjectField("新资源：", _newRes, typeof (UIAtlas));
	    }
        EndHorizontal();
        Space();
	    BeginHorizontal();
	    {
	        Button("输出使用了旧资源的Prefab", () => PrintUsingAtlasPrefab(_curRes as UIAtlas));
	        Button("一键替换所有", () => EditorHelper.Run(() => ChangeUsingAtlasPrefab(_curRes as UIAtlas, _newRes as UIAtlas)));
	    }
        EndHorizontal();
        Space();
	}


	private void LeftTopPanel()
	{
		_selectedPrefab = ObjectField("", _selectedPrefab, typeof(GameObject), true);
		if (_selectedPrefab != null && (_selectedDependence == null || _selectedDependence.Prefab != _selectedPrefab))
		{
			_selectedDependence = new NGUIPrefabDependence(_selectedPrefab as GameObject);
			_needUpdate = true;
			_dependenceDict.Clear();
		}

		if (_selectedPrefab != null)
		{
			if (_needUpdate)
			{
				UpdateDependenceDitc(_showWidgetType);
			}

			BeginTempScrollView("ResourcesSroll");
			{
				var tempKeyList = new List<Object>(_dependenceDict.Keys);
				tempKeyList.Sort((obj1, obj2) =>
				{
					var type1 = GetResourcesType(obj1);
					var type2 = GetResourcesType(obj2);

					if (type1 != type2)
					{
						return type1.CompareTo(type2);
					}
					else
					{
						return obj1.name.CompareTo(obj2.name);
					}
				});
				foreach (var key in tempKeyList)
				{
					BeginHorizontal();
					{
						ObjectField("", key, key.GetType());
						_dependenceDict[key] = ObjectField("", _dependenceDict[key], key.GetType());
					}
					EndHorizontal();
				}
			}
			EndTempScrollView();
		}
	}

	private void RightTopPanel()
	{
		var showType = _showWidgetType;
		_showWidgetType = EnumPopup<NGUIPrefabDependence.WidgetType>("显示内容：", _showWidgetType);
		if (showType != _showWidgetType)
		{
			_needUpdate = true;
		}

		Space();
		BeginTempScrollView("ButtonScroll");
		{
			Button("替换", ChangeSkin);
		}
		EndTempScrollView();
	}


	private void ChangeSkin()
	{
		foreach (var depend in _dependenceDict)
		{
			_selectedDependence.ChangeSkin(depend.Key, depend.Value);
		}
		UpdateDependenceDitc(_showWidgetType);
	}


	private void UpdateDependenceDitc(NGUIPrefabDependence.WidgetType type, bool clear = false)
	{
		_needUpdate = false;

		var tempDict = new Dictionary<Object, Object>();
		switch (type)
		{
			case NGUIPrefabDependence.WidgetType.None:
				{
					break;
				}
			case NGUIPrefabDependence.WidgetType.All:
				{
					foreach (var key in _selectedDependence.SpriteDict.Keys)
					{
						tempDict[key] = null;
					}

					foreach (var key in _selectedDependence.LabelDict.Keys)
					{
						tempDict[key] = null;
					}

					foreach (var key in _selectedDependence.TextureDict.Keys)
					{
						tempDict[key] = null;
					}

					break;
				}
			case NGUIPrefabDependence.WidgetType.Sprite:
				{
					foreach (var key in _selectedDependence.SpriteDict.Keys)
					{
						tempDict[key] = null;
					}
					break;
				}
			case NGUIPrefabDependence.WidgetType.Label:
				{
					foreach (var key in _selectedDependence.LabelDict.Keys)
					{
						tempDict[key] = null;
					}
					break;
				}
			case NGUIPrefabDependence.WidgetType.Texture:
				{
					foreach (var key in _selectedDependence.TextureDict.Keys)
					{
						tempDict[key] = null;
					}
					break;
				}
		}

		if (!clear)
		{
			foreach (var depend in _dependenceDict)
			{
				if (depend.Value != null && tempDict.ContainsKey(depend.Key))
				{
					tempDict[depend.Key] = depend.Value;
				}
			}
		}
		_dependenceDict = tempDict;
	}


	private enum ResourcesType
	{
		UIAtlas,
		UIFont,
		UITexture,
		ERROR,
	}


	private static ResourcesType GetResourcesType(Object obj)
	{
		if (obj is UIAtlas)
		{
			return ResourcesType.UIAtlas;
		}
		else if (obj is UIFont)
		{
			return ResourcesType.UIFont;
		}
		else if (obj is UITexture)
		{
			return ResourcesType.UITexture;
		}

		return ResourcesType.ERROR;
	}


    private static void PrintUsingAtlasPrefab(UIAtlas atlas)
    {
        var prefabList = GetUsingAtlasPrefabList(atlas);
        if (prefabList != null)
        {
            foreach (var resInfo in prefabList)
            {
                //Debug.Log(resInfo.path);
            }
        }
    }


    private static void ChangeUsingAtlasPrefab(UIAtlas oldAtlas, UIAtlas newAtlas)
    {
        if (oldAtlas == null || newAtlas == null || oldAtlas == newAtlas)
        {
            return;
        }
        
        //var prefabList = GetUsingAtlasPrefabList(oldAtlas);
        //if (prefabList != null)
        //{
        //    foreach (var resInfo in prefabList)
        //    {
        //        var prefab = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(resInfo.GUIDs[0])) as GameObject;
        //        var dependence = new NGUIPrefabDependence(prefab);
        //        dependence.ChangeSkin(oldAtlas, newAtlas);

        //        Debug.Log(string.Format("Change {0}", resInfo.path));
        //    }
        //}
    }


    private static List<ResInfo> GetUsingAtlasPrefabList(UIAtlas atlas)
    {
        if (atlas == null)
        {
            return null;
        }
        //TODO:换皮工具调整
        //return GetPrefabList().FindAll(info => info.refResList.FindIndex(s =>
        //{
        //    var name = s;
        //    var lastIndex = name.LastIndexOf("_");
        //    var prefix = name.Substring(0, lastIndex >= 0 ? lastIndex : name.Length);
        //    return prefix == atlas.name;
        //}) >= 0);
        return null;
    }


    private static List<ResInfo> GetPrefabList()
    {
        //TODO:换皮工具调整
        //var configPath = Path.Combine(GameResourceManager.EXPORT_FOLDER, GameResourceManager.RESCONFIG_FILE);

        //var resConfig = FileHelper.ReadJsonFile<ResConfig>(configPath);
        //var prefabList = new List<ResInfo>();
        //foreach (var resInfo in resConfig.allResDic.Values)
        //{
        //    if (resInfo.type == ResType.UIPrefab)
        //    {
        //        prefabList.Add(resInfo);
        //    }
        //}

        //return prefabList;
        return null;
    }
}
