// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillRangeController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class CrewPropPointController
{
    private int DEFAULT_SIZE = 120;//变量
    private const int DEFAULT_ITEM_SIZE = 14;

    private Texture2D ttSrc;//动态生成的贴图


    private List<int> tileList;
    private List<Vector2> pointList;

    public static CrewPropPointController Show(GameObject pParent,List<int> tileList)
    {
        List<IViewController> mCachedUIControllerList = null;
        List<GameObject> mUIList = null;
        var ctrl = AutoCacherHelper.AddChild<CrewPropPointController, CrewPropPoint>(
            pParent
            , CrewPropPoint.NAME
            , ref mUIList
            , ref mCachedUIControllerList);
        ctrl.gameObject.name = CrewPropPoint.NAME;
        ctrl.Show(tileList);

        return ctrl;
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void Show(List<int> _tileList)
    {
        if(tileList == _tileList) return;//一样的就不重绘
        tileList = _tileList;
        if(ttSrc == null)
        {
            ttSrc = ttSrc ?? new Texture2D(DEFAULT_SIZE,DEFAULT_SIZE,TextureFormat.ARGB32,false,false);
            ttSrc.filterMode = FilterMode.Point;
        }

        UpdatePoint();
        Draw();
        ttSrc.Apply();
        View.ttShow_UITexture.mainTexture = ttSrc;
    }

    private void UpdatePoint()
    {
        var circlePoint = new Vector2((DEFAULT_SIZE -2)/2,(DEFAULT_SIZE -2)/2);
        pointList = new List<Vector2>();
        for(int i = 0;i < tileList.Count;i++)
        {
            var point = new Vector2();
            var degree = i * 360 /tileList.Count;
            point.x = circlePoint.x + Mathf.Sin(degree * Mathf.Deg2Rad) * tileList[i] * DEFAULT_ITEM_SIZE;
            point.y = circlePoint.y + Mathf.Cos(degree * Mathf.Deg2Rad) * tileList[i] * DEFAULT_ITEM_SIZE;
            point.x = Mathf.FloorToInt(point.x);
            point.y = Mathf.FloorToInt(point.y);
            pointList.Add(point);
        }
    }

    private void Draw()
    {
        for(int i = 0;i < DEFAULT_SIZE;i++)
        {
            for(int j = 0;j < DEFAULT_SIZE;j++)
            {
                if(CeckPointInSide(i,j))
                {
                    ttSrc.SetPixel(i,j,Color.green);
                }
            }
        }
    }

    private bool CeckPointInSide(int x,int y)
    {
        int wn = 0,j = 0;
        for(var i = 0;i < pointList.Count;i++)
        {
            if(i == pointList.Count - 1)
            {
                j = 0;
            }else
            {
                j++;
            }
            if(pointList[i].y <= y)
            {
                if(pointList[j].y > y)
                {
                    if(LeftOfLine(pointList[i].x,pointList[i].y,pointList[j].x,pointList[j].y,x,y) > -1)
                    {
                        wn++;
                    }
                }
            }else
            {
                if(pointList[j].y <= y)
                {
                    if(LeftOfLine(pointList[i].x,pointList[i].y,pointList[j].x,pointList[j].y,x,y) < 1)
                    {
                        wn--;
                    }
                }
            }
        }
        return wn != 0;
    }

    //大于0左，小于0右
    public double LeftOfLine(float x1,float z1,float x2,float z2,float x,float z)
    {
        float t1 = (x2 - x1)*(z - z1) - (z2 - z1)*(x - x1);
        return t1;
    }
}
