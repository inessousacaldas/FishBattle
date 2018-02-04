// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFetterItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using AppDto;

public partial class CrewFetterItemController
{
    List<CrewFetterItemIconController> ItemIconList = new List<CrewFetterItemIconController>();
    //ICrewFetterItemVo vo;
    // 界面初始化完成之后的一些后续初始化工作

    //旋转过的角度
    int angle = 0;
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
    private void SetCheck(bool common)
    {
        _view.CheckMark_UISprite.gameObject.SetActive(common);
    }
    public void InitViewSpwan(int angle)
    {
        this.angle = angle;
        _view.transform.Rotate(UnityEngine.Vector3.forward, angle);
        //_view.RelateParners_UIGrid.transform.Rotate(UnityEngine.Vector3.forward, -angle);
        //if (angle > 180 && angle != 0)
        //    _view.RelateParners_UIGrid.pivot = UIWidget.Pivot.Left;
        //else if (angle <= 180 && angle != 0)
        //    _view.RelateParners_UIGrid.pivot = UIWidget.Pivot.Right;
        //else
        //    _view.RelateParners_UIGrid.pivot = UIWidget.Pivot.Center;

    }
    public void UpdateView(ICrewFetterItemVo vo,bool isChoice)
    {
        SetCheck(vo.Acitve);
        ItemIconList.ForEach(x =>
        {
            x.Hide();
        });
        int i = 0;
        var tempVoList = vo.CrewDic.ToList();
        //已经拥有的伙伴优先排序
        tempVoList.Sort((a, b) =>
        {
            if (a.Value == true && b.Value == false)
                return -1;
            else if (a.Value == false && b.Value == true)
                return 1;
            return 0;
        });
        foreach (var crew in tempVoList)
        {
            if(ItemIconList.Count < i + 1)
            {
                var con = AddChild<CrewFetterItemIconController, CrewFetterItemIcon>(_view.RelateParners_UIGrid.gameObject, CrewFetterItemIcon.NAME);
                var childWidget =  con.View.GetComponentsInChildren<UIWidget>(true);
                childWidget.ForEach(x => {
                    x.depth -= 3* i;
                });
                ItemIconList.Add(con);
                ItemIconList[i].InitAngle(this.angle);
            }
            ItemIconList[i].Show();
            ItemIconList[i].UpdateView(crew.Key,crew.Value, isChoice);
            i++;
        }
        _view.RelateParners_UIGrid.Reposition();
    }


    protected override void OnShow()
    {
        base.OnShow();
    }
    protected override void OnHide()
    {
        base.OnHide();
        _view.transform.rotation = UnityEngine.Quaternion.identity;
        _view.RelateParners_UIGrid.transform.rotation = UnityEngine.Quaternion.identity;
    }
}
