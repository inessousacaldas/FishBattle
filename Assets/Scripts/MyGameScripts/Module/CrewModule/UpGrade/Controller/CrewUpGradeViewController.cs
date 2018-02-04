// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewUpGradeViewController.cs
// Author   : xush
// Created  : 12/18/2017 5:53:51 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface ICrewUpGradeViewController
{

}

public partial class CrewUpGradeViewController
{
    private CrewIconController _crewIconCtrl;
    private ICrewBookData _crewData;
    private List<ItemCellController> _propList = new List<ItemCellController>(); 
    private string PropsIdStr = "";
    private int _lvMax;
    private int _playerLv;
    private List<Vector3> _posList = new List<Vector3>()
    {
        new Vector3(-358, 200, 0),
        new Vector3(-225, 208, 0),
        new Vector3(94, 208, 0)
    };
 
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        PropsIdStr = DataCache.GetStaticConfigValues(AppStaticConfigs.CREW_EXP_PROPS);
        _crewIconCtrl = AddController<CrewIconController, CrewIcon>(_view.CrewIcon);
        _playerLv = ModelManager.Player.GetPlayerLevel();
        for (int i = 0; i < _view.PropsGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.PropsGrid_UIGrid.GetChild(i);
            var ctrl = AddController<ItemCellController, ItemCell>(item.gameObject);
            _propList.Add(ctrl);
            var idx = i;
            _disposable.Add(ctrl.OnCellClick.Subscribe(_ => { OnPropsItemClick(idx); }));
        }
        
        _lvMax = DataCache.getArrayByCls<ExpGrade>().Count;
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICrewViewData data)
    {
        _crewData = data.CrewUpGradeData.GetBookDataById(data.GetCurCrewId);
        if (_crewData == null)
        {
            GameDebuger.LogError("=====数据出错,请查看====");
            return;
        }
        _crewIconCtrl.UpdateDataAndView(_crewData, -1, false);
        _crewIconCtrl.IsShowLv(false);
        var lv = _crewData.GetInfoDto.grade + 1 >= _lvMax ? 
            _crewData.GetInfoDto.grade : _crewData.GetInfoDto.grade + 1;
        ExpGrade grade = DataCache.getDtoByCls<ExpGrade>(lv);
        _view.Slider_UISlider.value = (float)_crewData.GetInfoDto.exp / (float)grade.petExp;
        _view.ExpLabel_UILabel.text = string.Format("{0}/{1}", _crewData.GetInfoDto.exp, grade.petExp);
        _view.LvLabel_UILabel.text = string.Format("{0}级", _crewData.GetInfoDto.grade);
        SetPropsList();
    }

    private void SetPropsList()
    {
        var idList = PropsIdStr.Split(';');
        int itemid;
        idList.ForEachI((id, i) =>
        {
            bool b = int.TryParse(id, out itemid);
            if (b)
            {
                var dto = BackpackDataMgr.DataMgr.GetItemByItemID(itemid);
                var cannot = dto == null ? true : 
                    (dto.item as Props).minGrade > _playerLv 
                    || (dto.item as Props).maxGrade < _playerLv;
                _propList[i].SetIconGrey(dto == null || cannot);
                _propList[i].Mark_UISprite = dto == null;
                if (dto == null)
                {
                    var prop = DataCache.getDtoByCls<GeneralItem>(itemid);
                    if (prop != null)
                        _propList[i].UpdateView(prop);
                    else
                        GameDebuger.LogError(string.Format("GeneralItem表找不到{0}", itemid));
                }
                else
                    _propList[i].UpdateView(dto);
            }
            else
                GameDebuger.LogError("伙伴升级道具id静态字段表有误,请检查");
        });
    }

    private void OnPropsItemClick(int idx)
    {
        var pos = _posList[idx];
        _propList[idx].GetTips.SetTipsPosition(pos, false);
    }
}
