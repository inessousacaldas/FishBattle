// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MoneyInfoViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum; 
public partial interface IMoneyInfoViewController
{
    UniRx.IObservable<VirtualItemEnum> OnClickStream { get; }
}
public partial class MoneyInfoViewController
{
    List<MoneyInfoItemViewController> itemCtrls;

    Subject<VirtualItemEnum> _onClickStream = new Subject<VirtualItemEnum>();
    public UniRx.IObservable<VirtualItemEnum> OnClickStream { get { return _onClickStream; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        itemCtrls = new List<MoneyInfoItemViewController>(3);
    }

    protected override void UpdateDataAndView(IPlayerModel data)
    {
        itemCtrls.ForEach<MoneyInfoItemViewController>(x => x.Hide());

        int index = 0;
        MoneyInfoViewLogic._virtualItemIDs.ForEachI((x, i) =>
        {
            MoneyInfoItemViewController ctrl = null;
            if(itemCtrls.Count <= i)
            {
                ctrl = AddChild<MoneyInfoItemViewController, MoneyInfoItemView>(View.Anchor, MoneyInfoItemView.NAME);
                itemCtrls.Add(ctrl);
                //var tempType = x;
                //_disposable.Add(itemCtrls[i].OnAddBtn_UIButtonClick.Subscribe(_ => {
                //    _onClickStream.OnNext(tempType);
                //}));
            }
            else
            {
                ctrl = itemCtrls[i];
            }

            var moneyCount = ModelManager.IPlayer.GetPlayerWealth(x);
            itemCtrls[i].UpdateView(x, moneyCount);
            itemCtrls[i].Show();

            index++;
        });

        //积分商店
        SetShowIntegralShop(index);
        View.Anchor_UIGrid.Reposition();
    }

    private void SetShowIntegralShop(int index, bool isShow=true)
    {
        MoneyInfoItemViewController ctrl = null;
        if (index+1 > itemCtrls.Count)
        {
            ctrl = AddChild<MoneyInfoItemViewController, MoneyInfoItemView>(View.Anchor, MoneyInfoItemView.NAME);
            itemCtrls.Add(ctrl);
        }
        else
        {
            ctrl = itemCtrls[index];
        }

        ctrl.UpdateView(AppVirtualItem.VirtualItemEnum.NONE, -1);
        ctrl.Show();
    }

    protected override void OnDispose()
    {
        itemCtrls.Clear();
        _onClickStream = _onClickStream.CloseOnceNull();
        base.OnDispose();
    }
}
