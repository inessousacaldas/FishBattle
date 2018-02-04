// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CopyMissionViewController.cs
// Author   : DM-PC092
// Created  : 1/20/2018 11:55:37 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface ICopyMissionViewController
{
    UniRx.IObservable<CopyItemData> GetIconClickEvt { get; }
    int GetdefaultNormal { get; }
    int GetdefaultElite { get; }
}
public partial class CopyMissionViewController{
    private Dictionary<int,Copy> mNormalCopyLevel = new Dictionary<int, Copy>();
    private Dictionary<int,Copy> mEliteCopyLevel = new Dictionary<int, Copy>();
    private Dictionary<GameObject,CopyPanelItemViewController> mCopyItemList = new Dictionary<GameObject,CopyPanelItemViewController>();
    private List<ItemCellController> mRewardIds = new List<ItemCellController>();
    private Subject<CopyItemData> _iconClickEvt = new Subject<CopyItemData>();
    public UniRx.IObservable<CopyItemData> GetIconClickEvt { get { return _iconClickEvt; } }
    private int curCopyType = -1;
    private int curCopyID = 0;
    private string SelectTipSprite = "btn_Tab2_Select";
    private string UnSelectTipSprite = "btn_Tab2_Normal";
    private string SelectCopyItem = "bg_Option_1Select";
    private string UnSelectCopyItem = "bg_Option_1Normal";
    //切换普通副本界面时候的默认ID
    public int defaultNormal = 0;
    //切换精英副本界面时候的默认ID
    public int defaultElite = 0;
    public int GetdefaultNormal
    {
        get { return defaultNormal; }
    }
    public int GetdefaultElite
    {
        get { return defaultElite; }
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for(int index = 0;index < 6;index++)
        {
            CopyPanelItemViewController mTreasurlRewardShowController= AddCachedChild<CopyPanelItemViewController,CopyPanelItemView>(View.UIGrid_UIGrid.gameObject,CopyPanelItemView.NAME);
            mCopyItemList.Add(mTreasurlRewardShowController.gameObject,mTreasurlRewardShowController);
            _disposable.Add(mTreasurlRewardShowController.GetClickEvt.Subscribe(d => { _iconClickEvt.OnNext(d); }));
        }

        for(int i = 0;i < 4;i++)
        {
            ItemCellController _cell = AddCachedChild<ItemCellController,ItemCell>(_view.DescBG_GameObject,ItemCell.NAME);
            _cell.CanDisplayCount(false);
            _cell.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
            _cell.transform.localPosition = new Vector3(-128 + 84 * i,-191);
            mRewardIds.Add(_cell);
        }
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        _view.UIGrid_UIGrid.onUpdateItem = UndateSimpleList;
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        curCopyType = -1;
        curCopyID = 0;
        mCopyItemList.Clear();
        mNormalCopyLevel.Clear();
        mEliteCopyLevel.Clear();
        mRewardIds.Clear();
        _iconClickEvt.CloseOnceNull();
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        _disposable = new CompositeDisposable();
        List<Copy> mCopyLevel = DataCache.getArrayByCls<Copy>();
        int a = 0,b = 0;
        mCopyLevel.ForEach(e =>
        {
            if(e.refreshId == 0)
            {
                mNormalCopyLevel.Add(a,e);
                a++;
            }
            else if(e.refreshId == 1)
            {
                mEliteCopyLevel.Add(b,e);
                b++;
            }
        });
        defaultNormal = mNormalCopyLevel[0].id;
        defaultElite = mEliteCopyLevel[0].id;
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICopyPanelData data){
        ChangeTitleTipStyle(data.GetCopyType());
        UndateList(data.GetCopyType());
        OnClickHandler(data.GetCurCopyID());
    }

    public void OnClickHandler(int index) {
        Dictionary<int,Copy> tempCopy = new Dictionary<int, Copy>();
        if(curCopyType == 0)
            tempCopy = mNormalCopyLevel;
        else if(curCopyType == 1)
            tempCopy = mEliteCopyLevel;

        int rewardIndex = 0;
        if(tempCopy.ContainsKey(index))
        {
            curCopyID = index;
            string tDes = tempCopy[index].desc;
            string tName = tempCopy[index].name;
            List<int> rewardIds = tempCopy[index].rewardIds;
            foreach(int i in rewardIds)
            {
                var itemData = ItemHelper.GetGeneralItemByItemId(i);
                if(itemData == null)
                    return;
                mRewardIds[rewardIndex].UpdateView(itemData);
                rewardIndex++;
            }

            for(int i = 0;i < mRewardIds.Count;i++)
            {
                if(i < rewardIndex)
                    mRewardIds[i].SetActive(true);
                else
                    mRewardIds[i].SetActive(false);
            }
            View.CopyContent_UILabel.text = tDes;
            mCopyItemList.ForEach(e => {
                e.Value.ChangeSlectStyle(index);
            });
        }
        else {
            if(mCopyItemList.Count <= 0) {
                GameDebuger.LogError("导表数据为空");
                return;
            }
            mCopyItemList.Values.ToList()[0].OnClickHandler();
        }
    }

    private void UndateSimpleList(GameObject go,int itemIdx,int dataIdx)
    {
        Dictionary<int,Copy> tempCopy = new Dictionary<int, Copy>();
        if(curCopyType == 0)
            tempCopy = mNormalCopyLevel;
        else if(curCopyType == 1)
            tempCopy = mEliteCopyLevel;
        if(tempCopy == null || tempCopy.Count == 0) return;
        CopyPanelItemViewController item = null;
        if(mCopyItemList.TryGetValue(go,out item))
        {
            var data = tempCopy.TryGetValue(dataIdx);
            if(data.Value != null)
            {
                item.Init(dataIdx,data.Value);
                item.ChangeSlectStyle(curCopyID);
            }
        }
    }

    void UndateList(int index) {
        Dictionary<int,Copy> tempCopy = new Dictionary<int, Copy>();
        if(index == 0)
            tempCopy = mNormalCopyLevel;
        else if(index == 1)
            tempCopy = mEliteCopyLevel;
        if(index == curCopyType) {
            curCopyType = index;
            View.UIGrid_UIGrid.UpdateChildrenData();
        }
        else {
            curCopyType = index;
            View.UIGrid_UIGrid.UpdateDataCount(tempCopy.Count,true);
            _view.UIGrid_UIGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        }
    }

    void ChangeTitleTipStyle(int index)
    {
        if(index == 0)
        {
            View.NormalUISprite_UISprite.spriteName = SelectTipSprite;
            View.EliteUISprite_UISprite.spriteName = UnSelectTipSprite;
            View.NormalUILabel_UILabel.effectStyle = UILabel.Effect.None;
            View.NormalUILabel_UILabel.color = Color.black;
            View.EliteUILabel_UILabel.effectStyle = UILabel.Effect.Outline8;
            View.EliteUILabel_UILabel.color = new Color(229,229 ,229);
        }
        else if(index == 1)
        {
            View.NormalUISprite_UISprite.spriteName = UnSelectTipSprite;
            View.EliteUISprite_UISprite.spriteName = SelectTipSprite;
            View.NormalUILabel_UILabel.effectStyle = UILabel.Effect.Outline8;
            View.NormalUILabel_UILabel.color = new Color(229,229,229);
            View.EliteUILabel_UILabel.effectStyle = UILabel.Effect.None;
            View.EliteUILabel_UILabel.color = Color.black;
        }
    }
}
