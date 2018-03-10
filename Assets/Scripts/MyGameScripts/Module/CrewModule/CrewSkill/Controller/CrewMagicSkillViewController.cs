// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewMagicSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using System.Collections.Generic;
using UniRx;

public partial interface ICrewMagicSkillViewController
{
    //UniRx.IObservable<ICrewSkillItemController> MagicSkillEquip { get; }

    UniRx.IObservable<ICrewSkillItemController> MagicSkillIconClick { get; }

    void SetData(ICrewSkillData data);
    void UpdateView(int id);
    void UpdateSkillDescripe(CrewSkillItemController skillItem);
}

public partial class CrewMagicSkillViewController
{
    private CompositeDisposable _disposable;

    //private Subject<ICrewSkillItemController> _magicSkillEquip = new Subject<ICrewSkillItemController>();
    //public UniRx.IObservable<ICrewSkillItemController> MagicSkillEquip { get { return _magicSkillEquip; } }

    private Subject<ICrewSkillItemController> _magicSkillIconClick = new Subject<ICrewSkillItemController>();
    public UniRx.IObservable<ICrewSkillItemController> MagicSkillIconClick { get { return _magicSkillIconClick; } }

    private ICrewSkillData _data;
    private CrewSkillItemController _curSkillItem;

    private int _curCrewId;

    private RoleSkillRangeController _rangeCtrl; //技能范围

    private List<CrewSkillItemController> _itemList = new List<CrewSkillItemController>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnMagicEquipBtn_UIButtonClick.Subscribe(_ => { ProxyQuartz.OpenQuartzMainView(); }));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _data = null;
        _curSkillItem = null;
        if (_itemList.Count > 0) _itemList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
        //EventDelegate.Add(View.MagicEquipBtn_UIButton.onClick, OnMagicEquipClick);
    }
    private void OnMagicEquipClick()
    {
        ProxyQuartz.OpenQuartzMainView();
    }

    public void SetData(ICrewSkillData data)
    {
        _data = data;
    }

    public void UpdateView(int id)
    {
        var list = _data.GetSkillCrafts(id);
        if (list == null) return;
        CreateSkillItem();
        UpdateSkillItem(id);

        if (_curSkillItem == null || _curCrewId != id)
            UpdateSkillDescripe(_itemList[0]);
        else
            UpdateSkillDescripe(_curSkillItem);

        _curCrewId = id;
    }

    public void CreateSkillItem()
    {
        if (_itemList.Count > 0) return;
        for (int i = 0; i < _view.SkillIconGroup_Transform.childCount; i++)
        {
            var lb = _view.SkillIconGroup_Transform.GetChild(i).gameObject;
            var com = AddController<CrewSkillItemController, CrewSkillItem>(lb);
            _itemList.Add(com);
            _disposable.Add(com.OnCrewSkillItem_UIButtonClick.Subscribe(_ => { _magicSkillIconClick.OnNext(com); }));
        }
    }
    public void UpdateSkillItem(int crewId)
    {
        var openDic = _data.MagicOpenDic;
        var magicList = _data.GetSkillMagic(crewId);
        int magicCount = 0;
        if (magicList != null)
            magicCount = magicList.Count;
        int playerLv = CrewViewDataMgr.DataMgr.GetCurCrewGrade;
        int openCount = 0;

        openDic.ForEachI((e, idx) =>
        {
            if (playerLv >= e.Key)
            {
                //已配置和未配置
                openCount = e.Value;
                if (magicCount > idx)
                    _itemList[idx].UpdateMagicView(true, e.Key, magicList[idx]);
                else
                    _itemList[idx].UpdateMagicView(true, e.Key);
            }
            else
            {
                //未解锁
                _itemList[idx].UpdateMagicView(false, e.Key);
            }
        });
    }

    public void UpdateSkillDescripe(CrewSkillItemController skillItem)
    {
        _curSkillItem = skillItem;
        CrewSkillMagicVO magic = skillItem.SkillVO as CrewSkillMagicVO;
        if (magic != null)
        {
            _view.SkillNameLbl_UILabel.text = magic.Name;
            _view.SkillLevelLbl_UILabel.text = string.Format("等级：{0}级", magic.Grade);
            _view.SkillTypeLbl_UILabel.text = "魔法";
            _view.SkillBeforeLbl_UILabel.text = magic.SkillTimeBefore;
            _view.SkillNatureLbl_UILabel.text = magic.SkillType;
            _view.SkillScopeLbl_UILabel.text = magic.Scope;
            _view.SkillConsumeLbl_UILabel.text = magic.magicVO.consume + "CP";
            _view.SkillEffectLabel_UILabel.text = RoleSkillUtils.Formula(magic.SkillDes, magic.Grade);
            UpdateRightRange(magic);
        }
        else
        {
            ClearDescripe();
        }
    }
    private void UpdateRightRange(CrewSkillMagicVO magicVo)
    {
        CrewSkillCraftsData tmp = CrewSkillHelper.GetCraftsData();
        var scopeVO = tmp.GetScopeByID(magicVo.magicVO.scopeId);
        var type = tmp.GetScopeTarType(magicVo.magicVO.scopeId);
        if (_rangeCtrl == null)
        {
            _rangeCtrl = RoleSkillRangeController.Show(View.rangeTrans_Transform.gameObject, scopeVO.scopeIndex, type);
            _rangeCtrl.transform.localPosition = new UnityEngine.Vector3(0, 0);
            _rangeCtrl.transform.localScale = UnityEngine.Vector3.one;
        }
        else
        {
            _rangeCtrl.Show(scopeVO.scopeIndex, type);
            if (!_rangeCtrl.View.gameObject.activeSelf)
                _rangeCtrl.View.gameObject.SetActive(true);
        }
    }
    public void ClearDescripe()
    {
        _view.SkillNameLbl_UILabel.text = string.Empty;
        _view.SkillLevelLbl_UILabel.text = string.Empty;
        _view.SkillTypeLbl_UILabel.text = "魔法";
        _view.SkillBeforeLbl_UILabel.text = string.Empty;
        _view.SkillNatureLbl_UILabel.text = string.Empty;
        _view.SkillScopeLbl_UILabel.text = string.Empty;
        _view.SkillConsumeLbl_UILabel.text = string.Empty;
        _view.SkillEffectLabel_UILabel.text = string.Empty;
        if (_rangeCtrl != null)
            _rangeCtrl.View.gameObject.SetActive(false);
    }

}
