// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 07/11/2017 17:08:44
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;
namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeMoneyInfoViewLogic = new StaticDispose.StaticDelegateRunner(
            () =>
            {
                MoneyInfoViewLogic._disposableUIEvt = UIModuleManager.Instance.TopModuleEvt.Subscribe(moduleName =>
                    {
                        // 没有一级界面定义 暂时用这个 fish
                        if (moduleName != MoneyInfoView.NAME
                            && (UIModuleManager.Instance.IsDefaultType(moduleName)
                                || (UIModuleManager.Instance.IsBaseType(moduleName))))
                        {
                            var l = DataCache.getArrayByCls<FunctionFrameState>();
                            var viewCfg = l.Find<FunctionFrameState>(c => string.Equals(c.name, moduleName));
                            if (viewCfg != null && !viewCfg.virtualItenEnumIds.IsNullOrEmpty())
                            {
                                MoneyInfoViewLogic.Open(viewCfg.virtualItenEnumIds);
                            }
                            else
                            {
                                UIModuleManager.Instance.CloseModule(MoneyInfoView.NAME);
                            }
                        }
                    }
                );
            });
    }
}

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMoneyInfoViewLogic = new StaticDelegateRunner(()=>{
            if (MoneyInfoViewLogic._disposableUIEvt != null)
            {
                MoneyInfoViewLogic._disposableUIEvt.Dispose();
                MoneyInfoViewLogic._disposableUIEvt = null;
            }
            
            MoneyInfoViewLogic.Dispose();
        });
    }
}

public static partial class MoneyInfoViewLogic
{
    private static CompositeDisposable _disposable;
    public static IDisposable _disposableUIEvt;
    public static IEnumerable<VirtualItemEnum> _virtualItemIDs;
    public static void Open(IEnumerable<int> virtualItemIDs)
    {
        _virtualItemIDs = virtualItemIDs.Map(s => (VirtualItemEnum) s);
        // open的参数根据需求自己调整
        var layer = UILayerType.FiveModule;
        var ctrl = MoneyInfoViewController.Show<MoneyInfoViewController>(
            MoneyInfoView.NAME
            , layer
            , false
            , false
            , PlayerModel.Stream);

        InitReactiveEvents(ctrl);
    }

    private static void InitReactiveEvents(IMoneyInfoViewController ctrl)
    {
        if (ctrl == null) return;
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
    }

    public static void Dispose()
    {
        _disposable = _disposable.CloseOnceNull();
        OnDispose();    
    }

    // 如果有自定义的内容需要清理，在此实现
    private static void OnDispose()
    {

    }
}


