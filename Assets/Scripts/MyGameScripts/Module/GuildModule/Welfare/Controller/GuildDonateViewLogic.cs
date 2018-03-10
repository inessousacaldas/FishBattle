// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 3/7/2018 2:48:22 PM
// **********************************************************************

using UniRx;

public sealed partial class GuildMainDataMgr
{
    
    public static partial class GuildDonateViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = GuildDonateViewController.Show<GuildDonateViewController>(
                GuildDonateView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IGuildDonateViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OncloseBtn_UIButtonClick.Subscribe(_ => closeBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnminusBtn_UIButtonClick.Subscribe(_=>minusBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OnaddBtn_UIButtonClick.Subscribe(_=>addBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OndonateBtn_UIButtonClick.Subscribe(_=>donateBtn_UIButtonClick(ctrl)));
           
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }
        private static void closeBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(GuildDonateView.NAME);
        }
        private static void minusBtn_UIButtonClick(IGuildDonateViewController ctrl)
        {
            ctrl.OnMinusBtnClick();
        }
        private static void addBtn_UIButtonClick(IGuildDonateViewController ctrl)
        {
            ctrl.OnAddBtnClick();
        }
        private static void donateBtn_UIButtonClick(IGuildDonateViewController ctrl)
        {
            if (DataMgr._data.DonateCount > 5)
            {
                TipManager.AddTip("每天只可以捐赠5次物资，明天再来吧！");
                return;
            }
            var item = ctrl.SelBagItem;
            if (item == null) return;
            var uniqueId = item.uniqueId;
            //uniqueId == 0 可叠加
            if (uniqueId == 0)
                GuildMainNetMsg.ReqDonateByItemId(item.itemId,ctrl.DonateCount);
            else
                GuildMainNetMsg.ReqDonateByUniqueId(item.uniqueId);
        }

    
    }
}

