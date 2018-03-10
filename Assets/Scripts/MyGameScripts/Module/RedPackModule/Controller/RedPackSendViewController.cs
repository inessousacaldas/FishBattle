// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackSendViewController.cs
// Author   : DM-PC092
// Created  : 3/6/2018 11:15:20 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public interface ISendRedPackViewData
{
    //帮会人数
    string Word { get; }
    int Total { get; }
    int Count { get; }
    string Title { get; }
    string NameTitle { get; }
    RedPackChannelType CurTab { get; }
}

public class SendRedPackViewData: ISendRedPackViewData
{

    private string redPackWord;
    public string Word { get { return redPackWord; } set { redPackWord = value; } }
    private int money;
    public int Total { get { return money; } set { money = value; } }
    private int redPackCount;
    public int Count { get { return redPackCount; } set { redPackCount = value; } }
    private string redPackTitle;
    public string Title { get { return redPackTitle; } set { redPackTitle = value; } }
    private string nameTitle;
    public string NameTitle { get { return nameTitle; }set { nameTitle = value; } }
    private RedPackChannelType tab;
    public RedPackChannelType CurTab { get { return tab; } }
    public static SendRedPackViewData Create(int total, int count, string word, string title, string nameTitle)
    {
        SendRedPackViewData data = new SendRedPackViewData();
        data.Total = total;
        data.Count = count;
        data.Word = word;
        data.Title = title;
        data.NameTitle = nameTitle;
        return data;
    }

}

public partial interface IRedPackSendViewController
{   
    string ViewTotal { get; }
    string ViewCount { get; }
    string Title { get; }
    string NameTitle { get; }
    string ViewWord { get; }
}

public partial class RedPackSendViewController
{
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        SetRedPackChannel();
        //ptctrlCount = AddController<PageTurnViewController, PageTurnView>(View.CountPageTurnView);
        //ptctrlCount.InitData_NumberInputer(1, 1, 9999, true, PageTurnViewController.InputerShowPos.Down);

        ptctrlMoney = AddController<PageTurnViewController, PageTurnView>(View.MoneyPageTurnView);
        ptctrlMoney.InitData_NumberInputer(1, 1, 9999, true, PageTurnViewController.InputerShowPos.Down);
    }
    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(View.World_UIButton.onClick, () =>
        {
            SetRedPackChannel();
        });
        EventDelegate.Add(View.Guild_UIButton.onClick, () =>
        {
            SetRedPackChannel();
        });
        
    }

    protected override void RemoveCustomEvent()
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
    protected override void UpdateDataAndView(IRedPackData data)
    {
        if (data.RedPackType == AppDto.RedPack.RedPackType.Common)
        {
            _view.Word_UILabel.text = "红包祝福";
            _view.RedPackMoney_UILabel.text = "红包金币";
            _view.LeastMoney_UILabel.text = "10000";
        }
        if (data.RedPackType == AppDto.RedPack.RedPackType.key)
        {
            _view.Word_UILabel.text = "红包口令";
            _view.RedPackMoney_UILabel.text = "红包钻石";
            _view.LeastMoney_UILabel.text = "100";
        }
        //data.GetRedPacketViewData
    }
    public string ViewTotal { get { return _view.MoneyInput_UIInput.value; } }
    public string ViewCount { get {return _view.CountInput_UIInput.value; } }
    public string Title { get { return _view.Word_UILabel.text; } }
    public string NameTitle { get { return _view.TitleName_UILabel.text; } }
    public string ViewWord { get { return _view.WordInput_UIInput.value; } }
    TabbtnManager tabmgr;
    public TabbtnManager TabMgr { get { return tabmgr; } }
    PageTurnViewController ptctrlCount_1;
    PageTurnViewController ptctrlCount_2;
    PageTurnViewController ptctrlMoney;
    public IObservable<int> OnSelectCountStream { get { return ptctrlCount_1.Stream; } }
    public IObservable<int> OnSelectMoneyStream { get { return ptctrlMoney.Stream; } }
    public void SetRedPackChannel()
    {
        if (_view.Toggle_World_UIToggle.value == true)
        {
            //发送到世界频道
            ptctrlCount_1 = AddController<PageTurnViewController, PageTurnView>(View.CountPageTurnView);
            ptctrlCount_1.InitData_NumberInputer(100, 100, 300, true, PageTurnViewController.InputerShowPos.Down);
            _view.TitleName_UILabel.text = "世界红包";
        }
        else
        {
            //发送到公会频道                        
            ptctrlCount_1.InitData_NumberInputer(20, 20, 100, true, PageTurnViewController.InputerShowPos.Down);
            ptctrlCount_2 = AddController<PageTurnViewController, PageTurnView>(View.CountPageTurnView);
            _view.TitleName_UILabel.text = "公会红包";
        }
    }
    
}
