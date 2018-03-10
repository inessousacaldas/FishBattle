// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 11/14/2017 10:31:50 AM
// **********************************************************************

using UniRx;

public sealed partial class ShortCutMessageViewDataMgr
{
    
    public static partial class ShortCutMessageViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.ChatModule;
            var ctrl = ShortCutMessageViewController.Show<ShortCutMessageViewController>(
                ShortCutMessageView.NAME
                , layer
                , false
                , false
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IShortCutMessageViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnPlayerAnimBtn_UIButtonClick.Subscribe(_=>PlayerAnimBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnShotCutBtn_UIButtonClick.Subscribe(_=>ShotCutBtn_UIButtonClick()));


            _disposable.Add(ctrl.OnClickMsgStream.Subscribe(i=> {
                var msg = DataMgr._data.shortCutMessageList.Find(x => x.Id == i);
                if(msg != null)
                {
                    if (msg.Id == -1)
                    {
                        DataMgr._data.isEdit = true;
                    }                       
                    else
                    {
                        if (TeamDataMgr.DataMgr.HasTeam())
                        {
                            //发送组队消息
                            ChatDataMgr.ChatNetMsg.ReqSendChatMsg(AppDto.ChatChannel.ChatChannelEnum.Team, 0, false, msg.content, () =>
                            {

                            });
                        }
                        else
                        {
                            if (ModelManager.Player.GetPlayerLevel() >= 15)
                            {
                                ChatDataMgr.ChatNetMsg.ReqSendChatMsg(AppDto.ChatChannel.ChatChannelEnum.Current, 0, false, msg.content, () =>
                                {

                                });
                            }
                            else
                                TipManager.AddTip("当前发言需要人物等级15级");
                        }
                    }
                }
                FireData();
            })); //发送消息
            _disposable.Add(ctrl.OnClickDeleteStream.Subscribe(i=> { DataMgr._data.DelateShortCutMessage(i); }));
            _disposable.Add(ctrl.OnClickAddStream.Subscribe(_=> { }));
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
        private static void PlayerAnimBtn_UIButtonClick()
        {
        }
        private static void ShotCutBtn_UIButtonClick()
        {
        }

    
    }
}

