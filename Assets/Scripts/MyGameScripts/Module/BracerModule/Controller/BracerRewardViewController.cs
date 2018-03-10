// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerRewardViewController.cs
// Author   : xjd
// Created  : 11/20/2017 5:15:14 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface IBracerRewardViewController
{
    void UpdateView();
}
public partial class BracerRewardViewController    {

    public static IBracerRewardViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IBracerRewardViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IBracerRewardViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _labelItemList.Add(View.ProLabel_1_UILabel);
        _labelItemList.Add(View.ProLabel_2_UILabel);
        _labelItemList.Add(View.ProLabel_3_UILabel);
        _labelItemList.Add(View.ProLabel_4_UILabel);
        _labelItemList.Add(View.ProLabel_5_UILabel);
        _labelItemList.Add(View.ProLabel_6_UILabel);
        _labelItemList.Add(View.ProLabel_7_UILabel);

        _valueItemList.Add(View.Value_1_UILabel);
        _valueItemList.Add(View.Value_2_UILabel);
        _valueItemList.Add(View.Value_3_UILabel);
        _valueItemList.Add(View.Value_4_UILabel);
        _valueItemList.Add(View.Value_5_UILabel);
        _valueItemList.Add(View.Value_6_UILabel);
        _valueItemList.Add(View.Value_7_UILabel);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;
    }

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnClose;
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private Dictionary<int, BracerGrade> _bracerGradeDic = DataCache.getDicByCls<BracerGrade>();
    private List<UILabel> _labelItemList = new List<UILabel>();
    private List<UILabel> _valueItemList = new List<UILabel>();
    private Dictionary<int, CharacterAbility> _characterDic = DataCache.getDicByCls<CharacterAbility>();
    private int defaultBgHeight = 36;
    private int defaultBgWidth = 22;
    private int defaultWidth = 72;

    public void UpdateView()
    {
        var bracerGrade = ModelManager.Player.GetBracerGrade;
        // 读下一等级
        if (!_bracerGradeDic.ContainsKey(bracerGrade+1) || _bracerGradeDic[bracerGrade+1].attrAdd.Count != _bracerGradeDic[bracerGrade+1].attrId.Count)
        {
            //最高级处理 todo xjd
            return;
        }

        _labelItemList.ForEachI((item,idx) =>
        {
            item.enabled = false;
            _valueItemList[idx].enabled = false;
        });

        int index = 0;
        var labelMaxWidth = 0;

        //导力器孔
        if (_bracerGradeDic[bracerGrade + 1].slotsCount > 0)
        {
            _labelItemList[index].text = "导力器孔";
            _valueItemList[index].text = string.Format("+{0}", _bracerGradeDic[bracerGrade + 1].slotsCount);
            labelMaxWidth = _labelItemList[index].width;
            _labelItemList[index].enabled = true;
            _valueItemList[index].enabled = true;
            index++;
        }

        //属性
        _bracerGradeDic[bracerGrade + 1].attrId.ForEachI((id,idx) =>
        {
            if(_characterDic.ContainsKey(id))
            {
                _labelItemList[index].text = _characterDic[id].name;
                _valueItemList[index].text = string.Format("+{0}", _bracerGradeDic[bracerGrade + 1].attrAdd[idx]);
                _labelItemList[index].enabled = true;
                _valueItemList[index].enabled = true;
                if (_labelItemList[index].width > labelMaxWidth)
                    labelMaxWidth = _labelItemList[index].width;
                index++;
            }
        });

        //重置label的长度
        _valueItemList.ForEachI((itemLabel,idx) =>
        {
            if (itemLabel.enabled)
            {
                _valueItemList[idx].transform.localPosition = new Vector3(_valueItemList[idx].transform.localPosition.x + labelMaxWidth, 0);
            }
        });

        View.Table_UITable.Reposition();
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Table_UITable.transform);
        View.Bg_UISprite.height = defaultBgHeight + (int)b.size.y;
        View.Bg_UISprite.width = defaultBgWidth + (int)b.size.x;
    }

    private void OnClose(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
            UIModuleManager.Instance.CloseModule(BracerRewardView.NAME);
    }
}
