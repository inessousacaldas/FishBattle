// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzForgeController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial class QuartzForgeController
{
    private CompositeDisposable _disposable;
    private int _curGrade;    //打造等级
    private int _smithMaxGrade = 0;     //当前可打造等级上限
    private bool _isStrength = false;    //是否强化打造

    private List<QuartzSmithGrade> _quartzSmithList = new List<QuartzSmithGrade>();
    private List<ItemCellController> _propsList = new List<ItemCellController>();
    private List<QuartzForgeItemController> _forgeItemList = new List<QuartzForgeItemController>();
    private List<BracerGrade> _bracerList = new List<BracerGrade>();

    private Vector3 _tipPos = new Vector3(-335, 148, 0);
    private Vector3 _gainPos = new Vector3(90, 30, 0);

    private string[] ItemName = {"1级打造", "2级打造", "3级打造", "4级打造", "5级打造"};

    public void UpdateDataAndView(IQuartzForgeData data)
    {
        UpdatePropsList(_curGrade);
    }
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        InitItemBtn();
        _quartzSmithList = DataCache.getArrayByCls<QuartzSmithGrade>();
        for (int i = 0; i < _view.PropsGrid_Transform.childCount; i++)
        {
            var propItem = AddController<ItemCellController, ItemCell>(
                _view.PropsGrid_Transform.GetChild(i).gameObject);
            
            _propsList.Add(propItem);
        }

        _bracerList = DataCache.getArrayByCls<BracerGrade>();
        _smithMaxGrade = _bracerList.Find(d => d.id == ModelManager.Player.GetBracerGrade).quartzSmithGrade;

        _forgeItemList.ForEachI((item, idx) =>
        {
            item.SetItemInfo(ItemName[idx]);
            item.IsLock = _smithMaxGrade < idx + 1;
        });

        _forgeItemList[0].IsSelect = _smithMaxGrade >= 1;
    }

    private delegate void BtnFunc(QuartzForgeItemController btn, int i);

    private void InitItemBtn()
    {
        BtnFunc f = delegate (QuartzForgeItemController btn, int i)
        {
            _disposable.Add(btn.OnClickHandler.Subscribe(_ => {
                if (i + 1 > _smithMaxGrade)
                {
                    var bracer = _bracerList.Find(d => d.quartzSmithGrade == i + 1);
                    TipManager.AddTip(string.Format("开启改结晶回路打造等级需要{0}", bracer.name));
                    return;
                }
                UpdatePropsList(i);
                _forgeItemList.ForEachI((item, idx) => { item.IsSelect = i == idx; });
            }));
        };

        for (int i = 0; i < _view.ItemGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.ItemGrid_UIGrid.GetChild(i);
            var controller = AddController<QuartzForgeItemController, QuartzForgeItem>(item.gameObject);
            f(controller, i);
            _forgeItemList.Add(controller);
        }
    }

    private delegate void CellFunc(ItemCellController controller, int idx);
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.CommonBtn_UIButton.onClick, OnCommnBtnClick);
        EventDelegate.Add(_view.StrengthBtn_UIButton.onClick, OnStrengthBtnClick);
        _disposable.Add(BackpackDataMgr.Stream.SubscribeAndFire(_ => { UpdatePropsList(_curGrade); }));
        _disposable.Add(PlayerModel.Stream.SubscribeAndFire(_ => { SetCashLb(); }));
        EventDelegate.Add(_view.StrengthToggle_UIButton.onClick, () =>
        {
            if (_curGrade < 3) //4,5级开放强化打造
            {
                TipManager.AddTip("结晶回路打造等级4级以上可以进行强化打造");
                return;
            }
            
            _isStrength = !_isStrength;
            UpdatePropsList(_curGrade);
        });

        CellFunc clickFunc = delegate(ItemCellController con, int idx)
        {
            List<ItemDto> data = _curGrade < 3 ? 
            _quartzSmithList[_curGrade].normalSmith: 
            _quartzSmithList[_curGrade].strengSmith;

            var item = BackpackDataMgr.DataMgr.GetItemByItemID(data[idx].itemId);
            ShowItemGainWay(data[idx].itemId);
            if (item == null)
                ShowItemTips(data[idx].itemId);
            else if (item.count < data[idx].count)
                ShowItemTips(item.item);
        };

        _propsList.ForEachI((item, idx)=>
        {
            _disposable.Add(item.OnCellClick.Subscribe(_ =>
            {
                clickFunc(item, idx);
            }));
        });
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void OnStrengthBtnClick()
    {
        var data = _quartzSmithList[_curGrade].strengSmith;
        for (int i = 0; i < data.Count; i++)
        {
            var cout = BackpackDataMgr.DataMgr.GetItemCountByItemID(data[i].itemId);
            if (cout < data[i].count)
            {
                ShowItemGainWay(data[i].itemId);
                ShowItemTips(data[i].itemId);
                TipManager.AddTip(string.Format("{0}不足", data[i].item.name));
                return;
            }
        }
        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, _quartzSmithList[_curGrade].silver, 
            () => { QuartzDataMgr.QuartzNetMsg.Quartz_Smith(_curGrade + 1, true); });
    }

    private void OnCommnBtnClick()
    {
        if (_smithMaxGrade == 0)
        {
            TipManager.AddTip(string.Format("开启改结晶回路打造等级需要{0}", _bracerList.TryGetValue(2).name));
            return;
        }
        var data = _quartzSmithList[_curGrade].normalSmith;
        for (int i = 0; i < data.Count; i++)
        {
            var cout = BackpackDataMgr.DataMgr.GetItemCountByItemID(data[i].itemId);
            if (cout < data[i].count)
            {
                ShowItemGainWay(data[i].itemId);
                ShowItemTips(data[i].itemId);
                TipManager.AddTip(string.Format("{0}不足", data[i].item.name));
                return;
            }
        }

        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, _quartzSmithList[_curGrade].silver, 
            ()=> { QuartzDataMgr.QuartzNetMsg.Quartz_Smith(_curGrade + 1, false); });
    }

    private void UpdatePropsList(int idx)
    {
        List<ItemDto> data = new List<ItemDto>();

        if (idx < 3)   //4,5级才开放强化打造
        {
            _view.hook_UISprite.gameObject.SetActive(false);
            _view.CommonBtn_UIButton.gameObject.SetActive(true);
            _view.StrengthBtn_UIButton.gameObject.SetActive(false);
        }
        else
        {
            _view.hook_UISprite.gameObject.SetActive(_isStrength);
            _view.CommonBtn_UIButton.gameObject.SetActive(!_isStrength);
            _view.StrengthBtn_UIButton.gameObject.SetActive(_isStrength);
        }        

        _curGrade = idx;
        if (IsStrengthSmith())
            data = _quartzSmithList[_curGrade].strengSmith;
        else
            data = _quartzSmithList[_curGrade].normalSmith;

        int _idx = 0;
        _propsList.ForEachI((item, index) =>
        {
            if (index < data.Count)
            {
                var hadCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(data[index].itemId);
                item.QuartzForgeItem(data[index], hadCount);
            }
        });
        SetCashLb();

        //最后一个是强化打造消耗的道具
        _propsList[7].gameObject.SetActive(IsStrengthSmith());
    }

    private void SetCashLb()
    {
        var silver = ModelManager.Player.GetPlayerWealthSilver();
        _view.NeedCashLb_UILabel.text = _quartzSmithList[_curGrade].silver > silver ?
            _quartzSmithList[_curGrade].silver.ToString().WrapColor(ColorConstantV3.Color_Red_Str)
            : _quartzSmithList[_curGrade].silver.ToString().WrapColor(ColorConstantV3.Color_White);
    }

    private bool IsStrengthSmith()
    {
        return _isStrength && (_curGrade == 3 || _curGrade == 4);
    }

    private void ShowItemGainWay(int itemId)
    {
        GainWayTipsViewController.OpenGainWayTip(itemId, _gainPos, ProxyQuartz.CloseQuartzMainView);
    }

    private void ShowItemTips(int itemId)
    {
        var itemDto = DataCache.getDtoByCls<GeneralItem>(itemId);
        ProxyTips.OpenTipsWithGeneralItem(itemDto, _tipPos);
    }

    private void ShowItemTips(GeneralItem item)
    {
        ProxyTips.OpenTipsWithGeneralItem(item, _tipPos);
    }
}
