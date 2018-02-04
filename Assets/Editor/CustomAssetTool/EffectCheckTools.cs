using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetPipeline;
using UnityEditor;
using System;
using System.Text;

public class EffectCheckTools
{

    [MenuItem("Assets/资源导入/特效规范检测", false, 113)]
    public static void CheckEffect()
    {
        string resGroup = ResGroup.Effect.ToString().ToLower();
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        Dictionary<string, TexRef> paths = new Dictionary<string, TexRef>();
        foreach (var assetBundleName in assetBundleNames)
        {
            if (assetBundleName.StartsWith(resGroup) == false)
                continue;
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            foreach (var assetPath in assetPaths)
            {
                foreach (var dependenciesPath in AssetDatabase.GetDependencies(assetPath))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(dependenciesPath);
                    if (tex == null)
                        continue;

                    TexRef texRef = null;
                    if (paths.TryGetValue(dependenciesPath, out texRef))
                    {
                        texRef.AddRef(assetPath);
                    }
                    else
                    {
                        paths.Add(dependenciesPath, new TexRef(dependenciesPath, tex.width, tex.height, assetPath, CheckTexture(tex)));
                    }
                }
            }
        }

        using (StreamWriter streamWriter = new StreamWriter(File.Create("Assets/EffectCheck.csv")))
        {
            streamWriter.WriteLine(TexRef.OutHead());
            foreach (var item in paths)
            {
                streamWriter.Write(item.Value.ToString());
                streamWriter.WriteLine();
            }
            streamWriter.Close();
        }
        AssetDatabase.Refresh();
        Selection.objects = new[] { AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/EffectCheck.csv") };
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    public static bool CheckTexture(Texture2D tex)
    {
        if (tex.height == tex.width)
        {
            int num = tex.height;
            if (CheckPowerOf2(num))
            {
                //2的N次幂图 512以内符合规范
                return num <= 512;
            }
        }
        //其他情况 小于256符合规范
        return tex.height <= 512 && tex.width <= 512;
    }

    public static bool CheckPowerOf2(int num)
    {
        int mask = 1;
        int bitCount = 0;
        for (int i = 0; i < 32; i++)
        {
            int tempMask = mask << i;
            if ((num & tempMask) != 0)
            {
                bitCount++;
                if (bitCount > 1)
                    return false;
            }
        }
        return bitCount == 1;
    }

    private class TexRef
    {
        public List<string> refEffect;
        public readonly int width;
        public readonly int height;
        public readonly string assetPath;
        public readonly bool pass;

        public int refCount
        {
            get { return refEffect.Count; }
        }

        public TexRef(string assetPath, int width, int height, string dependenciesPath, bool pass)
        {
            this.assetPath = assetPath;
            this.pass = pass;
            this.width = width;
            this.height = height;
            refEffect = new List<string>();
            AddRef(dependenciesPath);
        }

        internal void AddRef(string dependenciesPath)
        {
            if (refEffect.Contains(dependenciesPath) == false)
                refEffect.Add(dependenciesPath);
        }

        public override string ToString()
        {
            StringBuilder refEffectString = new StringBuilder();
            refEffect.ForEach(item =>
            {
                refEffectString.Append(Path.GetFileName(item));
                refEffectString.Append(";");
            });
            return ToStringHelper(Path.GetFileNameWithoutExtension(assetPath), width, height, refCount, pass, assetPath, refEffectString);
        }

        public static string OutHead()
        {
            return "assetName,width,height,refCount,isPass,assetPath";
        }
        private string ToStringHelper(params object[] param)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var s in param)
            {
                stringBuilder.Append(s);
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString();
        }
    }
}
