// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 8/29/2017 7:33:42 PM
// **********************************************************************

using UniRx;
using System.Collections.Generic;
using AppDto;

public sealed partial class MissionDataMgr
{
    
    public static partial class NpcDialogueViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(BaseNpcInfo npcinfo)
        {
            if(npcinfo.npcStateDto==null)
            {
                return;
            }
        // open的参数根据需求自己调整
            var ctrl = NpcDialogueViewController.Show<NpcDialogueViewController>(
                NpcDialogueView.NAME
                , UILayerType.Dialogue
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
            ProxyMainUI.Hide();
            ctrl.Open(npcinfo,DataMgr._data);
        }

        /// <summary>
        /// 打开自定义面板
        /// </summary>
        /// <param name="ctrl"></param>
        public static void OpenCustomPanel(Npc npcinfo,string content,List<string> optionList,System.Action<int> onSelect)
        {
            if(npcinfo == null)
            {
                return;
            }
            // open的参数根据需求自己调整
            var ctrl = NpcDialogueViewController.Show<NpcDialogueViewController>(
                NpcDialogueView.NAME
                , UILayerType.Dialogue
                , true
                , true
                , Stream);
            ProxyMainUI.Hide();
            InitReactiveEvents(ctrl);
            //ProxyMainUI.Hide();
            ctrl.OpenCommonDialogueByNpc(npcinfo,content,optionList,onSelect);
        }
        
        private static void InitReactiveEvents(INpcDialogueViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnClickBtnBoxCollider_UIButtonClick.Subscribe(_=>ClickBtnBoxCollider_UIButtonClick()));
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
        private static void ClickBtnBoxCollider_UIButtonClick()
        {
            //UIModuleManager.Instance.CloseModule(NpcDialogueView.NAME);
        }


        public static void Close()
        {
            if(UIModuleManager.Instance.IsModuleCacheContainsModule(NpcDialogueView.NAME)) {
                UIModuleManager.Instance.CloseModule(NpcDialogueView.NAME);
                ProxyMainUI.Show();
            }
        }
    }
}

