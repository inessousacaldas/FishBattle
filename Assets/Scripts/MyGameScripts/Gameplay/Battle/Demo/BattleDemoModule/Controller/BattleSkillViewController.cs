// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IBattleSkillViewController
{
    UniRx.IObservable<int> GetCellClickHandler { get; }
    UniRx.IObservable<Unit> GetCloseHandler { get; }
    void CloseTips();
}

public partial class BattleSkillViewController
{
    private List<SkillItemController> _skillItemList = new List<SkillItemController>(); 
    private CompositeDisposable _disposable;

    private Subject<int> _onCellClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetCellClickHandler { get { return _onCellClickEvt; } }

    private Subject<Unit> _onCloseEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetCloseHandler { get { return _onCloseEvt; } }  

    private delegate void SkillItemClickFunc (SkillItemController item, int idx);

    private SkillTipsController _tipsCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        UICamera.onClick -= OnCameraClick;
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetSkillList(IEnumerable<Skill> skillList)
    {
        _view.SkillGrid_UIGrid.gameObject.SetActive(skillList.Count() != 0);
        _view.DescLabel_UILabel.gameObject.SetActive(skillList.Count() == 0);
        if (skillList.Count() == 0)
        {
            SetBackGround(skillList.Count());
            return;
        }
            
        SkillItemClickFunc func = (item, idx) =>
        {
            var skill = skillList.TryGetValue(idx);
            if (skill == null)
            {
                GameDebuger.LogError("找不到对应的技能,请检查");
                return;;
            }
            _disposable.Add(item.ItemClickHandler.Subscribe(_ => {_onCellClickEvt.OnNext(skill.id); }));
            _disposable.Add(item.GetPressHandler.Subscribe(_ => { OpenSkillTips(skill); }));
        };
        for (int i = _view.SkillGrid_UIGrid.transform.childCount; i < skillList.Count(); i++)
        {
            var item = AddChild<SkillItemController, SkillItem>(_view.SkillGrid_UIGrid.gameObject,
                SkillItem.NAME);

            _skillItemList.Add(item);
            func(item, i);
        }

        _skillItemList.ForEachI((item, idx) =>
        {
            var skill = skillList.TryGetValue(idx);
            if (skill == null)
                item.gameObject.SetActive(false);
            else
            {
                item.UpdateView(skill);
                item.SetBgSprite("CommonRing", 55, 55);
            }
        });
        SetBackGround(skillList.Count());
        _view.SkillGrid_UIGrid.Reposition();
        _view.SkillScrollView_UIScrollView.ResetPosition();
    }

    public void CloseTips()
    {
        if(_tipsCtrl != null)
            _tipsCtrl.Close();
    }

    private void SetBackGround(int skillCount)
    {
        if (skillCount == 0)
        {
            _view.BackGround_UISprite.height = 119;
            return;
        }
        var rank = skillCount % 3 == 0 ? skillCount / 3 : skillCount / 3 + 1;   //3个一排
        _view.BackGround_UISprite.height = (rank - 1)*100 + 119;
    }

    private void OpenSkillTips(Skill skill)
    {
        if (_tipsCtrl != null)
        {
            _tipsCtrl.Close();
        }
        _tipsCtrl = ProxyTips.OpenSkillTips(skill.id);
        _tipsCtrl.SetTipsPosition(new Vector3(-84, 159, 0));
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if(panel != _view.BattleSkillView_UIPanel
            && panel != _view.SkillScrollView_UIScrollView.panel)
            _onCloseEvt.OnNext(new Unit());
    }
}
