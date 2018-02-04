
// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PropsCompositeViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public sealed partial class BackpackDataMgr
{
    public partial class PropsCompositeViewController    {
        public static void Open(CompositeTabType type, BagItemDto dto)
        {
	        DataMgr._data.CurCompositTab = type;
	        int len = 0;
	        if (type == CompositeTabType.Composite)
	        {
		        if (dto == null)
			        DataMgr._data.CompositeSelIdx = -1;
		        else
		        {
			        var items = DataMgr._data.GetCompositeItems(out len);
			        DataMgr._data.CompositeSelIdx = items.FindElementIdx(s=>s.index == dto.index);    
		        }
	        }else if(type == CompositeTabType.DeComposite)
            {
                if(dto == null)
                {
                    var items = DataMgr._data.GetDeComposeItems(DataMgr._data.CurDecomposeTab, out len);
                    var itemList =  items.ToList();
                    if(itemList.Count > 0)
                    {
                        DataMgr._data.DecomposeSelIdx = 0;
                    }
                    else
                        DataMgr._data.DecomposeSelIdx = -1;
                }   
                else
                {
                    var items = DataMgr._data.GetDeComposeItems(DataMgr._data.CurDecomposeTab,out len);
                    DataMgr._data.DecomposeSelIdx = items.FindElementIdx(s => s.index == dto.index);
                }
            }
	        
		    UIModuleManager.Instance.OpenFunModule<PropsCompositeViewController>(
			    PropsCompositeView.NAME
			    , UILayerType.DefaultModule
			    ,true);
	    }

	    private static ITabInfo[] TabInfoArr = new ITabInfo[2]
	    {
		    TabInfoData.Create((int)CompositeTabType.Composite, "合成")
			    , TabInfoData.Create((int)CompositeTabType.DeComposite, "分解"),
	    };

	    private ICompositeItem curMaterialItem;
	    private ICompositeItem outComItem;

	    private int curComposePage;
	    private int curDecomposePage;
	    private ItemsPageContainerController itemsContainer;
	    private PageTurnViewController composePageTurn;
	    private ItemCellController materialCell;
	    private ItemCellController outcomeCell;
	    
	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
	        InitPageTab();

	        InitItemsContainer();

	        InitCompositePageTurn();

	        InitConvinienceCheckBox();
	        
	        InitComposeItemCell();

            InitDecPageTabBtns();

            InitDecomposeItemCell();

            InitDecomposePageTurn();

            InitPopupPanel();

        }
	     
	    private void InitComposeItemCell()
	    {
		    materialCell = AddController<ItemCellController, ItemCell>(
				    _view.materialItem
			    );
		    
		    outcomeCell = AddController<ItemCellController, ItemCell>(
			    _view.outcome
		    );
	    }

	    private void InitConvinienceCheckBox()
	    {
		    var checkBox = AddController<GenericCheckBoxController, GenericCheckBox>(
			    _view.GenericCheckBox);
            checkBox.UpdateView(GenericCheckBoxData.Create("便捷合成", DataMgr._data.IsConvinienceComposite));

            checkBox.ClickStateHandler.Subscribe(_isSelect =>
            {
                DataMgr._data.isConvinienceComposite = _isSelect;
                FireData();
            });

        }

	    private void InitCompositePageTurn()
	    {
		    composePageTurn = AddController<PageTurnViewController, PageTurnView>(
				_view.ComposePageTurn
		    );
            composePageTurn.InitData_NumberInputer(0, 0, pos: PageTurnViewController.InputerShowPos.Down);

            //composePageTurn.InitData(min:0,
            //    showType: ShowType.singleNum_Zero
            //    , enableInput:true);

		    _disposable.Add(composePageTurn.Stream.Subscribe(
			    pageIdx =>
			    {
				    //更新合成消耗
				    var max = GetComposeMaxTime();
                    DataMgr._data.compositeTimes = Math.Min(pageIdx, max);
                    //composePageTurn.UpdateView(DataMgr._data.CompositeTimes, max + 1);
                    composePageTurn.SetPageInfo(DataMgr._data.CompositeTimes, max);

                    UpdateComposePrice(DataMgr._data);
			    }));
	    }
	    
		//这是一个显示需求： 最小显示一个，即使材料不足
	    private int GetComposeMaxTime()
	    {
            if (curMaterialItem == null)
                return 0;
		    var props = DataCache.getDtoByCls<CompositeProps>(curMaterialItem.Item.id);
		    if (props == null)
			    return 1;
		    return outComItem == null ? 1 : outComItem.Cnt / props.gainCount;				    
	    }

	    private void InitItemsContainer()
	    {
		    itemsContainer = AddChild<ItemsPageContainerController, ItemsPageContainer>(
			    _view.ItemsPos
			    , ItemsPageContainer.NAME
		    );
            	        
		    itemsContainer.PageMoveAsObserver.Subscribe(page =>
		    {
			    if (DataMgr._data.CompositeViewData.CurCompositTab == CompositeTabType.Composite)
			    {
				    curComposePage = page;
			    }
			    else
			    {
				    curDecomposePage = page;  
			    }
		    });
            
		    itemsContainer.OnItemClick.Subscribe(itemIdx =>
		    {
			    if (DataMgr._data.CompositeViewData.CurCompositTab == CompositeTabType.Composite)
			    {
                    int len;
                    var items = DataMgr._data.GetCompositeItems(out len)
                    .ToList().TryGetValue(itemIdx); ;
                    if(items != null)
                        DataMgr._data.CompositeSelIdx = itemIdx;
			    }
			    else
			    {
                    int len;
                    var items = DataMgr._data.GetDeComposeItems(DataMgr._data.CurDecomposeTab
                    , out len).ToList().TryGetValue(itemIdx);
                    if(items != null)
                        DataMgr._data.DecomposeSelIdx = itemIdx;
			    }
			    FireData();
		    });
            itemsContainer.UpdateTabBtns(DataMgr._data.Composite_itemsPageTab, 0);
            itemsContainer.OnTabbtnClick.Subscribe(i =>
            {
                DataMgr._data.CurDecomposeTab = (DecompositeTabType)DataMgr._data.Composite_itemsPageTab[i].EnumValue;
                FireData();
            });
        }

	    private void InitPageTab()
	    {
		    var idx = TabInfoArr.FindElementIdx(
			    s=>(CompositeTabType)s.EnumValue == DataMgr._data.CurCompositTab);
		    var tabBtnMgr = TabbtnManager.Create(
			    TabInfoArr
			    , delegate(int i)
			    {
				    return AddChild<TabBtnWidgetController, TabBtnWidget>(
					    _view.WinTabGroup
					    , TabbtnPrefabPath.TabBtnWidget.ToString()
					    , "Tabbtn_" + i);
			    }
			    , idx);
		    _disposable.Add(tabBtnMgr.Stream.Subscribe(
			    pageIdx =>
			    {
				    DataMgr._data.CurCompositTab = (CompositeTabType)pageIdx;
				    FireData();
			    }
		    ));
                     	        
	    }

	    // 客户端自定义代码
	    protected override void RegistCustomEvent ()
	    {

	    }

        protected override void OnDispose()
        {
	        _disposable.Dispose();
            _disposable = null;
        }

	    //在打开界面之前，初始化数据
	    protected override void InitData()
    	{
            _disposable = new CompositeDisposable();
    	}

        // 业务逻辑数据刷新
        private void UpdateDataAndView(IBackpackData _data)
        {
	        var data = _data.CompositeViewData;

	        var temp = curMaterialItem ;           

            curMaterialItem = data.CurMaterialItem;
	        outComItem = data.OutcomeItem;

            var deCompositeItem = curDecMaterialItem;
            curDecMaterialItem = data.CurDecomposeItem;

            if (temp == null
                || curMaterialItem == null
                || curMaterialItem.ItemID != temp.ItemID
	            || curMaterialItem.Cnt != temp.Cnt
	            || (curMaterialItem.CirType != temp.CirType
	                && !data.IsConvinienceComposite))
	        {
                var max = GetComposeMaxTime();
                DataMgr._data.compositeTimes = Math.Min(1, max);
                //DataMgr._data.compositeTimes = 1;
	        }
	        else
	        {
		        var max = GetComposeMaxTime();
		        DataMgr._data.compositeTimes = Math.Min(DataMgr._data.compositeTimes, max);
	        }

            if (deCompositeItem == null
                || curDecMaterialItem == null
                || deCompositeItem.Item.id != curDecMaterialItem.Item.id
                || deCompositeItem.Cnt != curDecMaterialItem.Cnt)
            {
                var max = GetDecomposeMaxTime();
                DataMgr._data.DecomposeTimes = Math.Min(1, max);
                //DataMgr._data.DecomposeTimes = 1;
            }
            else
            {
                var max = GetDecomposeMaxTime();
                DataMgr._data.DecomposeTimes = Math.Min(DataMgr._data.DecomposeTimes, max);
            }

	        UpdateItemsContainer(data);
            _view.TitleSprite_UISprite.spriteName = DataMgr._data.CurCompositTab == CompositeTabType.Composite
                ? "ModuleTitle_compose"
                : "ModuleTitle_Decompose";
            //_view.titleLabel_UILabel.text = TabInfoArr[(int)DataMgr._data.CurCompositTab].Name;
            switch (data.CurCompositTab)
            {
                case CompositeTabType.Composite:
                    UpdateCompositePage(data);
                    break;

                case CompositeTabType.DeComposite:
                    UpdateDecPage(data);
                    break;
            }
        }


	    private void UpdateCompositePage(ICompositViewData data)
	    {
		    if (curMaterialItem == null
		        || curMaterialItem.Item == null)
		    {
			    materialCell.UpdateViewWithNull();
			    materialCell.SetCountTxt(0, "拥有数量:{0}");
			    materialCell.SetNameLabel(string.Empty);
			    _view.composeRateLbl_UILabel.text = string.Empty;
		    }
		    else
		    {
			    materialCell.UpdateView(curMaterialItem.Item);
			    materialCell.SetCountTxt(curMaterialItem.Cnt, "拥有数量:{0}");
			    var colStr = ItemHelper.GetItemNameColorByID(curMaterialItem.Item.id);
			    materialCell.SetNameLabel(curMaterialItem.Item.name, colStr);
			    
			    var comProps = DataCache.getDtoByCls<CompositeProps>(curMaterialItem.Item.id);
			    if (comProps == null)
			    {
				    _view.composeRateLbl_UILabel.text = string.Empty;
			    }
			    else
			    {
				    _view.composeRateLbl_UILabel.text = string.Format("{0}合{1}", comProps.consumeCount, comProps.gainCount);   
			    }   
		    }

		    if (outComItem == null
		        || outComItem.Item == null)
		    {
			    outcomeCell.UpdateViewWithNull();
			    outcomeCell.SetCountTxt(0, "可得:{0}");
                outcomeCell.SetNameLabel(string.Empty);
		    }
		    else
		    {
                outcomeCell.UpdateView(outComItem.Item);
			    outcomeCell.SetCountTxt(outComItem.Cnt, "可得:{0}");
			    var colStr = ItemHelper.GetItemNameColorByID(outComItem.Item.id);
			    outcomeCell.SetNameLabel(outComItem.Item.name, colStr);
		    }

		    UpdateComposePrice(data);
		    
		    var max = GetComposeMaxTime();
            composePageTurn.SetPageInfo(DataMgr._data.CompositeTimes, max);
            //composePageTurn.UpdateView(DataMgr._data.CompositeTimes - 1, max + 1);
        }

	    private void UpdateComposePrice(ICompositViewData data)
	    {
		    var ty = AppVirtualItem.VirtualItemEnum.NONE;
		    var cnt = 0;
		    if (curMaterialItem != null
		        && curMaterialItem.Item != null){
                				    
                var props = DataCache.getDtoByCls<CompositeProps>(curMaterialItem.Item.id);

                if (props != null)
                {
                    cnt = data.CompositeTimes * props.compositePrice;
                    ty = (AppVirtualItem.VirtualItemEnum) props.virtualItemId;
                }
		    }
		    _view.ConsumeContent_UILabel.SetAppVirtualItemIconAndNum(
			    ty
			    , cnt
			    , "合成消耗：");
	    }


	    private void UpdateItemsContainer(ICompositViewData data)
	    {
		    int len = 0;
            IEnumerable<object> items = null;
		    int curPage = 0;
		    int maxPage = 0;
		    int selIdx = 0;
	        switch (data.CurCompositTab)
	        {
	            case CompositeTabType.Composite:
                    items = data.GetCompositeItems(out len).Map<BagItemDto, object>(s => s as object);
                    curPage = curComposePage;
                    maxPage = (int)Math.Floor((double)(len + ItemsContainerConst.PageCapability) / ItemsContainerConst.PageCapability);
                    selIdx = DataMgr._data.CompositeSelIdx;
	                break;

                case CompositeTabType.DeComposite:
                    items = data.GetDeComposeItems(data.CurDecomposeTab
                    , out len).Map<BagItemDto, object>(s => s as object);
                    curPage = curDecomposePage;
                    maxPage = (int)Math.Floor((double)(len + ItemsContainerConst.PageCapability) / ItemsContainerConst.PageCapability);
                    selIdx = DataMgr._data.DecomposeSelIdx;
                    break;
	        }
			    
		    itemsContainer.UpdateView(
			    curPage
			    , maxPage
			    , len
			    , items
			    , false
                , updateFinish:()=>itemsContainer.SetSelect(selIdx));
            itemsContainer.UpdateTabBtns(DataMgr._data.Composite_itemsPageTab, 0);
        }

	    private void CloseBtn_UIButtonClickHandler()
        {
	        DataMgr._data.ClearCompositeViewData();
	        DataMgr._data.ClearDecomposeViewData();
	        UIModuleManager.Instance.CloseModule(PropsCompositeView.NAME);
        }
	    
        private void ComposeBtn_UIButtonClickHandler()
        {
	        //todo fish:error code
	        if (curMaterialItem == null)
	        {
                TipManager.AddTip("请选择要合成的材料");
		        return;
	        }

	        var data = DataMgr._data;
	        
	        BackPackNetMsg.ReqComposeItem(
		        curMaterialItem.Item.id
	        	, DataMgr._data.CompositeTimes
	        	, curMaterialItem.CirType
	        	, data.isConvinienceComposite);
        }

	    private void ComposeTipBtn_UIButtonClickHandler()
	    {
            ProxyTips.OpenTextTips(9, new UnityEngine.Vector3(135, 156));
	    }
    }
}
