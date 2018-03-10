// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillMagicViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using UnityEngine;
using System.Collections.Generic;

public interface ICrewSkillMagicViewController
{
    UniRx.IObservable<ICrewSkillItemNController> MagicViewBtnUp { get; }
    IObservable<Unit> OnbtnDLQ_UIButtonClick { get; }
}

public partial class CrewSkillMagicViewController
{
    private ICrewSkillData data;
    private RoleSkillRangeController rangeCtrl; //技能范围

    private List<CrewSkillItemNController> itemNList = new List<CrewSkillItemNController>();
    private CompositeDisposable _disposable;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
        data = null;
        if (itemNList.Count > 0) itemNList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetData(ICrewSkillData data)
    {
        this.data = data;
    }

    //点击左侧伙伴，数据的更新
    public void UpdateView(int id)
    {
        CreateRightItem();
        UpdateRightItem(id);
    }

    private Subject<ICrewSkillItemNController> magicViewBtnUp = new Subject<ICrewSkillItemNController>();
    public UniRx.IObservable<ICrewSkillItemNController> MagicViewBtnUp { get { return magicViewBtnUp; } }
    //生成魔法左侧物品
    private void CreateRightItem()
    {
        if (itemNList.Count > 0) return;
        for(int i = 0,max = data.MagicOpenDic.Count; i < max; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemNController, CrewSkillItemN>(
                View.ItemList_Transform.gameObject,
                CrewSkillItemN.NAME
                );
            itemNList.Add(itemCtrl);
            _disposable.Add(itemCtrl.OnbtnUp_UIButtonClick.Subscribe(e => magicViewBtnUp.OnNext(itemCtrl)));
        }
    }

    private void UpdateRightItem(int crewId)
    {
        var openDic = data.MagicOpenDic;
        var magicList = data.GetSkillMagic(crewId);
        int magicCount = 0;
        if (magicList!=null)
            magicCount = magicList.Count;
        int playerLv = CrewViewDataMgr.DataMgr.GetCurCrewGrade;
        int openCount = 0;
        int idx = 0;
        openDic.ForEach(e =>
        {
            if (playerLv >= e.Key)
            {
                //已配置和未配置
                openCount = e.Value;
                if (magicCount > idx)
                    itemNList[idx].UpdateMagicView(true, e.Key, magicList[idx]);
                else
                    itemNList[idx].UpdateMagicView(true, e.Key);
            }
            else
            {
                //未解锁
                itemNList[idx].UpdateMagicView(false,e.Key);
            }
            idx++;
        });
    }
    
}
