using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BaseClassNS;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class NGUIPrefabAtlasEditorWindow: BaseEditorWindow
{
    private Object _selectedPrefab;

    private Dictionary<UISpriteData, Texture2D> _tempTextureDict = new Dictionary<UISpriteData, Texture2D>();


    [MenuItem("Custom/NGUIPrefabAtlas/Window")]
    private static NGUIPrefabAtlasEditorWindow Open()
    {
        var win = Open<NGUIPrefabAtlasEditorWindow>();

        return win;
    }


    protected override void CustomOnGUI()
    {
        Space();
        var tempPrefab = _selectedPrefab;
        _selectedPrefab = ObjectField("选择GameObject", _selectedPrefab, typeof (GameObject), true);
        if (tempPrefab != _selectedPrefab)
        {
            _tempTextureDict.Clear();
        }

        Space();
        if (_selectedPrefab != null)
        {
            var sprites = (_selectedPrefab as GameObject).GetComponentsInChildren<UISprite>(true);

            foreach (var sprite in sprites)
            {
                BeginVertical();
                {
                    BeginHorizontal();
                    {
                        ObjectField("", sprite.gameObject, typeof (GameObject), true);

                        ObjectField("", sprite.atlas != null ? sprite.atlas.gameObject : null, typeof (GameObject), true);

                        var spriteData = sprite.GetAtlasSprite();
                        if (spriteData != null)
                        {
                            if (!_tempTextureDict.ContainsKey(spriteData))
                            {
                                var paths = AssetDatabase.FindAssets(spriteData.name,
                                    new[] {Path.GetDirectoryName(AssetDatabase.GetAssetPath(sprite.atlas))});
                                for (int i = 0; i < paths.Length; i++)
                                {
                                    paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
                                }
                                var path =
                                    paths.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == spriteData.name);

                                if (!string.IsNullOrEmpty(path))
                                {
                                    _tempTextureDict[spriteData] = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;

                                }
                            }

                            ObjectField("",
                                _tempTextureDict.ContainsKey(spriteData) ? _tempTextureDict[spriteData] : null,
                                typeof (Texture2D));

                            if (_tempTextureDict.ContainsKey(spriteData))
                            {
                                Button("打开目录", () =>
                                {
                                    //                                    EditorUtility.OpenWithDefaultApp(Path.GetDirectoryName(AssetDatabase.GetAssetPath(_tempTextureDict[spriteData])));

                                    var path = AssetDatabase.GetAssetPath(_tempTextureDict[spriteData]);
                                    Debug.Log(path);
                                    EditorHelper.RevealInFinder(path);
                                });
                            }
                        }
                    }
                    EndHorizontal();
                }
                EndVertical();
            }
        }
    }
}
