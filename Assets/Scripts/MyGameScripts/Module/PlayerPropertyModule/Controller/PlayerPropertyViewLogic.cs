// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 8/28/2017 11:18:56 AM
// **********************************************************************

using UniRx;
using System.Collections.Generic;
using AppDto;
using System;
using MyGameScripts.Gameplay.Player;
using UnityEngine;

public sealed partial class PlayerPropertyDataMgr
{
    
    public static partial class PlayerPropertyViewLogic
    {
        private static CompositeDisposable _disposable;
        public static Comparison<CharacterAbility> _comparison = null;

        public static void Open()
        {
            //功能是否开启
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_1, true)) return;

            // open的参数根据需求自己调整
            var ctrl = PlayerPropertyViewController.Show<PlayerPropertyViewController>(
                PlayerPropertyView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IPlayerPropertyViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseButton_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));
            _disposable.Add(ctrl.BasePropertyEvtClick.Subscribe(_ => BasePropertyContainer_UIbuttonClickHandler()));
            _disposable.Add(ctrl.FightPropertyEvtClick.Subscribe(_ => FightPropertyContainer_UIbuttonClickHandler()));
            _disposable.Add(ctrl.OnAdvancedPropertyBtn_UIButtonClick.Subscribe(_ => AdvancedProperty_UIbuttonClickHandler()));
            _disposable.Add(ctrl.OnNoteBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(3, new Vector3(-132, -242), true, ModelManager.Player.ServerGrade.ToString(), ModelManager.Player.GetServerGradeUpLimit().ToString());
            }));
            _disposable.Add(ctrl.OnChangeNameBtn_UIButtonClick.Subscribe(_ =>
            {
                var renameCtrl = PlayerChangeNameController.Show<PlayerChangeNameController>(PlayerChangeName.NAME, UILayerType.ThreeModule, true, true);

                _disposable.Add(renameCtrl.OnClickRenameStream.Subscribe(name =>
                {
                    PlayerPropertyNetMsg.ReqPlayerChangeName(name);
                }));
            })); 

            _disposable.Add(ctrl.OnAppellationBtn_UIButtonClick.Subscribe(_ =>
            {
                TipManager.AddTip("改称号");
            }));

            _disposable.Add(ctrl.TabBtnMgr.Stream.Subscribe(i =>
            {
                if (i == (int)PlayerViewTab.InfoView)
                {
                    TipManager.AddTip("功能未开放");
                    FireData();
                    return;
                } 
                SetCurTab((PlayerViewTab)PlayerPropertyData._TabInfos[i].EnumValue);
                FireData();
            }));

            //ctrl.OnChildCtrlAdd += Ctrl_OnChildCtrlAdd;

            //请求数据
            PlayerPropertyNetMsg.ReqPlayerPropsInfo();
        }

        private static void SetCurTab(PlayerViewTab i)
        {
            DataMgr._data.CurTab = PlayerViewTab.PropertyView;

            FireData();
        }

        //private static void Ctrl_OnChildCtrlAdd(PlayerViewTab tab, IMonolessViewController ctrl)
        //{
        //    switch (tab)
        //    {
        //        case PlayerViewTab.PropertyView:
        //            InitReactiveEvents((IPlayerPropertyViewController)ctrl);
        //            break;
        //        case PlayerViewTab.SkillView:
        //            InitReactiveEvents((IPlayerSkillViewController)ctrl);
        //            break;
        //    }
        //}

        //private static void InitReactiveEvents(IPlayerPropertyViewController ctrl)
        //{

        //}

        //private static void InitReactiveEvents(IPlayerSkillViewController ctrl)
        //{

        //}

        private static void CloseBtn_UIButtonClickHandler()
        {
            ProxyPlayerProperty.ClosePlayerPropertyModule();
        }

        private static void BasePropertyContainer_UIbuttonClickHandler()
        {
            //var ctrl = PropertyTipController.Show<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true, true);
            var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
            ctrl.Init(PlayerPropertyTipType.BaseType, GlobalAttr.PANTNER_BASE_ATTRS);
            ctrl.AddMagicprops(ModelManager.Player.GetSlotsElementLimit,"", ModelManager.Player.FactionID);
            ctrl.SetPostion(new Vector3(-238, -42, 0));
        }

        private static void FightPropertyContainer_UIbuttonClickHandler()
        {
            var ctrl = UIModuleManager.Instance.OpenFunModule<PropertyTipController>(PropertyTip.NAME, UILayerType.SubModule, true);
            ctrl.Init(PlayerPropertyTipType.FightType, GlobalAttr.SECOND_ATTRS_TIPS);
            ctrl.SetPostion(new Vector3(-238, -42, 0));
        }

        private static List<CharacterAbility> DictSort(List<CharacterAbility> dict)
        {
            if (_comparison == null)
            {
                _comparison = (a, b) =>
                {
                    if (a.type == b.type)
                        return a.typeSort.CompareTo(b.typeSort);
                    else
                        return a.type.CompareTo(b.type);
                };
            }
            dict.Sort(_comparison);
            return dict;
        }

        private static void AdvancedProperty_UIbuttonClickHandler()
        {
            var controller = UIModuleManager.Instance.OpenFunModule<DetailInfoController>(DetailInfoView.NAME, UILayerType.SubModule, true);

            //controller.SetTitleInfo("高级属性");
            List<CharacterAbility> characterAbility = DataCache.getArrayByCls<CharacterAbility>();
            //List<CharacterPropertyDto> properties = DataCache.getArrayByCls<CharacterPropertyDto>();
            List<CharacterAbility> list = new List<CharacterAbility>();
            characterAbility.ForEach(data =>
            {
                if (data.typeSort > 0)   //类型排序typeSort 大于0
                    list.Add(data);
            });
            DictSort(list);

            controller.SetCrewDetailInfo(list);
            controller.SetPosition(new Vector3(-239, -43, 0));
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
    }
}

