using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShaderFinder : EditorWindow
{
	string _shaderName = "";
	Vector2 _scrollPos;
	bool _wholeWordOnly = false;
	List<Object> _resultList = new List<Object> (10);
	
	[MenuItem("Window/ShaderFinder")]
	public static void ShowWindow ()
	{
		var window = EditorWindow.GetWindow (typeof(ShaderFinder));
		window.Show ();
	}

//	//场景发生变更时清空数据
//	void OnHierarchyChange(){
//		if(!Application.isPlaying)
//			_resultList.Clear();
//	}

	public void OnGUI ()
	{
		GUILayout.Label ("Enter shader to find:");
		
		EditorGUILayout.BeginHorizontal ();
		_shaderName = EditorGUILayout.TextField (_shaderName, GUILayout.Width (200f));
		_wholeWordOnly = EditorGUILayout.Toggle ("WholeWordOnly", _wholeWordOnly);
		EditorGUILayout.EndHorizontal ();

		GUILayout.Space (20f);

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("FindInScene", GUILayout.Height (50f))) {
			FindShaderInScene (_shaderName);
		}
		if (GUILayout.Button ("FindInFolder", GUILayout.Height (50f))) {
			FindShaderInFolder (_shaderName);
		}
		if (GUILayout.Button ("FindErrorInFolder", GUILayout.Height (50f))) {
			FindErrorShaderInFolder ();
		}
		if (GUILayout.Button ("FindInSelecttion", GUILayout.Height (50f))) {
			FindShaderInSelection (_shaderName);
		}
		if (GUILayout.Button ("FindErrorInSelecttion", GUILayout.Height (50f))) {
			FindErrorShaderInSelection();
		}

		GUILayout.Space (5f);
		if (GUILayout.Button ("SelectAll", GUILayout.Height (50f))) {
			if (_resultList != null && _resultList.Count > 0) {
				Selection.objects = _resultList.ToArray ();
			}
		}
		GUILayout.EndHorizontal ();

		EditorHelper.DrawHeader (string.Format ("Result: {0}", _resultList.Count));
		GUILayout.BeginHorizontal ();
		GUILayout.Space (3f);
		GUILayout.BeginVertical ();
		if (_resultList != null && _resultList.Count > 0) {
			_scrollPos = EditorGUILayout.BeginScrollView (_scrollPos);
			for (int i=0; i<_resultList.Count; ++i) {
				Object target = _resultList [i];
				if (target != null) {
					GUILayout.Space (-1f);
					GUI.backgroundColor = Selection.activeObject == target ? Color.white : new Color (0.8f, 0.8f, 0.8f);

					GUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (20f));
					
					GUI.backgroundColor = Color.white;
					GUILayout.Label (i.ToString (), GUILayout.Width (40f));
					
					if (GUILayout.Button (target.name, "OL TextField", GUILayout.Height (20f))) {
						Selection.activeObject = target;
					}
					GUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.EndScrollView ();
		} else {
			GUILayout.Box (string.Format ("Unable to find <{0}>", _shaderName));
		}
		GUILayout.EndVertical (); 
		GUILayout.Space (3f);
		GUILayout.EndHorizontal ();
	}

	private void FindShaderInScene (string shaderName)
	{
		Renderer[] renderers = Object.FindObjectsOfType<Renderer> ();
		if (renderers == null || renderers.Length == 0) {
			ShowNotification (new GUIContent ("当前场景下没有Renderer"));
		}

		_resultList.Clear ();
		for (int i=0; i<renderers.Length; ++i) {
			Renderer renderer = renderers [i];
			if (renderer.sharedMaterials != null) {
				for (int j=0; j<renderer.sharedMaterials.Length; ++j) {
					Material mat = renderer.sharedMaterials [j];
					if (mat != null) {
						if (_wholeWordOnly && mat.shader.name == shaderName) {
							_resultList.Add (renderer.gameObject);
						} else if (!_wholeWordOnly && mat.shader.name.Contains (shaderName)) {
							_resultList.Add (renderer.gameObject);
						}
					}
				}
			}

			EditorUtility.DisplayProgressBar ("搜索匹配的材质球", string.Format (" {0} / {1} ", i, renderers.Length),
			                                 (float)(i) / renderers.Length);
		}

		EditorUtility.ClearProgressBar ();
	}

	private void FindShaderInFolder (string shaderName)
	{
		Object[] results = Selection.GetFiltered (typeof(Material), SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.Editable | SelectionMode.ExcludePrefab);
		if (results == null || results.Length == 0) {
			ShowNotification (new GUIContent ("当前目录下没有材质球"));
		}

		_resultList.Clear ();
		for (int i=0; i<results.Length; ++i) {
			Material mat = results [i] as Material;
			if (mat != null) {
				if (_wholeWordOnly && mat.shader.name == shaderName) {
					_resultList.Add (mat);
				} else if (!_wholeWordOnly && mat.shader.name.Contains (shaderName)) {
					_resultList.Add (mat);
				}
			}

			EditorUtility.DisplayProgressBar ("搜索匹配的材质球", string.Format (" {0} / {1} ", i, results.Length),
			                                 (float)(i) / results.Length);
		}

		EditorUtility.ClearProgressBar ();
	}

	private void FindErrorShaderInFolder ()
	{
		Object[] results = Selection.GetFiltered (typeof(Material), SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.Editable | SelectionMode.ExcludePrefab);
		if (results == null || results.Length == 0) {
			ShowNotification (new GUIContent ("当前目录下没有材质球"));
		}
		
		_resultList.Clear ();
		for (int i=0; i<results.Length; ++i) {
			Material mat = results [i] as Material;
			if (mat != null) {
				if(mat.shader == null)
					_resultList.Add(mat);
				else if(string.IsNullOrEmpty(mat.shader.name))
					_resultList.Add(mat);
				else if(mat.shader.name == "Hidden/InternalErrorShader")
					_resultList.Add(mat);
			}
			
			EditorUtility.DisplayProgressBar ("搜索匹配的材质球", string.Format (" {0} / {1} ", i, results.Length),
			                                  (float)(i) / results.Length);
		}
		
		EditorUtility.ClearProgressBar ();
	}

	private void FindShaderInSelection (string shaderName)
	{
		Object[] results = Selection.objects;
		if (results == null || results.Length == 0) {
			ShowNotification (new GUIContent ("当前选中列表为空"));
		}
		
		_resultList.Clear ();
		for (int i=0; i<results.Length; ++i) {
			Material mat = results [i] as Material;
			if (mat != null) {
				if (_wholeWordOnly && mat.shader.name == shaderName) {
					_resultList.Add (mat);
				} else if (!_wholeWordOnly && mat.shader.name.Contains (shaderName)) {
					_resultList.Add (mat);
				}
			}
			
			EditorUtility.DisplayProgressBar ("搜索匹配的材质球", string.Format (" {0} / {1} ", i, results.Length),
			                                  (float)(i) / results.Length);
		}
		
		EditorUtility.ClearProgressBar ();
	}

	private void FindErrorShaderInSelection ()
	{
		Object[] results = Selection.objects;
		if (results == null || results.Length == 0) {
			ShowNotification (new GUIContent ("当前选中列表为空"));
		}
		
		_resultList.Clear ();
		for (int i=0; i<results.Length; ++i) {
			Material mat = results [i] as Material;
			if (mat != null) {
				if(mat.shader == null)
					_resultList.Add(mat);
				else if(string.IsNullOrEmpty(mat.shader.name))
					_resultList.Add(mat);
				else if(mat.shader.name == "Hidden/InternalErrorShader")
					_resultList.Add(mat);
			}
			
			EditorUtility.DisplayProgressBar ("搜索匹配的材质球", string.Format (" {0} / {1} ", i, results.Length),
			                                  (float)(i) / results.Length);
		}
		
		EditorUtility.ClearProgressBar ();
	}
}