// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  WareHousePopupListController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IS3PopupListController
{

    UniRx.IObservable<PopUpItemInfo> OnChoiceIndexStream { get; }
}
public partial class S3PopupList
{
    //Prefab名字
    public const string PREFAB_WAREHOUSE = "S3PopupList"; //仓库使用
}
public partial class S3PopupItem
{
    public const string PREFAB_WAREHOUSEIITEM = "S3PopupItem";//仓库使用
    public const string PREFAB_EQUIPMENT = "S3PopupItem_Equip";//装备使用
    public const string PREFAB_TEAMBTN = "S3PopupTeamBtn";//组队使用
}
public partial class S3PopupListController
{
    #region Event
    Subject<PopUpItemInfo> onChoiceIndexStream = new Subject<PopUpItemInfo>();
    public UniRx.IObservable<PopUpItemInfo> OnChoiceIndexStream { get { return onChoiceIndexStream; } }

    /// <summary>
    /// 点解PopupList以外的位置
    /// </summary>
    public event Action OnClickOtherEvt;
    #endregion


    private List<S3PopupItemController> itemCtrl = new List<S3PopupItemController>();
    private bool isShowList = true;
    private string ItemPrefab = S3PopupItem.NAME;
    private List<PopUpItemInfo> nameList = new List<PopUpItemInfo>();
    private S3PopUpItemData OnNormalItemData;
    private S3PopUpItemData OnChoiceitemData;
    private bool isClickHide; //是否点击外部自动关闭
    private CompositeDisposable _mydisposable = new CompositeDisposable();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        HideList();
    }
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnBg_UIButtonClick.Subscribe(_ => {
            if (!isShowList)
            {
                ShowList();
            }
            else
            {
                if (isClickHide)
                    HideList();
            }
                
        });

        UICamera.onClick += OnClickCheck;
        UICamera.onDrag += OnDragCheck;
    }
    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        UICamera.onClick -= OnClickCheck;
        UICamera.onDrag -= OnDragCheck;
    }
    private void OnDragCheck(GameObject go, Vector2 delta)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
        {
            if(isClickHide)
                HideList();
            if (OnClickOtherEvt != null)
                OnClickOtherEvt();
        }
    }

    private void OnClickCheck(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
        {
            if(isClickHide)
            {
                if (go == View.Bg_UISprite.gameObject)
                    return;
                else
                    HideList();
            }
            if (OnClickOtherEvt != null)
                OnClickOtherEvt();
        }
    }

    protected override void OnDispose()
    {
        
    }

    /// <summary>
    /// 获取Prefab，自定义设置样式
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public S3PopupItemController GetItemObj(int enumValue)
    {
        int idx = nameList.FindIndex(x => x.EnumValue == enumValue);
        return itemCtrl[idx];
    }
    /// <summary>
    /// 设置Bg名字
    /// </summary>
    /// <param name="str"></param>
    public void SetBgLabel(string str)
    {
        View.PopupLabel_UILabel.text = str;
    }
    /// <summary>
    /// 更改某一个Item的名字~
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="Name"></param>
    public void SetPopupItemName(int enumValue, string Name)
    {
        int idx = nameList.FindIndex(x => x.EnumValue == enumValue);
        nameList[idx].Name = Name;
        GetItemObj(enumValue).SetName(Name);
    }
    /// <summary>
    /// 设置Bg是否显示
    /// </summary>
    /// <param name="common"></param>
    public void SetBgActive(bool common)
    {
        View.Bg_UISprite.gameObject.SetActive(common);
    }
    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="pos">世界坐标</param>
    public void SetPos(Vector3 pos)
    {
        this.transform.position = pos;
    }
    public void SetChoice(int enumValue, bool isSetLabel = false)
    {
        if (nameList == null)
            return;
        int idx = nameList.FindIndex(x => x.EnumValue == enumValue);
        if (idx < 0) return;
        itemCtrl.ForEachI((ctrl, i) => {
             bool isChoice = i == idx;
             ctrl.UpdateView(OnChoiceitemData, OnNormalItemData, nameList[i].Name, isChoice);
        });
        if(isSetLabel)
        {
            SetBgLabel(nameList[idx].Name);
        }
    }

    /// <summary>
    /// 设置列表背景的宽高
    /// </summary>
    /// <param name="widht"></param>
    /// <param name="height"></param>
    public void SetListBgDimensions(int widht =-1,int height=-1)
    {
        if(widht > 0)
        View.ItemListBg_UISprite.width = widht;
        if (height > 0)
            View.ItemListBg_UISprite.height = height;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="list"></param>
    /// <param name="OnChoiceitemData"></param>
    /// <param name="OnNormalItemData"></param>
    /// <param name="itemSpace"></param>
    /// <param name="ChoiceIndex">当前选中</param>
    /// <param name="isShowList">默认是否展示内部的list</param>
    /// <param name="isClickHide">是否点击自动隐藏内部的List</param>
    public void InitData(string prefabName = S3PopupItem.NAME,
        List<PopUpItemInfo> list = null,
        S3PopUpItemData OnChoiceitemData = null,
        S3PopUpItemData OnNormalItemData = null,
        int itemSpace = 45,
        int ChoiceIndex = 0,
        bool isShowList = false,
        bool isShowBg = true,
        bool isClickHide = true)
    {
        this.ItemPrefab = prefabName;
        this.OnChoiceitemData = OnChoiceitemData;
        this.OnNormalItemData = OnNormalItemData;
        this.isClickHide = isClickHide;

        SetBgActive(isShowBg);
        if (isShowList)
            ShowList();

        //this.nameList = list;
        if (list != null)
        {
            this.nameList = list;
            UpdateView(list, itemSpace);
        }
    }
    //public void InitData(string prefabName = S3PopupItem.NAME, List<string> nameStrs = null, S3PopUpItemData OnChoiceitemData = null, S3PopUpItemData OnNormalItemData = null, int itemSpace = 45, int ChoiceIndex = 0)
    //{
    //    nameList.Clear();
    //    nameStrs.ForEachI((x, i) =>
    //    {
    //        nameList.Add(new PopUpItemInfo(x, i));
    //    });
    //    InitData(prefabName, nameStrs, OnChoiceitemData, OnNormalItemData, itemSpace, ChoiceIndex);
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="OnChoiceitemData">选中效果</param>
    /// <param name="OnNormalItemData">没有选中的效果</param>
    /// <param name="nameStrs">名字集</param>
    /// <param name="itemSpace">一个Item的占用位置 = size+ padding</param>
    /// <param name="ChoiceIndex">当前选中</param>
    public void UpdateView(List<PopUpItemInfo> nameStrs,int itemSpace = 45, int ChoiceIndex = 0)
    {
        _mydisposable.Clear();
        itemCtrl.ForEach(x => x.Hide());
        var _ChoiceIndex = ChoiceIndex;
        this.nameList = nameStrs;
        nameStrs.ForEachI((x, i) => {
            S3PopupItemController ctrl;
            if (itemCtrl.Count <= i)
            {
                ctrl = AddChild<S3PopupItemController, S3PopupItem>(View.itemContent, this.ItemPrefab, "popup" + i);
                itemCtrl.Add(ctrl);
                var parentDepth = View.gameObject.ParentPanelDepth();
                View.gameObject.ResetPanelsDepth(parentDepth + 2);           
            }
            ctrl = itemCtrl[i];
            bool isChoice = _ChoiceIndex == i;
            ctrl.UpdateView(OnChoiceitemData, OnNormalItemData, x.Name, isChoice);
            _mydisposable.Add(ctrl.OnBg_UIButtonClick.Subscribe(_ =>
            {
                onChoiceIndexStream.OnNext(x);
                if (isClickHide)
                    HideList();
            }));
            ctrl.Show();
        });
        //Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(View.itemContent.transform);
        View.ItemListBg_UISprite.height = itemSpace * nameStrs.Count;
        var table =  View.itemContent.GetComponent<UITable>();
        if (table != null)
            table.Reposition();

        //View.PopupLabel_UILabel.text = nameStrs[ChoiceIndex];
    }
    public void UpdateView(List<string> nameStrs,   int itemSpace = 45, int ChoiceIndex = 0)
    {
        if (nameList == null)
            return;
        nameList.Clear();
        nameStrs.ForEachI((x, i) => {
            nameList.Add(new PopUpItemInfo(x, i));
        });
        UpdateView(nameList, itemSpace, ChoiceIndex);
        //View.PopupLabel_UILabel.text = nameStrs[ChoiceIndex];
    }
    private void ShowList()
    {
        if (View.itemList.activeSelf)
        {
            View.itemList.gameObject.SetActive(false);
            isShowList = false;
        }
        else
        {
            View.itemList.gameObject.SetActive(true);
            isShowList = true;
        }
    }
    private void HideList()
    {
        View.itemList.gameObject.SetActive(false);
        isShowList = false;
    }
}
