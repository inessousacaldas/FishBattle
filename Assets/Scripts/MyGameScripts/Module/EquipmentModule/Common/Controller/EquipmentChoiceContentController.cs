// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentChoiceContentController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;
public partial interface IEquipmentChoiceContentController
{
    UniRx.IObservable<EquipmentDto> OnChoiceStream { get; }
    TabbtnManager Tabbtn { get; }
}
public partial class EquipmentChoiceContentController
{
    public static ITabInfo[] tabinfos = new ITabInfo[]
    {
        TabInfoData.Create((int)EquipmentMainDataMgr.EquipmentHoldTab.Equip,"已装备"),
        TabInfoData.Create((int)EquipmentMainDataMgr.EquipmentHoldTab.Bag,"包裹"),
    };
    CompositeDisposable _disposble;
    TabbtnManager tabbtn;
    public TabbtnManager Tabbtn { get { return tabbtn; } }
    Subject<EquipmentDto> _onChoiceStream = new Subject<EquipmentDto>();
    public UniRx.IObservable<EquipmentDto> OnChoiceStream { get { return _onChoiceStream; } }


    List<EquipmentSmithCellController> smithCellCtrls = new List<EquipmentSmithCellController>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposble = new CompositeDisposable();
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                   View.tabBtn_1
                   , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
                   , "Tabbtn_" + i);
        tabbtn = TabbtnManager.Create(tabinfos, func);
        tabbtn.SetBtnLblFont(20, "444244FF", 18, "B5BAB5FF");
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    EquipmentMainDataMgr.EquipmentHoldTab CurTab;
    public void UpdateViewData(EquipmentMainDataMgr.EquipmentHoldTab curTab,IEnumerable<EquipmentDto> curList,EquipmentDto curChoice)
    {
        tabbtn.SetTabBtn((int)curTab);
        _disposble.Clear();

        smithCellCtrls.ForEach(x => x.Hide());
        curList.ForEachI((item, i) =>
        {
            if (smithCellCtrls.Count <= i)
            {
                var ctrl = AddChild<EquipmentSmithCellController, EquipmentSmithCell>(View.EquipmentCellContent_UIGrid.gameObject, EquipmentSmithCell.NAME);
                smithCellCtrls.Add(ctrl);
            }
            int index = i;
            var tempItem = item;
            bool isChoice = curChoice != null && curChoice.equipUid == tempItem.equipUid;
            smithCellCtrls[i].UpdateViewData(index, tempItem.equip as Equipment, isChoice, EquipmentSmithCellController.ShowType.Two,tempItem.property.quality);
            smithCellCtrls[i].Show();
            _disposble.Add(smithCellCtrls[i].OnEquipmentSmithCell_UIButtonClick.Subscribe(_ =>
            {
                _onChoiceStream.OnNext(tempItem);
            }));
        });
        View.EquipmentCellContent_UIGrid.Reposition();
        if (CurTab != curTab)
        {
            View.scrollView_UIScrollView.ResetPosition();
        }
        CurTab = curTab;
    }
}
