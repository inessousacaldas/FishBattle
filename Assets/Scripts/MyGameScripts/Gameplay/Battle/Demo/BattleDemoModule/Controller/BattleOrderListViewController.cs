using System.Collections.Generic;
using BattleDemoModel = BattleDataManager.BattleDemoModel;

public class BattleOrderListViewController : MonoViewController<BattleOrderListView>
{
    private const string battleOrderItemCellPath = "BattleOrderCell";
    private List<BattleOrderCellController> cellPools = new List<BattleOrderCellController>();
    private System.Action<BattleOrderInfo> mOrderSelectedHandler;

    private int _orderType = 1;
    //1-己方，2-敌方
    private long _targetId;

    #region IViewController implementation

    public void Open(int orderType, long targetId, System.Action<BattleOrderInfo> pOrderSelectedHandler)
    {
        _orderType = orderType;
        _targetId = targetId;
        mOrderSelectedHandler = pOrderSelectedHandler;

        IEnumerable<BattleOrderInfo> tempList = null;
        if (_orderType == 1)
        {
            tempList = BattleDataManager.DataMgr.BattleDemo.getMyOrderList();
        }
        else if (_orderType == 2)
        {
            tempList = BattleDataManager.DataMgr.BattleDemo.getEnemyOrderList();
        }

        var dataList = tempList.Filter(order =>
            !string.IsNullOrEmpty(order.orderName) || order.isAddButton || order.isClearButton || order.isAllClearButton)
            .ToList();

        for (int i = 0; i < dataList.Count; i++)
        {
            if ((i + 1) > cellPools.Count)
            {
                var com = AddCachedChild<BattleOrderCellController,BattleOrderCell>(View.itemGrid.gameObject, BattleOrderCell.NAME);
                cellPools.Add(com);
            }

            cellPools[i].setData(dataList[i], OnItemCellSelect);
        }

        for (var i = 0; i < cellPools.Count; i++)
        {
            cellPools[i].gameObject.SetActive(i < dataList.Count);
        }

        View.itemGrid.Reposition();

        var newLen = dataList.Count - 7;
        if (newLen <= 1)
        {
            View.ContentBg.height = 298;
        }
        else if (newLen == 2 || newLen == 3)
        {
            View.ContentBg.height = 364;
        }
        else if (newLen == 4 || newLen == 5)
        {
            View.ContentBg.height = 434;
        }
        else if (newLen == 6)
        {
            View.ContentBg.height = 500;
        }
        BattleDataManager.DataMgr.SetState(BattleSceneStat.ON_COMMAND_ENTER);
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.BgCollider_UIEventTrigger.onClick, OnClickBgCollider);
    }

    #endregion

    protected override void OnDispose()
    {
    }

    private void OnClickBgCollider()
    {
        CloseView(null);
    }

    private void OnItemCellSelect(BattleOrderCellController cell)
    {
        SelectedOrderCell = cell;
        if (cell.info == null) return;
        if (cell.info.isAddButton)
        {
            ProxyBattleDemoModule.OpenBattleOrderEditorView(cell.info.type);
            ProxyBattleDemoModule.HideBattleOrderListView();
        }
        else
        {
            var battleId = BattleDataManager.DataMgr.GetCurrentGameVideoId();

            if (cell.info.isClearButton)
            {
                OnSelectedOrder(cell.info);
            }
            else if (cell.info.isAllClearButton)
            {
                ClearAllOrder(cell.info);
            }
            else if (!string.IsNullOrEmpty(cell.info.orderName))
            {
                OnSelectedOrder(cell.info);
            }
        }
    }

    private void ClearAllOrder(BattleOrderInfo pBattleOrderInfo)
    {
        OnSelectedOrder(pBattleOrderInfo);
    }

    private void CloseView(BattleOrderInfo pBattleOrderInfo)
    {
        ProxyBattleDemoModule.HideBattleOrderListView();
        GameDebuger.TODO(@"ProxyMainUI.CloseBattleBuffTipsView();");
        if (null != mOrderSelectedHandler)
            mOrderSelectedHandler(pBattleOrderInfo);
    }

    private void OnSelectedOrder(BattleOrderInfo pBattleOrderInfo)
    {
        CloseView(pBattleOrderInfo);
        if (null != mOrderSelectedHandler)
            mOrderSelectedHandler(pBattleOrderInfo);
    }

    #region 选中的ITEM

    private BattleOrderCellController mSelectedOrderCell;

    private BattleOrderCellController SelectedOrderCell
    {
        get
        {
            return mSelectedOrderCell;
        }
        set
        {
            if (mSelectedOrderCell == value) return;
            if (null != mSelectedOrderCell)
                mSelectedOrderCell.Selected = false;
            mSelectedOrderCell = value;
            if (null != mSelectedOrderCell)
                mSelectedOrderCell.Selected = true;
        }
    }

    #endregion
}

