#if UNITY_EDITOR 
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(GameObject))]
public class ReplaceView : MonoBehaviour
{

    public List<GameObject> prefab = new List<GameObject>();
    public bool needCheck = true;

    public void DrawBtn()
    {
        for (int i = 0, max = prefab.Count; i < max; i++)
        {
            var e = prefab[i];
            if (e == null)
            {
                Debug.LogError("当前列表没有放置物体");
                continue;
            }
            string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/UI/Prefabs/Module" });
            for (int j = 0; j < ids.Length; j++)
            {
                string path = AssetDatabase.GUIDToAssetPath(ids[j]);
                GameObject g1 = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                foreach (Transform item in g1.transform)
                {
                    if (item.name == e.name)
                    {
                        if (needCheck)
                        {
                            if (item.FindChild("check") != null)
                            {
                                ReplayceObj(e, g1, item, path);
                                break;
                            }
                        }
                        else
                        {
                            ReplayceObj(e, g1, item, path);
                            break;
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }
        EditorUtility.DisplayDialog("Replace", "替换完成，请进行检查", "确定");
    }

    void ReplayceObj(GameObject e, GameObject g1, Transform item, string path)
    {
        GameObject ts = PrefabUtility.InstantiatePrefab(e) as GameObject;
        GameObject g2 = PrefabUtility.InstantiatePrefab(g1) as GameObject;
        string titleName = "";
        foreach (Transform g2Trans in g2.transform)
        {
            if (g2Trans.name == e.name)
            {
                if (g2Trans.Find("TitleBg/TitleNameSprite"))
                {
                    UISprite title = g2Trans.Find("TitleBg/TitleNameSprite").GetComponent<UISprite>();
                    if (title == null)
                        Debug.LogError("没有检测到" + g2.name + "的title名字，请手动更换");
                    else
                        titleName = title.spriteName;
                }
                else
                {
                    Debug.LogError("当前" + g2.name + "不是用的公共预制，请自己手动替换，并且进行UI测试");
                    DestroyImmediate(ts);
                    break;
                }
                if (needCheck)
                {
                    if (g2Trans.FindChild("check") != null)
                    {
                        DestroyImmediate(g2Trans.gameObject);
                        break;
                    }
                }
                else
                {
                    DestroyImmediate(g2Trans.gameObject);
                    break;
                }
            }
        }
        if (ts == null) return;
        UISprite newTitle = ts.transform.Find("TitleBg/TitleNameSprite").GetComponent<UISprite>();
        if (newTitle == null)
            Debug.LogError("没有获取到新生成的公共预制的title,所以不能进行Titlt的更换，请手动~~~");
        else
        {
            newTitle.spriteName = titleName;
            newTitle.MakePixelPerfect();
            newTitle.transform.localPosition = new Vector3(0, -1, 0);
        }
        ts.transform.parent = g2.transform;
        ts.transform.localPosition = Vector3.zero;
        ts.transform.localScale = Vector3.one;
        var pre = PrefabUtility.CreatePrefab(path, g2, ReplacePrefabOptions.ConnectToPrefab);
        Debug.Log("替换了：" + g2.name);
    }
}
#endif
