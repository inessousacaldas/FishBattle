// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GMCodeViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial interface IGMCodeViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(int index,IGMData data);
}
public partial class GMCodeViewController
{
    private TabbtnManager tabMgr = null;
    private List<GMCodeItemController> itemList = new List<GMCodeItemController>();
    private int curCount;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        CreateTabItem();
    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_UIGrid.gameObject
                    , TabbtnPrefabPath.TabBtnWidget_H4.ToString()
                    , "Tabbtn_" + i);
        ITabInfo[] tabInfoList =
                {
                    TabInfoData.Create(0, "全部"),
                };
        tabMgr = TabbtnManager.Create(tabInfoList,func);
    }

    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        if(itemList != null)
        {
            ClearItem();
            itemList.Clear();
            tabMgr = null;
        }
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(IGMData data)
    {
        if(data.CurCodeVO != null)
        {
            View.txtInput_UIInput.value = data.CurCodeVO.FullCode;
            View.lblDesc_UILabel.text = data.CurCodeVO.cfgVO.tooltip;
        }else
        {
            View.txtInput_UIInput.value = "";
            View.lblDesc_UILabel.text = "";
        }
        UpdateTabItem(data);
        OnTabChange((int)data.CodeTab,data);
    }

    private void UpdateTabItem(IGMData data)
    {
        Hash<string,ITabInfo> tabInfoList = new Hash<string, ITabInfo>();
        var i = 0;
        tabInfoList.Add("全部",TabInfoData.Create(i,"全部"));
        foreach(var type in data.CodeTabDict.Values)
        {
            i++;
            tabInfoList.Add(type,TabInfoData.Create(i,type));
        }
        tabMgr.UpdateTabs(tabInfoList.List,data.CodeTab);
    }

    public void OnTabChange(int index,IGMData data)
    {
        curCount = 0;
        View.scrollView_UIScrollView.ResetPosition();
        if(index == 0)
        {
            UpdateItem(data.CodeDict);
        }
        else
        {
            var dataList = data.GetCodeListByIndex(index);
            UpdateItem(dataList,Vector3.zero);
        }
        HideItem();
    }

    private void UpdateItem(List<GMDataCodeVO> list,Vector3 startIndex)
    {
        var row = 8;
        for(var i = 0;i < list.Count;i++)
        {
            GMCodeItemController itemCtrl;
            if(curCount < itemList.Count)
            {
                itemCtrl = itemList[curCount];
            }
            else
            {
                itemCtrl = AddCachedChild<GMCodeItemController,GMCodeItem>(View.Container_UIWidget.gameObject,GMCodeItem.NAME,GMCodeItem.NAME);
                itemCtrl.View.gameObject.name = GMCodeItem.NAME + curCount;
                GMDataMgr.GMCodeViewLogic.InitReactiveEvents(this,itemCtrl);
                itemList.Add(itemCtrl);
            }
            curCount++;
            itemCtrl.Show();
            itemCtrl.View.transform.localPosition = new UnityEngine.Vector3(55 + (i % row) * 110,-15 - (i / row) * 35 + startIndex.y);
            itemCtrl.UpdateView(list[i]);
        }
    }

    private void UpdateItem(Dictionary<string,List<GMDataCodeVO>> allDict)
    {
        var pos = Vector3.zero;
        foreach(var key in allDict.Keys)
        {
            var list = allDict[key];
            UpdateItem(list,pos);
            pos.y -= Mathf.Ceil(list.Count / 8f) * 35 + 10;
        }
        View.Container_UIWidget.height = Math.Abs((int)pos.y);
    }

    private void HideItem()
    {
        if(curCount < itemList.Count)
        {
            for(var i = curCount;i < itemList.Count;i++)
            {
                itemList[i].Hide();
            }
        }
    }

    private void ClearItem()
    {
        if(itemList.Count > 0)
        {
            for(var i = 0;i < itemList.Count;i++)
            {
                RemoveCachedChild<GMCodeItemController,GMCodeItem>(itemList[i]);
            }
        }
    }
}
