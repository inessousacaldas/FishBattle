#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 输入管理器，主要用于win的热键管理
/// </summary>
public class InputManager
{
    private static InputManager _instance;

    /// <summary>
    /// 仅存在于改列表的关闭模块能触发关闭模块
    /// </summary>
    public static readonly string[] CloseBgList;

    /// <summary>
    /// 临时用于检查点击的
    /// </summary>
    private static readonly UICamera.MouseOrTouch _tempTouch;

    /// <summary>
    /// 快捷键触发间隔
    /// </summary>
    public const float HotKeyInterval = 0.5f;
    /// <summary>
    /// 上一次触发快捷键的时间
    /// </summary>
    private float _hotKeyTriggerLastTime;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InputManager();
            }
            return _instance;
        }
    }

    static InputManager()
    {
        CloseBgList = new string[]
        {
            "ModuleBgBoxCollider(Clone)",
            "CloseBoxBG",
            "BgCollider",
            "ClickBtnBoxCollider",
        };

        _tempTouch = new UICamera.MouseOrTouch()
        {
            pos = Vector2.one,
        };
    }

    public void Setup()
    {
        UICamera.NotifyCallback += OnUICameraNotify;
        UICamera.onCustomInput += OnCustomUICameraInput;
    }

    public void Dispose()
    {
        UICamera.NotifyCallback -= OnUICameraNotify;
        UICamera.onCustomInput -= OnCustomUICameraInput;
    }

    private void OnCustomUICameraInput()
    {
        if (!CheckHotKeyCanTrigger() || !Input.anyKey)
        {
            return;
        }

        ClearKeyCodeDict();

        if (CheckKeyCode(KeyCode.Escape))
        {
            if (CheckModuleClose())
            {
                UpdateHotKeyTrigger();
                return;
            }
        }


        // 强制引导中，不触发快捷键
        GameDebuger.TODO(@"if (NewBieGuideManager.Instance.IsForceGuideRunning())
        {
            return;
        }");

        // 仅当游戏模式下触发
        if (LayerManager.Instance.CurUIMode == UIMode.GAME)
        {
            if (null != HotKeyManager.OpenUIModuleList)
            {  
                for (int i = 0; i < HotKeyManager.OpenUIModuleList.Length; i++)
                {
                    var openModule = HotKeyManager.OpenUIModuleList[i];
                    if (openModule.HotKeyList != null && !string.IsNullOrEmpty(openModule.ModuleName))
                    {
                        var trigger = true;
                        for (int j = 0; j < openModule.HotKeyList.Length; j++)
                        {
                            var keyCode = openModule.HotKeyList[j];
                            if (!CheckKeyCode(keyCode))
                            {
                                trigger = false;
                                break;
                            }
                        }
                        if (trigger)
                        {
                            openModule.Trigger();
                            UpdateHotKeyTrigger();
                            return;
                        }
                    }
                }
            }
        }
        else if (LayerManager.Instance.CurUIMode == UIMode.BATTLE && HotKeyManager.BattleActionList != null)
        {
            for (int i = 0; i < HotKeyManager.BattleActionList.Length; i++)
            {
                var action = HotKeyManager.BattleActionList[i];
                if (action.HotKeyList != null)
                {
                    var trigger = true;
                    for (int j = 0; j < action.HotKeyList.Length; j++)
                    {
                        var keyCode = action.HotKeyList[j];
                        if (!CheckKeyCode(keyCode))
                        {
                            trigger = false;
                            break;
                        }
                    }
                    if (trigger)
                    {
                        action.Trigger();
                        UpdateHotKeyTrigger();
                        return;
                    }
                }
            }
        }
    }

    private void UpdateHotKeyTrigger()
    {
        _hotKeyTriggerLastTime = Time.realtimeSinceStartup;
        ClearKeyCodeDict();
    }

    private bool CheckHotKeyCanTrigger()
    {
        return Time.realtimeSinceStartup - _hotKeyTriggerLastTime > HotKeyInterval;
    }

    private class KeyCodeState
    {
        public bool HasCheck;
        public bool BtnDownState;
    }

    private Dictionary<KeyCode, KeyCodeState> _keyCodeDict = new Dictionary<KeyCode, KeyCodeState>(new KeyCodeEqualityComparer());

    private void ClearKeyCodeDict()
    {
        foreach (var keyCodeState in _keyCodeDict)
        {
            keyCodeState.Value.HasCheck = false;
        }
    }

    private bool CheckKeyCode(KeyCode code)
    {
        if (!_keyCodeDict.ContainsKey(code))
        {
            _keyCodeDict[code] = new KeyCodeState();
        }

        if (!_keyCodeDict[code].HasCheck)
        {
            _keyCodeDict[code].HasCheck = true;
            _keyCodeDict[code].BtnDownState = Input.GetKey(code);
        }

        // 左Alt特殊处理
        if (code == KeyCode.LeftAlt)
        {
            return _keyCodeDict[code].BtnDownState || CheckKeyCode(KeyCode.RightAlt);
        }
        else
        {
            return _keyCodeDict[code].BtnDownState;
        }
    }

    private void OnUICameraNotify(GameObject go, string funcName, object obj)
    {
        if (funcName == "OnClick" && OnUICameraClick(go, funcName, obj))
        {
            return;
        }

        // 屏蔽右键
        if (UICamera.currentKey == KeyCode.Mouse1)
        {
            return;
        }

        UICamera.NativeNotify(go, funcName, obj);
    }


    private bool OnUICameraClick(GameObject go, string funcName, object obj)
    {
        if (UICamera.currentKey == KeyCode.Mouse1)
        {
            if (CheckModuleClose())
            {
                return true;
            }

            GameDebuger.TODO(@"if (BattleController.CheckRightMouseClick())
            {
                return true;
            }");
        }

        return false;
    }


    /// <summary>
    /// 检查是否符合关闭界面的要求
    /// </summary>
    /// <returns></returns>
    private bool CheckModuleClose(bool close = true)
    {
        UICamera.Raycast(_tempTouch);
        if (_tempTouch.current != null)
        {
            var objName = _tempTouch.current.name;
            for (int i = 0; i < CloseBgList.Length; i++)
            {
                if (CloseBgList[i] == objName)
                {
                    if (close)
                    {
                        UICamera.NativeNotify(_tempTouch.current, "OnClick", null);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private class KeyCodeEqualityComparer : IEqualityComparer<KeyCode>
    {
        public bool Equals(KeyCode x, KeyCode y)
        {
            return x == y;
        }

        public int GetHashCode(KeyCode obj)
        {
            return obj.GetHashCode();
        }
    }
}

#endif