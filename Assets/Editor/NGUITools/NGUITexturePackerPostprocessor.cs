using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class NGUITexturePackerPostprocessor: AssetPostprocessor
{
    public override int GetPostprocessOrder()
    {
        return 666;
    }


    private void OnPostprocessTexture(Texture2D tex)
    {
//        ApplyTexturePackerSetting(assetPath);
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromPath)
    {
        foreach (var importedAsset in importedAssets)
        {
            if (Path.GetExtension(importedAsset) == ".txt")
            {
                ApplyTexturePackerSetting(importedAsset);
            }
        }
    }


    public static void ApplyTexturePackerSetting(string assetPath)
    {
        var path = assetPath;
        Debug.Log(path);
        
        var taPath = string.Format("{0}/{1}.txt", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        var ta = AssetDatabase.LoadMainAssetAtPath(taPath) as TextAsset;
        if (ta == null)
        {
            return;
        }

        var prefabPath = string.Format("{0}/{1}.prefab", Path.GetDirectoryName(path),
            Path.GetFileNameWithoutExtension(path));
        var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
        if (prefab == null)
        {
            return;
        }

        ApplyTexturePackerSetting(prefab.GetComponent<UIAtlas>(), ta);
    }


    public static void ApplyTexturePackerSetting(UIAtlas atlas, TextAsset ta)
    {
        if (atlas == null || ta == null)
        {
            return;
        }

        // Ensure that this atlas has valid import settings
        if (atlas.texture != null) NGUIEditorTools.ImportTexture(atlas.texture, false, false, !atlas.premultipliedAlpha);

        NGUIEditorTools.RegisterUndo("Import Sprites", atlas);
        NGUIJson.LoadSpriteData(atlas, ta);
        atlas.MarkAsChanged();
    }
}
