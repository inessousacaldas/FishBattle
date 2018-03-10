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

public partial class RoleSkillRangeController
{
    private int DEFAULT_SIZE = 66;//变量
    private const int DEFAULT_ITEM_SIZE = 14;

    private int col = 4;//行
    private int row = 4;//列
    private int border = 2;//边框的厚度
    private Texture2D ttSrc;//动态生成的贴图

    private Color[] colorBlack;//背景色
    private Color[] colorFriend;//友方
    private Color[] colorEnemy;//敌方
    private Color[] colorDefault;//默认

    private Color enemy = new Color(136f / 255f, 245f / 255f, 152f / 255f);
    private Color borderColor = new Color(207f / 255f, 205f / 255f, 206f / 255f);//边界颜色
    private Color lucency = new Color(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);//透明色

    private List<int> tileList;

    public static RoleSkillRangeController Show(GameObject pParent,List<int> tileList,RoleSkillMainRangeState state)
    {
        List<IViewController> mCachedUIControllerList = null;
        List<GameObject> mUIList = null;
        var ctrl = AutoCacherHelper.AddChild<RoleSkillRangeController, RoleSkillRange>(
            pParent
            , RoleSkillRange.NAME
            , ref mUIList
            , ref mCachedUIControllerList);
            ctrl.gameObject.name = RoleSkillRange.NAME;
        ctrl.Show(tileList,state);
        
        return ctrl;
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void Show(List<int> _tileList,RoleSkillMainRangeState state)
    {
        if(tileList == _tileList) return;//一样的就不重绘
        tileList = _tileList;
        DEFAULT_SIZE = (DEFAULT_ITEM_SIZE + border) * col + border;
        if(ttSrc == null)
        {
            ttSrc = ttSrc ?? new Texture2D(DEFAULT_SIZE,DEFAULT_SIZE,TextureFormat.ARGB32,false,false);
            ttSrc.filterMode = FilterMode.Point;
            colorBlack = MakeColors(DEFAULT_ITEM_SIZE, DEFAULT_ITEM_SIZE, border, Color.black, lucency);
            colorFriend = MakeColors(DEFAULT_ITEM_SIZE, DEFAULT_ITEM_SIZE, border, enemy, lucency);
            colorEnemy = MakeColors(DEFAULT_ITEM_SIZE, DEFAULT_ITEM_SIZE, border, enemy, lucency);
            colorDefault = MakeColors(DEFAULT_ITEM_SIZE, DEFAULT_ITEM_SIZE, border, Color.white, lucency);//默认的方格区域（填充，边界）
        }
        
        var stateColor = state == RoleSkillMainRangeState.Friend ? colorFriend : colorEnemy;
        var index = 1;//站位由1开始
        for(var i = 0;i < col;i++)
        {
            for(var j = 0;j < row ;j++)
            {
                var color = tileList.Contains(index) ? stateColor : colorDefault;
                ttSrc.SetPixels((border / 2) + j * (DEFAULT_ITEM_SIZE + border),(border / 2) + i * (DEFAULT_ITEM_SIZE + border),DEFAULT_ITEM_SIZE + border,DEFAULT_ITEM_SIZE + border,color);
                index++;
            }
        }

        for(var i = 0;i < DEFAULT_SIZE;i++)
        {
            for(var j = 0;j < DEFAULT_SIZE;j++)
            {
                if(i < border / 2 || i >= DEFAULT_SIZE - border / 2 || j < border / 2 || j >= DEFAULT_SIZE - border / 2)
                {
                    ttSrc.SetPixel(i,j, lucency);
                }
            }
        }
        ttSrc.Apply();
        View.ttShow_Texture.mainTexture = ttSrc;
    }

    private Color[] MakeColors(int w,int h,int border,Color color,Color borderColor)
    {
        var colorList = new Color[(w+border)*(h +border)];
        var col = w + border;
        var row = h + border;
        for(int i = 0;i < col;i ++)
        {
            for(int j = 0;j < row;j++)
            {
                if(i< (border-1) || i > w || j < (border-1) || j > h)
                {
                    colorList[i*col + j] = borderColor;
                }else
                {
                    colorList[i * col + j] = color;
                }
            }
        }
        return colorList;
    }
}
