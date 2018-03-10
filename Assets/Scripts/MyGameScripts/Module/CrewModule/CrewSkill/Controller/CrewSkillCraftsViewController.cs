// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillCraftsViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

public interface ICrewSkillCraftsViewController
{
    UniRx.IObservable<ICrewSkillItemNController> CraftsViewBtnUp { get; }
}

public partial class CrewSkillCraftsViewController: ICrewSkillCraftsViewController
{
    //战技最多4个
    private const int ITEM_COUNT = 4;
    private Vector3[] posList = new Vector3[]
    {
        new Vector3(-143,146),new Vector3(-30,-8),new Vector3(-178,-118),new Vector3(-297,-28),
    };
    private List<CrewSkillItemNController> itemNList = new List<CrewSkillItemNController>();

    private ICrewSkillData data;
    private RoleSkillRangeController rangeCtrl;//技能范围

    private int curCraftsID = 0;//当前点击的战技ID
    private int curCrewID = 0;
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
        rangeCtrl = null;
        data = null;
        if (itemNList.Count > 0)
            itemNList.Clear();

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
        var list = data.GetSkillCrafts(id);
        if (list == null) return;
        CreateRightItem();
        UpdateItemView(list);
    }

    private Subject<ICrewSkillItemNController> craftsViewBtnUp = new Subject<ICrewSkillItemNController>();
    public UniRx.IObservable<ICrewSkillItemNController> CraftsViewBtnUp { get { return craftsViewBtnUp; } }

    private void CreateRightItem()
    {
        if (itemNList.Count > 0) return;
        for(int i = 0,max = ITEM_COUNT; i < max; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemNController, CrewSkillItemN>(
                View.ItemList_UIGrid.gameObject,
                CrewSkillItemN.NAME
                );
            itemNList.Add(itemCtrl);
            _disposable.Add(itemCtrl.OnbtnUp_UIButtonClick.Subscribe(e => craftsViewBtnUp.OnNext(itemCtrl)));
        }
    }

    private void UpdateItemView(List<CrewSkillCraftsVO> list)
    {
        int i = 0;
        for(int max= list.Count; i < max; i++)
        {
            itemNList[i].UpdateView(list[i]);
            itemNList[i].View.gameObject.SetActive(true);
        }
        for (; i < ITEM_COUNT;i++)
        {
            //没有的战技需隐藏
            itemNList[i].View.gameObject.SetActive(false);
        }
    }

}
