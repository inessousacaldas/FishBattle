// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillPassiveViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;

public enum PassiveState
{
    Lock,
    HaveItem,
    NeedItem
}

public enum PassiveType
{
    All,//全部
    Atk,//攻击
    Def,//防御
    Sup //辅助
}

public class PsvItemData
{
    public CrewPassiveSkill psvVO;
    public PassiveSkillDto psvDto;
    public PassiveState state;
}

public interface ICrewSkillPassiveViewController
{
    UniRx.IObservableExpand<int> TabStream { get; }
    UniRx.IObservable<ICrewSkillItemNController> PsvViewBtnUp { get; }
}

public partial class CrewSkillPassiveViewController:ICrewSkillPassiveViewController
{
  
    private int unLockNum = 0;
    private ICrewSkillData data;
    
    private TabbtnManager tabMgr = null;
    private List<CrewSkillItemNController> itemList = new List<CrewSkillItemNController>();
    private CompositeDisposable _disposable;
    #region 初始化自定义
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
    }
    public UniRx.IObservableExpand<int> TabStream
    {
        get { return tabMgr.Stream; }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
        if (tabMgr != null) tabMgr = null;
        if (data != null) data = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    #endregion


    public void SetData(ICrewSkillData data)
    {
        this.data = data;
    }
    /// <summary>
    /// 技巧固定4个控件
    /// </summary>
    /// <param name="id"></param>
    public void UpdateView(int id, ICrewSkillViewController ctrl)
    {
        //点击未拥有的伙伴会报空
        unLockNum = ExpressionManager.UnLockCrewSkillPassive(CrewViewDataMgr.DataMgr.GetCurCrewQuality);
        UpdatePsvView(id, ctrl);
    }

    private void UpdatePsvView(int id, ICrewSkillViewController ctrl)
    {
        CreateRightItem(ctrl);
        UpdateRightItem(id);
    }

    private Subject<ICrewSkillItemNController> psvViewBtnUp = new Subject<ICrewSkillItemNController>();
    public UniRx.IObservable<ICrewSkillItemNController> PsvViewBtnUp { get { return psvViewBtnUp; } }

    private void CreateRightItem(ICrewSkillViewController ctrl)
    {
        if (itemList.Count > 0) return;
        for(int i = 0; i < 4; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemNController, CrewSkillItemN>(
                View.ItemList_Transform.gameObject,
                CrewSkillItemN.NAME
                );
            _disposable.Add(itemCtrl.OnbtnUp_UIButtonClick.Subscribe(e => psvViewBtnUp.OnNext(itemCtrl)));
            itemList.Add(itemCtrl);
        }
    }

    
    private void UpdateRightItem(int id)
    {
        var list = data.GetPsvDtoList(id);
        //list为空是因为此时没有招募伙伴
        if (list == null) return;
        int i = 0;
        //先将有的赋值
        for (int max = list.Count; i < max; i++)
        {
            CrewPassiveSkill skill = data.GetPsvVO(list[i].id);
            if (skill != null) 
                itemList[i].UpdatePsvView(skill,list[i]);
        }
        //没有的比较处理
        for (; i < 4; i++)
        {
            itemList[i].UpdatePsvView(unLockNum,i);
        }
    }
    
}
