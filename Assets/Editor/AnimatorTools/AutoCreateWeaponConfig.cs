// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  Test.cs
// Author   : willson
// Created  : 2014/12/10 
// Porpuse  : 
// **********************************************************************

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AssetPipeline;

public class AutoCreateWeaponConfig : Editor
{
    private static WeaponConfig weaponConfig;
    private const string Config_Path = "Assets/GameResources/ConfigFiles/WeaponConfig/WeaponConfig.bytes";
    public static string[] MODELPREFAB_PATH = new string[] {
        "Assets/GameResources/ArtResources/Characters/Pet",
        "Assets/GameResources/ArtResources/Characters/Fashion/body"
    };

    [MenuItem("Tools/AutoCreateWeaponConfig", false, 2)]
    static void DoAutoCreateWeaponConfig()
    {
        if (EditorUtility.DisplayDialog("导出确认", "是否确认CreateWeaponConfig?", "确认", "取消"))
        {
            weaponConfig = new WeaponConfig();
            weaponConfig.list = new List<WeaponBindConfig>();

            string[] GUIDs = AssetDatabase.FindAssets("t:Prefab", MODELPREFAB_PATH);
            for (int i = 0; i < GUIDs.Length; ++i)
            {
                DoCreateAnimationAssets(AssetDatabase.GUIDToAssetPath(GUIDs[i]));
                EditorUtility.DisplayProgressBar("AutoCreateWeaponConfig", string.Format(" {0} / {1} ", i, GUIDs.Length), (float)i / (float)GUIDs.Length);
            }
            EditorUtility.ClearProgressBar();

            SaveConfig();

            Debug.Log(">>>>>>>>>>>  AutoCreateWeaponConfig 完成,请按 \"Ctrl + S\" 保存");
        }
    }

    static void SaveConfig()
    {
        FileHelper.SaveJsonObj(weaponConfig, Config_Path, false);
        AssetDatabase.Refresh();

        weaponConfig = null;

        weaponConfig = FileHelper.ReadJsonFile<WeaponConfig>(Config_Path, false);
        WeaponBindConfig config = weaponConfig.list[0];
        //DataHelper.SaveJsonFile (configInfo, BattleConfig_WritePath, false);
    }

    static void DoCreateAnimationAssets(string assetPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            string actorName = prefab.name;
            DoUpdateModelWeapon(go, actorName, "Bip001 Prop1");
            DoUpdateModelWeapon(go, actorName, "Bip001 Prop2");
            GameObject.DestroyImmediate(go);
        }
    }

    static void DoUpdateModelWeapon(GameObject go, string actorName, string bip001Name)
    {
        Transform tran = go.transform.Find(bip001Name);
        if (tran != null)
        {
            Debug.Log(go + " " + bip001Name);

            //Vector3 newPosition = tran.InverseTransformPoint(Vector3.zero);
            //Vector3 newEulerAngles = new Vector3(0f,-90f,-90f);

            GameObject newGo = new GameObject();
            newGo.transform.parent = tran;

            Vector3 newPosition = newGo.transform.localPosition;
            Vector3 newEulerAngles = newGo.transform.localEulerAngles;

            Debug.Log("newLocalPosition=" + newPosition.ToString());
            Debug.Log("newEulerAngles1=" + newEulerAngles.ToString());

            WeaponBindConfig config = new WeaponBindConfig();
            config.key = actorName + "/" + bip001Name;
            config.localPosition = newPosition;
            config.localEulerAngles = newEulerAngles;

            weaponConfig.list.Add(config);
        }
    }
}

