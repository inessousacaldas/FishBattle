// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/13/2017 6:07:28 PM
// **********************************************************************

using UniRx;

public sealed partial class SearchFriendDataMgr
{
    
    public static partial class SearchFriendViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = SearchFriendViewController.Show<SearchFriendViewController>(
                SearchFriendView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ISearchFriendViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            //_disposable.Add(ctrl.OnCancleInputBtn_UIButtonClick.Subscribe(_=>CancleInputBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnSearchBtn_UIButtonClick.Subscribe(_=>SearchBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OnChangeBatchBtn_UIButtonClick.Subscribe(_=>ChangeBatchBtn_UIButtonClick(ctrl)));
            _disposable.Add(ctrl.OnMapBtn_UIButtonClick.Subscribe(_=>MapBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnCloseButton_UIButtonClick.Subscribe(_=>CloseButton_UIButtonClick()));
            _disposable.Add(ctrl.OnSearchStream.Subscribe(id =>
            {
                SearchFriendNetMsg.ReqAddFriend(id);
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

        private static void CancleInputBtn_UIButtonClick()
        {
            FireData();
        }

        private static void SearchBtn_UIButtonClick(ISearchFriendViewController ctrl)
        {
            if (string.IsNullOrEmpty(ctrl.InputFiledText))
            {
                TipManager.AddTip("名字不能为空");
                return;
            }

            if (DataMgr._data.CanSearch)
            {
                DataMgr._data.CanSearch = false;

                SearchFriendNetMsg.ReqSearchFriend(ctrl.InputFiledText);
            }
            else
            {
                TipManager.AddTip("搜索过于频繁");
            }
        }

        //换一批
        private static void ChangeBatchBtn_UIButtonClick(ISearchFriendViewController ctrl)
        {
            if (DataMgr._data.CanChange)
            {
                SearchFriendNetMsg.ReqChange();
            }
            else
            {
                int cdTime = (int)((DataMgr._data.ChangeCDTime - SystemTimeManager.Instance.GetUTCTimeStamp()) / 1000);
                cdTime = cdTime > 1 ? cdTime : 1;
                TipManager.AddTip(string.Format("{0}秒才能换一批", cdTime));
            }
        }

        private static void MapBtn_UIButtonClick()
        {
            TipManager.AddTip("暂未开放");
        }

        private static void CloseButton_UIButtonClick()
        {
            FriendDataMgr.DataMgr.SetDefaultTabView();
            ProxyFriend.CloseSearchFriendView();
        }
    }
}

