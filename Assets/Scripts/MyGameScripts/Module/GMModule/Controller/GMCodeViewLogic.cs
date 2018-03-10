// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// **********************************************************************

using AppDto;
using AppServices;
using AssetPipeline;
using UniRx;
using UnityEngine;

public sealed partial class GMDataMgr
{

    public static partial class GMCodeViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(GMCodeViewController ctrl)
        {
            if(ctrl == null) return;
            if(_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }

            ctrl.TabMgr.Stream.Subscribe(e =>
            {
                ctrl.OnTabChange(e,DataMgr._data);
                DataMgr._data.codeTab = e;
            });
            ctrl.TabMgr.SetTabBtn(DataMgr._data.codeTab);
            ctrl.OnTabChange(DataMgr._data.codeTab,DataMgr._data);
            _disposable.Add(ctrl.OnbtnSend_UIButtonClick.Subscribe(_ => btnSend_UIButtonClick(ctrl)));
        }

        public static void InitReactiveEvents(GMCodeViewController ctrlParent,GMCodeItemController ctrl)
        {
            _disposable.Add(ctrl.OnbtnEnter_UIButtonClick.Subscribe(_ => btnEnter_UIButtonClick(ctrlParent,ctrl)));
        }

        private static void btnSend_UIButtonClick(GMCodeViewController ctrl)
        {
            var msg = ctrl.View.txtInput_UIInput.value;
            if(string.IsNullOrEmpty(msg) == false)
            {
                if(msg.IndexOf("#") != -1)
                {
                    GMNetMsg.OnGMExecute(DataMgr._data.curCodeVO,msg);
                }
                else
                {
                    TipManager.AddTip(string.Format("发送了一个不规范的指令:{0}",msg));
                }
            }
        }

        private static void btnEnter_UIButtonClick(GMCodeViewController ctrlParent,GMCodeItemController ctrl)
        {
            DataMgr._data.curCodeVO = ctrl.vo;
            if(DataMgr._data.IsGMCode(ctrl.vo.cfgVO.code) == false && string.IsNullOrEmpty(ctrl.vo.cfgVO.parm))
            {
                var method = typeof(GMCodeViewLogic).GetMethod(ctrl.vo.cfgVO.code,System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Static);
                if(method != null)
                {
                    TipManager.AddTip(string.Format("发送一个客户端GM指令:{0}",ctrl.vo.cfgVO.label));
                    var paramList = method.GetParameters();
                    if(paramList.Length ==  1)
                    {//仅支持单参数
                        var msg = ctrlParent.View.txtInput_UIInput.value;
                        method.Invoke(null,new object[] { msg });
                    }else if(paramList.Length == 0)
                    {
                        method.Invoke(null,null);
                    }
                    else
                    {
                        TipManager.AddTip(string.Format("方法：{0}的参数列超过1个，目前不支持，请检查下该方法",ctrl.vo.cfgVO.code));
                    }
                }
                else
                {
                    TipManager.AddTip(string.Format("指令：{0}，在客户端没有对应的方法，请检查下：{1}方法是否正确:",ctrl.vo.cfgVO.label,ctrl.vo.cfgVO.code));
                }
            }
            FireData();
        }

        public static void Dispose()
        {
            DataMgr._data.codeTab = 0;
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }

        private static void CloseView()
        {
            GMViewLogic.CloseView();
        }

        private static void OnDebugBtnClick()
        {
            GameDebuger.debugIsOn = !GameDebuger.debugIsOn;
            if(GameDebuger.debugIsOn)
            {
                GameDebugConsole.Setup();
            }
            else
            {
                GameDebugConsole.Dispose();
            }
            ProxyGMTestModule.Close();
        }

        private static void OnServerRequestCheckSwitch()
        {
            ServiceRequestAction.ServerRequestCheck = !ServiceRequestAction.ServerRequestCheck;
            TipManager.AddTip("预判开关:" + ServiceRequestAction.ServerRequestCheck);
        }

        private static void OnServerRequestDelaySwitch()
        {
            ServiceRequestAction.ServerRequestDelay = !ServiceRequestAction.ServerRequestDelay;
            TipManager.AddTip("延迟开关:" + ServiceRequestAction.ServerRequestDelay);
        }

        private static void OnServerRequestLockSwitch()
        {
            ServiceRequestAction.ServerRequestLock = !ServiceRequestAction.ServerRequestLock;
            TipManager.AddTip("锁请求开关:" + ServiceRequestAction.ServerRequestLock);
        }

        private static void OnResourcesLoadDelaySwitch()
        {
#if UNITY_EDITOR
            AssetManager.EditorLoadDelay = AssetManager.EditorLoadDelay == 0f ? 1f : 0f;
            TipManager.AddTip("加载延迟开关:" + AssetManager.EditorLoadDelay);
#endif
        }

        private static void OnShowMeTheExp()
        {
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_main_char_exp 1000000"));
            CloseView();
        }

        private static void OnShowMeTheMoney()
        {
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_ingot 1000000"));
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_silver 1000000"));
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_copper 50000000"));
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_score 100000"));
            ServiceRequestAction.requestServer(Services.Gm_Execute("#add_contribute 100000"));

            CloseView();
        }

        private static void OnTestSocketClose()
        {
            SocketManager.Instance.Close(false);
            CloseView();
        }

        private static void OnReLogin()
        {
            ExitGameScript.Instance.HanderRelogin();
            CloseView();
        }

        private static void OnMapTestClick()
        {
            ProxyGMTestModule.Close();
            ProxyWorldMapModule.OpenWorldMap();
        }

        private static void OnSpeedUp()
        {
            if(Time.timeScale == 1f)
            {
                Time.timeScale = 3f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            CloseView();
        }

        private static void OnSpeedUp100()
        {
            if(Time.timeScale == 1f)
            {
                Time.timeScale = 100f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            CloseView();
        }

        private static void OnSpeedDown()
        {
            if(Time.timeScale == 1f)
            {
                Time.timeScale = 0.3f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            CloseView();
        }

        private static void OnPetActionClick()
        {
            ProxyGMTestModule.Close();
            ProxyAnimatorTestModule.Open();
        }

        private static void OnReloadBattleConfig()
        {
            BattleConfigManager.Instance.Setup();
        }

        private static void OnBattleTestClick()
        {
            ProxyGMTestModule.Close();
            //        ProxyBattleDemo.Open();
            ProxyBattleDemoConfigModule.Open();
        }

        private static void OnBattleShowClick()
        {
            ProxyGMTestModule.Close();
            ProxyBattleDemoConfigModule.OpenEasyConfig();
        }

        private static void OnBattleSkillPreviewClick(string inputValue)
        {
            int pSkillId = inputValue.ToInt();
            if(pSkillId <= 0)
            {
                TipManager.AddTip("请输入要演示的技能ID！");
                return;
            }
            DemoSimulateHelper.SimulatePreview(pSkillId,CloseView);
        }

        private static void OnBattleSkillEditorClick()
        {
            CloseView();
            ProxyBattleSkillEditor.Open(ProxyBattleSkillEditorPreview.OpenPreview,ProxyBattleSkillEditorPreview.ReplayPreview,ProxyBattleSkillEditorPreview.ClosePreview);
        }

        private static void TestTipEnable()
        {
            BattleDataManager.DataMgr.BattleDemo.ShowTip = !BattleDataManager.DataMgr.BattleDemo.ShowTip;
            TipManager.AddTip(string.Format("当前是否弹窗显示重要提示:{0}",BattleDataManager.DataMgr.BattleDemo.ShowTip));
        }

        private static void TestCompleteRound()
        {
            DemoSimulateHelper.SimulateRoundStart();
            CloseView();
        }
    }
}


