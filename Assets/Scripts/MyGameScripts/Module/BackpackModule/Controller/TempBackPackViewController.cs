// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TempBackPackViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;

public sealed partial class BackpackDataMgr
{
    public partial class TempBackPackViewController
    {
        public static void Open()
        {
            DataMgr._data.curTempPageNum = 0;
            UIModuleManager.Instance.OpenFunModule<TempBackPackViewController>(
                TempBackPackView.NAME
                , UILayerType.DefaultModule
                , true
                , false);
        }

        private ItemsPageContainerController myPackContainer;
        private ItemContainerController tempPackContainer;
        private int selectedIdx = -1;
	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
            myPackContainer = AddChild<ItemsPageContainerController, ItemsPageContainer>(
                    View.MyBackAnchor
            , ItemsPageContainer.NAME
            , "MyBackpack"
            );
            
            _disposable.Add(myPackContainer.OnItemClick.Subscribe(i =>
            {
                if (i >= DataMgr._data.backPackDto.capability)
                    BackPackNetMsg.ExpandBag();
                //else
                //{
                //    var item = DataMgr._data.GetBagItemByIndex(i);
                //    if (item != null)
                //    {
                //        // 显示小菜单
                //    }
                //}
                myPackContainer.SetSelect(i);
            }));

            tempPackContainer = AddChild<ItemContainerController, ItemContainer>(
                View.TempBackAnchor
                , ItemContainer.NAME
                , "TempBackpack"
            );

            _disposable.Add(tempPackContainer.OnItemClick.Subscribe(i=>
                {
                    var tempbackDtoItem = DataMgr._data.GetTempBagItemByIndex(i);
                    if (tempbackDtoItem == null)
                        return;
                    selectedIdx = i;

                    tempPackContainer.SetSelect(selectedIdx);
                }));
        }

	    // 客户端自定义代码
	    protected override void RegistCustomEvent ()
        {
        
        }

        protected override void OnDispose()
        {
            selectedIdx = -1;
            DataMgr._data.curTempPageNum = 0;
            _disposable.Dispose();
        }

	    //在打开界面之前，初始化数据
	    protected override void InitData()
    	{
            selectedIdx = -1;
	        _disposable = new CompositeDisposable();
    	}

        private void UpdateDataAndView(IBackpackData data)
        {
            var viewData = data.TempBackpackViewData;
            var set = ItemHelper.TransBagDtoToObjectSet(
                viewData.GetBagItems()
                , viewData.ItemBagCapability);

            myPackContainer.UpdateView(
                DataMgr._data.curTempPageNum
                , viewData.PageNum
                , viewData.ItemBagCapability
                , set
                , true);
            
            tempPackContainer.UpdateView(
                    data.TempBackpackViewData.GetTempBagItems()
                    , ItemsContainerConst.PageCapability
                    );
        }

        private void TransBtn_UIButtonClickHandler()
        {
            BackPackNetMsg.TransItemFromTempBagToBack(selectedIdx, ClearSelect);
        }

        private void CloseBtn_UIButtonClickHandler()
        {
            selectedIdx = -1;
            UIModuleManager.Instance.CloseModule(TempBackPackView.NAME);
            DataMgr._data.AddedTaskItems.Clear();
        }
        private void TransAllBtn_UIButtonClickHandler()
        {
            BackPackNetMsg.TransAllItemFromTempBagToBack(ClearSelect);
        }
        private void ArrangeBtn_UIButtonClickHandler()
        {
            BackPackNetMsg.SortBagItems(ClearSelect);
        }

        private void ClearSelect()
        {
            selectedIdx = -1;
            tempPackContainer.SetSelect(selectedIdx);
        }

        private void DecomposeBtn_UIButtonClickHandler()
        {
            //TipManager.AddTip("敬请期待!");
            //return;
            var dto = DataMgr._data.backPackDto.items.Find(s=>s.index == DataMgr._data.CurBackPackSelectIdx);
            ProxyBackpack.OpenComposite(CompositeTabType.DeComposite, dto);
        }
        private void Composite_UIButtonClickHandler()
        {
            //TipManager.AddTip("敬请期待!");
            //return;
            var dto = DataMgr._data.backPackDto.items.Find(s=>s.index == DataMgr._data.CurBackPackSelectIdx);
            ProxyBackpack.OpenComposite(CompositeTabType.Composite, dto);
        }
        
    }
}
    