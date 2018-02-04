// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatChannelToggleViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public sealed partial class ChatDataMgr
{
    public partial class ChatChannelToggleViewController    {


	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
            Init();
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

        private void CloseBtn_UIButtonClickHandler()
        {
            ProxyChat.CloseChatChannelView();
        }
        private void ConfirmBtn_UIButtonClickHandler()
        {
            ProxyChat.CloseChatChannelView();
        }

        private List<GenericCheckBoxController> channelToggleTopCtlList;
        private List<GenericCheckBoxController> channelToggleBottomCtlList;
        //初始化聊天频道设置面板
        private void Init()
        {
            //todo fish:要么一行行写，要么建立一个映射
            channelToggleTopCtlList = new List<GenericCheckBoxController>();
            string[] nameTopStr = new string[3] { "世界频道", "帮派频道", "队伍频道" };
            channelToggleTopCtlList = InitChannelToggle(_view.Grid_Top_UIGrid.gameObject, nameTopStr,  channelToggleTopCtlList);

            channelToggleBottomCtlList = new List<GenericCheckBoxController>();
            string[] nameBottomStr = new string[3] { "世界频道音频", "帮派频道音频", "队伍频道音频" };
            channelToggleBottomCtlList = InitChannelToggle(_view.Grid_Bottom_UIGrid.gameObject, nameBottomStr,  channelToggleBottomCtlList);
        }

        //初始化Toggle列表
        private List<GenericCheckBoxController> InitChannelToggle(GameObject parent, string[] nameArray
            , List<GenericCheckBoxController> channelToggleCtlList)
        {
            nameArray.ForEach(name =>
            {
                    var ctrl = AddChild<GenericCheckBoxController, GenericCheckBox>(
                    parent
                , GenericCheckBox.NAME);
                
              //  ctrl.UpdateView(GenericCheckBoxData.Create(name, false));

                channelToggleCtlList.Add(ctrl);
             //   _disposable.Add(ctrl.ClickStateHandler.Subscribe(isSelect=>SetChannelToggleState(isSelect)));
            });
            
            return channelToggleCtlList;
        }

        
        //给toggle添加事件监听
        private void SetChannelToggleState(bool isSelect)
        {
            
        }
    }
}
