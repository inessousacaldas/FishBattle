// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatSettingPanelController.cs
// Author   : Cilu
// Created  : 11/8/2017 5:48:53 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;
using AssetPipeline;
using LITJson;


public partial interface IChatSettingPanelController
{

}
public partial class ChatSettingPanelController    {
    public enum ChatSettingEnum
    {
        ShowChannel,
        AutoPlayVoice,
    }
    CompositeDisposable _mydisposable = new CompositeDisposable();
    public static IChatSettingPanelController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IChatSettingPanelController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IChatSettingPanelController;
        
        return controller;        
    }



    ChatSettingEnum curSelect = ChatSettingEnum.ShowChannel;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _mydisposable.Clear();
        var data = ChatDataMgr.Stream.LastValue.chatSettingData;
        var prefab = ResourcePoolManager.Instance.LoadUI("ChatSettingTg");
        ChatSettingData.ShowChannelList.ForEachI((x, i) => {
            var go = NGUITools.AddChild(View.ShowChannelSetting_UIGrid.gameObject, prefab);
            var tg = go.GetComponent<UIToggle>();
            var tgevt = tg.OnValueChangedAsObservable();
            _mydisposable.Add(tgevt.Subscribe(b => { OnTgChange_Show(x, b); }));
            bool value;
            var label = tg.gameObject.FindScript<UILabel>("Label");
            label.text = ChatHelper.GetChannelName(x) +"频道";
            data.channelShowDic.TryGetValue(x.ToString(), out value);
            tg.Set(value, false);
        });

        ChatSettingData.ChannelAutoPlayList.ForEachI((x, i) =>
        {
            var go = NGUITools.AddChild(View.AutoPlayerVoiceSetting_UIGrid.gameObject, prefab);
            var tg = go.GetComponent<UIToggle>();
            var tgevt = tg.OnValueChangedAsObservable();
            _mydisposable.Add(tgevt.Subscribe(b => { OnTgChange_Voice(x, b); }));
            bool value;
            var label = tg.gameObject.FindScript<UILabel>("Label");
            label.text = ChatHelper.GetChannelName(x) + "频道";
            data.channelAutoPlayVoice.TryGetValue(x.ToString(), out value);
            tg.Set(value, false);
        });
        UpdateView();
    }

    protected override void RegistCustomEvent()
    {
        base.RegistCustomEvent();
        OnAutoPlayerVoiceBtn_UIButtonClick.Subscribe(_=> {
            curSelect = ChatSettingEnum.AutoPlayVoice;
            UpdateView();
        });
        OnShowChannelBtn_UIButtonClick.Subscribe(_=> {
            curSelect = ChatSettingEnum.ShowChannel;
            UpdateView();
        });
    }
    protected override void RemoveCustomEvent()
    {
        base.RemoveCustomEvent();
    }
    private void OnTgChange_Show(ChatChannel.ChatChannelEnum ChannelId,bool state)
    {
        var data = ChatDataMgr.Stream.LastValue.chatSettingData;
        data.channelShowDic.AddOrReplace(ChannelId.ToString(), state);
        TipManager.AddTip(state ? "设置成功" : "取消成功");
    }
    private void OnTgChange_Voice(ChatChannel.ChatChannelEnum ChannelId, bool state)
    {
        var data = ChatDataMgr.Stream.LastValue.chatSettingData;
        data.channelAutoPlayVoice.AddOrReplace(ChannelId.ToString(), state);
        TipManager.AddTip(state ? "设置成功" : "取消成功");
    }

    public void UpdateView()
    {
        switch(curSelect)
        {
            case ChatSettingEnum.ShowChannel:
                View.AutoPlayerVoiceSetting_UIGrid.gameObject.SetActive(false);
                View.ShowChannelSetting_UIGrid.gameObject.SetActive(true);
                View.Tips_UILabel.text = "勾选的频道消息将会显示在主界面上";
                break;
            case ChatSettingEnum.AutoPlayVoice:
                View.AutoPlayerVoiceSetting_UIGrid.gameObject.SetActive(true);
                View.ShowChannelSetting_UIGrid.gameObject.SetActive(false);
                View.Tips_UILabel.text = "勾选的频道语音将会在收到后自动播放";
                break;
        }
    }
    protected override void OnDispose()
    {
        base.OnDispose();
        _mydisposable = _mydisposable.CloseOnceNull();
        var data = ChatDataMgr.Stream.LastValue.chatSettingData;
        ChatSettingData.SaveData(data);
    }

}
