// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerMainViewController.cs
// Author   : xjd
// Created  : 1/13/2018 10:13:30 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;
using UnityEngine;

public partial interface IFlowerMainViewController
{
    string InputFiledText { get; set; }
    UniRx.IObservable<int> OnSelectCountStream { get; }
    PageTurnViewController PtCtrl { get; }
}

public partial class FlowerMainViewController
{
    public string InputFiledText
    {
        get { return View.InputField_UIInput.value; }
        set { View.InputField_UIInput.value = value; }
    }

    private List<FlowerFriendItemController> _friendList = new List<FlowerFriendItemController>();
    private List<FlowerItemController> _flowerList = new List<FlowerItemController>();
    private List<FlowerEffItemController> _flowerEffList = new List<FlowerEffItemController>();
    private readonly int[] _flowerIds = 
    {
        101010,101011,101012,101013
    };
    private int[] _flowerCounts = new int[3] 
    {
        99, 520, 999
    };
    private string[] _flowerNames = new string[3]
    {
        "花环", "花海", "花雨"
    };

    private PageTurnViewController _ptctrl;
    public UniRx.IObservable<int> OnSelectCountStream { get { return _ptctrl.Stream; } }
    public PageTurnViewController PtCtrl { get { return _ptctrl; } }

    CompositeDisposable viewDisposable = new CompositeDisposable();
    private int _flowerCount = 0;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        var flowerCountStr = DataCache.GetStaticConfigValues(AppStaticConfigs.FLOWERS_COUNT);
        var strs = flowerCountStr.Split('-');
        strs.ForEachI((str,index) =>
        {
            if (index < 3)
                _flowerCounts[index] = int.Parse(str);
        });

        var flowerNameStr = DataCache.GetStaticConfigValues(AppStaticConfigs.FLOWERS_NAME);
        strs = flowerNameStr.Split('-');
        strs.ForEachI((str, index) =>
        {
            if (index < 3)
                _flowerNames[index] = str;
        });

        InitPageTurn();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(View.InputEffLabel_UIInput.onChange, OnFlowerWishInputChange);
        EventDelegate.Add(View.InputField_UIInput.onChange, OnSearchInputChange);
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        viewDisposable = viewDisposable.CloseOnceNull();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private void OnFlowerWishInputChange()
    {
        View.InputEffLabel_UIInput.characterLimit = 15;
        FlowerMainViewDataMgr.DataMgr.SetCurFlowerContent(View.InputEffLabel_UIInput.value);
    }

    private void OnSearchInputChange()
    {
        if (string.IsNullOrEmpty(View.InputField_UIInput.value))
            FlowerMainViewDataMgr.DataMgr.ResetFriendList();
    }

    private void InitPageTurn()
    {
        _ptctrl = AddController<PageTurnViewController, PageTurnView>(View.PageTurn);
        _ptctrl.InitData_NumberInputer(1, 1, 9999, true, PageTurnViewController.InputerShowPos.Up);
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IFlowerMainViewData data)
    {
        viewDisposable.Clear();
        //好友
        _friendList.ForEach(item => item.Hide());
        int friendCout = 0;
        data.SearchList.GetElememtsByRange(friendCout, -1);
        if (data.SearchList.ToList().IsNullOrEmpty())
        {
            View.NoFriendPanel.SetActive(true);
            UIHelper.SetUITexture(View.Texture_UITexture, "npc_323", false);
        }
        else
            View.NoFriendPanel.SetActive(false);
        data.SearchList.ForEachI((itemDto, index) =>
        {
            var ctrl = AddFriendItemIfNotExist(index);
            ctrl.UpdateView(itemDto, itemDto.friendId==data.CurFriendId);
            ctrl.Show();

            viewDisposable.Add(ctrl.OnClickItemStream.Subscribe(id =>
            {
                FlowerMainViewDataMgr.DataMgr.SetCurFriendId(id);
            }));
        });
        View.FriendGrid_UIGrid.Reposition();

        //花
        var flowerCount = 0;    //默认选中花的数量>0的 最靠前的 花
        var curFlowerId = 0;    //默认选中花的id
        //所有鲜花数量都为0 默认选择第一个鲜花
        for(int i=0;i< _flowerIds.Length; i++)
        {
            flowerCount = BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(_flowerIds[i]);
            if (flowerCount > 0)
            {
                curFlowerId = _flowerIds[i];
                break;
            }
        }
        if (flowerCount == 0)
            data.CurFlowerId = _flowerIds[0];
        else
            flowerCount = 0;
        _flowerList.ForEach(item => item.Hide());
        _flowerIds.ForEachI((id, index) =>
        {
            //判断当前选中花id
            var ownCount = BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(id);
            if (data.CurFlowerId <= 0 && flowerCount == 0 && ownCount > 0)
            {
                flowerCount = ownCount;
                data.CurFlowerId = id;
            }

            var ctrl = AddFlowerItemIfNotExist(index);
            ctrl.UpdateView(id, id == data.CurFlowerId);
            ctrl.Show();
                
            viewDisposable.Add(ctrl.OnClickItemStream.Subscribe(flowerId =>
            {
                FlowerMainViewDataMgr.DataMgr.SetCurFlowerId(flowerId);
            }));
        });
        View.FlowerGrid_UIGrid.Reposition();

        //当前选中花的具体数据
        var itemdata = ItemHelper.GetGeneralItemByItemId(data.CurFlowerId);
        var curFlowerProp = new PropsParam_16();
        if (itemdata != null && itemdata as Props != null)
            curFlowerProp = (itemdata as Props).propsParam as PropsParam_16;

        //特效
        var curEffName = string.Empty;
        if (data.CurFlowerCount < _flowerCounts[0])
            curEffName = string.Empty;
        else if (data.CurFlowerCount < _flowerCounts[1])
            curEffName = _flowerNames[0];
        else if (data.CurFlowerCount < _flowerCounts[2])
            curEffName = _flowerNames[1];
        else
            curEffName = _flowerNames[2];

        _flowerEffList.ForEach(item => item.Hide());
        _flowerNames.ForEachI((name, index) =>
        {
            if (index < 3)
            {
                var ctrl = AddFlowerEffItemIfNotExist(index);
                ctrl.UpdateView(index, _flowerCounts[index], name, name == curEffName);
                ctrl.Show();

                viewDisposable.Add(ctrl.OnClickItemStream.Subscribe(id =>
                {
                    if(id < 3)
                    {
                        FlowerMainViewDataMgr.DataMgr.SetCurFlowerCout(_flowerCounts[id]);
                        _ptctrl.SetPageInfo(_flowerCounts[id], 9999);
                    }
                }));
            }
        });
        View.EffGrid_UIGrid.Reposition();

        if (curFlowerProp == null) return;
        //好友度  互为好友的degree 必然大于0
        var curFriendDto = FriendDataMgr.DataMgr.GetFriendDtoById(data.CurFriendId);
        if (data.SearchList.ToList().IsNullOrEmpty() || (curFriendDto != null && curFriendDto.degree > 0 && curFlowerProp != null))
            View.DegreeLabel_UILabel.text = string.Format("好友度：+{0}", data.CurFlowerCount * curFlowerProp.degree);
        else
            View.DegreeLabel_UILabel.text = "不是互为好友";

        //花语
        View.InputEffLabel_UIInput.characterLimit = 26;
        View.InputEffLabel_UIInput.value = curFlowerProp.context;
        View.InputEffLabel_UIInput.GetComponent<BoxCollider>().enabled = data.CurFlowerCount >= _flowerCounts[2];
        _flowerCount = data.CurFlowerCount;
    }

    private FlowerFriendItemController AddFriendItemIfNotExist(int idx)
    {
        FlowerFriendItemController ctrl = null;
        _friendList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<FlowerFriendItemController, FlowerFriendItem>(View.FriendGrid_UIGrid.gameObject, FlowerFriendItem.NAME);
            _friendList.Add(ctrl);
        }

        return ctrl;
    }

    private FlowerItemController AddFlowerItemIfNotExist(int idx)
    {
        FlowerItemController ctrl = null;
        _flowerList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<FlowerItemController, FlowerItem>(View.FlowerGrid_UIGrid.gameObject, FlowerItem.NAME);
            _flowerList.Add(ctrl);
        }

        return ctrl;
    }

    private FlowerEffItemController AddFlowerEffItemIfNotExist(int idx)
    {
        FlowerEffItemController ctrl = null;
        _flowerEffList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<FlowerEffItemController, FlowerEffItem>(View.EffGrid_UIGrid.gameObject, FlowerEffItem.NAME);
            _flowerEffList.Add(ctrl);
        }

        return ctrl;
    }
}
