// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ErrorViewController.cs
// Author   : fmd
// Created  : 7/28/2017 3:42:37 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IErrorViewController
{
    PageTurnViewController PageTurnCtrl { get;}
    string ErrorMsg { get; }
    void OnPageChange(int pageIndex);
    void ShowError(string s,string detail = "",bool needLog = true);
}
public partial class ErrorViewController
{
    private int count = 0;
    private string errorMsg = "";
    private List<string> errList;
    private PageTurnViewController pageTurnCtrl;
    public static IErrorViewController Show<T>(
          string moduleName
          ,UILayerType layerType
          ,bool addBgMask
          ,bool bgMaskClose = true)
          where T : MonoController, IErrorViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                    moduleName
                    , layerType
                    , addBgMask
                    , bgMaskClose) as IErrorViewController;

        return controller;
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitPageTurn();
    }

    //初始化操作次数控制栏
    private void InitPageTurn()
    {
        pageTurnCtrl = AddController<PageTurnViewController,PageTurnView>(
            View.pageTurn
            );

        pageTurnCtrl.InitData(
            showType: ShowType.numType
            ,enableInput: true);

    }

    public PageTurnViewController PageTurnCtrl { get { return pageTurnCtrl; } }
    public string ErrorMsg { get { return errorMsg; } }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {

    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        if(errList != null)
        {
            errList.Clear();
        }
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    public void ShowError(string s,string detail="",bool needLog = true)
    {
        s = GameDebuger.GetStackTrace(s);
        
        if(string.IsNullOrEmpty(detail) == false)
        {
            string[] detailArgs = detail.Split(',');
            s = string.Format(s,detailArgs);
        }
        if(errList == null)
        {
            errList = new List<string>();
        }
        if(count < 50)
        {
            if(s.Length > 8000)
            {
                s = s.Substring(0,8000);
            }
            errList.Add(s);
            LogErrorPanelMsg();
            count++;
        }
    }

        private void LogErrorPanelMsg()
    {
        errorMsg = View.lblMsg_UILabel.text = errList[errList.Count - 1];
        ResetPage();
    }

    public void OnPageChange(int pageIndex)
    {
        if(pageIndex < errList.Count)
        {
            View.lblMsg_UILabel.text = errList[errList.Count - 1 - pageIndex];
        }
        pageTurnCtrl.UpdateView(pageIndex,errList.Count);
    }

    private void ResetPage()
    {
        pageTurnCtrl.UpdateView(0,errList.Count);
    }

}
