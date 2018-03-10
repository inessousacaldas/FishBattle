// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleItemViewController.cs
// Author   : 
// Created  : fish
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public enum BattleUseItemViewTab
{
    All
    , Attack
    , Heal
}

public partial interface IBattleItemViewController
{
    BagItemDto CurItem { get; }
    void InitView(IEnumerable<BagItemDto> items);
    UniRx.IObservable<Unit> OnCloseHandler { get; }
    IBaseTipsController GetTips { get; }
}

public partial class BattleItemViewController
{
    public static readonly ITabInfo[] tabSet =
    {
        TabInfoData.Create((int)BattleUseItemViewTab.All, "全部")
        //, TabInfoData.Create((int)BattleUseItemViewTab.Attack, "攻击")
        //, TabInfoData.Create((int)BattleUseItemViewTab.Heal, "恢复")
    } ;

    private CompositeDisposable _disposable;

    private TabbtnManager tabMgr = null;

    private BattleUseItemViewTab curTab = BattleUseItemViewTab.All;

    private Subject<Unit> _closeEvt;
    public UniRx.IObservable<Unit> OnCloseHandler { get { return _closeEvt; } }

    private const int _itemNum = 20;    //物品数量上限
    private IEnumerable<BagItemDto> itemSet;
    private ItemCellController _curCell;
    public ItemCellController CurCell {
        get { return _curCell; }
    }

    private ItemCellController _lastCell;
    private IBaseTipsController _tips;
    public IBaseTipsController GetTips { get { return _tips; } }

    public BagItemDto CurItem { get; private set; }

    public void InitView(IEnumerable<BagItemDto> items)
    {
        CurItem = null;
        _lastCell = null;
        _curCell = null;
        itemSet = items;
        curTab = BattleUseItemViewTab.All;
        _cells = new List<ItemCellController>(20);
        InitItemList();
        UpdateView();
    }

    //没有物品的地方显示空格子
    private void InitItemList()
    {
        Predicate<BattleUseItemViewTab> pre = tab => { return true; };
        for (int i = 0; i < _itemNum; i++)
        {
            var cell = AddCachedChild<ItemCellController, ItemCell>(
                        View.Container
                        , ItemCell.Prefab_BagItemCell
                        , "itemcell_" + i);
            var idx = i;
            cell.ContainerScrollView = View.Content_UIScrollView;
            _cells.Add(cell);

            _disposable.Add(cell.OnCellClick.Subscribe(_ =>
            {
                _lastCell = _curCell;
                if (_lastCell != null)
                    _lastCell.isSelect = false;
                _curCell = cell;

                var temp = itemSet.Filter(s => pre(curTab));
                CurItem = temp.TryGetValue(idx);
                if (CurItem != null)
                {
                    _curCell.UpdateView(CurItem, true, false);
                    UpdateCurItemDesc(CurItem);
                    _tips = cell.GetTips;
                    _tips.SetTipsPosition(new Vector3(-74, 252, 0));
                }
            }));

            cell.SetBorderSprite("bg_battle_normal", "BattleUIAtlas");
            cell.SetSelectSprite("bg_battle_select", "BattleUIAtlas");
        }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _view.btnLabel_UILabel.text = "使用物品";
        tabMgr = TabbtnManager.Create(tabSet, i => AddTabBtn(i, _view.tabBtn, TabbtnPrefabPath.TabBtnBattle, "Tabbtn_"), 0);

        _disposable = new CompositeDisposable();
        _disposable.Add(tabMgr.Stream.Subscribe(idx =>
        {
            curTab = (BattleUseItemViewTab) idx;
            UpdateView();
        }));
        _closeEvt = new Subject<Unit>();
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, TabbtnPrefabPath tabPath, string name)
    {
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath.ToString()
            , name + i);

        ctrl.SetBtnImages("btn_battle_normal", "btn_battle_select");
        ctrl.SetBtnLblFont(18, "ffffff", 18, "ffffff");
        return ctrl;
    }

    public IObservableExpand<int> TabClickEvt {
        get { return tabMgr.Stream; }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        tabMgr.Dispose();
        UICamera.onClick -= OnCameraClick;
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private List<ItemCellController> _cells;
    
    private void UpdateView()
    {
        _cells.ForEach(item =>
        {
            item.UpdateViewWithNull();
            item.Border.enabled = true;
        });
        Predicate<BattleUseItemViewTab> pre = tab => { return true; };  
        
        var len = 0;
        itemSet.Filter(s => pre(curTab)) // filter by tab
        .ForEachI((dto, index) =>
            {
                if (index == 0)
                    CurItem = itemSet.Filter(s => pre(curTab)).TryGetValue(0);
                ItemCellController cell = null;
                _cells.TryGetValue(index, out cell);
                if (cell != null)
                {
                    cell.Show();
                    cell.UpdateView(dto, false, false);
                }
                len = index;
            });

        //_view.TipLabel_UILabel.text = len != 0 ? string.Empty : "身上没有可使用的道具!";
        UpdateCurItemDesc(CurItem);
    }

    private void UpdateCurItemDesc(BagItemDto item)
    {
        _view.LeftTimeLabel_UILabel.text = "剩余使用次数：10";
        if (item == null)
        {
            _view.NameLabel_UILabel.text = string.Empty;
            _view.DesLabel_UILabel.text = string.Empty;
            return;
        }
        _view.NameLabel_UILabel.text = item.item.name;
        _view.DesLabel_UILabel.text = item.item.description;
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != _view.BattleItemView_UIPanel
            && panel != _view.scrollView_UIPanel)
            _closeEvt.OnNext(new Unit());
    }
}
