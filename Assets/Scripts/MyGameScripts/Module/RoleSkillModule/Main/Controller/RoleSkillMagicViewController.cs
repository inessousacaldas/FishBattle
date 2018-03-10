// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillMagicViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Manager;
using Assets.Scripts.MyGameScripts.UI;
using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;
public interface IRoleSkillMagicViewController
{
    UniRx.IObservable<Unit> OnbtnUp_UIButtonClick { get; }
}

public partial class RoleSkillMagicViewController
{
    private Vector3[] posList = new Vector3[]
    {
        new Vector3(-177f,90.8f),new Vector3(-40.1f,-18.6f),new Vector3(-92.9f,-163f),new Vector3(-257.3f,-163f),
        new Vector3(-311.2f,-18.6f)
    };

    private List<RoleSkillCraftsItemController> _itemList = new List<RoleSkillCraftsItemController>();
    public RoleSkillCraftsItemController defaultItem;

    private RoleSkillRangeController rangeCtrl;//技能范围
    private bool setFirst = false;
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
        _itemList.Clear();
        setFirst = false;
        defaultItem = null;
        rangeCtrl = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    private void CreateItem(IRoleSkillMainData data)
    {
        if(_itemList.Count > 0) return;
        var dict = data.MagicOpenDic;
        var eNum = dict.GetEnumerator();
        var i = 0;
        while(eNum.MoveNext())
        {
            var key = eNum.Current.Key;
            var itemCtrl = AddChild<RoleSkillCraftsItemController, RoleSkillCraftsItem>(View.leftContent_Transform.gameObject, RoleSkillCraftsItem.NAME, RoleSkillCraftsItem.NAME);
            itemCtrl.View.transform.localPosition = posList[i];
            RoleSkillDataMgr.RoleSkillMainViewLogic.InitReactiveEvents(itemCtrl);
            _itemList.Add(itemCtrl);
            i++;
        }
    }

    public void UpdateView(IRoleSkillMainData data)
    {
        CreateItem(data);
        UpdatItem(data);
        UpdateRight(data,data.CurSelMagicCtrl);
    }

    private void UpdatItem(IRoleSkillMainData data)
    {
        var openDic = data.MagicOpenDic;
        var magicDic = data.MagicDtoDic;
        int magicCount = 0;
        if (magicDic != null)
            magicCount = magicDic.Count;
        int playerLv = ModelManager.Player.GetPlayerLevel();
        int idx = 0;
        openDic.ForEach(e =>
        {
            if (playerLv >= e.Key)
            {
                if (magicCount > idx)
                {
                    _itemList[idx].UpdateView(true,e.Key,magicDic[idx]);
                }
                else
                {
                    _itemList[idx].UpdateView(true,e.Key);
                }
            }
            else
            {
                _itemList[idx].UpdateView(false,e.Key);
            }
            idx++;
        });
    }

    public void UpdateRight(IRoleSkillMainData data, RoleSkillCraftsItemController itemCtrlNoUse)
    {
        RoleSkillCraftsItemController itemCtrl = null;
        if (!setFirst)
        {
            foreach (var value in _itemList)
            {
                if (value.magicVo != null && value.magicVo.isEquip)
                {
                    itemCtrl = value;
                    data.CurSelMagicCtrl = itemCtrl;
                    setFirst = true;
                    break;
                }
            }
        }
        else
        {
            itemCtrl = itemCtrlNoUse;
        }
        if (itemCtrl != null && itemCtrl.magicVo!= null)
        {
            View.lblCraftsName_UILabel.text = itemCtrl.magicVo.Name;
            View.lblDesc_UILabel.alignment = NGUIText.Alignment.Left;
            View.lblDesc_UILabel.text = data.GetMagicDesc(itemCtrl.magicVo.id);
            itemCtrl.SetSel(true);
            UpdateRange(data,itemCtrl.magicVo);
        }
        else
        {
            View.lblCraftsName_UILabel.text = "魔法";
            View.lblDesc_UILabel.alignment = NGUIText.Alignment.Center;
            View.lblDesc_UILabel.text = "\n\n\n\n\n你还没有装备任何魔法技能，请前往导力器界面进行装备魔法技能。";
            if (rangeCtrl != null)
            {
                rangeCtrl.Hide();
            }
        }
    }

    private void UpdateRange(IRoleSkillMainData data,RoleSkillMagicVO itemVO)
    {
        var scopeVO = DataCache.getDtoByCls<SkillScope>(itemVO.cfgVO.scopeId);
        var type = data.GetScopeTarType(itemVO.cfgVO.scopeId);
        if(rangeCtrl == null)
        {
            rangeCtrl = RoleSkillRangeController.Show(View.rightContent_Transform.gameObject,scopeVO.scopeIndex,type);
            rangeCtrl.transform.localPosition = new Vector3(307,56);
        }
        else
        {
            rangeCtrl.Show();
            rangeCtrl.Show(scopeVO.scopeIndex,type);
        }
    }
}
