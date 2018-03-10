using UnityEngine;
using System.Collections.Generic;

public class BattleOrderEditorViewController : MonoViewController<BattleOrderEditorView>
{
    private const string battleOrderItemCellPath = "BattleOrderEditorCell";


    private List<BattleOrderEditorCellController> cellPools = new List<BattleOrderEditorCellController>();
    private int _orderType = 1;
//1-己方，2-敌方

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.CloseBtn.onClick, OnClickCloseButton);
        EventDelegate.Set(View.enemyButton.onClick, OnClickEnemyButton);
        EventDelegate.Set(View.myButton.onClick, OnClickMyButton);

        GameEventCenter.AddListener(GameEvent.BATTLE_UI_ONORDERLISTUPDATE, setData);
    }

    public void setData()
    {
        IEnumerable<BattleOrderInfo> tempList = null;
        if (_orderType == 1)
        {
            tempList = BattleDataManager.DataMgr.BattleDemo.getMyOrderList();
        }
        else if (_orderType == 2)
        {
            tempList = BattleDataManager.DataMgr.BattleDemo.getEnemyOrderList();
        }

        var dataList = tempList.Filter(order=>!string.IsNullOrEmpty(order.orderName) || order.isAddButton )
            .GetElememtsByRange(0, 9)
            .ToList();
        
        for (int i = 0; i < dataList.Count; i++)
        {
            if ((i + 1) > cellPools.Count)
            {
                var com = AddCachedChild<BattleOrderEditorCellController,BattleOrderEditorCell>(View.itemGrid.gameObject, BattleOrderEditorCell.NAME);
                cellPools.Add(com);
            }

            cellPools[i].setData(dataList[i], OnItemCellSelect);
        }

        for (int i = 0; i < cellPools.Count; i++)
        {
            cellPools[i].gameObject.SetActive(i < dataList.Count);
        }


        View.itemGrid.Reposition();


        if (dataList.Count > 8)
        {
            View.ScrollView.SetDragAmount(0, 1f, false);
        }
        else
        {
            View.ScrollView.ResetPosition();
        }

        btnShowLogic();
    }

    private void OnItemCellSelect(BattleOrderEditorCellController cell)
    {
        if (null == cell) return;
        
        var info = cell.info;
        if (info == null) return;
        
        if (info.isAddButton)
        {
            ProxyWindowModule.OpenInputWindow(0, 8, "指 令", "输入指令：(最多4个字)", "请输入指令", "", str =>
            {
                if (!string.IsNullOrEmpty(str))
                {
                    BattleDataManager.DataMgr.BattleDemo.modifyOrderItemData(info.type, 0, str, true);
                }
            });
        }
        else
        {
            if (info.canEdit)
            {
                ProxyWindowModule.OpenInputWindow(0, 8, "指 令", "输入指令：(最多4个字)", "", info.orderName, str =>
                {
                    BattleDataManager.DataMgr.BattleDemo.modifyOrderItemData(info.type, info.index, str);
                });
            }
        }
    }

    private void OnClickCloseButton()
    {
        ProxyBattleDemoModule.CloseBattleOrderEditorView();
    }

    private void OnClickEnemyButton()
    {
        _orderType = 2;
        setData();
    }

    private void OnClickMyButton()
    {
        _orderType = 1;
        setData();
    }

    private void btnShowLogic()
    {
        View.myButton.normalSprite = _orderType == 1 ? "little-button-selected" : "little-button";
        View.enemyButton.normalSprite = _orderType == 2 ? "little-button-selected" : "little-button";
    }

    public int orderType
    {
        get
        {
            return _orderType;
        }
        set
        {
            _orderType = value;
        }

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        cellPools.Clear();
        GameEventCenter.RemoveListener(GameEvent.BATTLE_UI_ONORDERLISTUPDATE, setData);
    }
}