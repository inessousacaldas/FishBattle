// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 3/2/2018 4:07:45 PM
// **********************************************************************
using UnityEngine;
using AppDto;
using GamePlayer;
using UniRx;
public sealed partial class RedPackDataMgr
{
    
    public static partial class RedPackViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(RedPackChannelType type = RedPackChannelType.World)
        {
            DataMgr._data.curTab = type;
        // open的参数根据需求自己调整
        
	    var layer = UILayerType.DefaultModule;
            var ctrl = RedPackViewController.Show<RedPackViewController>(
                RedPackView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IRedPackViewController ctrl)
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
            _disposable.Add(ctrl.Onbutton_key_UIButtonClick.Subscribe(_=>button_key_UIButtonClick()));
            _disposable.Add(ctrl.Onbutton_common_UIButtonClick.Subscribe(_=>button_Common_UIButtonClick()));
            //_disposable.Add(ctrl.OnPage_guild_UIButtonClick.Subscribe(_=>Page_guild_UIButtonClick()));
            //_disposable.Add(ctrl.OnPage_world_UIButtonClick.Subscribe(_=>Page_world_UIButtonClick()));
//            _disposable.Add(ctrl.OnRedPackItem_UIButtonClick.Subscribe(_ => RedPackItem_UIButtonClick()));
            _disposable.Add(ctrl.TabStream.Subscribe(pageindex =>
            {
                DataMgr._data.curTabMainView = RedPackChannelType.World;
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
            UIModuleManager.Instance.CloseModule(RedPackView.NAME);
        }
        private static void button_key_UIButtonClick()
        {
            ProxyRedPack.OpenRedPackSendView();
            DataMgr._data.RedPackType = RedPack.RedPackType.key;
        }
        private static void button_Common_UIButtonClick()
        {
            //var delegateSendViewCtrl = RedPackSendViewController.Show<RedPackSendViewController>(RedPackSendView.NAME, UILayerType.ThreeModule, true, false);
            ProxyRedPack.OpenRedPackSendView();
            DataMgr._data.RedPackType = RedPack.RedPackType.Common;
        }
        private static void Page_guild_UIButtonClick()
        {

        }
        private static void Page_world_UIButtonClick()
        {
        }
        private static void RedPackItem_UIButtonClick()
        {
            if (!DataMgr.IsRedPackGet())//是否打开
            {
                if (DataMgr._data.RedPackType == RedPack.RedPackType.key)//口令
                {
                    //弹出界面
                    //输入口令
                    //if(DataMgr.InputWord == input.label.text)
                    if (DataMgr._data.IsEnough.enough == true)//是否足够
                    {
                        RedPackNetMsg.ReceiveRedPack(DataMgr._data.redPackId);
                        TipManager.AddTopTip("已领取");
                    }
                    else
                    {
                        TipManager.AddTopTip("手慢了，红包已被抢光！");
                    }
                }
                if (DataMgr._data.RedPackType == RedPack.RedPackType.Common)//普通
                {
                    //直接领取
                    if (DataMgr._data.IsEnough.enough == true)
                    {
                        RedPackNetMsg.ReceiveRedPack(DataMgr._data.redPackId);
                        TipManager.AddTopTip("已领取");
                    }
                    else
                    {
                        TipManager.AddTopTip("手慢了，红包已被抢光！");
                    }
                }
            }
            else
            {
                RedPackNetMsg.RedPackDetail(DataMgr._data.redPackId);
                var list = DataMgr._data.GetShowDetail;
            }
        }
    }
}

