// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  WindowOnlyMsgViewController.cs
// Author   : DM-PC092
// Created  : 7/26/2017 5:41:00 PM
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Manager;
using System;
using UniRx;
using UnityEngine;

public partial interface IWindowOnlyMsgViewController
{

}

public partial class WindowOnlyMsgViewController
{
    private static WindowOnlyMsgViewController instance;
    private static UnityEngine.GameObject mask = null;
    public static void Show(string msg)
    {
        if(instance == null || instance.View == null)//模块的gameobject可能会被释放了
        {
            instance = UIModuleManager.Instance.OpenFunModule<WindowOnlyMsgViewController>(
                    WindowOnlyMsgView.NAME
                    ,UILayerType.TopDialogue
                    ,false
                    ,false);
        }
        instance.UpdateView(msg);
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {

    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        mask = null;
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    public void UpdateView(string msg)
    {
        if (mask == null)
        {
            mask = UIComponentMgr.AddBgMask(View, View.transform, true, () =>
            {
                View.Hide();
            });
            mask.GetComponent<UISprite>().alpha = 1f / 255f;
        }
        char[] strArr = msg.ToCharArray ();
        if(strArr.Length < 19)
        {
            View.lblInfo_UILabel.alignment = NGUIText.Alignment.Center;
        }
        else
        {
            View.lblInfo_UILabel.alignment = NGUIText.Alignment.Left;
        }
        View.lblInfo_UILabel.text = msg;
        //View.spBg_UISprite.height = (int)View.lblInfo_UILabel.transform.localPosition.y + (int)View.lblInfo_UILabel.printedSize.y +10;
        var pos = View.transform.localPosition;
        var x = -View.lblInfo_UILabel.width / 2;
        //var y = View.lblInfo_UILabel.height / 2;
        View.transform.localPosition = new Vector3(x, 0);
        View.Show();
    }
}
