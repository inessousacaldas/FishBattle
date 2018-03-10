// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  NavigationArea.cs
// Author   : willson
// Created  : 2014/12/16 
// Porpuse  : 
// **********************************************************************
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using AppDto;

public class NavigationArea
{
	private Vector3 _posTopLeft;
	private Vector3 _posBottomRight;

	private List< List<NavigationPoin> > _poins;

	public NavigationArea()
    {
        float topLeft_x, topLeft_z, bottomRight_x, bottomRight_z;
        GetNavMeshRect(out topLeft_x, out topLeft_z, out bottomRight_x, out bottomRight_z);

        _posTopLeft = new Vector3((int)topLeft_x, 0, (int)topLeft_z);
        _posBottomRight = new Vector3((int)bottomRight_x, 0, (int)bottomRight_z);

        _poins = new List<List<NavigationPoin>>();
    }

    private static void GetNavMeshRect(out float topLeft_x, out float topLeft_z, out float bottomRight_x, out float bottomRight_z)
    {
        topLeft_x = 0;
        topLeft_z = 0;
        bottomRight_x = 0;
        bottomRight_z = 0;
        var nodes = AstarPath.active.astarData.navmesh.nodes;
        List<Vector3> posList = new List<Vector3>();
        foreach (var node in nodes)
        {
            for (int i = 0; i <  3; i++)
            {
                posList.Add((Vector3)node.GetVertex(i));
            }
        }
        // 分析 vertices 取得 TopLeft BottomRight
        foreach (Vector3 pos in posList)
        {
            if (pos.x < topLeft_x)
            {
                topLeft_x = pos.x;
            }

            if (pos.z > topLeft_z)
            {
                topLeft_z = pos.z;
            }

            //////

            if (pos.x > bottomRight_x)
            {
                bottomRight_x = pos.x;
            }

            if (pos.z < bottomRight_z)
            {
                bottomRight_z = pos.z;
            }
        }
    }

    public void CreateMoveNavigation()
	{
        int resId = WorldManager.Instance.GetModel().GetSceneDto().sceneMap.resId;
        // 隐藏 way all 显示 way
        string map = "World_" + resId;
		Transform sceneRoot = LayerManager.Root.SceneLayer.transform;

   //     if(sceneRoot == null)
   //     {
   //         TipManager.AddTip("地图文件错误,SceneLayer层 找不到地图文件: " + map);
			//Debug.LogError("地图文件错误,SceneLayer层 找不到地图文件: " + map);
   //         return;
   //     }

        Transform wayAlltf = sceneRoot.Find(resId + "_way_all");
        if(wayAlltf == null)
        {
            TipManager.AddTip(string.Format("地图文件错误,{0} 找不到地图GameObject: way_all", map));
			Debug.LogError(string.Format("地图文件错误,{0} 找不到地图GameObject: way_all", map));
            return;
        }

        Transform waytf = sceneRoot.Find(resId + "_way");
        if (waytf == null)
        {
            TipManager.AddTip(string.Format("地图文件错误,{0} 找不到地图GameObject: way", map));
			Debug.LogError(string.Format("地图文件错误,{0} 找不到地图GameObject: way", map));
            return;
        }

        if (wayAlltf.gameObject != null)
            wayAlltf.gameObject.SetActive(false);
        if (waytf.gameObject != null)
            waytf.gameObject.SetActive(true);

		bool needCheckBattlePoint = resId == 1001;

		for(int z = (int)_posTopLeft.z;z >= (int)_posBottomRight.z;z--)
		{
			List<NavigationPoin> poins = new List<NavigationPoin>();
			for(int x = (int)_posTopLeft.x;x <= (int)_posBottomRight.x;x++)
			{
				NavigationPoin poin = new NavigationPoin(new Vector3(x,0,z), needCheckBattlePoint); 
				poins.Add(poin);
			}
			_poins.Add(poins);
		}
        // 隐藏 way 显示 way all
        if (wayAlltf.gameObject != null)
            wayAlltf.gameObject.SetActive(true);
        if (waytf.gameObject != null)
            waytf.gameObject.SetActive(false);
	}

	public void OutputFile(string path)
	{
		//path = Application.dataPath + "/Docs/NavigationArea/";
		SceneDto dto = WorldManager.Instance.GetModel().GetSceneDto();
		string fileName = dto.id + ".txt";
		File.Delete(path + fileName); 

		string fileName2 = string.Format("map_{0}.txt",dto.id);
		File.Delete(path + fileName2);

		FileInfo file = new FileInfo(path + fileName); 
		FileInfo fileMap = new FileInfo(path + fileName2); 

		StreamWriter sw = file.CreateText();
		StreamWriter swMap = fileMap.CreateText();

		string str = "";
		for (int z = 0;z < _poins.Count;z++)
		{
			List<NavigationPoin> poins = _poins[z];
			string line = "";
			string lineMap = "";

			for(int x = 0;x < poins.Count;x++)
			{
				NavigationPoin npoin = poins[x];
				line += npoin.ToString() + ",";
				lineMap += npoin.canMove()? (npoin.canBattle()?"2,":"1,"):"0,";
			}

			line = line.Substring(0,line.Length - 1);
			lineMap = lineMap.Substring(0,lineMap.Length - 1);

			if(z != _poins.Count -1)
			{
				sw.WriteLine(line);
			}
			else
			{
				sw.Write(line);
			}

			swMap.WriteLine(lineMap);
		}

		sw.Close();
		sw.Dispose();

		swMap.Close();
		swMap.Dispose();

		string tips = string.Format("地图:{0} - {1} 行走数据成功成功,请察看文件目录 {2}",dto.name,dto.id,path);
		TipManager.AddTip(tips);
		Debug.Log(tips);
	}
}