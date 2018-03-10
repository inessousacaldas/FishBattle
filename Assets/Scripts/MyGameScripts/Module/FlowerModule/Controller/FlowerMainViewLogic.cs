// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/13/2018 10:13:30 AM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class FlowerMainViewDataMgr
{
    
    public static partial class FlowerMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(FriendInfoDto dto=null, int flowerId=0)
        {

	    //var id = DataCache.getDtoByCls<FunctionOpen>(FlowerMainView.NAME);
	    //if(FunctionOpenHelper.isFuncOpen(id, true))
	    //    return;

        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = FlowerMainViewController.Show<FlowerMainViewController>(
                FlowerMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
            DataMgr._data.AddAndSort(dto);
            DataMgr._data.CurFlowerId = flowerId;  //这里0也设置 刷新界面那里会处理
            DataMgr._data.CurFlowerCount = 1;
        }
        
        private static void InitReactiveEvents(IFlowerMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnSearchBtn_UIButtonClick.Subscribe(_=>SearchBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OnGiveBtn_UIButtonClick.Subscribe(_ => GiveBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnMaxBtn_UIButtonClick.Subscribe(_ => MaxBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OnSelectCountStream.Subscribe(i =>
            {
                DataMgr._data.CurFlowerCount = i;
                FireData();
            }));
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
        private static void CloseBtn_UIButtonClick()
        {
            ProxyFlowerMainView.Close();
        }
        private static void SearchBtn_UIButtonClick(IFlowerMainViewController ctrl)
        {
            FlowerMainViewNetMsg.ReqSearchFriend(ctrl.InputFiledText);
        }
        private static void GiveBtn_UIButtonClick()
        {
            var dto = DataMgr._data.SearchList.Find(x => x.friendId == DataMgr._data.CurFriendId);
            if(dto == null)
            {
                TipManager.AddTip("请选择一个目标再进行赠送鲜花哦！");
                return;
            }
            var name = dto.name;
            if(DataMgr._data.CurFlowerCount > BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(DataMgr._data.CurFlowerId))
            {
                var itemData = ItemHelper.GetGeneralItemByItemId(DataMgr._data.CurFlowerId);
                TipManager.AddTip(string.Format("身上的{0}数量不足", itemData == null ? "鲜花" : itemData.name));
                GainWayTipsViewController.OpenGainWayTip(DataMgr._data.CurFlowerId, new UnityEngine.Vector3(164, -46));
                return;
            }
            FlowerMainViewNetMsg.ReqGiveFlower(DataMgr._data.CurFriendId, name, DataMgr._data.CurFlowerId, 
                DataMgr._data.CurFlowerCount, DataMgr._data.CurFlowerContent);
        }

        private static void MaxBtn_UIButtonClick(IFlowerMainViewController ctrl)
        {
            DataMgr._data.CurFlowerCount = BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(DataMgr._data.CurFlowerId) > 9999 ? 9999 
                : BackpackDataMgr.DataMgr.GetBackpackItemCountByItemID(DataMgr._data.CurFlowerId);
            ctrl.PtCtrl.SetPageInfo(DataMgr._data.CurFlowerCount, 9999);
            FireData();
        }
    }
}

