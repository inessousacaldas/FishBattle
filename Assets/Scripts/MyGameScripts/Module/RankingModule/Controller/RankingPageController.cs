// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingPageController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface IRankItemData
{
    long Getuid { get; }

    Rankings GetRankings { get; }
}

public class RankItemData: IRankItemData
{
    private long _uid;
    private Rankings _rankings;

    public long Getuid { get { return _uid; } }
    public Rankings GetRankings { get { return _rankings; } }

    public static RankItemData Create(long uid, Rankings rankings)
    {
        RankItemData data = new RankItemData();
        data._uid = uid;
        data._rankings = rankings;
        return data;
    }
}

public partial interface IRankingPageController
{
    UniRx.IObservable<int> OnMenuClickHandler { get; }
    UniRx.IObservable<IRankItemData> OnPlayerClickHandler { get; }
}

public partial class RankingPageController
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    private const int SimpleItemMax = 12;   //普通排行榜显示上限12个
    private const int AppellationMax = 6;   //称谓排行榜显示上限6个
    private const int Third = 3;            //前三名

    private long _curPlayerId;
    private IRankData _data;
    private bool _isFirst = true;
    #region rankoptionbtn
    private delegate void MenuBtnClick(RankingOptionBtnController btn, Rankings data, int idx);
    private int _curItemId = 0;

    private IEnumerable<Rankings> _allMenuDataList = new List<Rankings>();
    private IEnumerable<Rankings> _firstMenuDataList = new List<Rankings>();
    private IEnumerable<Rankings> _secondMenuDataList = new List<Rankings>();
    private List<RankingOptionBtnController> _firstOptionList = new List<RankingOptionBtnController>();
    private List<RankingOptionBtnController> _secondOptionList = new List<RankingOptionBtnController>();

    private Rankings _curFirstMenu;
    private Rankings _secondCurMenu;
    #endregion

    #region
    private delegate void RankCellClick(RankingItemCellController btn);
    private delegate void RankAppellationClick(RankingAppellationItemController btn);

    private RankingTitleUIController _rankTitleCtr;
    private RankingInfoCellController _rankInfoCtr;

    private Rankings.RankStyle _rankStyle = Rankings.RankStyle.Normal;

    private Dictionary<GameObject, RankingItemCellController> _simpleRankCellCtrs = new Dictionary<GameObject, RankingItemCellController>();
    private Dictionary<GameObject, RankingItemCellController> _appellationRankCells = new Dictionary<GameObject, RankingItemCellController>();
    private List<RankingAppellationItemController> _appellationItems = new List<RankingAppellationItemController>();
    private List<Transform> _appellationAnchor = new List<Transform>();
    #endregion

    #region Subject
    private Subject<int> _onMenuClickEvt = new Subject<int>();
    public UniRx.IObservable<int> OnMenuClickHandler { get { return _onMenuClickEvt; } } 
    private Subject<IRankItemData> _onPlayerClickEvt = new Subject<IRankItemData>();
    public UniRx.IObservable<IRankItemData> OnPlayerClickHandler { get { return _onPlayerClickEvt; } }  
    #endregion
    public enum RankStyle
    {
        Simple = 1,
        Appellation = 2
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        initData();
        InitOptionBtn();
        InitSimpleList();
        InitAppellationList();
        
        _view.SecondTable_UITable.gameObject.SetActive(false);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _view.SimpleRankingGrid_UIRecycledList.onUpdateItem = UndateSimpleList;
        _view.AppellationRankingGrid_UIRecycledList.onUpdateItem = UpdateAppellationList;
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        
    }

    public void UpdateDataAndView(IRankData data)
    {
        _data = data;

        if (_secondCurMenu != null)
        {
            UpdateTitle(_secondCurMenu);
            UpdateRankInfo(_secondCurMenu);

            if (_rankStyle == Rankings.RankStyle.Normal)
                UpdateSimpleRank(_secondCurMenu);
            else
            {
                UpdateAppellationRank(_secondCurMenu);
                TheBestPlayerInfo();
            }
        }

        if (_isFirst)
        {
            _isFirst = false;
            InitSecondMenu(_firstMenuDataList.TryGetValue(0), _firstOptionList.TryGetValue(0));
        }
    }

    private void UndateSimpleList(GameObject go, int itemIdx, int dataIdx)
    {
        if (_secondCurMenu == null) return;
        RankingItemCellController item = null;
        if (_simpleRankCellCtrs.TryGetValue(go, out item))
        {
            var data = _data.GetRankInfoByRank(_secondCurMenu.id).list.TryGetValue(dataIdx);
            item.SetItemInfo(data, dataIdx + 1, _secondCurMenu.id);
        }
    }

    private void UpdateAppellationList(GameObject go, int itemIdx, int dataIdx)
    {
        if (_secondCurMenu == null) return;  
        RankingItemCellController item = null;
        if (_appellationRankCells.TryGetValue(go, out item))
        {
            var datalist = _data.GetRankInfoByRank(_secondCurMenu.id).list;
            var cnt = datalist.Count - 3;
            var data = datalist.GetElememtsByRange(3, cnt).TryGetValue(dataIdx);
            item.SetItemInfo(data, dataIdx + 1, _secondCurMenu.id);
        }
    }

    //称谓模式下的前三名
    private void TheBestPlayerInfo()
    {
        var data = _data.GetRankInfoByRank(_secondCurMenu.id);
        _appellationItems.ForEachI((item, idx) =>
        {
            item.SetItemInfo(data.list.TryGetValue(idx), idx + 1, _secondCurMenu.id);
        });
    }

    private void InitSimpleList()
    {
        RankCellClick func = btn =>
        {
            _disposable.Add(btn.ClickHandler.Subscribe(data =>
            {
                _onPlayerClickEvt.OnNext(data);
            }));
        };

        for (int i = 0; i < SimpleItemMax; i++)
        {
            var option = AddChild<RankingItemCellController, RankingItemCell>(
                _view.SimpleRankingGrid_UIRecycledList.gameObject,
                RankingItemCell.NAME);
            _simpleRankCellCtrs.Add(option.gameObject, option);
            func(option);
        }
    }

    private void InitAppellationList()
    {
        RankCellClick func = btn =>
        {
            _disposable.Add(btn.ClickHandler.Subscribe(data =>
            {
                _onPlayerClickEvt.OnNext(data);
            }));
        };

        for (int i = 0; i < AppellationMax; i++)
        {
            var option = AddChild<RankingItemCellController, RankingItemCell>(
                    _view.AppellationRankingGrid_UIRecycledList.gameObject,
                    RankingItemCell.NAME);
            _appellationRankCells.Add(option.gameObject, option);
            func(option);
        }

        RankAppellationClick _func = btn =>
        {
            _disposable.Add(btn.OnClickHandler.Subscribe(data =>
            {
                _onPlayerClickEvt.OnNext(data);
            }));
        };
        for (int i = 0; i < Third; i++)
        {
            var option = AddChild<RankingAppellationItemController, RankingAppellationItem>(
                    _appellationAnchor[i].gameObject,
                    RankingAppellationItem.NAME);
            _appellationItems.Add(option);
            _func(option);
        }
    }

    private void initData()
    {
        _allMenuDataList = DataCache.getArrayByCls<Rankings>();
        _firstMenuDataList = _allMenuDataList.Filter(d => d.parentId == 0);

        _appellationAnchor.Add(_view.FirstPrizeAnchor_Transform);
        _appellationAnchor.Add(_view.SecondPrizeAnchor_Transform);
        _appellationAnchor.Add(_view.ThirdPrizeAnchor_Transform);
    }

    private void InitOptionBtn()
    {
        MenuBtnClick func = (btn, data, idx) =>
        {
            _disposable.Add(btn.GetClickHandler.Subscribe(_ =>
            {
                InitSecondMenu(data, btn);
            }));
        };
        _firstMenuDataList.ForEachI((data, idx) =>
        {
            var option = AddChild<RankingOptionBtnController, RankingOptionBtn>(
                _view.OptionTable_UITable.gameObject,
                RankingOptionBtn.NAME);
            option.SetRankMenuBtn(data);
            _firstOptionList.Add(option);
            func(option, data, idx);
        });
    }

    private void InitSecondMenu(Rankings menu, RankingOptionBtnController parent)
    {

        if (_curFirstMenu == menu && _view.SecondTable_UITable.gameObject.activeSelf)
        {
            _view.SecondTable_UITable.gameObject.SetActive(false);
            _view.OptionTable_UITable.Reposition();
            parent.SetArrowAngles(false);
            return;
        }
        _firstOptionList.ForEach(button => button.SetArrowAngles(parent == button));
        _curFirstMenu = menu;
        _secondMenuDataList = _allMenuDataList.Filter(d => d.parentId == menu.id && d.rankShow);

        _view.SecondTable_UITable.gameObject.SetActive(_secondMenuDataList.Count() != 0);

        MenuBtnClick func = (btn, data, idx) =>
        {
            _disposable.Add(btn.GetClickHandler.Subscribe(_ =>
            {
                var rankings = _secondMenuDataList.TryGetValue(idx);
                UpdateItemListByMenu(rankings);
                _secondCurMenu = rankings;
                _secondOptionList.ForEach(button => button.IsSelect(btn == button));
            }));
        };
        _secondOptionList.ForEach(item => item.gameObject.SetActive(false));
        _secondMenuDataList.ForEachI((data, idx) =>
        {
            if (idx >= _view.SecondTable_UITable.transform.childCount)
            {
                var option = AddChild<RankingOptionBtnController, RankingOptionBtn>(
                    _view.SecondTable_UITable.gameObject,
                    RankingOptionBtn.NAME);
                option.SetRankMenuBtn(data, true);
                _secondOptionList.Add(option);
                func(option, data, idx);
            }
            else
            {
                var item = _secondOptionList.TryGetValue(idx);
                if (item != null)
                {
                    item.SetRankMenuBtn(data, true);
                    item.gameObject.SetActive(true);
                }
            }
        });

        _secondOptionList.ForEachI((go, idx) =>
        {
            if (idx == 0)
            {
                UpdateItemListByMenu(_secondMenuDataList.TryGetValue(0));
                go.IsSelect(true);
            }
            else
                go.IsSelect(false);
        });

        _view.SecondTable_UITable.transform.parent = parent.transform;
        _view.SecondTable_UITable.transform.localPosition = new Vector3(-83, -30, 0);
        _view.SecondTable_UITable.Reposition();

        _view.OptionTable_UITable.Reposition();
    }

    //显示右侧榜单
    private void UpdateItemListByMenu(Rankings menu)
    {
        if (menu == null || _secondCurMenu == menu) return;

        _rankStyle = (Rankings.RankStyle)menu.rankStyle;
        _secondCurMenu = menu;

        var dto = _data.GetRankInfoByRank(menu.id);
        if (dto == null)
            _onMenuClickEvt.OnNext(menu.id);
        else
        {
            UpdateTitle(menu);
            UpdateRankInfo(menu);

            if (_rankStyle == Rankings.RankStyle.Normal)
                UpdateSimpleRank(menu);
            else
            {
                UpdateAppellationRank(menu);
                TheBestPlayerInfo();
            }
        }
    }

    private void UpdateTitle(Rankings rank)
    {
        if (_rankTitleCtr == null)
        {
            _rankTitleCtr = AddChild<RankingTitleUIController, RankingTitleUI>(
                   _view.gameObject,
                   RankingTitleUI.NAME);
        }
        _rankTitleCtr.transform.localPosition = _rankStyle == Rankings.RankStyle.Normal
            ? new Vector3(54, 175, 0) : new Vector3(54, -31, 0);

        _rankTitleCtr.SetDate(rank);
    }

    private void UpdateRankInfo(Rankings menu)
    {
        if (_rankInfoCtr == null)
        {
            _rankInfoCtr = AddChild<RankingInfoCellController, RankingInfoCell>(
                   _view.gameObject,
                   RankingInfoCell.NAME);
            _rankInfoCtr.transform.localPosition = new Vector3(54, -239, 0);
            _disposable.Add(
                _rankInfoCtr.OnTipsBtn_UIButtonClick.Subscribe(
                    _ =>
                    {
                        ProxyTips.OpenTextTips(_secondCurMenu.tipId, new Vector3(-188, -131, 0));
                    }));
        }
        var dto = _data.GetAllRankData[menu.id];
        _rankInfoCtr.SetItemInfo(dto);
    }

    private void UpdateSimpleRank(Rankings menu)
    {
        _view.SimpleRankingPageUI.SetActive(true);
        _view.AppellationRankingPageUI.SetActive(false);

        var rankInfoDto = _data.GetRankInfoByRank(menu.id);
        if (rankInfoDto != null)
        {
            _view.SimpleRankingGrid_UIRecycledList.UpdateDataCount(rankInfoDto.list.Count, true);
            _view.SimpleRankingUIScrollView_UIScrollView.ResetPosition();
        }
    }

    private void UpdateAppellationRank(Rankings menu)
    {
        _view.SimpleRankingPageUI.SetActive(false);
        _view.AppellationRankingPageUI.SetActive(true);
        var rankInfoDto = _data.GetRankInfoByRank(menu.id);
        if (rankInfoDto != null)
        {
            _view.AppellationRankingGrid_UIRecycledList.UpdateDataCount(rankInfoDto.list.Count - 3, true);  //前三名排除在外,前三名显示在上面
        }
    }
}