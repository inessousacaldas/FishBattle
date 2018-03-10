// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPointController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AssetPipeline;
using UnityEngine;

public partial class RedPointController
{
    private RedPointType[] redPointEvtArr = null;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public static RedPointController Create(
        GameObject parent
        , RedPointType evt
        , ref Action act
        , int depth
        , bool isShowNum = false
        , string name = RedPoint.NAME){
        var arr = new RedPointType[1]{ evt}; 
        return Create(parent, arr, ref act, depth, isShowNum);
    }
    public static RedPointController Create(
        GameObject parent
        , RedPointType[] evtArr
        , ref Action act
        , int depth
        , bool isShowNum = false
        , string name = RedPoint.NAME){

        if (parent != null)
        {
            GameLog.LogRedPoint ("parent.name = " + parent.name);
        }

        var go = ResourcePoolManager.Instance.SpawnUIGo(name, parent);

        if (go ==  null) {
            return null;
        }

        UIHelper.AdjustDepthWithoutPanel (go, depth);

//        var ctrl = go.AddChild() new RedPointController(go);
//        ctrl.redPointEvtArr = evtArr;
//        ctrl.isShowNum = isShowNum;
//
//        var data = RedPointDataMgr.GetMergeData (ctrl.redPointEvtArr);
//        ctrl.redPointEvtArr.ForEach (s=>GameLog.LogRedPoint("evtArr elememt = "+ s.ToString()));
//        data.Print("AfterInitView data sample---");
//        ctrl.UpdateView (data);
//
//        act += delegate {
//            if (ctrl != null){
//                ctrl.Dispose();    
//            }
//
//            ctrl = null;
//        };
//        return ctrl;
        return null;
    }

    private bool isShowNum = false;
        
    #region 赋值
    private void UpdateRedPoint(RedPointType redPointEnum){
        bool b = redPointEvtArr.FindElementIdx (s => s == redPointEnum) > -1;

        if (b) {
            UpdateDataAndUI (redPointEnum);
        }
    }
       
    private void UpdateDataAndUI (RedPointType redPointEnum)
    {
        if (redPointEnum > RedPointType.invalid) {
            var data = RedPointDataMgr.GetMergeData (redPointEvtArr, redPointEnum);
            UpdateView (data);
        }
    }

    private void UpdateView(RedPointInfo data){
        data.Print ("redpoint ctrl---------UpdateView ");
        if (data == null) {
            _view.gameObject.SetActive (false);
        } else {
            data.Print ("redpoint controller evt response------repoint enum name" + data.redPointEnum.ToString());

            _view.gameObject.SetActive (data.IsShow());
            _view.Num_UILabel.gameObject.SetActive (isShowNum);
            _view.Num_UILabel.text = data.num.ToString ();
        }
    }
    #endregion

    protected override void OnDispose ()
    {
        transform.parent = null;
        ResourcePoolManager.Instance.DespawnUI (gameObject);
    }

}
