// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 3/6/2018 5:43:55 PM
// **********************************************************************

using UniRx;

public sealed partial class GuildMainDataMgr
{
    
    public static partial class GuildBoxViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = GuildBoxViewController.Show<GuildBoxViewController>(
                GuildBoxView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IGuildBoxViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnboxCloseBtn_UIButtonClick.Subscribe(_=>boxCloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnboxOpenBtn_UIButtonClick.Subscribe(_=>boxOpenBtn_UIButtonClick()));
           
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
        private static void boxCloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(GuildBoxView.NAME);
        }
        private static void boxOpenBtn_UIButtonClick()
        {
            if (!CheckOpenBox()) return;
            GuildMainNetMsg.ReqOepnBox();
        }

        private static bool CheckOpenBox()
        {
            var detail = DataMgr._data.GuildDetailInfo;
            if (detail == null) return false;
            var selfInfo = DataMgr._data.PlayerGuildInfo;
            if (selfInfo == null) return false;
            var selfPos = DataMgr._data.GuildPosition.Find(e => e.id == selfInfo.positionId);
            if (!selfPos.distributeGiftBox)
            {
                TipManager.AddTip("只有会长和副会长可以开启公会宝箱");
                return false;
            }
            var dto = DataMgr._data.GuildBox;
            if (dto == null || dto.hasPoint < dto.needPoint)
            {
                TipManager.AddTip("当前的公会宝箱积分不足以开启宝箱");
                return false;
            }
            return true;
        }

    }
}

